using Sigmoid.Game;

namespace Sigmoid.Interactables
{
	public class CustomChest : LootChest
	{
        public int NumCoins { get; set; }
        public int MaxCoins { get; set; }
        public override void Open()
        {
            CallOpenEvent();
            CollectableSpawner.Instance.DropCoins(transform.position, NumCoins, MaxCoins);
        }
    }
}
