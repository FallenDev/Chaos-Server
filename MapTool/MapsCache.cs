// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Collections.Generic;
using System.IO;

namespace ChaosTool
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class MapsCache
    {
        internal MainForm MainForm { get; }
        internal Dictionary<uint, Chaos.WorldMap> WorldMaps { get; set; }
        internal Dictionary<ushort, Chaos.Map> Maps { get; set; }
        private string HashKey => "edl396yhvnw85b6kd8vnsj296hj285bq";
        internal NewtonsoftSerializer Serializer { get; }
        internal StackExchangeRedisCacheClient Cache { get; }
        internal ConnectionMultiplexer DataConnection { get; }

        internal MapsCache(MainForm mainForm)
        {
            //create the serializing cache db
            JsonSerializerSettings jSettings = new JsonSerializerSettings();
            jSettings.TypeNameHandling = TypeNameHandling.All;
            Serializer = new NewtonsoftSerializer(jSettings);
            DataConnection = ConnectionMultiplexer.Connect("localhost:6379");
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            //sets
            Maps = new Dictionary<ushort, Chaos.Map>();
            WorldMaps = new Dictionary<uint, Chaos.WorldMap>();
            MainForm = mainForm;

            if (!Cache.Exists(HashKey))
                Cache.Add(HashKey, new byte[100]);
        }
        internal void Load()
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(Cache.Get<byte[]>(HashKey))))
            {
                reader.ReadInt32();

                //load worldmaps
                ushort worldMapCount = reader.ReadUInt16();
                for (int wMap = 0; wMap < worldMapCount; ++wMap)
                {
                    string field = reader.ReadString();

                    byte nodeCount = reader.ReadByte();
                    Chaos.WorldMapNode[] nodes = new Chaos.WorldMapNode[nodeCount];
                    for (int i = 0; i < nodeCount; i++)
                    {
                        ushort x = reader.ReadUInt16();
                        ushort y = reader.ReadUInt16();
                        string name = reader.ReadString();
                        ushort mapId = reader.ReadUInt16();
                        byte dX = reader.ReadByte();
                        byte dY = reader.ReadByte();
                        nodes[i] = new Chaos.WorldMapNode(new Chaos.Point(x, y), name, mapId, new Chaos.Point(dX, dY));
                    }

                    Chaos.WorldMap worldMap = new Chaos.WorldMap(field, nodes);
                    uint crc32 = worldMap.GetCheckSum();
                    WorldMaps[crc32] = worldMap;
                }

                ushort mapCount = reader.ReadUInt16();
                for (int map = 0; map < mapCount; map++)
                {
                    //load map information
                    ushort mapId = reader.ReadUInt16();
                    byte sizeX = reader.ReadByte();
                    byte sizeY = reader.ReadByte();
                    string name = reader.ReadString();
                    Chaos.MapFlags flags = (Chaos.MapFlags)reader.ReadUInt32();
                    sbyte music = reader.ReadSByte();
                    Chaos.Map newMap = new Chaos.Map(mapId, sizeX, sizeY, flags, name, music);

                    //load doors
                    byte doorCount = reader.ReadByte();
                    for (byte b = 0; b < doorCount; b++)
                    {
                        ushort x = reader.ReadUInt16();
                        ushort y = reader.ReadUInt16();
                        bool opensRight = reader.ReadBoolean();
                        newMap.Doors[new Chaos.Point(x, y)] = new Chaos.Door(mapId, x, y, false, opensRight);
                    }

                    //load warps
                    short warpCount = reader.ReadInt16();
                    for (int i = 0; i < warpCount; i++)
                    {
                        byte sourceX = reader.ReadByte();
                        byte sourceY = reader.ReadByte();
                        ushort targetMapId = reader.ReadUInt16();
                        byte targetX = reader.ReadByte();
                        byte targetY = reader.ReadByte();
                        Chaos.Warp warp = new Chaos.Warp(sourceX, sourceY, targetX, targetY, mapId, targetMapId);
                        newMap.Warps[new Chaos.Point(sourceX, sourceY)] = warp;
                    }

                    //load worldmaps for this map
                    byte wMapCount = reader.ReadByte();
                    for (int i = 0; i < wMapCount; i++)
                    {
                        byte x = reader.ReadByte();
                        byte y = reader.ReadByte();
                        uint CRC = reader.ReadUInt32();
                        if (WorldMaps.ContainsKey(CRC))
                            newMap.WorldMaps[new Chaos.Point(x, y)] = WorldMaps[CRC];
                    }

                    //add the map to the map list
                    Maps.Add(mapId, newMap);
                }
            }
        }


        internal void Save()
        {
            MemoryStream cacheStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(cacheStream))
            {
                writer.Write(1);

                //write world maps
                writer.Write((ushort)WorldMaps.Count);
                foreach (Chaos.WorldMap worldMap in WorldMaps.Values)
                {
                    writer.Write(worldMap.Field);
                    writer.Write((byte)worldMap.Nodes.Length);
                    foreach (Chaos.WorldMapNode worldMapNode in worldMap.Nodes)
                    {
                        writer.Write(worldMapNode.Position.X);
                        writer.Write(worldMapNode.Position.Y);
                        writer.Write(worldMapNode.Name);
                        writer.Write(worldMapNode.MapId);
                        writer.Write((byte)worldMapNode.Point.X);
                        writer.Write((byte)worldMapNode.Point.Y);
                    }
                }

                //write maps
                writer.Write((ushort)Maps.Count);
                foreach (Chaos.Map map in Maps.Values)
                {
                    //write map info
                    writer.Write(map.Id);
                    writer.Write(map.SizeX);
                    writer.Write(map.SizeY);
                    writer.Write(map.Name);
                    writer.Write((uint)map.Flags);
                    writer.Write(map.Music);

                    //write doors
                    writer.Write((byte)map.Doors.Count);
                    foreach (Chaos.Door door in map.Doors.Values)
                    {
                        writer.Write(door.Point.X);
                        writer.Write(door.Point.Y);
                        writer.Write(door.OpenRight);
                    }

                    //write warps
                    writer.Write((ushort)map.Warps.Count);
                    foreach (Chaos.Warp warp in map.Warps.Values)
                    {
                        writer.Write((byte)warp.SourceX);
                        writer.Write((byte)warp.SourceY);
                        writer.Write(warp.TargetMapId);
                        writer.Write((byte)warp.TargetX);
                        writer.Write((byte)warp.TargetY);
                    }

                    //write worldmaps for this map
                    writer.Write((byte)map.WorldMaps.Count);
                    foreach (KeyValuePair<Chaos.Point, Chaos.WorldMap> keyValuePair in map.WorldMaps)
                    {
                        writer.Write((byte)keyValuePair.Key.X);
                        writer.Write((byte)keyValuePair.Key.Y);
                        writer.Write(keyValuePair.Value.GetCheckSum());
                    }
                }
                Cache.Replace(HashKey, cacheStream.ToArray());
            }
        }
    }
}
