using UnityEngine;

namespace Sigmoid.Utilities
{
	/// <summary>
    /// Allows a class to be accessed globally
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance => InstanceExists ? instance : instance = FindObjectOfType<T>();
        public static bool InstanceExists => instance != null;
    }
}
