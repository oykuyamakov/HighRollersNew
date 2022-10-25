using CombatManagement.ProjectileManagement;
using CombatManagement.ProjectileManagement.Implementations;
using Events;
using UnityEngine;

namespace CombatManagement.EventImplementations
{
    public class GetDuganDiceEvent : Event<GetDuganDiceEvent>
    {
        public int Id;
        public DuganDice SenderProjectile;
        public DuganDice TargetProjectile;
        
        public static GetDuganDiceEvent Get(int itemId, DuganDice senderPro)
        {
            var evt = GetPooledInternal();
            evt.Id = itemId;
            evt.SenderProjectile = senderPro;
            return evt;
        }

        protected override void Reset()
        {
            SenderProjectile = null;
            TargetProjectile = null;
            base.Reset();
        }
    }
}
