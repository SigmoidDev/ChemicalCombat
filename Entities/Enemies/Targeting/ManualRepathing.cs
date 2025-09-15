using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Manual Repathing", menuName = "Enemies/Targeting/Repathing/Manual")]
    public class ManualRepathing : ScriptableInterval
    {
        public override RepathInterval Create() => new ManualRepather();
    }

    /// <summary>
    /// Only repaths when the Repath() function is explicitly called
    /// </summary>
    public class ManualRepather : RepathInterval
    {
        public ManualRepather() => canRepath = true;

        private bool canRepath;
        public override bool ShouldRepath => canRepath;
        public override void Update(){}
        public override void Reset() => canRepath = false;
        public bool Repath() => canRepath = true;
    }
}
