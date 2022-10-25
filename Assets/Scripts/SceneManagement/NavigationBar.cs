using Events;
using SceneManagement;
using Sirenix.OdinInspector;
using UnityCommon.Modules;
using UnityCommon.Runtime.UI.Animations;
using UnityEngine;

namespace Core
{
	public class NavigationBar : MonoBehaviour
	{
		[SerializeField]
		private UITranslateAnim m_Animation;

		private Conditional m_ResetBarTimer;

		private void Awake()
		{
			//GEM.AddListener<ToggleNavigationBarVisibilityEvent>(OnToggleNavigationBarRequest);
		}

		// private void OnToggleNavigationBarRequest(ToggleNavigationBarVisibilityEvent evt)
		// {
		// 	// if (SceneTransitionManager.Instance.CurrentSceneId != SceneId.Room)
		// 	// {
		// 	// 	// Ignore if not in room
		// 	// 	return;
		// 	// }
		//
		// 	m_Animation.Fade(evt.visible);
		// }

		[Button]
		public void TestEnabled()
		{
			m_Animation.Fade(true);
		}

		[Button]
		public void TestDisabled()
		{
			m_Animation.Fade(false);
		}
	}
}
