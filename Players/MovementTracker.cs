using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Players
{
    /// <summary>
    /// For the sole purpose of a single death message
    /// </summary>
	public class MovementTracker : Singleton<MovementTracker>
	{
		private float timeStoodStill;
        public static float TimeStoodStill => Instance.timeStoodStill;

        private void Update()
        {
            timeStoodStill += Time.deltaTime;
            if(Player.Instance.Movement.Body.velocity != Vector2.zero) timeStoodStill = 0f;
        }
	}
}
