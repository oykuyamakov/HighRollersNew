using UnityEngine;

namespace Sounds
{
	[CreateAssetMenu(menuName = "Sound")]
	public class Sound : ScriptableObject
	{
		[SerializeField]
		private AudioClip m_AudioClip;

		[SerializeField]
		private float m_Volume = 0.75f;

		[SerializeField]
		private float m_Pitch = 1f;
		
		[SerializeField]
		private bool m_Loop = false;

		public AudioClip Clip   => m_AudioClip;
		public float     Volume => m_Volume;
		public float     Pitch  => m_Pitch;

		public bool Loop => m_Loop;
	}
}
