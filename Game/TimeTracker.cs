using Sigmoid.Utilities;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Game
{
	public class TimeTracker : Singleton<TimeTracker>
	{
        private float timeElapsed;
        public float SecondsSurvived => timeElapsed;
        public string TimeSurvived
        {
            get
            {
                if(timeElapsed < 60f)
                {
                    float secs = Mathf.Floor(timeElapsed);
                    return $"{secs}s";
                }

                float mins = Mathf.Floor(timeElapsed / 60f);
                if(timeElapsed < 3600f)
                {
                    float secs = Mathf.Floor(timeElapsed % 60f);
                    return $"{mins}m{secs}s";
                }

                float hours = Mathf.Floor(timeElapsed / 3600f);
                return $"{hours}h{mins % 60f}m";
            }
        }

        private void Update()
        {
            if(PlayerUI.InMenu || SceneLoader.Instance.CurrentScene != GameScene.Labyrinth) return;
            timeElapsed += Time.deltaTime;
        }
	}
}
