using System;
using System.Collections;
using System.Collections.Generic;
using CharImplementations.PlayerImplementation;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using CombatManagement.ProjectileManagement.Implementations;
using DG.Tweening;
using Events;
using GameStages.BossOne.HitArea;
using GameStages.EventImplementations;
using SettingImplementations;
using Sirenix.OdinInspector;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using Utility;

namespace CharImplementations.EnemyImplementations.BossImplementations
{
    public class SkinnyDugan : Boss
    {
        [SerializeField] 
        private Transform m_ProjectileOriginTransform;

        [SerializeField]
        private Transform m_Hand;

        [SerializeField] 
        private Transform m_HandParent;


        [SerializeField] 
        private List<HitArea> HitAreas;
        
        [SerializeField]
        private Color m_VisorPhase1Color;
        [SerializeField]
        private Color m_VisorPhase2Color;
        [SerializeField] 
        private Color m_VisorPhase3Color;

        [SerializeField] 
        private LineRenderer m_Hook;
        
        [SerializeField]
        private HitArea m_ArmHit;

        public static Transform DuganTransform;
        
        private static readonly int m_Color0 = Shader.PropertyToID("_Color0");
        private BossOneSettings m_Settings => BossOneSettings.Get();
        private List<SpreadProjectileInfo> ThrowBundleInfo => m_Settings.PhaseOneValues.ThrowBundleInfo;
        private SpreadProjectileInfo RockSettings => m_Settings.PhaseOneValues.ThrowBundleInfo[m_RockMovementIndex % ThrowBundleInfo.Count];
        private float m_AttackRate => GetAttackRate();
        
        private PhaseOneAttacks m_PhaseOneCurrentAttackType = PhaseOneAttacks.Rock;
        
        private SpecialCondition<float> m_PhaseChangeCondition => new(m_Settings.BossHealthLimitForStage2, 
            () => SendChangePhaseEvent(2), ComparisionMethod.LesserOnce);

        private readonly Vector3 m_ForwardRot = new(1,0,1);
        private static Vector3 m_CurrentPlayerPosition => Player.PlayerTransform.position;
        
        private Material m_VisorMat;
        private Material m_ClonedVisorMat;
        
        private Conditional m_ResetLevelConditional;

        private TimedAction m_OnGetDamageAction;
        
        private Quaternion m_InitRot;
        
        private BoolState m_ArmHitOn;

        private int m_RockMovementIndex;
        private int m_ActiveDiceCount;
        private int m_CurrentPhase = 1;
        private int m_HitAreaIndex = 0;       
        private int AngleMultiplier;
        private int m_ThrowSpreadRockCount;

        private float m_LastHitTime;

        private float GetAttackRate()
        {
            if (m_CurrentPhase != 1)
            {
                if (m_CurrentPhase == 2)
                {
                    return m_ActiveDiceCount % 6 == 0
                        ? m_Settings.PhaseTwoValues.WaitBetween6Dice
                        : m_Settings.GetAttackRate(2);
                }
                
                return m_Settings.GetAttackRate(m_CurrentPhase);

            }
            
            if (m_PhaseOneCurrentAttackType == PhaseOneAttacks.HitGround)
            {
                return m_Settings.PhaseOneValues.HitRate;
            }
            
            if (RockSettings._AimMode != AimMode.Angle)
            {
                return m_ThrowSpreadRockCount < RockSettings.ThrowCount ? RockSettings.TimeBetweenThrow : m_Settings.GetAttackRate(m_CurrentPhase);
            }

            return RockSettings.ThrowCount * RockSettings.TimeBetweenThrow + m_Settings.GetAttackRate(m_CurrentPhase);


            // return m_CurrentPhase switch
            // {
            //     2 when m_ActiveDiceCount % 6 == 0 => m_Settings.PhaseTwoValues.WaitBetween6Dice,
            //     1 when m_ThroweSpreadRockCount != RockSettings.PespeseRockThrowCount => RockSettings.AttackRate,
            //     _ => m_Settings.GetAttackRate(m_CurrentPhase)
            // };
        }

        private void Awake()
        {
            GEM.AddListener<PhaseChangeEvent>(OnPhaseChangeEvent);
            GEM.AddListener<PlayerDeadEvent>(OnPlayerDeadEvent);
            GEM.AddListener<ResetLevelEvent>(OnResetLevel);
            
            Initialize();
            
            OnLevelStarted();

            SetupMaterials();
            SetupActions();
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<PhaseChangeEvent>(OnPhaseChangeEvent);
            GEM.RemoveListener<PlayerDeadEvent>(OnPlayerDeadEvent);
            GEM.RemoveListener<ResetLevelEvent>(OnResetLevel);

            m_ResetLevelConditional?.Cancel();
            m_OnGetDamageAction?.Pause();
        }

        private void SetupMaterials()
        {
            var colorMat = new Material(GetComponentInChildren<Renderer>().materials[1]);
            GetComponentInChildren<Renderer>().materials[1] = colorMat;
            m_ClonedVisorMat = GetComponentInChildren<Renderer>().materials[1];
            m_ClonedVisorMat.SetColor(m_Color0, m_VisorPhase1Color);
        }

        private void SetupActions()
        {
            m_ArmHitOn = new BoolState(false);
            m_OnGetDamageAction = new TimedAction(OnGetDamage, 0, 0.1f);
        }
        
        private void OnPlayerDeadEvent(PlayerDeadEvent evt)
        {
            FireAction = null;

            m_OnGetDamageAction?.Pause();
            m_ResetLevelConditional?.Cancel();

            AnimController.SetBool("Aim", false);
        }
        
        private void OnLevelStarted()
        {
            this.gameObject.SetActive(true);
            m_ActiveDiceCount = 0;
            
            m_ResetLevelConditional?.Cancel();
            m_OnGetDamageAction?.Pause();
            
            FireAction = null;
            
            Immune.Change(true);
            
            m_Health.AddCondition(m_PhaseChangeCondition);
        }

        protected override void OnResetLevel(ResetLevelEvent evt)
        {
            base.OnResetLevel(evt);

            OnLevelStarted();
        }
        
        private void Update()
        {
            FireAction?.Update(Time.deltaTime);

            DuganTransform = transform;

            m_LastHitTime += Time.deltaTime;
            m_LastHitTime = Mathf.Min(0.1f,m_LastHitTime);
        }

        public override void PhaseOne()
        {
            base.PhaseOne();
            
            AnimController.SetBool("Aim", true);

            var att = m_Settings.PhaseOneValues.PhaseOneAttack;
            m_PhaseOneCurrentAttackType = att;
            
            switch (m_PhaseOneCurrentAttackType)
            {
                case PhaseOneAttacks.Rock:
                {
                    using var evt2 = SoundPlayEvent.Get(m_Settings.BossThrowRockSound);
                    evt2.SendGlobal();
                    
                    AnimController.Toggle("Throw", true);
                    
                    if (RockSettings._AimMode == AimMode.Angle)
                    {
                        StartCoroutine(ThrowRock());
                    }
                    else
                    {
                        ThrowMultipleRock();
                    }
                }
                    break;
                case PhaseOneAttacks.HitGround:
                {
                    HitGround();
                }
                    break;
                case PhaseOneAttacks.Missile:
                {
                    FireMissile();
                }
                    break;
            }
            
            FireAction.Period = m_AttackRate;
        }

        public override void PhaseTwo()
        {
            base.PhaseTwo();

            ThrowDice();
            
            AnimController.Toggle("Throw", true);
        }

        [SerializeField] public bool ons;
        public override void PhaseThree()
        {
            base.PhaseThree();
            // m_AttackCount++;
            //
            // if (m_AttackCount > 8)
            // {
            //     m_AttackCount = 0;
            // }

            if (ons)
            {
                Pull();
            }
            else
            {
                ShakeArm();
            }
            //
            // if (m_AttackCount > 4)
            //Pull();
            //else
              //  ShakeArm();
            
        }

        private void SendChangePhaseEvent(int index)
        {
            using var evt = PhaseChangeEvent.Get(index);
            evt.SendGlobal();
        }

        private void OnPhaseChangeEvent(PhaseChangeEvent evt)
        {
            ChangePhase(evt.PhaseIndex);
        }
        
        [Button]
        public override void ChangePhase(int phaseIndex)
        {
            m_CurrentPhase = phaseIndex;
            
            using var evt2 = SoundPlayEvent.Get(m_Settings.PhaseChangeSound);
            evt2.SendGlobal();

            var c = m_CurrentPhase switch
            {
                1 => m_VisorPhase1Color,
                2 => m_VisorPhase2Color,
                3 => m_VisorPhase3Color,
            };

            m_ClonedVisorMat.SetColor(m_Color0, c);
            
            base.ChangePhase(phaseIndex);
            
            Immune.Change(phaseIndex == 2);

            AnimController.Trigger("PhaseChange1");

            FireAction = phaseIndex switch
            {
                1 => new TimedAction(PhaseOne, 2, m_AttackRate),
                2 => new TimedAction(PhaseTwo, 1, 2),
                3 =>  new TimedAction(PhaseThree, 2, 3),
            };
        }
        
        public override void Die()
        {
            base.Die();

            m_ResetLevelConditional = Conditional.Wait(3f).Do(SendResetLevelEvent);
        }

        private void SendResetLevelEvent()
        {
            using var resetLevelEvent = ResetLevelEvent.Get();
            resetLevelEvent.SendGlobal();
        }

        private void ThrowRock(Vector3 targetPos)
        {
            AnimController.Trigger("Shoot");
            
            //transform.LookAt(Player.PlayerTransform.position);

            using var evt = GetProjectileEvent.Get(ProjectileType.Rock);
            evt.SendGlobal();
            var layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Obstacle");

            var rock = evt.Projectile.GetComponent<Rock>();
            rock.Initialize(m_ProjectileOriginTransform.position, targetPos, RockSettings.ProjectileInfo.Damage, CharType.Player,
                layerMask, "ProjectileEnemy");
            
            var movement = ThrowBundleInfo[m_RockMovementIndex % ThrowBundleInfo.Count];
            rock.InitializeProjectileInfoParameters(movement.ProjectileInfo, targetPos);
        }
        
        //rotate a point around another point by a given angle
        private Vector3 RotatePointAroundPivot2(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            var dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }


        private IEnumerator ThrowRock()
        {
            AngleMultiplier = RockSettings.CircularThrow ? AngleMultiplier = RockSettings.ThrowCount/2 : 1;
            for (int j = 1; j < RockSettings.ThrowCount; j++)
            {
                var m = j <=  RockSettings.ThrowCount/2 ? -1 : 1;
                
                ThrowRock(
                    RotatePointAroundPivot(-transform.right.WithY(0) * RockSettings.ProjectileInfo.Range,
                        transform.position.WithY(0),
                        (m_ForwardRot.WithX(m) * RockSettings.SpreadAngle * AngleMultiplier)));

                if (RockSettings.CircularThrow)
                {
                    AngleMultiplier--;
                    if (AngleMultiplier == 0)
                    {
                        AngleMultiplier = RockSettings.ThrowCount/2;
                    }
                }
                else
                {
                    AngleMultiplier++;
                    if (AngleMultiplier > RockSettings.ThrowCount/2)
                    {
                        AngleMultiplier = 1;
                    }
                }
                
                yield return new WaitForSeconds(RockSettings.TimeBetweenThrow);

            }

            m_ThrowSpreadRockCount++;
            
            if (m_ThrowSpreadRockCount >= RockSettings.ThrowCount)
            {
                m_ThrowSpreadRockCount = 0;
                m_RockMovementIndex++;
                while (!RockSettings.Execute)
                {
                    m_RockMovementIndex++;
                }
            }
        }
        
        private void ThrowMultipleRock()
        {
            var z = 1;
            for (int j = 1; j < RockSettings.CountAtOnce + 1; j++)
            {
                var m = j <= Mathf.Round(RockSettings.CountAtOnce)/ 2 ? -1 : 1;

                if (RockSettings._AimMode == AimMode.Front)
                {
                    
                    ThrowRock(RotatePointAroundPivot2(transform.forward * 10,
                        transform.position.WithY(3.15f),(RockSettings.SpreadAngle  * z) * m_ForwardRot.WithX(m)));
                    
                }
                else if (RockSettings._AimMode == AimMode.Player)
                {
                    
                    ThrowRock(RotatePointAroundPivot2(Player.PlayerTransform.position,
                        transform.position.WithY(3.15f),(RockSettings.SpreadAngle  * z) * m_ForwardRot.WithX(m)));
                }
                
                z++;
                if (z > Mathf.Round(RockSettings.CountAtOnce)/ 2)
                {
                    z = 1;
                }
            }

            m_ThrowSpreadRockCount++;

            if (m_ThrowSpreadRockCount >= RockSettings.ThrowCount)
            {
                m_ThrowSpreadRockCount = 0;

                
                m_RockMovementIndex++;
                while (!RockSettings.Execute)
                {
                    m_RockMovementIndex++;
                }
                return;
            }

            if (m_Settings.PhaseOneValues.Mayhem)
            {
                m_RockMovementIndex++;
                while (!RockSettings.Execute)
                {
                    m_RockMovementIndex++;
                }
            }

        }
        

        private void HitGround()
        {
            using var evt2 = SoundPlayEvent.Get(m_Settings.BossHitGorundSound);
            evt2.SendGlobal();

            AnimController.Toggle("HitGround", true);
            m_HitAreaIndex++;
            var area = HitAreas[m_HitAreaIndex % HitAreas.Count];

            area.ActivateInstantDamage(CharType.Player, 5, m_Settings.PhaseOneValues.HitDur,
                m_Settings.PhaseOneValues.HitIndicatorDur);
        }

        private void ElectrifiyGrounds()
        {
            using var evt2 = SoundPlayEvent.Get(m_Settings.BossHitGorundSound);
            evt2.SendGlobal();

            AnimController.Toggle("HitGround", true);
            
            for (int i = 0; i < HitAreas.Count; i++)
            {
                HitAreas[i].ActivateBuildUpDamage(CharType.Player, 5, 4f, 1f);
            }
        }

        private void FireMissile()
        {
            
        }

        private void ThrowDice()
        {
            using var evt2 = SoundPlayEvent.Get(m_Settings.BossThrowDiceSound);
            evt2.SendGlobal();
            
            AnimController.Toggle("Dice", 1f);

            if (m_ActiveDiceCount == 32)
            {
                FireAction = null;
                return;
            }
            
            m_ActiveDiceCount++;

            FireAction.Period = m_AttackRate;

            using var evt = GetProjectileEvent.Get(ProjectileType.DuganDice1);
            evt.SendGlobal();
            var pr = evt.Projectile;

            var layermask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("ProjectilePlayer");

            var pos = HitAreas[m_HitAreaIndex % HitAreas.Count].transform.position;
            m_HitAreaIndex++;

            pr.Initialize(m_ProjectileOriginTransform.position, Player.PlayerTransform.position, 10, CharType.Player,
                layermask, "ProjectileEnemy");
            //pr.GetComponent<MergeableProjectile>().SetId(1);

            pr.Mover.SnuckJumpMovement(pos, 3, 1, 0.5f);
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }
        
        private void ShakeArm()
        {
            m_ArmHitOn.Toggle(true, 0.5f);
            AnimController.Trigger("ArmSwing");
        }


        private void OnTriggerEnter(Collider other)
        {
            if(!m_ArmHitOn.CurrentValue)
                return;
            
            if (other.TryGetComponent<Character>(out var chara))
            {
                if (chara.GetCharType() == CharType.Player)
                {
                    chara.OnImpact(CharType.Player, -10);
                }
            }
        }
        private void Pull()
        {
            // m_Hook.enabled = true;
            // m_Hook.SetPosition(0, Vector3.zero);
            // m_Hook.positionCount = 2;
            // m_Hook.SetPosition(1,Vector3.zero);
            var targetPos = Player.PlayerTransform.position;
            
            Conditional.For(0.2f).Do(() =>
            {
                var p = Vector3.Lerp(transform.position, targetPos, 1 * Time.deltaTime);
                //m_Hook.SetPosition(1,new Vector3(-p.magnitude,0,0));

                m_Hand.transform.position = p;
                m_Hand.SetParent(null);
            }).OnComplete(() =>
            {
                var dir = -(transform.position.WithY(Player.GlobalProjectileY) - targetPos.WithY(Player.GlobalProjectileY) );
                if(Physics.Raycast(transform.position.WithY(Player.GlobalProjectileY) , dir,out var hit,dir.magnitude))
                {
                    Debug.Log("Hit smthn");
                    if (hit.transform.TryGetComponent<Player>(out var pl))
                    {
                        Debug.Log("Hit player");

                        pl.GetSnared(1);
                        pl.transform.DOMove( (transform.position - (dir/3)).WithY(Player.PlayerTransform.position.y), 0.51f);
                    }
                    
                    m_Hand.SetParent(m_HandParent);
                    m_Hand.transform.localPosition = Vector3.zero;
                    
                }
            });
        }

        private void JumpAndAOE()
        {
        }

        private void ShrinkMap()
        {
        }

        private void OnGetDamage()
        {
            m_ClonedVisorMat.DOKill();
            m_ClonedVisorMat.DOColor(Color.red, m_Color0, 0.05f).SetLoops(3).OnComplete(() =>
            {
                var c = m_CurrentPhase switch
                {
                    1 => m_VisorPhase1Color,
                    2 => m_VisorPhase2Color,
                    3 => m_VisorPhase3Color,
                };

                m_ClonedVisorMat.SetColor(m_Color0, c);
            });

            using var evt2 = SoundPlayEvent.Get(m_Settings.BossGetDamageSound);
            evt2.SendGlobal();
            
            m_LastHitTime = 0f;
        }

        public override void ChangeHealth(float amount)
        {
            m_OnGetDamageAction.Update(m_LastHitTime);
            
            base.ChangeHealth(amount);
        }
    }
}