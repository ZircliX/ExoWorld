using UnityEngine;
using ZTools.Logger.Core.Enums;
using ZTools.Logger.Core.Interfaces;

namespace ZTools.Logger.Core
{
    /// <summary>
    /// A simple logger utility for Unity that wraps around Unity's Debug class.
    /// </summary>
    public class LogProvider : ILogSource
    {
        public string Name { get; set; }
        public readonly string toolType;
        private readonly ToolDefinition toolDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProvider"/> class with an optional name.
        /// Typically used to identify the source of the log messages (e.g. "GameController Logger", "Player Logger", etc.).
        /// </summary>
        /// <param name="name">Name of the Logger</param>
        /// <param name="toolType">Type of the Tool, used to find the corresponding <see cref="toolDefinition"/> in the settings</param>
        public LogProvider(string name, string toolType)
        {
            Name = name;
            this.toolType = toolType;

            toolDefinition = LogProviderUtils.GetLogMode(toolType);
        }

        /// <summary>
        /// Logs a message to the Unity console with the specified sender.
        /// </summary>
        /// <param name="logSource">The ILogSource requesting a Log</param>
        /// <param name="message">The String sent by the <see cref="logSource"/></param>
        [HideInCallstack]
        public void Log(ILogSource logSource, string message)
        {
            if (!toolDefinition.CanLog(LogMode.Log)) return;
            
            logSource ??= this;
            Debug.Log($"[{toolType}] - [{logSource.Name}]" +
                      $"\n{message}", logSource as Object);
        }
        
        /// <summary>
        /// Logs an error to the Unity console without a specific sender.
        /// Uses <see cref="Log(ILogSource,string)"/> under the hood
        /// </summary>
        /// <param name="message">The String to log</param>
        [HideInCallstack]
        public void Log(string message)
        {
            Log(null, message);
        }

        /// <summary>
        /// Logs a warning to the Unity console with the specified sender.
        /// </summary>
        /// <param name="logSource">The ILogSource requesting a Log</param>
        /// <param name="message">The String sent by the <see cref="logSource"/></param>
        [HideInCallstack]
        public void LogWarning(ILogSource logSource, string message)
        {
            if (!toolDefinition.CanLog(LogMode.LogWarning)) return;
            
            logSource ??= this;
            Debug.LogWarning($"[{toolType}] - [{logSource.Name}] " +
                             $"\n{message}", logSource as Object);
        }
        
        /// <summary>
        /// Logs an error to the Unity console without a specific sender.
        /// Uses <see cref="LogWarning(ILogSource,string)"/> under the hood
        /// </summary>
        /// <param name="message">The String to log</param>
        [HideInCallstack]
        public void LogWarning(string message)
        {
            LogWarning(null, message);
        }

        /// <summary>
        /// Logs an error to the Unity console with the specified sender.
        /// </summary>
        /// <param name="logSource">The ILogSource requesting a Log</param>
        /// <param name="message">The String sent by the <see cref="logSource"/></param>
        [HideInCallstack]
        public void LogError(ILogSource logSource, string message)
        {
            if (!toolDefinition.CanLog(LogMode.LogError)) return;
            
            logSource ??= this;
            Debug.LogError($"[{toolType}] - [{logSource.Name}] " +
                           $"\n{message}", logSource as Object);
        }
        
        /// <summary>
        /// Logs an error to the Unity console without a specific sender.
        /// Uses <see cref="LogError(ILogSource,string)"/> under the hood
        /// </summary>
        /// <param name="message">The String to log</param>
        [HideInCallstack]
        public void LogError(string message)
        {
            LogError(null, message);
        }
    }
}