using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using Roro.Scripts.Utility;
using SceneManagement;
using Sounds;
using Sounds.Helpers;
using UnityCommon.Singletons;
using UnityCommon.Variables;
using UnityEngine;
using Utility;

namespace Roro.Scripts.Sounds.Core
{
	[DefaultExecutionOrder(ExecOrder.SoundManager)]
	[RequireComponent(typeof(AudioSource))]
	public class SoundManager : SingletonBehaviour<SoundManager>
	{
		private List<AudioSource> m_AudioSources => GetComponents<AudioSource>().ToList();
		
		private int m_SourceIndex;

		private int m_AvailableSourceCount;

		//private BoolVariable m_SoundsDisabled;

		private void Awake()
		{
			if(!SetupInstance(false))
				return;
			
			//m_SoundsDisabled = Var.Get<BoolVariable>("SFXDisabled");

			m_SourceIndex = 0;
			
			Reset();

			GEM.AddListener<SoundPlayEvent>(OnSounPlayEvent);
			GEM.AddListener<OnSceneLoadedEvent>(OnSceneLoadEvent);
		}

		private void OnSounPlayEvent(SoundPlayEvent evt)
		{
			PlaySound(evt.Sound);
		}

		public void PlaySound(Sound sound)
		{
			PlayOneShot(sound);
		}

		public void OnSceneLoadEvent(OnSceneLoadedEvent evt)
		{
			if (!evt.SceneController.IsPermanent)
			{
				Reset();
			}
		}
		
		private AudioSource GetSource(bool loop)
		{
			var index = 0;
			if (loop)
			{
				index = (m_AudioSources.Count - 1) - (m_AudioSources.Count - m_AvailableSourceCount);
				m_AvailableSourceCount--;
			}
			else
			{
				index = m_SourceIndex++;
			}
			var src = m_AudioSources[index];
			m_SourceIndex = m_SourceIndex % m_AvailableSourceCount;
			return src;
		}
		public void Reset()
		{
			m_AudioSources.ForEach(source =>
			{
				if (source.isPlaying)
				{
					source.DOFade(0, 0.1f).OnComplete(() =>source.loop = false);
				}

				m_AvailableSourceCount = m_AudioSources.Count;
			});
		}

		public void PlayOneShot(Sound sound, float volume = 1f, float pitch = 1f)
		{
			// if (m_SoundsDisabled.Value)
			// 	return;
			
			if (!sound || !sound.Clip || sound.Volume < 1e-2f)
			{
				Debug.Log($"Ignoring sound {sound.name}");
				return;
			}

			var src = GetSource(sound.Loop);
			src.PlayOneShot(sound, volume, pitch);
		}
		
		
#if UNITY_EDITOR

		public void GetSounds()
		{
			
		}
#endif

		[Serializable]
		public class SoundPair
		{
			public int id;
			public Sound sound;
		}

	}
}
