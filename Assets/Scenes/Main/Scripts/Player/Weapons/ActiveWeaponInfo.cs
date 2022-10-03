using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [System.Flags]
    public enum ActiveWeaponStats
    {
        None = 0,
        Pierce = 1,
        Duration = 2,
    }

    [CreateAssetMenu(
        fileName = "ActiveWeapon.asset",
        menuName = "Combat/ActiveWeapon"
    )]
    public class ActiveWeaponInfo : ScriptableObject
    {
        public string Name;
        public float Interval;
        public GameObject Prefab;
        public ActiveWeaponStats AvailableStats;
    }
}
