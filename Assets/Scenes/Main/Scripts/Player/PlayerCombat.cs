using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    public abstract class PassiveWeapon : MonoBehaviour
    {
        
    }

    [DisallowMultipleComponent]
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField]
        LayerMask humanMask;

        [SerializeField]
        ActiveWeaponInfo defaultActiveWeapon;

        [System.NonSerialized]
        public ActiveWeaponInfo ActiveWeaponInfo;
        [System.NonSerialized]
        public ActiveWeapon ActiveWeapon;

        List<PassiveWeapon> passiveWeapons = new List<PassiveWeapon>();

        Vector2 fireDirection = new Vector2();
        float lastFireTime = -1f;

        bool CanFire =>
            ActiveWeapon != null
            && (
                lastFireTime < 0
                || GameTime.Time > lastFireTime + 1/ActiveWeapon.RealizedStats.Rate
            )
            && !GameTime.Instance.Paused;

        public void AttachActiveWeapon(ActiveWeaponInfo info)
        {
            if (ActiveWeapon != null)
            {
                ActiveWeapon.OnDetach();
            }

            // We clone ActiveWeaponInfo because we want to mutate it
            ActiveWeaponInfo = Instantiate(info);
            var go = Instantiate(ActiveWeaponInfo.Prefab);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.name = ActiveWeaponInfo.Name;

            ActiveWeapon = go.GetComponent<ActiveWeapon>();
            Debug.Assert(ActiveWeapon != null, "UseWeapon called with GO that has no active weapon");

            ActiveWeapon.PlayerCombat = this;
            ActiveWeapon.Info = ActiveWeaponInfo;
            ActiveWeapon.OnAttach();
        }

        public void UpgradeActiveWeapon(ActiveWeaponUpgrades upgrades)
        {
            ActiveWeapon.Upgrades += upgrades;
        }

        void Awake()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            AttachActiveWeapon(defaultActiveWeapon);
        }

        void Start()
        {
            var actions = State.Local.Player.InputManager.Actions;
            actions.Player.Fire.started += OnInputFire;
            actions.Player.Fire.performed += OnInputFire;
            actions.Player.Fire.canceled += OnInputFire;
        }

        private void OnInputFire(InputAction.CallbackContext ctx)
        {
            fireDirection = ctx.ReadValue<Vector2>().normalized;
        }

        void Update()
        {
            if(fireDirection != Vector2.zero && CanFire)
            {
                lastFireTime = GameTime.Time;
                ActiveWeapon.OnFire(fireDirection);
            }
        }
    }
}
