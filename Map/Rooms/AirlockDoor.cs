using UnityEngine;

namespace Sigmoid.Generation
{
	public class AirlockDoor : MonoBehaviour
	{
		[SerializeField] private Animator anim;
		[SerializeField] private SpriteRenderer sprite;
        [SerializeField] private bool isClosed;

        private void Awake(){ if(isClosed) Close(); }
        public void Open() => anim.Play("Open");
        public void Close() => anim.Play("Close");
    }
}
