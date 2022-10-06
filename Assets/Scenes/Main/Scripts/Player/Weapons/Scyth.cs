using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class Scyth : MonoBehaviour
    {
        [System.NonSerialized]
        public Vector2 Direction;
        [System.NonSerialized]
        public ScythWeapon Weapon;

        public LayerMask HumanMask;

        float duration;

        float startTime = 0f;

        int nKilled = 0;

        Queue<Human> hits = new Queue<Human>();

        float velocitySign = 1f;

        void Start()
        {
            var midTheta = Mathf.Sign(Direction.y) * Mathf.Acos(Direction.x);
            // range = 10 -> goes from -pi to pi
            var highTheta = midTheta - Weapon.RealizedStats.Range;
            var lowTheta = midTheta + Weapon.RealizedStats.Range;
            if(-Mathf.PI/3 < midTheta && midTheta < Mathf.PI / 3)
            {
                // Make it swing CCW if on right to make it from top to bottom
                transform.rotation = Quaternion.AngleAxis(
                    lowTheta * Mathf.Rad2Deg,
                    Vector3.forward
                );
                velocitySign = -1;
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(
                    highTheta * Mathf.Rad2Deg,
                    Vector3.forward
                );
            }
            
            startTime = GameTime.Time;
            duration = (lowTheta - highTheta)/Weapon.RealizedStats.Velocity;
        }

        void Update()
        {
            transform.rotation = Quaternion.AngleAxis(
                velocitySign * Weapon.RealizedStats.Velocity * GameTime.DeltaTime * Mathf.Rad2Deg,
                Vector3.forward
            ) * transform.rotation;
            if (GameTime.Time > startTime + duration)
            {
                Destroy(gameObject);
            }

            while (hits.Count > 0 && nKilled <= Weapon.RealizedStats.Pierce)
            {
                var human = hits.Dequeue();
                human.Kill();
                nKilled++;
            }
            if (nKilled >= Weapon.RealizedStats.Pierce)
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
