using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    [System.Flags]
    [System.Serializable]
    public enum ActiveWeaponStatField
    {
        None = 0,
        Rate = 1,
        Pierce = 2,
        Range = 4,
        Velocity = 8,
    }

    [System.Serializable]
    public class ActiveWeaponUpgrades
    {
        public int Rate = 0;
        public int Pierce = 0;
        public int Range = 0;
        public int Velocity = 0;

        public static ActiveWeaponUpgrades operator +(ActiveWeaponUpgrades a, ActiveWeaponUpgrades b)
            => new ActiveWeaponUpgrades
            {
                Rate = a.Rate + b.Rate,
                Pierce = a.Pierce + b.Pierce,
                Range = a.Range + b.Range,
                Velocity = a.Velocity + b.Velocity,
            };
    }

    [System.Serializable]
    public class ActiveWeaponStats
    {
        public float Rate = 0;
        public int Pierce = 0;
        public float Range = 0;
        public float Velocity = 0;

        public ActiveWeaponStats() { }
        public ActiveWeaponStats(ActiveWeaponUpgrades upgrades) { 
            Rate = upgrades.Rate;
            Pierce= upgrades.Pierce;
            Range = upgrades.Range;
            Velocity = upgrades.Velocity;
        }

        public static ActiveWeaponStats operator+(ActiveWeaponStats a, ActiveWeaponStats b)
            => new ActiveWeaponStats {
            Rate = a.Rate + b.Rate,
            Pierce = a.Pierce + b.Pierce,
            Range = a.Range + b.Range,
            Velocity = a.Velocity + b.Velocity,
        };

        public static ActiveWeaponStats operator *(ActiveWeaponStats a, ActiveWeaponStats b)
            => new ActiveWeaponStats
            {
                Rate = a.Rate * b.Rate,
                Pierce = a.Pierce * b.Pierce,
                Range = a.Range * b.Range,
                Velocity = a.Velocity * b.Velocity,
            };
    }

    public abstract class ActiveWeapon : MonoBehaviour
    {
        public ActiveWeaponStats StatsOffset = new ActiveWeaponStats
        {
            Rate = 1f,
            Pierce = 0,
            Range = 1f,
            Velocity = 1f,
        };
        public ActiveWeaponStats StatsMult = new ActiveWeaponStats
        {
            Rate = 1f,
            Pierce = 1,
            Range = 1f,
            Velocity = 1f,
        };

        public ActiveWeaponStats RealizedStats => (new ActiveWeaponStats(Upgrades) + StatsOffset) * StatsMult;

        [System.NonSerialized]
        public PlayerCombat PlayerCombat;
        [System.NonSerialized]
        public ActiveWeaponInfo Info;
        [System.NonSerialized]
        public ActiveWeaponUpgrades Upgrades = new ActiveWeaponUpgrades();

        public virtual void OnAttach() { }
        public virtual void OnDetach() { }

        public virtual void OnFire(Vector2 direction) { }
    }
}
