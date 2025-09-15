using UnityEngine;

namespace Sigmoid.Weapons
{
	[CreateAssetMenu(fileName = "New Weapon Type", menuName = "Players/Create New Weapon")]
	public class ScriptableWeapon : ScriptableObject
	{
		[field: SerializeField] public float FireRate { get; private set; }
		[field: SerializeField] public float ReloadSpeed { get; private set; }
        [field: SerializeField] public float Responsiveness { get; private set; }
	}
}
