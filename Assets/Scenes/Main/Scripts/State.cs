using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [System.Serializable]
    public class LocalStateData
    {
        [System.NonSerialized]
        public Player Player;
        [System.NonSerialized]
        public Population Population;

        public int ChildrenPerHuman = 1;

        public List<WeaponTier> WeaponPool;
    }

    public class State : MonoBehaviour
    {
        public static State Instance { get; private set; }
        public static LocalStateData Local => Instance == null ? null : Instance.local;

        static event System.Action ready;
        public static event System.Action Ready
        {
            add
            {
                if(Instance != null)
                {
                    value();
                }
                else
                {
                    ready += value;
                }
            }
            remove
            {
                ready -= value;
            }
        }

        [SerializeField]
        LocalStateData local = new LocalStateData();

        void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
                ready?.Invoke();
            }
        }
    }
}
