using UnityEngine;

namespace Sigmoid.Utilities
{
    public class WaitForAnimation : CustomYieldInstruction
    {
        private readonly Animator animator;
        private readonly float fraction;

        public WaitForAnimation(Animator animator, float fraction)
        {
            this.animator = animator;
            this.fraction = Mathf.Clamp01(fraction);
        }

        public override bool keepWaiting => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < fraction;
    }
}