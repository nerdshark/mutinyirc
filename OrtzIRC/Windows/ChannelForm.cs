﻿namespace OrtzIRC
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using FlamingIRC;

    public partial class ChannelForm : Form
    {
        private Server Server;
        private Channel Channel;
        private string ChannelName;

        public ChannelForm(Channel channel, Server server)
        {
            InitializeComponent();

            Server = server;
            Channel = channel;
            ChannelName = channel.Name;

            this.Text = channel.Name;

            Channel.OnReceivedNames += new ReceivedNamesEventHandler(Channel_OnReceivedNames);

            //var tempBinding = new Binding("Items", Channel.NickList, "NamesLiteral");
            //nickListBox.DataBindings.Add(tempBinding);
            //this.nickListBox.DataSource = channel.NickList;
            this.nickListBox.DisplayMember = "NamesLiteral";

            Channel.OnMessage += new ChannelMessageEventHandler(Channel_OnMessage);
            Channel.OnAction += new ChannelMessageEventHandler(Channel_OnAction);
            Channel.OnShowTopic += new TopicShowEventHandler(Channel_OnShowTopic);
            Channel.OnJoin += new ChannelJoinEventHandler(Channel_OnJoin);
            Channel.OnUserPart += new ChannelPartOtherEventHandler(Channel_OnPartOther);
            Channel.OnUserQuit += new ChannelQuitEventHandler(Channel_OnUserQuit);
            Channel.OnNick += new Server_NickEventHandler(Channel_OnNick);
            Channel.OnKick += new ChannelKickEventHandler(Channel_OnKick);

            this.commandTextBox.Focus();
        }

        void Channel_OnKick(User nick, string kickee, string reason)
        {
            this.AddLine("-- Kick: (" + nick.Nick + ") was kicked by (" + kickee + ") " + reason);
        }

        void Channel_OnReceivedNames(List<User> nickList)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                nickListBox.Items.Clear();

                foreach (User nick in nickList)
                {
                    nickListBox.Items.Add(nick);
                }
            });
        }

        void Channel_OnNick(User nick, string newNick)
        {
            this.AddLine("-- Nick: (" + nick.Nick + ") is now known as (" + newNick + ")");
        }

        void Channel_OnUserQuit(User nick, string message)
        {
            this.AddLine("-- Quit: (" + nick.Nick + ") (" + nick.HostMask + ") " + message);
        }

        void Channel_OnPartOther(User nick, string message)
        {
            if (message != String.Empty)
                this.AddLine("-- Parted: (" + nick.Nick + ") (" + nick.HostMask + ") " + message);
            else
                this.AddLine("-- Parted: (" + nick.Nick + ") (" + nick.HostMask + ")");
        }

        void Channel_OnJoin(User nick)
        {
            this.AddLine("-- Joined: (" + nick.Nick + ") (" + nick.HostMask + ")");
        }

        void Channel_OnShowTopic(string topic)
        {
            this.AddLine("topic: (" + topic + ")");
        }

        void Channel_OnAction(User nick, string message)
        {
            this.AddLine("-- " + nick.Nick + " " + message);
        }

        void Channel_OnMessage(User nick, string message)
        {
            this.AddLine(nick.NamesLiteral + ": " + message);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Server.Connection.Sender.Part(ChannelName);
        }

        public void AddLine(string line)
        {
            channelOutputBox.AppendLine(line);
        }
    }
}
