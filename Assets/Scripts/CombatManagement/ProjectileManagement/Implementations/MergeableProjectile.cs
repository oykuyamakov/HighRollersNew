using System.Collections;
using System.Linq;
using AnimationManagement;
using CharImplementations;
using CharImplementations.EnemyImplementations.BossImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using DG.Tweening;
using Events;
using GameStages.EventImplementations;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using SettingImplementations;
using TMPro;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations
{
    [RequireComponent(typeof(AnimationController))]
    public class MergeableProjectile : Projectile
    {
        [SerializeField]
        private LineRenderer m_LineRenderer;

        //[SerializeField]
        private int m_MergeLevel;
        
        [SerializeField] 
        private TextMeshPro m_Text;

        private static readonly int Property = Shader.PropertyToID("_Color");
        
        private BossOneSettings m_BossOneSettings => BossOneSettings.Get();
        
        private AnimationController m_AnimationController => GetComponent<AnimationController>();
        private PhaseTwoValues m_PhaseTwoValues => BossOneSettings.Get().PhaseTwoValues;

        private LayerMask m_LayerMask;
        private LayerMask m_FireLayerMask;
        
        private Material m_ClonedColorMat;
        private Material m_ClonedShaderMat;
        private Material m_ClonedDiceMat;

        private Conditional m_CheckMergeableCond;
        private Conditional m_DiceOneWaitCond;

        private IAction m_FireAction;

        private bool m_Merging;
        private bool m_IsMother;
        private bool m_AvailableToMerge;
        
        private bool m_CheckForPlayerToStun;
        private bool m_TryStunning;

        private bool m_OnDeActiveMove;
        private bool m_BeamStarted;
        private bool m_Firing;
        private bool m_DiceOnStop;
        
        private Vector3 m_PlayerPosOnBeamStart;
        private Vector3 m_MergeMidPoint;

        private float m_T;
        
        private int m_UniqueId;

        protected override void Awake()
        {
            m_FireLayerMask = 
                1 <<  LayerMask.NameToLayer("Obstacle")
                | 1 <<  LayerMask.NameToLayer("Player") 
                | 1 << LayerMask.NameToLayer("ProjectilePlayer");
            
            //GEM.AddListener<ResetLevelEvent>(OnReset);
            
            //CloneMaterials();
            
            base.Awake();
        }

        public override void Initialize(Vector3 origin, Vector3 target, float damage, CharType targetType,
            LayerMask layersToCollide,string layer)
        {
            base.Initialize(origin, target, damage, targetType, layersToCollide,layer);
        }
        
        private void OnDeActiveMove()
        {
            m_CheckForPlayerToStun = false;
            m_DiceOnStop = false;
            m_DiceOneWaitCond?.Cancel();
            using var evt = ParticleSpawnEvent.Get(ParticleType.MissileExplosion);
            evt.SendGlobal();
            var particle = evt.Particle;
            particle.Initialize(transform.position);

            //m_ClonedColorMat.color = m_BossOneSettings.PhaseTwoValues.DeactiveDiceColor;
            Mover.Reset();
            m_FireAction = null;
            if(this.ProjectileType == ProjectileType.DuganDice4)
                m_LineRenderer.enabled = false;
            m_OnDeActiveMove = true;
            
            if (m_AvailableToMerge) 
                return;
            
            m_CheckMergeableCond = Conditional.Repeat(0.5f, 30, CheckToMerge);
            m_AvailableToMerge = true;
        }
        
        public override void DisableSelf()
        {
            m_AvailableToMerge = false;
            m_CheckMergeableCond?.Cancel();
            m_Merging = false;
            m_FireAction = null;
            m_DodgeCount = 0;
            if(this.ProjectileType == ProjectileType.DuganDice4)
                m_LineRenderer.enabled = false;
            m_OnDeActiveMove = false;
            m_Firing = false;
            m_TryStunning = false;
            m_CheckForPlayerToStun = false;
            m_IsMother = false;
            
            //Debug.Log("why" + m_MergeLevel);
            
            GEM.RemoveListener<GetDuganDiceEvent>(GetCheckedToMerge, m_MergeLevel);

            base.DisableSelf();
        }
        
        public void SetId(int id)
        {
            m_MergeLevel = id;
            m_UniqueId = Random.Range(0, 12123);

            //SetDiceMaterials();
            
            GEM.AddListener<GetDuganDiceEvent>(GetCheckedToMerge, Priority.High, m_MergeLevel);
            
            transform.rotation = Quaternion.identity;
            
            var dice = m_BossOneSettings.PhaseTwoValues.DiceBehaviours[m_MergeLevel -1];
            Health = dice.DiceInfo.Health;

            InitializeProjectileInfoParameters(dice.DiceInfo, Player.PlayerTransform, true);

            switch (id)
            {
                // case 1:
                //     m_AnimationController.SetBool("Walk",true);
                //     break;
                // case 2:
                //     DiceTwo();
                //     break;
                // case 4:
                //     DiceFour();
                //     break;
                // case 5:
                    // DiceFive();
                    // break;       
                case 6:
                    //DiceSix();
                    break;
            }
        }
        
        public override void GetDamage(int damage)
        {
            Health += -Mathf.Abs(damage);
            
            Debug.Log(Health);

            StartCoroutine(AnimateDamageText(damage.ToString()));

            if (Health < 0)
            {
                OnDeActiveMove();
            }
        }

        // private void FixedUpdate()
        // {
        //     if (m_CheckForPlayerToStun)
        //     {
        //         CheckForPlayer();
        //     }
        // }
        //
        // private void Update()
        // {
        //     m_FireAction?.Update(Time.deltaTime);
        // }
        //
        // private void CheckForPlayer()
        // {
        //     if(m_TryStunning)
        //         return;
        //     
        //     //DebugExtensions.DrawSphere(SelfCollider().bounds.center,transform.localScale.x * 5, Color.magenta);
        //     
        //     Collider[] hitDetects = Physics.OverlapSphere(transform.position, 3f, 1 << LayerMask.NameToLayer("Player"));
        //
        //     if(!hitDetects.Any())
        //         return;
        //     
        //     var vector1 = transform.position;
        //     var vector2 = hitDetects[0].transform.position;
        //     // translate the vector1 to %60 of the vector2
        //     var vector3 = Vector3.Lerp(vector1, vector2, 0.8f);
        //
        //     JumpAndTryStun(vector3);
        // }
        //
        // private void JumpAndTryStun(Vector3 pos)
        // {
        //     if(m_TryStunning)
        //         return;
        //     
        //     m_TryStunning = true;
        //     
        //     m_AnimationController.Trigger("Jump");
        //     
        //     Mover.SnuckJumpMovement(pos, Random.Range(3,5), 1, 0.3f, TryStun);
        // }
        //
        // // TODO non alloc
        // private void TryStun()
        // {
        //     Collider[] hitDetects = Physics.OverlapSphere(transform.position, 3f, 1 << LayerMask.NameToLayer("Player"));
        //     
        //     for (int i = 0; i < hitDetects.Length; i++)
        //     {
        //         var h = hitDetects[i].transform;
        //         if (h.TryGetComponent<Player>(out var pl))
        //         {
        //             pl.OnImpact(TargetType, -ProjectileDamage);
        //             pl.GetStunned(5f);
        //         }
        //     }
        //
        //     Conditional.Wait(1f).Do(() => m_TryStunning = false);
        //     //m_TryStunning = false;
        // }
        //
        // private void SendLaser()
        // {
        //     if (!m_BeamStarted)
        //     {
        //         m_PlayerPosOnBeamStart = Player.PlayerTransform.position;
        //         m_T = 0;
        //     }
        //     
        //     m_BeamStarted = true;
        //
        //     m_LineRenderer.enabled = true;
        //
        //     m_LineRenderer.positionCount = 2;
        //
        //     m_LineRenderer.transform.rotation = Quaternion.identity;
        //
        //     m_T += m_PhaseTwoValues.BeamTrackSpeed * Time.deltaTime;
        //
        //     var targetPos = Vector3.Lerp(m_PlayerPosOnBeamStart, Player.PlayerTransform.position,
        //         m_T);
        //
        //     var hitPos = FireManager.FireRay(transform.position,
        //         targetPos, 100, ProjectileDamage, CharType.Player, m_FireLayerMask, transform);
        //     
        //     transform.forward = hitPos.WithY(Player.GlobalProjectileY) -
        //                         transform.position.WithY(Player.GlobalProjectileY);
        //     var posX = hitPos.x - transform.position.x;
        //
        //     m_LineRenderer.SetPositions(new[] { Vector3.zero, new Vector3(posX, 0, hitPos.z - transform.position.z) });
        //
        //     m_Firing = true;
        // }
        //
        // private void OnLaserCoolDown()
        // {
        //     m_LineRenderer.enabled = false;
        //
        //     m_Firing = false;
        //
        //     m_BeamStarted = false;
        // }

        // private void SendBullet()
        // {
        //     using var evt = GetProjectileEvent.Get(ProjectileType.Dice5Projectile);
        //     evt.SendGlobal();
        //     
        //     var bullet = evt.Projectile;
        //
        //     var pos = Player.PlayerTransform.position + Player.PlayerTransform.forward * 2;
        //     bullet.Initialize(transform.position,
        //         pos,
        //         ProjectileDamage, CharType.Player, m_FireLayerMask, "ProjectileEnemy");
        //                 
        //     bullet.Mover.MoveToPos(pos,m_PhaseTwoValues.Dice5BulletSpeed);
        // }

        // private void DiceOneOnStop()
        // {
        //     if(m_DiceOnStop)
        //         return;
        //
        //     m_DiceOnStop = true;
        //     
        //     m_AnimationController.SetBool("Walk", false);
        //     
        //     Mover.Reset();
        //     
        //     m_DiceOneWaitCond = Conditional.Wait(0.5f).Do(() =>
        //     {
        //         DiceOneBlow();
        //     });
        // }

        // private void DiceOneBlow()
        // {
        //     using var evt2 = ParticleSpawnEvent.Get(ParticleType.SphereExplosion);
        //     evt2.SendGlobal();
        //
        //     var part = evt2.Particle;
        //     part.Initialize(transform.position);
        //     part.transform.localScale = Vector3.one * 2;
        //         
        //     var layermask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("ProjectileEnemy");
        //
        //     var hitColliders = Physics.OverlapSphere(transform.position, 3f, layermask);
        //     foreach (var hitCollider in hitColliders)
        //     {
        //         if (hitCollider.TryGetComponent<Player>(out var pl))
        //         {
        //             pl.OnImpact(TargetType, -ProjectileDamage);
        //             pl.GetStunned(5f);
        //         }
        //         else if (hitCollider.transform.TryGetComponent<MergeableProjectile>(out var pro))
        //         {
        //             if(pro.m_MergeLevel == m_MergeLevel)
        //                 pro.DiceOneOnStop();
        //         }
        //     }
        //         
        //     OnDeActiveMove();
        // }
        
        // private void DiceTwo()
        // {
        //     m_CheckForPlayerToStun = true;
        //     
        //     m_AnimationController.SetBool("Walk",true);
        // }
        
        // private void DiceFour()
        // {
        //     m_FireAction = new CooldownedAction(SendLaser,
        //         m_PhaseTwoValues.BeamInitialDelay, m_PhaseTwoValues.BeamCoolDownDuration,
        //         m_PhaseTwoValues.BeamFireDuration, OnLaserCoolDown);
        //
        //     m_LineRenderer.transform.localScale /= m_ScaleMultiplier;
        // }

        // private void DiceFive()
        // {
        //     m_FireAction = new ArbitraryAction(SendBullet, m_PhaseTwoValues.Dice5AttackValues);
        // }
        // private void DiceSix()
        // {
        //     Mover.MoveToPos(SkinnyDugan.DuganTransform.position, 5);
        //
        //     Mover.OnDone += () =>
        //     {
        //         using var evt = PhaseChangeEvent.Get(3);
        //         evt.SendGlobal();
        //         
        //         DisableSelf();
        //     };
        // }
        
        public void OnHit(Transform hitter)
        {
            if (hitter.TryGetComponent(out Player player))
            {
                switch (m_MergeLevel)
                {
                    // case 1:
                    //     DiceOneOnStop();
                    //     break;
                    // case 2:
                    //     if(m_TryStunning)
                    //         return;
                    //     
                    //     m_AnimationController.SetBool("Walk",false);
                    //
                    //     //player.OnImpact(TargetType, -ProjectileDamage);
                    //     break;
                    // case 3:
                    //     player.OnImpact(TargetType, -ProjectileDamage);
                    //     break;
                    case 4:
                        player.OnImpact(TargetType, -ProjectileDamage);
                        break;
                    case 5: 
                        player.OnImpact(TargetType, -ProjectileDamage);
                        break;
                }
                
                return;
            }

            if (hitter.TryGetComponent<IDamager>(out var damager))
            {
                var damage = (int)damager.Damage;
                switch (m_MergeLevel)
                {
                    // case 1:
                    //     GetDamage(damage);
                    //     
                    //     break;
                    // case 2:
                    //     if(m_TryStunning)
                    //         return;
                    //     
                    //     GetDamage(damage);
                    //     
                    //     break;
                    // case 3:
                    //     if (m_Moving)
                    //         return;
                    //
                    //     GetDamage(damage);
                    //     
                    //     break;
                    case 4:
                        GetDamage(damage);
                        
                        break;
                    case 5: 
                        
                        if (m_DodgeCount > 3)
                        {
                            GetDamage(damage);
                            return;
                        }

                        Mover.SnuckJumpMovement(transform.forward * 2, 1, 1, 0.2f);
                        m_DodgeCount++;
                        
                        break;
                }
            }
        }
        

      

        public override void OnContact(Transform contacted)
        {
            if(m_OnDeActiveMove)
                return;

            OnHit(contacted);
        }
        
        private void GetCheckedToMerge(GetDuganDiceEvent evt)
        {
            // if (m_Merging || !m_AvailableToMerge)
            //     return;
            //
            // if (evt.Id != m_MergeLevel || evt.SenderProjectile.m_UniqueId == m_UniqueId) 
            //     return;
            //
            // m_IsMother = false;
            // m_Merging = true;
            //
            // evt.TargetProjectile = this;
            // OnMerge(evt.SenderProjectile.transform);
            //
            // evt.Consume();
        }

        private void CheckToMerge()
        {
            if (!m_AvailableToMerge)
                return;

            if (m_Merging)
            {
                m_CheckMergeableCond?.Cancel();
                return;
            }

            // using var evt = GetDuganDiceEvent.Get(m_MergeLevel, this);
            // evt.SendGlobal(m_MergeLevel);

            // if (!evt.TargetProjectile)
            //     return;
            //
            // m_IsMother = true;
            // m_Merging = true;
            //
            // OnMerge(evt.TargetProjectile.SelfTransform());
            m_CheckMergeableCond.Cancel();
        }
        
        private void OnMerge(Transform t)
        {
            GEM.RemoveListener<GetDuganDiceEvent>(GetCheckedToMerge, m_MergeLevel);

            m_MergeMidPoint = Vector3.Lerp(transform.position, t.position, 0.5f);

            using var partEvt = ParticleSpawnEvent.Get(ParticleType.DiceMerge);
            partEvt.SendGlobal();

            var particle = partEvt.Particle;
            particle.Initialize(m_MergeMidPoint);
            particle.transform.localScale = Vector3.one * 6f;
            
            transform.DOMove(m_MergeMidPoint, 0.5f).OnComplete(() =>
            {
                if (m_IsMother)
                {
                    CreateNextDice();
                }

                //m_CanBeDestroyed = true;

                DisableSelf();
            });
        }

        private static ProjectileType GetDiceName(int id)
        {
            return id switch{
                1 => ProjectileType.DuganDice1,
                2 => ProjectileType.DuganDice2,
                3 => ProjectileType.DuganDice3,
                4 => ProjectileType.DuganDice4,
                5 => ProjectileType.DuganDice5,
                6 => ProjectileType.DuganDice6,
                _ => ProjectileType.DuganDice1
            };
        }

        private void CreateNextDice()
        {
            using var evt = GetProjectileEvent.Get(GetDiceName(m_MergeLevel + 1));
            evt.SendGlobal();

            var layermask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("ProjectilePlayer");
            var pr = evt.Projectile;
                    
            pr.Initialize(m_MergeMidPoint, Player.PlayerTransform.position,
                m_MergeLevel * 1.2f, CharType.Player,layermask, "ProjectileEnemy");

            pr.GetComponent<MergeableProjectile>().SetId(m_MergeLevel + 1);
        }
        
        private void CloneMaterials()
        {
            var colormat = new  Material(GetComponent<Renderer>().materials[0]);
            GetComponent<Renderer>().materials[0] = colormat;
            m_ClonedColorMat = GetComponent<Renderer>().materials[0]; 
            
            var shaderMat = new  Material(GetComponent<Renderer>().materials[2]);
            GetComponent<Renderer>().materials[2] = shaderMat;
            m_ClonedShaderMat = GetComponent<Renderer>().materials[2];
            
            var diceMat = new  Material(GetComponent<Renderer>().materials[1]);
            GetComponent<Renderer>().materials[1] = diceMat;
            m_ClonedDiceMat = GetComponent<Renderer>().materials[1];
        }
        
        private IEnumerator AnimateDamageText(string damage)
        {
            m_Text.enabled = true;
            m_Text.text = $"- {damage}";
            yield return new WaitForSeconds(0.2f);
            m_Text.enabled = false;
        }
        
        private void SetDiceMaterials()
        {
            m_ClonedDiceMat.mainTexture = m_BossOneSettings.GetDiceFace(m_MergeLevel - 1);
            
            var dice = m_BossOneSettings.PhaseTwoValues.DiceBehaviours[m_MergeLevel -1];

            m_ClonedShaderMat.SetColor(Property,dice.Color);
            m_ClonedColorMat.color = dice.Color;
            
            m_ClonedDiceMat.mainTexture = m_BossOneSettings.GetDiceFace(m_MergeLevel - 1);
        }
    }
}