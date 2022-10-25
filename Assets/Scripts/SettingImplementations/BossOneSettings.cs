using System;
using System.Collections.Generic;
using CombatManagement.ProjectileManagement;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace SettingImplementations
{
    [CreateAssetMenu(fileName ="BossOneSettings" )]
    public class BossOneSettings : ScriptableObject
    {
        [SerializeField][HideInInspector]
        private List<Texture> DiceFaces;
        
        public Texture GetDiceFace(int index)
        {
            return DiceFaces[index];
        }

        public float GetAttackRate(int phase)
        {
            return phase switch
            {
                1 => PhaseOneValues.TimeBetweenSpreadProjectile,
                2 => PhaseTwoValues.AttackRate,
                3 => PhaseThreeValues.AttackRate,
            };
        }

        public int BossHealthLimitForStage2 = 1000;

        [Title("PHASE 1")] [InspectorName("Phase 1 Designer")]
        public PhaseOneValues PhaseOneValues;
        
        [Title("PHASE 2")] [InspectorName("Phase 2 Designer")]
        public PhaseTwoValues PhaseTwoValues;

        [Title("PHASE 3")] [InspectorName("Phase 3 Designer")]
        public PhaseThreeValues PhaseThreeValues;

        [FoldoutGroup("Sound")] [HideInInspector]
        public Sound MissileFireSound;
        public Sound BossHitGorundSound;
        public Sound BossGetDamageSound;
        public Sound PhaseChangeSound;
        public Sound BossThrowRockSound;
        public Sound BossThrowDiceSound;
        
        
        private static BossOneSettings _BossOnesettings;

        private static BossOneSettings bossOnesettings
        {
            get
            {
                if (!_BossOnesettings)
                {
                    _BossOnesettings = Resources.Load<BossOneSettings>($"Settings/BossOneSettings");

                    if (!_BossOnesettings)
                    {
#if UNITY_EDITOR
                        Debug.Log("Creating Boss 1 Settings");
                        // _BossOnesettings = CreateInstance<BossOneSettings>();
                        // var path = "Assets/Resources/Settings/BossOneSettings.asset";
                        // AssetDatabaseHelpers.CreateAssetMkdir(_BossOnesettings, path);
#else
 				//		throw new Exception("Global settings could not be loaded");
#endif
                    }
                }

                return _BossOnesettings;
            }
        }

        public static BossOneSettings Get()
        {
            return bossOnesettings;
        }
        
    }

    [Serializable]
    public class PhaseOneValues
    {
        [PropertySpace(SpaceBefore = 2, SpaceAfter = 15)]
        [EnumToggleButtons][Title("Attack Phase")][HideLabel]
        public PhaseOneAttacks PhaseOneAttack;

        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.Rock")][LabelText("Atis Sekilleri arasi sure")]
        public float TimeBetweenSpreadProjectile;
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.Rock")]
        [LabelText("Atis Sekilleri")][ListDrawerSettings(Expanded = false, ShowIndexLabels = true)][FormerlySerializedAs("RockMovements")]
        public List<SpreadProjectileInfo> ThrowBundleInfo;
       
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.Rock")]
        public bool Mayhem;
        
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.HitGround")]
        public float HitIndicatorDur;
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.HitGround")]
        public float HitRate;        
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.HitGround")]
        [LabelText("Spike Suresi")]
        public float HitDur;
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.HitGround")]
        public Color HitIndicatorColor = Color.blue;
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.HitGround")][InspectorName("Execute")]

        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.Missile")]
        public ProjectileInfo MissileMovement;
        [ShowIf("@PhaseOneAttack == PhaseOneAttacks.Missile")]
        public RepeatValues MissileRepeatValues;

    }

    [Serializable]
    public class SpreadProjectileInfo
    {
        [LabelText("Aradaki Mesafe")]
        public float SpreadAngle;
        [LabelText("Bir Firlatista Cikan")][HideIf("@_AimMode == AimMode.Angle")]
        [FormerlySerializedAs("VerticalCount")] 
        public int CountAtOnce;
        [FormerlySerializedAs("HorizontalCount")]
        [LabelText("Atis Sayisi")]
        public int ThrowCount;
        [LabelText("Atislar Arasi Sure")]
        public float TimeBetweenThrow;

        public bool Execute;
        
        public AimMode _AimMode;
        [ShowIf("@_AimMode == AimMode.Angle")]
        public bool CircularThrow;
        
        [LabelText("Atis Bilgileri")]
        public ProjectileInfo ProjectileInfo;
    }
    
    [Serializable]
    public enum AimMode{
        Player,
        Front,
        Angle
    }
    
    [Serializable]
    public class PhaseTwoValues
    {
        public float AttackRate;

        public float WaitBetween6Dice;

        public Color DeactiveDiceColor;


        [Header("Dice 2")]
        public Sound Dice2JumpSound;
        public float ScanAreaScale;
        public float JumpDuration;
        public float JumpCooldown;
        public float FollowSpeed;
        
        [Header("Dice 3")]

        
        [Header("Dice 4")]
        public float BeamTrackSpeed;
        public float BeamInitialDelay;
        public float BeamFireDuration;
        public float BeamCoolDownDuration;
        
        [Header("Dice 5")]
        public RepeatValues Dice5AttackValues = new(1f, 5, 0.8f, 1.5f);
        public float Dice5BulletSpeed = 1;
        
        [TableList(ShowIndexLabels = false)]
        public List<DiceValues> DiceBehaviours;

        [Serializable]
        public class DiceValues
        {
            [PreviewField][TableColumnWidth(50, true)][BoxGroup("Dice")][HideLabel]
            public Texture Icon; 
            [BoxGroup("Dice")]
            public Color Color;
            public ProjectileInfo DiceInfo;
        }
    }
    
    [Serializable]
    public class PhaseThreeValues
    {
        public float AttackRate;
    }

    [Serializable]
    public enum PhaseOneAttacks
    {
        Rock,
        HitGround,
        Missile
    }
}
