using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using MoreMountains.NiceVibrations;
using Roro.Scripts.Serialization;
using Roro.Scripts.Utility;
using SceneManagement.EventImplementations;
using SettingImplementations;
using Sirenix.OdinInspector;
using UI.Menu;
using UnityCommon.Modules;
using UnityCommon.Runtime.UI;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneManagement
{
	[System.Serializable]
	public enum SceneId
	{
		None = 0,
		Intro = 1,
		Menu = 2,
		Shared = 4,
		Hub = 16,
		BossOne = 32,
		Loading = 64,
	}

	public static class SceneExtensions
	{
		public static Scene GetScene(this SceneId id)
		{
			return SceneManager.GetSceneByName(id.ToString());
		}
	}

	public static class Shared
	{
		public static Camera MainCamera => m_Cam == null ? m_Cam = Camera.main : m_Cam;
		private static Camera m_Cam;
	}

	[DefaultExecutionOrder(ExecOrder.SceneManager)]
	public class SceneTransitionManager : SingletonBehaviour<SceneTransitionManager>
	{
		[SerializeField]
		private SceneId m_InitialScene = SceneId.Menu;

		private List<SceneController> m_SceneControllersList = new();
		private Dictionary<Scene, SceneController> m_SceneControllers = new();
		
		[ShowInInspector]
		public SceneId CurrentSceneId => m_CurrentSceneId;
		private SceneId m_CurrentSceneId;
		private SceneId m_NextSceneId;	
		private SceneId m_SceneToUnload;
		private SceneId m_CurrentTempSceneId;

		private SerializationWizard m_SerializationContext;

		public bool debug = false;
		
		private List<Action<SceneId>> m_SceneChangeListeners = new List<Action<SceneId>>();
		private List<SceneController> m_PermanentSceneControllers = new();

		private Camera m_Camera;

		private Conditional m_CameraDisableTimer;

		private bool m_NextTempSceneLoaded;		
		private bool m_LoadingTimePassed;
		private bool m_NextSceneActivated;
		

		
		
		private void Awake()
		{
			if (!SetupInstance(false))
				return;

			m_Camera = Camera.main;

			m_NextTempSceneLoaded = false;
			
			m_SerializationContext = SerializationWizard.Default;

			GEM.AddListener<SceneChangeRequestEvent>(OnSceneChangeRequest);

			if (debug)
			{
				Conditional.WaitFrames(2).Do(OnAllScenesLoaded);
			}
		}
		
		public void OnAllScenesLoaded()
		{
			m_SceneControllersList = FindObjectsOfType<SceneController>().ToList();
			m_PermanentSceneControllers = FindObjectsOfType<SceneController>().ToList();
			m_SceneControllers = m_SceneControllersList.ToDictionary(act => act.gameObject.scene);

			ChangeScene(m_InitialScene);
			
			// var ctrl = m_SceneControllers[m_InitialScene.GetScene()];
			// ctrl.SetActiveState(true);
		}

		private void OnSceneChangeRequest(SceneChangeRequestEvent evt)
		{
			m_SerializationContext.Push();
			bool changed = ChangeScene(evt.sceneId);

			evt.result = changed ? EventResult.Positive : EventResult.Negative;
		}

		public IEnumerator EnableLoadingScene()
		{
			ActivateScene(SceneId.Loading,false);

			yield return new WaitForSeconds(GeneralSettings.Get().SceneTransitionDuration);

			m_LoadingTimePassed = true;

			while (!m_NextTempSceneLoaded)
			{
				yield return null;
			}

			//if (m_NextSceneLoaded && m_NextSceneActivated)
			ActivateScene(m_NextSceneId,false);
			
			m_NextTempSceneLoaded = false;
		}

		public bool ActivateScene(SceneId id, bool enableLoading)
		{
			for (int i = 0; i < m_PermanentSceneControllers.Count; i++)
			{
				var controller = m_PermanentSceneControllers[i];
				
				if (!controller.IsPermanent)
				{
					continue;
				}
				if(controller.SceneId != id)
					controller.TogglePermanentScene(false);
				else
				{
					//if the scene to activate is permanent scene, set it active and return
					controller.TogglePermanentScene(true);
					
					var sceneName = controller.SceneName;
					var sceneToLoad = SceneManager.GetSceneByName(sceneName);
					SceneManager.SetActiveScene(sceneToLoad);
					
					return true;
				}
			}

			if (enableLoading)
			{
				m_NextSceneId = id;
				StartCoroutine(EnableLoadingScene());
				
				//unload previous temp scene
				//Debug.Log($"Unload if there is a temp scene enabled : {m_CurrentTempSceneId.ToString()}");
				m_SceneToUnload = m_CurrentTempSceneId;
				StartCoroutine(UnLoadScene(m_SceneToUnload));
			}

			//Debug.Log( $"Next scene is  {id.ToString()} ");
			//load requested temp scene
			StartCoroutine(LoadScene(id, enableLoading));

			return true;
		}
		private GeneralSettings m_Settings = GeneralSettings.Get();
		public IEnumerator LoadScene(SceneId sceneId, bool waitForLoadingScene)
		{
			var sceneName = sceneId.ToString();
			
			var sceneToLoad = SceneManager.GetSceneByName(sceneName);
			if (sceneToLoad.IsValid())
			{
				//Debug.Log($"Already loaded scene {sceneId.ToString()} is activating" );
				m_NextTempSceneLoaded = true;

				m_CurrentTempSceneId = sceneId;
				if (waitForLoadingScene) 
					yield break;
				
				SceneManager.SetActiveScene(sceneToLoad);

				if (m_Settings.MusicsOnScenes.ContainsKey(sceneId))
				{
					var sceneSound = m_Settings.MusicsOnScenes[sceneId];
				
					using var musicEvent = SoundPlayEvent.Get(sceneSound, true);
					musicEvent.SendGlobal();
				}
				
				var evt2 = GetSceneControllerEvent.Get(sceneId).SendGlobal();
				if (evt2.Controller != null)
				{
					evt2.Controller.TogglePermanentScene(true);
				}
				
				yield break;
			}
		
			yield return null;
		
			
			yield return new WaitForSeconds(GeneralSettings.Get().SceneTransitionDuration/3f);

			var asyncOp = SceneManager.LoadSceneAsync(
				sceneName, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
		
			while (!asyncOp.isDone)
			{
				yield return null;
			}
		
			yield return null;

			//Debug.Log($"{sceneId.ToString()} scene is loaded and will be activated after loading time passes" );
			
			m_NextTempSceneLoaded = true;

			m_CurrentTempSceneId = sceneId;

			if (waitForLoadingScene) 
				yield break;
			
			var loadedScene = SceneManager.GetSceneByName(sceneName);
			SceneManager.SetActiveScene(loadedScene);
			
			var evt = GetSceneControllerEvent.Get(sceneId).SendGlobal();
			if (evt.Controller != null)
			{
				evt.Controller.TogglePermanentScene(true);
			}
			
		}
		
		public IEnumerator UnLoadScene(SceneId sceneId)
		{
			if (sceneId != SceneId.None)
			{
				var sceneName = sceneId.ToString();
				var sceneToLoad = SceneManager.GetSceneByName(sceneName);
				if (!sceneToLoad.IsValid())
				{
					//Debug.Log($"Current temp scene : {sceneName} is not loaded?");
				}
				else
				{
					yield return null;
					
					var evt = GetSceneControllerEvent.Get(sceneId).SendGlobal();
					if (evt.Controller != null)
					{
						evt.Controller.TogglePermanentScene(false);
					}

					SceneManager.UnloadSceneAsync(sceneName);

					yield return null;
					
					//Debug.Log($"Unloaded : {sceneId.ToString()}" );

				}
			}
			else
			{
				//Debug.Log("Scene ID cannot be \"None\", nothing to unload");

				yield return null;
			}
		}

		
		public bool ChangeScene(SceneId sceneId, bool animate = true)
		{
			m_Camera = Camera.main;

			for (var i = 0; i < m_SceneChangeListeners.Count; i++)
			{
				var listener = m_SceneChangeListeners[i];
				listener?.Invoke(m_CurrentSceneId);
			}

			return ActivateScene(sceneId,true);

		}

		public void AddSceneChangeListener(Action<SceneId> action)
		{
			m_SceneChangeListeners.Add(action);
		}

		public void RemoveSceneChangeListener(Action<SceneId> action)
		{
			m_SceneChangeListeners.Remove(action);
		}
	}
}
