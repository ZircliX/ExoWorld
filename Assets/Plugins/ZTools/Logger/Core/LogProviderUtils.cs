using System.Collections.Generic;
using UnityEngine;
using ZTools.Core.Settings;
using ZTools.Logger.Core.Enums;

namespace ZTools.Logger.Core
{
    public static class LogProviderUtils
    {
        /// <summary>
        /// Gets the log mode for this logger based on the tool type.
        /// This value is retrieved from the <see cref="ZToolsSettingsData"/> settings.
        /// </summary>
        /// <returns>The <see cref="LogMode"/> found in the settings</returns>
        [HideInCallstack]
        public static ToolDefinition GetLogMode(string targetToolType)
        {
            IReadOnlyList<ToolDefinition> toolSettingsArray = ZToolsSettingsData.GetToolsSettings();
            foreach (ToolDefinition tool in toolSettingsArray)
            {
                if (tool.ToolID == targetToolType)
                    return tool;
            }

            return null;
        }

        /// <summary>
        /// Checks if the current <see cref="LogMode"/> allows logging for the specified mode.
        /// </summary>
        /// <param name="currentLogMode">The Current <see cref="LogMode"/></param>
        /// <param name="targetLogMode">The Target <see cref="LogMode"/></param>
        /// <returns>If the Current <see cref="LogMode"/> value is higher than the Target one</returns>
        [HideInCallstack]
        public static bool CanLog(this ToolDefinition currentLogMode, LogMode targetLogMode)
        {
            if ((int)currentLogMode.LogMode == -1) return true; // Everything
            if (currentLogMode.LogMode == 0) return false; // Nothing
            return (currentLogMode.LogMode & targetLogMode) != 0;
        }
    }
}