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

        Stack<string> actionMaps = new Stack<string>();

        public void PushActionMap(string nameOrId)
        {
            actionMaps.Push(PlayerInput.currentActionMap.id.ToString());
            PlayerInput.SwitchCurrentActionMap(nameOrId);
        }

        public void PopActionMap()
        {
            if(actionMaps.Count > 0)
            {
                PlayerInput.SwitchCurrentActionMap(actionMaps.Pop());
            }
        }

        void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            Actions = new InputActions();
            PlayerInput.actions = Actions.asset;
        }
    }
}
