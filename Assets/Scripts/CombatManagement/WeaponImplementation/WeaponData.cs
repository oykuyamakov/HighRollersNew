using System;
using CombatManagement.ProjectileManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CombatManagement.WeaponImplementation
{
    [Serializable]
    public class WeaponData
    {
        public string WeaponName;
        [PreviewField][TableColumnWidth(50)]
        public Sprite Preview;
        [SerializeField]
        private WeaponType m_WeaponType;
        [ShowIf("@m_WeaponType == WeaponType.Automatic || m_WeaponType == WeaponType.SingleShot" )]
        public ProjectileType ProjectileType;
        [ShowIf("@m_WeaponType == WeaponType.Automatic || m_WeaponType == WeaponType.SingleShot" )]
        public float ProjectileSpeed;
        [Min(0)]
        public float ReloadTime;
        [Min(1)][SerializeField]
        private int m_MagazineSize;
        [Min(0)]
        public float FireRate;
        [Min(0)] [ShowIf("@m_WeaponType == WeaponType.SingleShot" )][SerializeField]
        private float m_AutomaticFireRate;
        public float Damage;
        public float Range;
        public bool Spread;

        [ShowIf("@Spread == true" )]
        public float WiderSpreadAngle = 20;
        [ShowIf("@Spread == true" )][MaxValue(1)][MinValue(0)]
        public float WiderSpreadProbability = 0.3f; 
        [ShowIf("@Spread == true" )]
        public float NarrowSpreadAngle = 10;
        [ShowIf("@Spread == true" )][MaxValue(1)][MinValue(0)]
        public float NarrowSpreadProbability = 0.2f;
        
        public int RemainingMagazine;

        [SerializeField]
        private bool m_BigGun;
        

        public WeaponType GetWeaponType()
        {
            return m_WeaponType;
        }
        
        public int GetMagazineSize()
        {
            return m_MagazineSize;
        }

        public void DecreaseMagazineByOne()
        {
            RemainingMagazine -= 1;
        }

        public float GetReloadTime()
        {
            return ReloadTime;
        }

        public bool IsBigGun()
        {
            return m_BigGun;
        }

        public float AutomaticFireRate()
        {
            return m_WeaponType == WeaponType.Automatic ? FireRate : m_AutomaticFireRate;
        }

        public void ResetMagazine()
        {
            RemainingMagazine = m_MagazineSize;
        }
    }

    [Serializable]
    public enum WeaponType
    {
        Automatic,
        SingleShot,
        Beam,
        Charge,
    }
}
