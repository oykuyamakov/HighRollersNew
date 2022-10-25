using System.Collections;
using AnimationManagement;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using DG.Tweening;
using Events;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using SettingImplementations;
using TMPro;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations
{
    public class DuganDice : Projectile
    {
        protected BossOneSettings m_BossOneSettings => BossOneSettings.Get();
        protected AnimationController m_AnimationController => GetComponent<AnimationController>();
        protected PhaseTwoValues m_PhaseTwoValues => BossOneSettings.Get().PhaseTwoValues;
        protected int m_MergeLevel => GetDiceLevel(this.ProjectileType);
        
        private TextMeshPro m_Text => GetComponentInChildren<TextMeshPro>();

        private Conditional m_CheckMergeableCond;

        private bool m_Merging;
        private bool m_IsMother;
        private bool m_AvailableToMerge;
        
        private bool m_DeActive;
        
        private Vector3 m_MergeMidPoint;
        
        protected int m_UniqueId;

        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);
            
            m_UniqueId = Random.Range(0, 12123);

            //SetDiceMaterials();
            transform.position = origin.WithY(m_YOffset);

            
            GEM.AddListener<GetDuganDiceEvent>(GetCheckedToMerge, Priority.High, m_MergeLevel);
            
            transform.rotation = Quaternion.identity;
            
            var dice = m_BossOneSettings.PhaseTwoValues.DiceBehaviours[m_MergeLevel -1];
            Health = dice.DiceInfo.Health;

            InitializeProjectileInfoParameters(dice.DiceInfo, Player.PlayerTransform, true);
        }

        protected virtual void DeActivate()
        {
            if(m_DeActive)
                return;
            
            using var evt = ParticleSpawnEvent.Get(ParticleType.MissileExplosion);
            evt.SendGlobal();
            var particle = evt.Particle;
            particle.Initialize(transform.position);

            //m_ClonedColorMat.color = m_BossOneSettings.PhaseTwoValues.DeactiveDiceColor;
            
            Mover.Reset();
            m_DeActive = true;
            
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
            
            // TODO WHY THERE IS DODGE COUNT ON PROJECTILE?
            m_DodgeCount = 0;
            m_DeActive = false;
            m_IsMother = false;
            
            Mover.Reset();
            
            GEM.RemoveListener<GetDuganDiceEvent>(GetCheckedToMerge, m_MergeLevel);

            base.DisableSelf();
        }
        public override void GetDamage(int damage)
        {
            if(m_DeActive)
                return;
            
            Health += -Mathf.Abs(damage);
            StartCoroutine(AnimateDamageText(damage.ToString()));

            if (Health < 0)
            {
                DeActivate();
            }
        }
        
        public virtual void OnHit(Transform hitter)
        {
        }
        

     

        public override void OnContact(Transform contacted)
        {
            if(m_DeActive)
                return;

            OnHit(contacted);
        }
        
        private void GetCheckedToMerge(GetDuganDiceEvent evt)
        {
            if (m_Merging || !m_AvailableToMerge)
                return;

            if (evt.Id != m_MergeLevel || evt.SenderProjectile.m_UniqueId == m_UniqueId) 
                return;
            
            m_IsMother = false;
            m_Merging = true;
            
            evt.TargetProjectile = this;
            OnMerge(evt.SenderProjectile.transform);
            
            evt.Consume();
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

            using var evt = GetDuganDiceEvent.Get(m_MergeLevel, this);
            evt.SendGlobal(m_MergeLevel);

            if (!evt.TargetProjectile)
                return;
            
            m_IsMother = true;
            m_Merging = true;

            OnMerge(evt.TargetProjectile.SelfTransform());
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
        private static int GetDiceLevel(ProjectileType type)
        {
            return type switch
            {
                ProjectileType.DuganDice1 => 1,
                ProjectileType.DuganDice2 => 2,
                ProjectileType.DuganDice3 => 3,
                ProjectileType.DuganDice4 => 4,
                ProjectileType.DuganDice5 => 5,
                ProjectileType.DuganDice6 => 6,
                _=> 1
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
        }
        
        // private void CloneMaterials()
        // {
        //     var colormat = new  Material(GetComponent<Renderer>().materials[0]);
        //     GetComponent<Renderer>().materials[0] = colormat;
        //     m_ClonedColorMat = GetComponent<Renderer>().materials[0]; 
        //     
        //     var shaderMat = new  Material(GetComponent<Renderer>().materials[2]);
        //     GetComponent<Renderer>().materials[2] = shaderMat;
        //     m_ClonedShaderMat = GetComponent<Renderer>().materials[2];
        //     
        //     var diceMat = new  Material(GetComponent<Renderer>().materials[1]);
        //     GetComponent<Renderer>().materials[1] = diceMat;
        //     m_ClonedDiceMat = GetComponent<Renderer>().materials[1];
        // }
        
        private IEnumerator AnimateDamageText(string damage)
        {
            m_Text.enabled = true;
            m_Text.text = $"- {damage}";
            yield return new WaitForSeconds(0.2f);
            m_Text.enabled = false;
        }
        
        // private void SetDiceMaterials()
        // {
        //     m_ClonedDiceMat.mainTexture = m_BossOneSettings.GetDiceFace(m_MergeLevel - 1);
        //     
        //     var dice = m_BossOneSettings.PhaseTwoValues.DiceBehaviours[m_MergeLevel -1];
        //
        //     m_ClonedShaderMat.SetColor(Property,dice.Color);
        //     m_ClonedColorMat.color = dice.Color;
        //     
        //     m_ClonedDiceMat.mainTexture = m_BossOneSettings.GetDiceFace(m_MergeLevel - 1);
        // }

    }
}
