using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine.Jobs;
using System;
using Unity.Jobs;
using System.Reflection;

namespace LD51
{
    public class PopulationV2 : MonoBehaviour
    {
        public int Count => activeHumans;
        public int Deaths => humansKilled;

        [SerializeField]
        HumanParameters humanParameters;

        [SerializeField]
        GameObject humanPrefab;

        [SerializeField]
        int startPopulation;

        [SerializeField]
        int maxPopulation;

        [SerializeField]
        Vector2 range;

        [SerializeField]
        Vector2 visibleRange;

        HumanV2[] humans;
        //HumanInstanceData[] humanData;

        // Marker to update livingHumanTransforms
        // Happens on activate/kill

        int activeHumans;
        int allocatedHumans;
        int maxAllocatedHumans => 2 * maxPopulation;

        TransformAccessArray livingHumanTransforms;
        NativeArray<HumanInstanceData> humanData;
        
        Unity.Mathematics.Random random;

        HumanTransformJob humanTransformJob;
        JobHandle humanTransformJobHandle;

        int humansKilled = 0;

        int humansToAdd = 0;
        float humanAddSpeed = 0f;

        // sorted so that we get the last index first
        // (when we deactivate a human, the activated - 1th human
        // gets moved, so we deactivate the last human first
        // in case it is the activated - 1th human)
        public SortedSet<int> humansToKill = new SortedSet<int>();

        public void KillHuman(int index)
        {
            // don't touch their gameobject until after 
            // job runs, so just store their index
            humansToKill.Add(index);
        }

        public void KillHuman(HumanV2 human)
        {
            KillHuman(human.Index);
        }

        void Awake()
        {
            foreach(Transform childTransform in transform)
            {
                Destroy(childTransform.gameObject);
            }
        }

        void Start()
        {
            if (State.Local.PopulationV2 != null)
            {
                Destroy(State.Local.PopulationV2);
            }
            State.Local.PopulationV2 = this;

            GameTime.Instance.EveryTenSeconds += () =>
            {
                Debug.Log("Ten seconds passed");
            };
            GameTime.Instance.EveryTenSeconds += Populate;

            humans = new HumanV2[maxAllocatedHumans];
            livingHumanTransforms = new TransformAccessArray(startPopulation);
            humanData = new NativeArray<HumanInstanceData>(
                maxAllocatedHumans,
                Allocator.Persistent
            );
            humanTransformJob = new HumanTransformJob
            {
                Random = random,
                HumanParameters = humanParameters,
                HumanInstances = humanData,
                Time = GameTime.Time,
                DeltaTime = GameTime.DeltaTime
            };

            activeHumans = 0;
            allocatedHumans = 0;

            InitPopulation();
            InitPool();
        }

        void Update()
        {
            // if we populated, update population (over time)
            PopulateUpdate();

            if (!GameTime.Instance.Paused)
            {
                random = new Unity.Mathematics.Random(
                    Reinterpret.IntToUInt(
                        Random.Range(int.MinValue, int.MaxValue)
                    )
                );

                humanTransformJob = new HumanTransformJob
                {
                    Random = random,
                    HumanParameters = humanParameters,
                    HumanInstances = humanData,
                    Time = GameTime.Time,
                    DeltaTime = GameTime.DeltaTime
                };
                humanTransformJobHandle = humanTransformJob.Schedule(livingHumanTransforms);
            }
        }

        void LateUpdate()
        {
            humanTransformJobHandle.Complete();

            if(humansToKill.Count > 0)
            {
                foreach (int index in humansToKill.Reverse())
                {
                    DeactivateHuman(index);
                    humansKilled++;
                }
                humansToKill.Clear();
            }
        }

        void OnDestroy()
        {
            livingHumanTransforms.Dispose();
            humanData.Dispose();
        }

        HumanV2 AllocateHuman()
        {
            if(allocatedHumans >= maxAllocatedHumans)
            {
                Debug.LogWarning("Tried to allocate human beyond max capacity");
                return null;
            }
            var go = Instantiate(humanPrefab);
            go.name = string.Format("Human {0}", allocatedHumans);
            go.SetActive(false);
            go.transform.SetParent(transform);

            var human = humans[allocatedHumans] = go.GetComponent<HumanV2>();
            human.Index = allocatedHumans;

            allocatedHumans++;

            return human;
        }

        HumanV2[] AllocateHumans(int n)
        {
            if (allocatedHumans + n > maxAllocatedHumans)
            {
                Debug.LogWarning("Tried to allocate human beyond max capacity");
                return null;
            }
            HumanV2[] humans = new HumanV2[n];
            for(int i = 0; i < n; i++)
            {
                humans[i] = AllocateHuman();
            }
            return humans;
        }

        HumanV2 ActivateHuman()
        {
            var human = humans[activeHumans];
            humanData[activeHumans] = new HumanInstanceData
            {
                Waiting = false,
                EndStateTime = -math.INFINITY, // job will change state immediately
            };
            human.gameObject.SetActive(true);
            livingHumanTransforms.Add(human.transform);
            activeHumans++;

            return human;
        }

        HumanV2[] ActivateHumans(int n)
        {
            HumanV2[] activatedHumans = new HumanV2[n];
            for(int i = 0; i < n; i++)
            {
                activatedHumans[i] = ActivateHuman();
            }
            return activatedHumans;
        }

        void DeactivateHuman(int index)
        {
            // swap human at index with last human
            var human = humans[index];
            human.gameObject.SetActive(false);
            SwapHumans(index, activeHumans - 1);
            // Note: SwapHumans does not sawp transformAccessArray
            // BUT RemoveAtSwapBack does the same thing
            livingHumanTransforms.RemoveAtSwapBack(index);
            activeHumans--;
            if(humansToAdd > 0)
            {
                humansToAdd--;
            }
        }

        void SwapHumans(int i, int j)
        {
            humans[i].Index = j;
            humans[j].Index = i;
            (humans[i], humans[j]) = 
                (humans[j], humans[j]);

            (humanData[i], humanData[j])
                = (humanData[j], humanData[i]);
        }

        void InitPopulation()
        {
            AllocateHumans(startPopulation);
            var startHumans = ActivateHumans(startPopulation);
            foreach(var human in startHumans)
            {
                human.transform.position = (Vector3)new Vector2(
                    Random.Range(-1f, 1f) * range.x,
                    Random.Range(-1f, 1f) * range.y
                );
            }
        }

        void InitPool()
        {
            // pool inits linearly
            // population increases exponentially (2^(t/10))
            // -> if pool allocates to maxPopulation at the same time
            // population reaches there, there'll never be a problem
            // we'll be two steps ahead, (InitPoolOverTime will allocate 2*maxPopulation)
            // (and we divide by two)
            // startPopulation * 2^(t/10) = maxPopulation
            // t = 10*log_2(maxPopulation/startPopulation)
            // There can also be fancy code to allocate according to current population
            var timeToAllocate = 10 * math.log2((float)maxPopulation / startPopulation / 2f);
            // second arg is batchSize
            // not sure how it affects FPS lmao
            // lower batch size would probably be smoother, but more consistent FPS loss
            // higher batch size might have spikes
            // there is probably a negative impact of having too low 
            // batch size, b/c going from main to coroutine
            StartCoroutine(InitPoolOverTime(timeToAllocate, 10));
        }

        IEnumerator InitPoolOverTime(float seconds, int batchSize)
        {
            int batches = (int)Mathf.Ceil((float)maxAllocatedHumans / batchSize);
            float batchInterval = seconds / batches;
            while(allocatedHumans < maxAllocatedHumans)
            {
                AllocateHumans(
                    allocatedHumans + batchSize <= maxAllocatedHumans
                    ? batchSize
                    : maxAllocatedHumans - allocatedHumans
                );
                yield return new WaitForSeconds(batchInterval);
            }
        }

        void Populate()
        {
            humansToAdd = activeHumans;
            humanAddSpeed = humansToAdd / 0.5f;
        }

        void PopulateUpdate()
        {
            if(humansToAdd > 0)
            {
                int clampedBatchSize = math.clamp(
                    (int)(GameTime.DeltaTime * humanAddSpeed),
                    1,
                    humansToAdd
                );

                for (int i = 0; i < clampedBatchSize; i++)
                {
                    var parent = humans[humansToAdd - i - 1];
                    var child = ActivateHuman();
                    child.transform.position = parent.transform.position;
                }
                humansToAdd -= clampedBatchSize;
                if(humansToAdd <= 0)
                {
                    humanAddSpeed = 0;
                    humansToAdd = 0;
                }
            }
        }
    }
}
