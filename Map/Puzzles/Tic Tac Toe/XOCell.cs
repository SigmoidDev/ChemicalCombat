using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class XOCell : MonoBehaviour, IInteractable
    {
        [SerializeField] private TicTacToe board;
        [SerializeField] private Vector2Int reference;
        [SerializeField] private SpriteRenderer contents;
        [SerializeField] private Sprite xSprite;
        [SerializeField] private Sprite xWinner;
        [SerializeField] private Sprite oSprite;
        [SerializeField] private Sprite oWinner;
        private bool isUsed;

        private static readonly Color InvisibleColor = new Color(1f, 1f, 1f, 0f);
        private static readonly Color HoveringColor = new Color(1f, 1f, 1f, 0.3f);
        private static readonly Color PlacedColor = new Color(1f, 1f, 1f, 1f);

        public bool CanInteract => !isUsed && board.CanPlay;
        public void Highlight() => contents.color = HoveringColor;
        public void Unhighlight()
        {
            if(!CanInteract) return;
            contents.color = InvisibleColor;
        }

        public void InteractWith()
        {
            bool playerWins = board.Place(reference, false);
            if(!playerWins) board.PlayRobot();
        }

        public void Place(bool isX)
        {
            isUsed = true;
            contents.color = PlacedColor;
            contents.sprite = isX ? xSprite : oSprite;
        }

        public void Glow(bool isX) => contents.sprite = isX ? xWinner : oWinner;
    }
}
