﻿using Data;
using Tera.Game.Messages;

namespace DamageMeter.Processing
{
    internal static class S_USER_STATUS
    {
        internal static void Process(SUserStatus message)
        {
            if (message.User != PacketProcessor.Instance.EntityTracker.MeterUser.Id) { return; }
            
            if (BasicTeraData.Instance.WindowData.IdleResetTimeout <= 0) { return; }
            if (message.Status != 1) { DamageTracker.Instance.LastIdleStartTime = message.Time.Ticks; }
        }
    }
}