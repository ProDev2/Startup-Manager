using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Manager
{
    public static class Config
    {
        //Application
        public static readonly string ApplicationName = "Startup Manager";

        public static readonly string EmptyItemPlaceholder = "<empty>";

        //Registry
        public static readonly RegistryKey RegistrySpace = Registry.CurrentUser;

        public static readonly string RegistryFolder = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static readonly string RegistryKey = ApplicationName;

        //Config storage
        public static readonly string RelativeConfigStoragePath = "config.txt";

        //Config storage keys
        public static readonly string ConfigEnableRegistryEntryKey = "enable_registry_entry";
        public static readonly string ConfigRedirectRegistryEntryKey = "redirect_registry_entry";

        public static readonly string ConfigIntervalKey = "update_interval";
        public static readonly string ConfigShowIconAlwaysKey = "show_icon_always";
        public static readonly string ConfigEnableGuiKey = "enable_gui";
        public static readonly string ConfigEnableBalloonTipKey = "enable_balloon_tip";
        public static readonly string ConfigEnableExitOptionKey = "enable_exit_option";

        //Config storage default values
        public static readonly bool ConfigDefaultEnableRegistryEntryValue = false;
        public static readonly string ConfigDefaultRedirectRegistryEntryValue = "";

        public static readonly long ConfigDefaultIntervalValue = 1 * 60 * 1000;
        public static readonly bool ConfigDefaultShowIconAlwaysValue = true;
        public static readonly bool ConfigDefaultEnableGuiValue = true;
        public static readonly bool ConfigDefaultEnableBalloonTipValue = false;
        public static readonly bool ConfigDefaultEnableExitOptionValue = true;

        //Task storage
        public static readonly string RelativeTaskStoragePath = "tasks.txt";
    }
}
