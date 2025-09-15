using System.Collections.Generic;
using Sigmoid.Chemicals;
using Sigmoid.Weapons;
using Sigmoid.Effects;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class MagazineDisplay : MonoBehaviour
	{
        [SerializeField] private int weaponID;
		private Weapon linkedWeapon;

		[SerializeField] private Image reloadTimer;
		[SerializeField] private PaletteSwapper normalSwapper;
		[SerializeField] private PaletteSwapper reloadSwapper;
		[SerializeField] private Transform listParent;
		[SerializeField] private BulletDisplay chemicalPrefab;

		private void Awake()
		{
            linkedWeapon = WeaponManager.Instance.Weapons[weaponID];
			linkedWeapon.OnMagazineChanged += ChangeMagazine;
			linkedWeapon.OnFire += FireBullet;
			linkedWeapon.OnReload += ReloadMagazine;
			linkedWeapon.OnReload += RecolourOnReload;
			linkedWeapon.OnTimer += RefreshTimer;
		}

		private List<BulletDisplay> chemicalList;
		private void ChangeMagazine(List<Chemical> newMagazine)
		{
			foreach(Transform child in listParent)
				Destroy(child.gameObject);

			chemicalList = new List<BulletDisplay>();
			foreach(Chemical chemical in newMagazine)
				chemicalList.Add(Instantiate(chemicalPrefab, listParent).Initialise(chemical));

			RecolourOnReload();
		}

		private void FireBullet(int number)
		{
			if(number <= 0) return;
			chemicalList[number - 1].Expend();
			chemicalList[number - 1].transform.SetSiblingIndex(chemicalList.Count - 1);

			if(number >= chemicalList.Count) RefreshPotion(Chemical.None);
			else RefreshPotion(chemicalList[number].Chemical);
		}

		private void ReloadMagazine()
		{
            for(int i = 0; i < chemicalList.Count; i++)
            {
                BulletDisplay display = chemicalList[i];
				display.transform.SetSiblingIndex(i);
                display.Regain();
            }
        }

		private void RecolourOnReload() => RefreshPotion(chemicalList.Count > 0 ? chemicalList[0].Chemical : Chemical.None);
		private void RefreshPotion(Chemical chemical)
		{
			ScriptableChemical info = ChemicalManager.Get(chemical);
			Color[] colours = new Color[3]
			{
				info.colours[1],
				info.colours[2],
				info.colours[3]
			};

			normalSwapper.UpdateLUT(normalSwapper.Originals, colours);
			reloadSwapper.UpdateLUT(reloadSwapper.Originals, colours);
		}

        private void RefreshTimer(float fraction) => reloadTimer.fillAmount = 1f - fraction;
    }
}
