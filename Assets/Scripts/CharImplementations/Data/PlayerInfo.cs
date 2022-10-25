using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;

namespace CharImplementations.Data
{
    [CreateAssetMenu(fileName = "New Player", menuName = "Char/Player")]
    public class PlayerInfo : CharInfo
    {
        public float DodgeDistance;
        public float DodgeLimit;
        
        public float DodgeDuration;
        public float DodgeImmuneDuration;
        public float DamageStunDuration;
        public int DodgeCount = 2;
        public float DodgeCooldownDuration = 2;
        public float FireAnimReleaseDur = 0.3f;
        public float WalkSoundPeriod;
        [Min(0)][MaxValue(1)]
        public float SpeedDecreaseOnAim = 0.2f;        
        
        public Sound DamageSound;
        public Sound DeathSound;
        public Sound DodgeSound;
        public Sound FireSound;
        public Sound WalkSound;
    }
}