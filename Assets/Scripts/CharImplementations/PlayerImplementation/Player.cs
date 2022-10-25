using System.Collections.Generic;
using System.Linq;
using CharImplementations.Data;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.EventImplementations;
using CombatManagement.WeaponImplementation;
using DG.Tweening;
using Events;
using Fate;
using Fate.EventImplementations;
using GameStages.EventImplementations;
using InputManagement.EventImplementations;
using Misc;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.UI;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using Utility;
using Utility.Extensions;
using InputManager = InputManagement.InputManager;

namespace CharImplementations.PlayerImplementation
{
    public class Player : Character
    {
        [SerializeField] 
        private List<WeaponBehaviour> m_ActiveWeapons;

        [SerializeField]
        private Material m_ImmuneMaterial;
        
        public static Transform PlayerTransform;

        public static float GlobalProjectileY = 6f;

        public static Transform AimAssistTarget;
        
        public bool InvertedControls;
        
        public override CharType GetCharType() => CharType.Player;
        public static Vector3 MoveDir => InputManager.Instance.MoveDirY + InputManager.Instance.MoveDirX;
        private CharacterController CharacterController => GetComponent<CharacterController>();
        private WeaponBehaviour ActiveWeaponObject => m_ActiveWeapons[m_WeaponIndex % m_ActiveWeapons.Count];
        private WeaponData WeaponData => ActiveWeaponObject.WeaponData;
        private Vector3 m_OriginPos => transform.position.WithY(transform.position.y);
        private float DodgeDistance => m_PlayerInfo.DodgeDistance;
        private float DodgeDur => m_PlayerInfo.DodgeDuration;
        private float DodgeImmuneDur => m_PlayerInfo.DodgeImmuneDuration;
        private PlayerInfo m_PlayerInfo => (PlayerInfo) m_CharInfo;

        public static float PlayerHealth;

        private Color m_ImmuneColor;

        private bool m_Firing;
        private bool m_Dead;
        private bool m_Aiming;       
        private bool m_AimHolding;
        
        private int m_WeaponIndex;
        private int m_DodgeCount;

        private float m_GroundCheckGap = 2;
        private float m_LastHitTime;
        private float m_FireTimer;
        private float m_RotateToAimTime = 0.12f;

        private IntState m_AttackRemaining;

        private BoolState m_Moving;
        private BoolState m_IsDodging;
        private BoolState m_CanDodge;

        private Vector3 m_InitPos;
        private Vector3 m_TargetPos;
        
        private TimedAction m_OnDamageAction;        
        private TimedAction m_WalkSoundAction;

        
        private Conditional m_FireAnimDisableConditional;
        private Conditional m_ResetLevelConditional;
        
        private bool m_AimAssistEnabled;
        private Transform m_AimAssitTarget;

        private Tween m_RotateToAimTween;

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            GEM.AddListener<SlideEvent>(TryDodge);
            GEM.AddListener<FireButtonReleasedEvent>(OnFireReleasedEvent);
            GEM.AddListener<FireButtonPressedEvent>(OnFirePressedEvent);
            GEM.AddListener<FireButtonRepeatingEvent>(OnFireContinuousEvent);
            GEM.AddListener<GetPlayerTransformEvent>(OnRequestTransformEvent, Priority.Critical);
            GEM.AddListener<WeaponWheelEvent>(OnWeaponWheel);
            GEM.AddListener<GetPlayerEvent>(OnGetPlayer);
            GEM.AddListener<RightMousePressedEvent>(Aim);
            GEM.AddListener<RightMouseButtonReleasedEvent>(AimReleased);
            GEM.AddListener<FateAttackActivatedEvent>(OnFateActivated);
            GEM.AddListener<ResetLevelEvent>(OnResetLevel);
            GEM.AddListener<ActivatePlayerObjectPlacerEvent>(OnPlacementMode);
            GEM.AddListener<ActivatePlayerProjectileThrowerEvent>(OnPlacementMode);
            
            m_InitPos = transform.position;
            
            SetupWeaponMagazines();
            SetupStates();
            SetupActions();
            SetupMaterials();
            
            base.Initialize();

            m_Health.OnChange = SendHealthChangeEvent;
            
            PlayerHealth = m_Health.DefaultValue;
            m_WalkSoundAction = new TimedAction(WalkSoundAction, 0, m_PlayerInfo.WalkSoundPeriod);

            SendHealthChangeEvent();

            PlayerTransform = transform;
            using var evt = PlayerInitializedEvent.Get().SendGlobal();
        }
        
        private void SetupMaterials()
        {
            m_ImmuneMaterial.color = m_ImmuneMaterial.color.WithAlpha(0);
            m_ImmuneColor = m_ImmuneMaterial.color.WithAlpha(1);
        }

        private void SetupActions()
        {
            FireAction = new TimedAction(Attack, 0, WeaponData.FireRate);
            m_OnDamageAction = new TimedAction(OnGetDamage, 0, 0.2f);
        }

        private void SetupWeaponMagazines()
        {
            for (int i = 0; i < m_ActiveWeapons.Count; i++)
            {
                m_ActiveWeapons[i].WeaponData.ResetMagazine();
            }
        }

        private void SetupStates()
        {
            m_IsDodging = new BoolState();
            m_Moving = new BoolState(false, OnMove);
            m_CanDodge = new BoolState(true);
            m_AttackRemaining = new IntState(WeaponData.GetMagazineSize(), null,
                new SpecialCondition<int>(0, OnReload, ComparisionMethod.Equal));
        }


        private void OnDestroy()
        {
            GEM.RemoveListener<SlideEvent>(TryDodge);
            GEM.RemoveListener<FireButtonReleasedEvent>(OnFireReleasedEvent);
            GEM.RemoveListener<FireButtonPressedEvent>(OnFirePressedEvent);
            GEM.RemoveListener<FireButtonRepeatingEvent>(OnFireContinuousEvent);
            GEM.RemoveListener<GetPlayerTransformEvent>(OnRequestTransformEvent);
            GEM.RemoveListener<GetPlayerTransformEvent>(OnRequestTransformEvent);
            GEM.RemoveListener<WeaponWheelEvent>(OnWeaponWheel);
            GEM.RemoveListener<GetPlayerEvent>(OnGetPlayer);
            GEM.RemoveListener<RightMousePressedEvent>(Aim);
            GEM.RemoveListener<RightMouseButtonReleasedEvent>(AimReleased);
            GEM.RemoveListener<FateAttackActivatedEvent>(OnFateActivated);
            GEM.RemoveListener<ActivatePlayerObjectPlacerEvent>(OnPlacementMode);
            GEM.RemoveListener<ActivatePlayerProjectileThrowerEvent>(OnPlacementMode);
            
            m_FireAnimDisableConditional?.Cancel();
            m_ResetLevelConditional?.Cancel();
        }

        public override float MoveSpeed
        {
            get
            {
                if(m_Aiming)
                    return m_CharInfo.MovementSpeed * (1 - m_PlayerInfo.SpeedDecreaseOnAim);
                
                return m_CharInfo.MovementSpeed;
            }
            set => m_CharInfo.MovementSpeed = value;
        }

        private void OnResetLevel()
        {
            for (int i = 0; i < m_ActiveWeapons.Count; i++)
            {
                m_ActiveWeapons[i].WeaponData.ResetMagazine();
            }

            m_Dead = false;
            m_DodgeCount = 0;

            transform.position = m_InitPos;

            AnimController.SetBool("Death", false);

            m_ImmuneMaterial.color = m_ImmuneMaterial.color.WithAlpha(0);

            m_CanDodge.Change(true);
            m_IsDodging.Change(false);
            m_Moving.Change(false);
            m_AttackRemaining.Change(WeaponData.GetMagazineSize());

            base.OnResetLevel(null);

            SendHealthChangeEvent();
        }

        private void WalkSoundAction()
        {
            var evt = SoundPlayEvent.Get(m_PlayerInfo.WalkSound);
            evt.SendGlobal();
        }

        private void FixedUpdate()
        {
            PlayerTransform = transform;
            //GlobalProjectileY = 5;

            m_FireTimer += Time.deltaTime;

            m_LastHitTime += Time.deltaTime;
            m_LastHitTime = Mathf.Min(0.2f, m_LastHitTime);

            Move();
        }

        
        public override void Move()
        {
            if (!CanMove.CurrentValue)
                return;

            if (m_IsDodging.CurrentValue)
                return;
            
            var moveDir = MoveDir * (InvertedControls ? -1 : 1);

            if (!CheckForGround(transform.position + moveDir.normalized * m_GroundCheckGap, true, out _))
            {
                //Debug.Log("no ground");
                m_Moving.Change(false);
                
                
                if (!m_Firing && !m_AimHolding)
                {
                    transform.forward = moveDir;
                }
                
                return;
            }

            if (moveDir == Vector3.zero)
            {
                m_Moving.Change(false);
                m_Rigidbody.velocity = moveDir;
                return;
            }

            base.Move();

            m_Moving.Change(true);
            
            
            m_WalkSoundAction.Update(Time.deltaTime);
            m_WalkSoundAction.Period = m_PlayerInfo.WalkSoundPeriod;

            CharacterController.SimpleMove((moveDir.normalized * (MoveSpeed * Time.deltaTime)));

            if (!m_Firing && !m_AimHolding)
            {
                transform.forward = moveDir;
            }
        }

        private void Aim(RightMousePressedEvent evt)
        {
            m_Aiming = true;
            m_AimHolding = true;
            AnimController.SetBool("Aim", true);
        }
        
        private void AimReleased(RightMouseButtonReleasedEvent evt)
        {
            m_Aiming = false;
        }

        private void OnMove()
        {
            AnimController.SetBool("Walking", !m_Moving.CurrentValue);
        }
        
        //this function is executed only in defined interval to prevent cascade
        private void OnGetDamage()
        {
            using var soundEvet = SoundPlayEvent.Get(m_PlayerInfo.DamageSound);
            soundEvet.SendGlobal();
            
            AnimController.Toggle("Damage", true);
            GetStunned(m_PlayerInfo.DamageStunDuration);
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);

            if (Immune.CurrentValue)
            {
                using var soundEvet = SoundPlayEvent.Get(m_PlayerInfo.DodgeSound);
                soundEvet.SendGlobal();
                return;
            }

            m_OnDamageAction.Update(m_LastHitTime);
            m_LastHitTime = 0f;

            m_ImmuneMaterial.DOColor(Color.red, 0.1f).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
            {
                m_ImmuneMaterial.color = m_ImmuneColor.WithAlpha(0);
            });
        }

        protected override void OnResetLevel(ResetLevelEvent evt)
        {
            OnResetLevel();
        }

        public override void Die()
        {
            if (m_Dead)
                return;

            m_Dead = true;

            base.Die();
            
            using var soundEvet = SoundPlayEvent.Get(m_PlayerInfo.DeathSound);
            soundEvet.SendGlobal();
            
            FadeInOut.Instance.DoTransition(() =>
            {
                using var evt = PlayerDeadEvent.Get();
                evt.SendGlobal();
            }, 0.41f, Color.black, 1f);

        }
        
        

        public override void Attack()
        {
            if(FateManager.FateAttackInProgress)
                return;
            
            FireAction.Period = WeaponData.FireRate;

            if (!CanAttack.CurrentValue)
                return;

            WeaponData.DecreaseMagazineByOne();
            m_AttackRemaining.DecreaseOne();

            base.Attack();

            //AnimController.Toggle("Shoot", 0.7f);

            if (AimAssistTarget != null)
            {
                m_TargetPos = AimAssistTarget.position;
                m_Aiming = true;
            }
            
            m_Firing = true;

            m_RotateToAimTween?.Kill();
            m_RotateToAimTween = transform.DOLookAt(m_TargetPos.WithY(transform.position.y), m_RotateToAimTime);
            
            //transform.forward = -(m_OriginPos - m_TargetPos);

            ActiveWeaponObject.Attack(m_OriginPos, m_TargetPos, CharType.Enemy, m_Aiming);
        }
        
        private void OnReload()
        {
            AnimController.Trigger("Reloading");

            CanAttack.Toggle(false, WeaponData.ReloadTime, ResetMagazine);
        }

        private void ResetMagazine()
        {
            m_AttackRemaining.Change(WeaponData.GetMagazineSize());
            WeaponData.ResetMagazine();
        }

        private Tween m_WeaponSwitchTween;
        
        private void OnWeaponWheel(WeaponWheelEvent evt)
        {
            ActiveWeaponObject.gameObject.SetActive(false);

            var oldGunSize = WeaponData.IsBigGun() ? 100 : 3;
            m_WeaponIndex++;

            FireAction.Period = WeaponData.FireRate;

            ActiveWeaponObject.gameObject.SetActive(true);

            m_WeaponSwitchTween.Kill();
            m_WeaponSwitchTween = DOTween.To(val => AnimController.SetFloat("GunWeight", val), oldGunSize, WeaponData.IsBigGun() ? 100 : 3, 0.1f);

            m_AttackRemaining.DefaultValue = WeaponData.GetMagazineSize();
            m_AttackRemaining.Change(WeaponData.RemainingMagazine);

            using var wEvt = WeaponChangeEvent.Get(WeaponData);
            wEvt.SendGlobal();
        }

        private void OnFirePressedEvent(FireButtonPressedEvent evt)
        {
            AnimController.Trigger("Shoot");
            AnimController.SetBool("Aim", true);
            
            if (WeaponData.GetWeaponType() == WeaponType.Charge)
                return;

            m_TargetPos = m_AimAssistEnabled ? m_AimAssitTarget.position.WithY(transform.position.y) : evt.m_AimLoc.WithY(transform.position.y);

            if (m_FireTimer > WeaponData.FireRate)
            {
                FireAction.Update(WeaponData.FireRate);
                m_FireTimer = 0f;
            }
        }

        private void OnFireReleasedEvent(FireButtonReleasedEvent evt)
        {
            m_Firing = false;

            // TODO more elegant way to do this
            m_FireAnimDisableConditional?.Cancel();
            m_FireAnimDisableConditional = Conditional.Wait(0.6f).Do(() =>
            {
                AnimController.SetBool("Aim", false);
                m_AimHolding = false;
            });

            if (WeaponData.GetWeaponType() != WeaponType.Charge)
            {
                ActiveWeaponObject.Disable();
                return;
            }

            m_TargetPos = m_AimAssistEnabled ? m_AimAssitTarget.position.WithY(transform.position.y) : evt.m_AimLoc.WithY(transform.position.y);

            FireAction.Update(WeaponData.FireRate);
        }


        private void OnFireContinuousEvent(FireButtonRepeatingEvent evt)
        {
            AnimController.Toggle("Shoot", true);

            m_TargetPos = evt.m_AimLoc.WithY(transform.position.y);

            if (m_FireTimer > WeaponData.AutomaticFireRate())
            {
                FireAction.Update(WeaponData.FireRate);

                m_FireTimer = 0;
            }
        }
        private void OnDodgeFinish()
        {
            transform.position = transform.position.WithY(3.15f);
        }

        public override void GetImmune(float dur)
        {
            m_ImmuneMaterial.color = m_ImmuneColor.WithAlpha(1);

            m_ImmuneMaterial.DOColor(m_ImmuneColor.WithAlpha(0), dur);

            Immune.Toggle(true, dur);
        }

        private void Dodge(Vector3 pos, Particle particle) 
        {
            //var dodgeDir = MoveDir == Vector3.zero ? transform.forward : MoveDir;

            m_IsDodging.Toggle(true, DodgeDur,OnDodgeFinish);
            
            transform.position = pos.WithY(transform.position.y);
            
            particle.MoveToPos(transform.position.WithY(-2));
            
            GetImmune(DodgeImmuneDur);
        }

        private void TryDodge(SlideEvent evt)
        {
            if (m_IsDodging.CurrentValue)
                return;

            if (!m_CanDodge.CurrentValue || !CanMove.CurrentValue)
                return;

            if (m_DodgeCount >= m_PlayerInfo.DodgeCount)
            {
                m_CanDodge.Toggle(false, m_PlayerInfo.DodgeCooldownDuration);
                m_DodgeCount = 0;
                
                return;
            }
            
            var pos = transform.position +  (transform.forward * DodgeDistance);
            
            //Ray ray = new Ray(transform.position, transform.forward);
            //Debug.DrawRay(ray.origin,ray.direction * DodgeDistance, Color.green, 1f);
            //Conditional.For(1).Do(() => { DebugExtensions.DrawPoint(pos, 1, Color.green); });
            
            var results = Physics.OverlapSphere(pos, 1, 1 << LayerMask.NameToLayer("Obstacle") 
                                                        | 1 << LayerMask.NameToLayer("NonHit")
                                                        | 1 << LayerMask.NameToLayer("Enemy"));
            
            using var evtPart = ParticleSpawnEvent.Get(ParticleType.Dodge);
            evtPart.SendGlobal();
            var particle = evtPart.Particle;
            particle.Initialize(transform.position.WithY(-2));
            particle.transform.forward = transform.forward;
            
            AnimController.Toggle("Dashing", true);

            //AnimController.SetSpeedMultiplier("DodgeSpeed", 1/DodgeDur);

            if (!results.Any()) {

                if (CheckForGround(pos, false, out var newPos))
                {
                    Dodge(newPos, particle);
                }
                return;
            }

            var limitPos = pos + (transform.forward * m_PlayerInfo.DodgeLimit);
            
            // Debug.DrawRay(ray.origin,ray.direction * m_PlayerInfo.DodgeLimit, Color.magenta, 1f);
            // Conditional.For(1).Do(() => { DebugExtensions.DrawPoint(limitPos, 2, Color.magenta); });

            for (int i = 0; i < results.Length; i++) {
                
                var freePoint = results[i].bounds.ClosestPoint(limitPos);
                var distance = transform.position - freePoint;
                var limitDistance = transform.position - limitPos;
                
                // Conditional.For(1).Do(() => { DebugExtensions.DrawPoint(freePoint, 1, Color.red); });
                // Debug.Log(distance.magnitude + "   limit " + limitDistance.magnitude);

                if (!(distance.magnitude < limitDistance.magnitude)) 
                    continue;
                
                if (CheckForGround(pos, true, out var newPos))
                {
                    Dodge(newPos, particle);
                }
                return;
                // else {
                //     Debug.Log("no dodge bc of " + results[i].transform.name);
                //
                //     var availablePoint = results[i].bounds.ClosestPoint(transform.position);
                //     Conditional.For(1).Do(() => {
                //         DebugExtensions.DrawPoint(availablePoint, 1, Color.blue);
                //     });
                // }
            }
            
        }

        private bool CheckForGround(Vector3 pos, bool ifOnLimit, out Vector3 finPos)
        {
            finPos = pos;
            
            var result = Physics.Raycast(pos, Vector3.down * 10,5,  1 << LayerMask.NameToLayer("Level"));
            if (!result && !ifOnLimit)
            {
                var limitPos = pos + (transform.forward * m_PlayerInfo.DodgeLimit);

                result = Physics.Raycast(limitPos, Vector3.down * 10,out var hit, 5, 1 << LayerMask.NameToLayer("Level"));

                if (result)
                {
                    finPos = hit.collider.bounds.ClosestPoint(transform.position);
                    //var s = finPos;
                    //Conditional.For(1).Do(() => { DebugExtensions.DrawPoint(s, 2, Color.magenta); });
                }
            }
            return result;
        }
        
        // TODO: definitely need to refactor this
        private void OnPlacementMode(object arg)
        {
            m_CanDodge.Change(false);

            if (arg is ActivatePlayerObjectPlacerEvent evt1)
            {
                evt1.PlacedPromise.OnResultT += (b, b1) => { m_CanDodge.Change(true); };
            }
            else if (arg is ActivatePlayerProjectileThrowerEvent evt2)
            {
                evt2.ProjectilePromise.OnResultT += (b, b1) => { m_CanDodge.Change(true); };
            }
        }
        
        private void OnFateActivated(FateAttackActivatedEvent evt)
        {
            AnimController.Toggle("Fate", true);
            AnimController.SetBool("FateOnSelf", !evt.ActiveModule.IsPlaceable);
        }

        private void OnRequestTransformEvent(GetPlayerTransformEvent evt)
        {
            evt.playerT = m_Transform;
        }

        private void SendHealthChangeEvent()
        {
            using var evt = PlayerHealthChangeEvent.Get(Health);
            evt.SendGlobal();
        }
        
        private void OnGetPlayer(GetPlayerEvent evt)
        {
            evt.Player = this;
        }

        public void ToggleAimAssist(Transform target)
        {
            m_AimAssitTarget = target;
            m_AimAssistEnabled = target != null;
        }
    }
}