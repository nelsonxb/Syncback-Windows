using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Syncback
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool isExiting;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            if (uqCheck())
            {
                MainWindow = new MainWindow();
                MainWindow.Closing += OnMainClosing;
            }
            else {
                Shutdown();
            }
        }

        internal void MakeExit()
        {
            isExiting = true;
            MainWindow.Close();
        }

        private void MakeHidden()
        {
            isExiting = false;
            MainWindow.Hide();
        }

        private void MakeShown()
        {
            ((MainWindow)Current.MainWindow).MakeForeground();
        }

        private void OnMainClosing(object sender, CancelEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                MainWindow.Hide();
            }
        }

        #region Unique instance rule
        private const string uqMutexName = "{bb16b2d3-8ecc-4ec8-9666-4e22e5211c86}";
        private const string uqEventName = "{4006a708-bc03-49e1-bc68-2fa3b8ea87c0}";
        private Mutex uqMutex;
        private EventWaitHandle uqEventHandle;

        private bool uqCheck()
        {
            bool isFirstInstance;
            uqMutex = new Mutex(true, uqMutexName, out isFirstInstance);
            uqEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, uqEventName);

            if (isFirstInstance)
            {
                var thread = new Thread(
                    () =>
                    {
                        while (uqEventHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => MakeShown())
                            );
                        }
                    }
                );

                thread.IsBackground = true;
                thread.Start();
                return true;
            }
            else
            {
                uqEventHandle.Set();
                return false;
            }
        }
        #endregion
    }
}
