using UnityEngine;
using System.IO;

namespace Sigmoid.Utilities
{
    /// <summary>
    /// Allows the user to take screenshots and saves them to appdata/LocalLow (or equivalent) with a timestamp
    /// </summary>
	public class Screenshotter : Singleton<Screenshotter>
	{
        /// <summary>
        /// Gets (and creates if missing) the screenshot folder within Application.persistentDataPath
        /// </summary>
        public static string Folder
        {
            get
            {
                string path = Application.persistentDataPath + "/Screenshots/";
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// Takes a screenshot and timestamps it with the current date and time
        /// </summary>
        /// <param name="upscaling"></param>
		public static void TakeScreenshot(int upscaling = 1)
        {
            string time = System.DateTime.Now.ToString("dd-MM-yy @ HH-mm-ss");
            string file = Folder + $"Screenshot {time}.png";
            ScreenCapture.CaptureScreenshot(file, upscaling);
        }
	}
}
