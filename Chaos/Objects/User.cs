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
using System;
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class User : Creature
    {
        [JsonProperty]
        internal Panel<Skill> SkillBook { get; set; }
        [JsonProperty]
        internal Panel<Spell> SpellBook { get; set; }
        [JsonProperty]
        internal Panel<Item> Inventory { get; set; }
        [JsonProperty]
        internal Panel<Item> Equipment { get; set; }
        [JsonProperty]
        internal IgnoreList IgnoreList { get; set; }
        [JsonProperty]
        internal UserOptions UserOptions { get; set; }
        [JsonProperty]
        internal DisplayData DisplayData { get; set; }
        [JsonProperty]
        internal Attributes Attributes { get; set; }
        [JsonProperty]
        internal Legend Legend { get; set; }
        [JsonProperty]
        internal Personal Personal { get; set; }
        [JsonProperty]
        internal Guild Guild { get; set; }
        [JsonIgnore]
        internal Group Group { get; set; }
        [JsonIgnore]
        internal Client Client { get; set; }
        [JsonProperty]
        internal SocialStatus SocialStatus { get; set; }
        [JsonProperty]
        internal Nation Nation { get; set; }
        [JsonProperty]
        internal BaseClass BaseClass { get; set; }
        [JsonProperty]
        internal AdvClass AdvClass { get; set; }
        [JsonProperty]
        internal bool IsMaster { get; set; }
        [JsonProperty]
        internal string Spouse { get; set; }
        [JsonProperty]
        internal List<string> Titles { get; set; }
        [JsonProperty]
        internal bool IsAdmin { get; set; }
        [JsonProperty]
        internal bool IsAlive { get; set; }
        [JsonProperty]
        internal Gender Gender { get; set; }
        internal bool IsChanting { get; set; }
        internal bool IsGrouped => Group != null;
        internal Exchange Exchange { get; set; }
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal override byte HealthPercent => (byte)Utility.Clamp<uint>((CurrentHP * 100) / MaximumHP, 0, MaximumHP);
        internal override uint MaximumHP { get { return Attributes.MaximumHP; } }
        internal override uint CurrentHP { get { return Attributes.CurrentHP; } set { Attributes.CurrentHP = value; } }

        internal User(Gender gender, string name, Point point, Map map, Direction direction)
            :base(name, 0, CreatureType.User, point, map, direction)
        {
            Gender = gender;
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            IgnoreList = new IgnoreList();
            UserOptions = new UserOptions();
            Attributes = new Attributes();
            Legend = new Legend();
            Group = null;
            Spouse = null;
            BaseClass = BaseClass.Peasant;
            AdvClass = AdvClass.None;
            Nation = Nation.None;
            SocialStatus = SocialStatus.Awake;
            IsMaster = false;
            Spouse = string.Empty;
            Titles = new List<string>();
            IsAdmin = false;
            IsAlive = true;
            IsChanting = false;
            LastClicked = DateTime.MinValue;
        }

        [JsonConstructor]
        internal User(string name, Point point, Map map, Direction direction, Panel<Skill> skillBook, Panel<Spell> spellBook, Panel<Item> inventory, Panel<Item> equipment, IgnoreList ignoreList, UserOptions userOptions, DisplayData displayData, Attributes attributes,
               Legend legend, Personal personal, Guild guild, SocialStatus socialStatus, Nation nation, BaseClass baseClass, AdvClass advClass, bool isMaster, string spouse, List<string> titles, Gender gender, bool isAdmin, bool isAlive)
            : base(name, 0, CreatureType.User, point, map)
        {
            SkillBook = skillBook;
            SpellBook = spellBook;
            Inventory = inventory;
            Equipment = equipment;
            IgnoreList = ignoreList;
            UserOptions = userOptions;
            DisplayData = displayData;
            Attributes = attributes;
            Legend = legend;
            Personal = personal;
            Guild = guild;
            SocialStatus = socialStatus;
            Nation = nation;
            BaseClass = baseClass;
            AdvClass = advClass;
            IsMaster = isMaster;
            Spouse = spouse;
            Titles = titles;
            Gender = gender;
            Client = null;
            Group = null;
            DisplayData.User = this;
            IsAdmin = isAdmin;
            IsAlive = isAlive;
            IsChanting = false;
            LastClicked = DateTime.MinValue;
        }

        internal void Resync(Client client)
        {
            Client = client;
            Client.User = this;
            Map = Game.World.Maps[Map.Id];
        }

        internal void Save() => Client.Server.DataBase.TrySaveUser(this);
    }
}
