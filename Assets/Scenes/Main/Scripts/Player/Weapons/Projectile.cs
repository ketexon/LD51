using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [System.NonSerialized]
        public Vector2 Direction;
        [System.NonSerialized]
        public ProjectileWeapon Weapon;

        public LayerMask HumanMask;

        float duration;

        float startTime = 0f;

        int nKilled = 0;

        Queue<Human> hits = new Queue<Human>();

        void Start()
        {
            duration = Weapon.RealizedStats.Range / Weapon.RealizedStats.Velocity;
            startTime = GameTime.Time;
            var theta = Mathf.Acos(Direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(
                Mathf.Sign(Direction.y) * theta,
                Vector3.forward
            );
        }

        void Update()
        {
            if(GameTime.Time > startTime + duration)
            {
                Destroy(gameObject);
            }

            transform.position += (Vector3)Direction * Weapon.RealizedStats.Velocity * GameTime.DeltaTime;

            while (hits.Count > 0 && nKilled <= Weapon.RealizedStats.Pierce)
            {
                var human = hits.Dequeue();
                human.Kill();
                nKilled++;
            }
            if(nKilled > Weapon.RealizedStats.Pierce)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            var colliderLayerMask = (2 << (collision.gameObject.layer - 1));
            if ((colliderLayerMask & HumanMask.value) != 0)
            {
                var human = collision.gameObject.GetComponent<Human>();
                hits.Enqueue(human);
            }
        }
    }
}
