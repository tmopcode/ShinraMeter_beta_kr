﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using Data;
using Tera.Game;
using Tera.Game.Abnormality;
using Tera.Game.Messages;

namespace DamageMeter
{
    public class HudManager : DependencyObject
    {
        private static HudManager _instance;
        public static bool Logged;

        private SynchronizedObservableCollection<Boss> _bosses = new SynchronizedObservableCollection<Boss>();

        public static HudManager Instance => _instance ?? (_instance = new HudManager());

        public SynchronizedObservableCollection<Boss> CurrentBosses
        {
            get => _bosses;
            set
            {
                if (_bosses == value) { return; }
                _bosses = value;
            }
        }

        public void AddOrUpdateBoss(S_BOSS_GAGE_INFO message)
        {
            var boss = _bosses.ToSyncArray().FirstOrDefault(x => x.EntityId == message.EntityId);
            if (boss == null && message.HpRemaining > 0)
            {
                var bossEntity = PacketProcessor.Instance.EntityTracker.GetOrNull(message.EntityId) as NpcEntity;
                if (bossEntity == null) { return; }
                boss = new Boss(bossEntity, Visibility.Visible);
                _bosses.Add(boss);
                PacketProcessor.Instance.AbnormalityTracker.BuffList(bossEntity.Id).ForEach(x=>boss.AddOrRefresh(x));
            }
            else if (boss == null) return;
            else if (message.HpRemaining <= 0)
            {
                _bosses.Remove(boss);
                boss.Dispose();
                return;
            }
            boss.MaxHP = message.TotalHp;
            boss.CurrentHP = message.HpRemaining;
        }


        public void UpdateBoss(SCreatureChangeHp hpChange)
        {   
            var boss = _bosses.ToSyncArray().FirstOrDefault(x => x.EntityId == hpChange.TargetId);
            if (boss == null){return;}
            if (hpChange.TotalHp != boss.MaxHP) {boss.MaxHP = hpChange.TotalHp;}
            if (hpChange.HpRemaining > 0) {boss.CurrentHP = hpChange.HpRemaining;}
            else {_bosses.Remove(boss); boss.Dispose();}

        }

        public void AddBoss(NpcEntity entity)
        {
            var boss = _bosses.ToSyncArray().FirstOrDefault(x => x.EntityId == entity.Id);
            if (boss == null)
            {
                if (entity == null) { return; }
                boss = new Boss(entity, Visibility.Visible);
                _bosses.Add(boss);
                PacketProcessor.Instance.AbnormalityTracker.BuffList(entity.Id).ForEach(x=>boss.AddOrRefresh(x));
            }
            boss.MaxHP = entity.Info.HP;
        }

        public void RemoveBoss(SDespawnNpc message)
        {
            var boss = _bosses.ToSyncArray().FirstOrDefault(x => x.EntityId == message.Npc);
            if (boss == null) { return; }
            _bosses.Remove(boss);
            boss.Dispose();
        }

        public void UpdateRunemarks(S_WEAK_POINT message)
        {
            var boss = _bosses.ToSyncArray().FirstOrDefault(x => x.EntityId == message.Target);
            if (boss == null) { return; }
            boss.Runmarks = (int) message.RunemarksAfter;
        }
    }

    public class BuffDuration : TSPropertyChanged, IDisposable
    {
        private readonly Timer _timer = new Timer(1000);

        private long _duration;
        private long _durationLeft;

        private int _stacks;

        public BuffDuration(HotDot b, long d, int s)
        {
            Buff = b;
            Duration = d;
            Stacks = s;
            DurationLeft = d;
            _timer.Elapsed += (se, ev) =>
            {
                if (DurationLeft <= 1000)
                {
                    DurationLeft=0;
                    _timer.Stop(); 
                }
                else {DurationLeft = DurationLeft - 1000;}
            };
            if (b.Time != 0) { _timer.Start(); }
        }

        public HotDot Buff { get; }

        public long DurationLeft
        {
            get => _durationLeft;
            set
            {
                if (value == _durationLeft) { return; }
                _durationLeft = value;
                NotifyPropertyChanged();
            }
        }

        public long Duration
        {
            get => _duration;
            set
            {
                if (value == _duration) { return; }
                _duration = value;
                NotifyPropertyChanged();
            }
        }

        public int Stacks
        {
            get => _stacks;
            set
            {
                if (value == _stacks) { return; }
                _stacks = value;
                NotifyPropertyChanged();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public void Refresh()
        {
            _timer.Stop();
            if (Buff.Time != 0) { _timer.Start(); }
            NotifyPropertyChanged();
        }
    }

    public class Boss : TSPropertyChanged, IDisposable
    {
        private SynchronizedObservableCollection<BuffDuration> _buffs;
        private long _currentHp;

        private bool _enraged;

        private long _maxHp;
        private string _name;
        private int _runemarks;

        private Visibility visible;

        public Boss(EntityId eId, long maxHP, long curHP, string name, Visibility visible)
        {
            EntityId = eId;
            Name = name;
            MaxHP = maxHP;
            CurrentHP = curHP;
            Buffs = new SynchronizedObservableCollection<BuffDuration>();
            Visible = visible;
        }

        public Boss(NpcEntity entity, Visibility visible)
        {
            Entity = entity;
            EntityId = Entity.Id;
            Name = Entity.Info.Name;
            MaxHP = Entity.Info.HP;
            CurrentHP = MaxHP;
            Buffs = new SynchronizedObservableCollection<BuffDuration>();
            Visible = visible;
        }

        public EntityId EntityId { get; }
        public NpcEntity Entity { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) { return; }
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public SynchronizedObservableCollection<BuffDuration> Buffs
        {
            get => _buffs;
            set
            {
                if (_buffs == value) { return; }
                _buffs = value;
                NotifyPropertyChanged();
            }
        }

        public bool Enraged
        {
            get => _enraged;
            set
            {
                if (_enraged == value) { return; }
                _enraged = value;
                NotifyPropertyChanged();
            }
        }

        public long MaxHP
        {
            get => _maxHp;
            set
            {
                if (_maxHp == value) { return; }
                _maxHp = value;
                NotifyPropertyChanged();
            }
        }

        public long CurrentHP
        {
            get => _currentHp;
            set
            {
                if (_currentHp == value) { return; }
                _currentHp = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentPercentage));
            }
        }

        public float CurrentPercentage => _maxHp == 0 ? 0 : (float)_currentHp / _maxHp;

        public Visibility Visible
        {
            get => visible;
            set
            {
                if (visible == value) { return; }
                visible = value;
                NotifyPropertyChanged();
            }
        }

        public int Runmarks
        {
            get => _runemarks;
            set
            {
                if (_runemarks == value) { return; }
                _runemarks = value;
                NotifyPropertyChanged();
            }
        }

        public void Dispose()
        {
            _buffs.DisposeAll();
        }

        public void AddOrRefresh(Abnormality abnormality)
        {
            if (abnormality.HotDot.Id == (int) HotDotDatabase.StaticallyUsedBuff.Enraged)
            {
                Enraged = true;
                return;
            }
            if (BasicTeraData.Instance.WindowData.NoAbnormalsInHUD) return;
            var existing = Buffs.ToSyncArray().FirstOrDefault(x => x.Buff.Id == abnormality.HotDot.Id);
            if (existing == null)
            {
                Buffs.Add(new BuffDuration(abnormality.HotDot, abnormality.Duration, abnormality.Stack));
                return;
            }
            existing.Duration = abnormality.Duration;
            existing.DurationLeft = (long) Math.Round((double)abnormality.TimeBeforeEnd / TimeSpan.TicksPerMillisecond);
            existing.Stacks = abnormality.Stack;
            existing.Refresh();
        }

        public void EndBuff(int id)
        {
            if (id == (int) HotDotDatabase.StaticallyUsedBuff.Enraged)
            {
                Enraged = false;
                return;
            }
            try
            {
                var buff = Buffs.ToSyncArray().FirstOrDefault(x => x.Buff.Id == id);
                if (buff == null) { return; }
                Buffs.Remove(buff);
                buff.Dispose();
            }
            catch (Exception) { Debug.WriteLine("Cannot remove {0}", id); }
        }

        public override string ToString()
        {
            return $"{EntityId} - {Name}";
        }
    }
}