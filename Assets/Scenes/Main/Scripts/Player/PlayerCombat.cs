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
        ActiveWeaponInfo defaultActiveWeapon;

        ActiveWeaponInfo activeWeaponInfo;
        ActiveWeapon activeWeapon;
        List<PassiveWeapon> passiveWeapons = new List<PassiveWeapon>();

        Vector2 fireDirection = new Vector2();
        float lastFireTime = -1f;

        bool CanFire =>
            activeWeapon != null
            && (
                lastFireTime < 0
                || GameTime.Time > lastFireTime + activeWeaponInfo.Interval
            )
            && !GameTime.Instance.Paused;

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
                activeWeapon.OnFire(fireDirection);
            }
        }

        void AttachActiveWeapon(ActiveWeaponInfo info)
        {
            if(activeWeapon != null)
            {
                activeWeapon.OnDetach();
            }

            // We clone ActiveWeaponInfo because we want to mutate it
            activeWeaponInfo = Instantiate(info);
            var go = Instantiate(activeWeaponInfo.Prefab);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.name = activeWeaponInfo.Name;

            activeWeapon = go.GetComponent<ActiveWeapon>();
            Debug.Assert(activeWeapon != null, "UseWeapon called with GO that has no active weapon");

            activeWeapon.OnAttach(this);
        }
    }
}
