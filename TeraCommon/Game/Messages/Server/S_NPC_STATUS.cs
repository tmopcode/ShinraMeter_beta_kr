﻿using System.Diagnostics;

namespace Tera.Game.Messages
{
    public class SNpcStatus : ParsedMessage

    {
        internal SNpcStatus(TeraMessageReader reader) : base(reader)
        {
            Npc = reader.ReadEntityId();
            Enraged = (reader.ReadByte() & 1) == 1;
            RemainingEnrageTime = reader.Factory.ReleaseVersion >= 7900 ? reader.ReadInt32() : 36000;
            reader.Skip(4);
            Target = reader.ReadEntityId();
            //Debug.WriteLine("NPC:" + Npc + ";Target:" + Target + (Enraged?" Enraged":""));
        }

        public EntityId Npc { get; }
        public bool Enraged { get; }
        public EntityId Target { get; }
        public int RemainingEnrageTime { get; }
    }
}