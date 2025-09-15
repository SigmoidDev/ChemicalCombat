using System.Collections.Generic;
using Sigmoid.Upgrading;
using Sigmoid.Effects;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class TreeNode : MonoBehaviour
	{
        [field: SerializeField] public Perk Perk { get; private set; }
		[field: SerializeField] public PurchaseState State { get; private set; }
        [field: SerializeField] public Vector2Int Index { get; private set; }

        [SerializeField] private Image image;
        [SerializeField] private PaletteSwapper swapper;
        [SerializeField] private HoverGrow hoverer;
        [SerializeField] private TreeQuadrant parentQuadrant;
        [SerializeField] private List<TreeNode> associatedNodes;

        public Color[] DarkColours => parentQuadrant.HiddenColours;
        public Color[] Colours => parentQuadrant.PurchasedColours;

        private void Awake()
        {
            PerkTree.Instance.RegisterNode(this);
            UpdateColours();
        }

        public void UpdateColours() => swapper.UpdateLUT(swapper.Originals, GetAppropriateColours());
        public Color[] GetAppropriateColours()
        {
            return State == PurchaseState.Purchased ? parentQuadrant.PurchasedColours
            : State == PurchaseState.Available ? parentQuadrant.AvailableColours
            : parentQuadrant.HiddenColours;
        }

        public void Unlock()
        {
            if(State == PurchaseState.Hidden)
            {
                tag = "Clickable";
                State = PurchaseState.Available;
                UpdateColours();
            }
        }

        public void Purchase()
        {
            if(State == PurchaseState.Available)
            {
                State = PurchaseState.Purchased;
                UpdateColours();
            }

            Perks.Unlock(Perk);
            foreach(TreeNode node in associatedNodes)
                node.Unlock();
        }

        public void Click()
        {
            if(State == PurchaseState.Hidden) return;
            PerkTree.Instance.FocusOn(this, parentQuadrant.OnRightSide, image.sprite);
        }
    }

    public enum PurchaseState
    {
        Hidden,
        Available,
        Purchased
    }
}
