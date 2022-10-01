using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class LocalStateData
    {
        public float Time = 0f;
        public float FixedTime = 0f;
    }

    public class State : MonoBehaviour
    {
        public static State Instance { get; private set; }
        public static LocalStateData Local => Instance == null ? null : Instance.local;

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
            }
        }
    }
}
