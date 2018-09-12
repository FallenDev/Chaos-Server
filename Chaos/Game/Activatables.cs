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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    #pragma warning disable IDE0022
    internal class Activatables
    {
        private Server Server { get; set; }
        private World World { get; set; }

        internal Activatables(Server server, World world)
        {
            Server = server;
            World = world;
        }

        /// <summary>
        /// Applies death to a user, stripping them of all buffs and teleporting them to the death location.
        /// </summary>
        /// <param name="user">The user to apply death to.</param>
        internal void KillUser(User user)
        {
            user.EffectsBar.Clear();


            //disable casting
            //death stuff

            user.AddFlag(UserState.DeathDisplayed);
            WarpObj(user, new Warp(user.Location, CONSTANTS.DEATH_LOCATION));
        }

        /// <summary>
        /// Applies death to a monster, removing them from the world, distributing exp, and rolling for loot.
        /// </summary>
        /// <param name="monster">The monster to apply death to.</param>
        internal void KillMonster(Monster monster)
        {
            //distribute exp
            //other things?

            //remove from screen
            monster.Map.RemoveObject(monster);
        }

        /// <summary>
        /// Revives a user, restoring their hp, and mp.
        /// </summary>
        /// <param name="user">The user to revive.</param>
        internal void ReviveUser(User user)
        {
            if (!user.IsAlive)
            {
                user.Attributes.CurrentHP = user.Attributes.MaximumHP;
                user.Attributes.CurrentMP = user.Attributes.MaximumMP;
                user.RemoveFlag(UserState.DeathDisplayed);
                user.Client.Refresh(true);
            }
        }

        /// <summary>
        /// Retreives targets based on the target type of the effect.
        /// </summary>
        /// <param name="client">The client source of the effect.</param>
        /// <param name="targetPoint">The target point of the effect.</param>
        /// <param name="type">The target type to base the return on.</param>
        internal List<Creature> GetTargetsFromType(Client client, Point targetPoint, TargetsType type = TargetsType.None)
        {
            var creatures = new List<Creature>();

            switch (type)
            {
                //generally skill types
                case TargetsType.None:
                    break;
                case TargetsType.Self:
                    creatures.Add(client.User);
                    break;
                case TargetsType.Front:
                    if(client.User.Map.TryGet(client.User.Point.NewOffset(client.User.Direction), out Creature creature))
                        creatures.Add(creature);
                    break;
                case TargetsType.Surround:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(client.User, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cleave:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(client.User, false, 2).OfType<Creature>().Where(c =>
                        (c.Point.Distance(client.User.Point) == 1 && c.Point.Relation(client.User.Point) != client.User.Direction.Reverse()) || 
                        Utility.GetDiagonalPoints(client.User.Point, 1, client.User.Direction).Contains(c.Point)));
                    break;
                case TargetsType.StraightProjectile:
                    int distance = 13;
                    List<Point> line = Utility.GetLinePoints(client.User.Point, 13, client.User.Direction);
                    creature = null;

                    foreach (Creature c in client.User.Map.ObjectsVisibleFrom(client.User))
                    {
                        if (line.Contains(c.Point))
                        {
                            int dist = c.Point.Distance(client.User.Point);

                            if (dist < distance)
                            {
                                distance = dist;
                                creature = c;
                            }
                        }
                    }

                    if (creature != null)
                        creatures.Add(creature);
                    break;




                //generally spell types
                case TargetsType.Cluster1:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cluster2:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 2).OfType<Creature>());
                    break;
                case TargetsType.Cluster3:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 3).OfType<Creature>());
                    break;
                case TargetsType.Screen:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false).OfType<Creature>());
                    break;
            }

            return creatures.Where(c => c.IsAlive).ToList();
        }

        /// <summary>
        /// Retreives points based on the target type of the effect.
        /// </summary>
        /// <param name="client">The client source of the effect.</param>
        /// <param name="targetPoint">The target point of the effect.</param>
        /// <param name="type">The target type to base the return on.</param>
        internal List<Point> GetPointsFromType(Client client, Point targetPoint, TargetsType type = TargetsType.None)
        {
            var points = new List<Point>();

            switch (type)
            {
                //generally self cast types
                case TargetsType.None:
                    break;
                case TargetsType.Self:
                    points.Add(client.User.Point);
                    break;
                case TargetsType.Front:
                    points.Add(client.User.Point.NewOffset(client.User.Direction));
                    break;
                case TargetsType.Surround:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(client.User.Point) == 1));
                    break;
                case TargetsType.Cleave:
                    points.AddRange(Utility.GetDiagonalPoints(client.User.Point, 1, client.User.Direction));
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(client.User.Point) == 1 && p.Relation(client.User.Point) != client.User.Direction.Reverse()));
                    break;
                case TargetsType.StraightProjectile:
                    int distance = 13;
                    List<Point> line = Utility.GetLinePoints(client.User.Point, 13, client.User.Direction);
                    Creature creature = null;

                    foreach (Creature c in client.User.Map.ObjectsVisibleFrom(client.User))
                    {
                        if (line.Contains(c.Point))
                        {
                            int dist = c.Point.Distance(client.User.Point);

                            if (dist < distance)
                            {
                                distance = dist;
                                creature = c;
                            }
                        }
                    }

                    if (creature != null)
                        points.Add(creature.Point);
                    break;




                //generally spell types
                case TargetsType.Cluster1:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 1));
                    break;
                case TargetsType.Cluster2:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 2));
                    break;
                case TargetsType.Cluster3:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 3));
                    break;
                case TargetsType.Screen:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 13));
                    break;
            }

            return points;
        }

        /// <summary>
        /// Updates the targets and nearby users of each target to reflect the current state of each changed object.
        /// </summary>
        /// <param name="client">The client source of the changes.</param>
        /// <param name="obj">The object effect causing the change.</param>
        /// <param name="targets">The targets of the effect.</param>
        /// <param name="sfx">Special effects, generally point based animations.</param>
        /// <param name="updateType">Stat upate to send to the targets. No need to use vitality.</param>
        /// <param name="displayHealth">Whether or not to display health for the targets.</param>
        /// <param name="refreshClient">Whether or not to refresh the source of change.</param>
        /// <param name="refreshTargets">Whether or not to refresh the targets.</param>
        internal void ApplyActivation(Client client, PanelObject obj, List<Creature> targets, List<Animation> sfx, StatUpdateType updateType, bool displayHealth = false, bool refreshClient = false, bool refreshTargets = false)
        {
            if (targets.Count == 0)
                return;

            lock (obj)
            {
                obj.LastUse = DateTime.UtcNow;
                //grab all nearby Users
                var nearbyUsers = client.User.Map.ObjectsVisibleFrom(client.User, true).OfType<User>().ToList();
                //send the skill cooldown to the skill user
                client.Enqueue(ServerPackets.Cooldown(obj));

                //refresh the client if needed
                if (refreshClient)
                    client.Refresh(true);

                //refresh targets if needed
                foreach (User u in targets.OfType<User>())
                {
                    if (refreshTargets)
                        u.Client.Refresh(true);
                    else if (updateType != StatUpdateType.None)
                        u.Client.SendAttributes(updateType);

                }

                //send all nearby clients the bodyanimation
                if (obj.BodyAnimation != BodyAnimation.None)
                    foreach (User u in nearbyUsers)
                        u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, obj.BodyAnimation, 25));

                //for each target
                foreach (Creature c in targets)
                {
                    //get all users they can see including self
                    var usersNearC = client.User.Map.ObjectsVisibleFrom(c, true).OfType<User>().ToList();

                    //if animation should be displayed
                    if (obj.Animation != Animation.None)
                    {
                        //create new animation for this target
                        Animation newAnimation = obj.Animation.GetTargetedAnimation(c.Id, client.User.Id);
                        c.AnimationHistory[newAnimation.GetHashCode()] = DateTime.UtcNow;
                        //send this animation to all visible users
                        foreach (User u in usersNearC)
                        {
                            u.Client.SendAnimation(newAnimation);
                        }
                    }

                    //if health should be displayed
                    if (displayHealth)
                    {
                        //send all visible users the target's healthbar
                        foreach (User u in usersNearC)
                            u.Client.Enqueue(ServerPackets.HealthBar(c));
                    }
                }

                //if there are any additional special effects with the spell, display them to all users who should be able to see them
                if (sfx != null)
                    foreach (Animation ani in sfx)
                        foreach (User u in client.User.Map.ObjectsVisibleFrom(ani.TargetPoint, true).OfType<User>())
                            u.Client.SendAnimation(ani);
            }
            return;
        }

        /// <summary>
        /// Applies an amount of damage to a creature. Calculates defenses, stats, and other things. Changes the creature's alive state if necessary.
        /// </summary>
        /// <param name="obj">The creature to apply the damage to.</param>
        /// <param name="amount">The flat amount of damage to apply.</param>
        /// <param name="ignoreDefense">Whether to ignore defenses or not.</param>
        /// <param name="mana">Whather the damage dealt is to mana or not.</param>
        internal void ApplyDamage(Creature obj, int amount, bool ignoreDefense = false, bool mana = false)
        {
            User user = null;

            if ((user = obj as User) == null && mana)
                return;

            lock (obj)
            {
                //ac, damage, other shit
                //damage additions based on stats will be moved here later probably
                if (mana)
                    user.CurrentMP = Utility.Clamp<uint>(user.CurrentMP - amount, 0, (int)user.MaximumMP);
                else
                    obj.CurrentHP = Utility.Clamp<uint>(obj.CurrentHP - amount, 0, (int)obj.MaximumHP);

                if (user != null)
                    user.Client.SendAttributes(StatUpdateType.Vitality);
            }
        }

        /// <summary>
        /// Moves a user from one location to another.
        /// </summary>
        /// <param name="obj">The user to warp from and to a position/map.</param>
        /// <param name="warp">The warp the user is using to warp.</param>
        /// <param name="worldMap">Whether or not they are using a worldMap to warp.</param>
        internal void WarpObj(VisibleObject obj, Warp warp, bool worldMap = false)
        {
            if (!World.Maps.ContainsKey(warp.TargetMapId))
                return;

            Map targetMap = World.Maps[warp.TargetMapId];

            if (warp.Location == obj.Location)
            {
                if ((obj as User)?.IsAdmin != true && targetMap.IsWall(warp.TargetPoint))
                {
                    Point nearestPoint = Point.None;
                    int distance = int.MaxValue;
                    ushort x = Utility.Clamp<ushort>(warp.TargetPoint.X - 25, 0, targetMap.SizeX);
                    int width = Math.Min(x + 50, targetMap.SizeX);
                    ushort y = Utility.Clamp<ushort>(warp.TargetPoint.Y - 25, 0, World.Maps[warp.TargetMapId].SizeY);
                    int height = Math.Min(y + 50, targetMap.SizeY);

                    //search up to 2500 tiles for a non wall
                    for (; x < width; x++)
                        for (; y < height; y++)
                        {
                            Point newPoint = (x, y);
                            if (!targetMap.IsWall(newPoint))
                            {
                                distance = warp.TargetPoint.Distance(newPoint);
                                nearestPoint = newPoint;
                            }
                        }

                    warp = new Warp(warp.Location, (warp.TargetMapId, nearestPoint));
                }

                if (!worldMap)
                    obj.Map.RemoveObject(obj);

                World.Maps[warp.TargetMapId].AddObject(obj, warp.TargetPoint);
            }
        }
    }
}