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

        public int Pierce = 0;
        public float Velocity = 20f;
        public float Duration = 2f / 20f;
        public LayerMask HumanMask;


        new Collider2D collider;
        float startTime = 0f;

        int nKilled = 0;

        Queue<Human> hits = new Queue<Human>();

        void Awake()
        {
            collider = GetComponent<Collider2D>();
        }

        void Start()
        {
            startTime = GameTime.Time;
            var theta = Mathf.Acos(Direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(
                Mathf.Sign(Direction.y) * theta,
                Vector3.forward
            );
        }

        void Update()
        {
            transform.position += (Vector3)Direction * Velocity * GameTime.DeltaTime;
            if(GameTime.Time > startTime + Duration)
            {
                Destroy(gameObject);
            }

            while(hits.Count > 0 && nKilled <= Pierce)
            {
                var human = hits.Dequeue();
                human.Kill();
                nKilled++;
            }
            if(nKilled > Pierce)
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
