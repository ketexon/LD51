using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputManager : MonoBehaviour
    {
        [System.NonSerialized]
        public PlayerInput PlayerInput;
        [System.NonSerialized]
        public InputActions Actions;

        void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            Actions = new InputActions();
            PlayerInput.actions = Actions.asset;
        }
    }
}
