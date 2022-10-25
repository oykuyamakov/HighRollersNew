using Events;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class CharacterDamageEvent : Event<CharacterDamageEvent>
    {
        public CharType CharType;
        
        public static CharacterDamageEvent Get(CharType charType)
        {
            var evt = GetPooledInternal();
            evt.CharType = charType;

            return evt;
        }
    }
}