using System.Collections;
using System.Collections.Generic;
using Events;
using Sounds;
using UnityEngine;

public class SoundPlayEvent : Event<SoundPlayEvent>
{
    public Sound Sound;
    public bool Loop;
    public static SoundPlayEvent Get(Sound sound, bool loop= false)
    {
        var evt = GetPooledInternal();
        evt.Sound = sound;
        evt.Loop = loop;
        return evt;
    }
}
