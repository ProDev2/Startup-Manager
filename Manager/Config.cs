using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public static class Config
    {
        //Application
        public static readonly string EmptyItemPlaceholder = "<empty>";

        //Config
        public static readonly string RelativeConfigStoragePath = "config.txt";

        public static readonly string ConfigIntervalKey = "update_interval";
        public static readonly string ConfigShowIconAlwaysKey = "show_icon_always";
        public static readonly string ConfigEnableGuiKey = "enable_gui";
        public static readonly string ConfigEnableBalloonTipKey = "enable_balloon_tip";
        public static readonly string ConfigEnableExitOptionKey = "enable_exit_option";

        public static readonly long ConfigDefaultIntervalValue = 2 * 60 * 1000;
        public static readonly bool ConfigDefaultShowIconAlwaysValue = true;
        public static readonly bool ConfigDefaultEnableGuiValue = true;
        public static readonly bool ConfigDefaultEnableBalloonTipValue = false;
        public static readonly bool ConfigDefaultEnableExitOptionValue = true;

        //Tasks
        public static readonly string RelativeTaskStoragePath = "tasks.txt";
    }
}
