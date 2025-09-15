using Sigmoid.Upgrading;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Interactables
{
    public class LootChest : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ScriptableLootTable lootTable;
        [field: SerializeField] public bool IsLocked { get; private set; }
        public void Lock()
        {
            IsLocked = true;
            animator.SetBool("Locked", true);
        }

        public void Unlock()
        {
            IsLocked = false;
            animator.SetBool("Locked", false);
        }

        private void Awake() => animator.SetBool("Locked", IsLocked);

        public event System.Action OnOpened;
        public virtual void Open()
        {
            animator.Play("Open");

            if(lootTable == null) return;
            CollectableSpawner.Instance.DropCoins(transform.position, lootTable.Coins, 8);

            foreach(Vector2Int healthRange in lootTable.hearts)
                CollectableSpawner.Instance.SpawnHeart(transform.position, Random.Range(healthRange.x, healthRange.y));

            //25% chance to drop a heart
            //75% chance to drop 6-10 coins
            if(Perks.Has(Perk.Scavenger))
            {
                if(Random.value < 0.25f) CollectableSpawner.Instance.SpawnHeart(transform.position, 1);
                else CollectableSpawner.Instance.DropCoins(transform.position, Random.Range(6, 11), 8);
            }

            CallOpenEvent();
        }

        protected void CallOpenEvent() => OnOpened?.Invoke();
    }
}
