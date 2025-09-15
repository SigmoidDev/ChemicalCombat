using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// Controls the difficulty of the game, including enemy health and speed, room credits, puzzle difficulty, and coins earned
    /// </summary>
	public class DifficultyManager : Singleton<DifficultyManager>
	{
		private Difficulty selectedDifficulty;
        public static Difficulty Difficulty
        {
            get => Instance.selectedDifficulty;
            set => Instance.selectedDifficulty = value;
        }

        public static float HealthMultipler => Difficulty switch
        {
            Difficulty.Rookie => 0.7f,
            Difficulty.Veteran => 1.33f,
            _ => 1f
        };
        public static float SpeedMultipler => Difficulty switch
        {
            Difficulty.Rookie => 0.75f,
            Difficulty.Veteran => 1.33f,
            _ => 1f
        };
        public static float CoinsMultipler => Difficulty switch
        {
            Difficulty.Rookie => 1.4f,
            Difficulty.Veteran => 0.75f,
            _ => 1f
        };
        public static float CreditsMultiplier => Difficulty switch
        {
            Difficulty.Rookie => 0.667f,
            Difficulty.Veteran => 1.25f,
            _ => 1f
        };

        /// <summary>
        /// Scales enemy health based on either:<br/>
        /// 1 + 0.08x^1.4 + 0.02x on Rookie,<br/>
        /// 1 + 0.08x^1.6 + 0.08x on Skilled, or<br/>
        /// 1 + 0.08x^1.8 + 0.16x on Veteran
        /// </summary>
        /// <returns></returns>
        public static float GetHealthScaling() => Difficulty switch
        {
            Difficulty.Rookie => 1f + 0.08f * (Mathf.Pow(FloorManager.Instance.FloorNumber, 1.4f) + 0.25f * FloorManager.Instance.FloorNumber),
            Difficulty.Skilled => 1f + 0.08f * (Mathf.Pow(FloorManager.Instance.FloorNumber, 1.6f) + FloorManager.Instance.FloorNumber),
            _ => 1f + 0.08f * (Mathf.Pow(FloorManager.Instance.FloorNumber, 1.8f) + 2f * FloorManager.Instance.FloorNumber)
        };

        /// <summary>
        /// Whether or not the player has completed the tutorial on this PC (saved through playerPrefs)
        /// </summary>
        public static bool HasCompletedTutorial => PlayerPrefs.HasKey("tutorialCompleted");

        /// <summary>
        /// Sets a flag in the playerPrefs file to indicate that the tutorial has already been completed
        /// </summary>
        public static void CompleteTutorial() => PlayerPrefs.SetInt("tutorialCompleted", 1);
	}

    public enum Difficulty
    {
        Rookie,
        Skilled,
        Veteran
    }
}
