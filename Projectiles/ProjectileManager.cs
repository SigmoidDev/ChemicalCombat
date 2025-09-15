using System.Collections.Generic;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Projectiles
{
	public class ProjectileManager : Singleton<ProjectileManager>
	{
		[SerializeField] private List<ProjectilePool> pools;
        private Dictionary<Projectile, ProjectilePool> dict;
        public ProjectilePool Get(Projectile type) => dict.TryGetValue(type, out ProjectilePool pool) ? pool : null;

        private void Awake()
        {
            dict = new Dictionary<Projectile, ProjectilePool>();
            foreach(ProjectilePool pool in pools)
                dict.Add(pool.Prefab, pool);
        }
    }
}
