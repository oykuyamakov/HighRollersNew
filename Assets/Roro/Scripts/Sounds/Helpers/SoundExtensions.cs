using UnityEngine;

namespace Sounds.Helpers
{
	public static class SoundExtensions
	{
		public static void PlayOneShot(this AudioSource src, Sound sound, float volume = 1f, float pitch = 1f)
		{
			src.pitch = sound.Pitch * pitch;
			src.loop = sound.Loop;
			src.PlayOneShot(sound.Clip, sound.Volume * volume);
		}
	}
}
