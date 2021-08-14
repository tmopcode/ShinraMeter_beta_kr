﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tera.Game.Messages;

namespace Tera.Game
{
    // Creates a ParsedMessage from a Message
    // Contains a mapping from OpCodeNames to message types and knows how to instantiate those
    // Since it works with OpCodeNames not numeric OpCodes, it needs an OpCodeNamer
    public class MessageFactory
    {
        private static readonly Delegate UnknownMessageDelegate = Helpers.Contructor<Func<TeraMessageReader, UnknownMessage>>();
        private static readonly Dictionary<ushort, Delegate> OpcodeNameToType = new Dictionary<ushort, Delegate> {{ 19900, Helpers.Contructor<Func<TeraMessageReader, C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate> CoreServices = new Dictionary<string, Delegate>
        {
            {"C_CHECK_VERSION", Helpers.Contructor<Func<TeraMessageReader,C_CHECK_VERSION>>()},
            {"C_LOGIN_ARBITER", Helpers.Contructor<Func<TeraMessageReader,C_LOGIN_ARBITER>>()},
            {"S_EACH_SKILL_RESULT", Helpers.Contructor<Func<TeraMessageReader,EachSkillResultServerMessage>>()},
            {"S_SPAWN_USER", Helpers.Contructor<Func<TeraMessageReader,SpawnUserServerMessage>>()},
            {"S_SPAWN_ME", Helpers.Contructor<Func<TeraMessageReader,SpawnMeServerMessage>>()},
            {"S_SPAWN_NPC", Helpers.Contructor<Func<TeraMessageReader,SpawnNpcServerMessage>>()},
            {"S_SPAWN_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,SpawnProjectileServerMessage>>()},
            {"S_LOGIN", Helpers.Contructor<Func<TeraMessageReader,LoginServerMessage>>()},
            {"S_GUILD_NAME", Helpers.Contructor<Func<TeraMessageReader,S_GUILD_NAME>>()},
//            {"S_GUILD_INFO", Helpers.Contructor<Func<TeraMessageReader,S_GUILD_INFO>>()},
//            {"S_TARGET_INFO", Helpers.Contructor<Func<TeraMessageReader,STargetInfo>>()},
            {"S_LOAD_TOPO", Helpers.Contructor<Func<TeraMessageReader,S_LOAD_TOPO>>()},
            {"S_START_USER_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,StartUserProjectileServerMessage>>()},
            {"S_CREATURE_CHANGE_HP", Helpers.Contructor<Func<TeraMessageReader,SCreatureChangeHp>>()},
            {"S_BOSS_GAGE_INFO", Helpers.Contructor<Func<TeraMessageReader,S_BOSS_GAGE_INFO>>()},
//            {"S_NPC_TARGET_USER", Helpers.Contructor<Func<TeraMessageReader,SNpcTargetUser>>()},
            {"S_NPC_OCCUPIER_INFO", Helpers.Contructor<Func<TeraMessageReader,SNpcOccupierInfo>>()},
            {"S_ABNORMALITY_BEGIN", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityBegin>>()},
            {"S_ABNORMALITY_END", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityEnd>>()},
            {"S_ABNORMALITY_REFRESH", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityRefresh>>()},
            {"S_DESPAWN_NPC", Helpers.Contructor<Func<TeraMessageReader,SDespawnNpc>>()},
            {"S_PLAYER_CHANGE_MP", Helpers.Contructor<Func<TeraMessageReader,SPlayerChangeMp>>()},
//            {"S_PARTY_MEMBER_ABNORMAL_ADD", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalAdd>>()},
//            {"S_PARTY_MEMBER_CHANGE_MP", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberChangeMp>>()},
            {"S_PARTY_MEMBER_CHANGE_HP", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberChangeHp>>()},
//            {"S_PARTY_MEMBER_ABNORMAL_CLEAR", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalClear>>()},
//            {"S_PARTY_MEMBER_ABNORMAL_DEL", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalDel>>()},
//            {"S_PARTY_MEMBER_ABNORMAL_REFRESH", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalRefresh>>()},
            {"S_DESPAWN_USER", Helpers.Contructor<Func<TeraMessageReader,SDespawnUser>>()},
            {"S_USER_STATUS", Helpers.Contructor<Func<TeraMessageReader,SUserStatus>>()},
            {"S_CREATURE_LIFE", Helpers.Contructor<Func<TeraMessageReader,SCreatureLife>>()},
            {"S_CREATURE_ROTATE", Helpers.Contructor<Func<TeraMessageReader,S_CREATURE_ROTATE>>()},
            {"S_NPC_STATUS", Helpers.Contructor<Func<TeraMessageReader,SNpcStatus>>()},
            {"S_NPC_LOCATION", Helpers.Contructor<Func<TeraMessageReader,SNpcLocation>>()},
            {"S_USER_LOCATION", Helpers.Contructor<Func<TeraMessageReader,S_USER_LOCATION>>()},
            {"C_PLAYER_LOCATION", Helpers.Contructor<Func<TeraMessageReader,C_PLAYER_LOCATION>>()},
            {"S_INSTANT_MOVE", Helpers.Contructor<Func<TeraMessageReader,S_INSTANT_MOVE>>()},
            {"S_INSTANT_DASH", Helpers.Contructor<Func<TeraMessageReader,S_INSTANT_DASH>>()},
            {"S_ACTION_STAGE", Helpers.Contructor<Func<TeraMessageReader,S_ACTION_STAGE>>()},
            {"S_ACTION_END", Helpers.Contructor<Func<TeraMessageReader,S_ACTION_END>>()},
            {"S_CHANGE_DESTPOS_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,S_CHANGE_DESTPOS_PROJECTILE>>()},
            {"S_ADD_CHARM_STATUS", Helpers.Contructor<Func<TeraMessageReader,SAddCharmStatus>>()},
            {"S_ENABLE_CHARM_STATUS", Helpers.Contructor<Func<TeraMessageReader,SEnableCharmStatus>>()},
            {"S_REMOVE_CHARM_STATUS", Helpers.Contructor<Func<TeraMessageReader,SRemoveCharmStatus>>()},
            {"S_RESET_CHARM_STATUS", Helpers.Contructor<Func<TeraMessageReader,SResetCharmStatus>>()},
            {"S_PARTY_MEMBER_CHARM_ADD", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberCharmAdd>>()},
            {"S_PARTY_MEMBER_CHARM_DEL", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberCharmDel>>()},
            {"S_PARTY_MEMBER_CHARM_ENABLE", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberCharmEnable>>()},
            {"S_PARTY_MEMBER_CHARM_RESET", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberCharmReset>>()},
            {"S_PARTY_MEMBER_STAT_UPDATE", Helpers.Contructor<Func<TeraMessageReader,S_PARTY_MEMBER_STAT_UPDATE>>()},
            {"S_PLAYER_STAT_UPDATE", Helpers.Contructor<Func<TeraMessageReader,S_PLAYER_STAT_UPDATE>>()},
            {"S_PARTY_MEMBER_LIST", Helpers.Contructor<Func<TeraMessageReader,S_PARTY_MEMBER_LIST>>()},
            {"S_LEAVE_PARTY_MEMBER", Helpers.Contructor<Func<TeraMessageReader,S_LEAVE_PARTY_MEMBER>>()},
            {"S_BAN_PARTY_MEMBER", Helpers.Contructor<Func<TeraMessageReader,S_BAN_PARTY_MEMBER>>()},
            {"S_LEAVE_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_LEAVE_PARTY>>()},
            {"S_BAN_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_BAN_PARTY>>()},
//            {"S_GET_USER_LIST", Helpers.Contructor<Func<TeraMessageReader,S_GET_USER_LIST>>()},
            {"S_GET_USER_GUILD_LOGO", Helpers.Contructor<Func<TeraMessageReader,S_GET_USER_GUILD_LOGO>>()},
            {"S_MOUNT_VEHICLE_EX", Helpers.Contructor<Func<TeraMessageReader,S_MOUNT_VEHICLE_EX>>() },
            {"S_CREST_INFO", Helpers.Contructor<Func<TeraMessageReader,S_CREST_INFO>>() },
        };

        private static readonly Dictionary<string, Delegate> ChatServices = new Dictionary<string, Delegate>
        {
            {"C_CHAT", Helpers.Contructor<Func<TeraMessageReader,C_CHAT>>() },
            {"C_WHISPER", Helpers.Contructor<Func<TeraMessageReader,C_WHISPER>>()},
            {"S_RETURN_TO_LOBBY", Helpers.Contructor<Func<TeraMessageReader,S_RETURN_TO_LOBBY>>() },
            {"S_UPDATE_NPCGUILD", Helpers.Contructor<Func<TeraMessageReader,S_UPDATE_NPCGUILD>>()},
            {"S_AVAILABLE_EVENT_MATCHING_LIST", Helpers.Contructor<Func<TeraMessageReader,S_AVAILABLE_EVENT_MATCHING_LIST>>()},
            {"S_SYSTEM_MESSAGE", Helpers.Contructor<Func<TeraMessageReader,S_SYSTEM_MESSAGE>>()},
            {"S_START_COOLTIME_SKILL", Helpers.Contructor<Func<TeraMessageReader,S_START_COOLTIME_SKILL>>()},
            {"S_CREST_MESSAGE", Helpers.Contructor<Func<TeraMessageReader,S_CREST_MESSAGE>>()},
            {"S_CHAT", Helpers.Contructor<Func<TeraMessageReader,S_CHAT>>()},
            {"S_WHISPER", Helpers.Contructor<Func<TeraMessageReader,S_WHISPER>>()},
            {"S_TRADE_BROKER_DEAL_SUGGESTED", Helpers.Contructor<Func<TeraMessageReader,S_TRADE_BROKER_DEAL_SUGGESTED>>()},
            {"S_OTHER_USER_APPLY_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_OTHER_USER_APPLY_PARTY>>() },
            {"S_PRIVATE_CHAT", Helpers.Contructor<Func<TeraMessageReader,S_PRIVATE_CHAT>>() },
            {"S_FIN_INTER_PARTY_MATCH", Helpers.Contructor<Func<TeraMessageReader,S_FIN_INTER_PARTY_MATCH>>() },
            {"S_BATTLE_FIELD_ENTRANCE_INFO", Helpers.Contructor<Func<TeraMessageReader,S_BATTLE_FIELD_ENTRANCE_INFO>>() },
            {"S_REQUEST_CONTRACT", Helpers.Contructor<Func<TeraMessageReader,S_REQUEST_CONTRACT>>() },
//            {"S_BEGIN_THROUGH_ARBITER_CONTRACT", Helpers.Contructor<Func<TeraMessageReader,S_BEGIN_THROUGH_ARBITER_CONTRACT>>() },
            {"S_CHECK_TO_READY_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_CHECK_TO_READY_PARTY>>() },
            //{"S_GUILD_QUEST_LIST", Helpers.Contructor<Func<TeraMessageReader,S_GUILD_QUEST_LIST>>() },
            //{"S_START_GUILD_QUEST", Helpers.Contructor<Func<TeraMessageReader, S_START_GUILD_QUEST>>() },
            {"S_WEAK_POINT", Helpers.Contructor<Func<TeraMessageReader, S_WEAK_POINT>>() },
            {"S_VISIT_NEW_SECTION", Helpers.Contructor<Func<TeraMessageReader,S_VISIT_NEW_SECTION>>()},
            {"S_SHOW_PARTY_MATCH_INFO", Helpers.Contructor<Func<TeraMessageReader,S_SHOW_PARTY_MATCH_INFO>>()},
            {"C_REGISTER_PARTY_INFO", Helpers.Contructor<Func<TeraMessageReader,C_REGISTER_PARTY_INFO>>()},
            {"S_CHANGE_EVENT_MATCHING_STATE", Helpers.Contructor<Func<TeraMessageReader,S_CHANGE_EVENT_MATCHING_STATE>>()},
        };

        public static List<string> OpcodesList => CoreServices.Keys.Where(o => !(o.Equals(nameof(C_CHECK_VERSION)) || o.Equals(nameof(C_LOGIN_ARBITER)))).Concat(ChatServices.Keys).ToList();


        private readonly OpCodeNamer _opCodeNamer;
        private readonly OpCodeNamer _sysMsgNamer;
        public string Region;
        public uint Version;
        public int ReleaseVersion = 9901;
        public bool ChatEnabled {
            get { return _chatEnabled; }
            set
            {
                _chatEnabled = value;
                if (OpcodeNameToType.Count==1) return;
                OpcodeNameToType.Clear();
                CoreServices.ToList().ForEach(x => OpcodeNameToType[_opCodeNamer.GetCode(x.Key)] = x.Value);
                if (_chatEnabled) ChatServices.ToList().ForEach(x => OpcodeNameToType[_opCodeNamer.GetCode(x.Key)] = x.Value);
                OpcodeNameToType[0] = UnknownMessageDelegate;
            }
        }

        private bool _chatEnabled;

        public MessageFactory(OpCodeNamer opCodeNamer, string region, uint version, bool chatEnabled=false, OpCodeNamer sysMsgNamer=null)
        {
            _opCodeNamer = opCodeNamer;
            _sysMsgNamer = sysMsgNamer;
            OpcodeNameToType.Clear();
            CoreServices.ToList().ForEach(x=>OpcodeNameToType[_opCodeNamer.GetCode(x.Key)]=x.Value);
            if (chatEnabled) ChatServices.ToList().ForEach(x => OpcodeNameToType[_opCodeNamer.GetCode(x.Key)] = x.Value);
            OpcodeNameToType[0] = UnknownMessageDelegate;
            Version = version;
            Region = region;
            _chatEnabled = chatEnabled;
        }

        public void ReloadSysMsg() { _sysMsgNamer?.Reload(Version, ReleaseVersion); }

        public MessageFactory()
        {
            _opCodeNamer = new OpCodeNamer(new Dictionary<ushort,string>{{19900 , "C_CHECK_VERSION" }} );
            Version = 0;
            Region = "Unknown";
        }

        private ParsedMessage Instantiate(ushort opCode, TeraMessageReader reader)
        {
            Delegate type;
            
            if (!OpcodeNameToType.TryGetValue(opCode, out type))
                type = UnknownMessageDelegate;
            return (ParsedMessage) type.DynamicInvoke(reader);
        }

        public ParsedMessage Create(Message message)
        {
            var reader = new TeraMessageReader(message, _opCodeNamer, this, _sysMsgNamer);
            return Instantiate(message.OpCode, reader);
        }
    }
}
