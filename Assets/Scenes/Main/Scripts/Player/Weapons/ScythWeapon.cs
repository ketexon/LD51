using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class ScythWeapon : ActiveWeapon
    {
        [SerializeField]
        GameObject scythPrefab;
        void Awake()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public override void OnFire(Vector2 direction)
        {
            // direction . <1, 0> = cos(theta)
            // -> theta = arccos(direction.x)
            var projectileGO = Instantiate(scythPrefab);
            projectileGO.transform.SetParent(transform, false);
            projectileGO.transform.localPosition = Vector3.zero;
            var projectile = projectileGO.GetComponent<Scyth>();
            projectile.Weapon = this;
            projectile.Direction = direction;
        }
    }
}
