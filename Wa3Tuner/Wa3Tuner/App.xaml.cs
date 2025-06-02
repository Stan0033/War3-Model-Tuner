using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wa3Tuner.Dialogs;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        debug_console_w Debug_Console;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Debug_Console = new debug_console_w();
            try
            {

           
                    string file = "";
                    if (e.Args.Length > 0)
                    {
                       file = e.Args[0];
                    }
                var mainWindow = new MainWindow(Debug_Console, file );
                mainWindow.Show();
            
            }  catch (Exception ex) { Debug_Console.Add(ex.Message); MessageBox.Show(ex.Message); }

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        private void App_DispatcherUnhandledException(object? sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle UI thread exceptions
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true; // Prevent the app from crashing
            Debug_Console.Add(e.Exception.Message);
        }
        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            // Handle non-UI thread exceptions
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Critical exception: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug_Console.Add(ex.Message);
            }
        }
    }
}