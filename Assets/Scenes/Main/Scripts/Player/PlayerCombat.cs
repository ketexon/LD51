using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD51
{
    public abstract class ActiveWeapon : MonoBehaviour
    {
        public abstract float Interval { get; }
        public abstract string Name { get; }

        public virtual void OnAttach(PlayerCombat playerCombat) { }
        public virtual void OnDetach() { }

        public virtual void OnFire(Vector2 direction) { }
    }

    public abstract class PassiveWeapon : MonoBehaviour
    {
        
    }

    [DisallowMultipleComponent]
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField]
        GameObject defaultActiveWeapon;

        ActiveWeapon activeWeapon;
        List<PassiveWeapon> passiveWeapons = new List<PassiveWeapon>();

        Vector2 fireDirection = new Vector2();
        float lastFireTime = -1f;

        bool CanFire =>
            activeWeapon != null
            && (
                lastFireTime < 0
                || GameTime.Time > lastFireTime + activeWeapon.Interval
            )
            && !GameTime.Instance.Paused;

        void Awake()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            AttachActiveWeapon(Instantiate(defaultActiveWeapon));
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

        void AttachActiveWeapon(GameObject go)
        {
            if(activeWeapon != null)
            {
                activeWeapon.OnDetach();
            }

            activeWeapon = go.GetComponent<ActiveWeapon>();
            Debug.Assert(activeWeapon != null, "UseWeapon called with GO that has no active weapon");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.name = activeWeapon.Name;

            activeWeapon.OnAttach(this);
        }
    }
}
