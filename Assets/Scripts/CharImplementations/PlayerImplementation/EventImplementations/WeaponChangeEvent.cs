using CombatManagement.WeaponImplementation;
using Events;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class WeaponChangeEvent : Event<WeaponChangeEvent>
    {
        public WeaponData WeaponData;
        
        public static WeaponChangeEvent Get(WeaponData newWeapon)
        {
            var evt = GetPooledInternal();
            evt.WeaponData = newWeapon;
            return evt;
        }
    }
}