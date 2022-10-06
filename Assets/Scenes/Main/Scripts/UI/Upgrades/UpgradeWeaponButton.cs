using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace LD51
{
    [RequireComponent(typeof(Button))]
    public class UpgradeWeaponButton : MonoBehaviour, ISubmitHandler
    {
        public event System.Action OnSubmitEvent;

        [SerializeField]
        Image iconImage;

        [SerializeField]
        Sprite iconSprite;

        [SerializeField]
        public ActiveWeaponStatField UpgradeStat;

        [SerializeField]
        public ActiveWeaponUpgrades Upgrades;

        [System.NonSerialized]
        public Button Button;

        void Awake()
        {
            if(iconSprite != null)
            {
                iconImage.sprite = iconSprite;
            }

            Button = GetComponent<Button>();
        }

        public void OnSubmit(BaseEventData data)
        {
            OnSubmitEvent?.Invoke();
        }
    }
}
