﻿using Ninject;
using Ninject.Parameters;

namespace OrtzIRC.WPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FlamingIRC;
    using MvvmFoundation.Wpf;
    using OrtzIRC.Common;
    using OrtzIRC.PluginFramework;
    using OrtzIRC.WPF.Properties;

    public class MainViewModel : ViewModelBase
    {
        private PluginManager _pluginManager;
        public MTObservableCollection<IrcViewModel> Panels { get; protected set; }

        public MainViewModel(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
            Panels = new MTObservableCollection<IrcViewModel>();

            System.Windows.DependencyObject dep = new System.Windows.DependencyObject();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(dep))
            {
                //Executes in Design mode. Use for mockups.
                Panels.Add(new ServerViewModel());
                return;
            }

            Settings.Default.SettingsSaving += Default_SettingsSaving;
            ServerManager.Instance.ServerAdded += Instance_ServerCreated;

            LoadSettings();

            List<ServerSettings> servers = IrcSettingsManager.Instance.GetAutoConnectServers();
            if (servers.Count == 0)
            {
                
            }

            foreach (ServerSettings server in servers)
            {
                if (server.Nick == null)
                    server.Nick = Settings.Default.FirstNick;

                Server newServer = ServerManager.Instance.Create(new ConnectionArgs(server.Nick, server.Url, server.Ssl));
                newServer.JoinSelf += Server_JoinSelf;
                newServer.Connect();
            }

            _pluginManager.LoadPlugins(Path.Combine(Environment.CurrentDirectory, "plugins"));
            //PluginManager.LoadPlugins(Settings.Default.UserPluginDirectory);
            RandomMessages.Load();
        }

        private void Server_JoinSelf(object sender, Common.DataEventArgs<Channel> e)
        {
            var chan = CompositionRoot.Resolve<ChannelViewModel>(new ConstructorArgument("channel", e.Data));
            chan.RequestClose += Chan_RequestClose;
            Panels.Add(chan);
            var same = _pluginManager.Equals(chan.PluginManager);
        }

        private void Chan_RequestClose(object sender, EventArgs e)
        {
            var chan = (ChannelViewModel)sender;
            chan.RequestClose -= Chan_RequestClose;
            Panels.Remove(chan);
        }

        private void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            TextLoggerManager.LoggerActive = Settings.Default.LoggerActivated;
            TextLoggerManager.AddTimestamp = Settings.Default.LoggerTimestampsActivated;
            TextLoggerManager.TimeFormat = Settings.Default.LoggerTimestampFormat;
        }

        private void Instance_ServerCreated(object sender, ServerEventArgs e)
        {
            CreateServerPanel(e.Server);
            //e.Server.PrivateMessageSessionAdded += Server_PrivateMessageSessionAdded;
        }

        private void CreateServerPanel(Server server)
        {
            Panels.Add(new ServerViewModel(server));
        }

        public override void Close()
        {
            for (int i = 0; i < Panels.Count; i++)
            {
                IrcViewModel viewModel = Panels[i];
                viewModel.Close();
            }

            base.Close();
        }
    }
}
