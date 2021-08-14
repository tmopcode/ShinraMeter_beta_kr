﻿using System.Windows;
using System.Windows.Input;
using Lang;

namespace DamageMeter.UI.EntityStats
{
    /// <summary>
    ///     Logique d'interaction pour EnduranceDebuff.xaml
    /// </summary>
    public partial class EnduranceDebuffHeader
    {
        public EnduranceDebuffHeader()
        {
            InitializeComponent();

            LabelClass.Content = LP.Class;
            LabelAbnormalityDuration.Content = LP.EffectTime;
            LabelInterval.Content = LP.Fight;
            LabelAbnormalityDurationPercentage.Content = LP.FightPercent;
            LabelName.Content = LP.Name;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e) { ((ClickThrouWindow)Window.GetWindow(this))?.Move(sender, e); }
    }
}