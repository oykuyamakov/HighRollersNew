using Events;

namespace CombatManagement.EventImplementations
{
    public class ProjectileDamageEvent : Event<ProjectileDamageEvent>
    {
        public static ProjectileDamageEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}