using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [DisallowMultipleComponent]
    public class Population : MonoBehaviour
    {
        public List<Human> Humans = new List<Human>();
        public int Count => Humans.Count;
        public int Births { get; private set; } = 0;
        public int Deaths => Births - Count;

        [SerializeField]
        GameObject humanPrefab;

        [SerializeField]
        int startPopulation;

        [SerializeField]
        Vector2 range;

        [SerializeField]
        Vector2 visibleRange;

        public void RegisterBirth(Human child)
        {
            Humans.Add(child);
            child.gameObject.name = string.Format("Human {0}", Births++);
            child.transform.parent = transform;
        }

        public void RegisterDeath(Human deceased)
        {
            Humans.Remove(deceased);
        }

        void Awake()
        {
            // kill all children at first, in case there is a human there
            // for debugging
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        void Start()
        {
            if(State.Local.Population != null)
            {
                Destroy(State.Local.Population);
            }
            State.Local.Population = this;

            for (int i = 0; i < startPopulation; i++)
            {
                var humanGO = Instantiate(humanPrefab);
                var human = humanGO.GetComponent<Human>();
                RegisterBirth(human);
                humanGO.transform.position = (Vector3)new Vector2(
                    Random.Range(-1f, 1f) * range.x,
                    Random.Range(-1f, 1f) * range.y
                );
            }
        }

        // Make all humans in the range
        void EnforceRange()
        {

        }
    }
}
