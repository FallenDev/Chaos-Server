﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

namespace Chaos
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        internal static readonly object SyncWrite = new object();
        internal static int NextClientId = 10000;
        internal static int NextId = 1;
        internal IPAddress LocalIp;
        internal IPEndPoint LocalEndPoint;
        internal int LocalPort;
        internal Socket ServerSocket { get; set; }
        internal ServerPackets Packets { get; }
        internal byte[] Table { get; }
        internal uint TableCheckSum { get; }
        internal byte[] LoginMessage { get; }
        internal uint LoginMessageCheckSum { get; }
        internal ConcurrentDictionary<Socket, Client> Clients { get; }
        internal List<Client> WorldClients => Clients.Values.Where(client => client.ServerType == ServerType.World).ToList();
        internal DataBase DataBase { get; }
        internal GameTime ServerTime => GameTime.Now;
        internal LightLevel LightLevel => ServerTime.TimeOfDay;
        internal List<Redirect> Redirects { get; set; }
        internal static List<string> Admins = new List<string>() { "Sichi", "Jinori", "Vorlof" };

        internal Server(IPAddress ip, int port)
        {
            WriteLog("Initializing server...");

            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            Clients = new ConcurrentDictionary<Socket, Client>();
            Packets = new ServerPackets();
            DataBase = new DataBase(this);
            Redirects = new List<Redirect>();

            byte[] notif = Encoding.GetEncoding(949).GetBytes($@"{{={(char)MessageColor.Orange}Under Construction");
            LoginMessageCheckSum = Crypto.Generate32(notif);

            using (MemoryStream compressor = ZLIB.Compress(notif))
                LoginMessage = compressor.ToArray();

            MemoryStream tableStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(tableStream))
            {
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write(Dns.GetHostEntry(Host.Name).AddressList[1].GetAddressBytes());
                writer.Write((byte)(LocalPort / 256));
                writer.Write((byte)(LocalPort % 256));
                writer.Write(Encoding.GetEncoding(949).GetBytes("Chaos;Under Construction\0"));
                writer.Write(notif);
                writer.Flush();

                TableCheckSum = Crypto.Generate32(tableStream.ToArray());
                using (MemoryStream table = ZLIB.Compress(tableStream.ToArray()))
                    Table = table.ToArray();

            }
        }

        internal void Start()
        {
            Game.Set(this);

            //display dns ip for others to connect to
            WriteLog($"Server IP: {Dns.GetHostAddresses(Host.Name)[1]}");
            WriteLog("Starting the serverloop...");

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(LocalEndPoint);
            ServerSocket.Listen(10);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), ServerSocket);
        }

        internal void EndAccept(IAsyncResult ar)
        {
            //create the user, and add them to the userlist
            Client newClient = new Client(this, ServerSocket.EndAccept(ar));
            WriteLog($@"Incoming connection", newClient);

            if (Clients.TryAdd(newClient.ClientSocket, newClient))
                newClient.Connect();

            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), ServerSocket);
        }

        internal static void WriteLog(string message, Client client = null)
        {
            lock (SyncWrite)
            {
                if (client == null)
                    message = $@"[{DateTime.Now.ToString("HH:mm")}] Server: {message}";
                else
                    message = $@"[{DateTime.Now.ToString("HH:mm")}] {(client.ClientSocket.RemoteEndPoint as IPEndPoint).Address}: {message}";

                Console.WriteLine(message);
                using (StreamWriter writer = File.AppendText($@"{Paths.LogFiles}{DateTime.UtcNow.ToString("MMM dd yyyy")}.log"))
                    writer.Write($@"{message}{Environment.NewLine}");
            }
        }

        internal bool TryGetUser(string name, out User user) => (user = WorldClients.FirstOrDefault(client => client.User?.Name?.Equals(name, StringComparison.CurrentCultureIgnoreCase) == true)?.User) != null;
    }
}
