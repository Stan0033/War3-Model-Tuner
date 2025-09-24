using System;
using System.Windows;
using System.Windows.Threading;
using Wa3Tuner.Dialogs;

namespace Wa3Tuner
{
    public partial class App : Application
    {
        debug_console_w Debug_Console;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Register global handlers first
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);

            // 2. Initialize debug console
            Debug_Console = new debug_console_w();

            // 3. Wrap MainWindow creation safely
            try
            {
                string file = e.Args.Length > 0 ? e.Args[0] : "";
                MainWindow mainWindow = null;

                try
                {
                    mainWindow = new MainWindow(Debug_Console, file);
                }
                catch (System.Windows.Markup.XamlParseException xamlEx)
                {
                    ShowError("XAML Error: " + xamlEx.Message, xamlEx);
                    Shutdown(); // Stop the app if XAML is broken
                    return;
                }

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                ShowError("Startup Error: " + ex.Message, ex);
                Shutdown(); // Stop the app if startup fails
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // UI thread exceptions
            ShowError("Unhandled exception: " + e.Exception.Message, e.Exception);
            e.Handled = true; // Prevent crash if possible
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Non-UI thread exceptions
            if (e.ExceptionObject is Exception ex)
            {
                // Make sure to show the MessageBox on the UI thread
                Dispatcher.Invoke(() =>
                {
                    ShowError("Critical exception: " + ex.Message, ex);
                });
            }
        }

        private void ShowError(string message, Exception ex)
        {
            try
            {
                Debug_Console?.Add(ex.Message);
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // Swallow any exceptions thrown while showing the error
            }
        }
    }
}
