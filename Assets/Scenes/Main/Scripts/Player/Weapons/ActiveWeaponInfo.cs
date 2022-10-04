using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [CreateAssetMenu(
        fileName = "ActiveWeapon.asset",
        menuName = "Combat/ActiveWeapon"
    )]
    public class ActiveWeaponInfo : WeaponInfo
    {
        public GameObject Prefab;
        public ActiveWeaponStatField UsedStats;
    }
}
