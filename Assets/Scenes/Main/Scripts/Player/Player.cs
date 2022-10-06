using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(PlayerCombat))]
    public class Player : MonoBehaviour
    {
        [System.NonSerialized]
        public PlayerMovement Movement;
        [System.NonSerialized]
        public PlayerInputManager InputManager;
        [System.NonSerialized]
        public PlayerCombat Combat;
        void Awake()
        {
            Movement = GetComponent<PlayerMovement>();
            InputManager = GetComponent<PlayerInputManager>();
            Combat = GetComponent<PlayerCombat>();

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
