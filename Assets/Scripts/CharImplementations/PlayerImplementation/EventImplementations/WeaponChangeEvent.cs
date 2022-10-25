using CombatManagement.WeaponImplementation;
using Events;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class WeaponChangeEvent : Event<WeaponChangeEvent>
    {
        public WeaponData PreviousWeaponData;
        public WeaponData WeaponData;
        
        public static WeaponChangeEvent Get(WeaponData newWeapon, WeaponData prevWeaponData)
        {
            var evt = GetPooledInternal();
            evt.WeaponData = newWeapon;
            evt.PreviousWeaponData = prevWeaponData;
            return evt;
        }
    }
}