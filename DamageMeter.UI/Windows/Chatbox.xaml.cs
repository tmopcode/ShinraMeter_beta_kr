﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Data;

namespace DamageMeter.UI
{
    public partial class Chatbox
    {
        private bool _updated;

        public Chatbox()
        {
            InitializeComponent();
            CloseWindow.Source = BasicTeraData.Instance.ImageDatabase.Close.Source;

            PacketProcessor.Instance.TickUpdated += Update;

        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            PacketProcessor.Instance.TickUpdated -= Update;
            Close();
        }

        public void Update(UiUpdateMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                var chatbox = message.Chatbox;
                if (_updated) { return; }

                _updated = true;
                if (chatbox.Count == 0) Close();
                for (var i = 0; i < chatbox.Count; i++)
                {
                    if (ChatboxList.Items.Count > i) { ((ChatMessageUi) ChatboxList.Items.GetItemAt(i)).Update(chatbox[i]); }
                    else { ChatboxList.Items.Add(new ChatMessageUi(chatbox[i])); }
                }

                SnapToScreen();
            });
        }

        private void ChatboxList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ChatboxList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedItems.Count <= 1) { return; }
            var messages = "";
            foreach (var messageUi in ((ListBox) sender).SelectedItems.Cast<ChatMessageUi>().OrderBy(x => x.Time.Content))
            {
                messages = messages + $"{messageUi.Time.Content} {messageUi.Channel.Content} {messageUi.Sender.Content}: {messageUi.Message.Text}" +
                           Environment.NewLine;
            }
            Clipboard.SetDataObject(messages);
        }

        protected override bool Empty => !ChatboxList.HasItems && _updated;
    }
}