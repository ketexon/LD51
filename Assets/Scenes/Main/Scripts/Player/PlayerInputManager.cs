using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance { get; private set; } = null;

        [System.NonSerialized]
        public PlayerInput PlayerInput;
        [System.NonSerialized]
        public InputActions Actions;

        void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("Instance nonnull when PlayerInputManager instanciated.");
                Destroy(Instance);
            }
            Instance = this;

            PlayerInput = GetComponent<PlayerInput>();
            Actions = new InputActions();
            PlayerInput.actions = Actions.asset;
        }
    }
}
