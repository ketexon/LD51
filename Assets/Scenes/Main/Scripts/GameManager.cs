using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class GameManager : MonoBehaviour
    {
        public event System.Action EveryTenSeconds;

        public static GameManager Instance { get; private set; }
        public static float DeltaTime => Instance.paused ? 0f : UnityEngine.Time.deltaTime;
        public static float FixedDeltaTime => Instance.paused ? 0f : UnityEngine.Time.fixedDeltaTime;
        public static float Time => Instance.paused ? Instance.lastPauseTime : UnityEngine.Time.time - Instance.netPauseTime;


        bool paused = false;
        public bool Paused { 
            get => paused;
            set
            {
                if(paused != value)
                {
                    if (value)
                    {
                        Pause();
                    }
                    else
                    {
                        Unpause();
                    }
                }
            }
        }


        float lastPauseTime;
        float netPauseTime = 0f;

        float lastEveryTenSecondsTime;


        public void TogglePause()
        {
            if (paused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (paused)
                return;

            lastPauseTime = UnityEngine.Time.time;

            paused = true;

        }

        public void Unpause()
        {
            if (!paused)
                return;

            netPauseTime += UnityEngine.Time.time - lastPauseTime;

            paused = false;
        }

        void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("Instance not null");
                Destroy(Instance);
            }
            Instance = this;
        }

        void Start()
        {
            State.Local.Time = 0f;
            lastEveryTenSecondsTime = Time;
        }

        void Update()
        {
            if(Time >= lastEveryTenSecondsTime + 10)
            {
                EveryTenSeconds?.Invoke();
                lastEveryTenSecondsTime = Time;
            }
        }
    }
}
