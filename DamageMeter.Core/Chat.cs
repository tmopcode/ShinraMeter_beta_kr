﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DamageMeter.Processing;
using Data;
using Lang;
using Tera.Game.Messages;
using static Tera.Game.Messages.S_CHAT;

namespace DamageMeter
{
    public class Chat
    {
        public enum ChatType
        {
            Whisper = 0,
            Normal = 1,
            PrivateChannel = 2
        }

        private static Chat _instance;

        private readonly LinkedList<ChatMessage> _chat = new LinkedList<ChatMessage>();
        private readonly int _maxMessage = 200;


        private Chat() { }

        public static Chat Instance => _instance ?? (_instance = new Chat());

        public void Add(S_CHAT message)
        {
            Add(message.Username, message.Text, ChatType.Normal, message.Channel);
        }

        public void Add(S_PRIVATE_CHAT message)
        {
            Add(message.AuthorName, message.Text, ChatType.PrivateChannel);
        }

        public void Add(S_WHISPER message)
        {
            Add(message.Sender, message.Text, ChatType.Whisper);
        }

        private void Add(string sender, string message, ChatType chatType, ChannelEnum? channel = null)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var rgx = new Regex("<[^>]+>");
            message = rgx.Replace(message, "");
            message = WebUtility.HtmlDecode(message);
            if (string.IsNullOrWhiteSpace(message)) { return; }

            if (_chat.Count == _maxMessage) { _chat.RemoveFirst(); }
            var chatMessage = new ChatMessage(sender, message, chatType, channel, time);
            _chat.AddLast(chatMessage);

            if (PacketProcessor.Instance.EntityTracker?.MeterUser == null) { return; }

            if (chatType == ChatType.Whisper && PacketProcessor.Instance.EntityTracker.MeterUser.Name != sender &&
                (BasicTeraData.Instance.WindowData.ShowAfkEventsIngame || !TeraWindow.IsTeraActive()))
            {
                PacketProcessor.Instance.FlashMessage.Add(NotifyProcessor.Instance.DefaultNotifyAction(LP.Whisper + ": " + sender, message, EventType.Whisper));
            }

            if (chatType != ChatType.Whisper && PacketProcessor.Instance.EntityTracker.MeterUser.Name != sender &&
                (BasicTeraData.Instance.WindowData.ShowAfkEventsIngame || !TeraWindow.IsTeraActive()) &&
                message.Contains("@" + PacketProcessor.Instance.EntityTracker.MeterUser.Name))
            {
                PacketProcessor.Instance.FlashMessage.Add(NotifyProcessor.Instance.DefaultNotifyAction(LP.Chat + ": " + sender, message, EventType.Mention));
            }

            if ((chatType == ChatType.PrivateChannel || chatType == ChatType.Normal &&
                 (channel == ChannelEnum.Group || channel == ChannelEnum.Guild || channel == ChannelEnum.Raid)) &&
                (BasicTeraData.Instance.WindowData.ShowAfkEventsIngame || !TeraWindow.IsTeraActive()) && message.Contains("@@"))
            {
                PacketProcessor.Instance.FlashMessage.Add(NotifyProcessor.Instance.DefaultNotifyAction("Wake up, " + PacketProcessor.Instance.EntityTracker.MeterUser.Name,
                        "Wake up, " + PacketProcessor.Instance.EntityTracker.MeterUser.Name, EventType.WakeUp));
            }

            // Trolling 
            if(sender == "Yukipédia" && PacketProcessor.Instance.Server.Name == "Seren" && message.Contains("@#"))
            {
                PacketProcessor.Instance.FlashMessage.Add(NotifyProcessor.Instance.DefaultNotifyAction("", message.Replace("@#",""), EventType.WakeUp));
            }
        }

        public List<ChatMessage> Get()
        {
            return _chat.ToList();
        }
    }
}