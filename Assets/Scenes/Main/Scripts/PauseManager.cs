using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    public class PauseManager : MonoBehaviour
    {
        void Start()
        {
            PlayerInputManager.Instance.Actions.Player.Pause.started += OnInputPause;
        }

        void OnInputPause(InputAction.CallbackContext obj)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
