using CharImplementations;
using CharImplementations.EnemyImplementations.BossImplementations;
using Events;
using GameStages.EventImplementations;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceSix : DuganDice
    {
        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);
            
            Mover.MoveToPos(SkinnyDugan.DuganTransform.position, 5);

            Mover.OnDone += () =>
            {
                using var evt = PhaseChangeEvent.Get(3);
                evt.SendGlobal();
                
                DisableSelf();
            };
        }
    }
}
