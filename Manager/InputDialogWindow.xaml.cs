using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manager
{
    public partial class InputDialogWindow : Window
    {
        public delegate void OnCancel(string message);
        public delegate void OnMessage(string message);

        public event OnCancel OnCancelEvent;
        public event OnMessage OnMessageEvent;

        private string Title;
        private string Text;

        private bool SingleLine;

        private Brush NormalBrush;
        private Brush ErrorBrush;

        public string DialogTitle
        {
            get
            {
                return Title;
            }

            set
            {
                Title = value;
                if (Title == null)
                    Title = "";

                base.Title = Title;
                if (TitleLabel != null)
                    TitleLabel.Content = Title;
            }
        }

        public string DialogText
        {
            get
            {
                return Text;
            }

            set
            {
                Text = value;
                if (Text == null)
                    Text = "";

                if (InputTextBox != null)
                    InputTextBox.Text = Text;

                bool canAccept = Text != null && Text.Length > 0;
                if (AcceptButton != null)
                    AcceptButton.IsEnabled = canAccept;
            }
        }

        public bool DialogSingleLine
        {
            get
            {
                return SingleLine;
            }

            set
            {
                SingleLine = value;

                if (InputTextBox != null)
                    InputTextBox.AcceptsReturn = !SingleLine;
            }
        }

        public InputDialogWindow(string title) : this(title, null)
        {
        }

        public InputDialogWindow(string title, string text) : this(title, text, true)
        {
        }

        public InputDialogWindow(string title, string text, bool singleLine)
        {
            //Initialize Window
            InitializeComponent();

            //Setup elements
            DialogTitle = title;
            DialogText = text;

            DialogSingleLine = singleLine;

            //Setup variables
            NormalBrush = InputTextBox.BorderBrush;
            ErrorBrush = new SolidColorBrush(Colors.Red);

            //Setup drag behavior
            ToolbarLayout.MouseDown += (object sender, MouseButtonEventArgs args) =>
            {
                DragMove();
            };

            //Setup close behavior
            CloseButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnDeny())
                    Close();
            };

            KeyDown += (object sender, KeyEventArgs args) =>
            {
                if (args.Key == Key.Return && SingleLine)
                {
                    args.Handled = true;

                    if (OnAccept())
                        Close();
                }

                if (args.Key == Key.Escape && DenyButton != null)
                {
                    if (OnDeny())
                        Close();
                }
            };

            Closing += (object sender, CancelEventArgs args) =>
            {
                OnDeny();
            };

            //Setup events
            Loaded += OnLoaded;

            InputTextBox.TextChanged += (object sender, TextChangedEventArgs args) =>
            {
                try
                {
                    string message = "";
                    if (InputTextBox != null)
                        message = InputTextBox.Text;
                    if (message == null)
                        message = "";

                    if (AcceptButton != null)
                        AcceptButton.IsEnabled = message.Length > 0;

                    if (InputTextBox != null && NormalBrush != null && ErrorBrush != null)
                    {
                        Brush brush = message.Length > 0 ? NormalBrush : ErrorBrush;
                        if (brush != null)
                        {
                            InputTextBox.BorderBrush = brush;
                        }
                    }
                } catch (Exception e)
                {
                }
            };

            AcceptButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnAccept())
                    Close();
            };

            DenyButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnDeny())
                    Close();
            };
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (InputTextBox != null)
            {
                InputTextBox.Focus();

                if (InputTextBox.Text != null && InputTextBox.Text.Length > 0)
                {
                    InputTextBox.SelectionStart = InputTextBox.Text.Length;
                    InputTextBox.SelectionLength = 0;
                }
            }
        }

        protected bool OnAccept()
        {
            try
            {
                string message = "";
                if (InputTextBox != null)
                    message = InputTextBox.Text;
                if (message == null)
                    message = "";

                if (message.Length > 0)
                {
                    if (OnMessageEvent != null)
                        OnMessageEvent(message);
                    return true;
                }
                else if (InputTextBox != null)
                {
                    InputTextBox.Text = message;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        protected bool OnDeny()
        {
            try
            {
                string message = "";
                if (InputTextBox != null)
                    message = InputTextBox.Text;
                if (message == null)
                    message = "";

                if (OnCancelEvent != null)
                    OnCancelEvent(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }
    }
}
