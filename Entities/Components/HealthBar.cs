using Sigmoid.Effects;
using UnityEngine;
using DG.Tweening;
using Sigmoid.Game;

namespace Sigmoid.Enemies
{
    public class HealthBar : MonoBehaviour
    {
        //bro istg i fucking hate dotween (this is overkill but i now have ptsd from debugging it)
        public string UID => $"HealthBar_{GetInstanceID()}{transform.parent.GetInstanceID()}";

        [SerializeField] private SpriteRenderer backing;
        [SerializeField] private SpriteRenderer filling;
        [SerializeField] private PaletteSwapper swapper;
        [SerializeField] private Gradient[] gradients;

        public void Refresh(float fraction)
        {
            fraction = Mathf.Clamp01(fraction);

            shown = true;
            elapsed = 0f;
            DOTween.Kill(UID);

            backing.color = Color.white;
            filling.color = Color.white;
            backing.enabled = true;
            filling.enabled = true;
            filling.size = new Vector2(0.875f * fraction, 0.25f);

            Color[] colours = new Color[6]
            {
                gradients[0].Evaluate(fraction),
                gradients[1].Evaluate(fraction),
                gradients[2].Evaluate(fraction),
                gradients[3].Evaluate(fraction),
                gradients[4].Evaluate(fraction),
                gradients[5].Evaluate(fraction)
            };
            swapper.UpdateLUT(swapper.Originals, colours);
        }

        private bool shown;
        private float elapsed;
        private Color clear = new Color(1, 1, 1, 0);

        private void Update()
        {
            if((elapsed += Time.deltaTime) >= 2f && shown)
            {
                elapsed = 0f;

                Sequence sequence = DOTween.Sequence();
                sequence.SetId(UID);
                sequence.SetUpdate(false);

                sequence.Insert(0f, backing.DOColor(clear, 1f));
                sequence.Insert(0f, filling.DOColor(clear, 1f)).OnComplete(() =>
                {
                    shown = false;
                    backing.enabled = false;
                    filling.enabled = false;
                });
            }
        }

        private void Awake() => SceneLoader.Instance.OnSceneUnloading += DeleteOnUnload;
        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneUnloading -= DeleteOnUnload;
        }
        private void DeleteOnUnload(GameScene scene) => ForceHide();

        public void ForceHide()
        {
            DOTween.Kill(UID);
            shown = false;
            backing.enabled = false;
            filling.enabled = false;
        }
    }
}