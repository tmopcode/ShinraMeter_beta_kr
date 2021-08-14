﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Data;
using Lang;
using Tera.Game;
using Tera.Game.Abnormality;

namespace DamageMeter.UI.EntityStats
{
    /// <summary>
    ///     Logique d'interaction pour EnduranceDebuff.xaml
    /// </summary>
    public partial class EnduranceDebuff
    {
        public EnduranceDebuff()
        {
            InitializeComponent();
        }

        public void Update(HotDot hotdot, AbnormalityDuration abnormalityDuration, long firstHit, long lastHit)
        {
            SkillIcon.ImageSource = BasicTeraData.Instance.Icons.GetImage(hotdot.IconName);
            SkillIconWrapper.ToolTip = string.IsNullOrEmpty(hotdot.ItemName) ? null : hotdot.ItemName;
            LabelClass.Content = LP.ResourceManager.GetString(abnormalityDuration.InitialPlayerClass.ToString(), LP.Culture);
            var intervalEntity = lastHit - firstHit;
            var ticks = abnormalityDuration.Duration(firstHit, lastHit);
            var interval = TimeSpan.FromTicks(ticks);
            LabelAbnormalityDuration.Content = interval.ToString(@"mm\:ss");

            if (intervalEntity == 0) { LabelAbnormalityDurationPercentage.Content = "0%"; }
            else { LabelAbnormalityDurationPercentage.Content = abnormalityDuration.Duration(firstHit, lastHit) * 100 / intervalEntity + "%"; }
            interval = TimeSpan.FromTicks(intervalEntity);
            LabelInterval.Content = interval.ToString(@"mm\:ss");

            LabelName.Content = hotdot.Name;
            LabelName.ToolTip = string.IsNullOrEmpty(hotdot.Tooltip) ? null : hotdot.Tooltip;
            LabelAbnormalityDurationPercentage.ToolTip = hotdot.Id;
            var count = 0;
            foreach (var stack in abnormalityDuration.Stacks(firstHit, lastHit))
            {
                if (StacksDetailList.Items.Count > count) { ((EnduranceDebuffDetail) StacksDetailList.Items[count]).Update(hotdot, stack, abnormalityDuration, firstHit, lastHit);}
                else { StacksDetailList.Items.Add(new EnduranceDebuffDetail(hotdot, stack, abnormalityDuration, firstHit, lastHit)); }
                count++;
            }
            while (StacksDetailList.Items.Count > count + 1) StacksDetailList.Items.RemoveAt(count + 1);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e) { ((ClickThrouWindow)Window.GetWindow(this))?.Move(sender, e); }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Background = new SolidColorBrush(Color.FromArgb(0x10, 255, 255, 255));
        }
    }
}