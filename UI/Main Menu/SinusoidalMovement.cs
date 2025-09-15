using UnityEngine;

namespace Sigmoid.UI
{
	public class SinusoidalMovement : MonoBehaviour
	{
		[SerializeField] private Vector2 speed;
		[SerializeField] private Vector2 strength;

		private void Update()
		{
			float x = Mathf.Cos(Time.time * speed.x);
			float y = Mathf.Sin(Time.time * speed.y);
			transform.localPosition = new Vector2(x, y) * strength;
		}
	}
}
