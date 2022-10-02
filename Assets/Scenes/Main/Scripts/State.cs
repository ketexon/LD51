using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class LocalStateData
    {
        public Player Player;
        public Population Population;

        public int ChildrenPerHuman = 1;
    }

    public class State : MonoBehaviour
    {
        public static State Instance { get; private set; }
        public static LocalStateData Local => Instance == null ? null : Instance.local;

        public static event System.Action Ready;

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
                Ready?.Invoke();
            }
        }
    }
}
