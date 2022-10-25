using System.Collections.Generic;
using System.Linq;
using CharImplementations;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations
{
    public class Rock : Projectile
    {
        [SerializeField] 
        private List<GameObject> m_RockModels;

        private int ActiveRockIndex;

        public override void Initialize(Vector3 origin, Vector3 target, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            // ActiveRockIndex = UnityEngine.Random.Range(0, m_RockModels.Count);
            // if(m_RockModels.Any())
            //     m_RockModels[ActiveRockIndex].SetActive(true);
            
            base.Initialize(origin, target, damage, targetType, layersToCollide, layer);
        }


        public override void OnContact(Transform t)
        {
            if (t.TryGetComponent<Projectile>(out var pro))
            {
                if (pro.SelfLayer == "ProjectilePlayer")
                    return;
            }
            base.OnContact(t);

            if (!t.TryGetComponent<Character>(out var chara)) 
                return;
            
            if (chara.GetCharType() == TargetType)
            {
                DisableSelf();
            }
        }


        private  void OnDisable()
        {
            if(m_RockModels.Any())
                m_RockModels[ActiveRockIndex].SetActive(false);
            
        }
    }
}
