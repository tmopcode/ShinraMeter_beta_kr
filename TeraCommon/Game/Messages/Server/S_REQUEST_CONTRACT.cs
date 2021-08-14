﻿using System.Diagnostics;
namespace Tera.Game.Messages
{
    public class S_REQUEST_CONTRACT : ParsedMessage
    {
        internal S_REQUEST_CONTRACT(TeraMessageReader reader) : base(reader)
        {
            //PrintRaw();
            var senderOffset = reader.ReadUInt16();
            var recipientOffset = reader.ReadUInt16();
            reader.Skip(20);
            short type = reader.ReadInt16();
            Type = (RequestType)type;
            reader.Skip(14);
            //int unk3 = reader.ReadInt32();
            //int time = reader.ReadInt32();
            reader.BaseStream.Position = senderOffset - 4;
            Sender = reader.ReadTeraString();
            reader.BaseStream.Position = recipientOffset - 4;
            Recipient = reader.ReadTeraString();
            Debug.WriteLine("type:"+type+";translated:"+Type+"; Sender:"+Sender+";Recipient"+Recipient);
        }

        public string Sender { get; private set; }
        public string Recipient { get; private set; }
        public enum RequestType
        {
            TradeRequest = 3,
            PartyInvite = 4,
            Mailbox = 8,
            ShopOpen = 9,
            Duel = 11,
            MapTeleporter = 14,
            DungeonTeleporter = 15,
            UnStuck = 16,
            ContinentMapTeleporter = 17,
            DeathmatchInvite = 18,
            MedalShop = 20, //(aka: goldfinger + elion token + ...)
            DethmatchBet = 25,
            BankOpen = 26,
            LearnSkillShop = 27,
            Craft = 31,
            Personalize = 32,
            MergeItems = 33,
            Enchant = 34,
            WaitBuyerResponse = 35,
            NegotiateWithBuyer = 36,
            InventoryExpansion = 37,
            EggIncubator = 39,
            RemodelGear = 40,
            RestoreGearAppearance = 41,
            Dye = 42,
            OpenBox = 43,
            VanguardShop = 48,
            BattlegroundShop = 49,
            GearRolls = 51,
            LootBox = 52,
            TeraClubMapTeleporter = 53,
            TeraClubTravelJournalTeleporter = 54,
            FriendSummon = 56,
            ShapeChange = 61,
            OldTeraStore = 62,
            Bamarama = 66,
            CrystalFusion = 69,
            Awakening = 70,
            TeleporterToNearestCity = 71,
            LiberateItem = 74,
            Dressroom = 76,
            EP = 77,
            AceDungeonShop = 79,
            EmporiumShop = 80,
            GuildGold = 83,
            ConvertItem = 84,
            ActivateGearDualSet = 85,
            DualGearSetSwitch = 86,
            Enchant1 = 87, // ?? which one
            Upgrade = 88,
            Dismantle = 89,
            ReastyleAccessories = 90,
            NewHairUI = 91,
            FishShop = 92
        }

        public RequestType Type { get; private set; }
    }
}

