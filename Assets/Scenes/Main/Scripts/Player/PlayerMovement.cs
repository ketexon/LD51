using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    public class PlayerMovement : MonoBehaviour
    {
        Vector2 input;
        [SerializeField]
        float velocity;

        void Start()
        {
            PlayerInputManager.Instance.Actions.Player.Move.started += OnPlayerMove;
            PlayerInputManager.Instance.Actions.Player.Move.performed += OnPlayerMove;
            PlayerInputManager.Instance.Actions.Player.Move.canceled += OnPlayerMove;
        }

        void OnPlayerMove(InputAction.CallbackContext ctx)
        {
            input = ctx.ReadValue<Vector2>().normalized;
        }

        void FixedUpdate()
        {
            transform.position += (Vector3) input * velocity * GameManager.FixedDeltaTime;
        }
    }

}