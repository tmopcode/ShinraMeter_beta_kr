
﻿using System;

namespace Tera.Game.Messages
{
    public class S_BEGIN_THROUGH_ARBITER_CONTRACT : ParsedMessage
    {
        internal S_BEGIN_THROUGH_ARBITER_CONTRACT(TeraMessageReader reader)
            : base(reader)
        {
            reader.Skip(18);
            //reader.Skip(8);
            //var type = reader.ReadByte();
            //reader.Skip(9);
            InviteName = reader.ReadTeraString();
            try
            {
                PlayerName = reader.ReadTeraString();
            }
            catch
            {
                PlayerName = "Error parsing S_BEGIN_THROUGH_ARBITER_CONTRACT: " + BitConverter.ToString(Raw);
            }

            //Debug.WriteLine("InviteName:" + InviteName + " PlayerName:" + PlayerName);
        }

        public string InviteName { get; set; }
        public string PlayerName { get; set; }
    }
}
