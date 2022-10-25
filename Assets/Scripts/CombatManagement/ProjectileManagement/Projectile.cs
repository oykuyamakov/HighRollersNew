using System;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using Events;
using GameStages.EventImplementations;
using Pooling;
using Pooling.EventImplementations;
using Promises;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace CombatManagement.ProjectileManagement
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IPoolable<Projectile>, IProjectile, IDamager
    {
        [SerializeField] private ProjectileType m_ProjectileType;
        public CharType TargetType { get; set; }
        public LayerMask TargetLayers { get; set; }
        public ProjectileType ProjectileType => m_ProjectileType;
        public string SelfLayer { set => gameObject.layer = LayerMask.NameToLayer(value); get => gameObject.layer.ToString(); }
        public float ProjectileDamage { get; set; }
        public float Damage => ProjectileDamage;
        protected float m_YOffset => Player.GlobalProjectileY;
        public Mover Mover => GetComponent<Mover>();

        #region Components

        public Collider SelfCollider() => GetComponent<Collider>();
        public Transform SelfTransform() => GetComponent<Transform>();
        public Rigidbody SelfRigid() => GetComponent<Rigidbody>();
        public NavMeshAgent NavMeshAgent() => GetComponent<NavMeshAgent>();

        #endregion

        public bool HasHealth => Health > 0;

        protected int Health;
        protected bool m_Enabled;

        protected bool m_Moving => Mover.Moving;
        protected int m_DodgeCount;

        protected float m_ScaleMultiplier;

        protected virtual void Awake()
        {
            GEM.AddListener<ResetLevelEvent>(OnReset);
        }

        public Projectile Return(Transform parent)
        {
            SelfTransform().position = Vector3.zero;
            m_Enabled = false;
            Mover.Enabled = false;
            this.gameObject.SetActive(false);
            return this;
        }

        public Projectile Get()
        {
            m_Enabled = true;
            Mover.Enabled = true;
            gameObject.SetActive(true);
            SelfTransform().SetParent(null);
            SelfCollider().enabled = true;

            return this;
        }


        public void InitializeProjectileInfoParameters(ProjectileInfo info, Transform target, bool recursive)
        {
            ProjectileDamage = info.Damage;
            Mover.Target = target;
            m_ScaleMultiplier = info.Scale;
            transform.localScale = Vector3.one * info.Scale;

            if (!info.Recursive)
            {
                Mover.OnDone += this.DisableSelf;
            }
            
            switch (info.MovementType)
            {
                case ProjectileInfo.ProjectileMovementType.RoboticJump:
                {
                    if (recursive)
                    {
                        Mover.JumpToPosFixedRecursive(target, info.JumpPower, info.JumpCount, info.Speed,
                            info.WaitTime);
                    }
                    else
                    {
                        Mover.JumpToPosFixed(target.position, info.JumpPower, info.JumpCount, info.Speed);
                    }
                }
                    break;
                case ProjectileInfo.ProjectileMovementType.OrganicJump:
                    Mover.JumpToPos(target.position, info.Height);
                    break;
                case ProjectileInfo.ProjectileMovementType.LinearMove:
                    if (recursive)
                    {
                        Mover.MoveToTargetRecursive(target, info.Speed, info.WaitTime, info.Range);
                    }
                    else
                    {
                        Mover.MoveToPos(target.position, info.Speed, info.Range);
                    }

                    break;
                case ProjectileInfo.ProjectileMovementType.FollowTarget:
                    Mover.FollowTarget(target, info.Speed, info.Acceleration);
                    break;
                case ProjectileInfo.ProjectileMovementType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitializeProjectileInfoParameters(ProjectileInfo info, Vector3 pos)
        {
            ProjectileDamage = info.Damage;
            transform.localScale = Vector3.one * info.Scale;
            m_ScaleMultiplier = info.Scale;

            // if (info.Rotate)
            // {
            //     Mover.Rotate(info.Speed);
            // }
            if (!info.Recursive)
            {
                Mover.OnDone += this.DisableSelf;
            }

            switch (info.MovementType)
            {
                case ProjectileInfo.ProjectileMovementType.RoboticJump:
                {
                    Mover.JumpToPosFixed(pos, info.JumpPower, info.JumpCount, info.Speed);
                }
                    break;
                case ProjectileInfo.ProjectileMovementType.OrganicJump:
                    Mover.JumpToPos(pos, info.Height);
                    break;
                case ProjectileInfo.ProjectileMovementType.LinearMove:
                    Mover.MoveToPos(pos, info.Speed, info.Range);
                    break;
                case ProjectileInfo.ProjectileMovementType.FollowTarget:
                    Debug.LogError("not supported movement");
                    break;
                case ProjectileInfo.ProjectileMovementType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void Initialize(Vector3 origin, Vector3 targetPos,
            float damage, CharType targetType, LayerMask layersToCollide, string layer)
        {
            ProjectileDamage = damage;
            TargetType = targetType;
            SelfLayer = layer;
            TargetLayers = layersToCollide;

            m_Enabled = true;

            if (this.ProjectileType != ProjectileType.DuganDice3 
                && this.ProjectileType != ProjectileType.DuganDice4
                && this.ProjectileType != ProjectileType.DuganDice5
                && this.ProjectileType != ProjectileType.DuganDice6)
            {
                Mover.OnDone += this.DisableSelf;
                SelfTransform().position = origin.WithY(m_YOffset);

            }

            SelfTransform().forward = origin - targetPos;
        }


        public virtual void GetDamage(int damage)
        {
            damage = -Mathf.Abs(damage);
            Health += damage;

            if (Health < 0)
            {
                DisableSelf();
            }
        }

        public virtual void OnContact(Transform t)
        {
            if (t.TryGetComponent<Projectile>(out var pro))
            {
                if (pro.ProjectileType == m_ProjectileType)
                    return;
            }
            
            // TODO instant fix for rock and roll
            if (t.TryGetComponent<Projectile>(out var pro2))
            {
                if (pro2.ProjectileType == ProjectileType.Rock && m_ProjectileType == ProjectileType.Celeste)
                    return;
                
                if (pro2.ProjectileType == ProjectileType.Rock && m_ProjectileType == ProjectileType.Thomas)
                    return;
            }

            if (t.TryGetComponent<Character>(out var chara))
            {
                if (chara.GetCharType() == TargetType)
                {
                    chara.OnImpact(TargetType, -ProjectileDamage);
                }
                else
                {
                    return;
                }
            }

            if (t.TryGetComponent<IDamager>(out var damager))
            {
                GetDamage((int)damager.Damage);
            }
            
            //DisableSelf();
        }

        public void GetStunned(float duration)
        {
            var promise = Promise<bool>.Create();
            Mover.SnuckMovement(promise);

            Conditional.Wait(duration)
                .Do(() => promise.Complete(true));
        }

        public void GetKnocked(float duration, Vector3 direction)
        {
            Mover.SnuckKnockbackMovement(direction);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if ((TargetLayers & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                OnContact(other.transform);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((TargetLayers & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                OnContact(other.transform);
            }
        }

        public virtual void DisableSelf()
        {
            if (!m_Enabled)
                return;

            m_Enabled = false;

            Mover.DisableSelf();

            using var evt = PoolReleaseEvent<Projectile>.Get(this);
            evt.SendGlobal();
            
            this.GameObject().SetActive(false);
        }

        protected void OnDestroy()
        {
            DisableSelf();
        }

        protected void OnReset(ResetLevelEvent evt)
        {
            DisableSelf();
        }
    }

    public enum ProjectileType
    {
        Celeste,
        Nirvana,
        Thomas,
        Rock,
        Arm,
        Dice,
        Missile,
        Dugan,
        Dice5Projectile,
        Fuse,
        DuganDice1,
        DuganDice2,
        DuganDice3,
        DuganDice4,
        DuganDice5,
        DuganDice6,
    }

    [Serializable]
    public class ProjectileInfo
    {
        [PropertySpace(SpaceBefore = 2, SpaceAfter = 15)] [EnumPaging] [Title("Movement Style", bold: true)] [HideLabel]
        public ProjectileMovementType MovementType;

                
        [FoldoutGroup("ProjectileData")]
        [HorizontalGroup("ProjectileData/Horizontal", Width = 0.5f )]
        [BoxGroup("ProjectileData/Horizontal/Basic")]
        public int Health;
        [Min(0.5f)] [BoxGroup("ProjectileData/Horizontal/Basic")]
        public float Scale = 1;
        [BoxGroup("ProjectileData/Horizontal/Basic")]
        public float Damage = 5;
        
        //[FoldoutGroup("Movement")]
        //[HorizontalGroup("Movement/Horizontal" )]
        [BoxGroup("ProjectileData/Horizontal/Movement")][Min(0.5f)]
        public float Speed = 5;
        [BoxGroup("ProjectileData/Horizontal/Movement")] [Min(0.1f)]
        public float Range = 1;
        [BoxGroup("ProjectileData/Horizontal/Movement")][ShowIf("@MovementType == ProjectileMovementType.OrganicJump")]
        public float Height = 3;
        [BoxGroup("ProjectileData/Horizontal/Movement")][ShowIf("@MovementType == ProjectileMovementType.RoboticJump")]
        public int JumpPower = 4;
        [BoxGroup("ProjectileData/Horizontal/Movement")][ShowIf("@MovementType == ProjectileMovementType.RoboticJump")]
        public int JumpCount = 2;
        [BoxGroup("ProjectileData/Horizontal/Movement")] [ShowIf("@MovementType == ProjectileMovementType.FollowTarget")]
        public int Acceleration = 12;
        [BoxGroup("ProjectileData/Horizontal/Movement")] [ShowIf("@MovementType == ProjectileMovementType.LinearMove || MovementType == ProjectileMovementType.RoboticJump")]
        public bool Recursive;
        [BoxGroup("ProjectileData/Horizontal/Movement")] [ShowIf("@Recursive")]
        public float WaitTime = 2;
        // [BoxGroup("Movement/Horizontal/One")]
        // public bool Rotate;
        // [BoxGroup("Movement/Horizontal/One")][ShowIf("@Rotate")]  
        // public int SpinSpeed = 2;


        [Serializable]
        public enum ProjectileMovementType
        {
            RoboticJump,
            OrganicJump,
            LinearMove,
            FollowTarget,
            None
        }
    }
}