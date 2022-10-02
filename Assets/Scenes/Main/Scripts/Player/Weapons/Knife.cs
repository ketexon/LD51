using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class Knife : ActiveWeapon
    {
        public int Pierce = 0;

        [SerializeField]
        float range = 2f;

        [SerializeField]
        LayerMask humanMask;

        [SerializeField]
        GameObject projectilePrefab;

        public override float Interval => 1;
        public override string Name => "knife";


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
            var projectile = projectileGO.GetComponent<KnifeProjectile>();
            projectile.Direction = direction;
            projectile.Pierce = Pierce;
        }
    }
}
