using System.IO;
using System;
using UnityEngine;

namespace Sigmoid.Utilities
{
    /// <summary>
    /// Allows for Unity debug logs and errors to be output to a file
    /// </summary>
	public class Logger : Singleton<Logger>
	{
		[field: SerializeField] public ToggledLogs ToggledLogs { get; private set; }

        /// <summary>
        /// The current log file (named as the launch time of the application)
        /// </summary>
		public string LogFile { get; private set; }
        private void Awake()
        {
            string logFolder = Application.persistentDataPath + "/Logs/";
            if(!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);

            string launchTime = DateTime.Now.ToString("dd-MM-yy @ HH-mm-ss");
            LogFile = logFolder + launchTime + ".txt";
        }

        private void OnEnable() => Application.logMessageReceivedThreaded += Log;
		private void OnDisable() => Application.logMessageReceivedThreaded -= Log;

        /// <summary>
        /// Writes a Unity Log to the currently active log file in persistentDataPath
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stack"></param>
        /// <param name="type"></param>
        public static void Log(string message, string stack, LogType type)
		{
			if(Application.isEditor || (1 << (int) type & (int) Instance.ToggledLogs) == 0) return;
			try { File.AppendAllText(Instance.LogFile, $"{type}: {message} \n{stack}\n"); } catch { }
		}
	}

    [Flags]
    public enum ToggledLogs
    {
        None = 0,
        Error = 1,
        Assert = 2,
        Warning = 4,
        Log = 8,
        Exception = 16
    }
}
