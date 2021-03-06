﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public partial class RegisterWindow : Window
    {
        private ConfigStorage ConfigStorage;

        private string RedirectPath;

        public RegisterWindow()
        {
            //Initialize storages
            ConfigStorage = ConfigStorage.Global;

            //Initialize Window
            InitializeComponent();

            //Setup values
            RedirectPath = ConfigStorage.GetValue(Config.ConfigRedirectRegistryEntryKey, Config.ConfigDefaultRedirectRegistryEntryValue);
            if (RedirectPath == null) RedirectPath = "";
            RedirectPath = RedirectPath.Trim();

            //Setup elements
            TitleLabel.Content = Title;

            RedirectPathTextBox.Text = RedirectPath;

            //Setup drag behavior
            ToolbarLayout.MouseDown += (object sender, MouseButtonEventArgs args) =>
            {
                DragMove();
            };

            //Setup close behavior
            CloseButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Close();
            };

            KeyDown += (object sender, KeyEventArgs args) =>
            {
                if (args.Key == Key.Return)
                {
                    args.Handled = true;

                    OnApply();
                    Close();
                }

                if (args.Key == Key.Escape)
                {
                    Close();
                }
            };

            //Setup events
            EnableButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (ConfigStorage == null) return;

                OnApply();

                try
                {
                    ConfigStorage.SetBool(Config.ConfigEnableRegistryEntryKey, true);
                    RegisterHelper.PutEntry();

                    Update();
                } catch (Exception e)
                {
                }
            };

            DisableButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (ConfigStorage == null) return;

                try
                {
                    ConfigStorage.SetBool(Config.ConfigEnableRegistryEntryKey, false);
                    RegisterHelper.RemoveEntry();

                    Update();
                }
                catch (Exception e)
                {
                }
            };

            AcceptButton.Click += (object sender, RoutedEventArgs args) =>
            {
                OnApply();
                Close();
            };

            //Load
            Update();
        }

        private void Update()
        {
            //Update info text
            string info = "";

            try
            {
                bool hasEntry = RegisterHelper.HasEntryWithCurrentPath();
                if (hasEntry)
                {
                    info += "Registry entry is set:\n\n";

                    string entryPath = RegisterHelper.GetRegistryPath();
                    string entryKey = RegisterHelper.GetRegistryKey();

                    if (entryPath != null && entryPath.Length > 0)
                        info += "Path: " + entryPath + "\n";
                    if (entryKey != null && entryKey.Length > 0)
                        info += "Entry: " + entryKey + "\n";
                } else
                {
                    info += "Registry entry isn't set";
                }
            } catch (Exception e)
            {
                info = "";
            }

            InfoTextBox.Text = info;
        }

        private void OnApply()
        {
            try
            {
                string newRedirectPath = RedirectPathTextBox.Text;
                if (newRedirectPath == null) newRedirectPath = "";
                newRedirectPath = newRedirectPath.Trim();

                if (RedirectPath != null && !RedirectPath.Equals(newRedirectPath))
                {
                    RedirectPath = newRedirectPath;

                    if (ConfigStorage != null)
                        ConfigStorage.SetValue(Config.ConfigRedirectRegistryEntryKey, newRedirectPath);

                    RegisterHelper.SetRedirectPath(newRedirectPath, false);
                    if (ConfigStorage != null)
                    {
                        bool registryEntryEnabled = ConfigStorage.GetBool(Config.ConfigEnableRegistryEntryKey, Config.ConfigDefaultEnableRegistryEntryValue);
                        if (registryEntryEnabled)
                            RegisterHelper.PutEntry();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
