using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Cameras;
using Sigmoid.Effects;
using Sigmoid.Audio;
using Sigmoid.Game;
using Sigmoid.UI;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Sigmoid.Upgrading
{
    /// <summary>
    /// The interface that allows you to purchase upgrades once selected in the Perk Tree
    /// </summary>
	public class PerkBuyer : Singleton<PerkBuyer>
	{
        private static WaitForSecondsRealtime _waitForSecondsRealtime0_2 = new WaitForSecondsRealtime(0.2f);

        [field: SerializeField] public RectTransform Rect { get; private set; }
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public RectTransform Box { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Title { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Description { get; private set; }
        [field: SerializeField] public PaletteSwapper Palette { get; private set; }
        [field: SerializeField] public Image Fill { get; private set; }
        [field: SerializeField] public CanvasGroup Information { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Price { get; private set; }

        [SerializeField] private Color affordableColour;
        [SerializeField] private Color expensiveColour;

        [SerializeField] private AudioPlayer errorSound;
        [SerializeField] private AudioPlayer purchaseSound;
        [SerializeField] private AudioPlayer typingSound;
        [SerializeField] private AudioSource buyingTone;

        private int SelectedPrice => Perks.GetInfo(node.Perk).basePrice;
        private Coroutine textFade;
        private TreeNode node;

        /// <summary>
        /// Populates the description, title, and sprite with the necessary information
        /// </summary>
        /// <param name="node"></param>
        /// <param name="side"></param>
        /// <param name="sprite"></param>
        public void Initialise(TreeNode node, bool side, Sprite sprite)
        {
            fillAmount = 0f;
            this.node = node;
            Vector2 boxPosition = Vector2.right * (side ? -32f : 32f);

            ScriptablePerk info = Perks.GetInfo(node.Perk);
            Price.SetText(info.basePrice.ToString());

            Box.gameObject.SetActive(true);
            Box.anchoredPosition = boxPosition;
            Description.horizontalAlignment = side ? HorizontalAlignmentOptions.Right : HorizontalAlignmentOptions.Left;
            Title.horizontalAlignment = side ? HorizontalAlignmentOptions.Right : HorizontalAlignmentOptions.Left;

            string description =  Options.Current.Rigorousness == RigorousnessLevel.Careless ? info.shortDescription
                                : Options.Current.Rigorousness == RigorousnessLevel.Sufficient ? info.mediumDescription
                                : info.longDescription;

            if(textFade != null) StopCoroutine(textFade);
            textFade = StartCoroutine(TextUtilities.RevealText(Description, description, 0.6f, true));
            Title.text = node.Perk.ToString();
            StartCoroutine(PlayTextNoises(0.6f, 0.1f));

            IEnumerator PlayTextNoises(float duration, float interval)
            {
                float elapsed = 0f;
                while((elapsed += interval) < duration)
                {
                    if(typingSound.gameObject.activeInHierarchy) typingSound.Play();
                    yield return new WaitForSecondsRealtime(interval);
                }
            }

            StartCoroutine(RefreshGameObject());
            IEnumerator RefreshGameObject()
            {
                yield return null;
                Information.gameObject.SetActive(false);
                if(node.State != PurchaseState.Purchased) Information.gameObject.SetActive(true);
            }

            Icon.sprite = sprite;
            Fill.sprite = sprite;
            Palette.UpdateLUT(Palette.Originals, node.Colours);
        }

        private float fillAmount;
        private bool isHeld;

        private void Update()
        {
            if(node == null) return;
            if(node.State == PurchaseState.Purchased)
            {
                Fill.fillAmount = 1f;
                return;
            }

            if(isHeld || fillAmount > 0.85f)
            {
                if(Options.Current.QuickPurchase) fillAmount = 1f;
                else
                {
                    float shakeFactor = Mathf.Lerp(0.02f, 0.05f, fillAmount);
                    MainCamera.ShakeScreen(Time.unscaledDeltaTime, shakeFactor, 1f, true);
                    fillAmount = Mathf.Clamp01(fillAmount + 0.5f * Time.unscaledDeltaTime);

                    buyingTone.pitch = Mathf.Lerp(0.5f, 1.5f, fillAmount);
                }
            }
            else fillAmount = Mathf.Lerp(fillAmount, 0f, 8f * Time.unscaledDeltaTime);

            Fill.fillAmount = fillAmount;
            if(fillAmount >= 0.99f) BuyPerk();
        }

        /// <summary>
        /// Unlocks a perk, having already checked that it can be purchased
        /// </summary>
        private void BuyPerk()
        {
            CoinManager.Spend(SelectedPrice);
            node.Purchase();

            purchaseSound.Play();
            MainCamera.ShakeScreen(0.4f, 0.4f, 0.97f, true);
            PerkTree.Instance.Unfocus();
        }

        private Coroutine flash;
        /// <summary>
        /// Temporarily flashes the price in red to indicate that it is too expensive
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlashPrice()
        {
            Price.color = expensiveColour;
            yield return _waitForSecondsRealtime0_2;
            Price.color = affordableColour;
        }

        /// <summary>
        /// Begins lerping the fillAmount and pitch of the tone if this upgrade is affordable
        /// </summary>
        public void Hold()
        {
            if(node == null) return;

            if(!CoinManager.CanAfford(SelectedPrice))
            {
                if(flash != null) StopCoroutine(flash);
                flash = StartCoroutine(FlashPrice());

                errorSound.Play();
                MainCamera.ShakeScreen(0.4f, 0.4f, 0.97f, true);
                return;
            }

            isHeld = true;
            buyingTone.Play();
        }

        /// <summary>
        /// Stops lerping fillAmount and cancels the noise
        /// </summary>
        public void Release()
        {
            isHeld = false;
            buyingTone.Stop();
        }
    }
}
