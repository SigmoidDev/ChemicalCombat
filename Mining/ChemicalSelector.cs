using Sigmoid.Chemicals;
using Sigmoid.UI;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.Mining
{
    //look i had to name it something other than ChemicalPicker cause that already exists
	public class ChemicalSelector : MonoBehaviour
	{
		[SerializeField] private Chemical chemical;
		[SerializeField] private Image image;

        private void Start()
        {
            UpdateSprite();
            ChemicalManager.Instance.OnChemicalUnlocked += CheckUpdates; //unsubscribed automatically when the drilling menu is unloaded
        }

        private void CheckUpdates(Chemical chemical)
        {
            if(chemical == this.chemical) UpdateSprite();
        }

        public void UpdateSprite()
        {
            ScriptableChemical info = ChemicalManager.Get(chemical);
            bool unlocked = ChemicalManager.IsUnlocked(chemical);
            image.sprite = unlocked ? info.unlockedSprite : info.digitalSprite;
        }

        public void View() => DrillingMenu.Instance.Select(chemical);
    }
}
