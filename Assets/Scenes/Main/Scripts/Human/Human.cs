using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [RequireComponent(typeof(Collider2D))]
    public class Human : MonoBehaviour
    {
        [SerializeField]
        float velocity = 3f;

        [SerializeField]
        [Min(0f)]
        float minTargetPositionDistance = 1f;
        [SerializeField]
        [Min(0f)]
        float maxTargetPositionDistance = 3f;

        [SerializeField]
        [Min(0f)]
        float minWaitTimeBetweenMovements = 0.2f;
        [SerializeField]
        [Min(0f)]
        float maxWaitTimeBetweenMovements = 1f;

        new Collider2D collider;

        Population population;

        Vector2 targetPosition;
        Vector2 originalPosition;
        Vector2 movingDirection;
        float movementDistance => (targetPosition - originalPosition).magnitude;
        float curDistance => ((Vector2)transform.position - originalPosition).magnitude;

        bool waiting = false;
        float waitTime = 0f;
        float startWaitingTime = 0f;

        public void Kill()
        {
            State.Local.Population.RegisterDeath(this);
            Destroy(gameObject);
        }

        void Awake()
        {
            collider = GetComponent<Collider2D>();

            waiting = false;
        }

        void Start()
        {
            population = State.Local.Population;
            targetPosition = transform.position;
            GameTime.Instance.EveryTenSeconds += Populate;
        }

        void Update()
        {
            if (!waiting)
            {

                if(curDistance >= movementDistance)
                {
                    waitTime = Random.Range(
                        minWaitTimeBetweenMovements, 
                        maxWaitTimeBetweenMovements
                    );
                    waiting = true;
                    startWaitingTime = GameTime.Time;

                    originalPosition = targetPosition;
                    var dist = Random.Range(
                        minTargetPositionDistance,
                        maxTargetPositionDistance
                    );
                    var theta = Random.Range(0, 2 * Mathf.PI);
                    targetPosition += new Vector2(
                        Mathf.Cos(theta),
                        Mathf.Sin(theta)
                    ) * dist;
                    movingDirection = targetPosition - originalPosition;
                    movingDirection.Normalize();
                }
                else
                {
                    transform.position += 
                        GameTime.DeltaTime * velocity * (Vector3)movingDirection;
                }
            }
            else
            {
                if(GameTime.Time >= startWaitingTime + waitTime)
                {
                    waiting = false;
                }
            }
        }

        void OnDestroy()
        {
            GameTime.Instance.EveryTenSeconds -= Populate;
        }

        void Populate()
        {
            for(int i = 0; i < State.Local.ChildrenPerHuman; i++)
            {
                var go = Instantiate(gameObject);
                var child = go.GetComponent<Human>();
                population.RegisterBirth(child);
            }
        }
    }
}
