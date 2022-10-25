using System.Collections;
using System.Collections.Generic;
using Based2.EventImplementations;
using Events;
using Roro.Scripts.Serialization;
using SceneManagement;
using SettingImplementations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roro.Scripts.Helpers
{
	public class Initializer : MonoBehaviour
	{
		//private static readonly int HashFill = Shader.PropertyToID("_Fill");


		// [SerializeField]
		// private Image m_ProgressFillImage;
		//
		// [SerializeField]
		// private Text m_ProgressText;

		// [SerializeField]
		// private Canvas m_ProgressCanvas;

		[SerializeField]
		private List<SceneId> m_ScenesToLoad = new()
		{  SceneId.Shared, SceneId.Loading };

		//private float progress = 0f;

		public void Start()
		{
			//DontDestroyOnLoad(m_ProgressCanvas.gameObject);

			// var mat = m_ProgressFillImage.material;
			//
			// float fill = 0f;
			// // mat.SetFloat(HashFill, fill);
			// m_ProgressFillImage.fillAmount = fill;
			//
			// Conditional.While(() => m_ProgressFillImage).Do(() =>
			// {
			// 	fill = Mathf.Lerp(fill, progress, Time.deltaTime * 6f);
			// 	// mat.SetFloat(HashFill, fill);
			//
			// 	m_ProgressFillImage.fillAmount =
			// 		Mathf.Lerp(m_ProgressFillImage.fillAmount, progress, Time.deltaTime * 6f);
			//
			// 	m_ProgressText.text = $"{Mathf.RoundToInt(fill * 100f)}%";
			// });

			StartCoroutine(InitializeAsync());
		}


		private IEnumerator InitializeAsync()
		{
			yield return null;
			
			yield return new WaitForSeconds(1.5f);

			var ctx = SerializationWizard.Default;

			yield return LoadScenes();
		}

		private IEnumerator LoadScenes()
		{
			float p0 = 0f;
			float ps = 1f / m_ScenesToLoad.Count;
		
			Application.backgroundLoadingPriority = ThreadPriority.High;
		
			for (var i = 0; i < m_ScenesToLoad.Count; i++)
			{
				var sceneId = m_ScenesToLoad[i];
				var sceneName = sceneId.ToString();
		
				yield return null;
		
				var asyncOp = SceneManager.LoadSceneAsync(
					sceneName, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
		
				float p = p0;
				//progress = p;
				while (!asyncOp.isDone)
				{
					p = p0 + asyncOp.progress * ps;
					//progress = p;
					yield return null;
				}
		
				yield return null;
				if (sceneId == SceneId.Shared)
				{
					var firstScene = SceneManager.GetSceneByName(sceneId.ToString());
					SceneManager.SetActiveScene(firstScene);
				}
		
				yield return null;
		
				p = p0 + 1f * ps;
				//progress = p;
		
				p0 = p;
			}
		
			yield return null;
			yield return null;

			yield return new WaitForSeconds(GeneralSettings.Get().IntroWaitDuration);
		
			var sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
			sceneTransitionManager.OnAllScenesLoaded();
		
			yield return null;
			yield return null;
		
			using (var scenesLoadedEvt = AllScenesLoadedEvent.Get())
			{
				scenesLoadedEvt.SendGlobal();
			}
		
			// Conditional.WaitFrames(30).Do(() => { System.GC.Collect(); });
		
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		
			// Conditional.WaitFrames(30).Do(() =>
			// {
			// 	// progress = 1f;
			// 	m_ProgressCanvas.GetComponent<CanvasGroup>().DOFade(0f, 0.35f).SetEase(Ease.InQuad)
			// 	                .OnComplete(() =>
			// 	                {
			// 		                m_ProgressCanvas.gameObject.Destroy();
			// 		                Conditional.WaitFrames(2).Do(() => { Resources.UnloadUnusedAssets(); });
			// 	                });
			// });
		
		
			SceneManager.UnloadSceneAsync("Intro");
		}
	}
}
