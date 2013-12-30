﻿namespace OrtzIRC.WPF
{
    using System;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainViewModel viewModel;
        private MainWindow window = new MainWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            CompositionRoot.Wire(new Bindings());
            viewModel = CompositionRoot.Resolve<MainViewModel>();
            base.OnStartup(e);

            viewModel.RequestClose += ViewModelRequestClose;

            window.DataContext = viewModel;
            window.Closing += Window_Closing;
            window.Show();
        }

        private void ViewModelRequestClose(object sender, EventArgs e)
        {
            viewModel.RequestClose -= ViewModelRequestClose;
            window.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            window.Closing -= Window_Closing;
            viewModel.RequestClose -= ViewModelRequestClose; //Otherwise Close gets called again
            viewModel.CloseCommand.Execute(null);
        }
    }
}