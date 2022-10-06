using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class ProjectileWeapon : ActiveWeapon
    {
        [SerializeField]
        GameObject projectilePrefab;

        void Awake()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public override void OnFire(Vector2 direction)
        {
            // direction . <1, 0> = cos(theta)
            // -> theta = arccos(direction.x)
            var projectileGO = Instantiate(projectilePrefab);
            projectileGO.transform.position = transform.position;
            var projectile = projectileGO.GetComponent<Projectile>();
            projectile.Weapon = this;
            projectile.Direction = direction;
        }
    }
}
