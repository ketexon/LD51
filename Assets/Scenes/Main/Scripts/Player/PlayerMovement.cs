using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    public class PlayerMovement : MonoBehaviour
    {
        public Vector2 Position => transform.position;

        Vector2 input;
        [SerializeField]
        float velocity;

        void Start()
        {
            var actions = State.Local.Player.InputManager.Actions;
            actions.Player.Move.started += OnPlayerMove;
            actions.Player.Move.performed += OnPlayerMove;
            actions.Player.Move.canceled += OnPlayerMove;
        }

        void OnPlayerMove(InputAction.CallbackContext ctx)
        {
            input = ctx.ReadValue<Vector2>().normalized;
        }

        void Update()
        {
            // We use delta for position b/c we don't want jittery movement
            transform.position += (Vector3) input * velocity * GameTime.DeltaTime;
        }
    }

}