using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace LD51
{
    [RequireComponent(typeof(Button))]
    public class NewWeaponButton : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        public event System.Action OnSelectEvent;
        public event System.Action OnSubmitEvent;

        [System.NonSerialized]
        public Button Button;
        [System.NonSerialized]
        public WeaponInfo WeaponInfo;

        [SerializeField]
        Image spriteImage;
        [SerializeField]
        TMP_Text nameTMP;

        void Awake()
        {
            Button = GetComponent<Button>();
        }

        public void OnSelect(BaseEventData data)
        {
            OnSelectEvent?.Invoke();
        }

        public void OnSubmit(BaseEventData data)
        {
            OnSubmitEvent?.Invoke();
        }

        public void Load(WeaponInfo weaponInfo)
        {
            WeaponInfo = weaponInfo;
            if (WeaponInfo.Sprite)
            {
                spriteImage.sprite = WeaponInfo.Sprite;
            }
            
            nameTMP.text = WeaponInfo.Name;
        }

        public void SetNavigation(Button onUp, Button onDown, Button onLeft, Button onRight)
        {
            Button.navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                wrapAround = false,
                selectOnUp = onUp,
                selectOnDown = onDown,
                selectOnLeft = onLeft,
                selectOnRight = onRight
            };
        }
    }
}
