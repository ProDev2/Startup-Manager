using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.ComponentModel;

namespace Manager
{
    public class NotifyIconHelper
    {
        private Window Window;
        private WindowState WindowState;

        private NotifyIcon Icon;
        private Data IconData;

        private bool ShowingContextMenu;

        public NotifyIconHelper(Window window, Data data) : this(window, window != null ? window.WindowState : WindowState.Normal, data)
        {
        }

        public NotifyIconHelper(Window window, WindowState state, Data data)
        {
            if (window == null)
                throw new ArgumentNullException("No window");
            if (state == WindowState.Minimized)
                state = WindowState.Normal;

            this.Window = window;
            this.WindowState = state;
            this.IconData = data;
        }

        public Data GetData()
        {
            return IconData;
        }

        public void SetData(Data data, bool recreate)
        {
            this.IconData = data;

            if (recreate)
                Create();
        }

        public void Create()
        {
            Destroy();

            if (Window == null) return;

            if (IconData == null)
                IconData = new Data();

            if (IconData.BalloonTipTitle == null)
                IconData.BalloonTipTitle = Window.Title;

            Icon = new NotifyIcon();

            if (!IconData.NoMenu)
            {
                if (IconData.ContextMenu != null) Icon.ContextMenu = IconData.ContextMenu;
                if (IconData.ContextMenuStrip != null) Icon.ContextMenuStrip = IconData.ContextMenuStrip;
            }

            if (IconData.Icon != null) Icon.Icon = IconData.Icon;

            if (IconData.BalloonTipTitle != null) Icon.BalloonTipTitle = IconData.BalloonTipTitle;
            if (IconData.BalloonTipText != null) Icon.BalloonTipText = IconData.BalloonTipText;
            if (IconData.Text != null) Icon.Text = IconData.Text;

            Icon.MouseClick += OnClick;

            if (Icon.ContextMenu != null)
                Icon.ContextMenu.Popup += OnOpenContextMenu;
            if (Icon.ContextMenuStrip != null)
                Icon.ContextMenuStrip.Opening += OnOpenContextMenu;

            Window.Closing += OnClose;
            Window.StateChanged += OnStateChanged;
            Window.IsVisibleChanged += OnIsVisibleChanged;

            UpdateIcon();
        }

        public void Show()
        {
            if (Icon == null)
                Create();

            if (Icon != null)
            {
                Icon.Visible = true;
            }
        }

        public void Hide()
        {
            if (Icon != null)
            {
                Icon.Visible = false;
            }
        }

        public void Destroy()
        {
            Hide();

            if (Icon != null)
            {
                Icon.Visible = false;
                Icon.Dispose();

                if (Icon.ContextMenu != null)
                    Icon.ContextMenu.Popup -= OnOpenContextMenu;
                if (Icon.ContextMenuStrip != null)
                    Icon.ContextMenuStrip.Opening -= OnOpenContextMenu;

                Icon = null;
            }

            if (Window != null)
            {
                Window.Closing -= OnClose;
                Window.StateChanged -= OnStateChanged;
                Window.IsVisibleChanged -= OnIsVisibleChanged;
            }
        }

        protected void OnClose(object sender, CancelEventArgs args)
        {
            if (Icon == null) return;
            UpdateIcon();

            if (IconData == null || !IconData.DestroyOnClose)
            {
                args.Cancel = true;

                if (Window != null)
                {
                    Window.WindowState = WindowState.Minimized;
                }
            } else
            {
                Destroy();
            }
        }

        protected void OnStateChanged(object sender, EventArgs args)
        {
            if (Icon == null) return;
            UpdateIcon();

            if (Window != null)
            {
                if (Window.WindowState == WindowState.Minimized)
                {
                    if (Window != null)
                        Window.Hide();

                    if ((IconData == null || !IconData.NoBalloonTip) && Icon != null)
                        Icon.ShowBalloonTip(2000);
                }
                else
                {
                    if (Window.WindowState == WindowState.Minimized)
                        WindowState = WindowState.Normal;
                    else
                        WindowState = Window.WindowState;
                }
            }
        }

        protected void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (Icon == null) return;
            UpdateIcon();
        }

        protected void OnClick(object sender, MouseEventArgs args)
        {
            if (Icon == null) return;

            if (!ShowingContextMenu && Window != null)
            {
                if (IconData == null || !IconData.NoClickEvent)
                {
                    Window.Show();
                    Window.WindowState = WindowState;

                    Window.Activate();

                    UpdateIcon();
                }
            }

            ShowingContextMenu = false;
        }

        public void OnOpenContextMenu(object sender, EventArgs args)
        {
            ShowingContextMenu = true;
        }

        public void UpdateIcon()
        {
            if (IconData == null || !IconData.ShowAlways)
            {
                if (Window != null)
                {
                    if (Window.IsVisible)
                        Hide();
                    else
                        Show();
                }
            }
            else if (!Icon.Visible)
            {
                Show();
            }
        }

        public class Data
        {
            public ContextMenu ContextMenu;
            public ContextMenuStrip ContextMenuStrip;

            public bool ShowAlways;
            public bool DestroyOnClose;

            public bool NoClickEvent;
            public bool NoMenu;

            public bool NoBalloonTip;

            public Icon Icon;
            public string BalloonTipTitle;
            public string BalloonTipText;
            public string Text;
        }
    }
}
