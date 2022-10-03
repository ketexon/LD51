using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public abstract class ActiveWeapon : MonoBehaviour
    {
        public virtual void OnAttach(PlayerCombat playerCombat) { }
        public virtual void OnDetach() { }

        public virtual void OnFire(Vector2 direction) { }
    }
}
