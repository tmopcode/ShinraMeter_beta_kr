﻿namespace Tera.Game.Messages
{
    public class S_BOSS_GAGE_INFO : ParsedMessage
    {
        internal S_BOSS_GAGE_INFO(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadEntityId();
            HuntingZoneId = reader.ReadUInt32();
            TemplateId = reader.ReadUInt32();
            TargetId = reader.ReadEntityId();
            Unk1 = reader.ReadInt32();
            if (reader.Factory.ReleaseVersion < 6200) {
                HpChange = reader.ReadSingle();
                Unk2 = reader.ReadByte(); //enrage?
                HpRemaining = (long)reader.ReadSingle();
                TotalHp = (long)reader.ReadSingle();
            }
            else {
                Unk2 = reader.ReadByte(); //enrage?
                HpRemaining = reader.ReadInt64();
                TotalHp = reader.ReadInt64();
            }
        }

        public byte Unk2 { get; }
        public int Unk1 { get; }
        public uint HuntingZoneId { get; }
        public uint TemplateId { get; }
        public float HpChange { get; }
        public long HpRemaining { get; }
        public long TotalHp { get; }
        public EntityId TargetId { get; }
        public EntityId EntityId { get; }
    }
}