using Sigmoid.Cameras;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class SimpleAnimator : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer sprite;
		[SerializeField] private AnimationFrame[] frames;

        private AnimationFrame current;
        private int index;
        private float timer;

        private void Awake() => current = frames[0];
        private void Update()
        {
            if((timer += Time.deltaTime) >= current.duration)
            {
                timer -= current.duration;
                index = (index + 1) % frames.Length;

                if(MainCamera.InstanceExists && !MainCamera.WithinView(transform.position, 6f)) return;
                current = frames[index];
                sprite.sprite = current.sprite;
            }
        }
	}

    [System.Serializable]
    public class AnimationFrame
    {
        public Sprite sprite;
        public float duration;
    }
}
