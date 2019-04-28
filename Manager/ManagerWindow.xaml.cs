using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manager
{
    public partial class ManagerWindow : Window
    {
        private NotifyIconHelper IconHelper;
        private BackgroundTask BackgroundTask;

        private ConfigStorage ConfigStorage;
        private TaskStorage TaskStorage;

        public ManagerWindow()
        {
            //Initialize
            StartHelper.Initialize();

            //Initialize storages
            ConfigStorage = ConfigStorage.Global;
            TaskStorage = TaskStorage.Global;

            //Initialize Window
            InitializeComponent();

            //Hide window on startup
            Hide();

            //Create context menu for notify icon
            ContextMenu contextMenu = new ContextMenu();

            bool hasExitOption = false;

            if (ConfigStorage != null)
            {
                hasExitOption = ConfigStorage.GetBool(Config.ConfigEnableExitOptionKey, Config.ConfigDefaultEnableExitOptionValue);
            }

            if (hasExitOption)
            {
                MenuItem menuExitItem = new MenuItem();
                menuExitItem.Text = "Exit";
                menuExitItem.Click += (object sender, EventArgs e) =>
                {
                    if (IconHelper != null)
                        IconHelper.Destroy();
                    if (BackgroundTask != null)
                        BackgroundTask.Stop();

                    //Exit application
                    if (System.Windows.Forms.Application.MessageLoop)
                    {
                        // WinForms app
                        System.Windows.Forms.Application.Exit();
                    }
                    else
                    {
                        // Console app
                        Environment.Exit(1);
                    }
                };
                contextMenu.MenuItems.Add(menuExitItem);
            }

            //Create notification icon in taskbar
            NotifyIconHelper.Data data = new NotifyIconHelper.Data();
            data.ContextMenu = contextMenu;

            if (ConfigStorage != null)
            {
                data.ShowAlways = ConfigStorage.GetBool(Config.ConfigShowIconAlwaysKey, Config.ConfigDefaultShowIconAlwaysValue);
                data.DestroyOnClose = false;

                data.NoClickEvent = !ConfigStorage.GetBool(Config.ConfigEnableGuiKey, Config.ConfigDefaultEnableGuiValue);
                data.NoMenu = false;

                data.NoBalloonTip = !ConfigStorage.GetBool(Config.ConfigEnableBalloonTipKey, Config.ConfigDefaultEnableBalloonTipValue);
            }

            data.Icon = Properties.Resources.ic_application;
            data.BalloonTipTitle = Title;
            data.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            data.Text = Title;

            IconHelper = new NotifyIconHelper(this, data);
            IconHelper.Create();

            //Create background worker
            BackgroundTask = new BackgroundTask();
            BackgroundTask.Start();

            //Setup controls
            SetupControls();
        }
    }
}
