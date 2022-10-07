using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using TMPro;

namespace LD51
{
    [RequireComponent(typeof(Animator))]
    public class Upgrades : MonoBehaviour
    {
        [SerializeField]
        WeaponPool defaultWeaponPool;

        [SerializeField]
        Image curActiveWeaponImage;

        [SerializeField]
        TMP_Text curActiveWeaponTMP;

        [SerializeField]
        GameObject newWeaponButtonContainer;

        [SerializeField]
        GameObject newWeaponButtonPrefab;

        [SerializeField]
        List<UpgradeWeaponButton> upgradeButtons;

        List<NewWeaponButton> newWeaponButtons;

        [SerializeField]
        string animatorActivatedId = "Activated";
        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(upgradeButtons.Count > 0, "must be at least 1 upgrade button");
            State.Ready += OnStateReady;
        }

        private void OnStateReady()
        {
            State.Local.WeaponPool = new List<WeaponTier>(defaultWeaponPool.WeaponTiers);
        }

        void Start()
        {
            InitNavigation();
            //Activate();
        }

        List<WeaponInfo> SelectNewWeaponsFromPool(int max = 4)
        {
            // clone to mutate locally
            // b/c we remove weapons we already selected
            var pool = State.Local.WeaponPool.ConvertAll(tier => tier.Copy());

            float netCommonness = 0f;
            foreach (var tier in pool)
            {
                if (tier.Weapons.Count > 0)
                {
                    netCommonness += tier.Commonness;
                }
            }
            List<WeaponInfo> selected = new List<WeaponInfo>();

            for (int i = 0; i < max; i++)
            {
                float value = Random.Range(0, netCommonness);
                foreach (var tier in pool)
                {
                    if (tier.Weapons.Count > 0)
                    {
                        if (value < tier.Commonness)
                        {
                            int weaponIndex = Random.Range(0, tier.Weapons.Count);
                            selected.Add(tier.Weapons[weaponIndex]);
                            tier.Weapons.RemoveAt(weaponIndex);
                            if (tier.Weapons.Count == 0)
                            {
                                netCommonness -= tier.Commonness;
                            }
                            break;
                        }
                        value -= tier.Commonness;
                    }
                }
            }

            return selected;
        }

        public void Activate()
        {
            GameTime.Instance.Pause();
            State.Local.Player.InputManager.PushActionMap("UI");

            // Update current active weapon image and text
            WeaponInfo curActiveWeaponInfo = State.Local.Player.Combat.ActiveWeaponInfo;
            if (curActiveWeaponInfo.Sprite != null)
            {
                curActiveWeaponImage.sprite = curActiveWeaponInfo.Sprite;
            }
            curActiveWeaponTMP.text = curActiveWeaponInfo.Name;

            // Display new weapons
            foreach (Transform child in newWeaponButtonContainer.transform)
            {
                Destroy(child.gameObject);
            }

            var newWeapons = SelectNewWeaponsFromPool();
            newWeaponButtons = new List<NewWeaponButton>();
            foreach (WeaponInfo newWeapon in newWeapons)
            {
                var newWeaponButtonGO = Instantiate(newWeaponButtonPrefab);
                newWeaponButtonGO.name = string.Format("{0} Button", newWeapon.Name);
                newWeaponButtonGO.transform.SetParent(newWeaponButtonContainer.transform, false);
                var newWeaponButton = newWeaponButtonGO.GetComponent<NewWeaponButton>();
                newWeaponButton.Load(newWeapon);
                newWeaponButtons.Add(newWeaponButton);
                newWeaponButton.OnSelectEvent += () => UpdateLastSelectedNewWeaponButton(newWeaponButton);
            }
            // Update navigation for weapon buttons (default nav breaks)
            for (int i = 0; i < newWeaponButtons.Count; i++)
            {
                var button = newWeaponButtons[i];
                button.SetNavigation(
                    onUp: i == 0 ? null : newWeaponButtons[i - 1].Button,
                    onDown: i == newWeaponButtons.Count - 1 ? null : newWeaponButtons[i + 1].Button,
                    onLeft: upgradeButtons[upgradeButtons.Count - 1].Button,
                    onRight: null
                );
            }

            // Select first button
            upgradeButtons[0].Button.Select();

            // last button has explicit nav, so update its onRight
            var lastUpgradeButton = upgradeButtons[upgradeButtons.Count - 1].Button;
            var newLastButtonNav = lastUpgradeButton.navigation;
            newLastButtonNav.selectOnRight = newWeaponButtons.Count > 0 ? newWeaponButtons[0].Button : null;
            lastUpgradeButton.navigation = newLastButtonNav;
            

            // Register callbacks for each button press
            foreach(var button in upgradeButtons)
            {
                button.OnSubmitEvent += () => OnSubmitUpgrade(button);
            }
            foreach (var button in newWeaponButtons)
            {
                button.OnSubmitEvent += () => OnSubmitNewWeapon(button);
            }

            animator.SetBool(Animator.StringToHash(animatorActivatedId), true);
        }

        void Deactivate()
        {
            animator.SetBool(Animator.StringToHash(animatorActivatedId), false);
            State.Local.Player.InputManager.PopActionMap();
            GameTime.Instance.Unpause();
        }

        void InitNavigation()
        {
            // init last upgrade button to have explicit navigation
            upgradeButtons[upgradeButtons.Count - 1].Button.navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                wrapAround = false,
                selectOnUp = null,
                selectOnDown = null,
                selectOnLeft = upgradeButtons.Count > 1 ? upgradeButtons[upgradeButtons.Count - 2].Button : null,
                selectOnRight = null
            };
        }

        void UpdateLastSelectedNewWeaponButton(NewWeaponButton newWeaponButton)
        {
            Button lastButton = upgradeButtons[upgradeButtons.Count - 1].Button;
            var newNavigation = lastButton.navigation;
            newNavigation.selectOnRight = newWeaponButton.Button;
            lastButton.navigation = newNavigation;
        }

        void OnSubmitUpgrade(UpgradeWeaponButton button)
        {
            State.Local.Player.Combat.UpgradeActiveWeapon(button.Upgrades);
            Deactivate();
        }

        void OnSubmitNewWeapon(NewWeaponButton button)
        {
            if(button.WeaponInfo is ActiveWeaponInfo activeWeaponInfo)
            {
                State.Local.Player.Combat.AttachActiveWeapon(activeWeaponInfo);
            }
            // TODO: Passive weapons
            Deactivate();
        }
    }
}
