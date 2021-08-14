﻿using System;
using System.Collections.Generic;
using DamageMeter.Database.Structures;
using Tera.Game;
using Tera.Game.Abnormality;
using Skill = DamageMeter.Database.Structures.Skill;

namespace DamageMeter.TeraDpsApi
{
    internal class ExtendedStats
    {
        public Skills AllSkills;
        public AbnormalityStorage Abnormals;
        public Dictionary<HotDot, AbnormalityDuration> Debuffs = new Dictionary<HotDot, AbnormalityDuration>();
        public SortedDictionary<string, PlayerAbnormals> PlayerBuffs = new SortedDictionary<string, PlayerAbnormals>();
        public Dictionary<string, double> PlayerCritDamageRate = new Dictionary<string, double>();

        public Dictionary<string, Tuple<int, long>> PlayerReceived = new Dictionary<string, Tuple<int, long>>();
        public SortedDictionary<string, List<Skill>> PlayerSkills = new SortedDictionary<string, List<Skill>>();

        public Dictionary<string, IEnumerable<SkillAggregate>> PlayerSkillsAggregated = new Dictionary<string, IEnumerable<SkillAggregate>>();

        public EncounterBase BaseStats { get; set; }
        public NpcEntity Entity { get; set; }
        public long FirstTick { get; set; }
        public long LastTick { get; set; }
    }
}