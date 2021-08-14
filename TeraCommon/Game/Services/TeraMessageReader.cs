﻿using System.IO;
using System.Text;

namespace Tera.Game
{
    // Used by `ParsedMessage`s to parse themselves
    public class TeraMessageReader : BinaryReader
    {
        public TeraMessageReader(Message message, OpCodeNamer opCodeNamer, MessageFactory factory, OpCodeNamer sysMsgNamer)
            : base(GetStream(message), Encoding.Unicode)
        {
            Message = message;
            OpCodeName = opCodeNamer.GetName(message.OpCode);
            SysMsgNamer = sysMsgNamer;
            Factory = factory;
        }

        public TeraMessageReader(Message message): base(GetStream(message), Encoding.Unicode)
        {
            Message = message;
        }

        public Message Message { get; private set; }
        public string OpCodeName { get; private set; }
        internal MessageFactory Factory { get; private set; }
        internal OpCodeNamer SysMsgNamer { get; private set; }

        private static MemoryStream GetStream(Message message)
        {
            return new MemoryStream(message.Payload.Array, message.Payload.Offset, message.Payload.Count, false, true);
        }

        public EntityId ReadEntityId()
        {
            var id = ReadUInt64();
            return new EntityId(id);
        }

        public Vector3f ReadVector3f()
        {
            Vector3f result;
            result.X = ReadSingle();
            result.Y = ReadSingle();
            result.Z = ReadSingle();
            return result;
        }

        public Angle ReadAngle()
        {
            return new Angle(ReadInt16());
        }

        public void Skip(int count)
        {
            ReadBytes(count);
        }

        // Tera uses null terminated litte endian UTF-16 strings
        public string ReadTeraString()
        {
            var builder = new StringBuilder();
            try
            {
                while (true)
                {
                    var c = ReadChar();
                    if (c == 0) return builder.ToString();
                    builder.Append(c);
                }
            }
            catch { return builder.ToString(); } // don't crash on parsing strings from corrupted packets
        }
    }
}