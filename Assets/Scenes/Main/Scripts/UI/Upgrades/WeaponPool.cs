using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [System.Serializable]
    public class WeaponTier
    {
        public float Commonness;
        public List<WeaponInfo> Weapons;

        public WeaponTier Copy()
        {
            return new WeaponTier
            {
                Commonness = Commonness,
                Weapons = new List<WeaponInfo>(Weapons)
            };
        }
    }

    [CreateAssetMenu(
        fileName = "WeaponPool.asset", 
        menuName = "Combat/WeaponPool"
    )]
    public class WeaponPool : ScriptableObject
    {
        public List<WeaponTier> WeaponTiers;
    }
}
