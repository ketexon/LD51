using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine.Jobs;

namespace LD51
{
    public class PopulationV2 : MonoBehaviour
    {
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

        Transform[] humanTransforms;
        int activeHumans;
        int allocatedHumans;
        int maxAllocatedHumans => 2 * maxPopulation;

        TransformAccessArray livingHumansTransforms;

        void Awake()
        {
            foreach(Transform childTransform in transform)
            {
                Destroy(childTransform.gameObject);
            }
        }

        void Start()
        {
            humanTransforms = new Transform[maxAllocatedHumans];
            activeHumans = 0;
            allocatedHumans = 0;

            InitPopulation();
        }

        Transform AllocateHuman()
        {
            if(allocatedHumans >= maxAllocatedHumans)
            {
                Debug.LogWarning("Tried to allocate human beyond max capacity");
                return null;
            }
            var go = Instantiate(humanPrefab);
            humanTransforms[allocatedHumans] = go.transform;
            allocatedHumans++;
            return go.transform;
        }

        Transform[] AllocateHumans(int n)
        {
            if (allocatedHumans + n > maxAllocatedHumans)
            {
                Debug.LogWarning("Tried to allocate human beyond max capacity");
                return null;
            }
            Transform[] transforms = new Transform[n];
            for(int i = 0; i < n; i++)
            {
                var go = Instantiate(humanPrefab);
                humanTransforms[allocatedHumans] = go.transform;
                allocatedHumans++;
                transforms[i] = go.transform;
            }
            return transforms;
        }

        void InitPopulation()
        {
            var startHumans = AllocateHumans(startPopulation);
            foreach(var human in startHumans)
            {
                human.position = (Vector3)new Vector2(
                    Random.Range(-1f, 1f) * range.x,
                    Random.Range(-1f, 1f) * range.y
                );
            }
        }

        void InitPool()
        {
            // init maxhumans before 9 seconds
            StartCoroutine(InitPoolOverTime(9.0f, 10));
        }

        IEnumerator InitPoolOverTime(float seconds, int batchSize)
        {
            int batches = (int)Mathf.Ceil((float)maxAllocatedHumans / batchSize);
            float timePerBatch = seconds / batches;
            while(allocatedHumans < maxAllocatedHumans)
            {
                AllocateHumans(
                    allocatedHumans + batchSize <= maxAllocatedHumans
                    ? batchSize
                    : maxAllocatedHumans - allocatedHumans
                );
                yield return new WaitForSeconds(timePerBatch);
            }
        }
    }
}
