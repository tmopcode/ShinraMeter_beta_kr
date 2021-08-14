﻿namespace Tera.Game.Messages
{
    public class SPartyMemberAbnormalAdd : ParsedMessage
    {
        internal SPartyMemberAbnormalAdd(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            AbnormalityId = reader.ReadInt32();
            Duration = reader.ReadInt64();
            Stack = reader.ReadInt32();
            //  Debug.WriteLine("target = " + TargetId + ";Abnormality:" + AbnormalityId + ";Duration:" + Duration +
            //                  ";Stack:" + Stack);
        }

        public uint ServerId { get; }
        public uint PlayerId { get; }

        public int AbnormalityId { get; }

        public long Duration { get; }
        public int Stack { get; }
    }
}