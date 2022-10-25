using Based.Utility;
using Events;
using Roro.Scripts.GameManagement;
using UnityEngine;

namespace Misc.EventImplementations
{
    public class ParticleSpawnEvent : Event<ParticleSpawnEvent>
    {
        public ParticleType ParticleType;
        public Particle Particle;
        
        public static ParticleSpawnEvent Get(ParticleType particleType)
        {
            var evt = GetPooledInternal();
            evt.ParticleType = particleType;
            return evt;
        }
    }
}
