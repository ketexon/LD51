using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputManager))]
    public class Player : MonoBehaviour
    {
        [System.NonSerialized]
        public PlayerMovement Movement;
        [System.NonSerialized]
        public PlayerInputManager InputManager;
        void Awake()
        {
            Movement = GetComponent<PlayerMovement>();
            InputManager = GetComponent<PlayerInputManager>();

            State.Ready += OnStateReady;
        }

        private void OnStateReady()
        {
            if (State.Local.Player != null)
            {
                Destroy(State.Local.Player);
            }
            State.Local.Player = this;
        }
    }
}
