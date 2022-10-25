using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
	public class NavigationButton : MonoBehaviour
	{
		public SceneId SceneId => m_SceneId;

		[SerializeField]
		private SceneId m_SceneId;

		// [SerializeField]
		// private Graphic m_PingGraphic;

		[SerializeField] 
		//private AnimationController m_PingAnimationController;

		private Transform m_Transform;

		private bool m_IsPinging;

		private Tweener enableTween;
		private Tweener pingTween;

		private void Awake()
		{
			m_Transform = transform;

			//m_PingGraphic.enabled = false;
		}

		public void SetSelected(bool selected)
		{
			//var t = m_Transform;
			//t.DOKill();

			// var scale = selected ? 1.16f : 1f;
			// var overshoot = selected ? 8f : 4f;
			// t.DOScale(scale, 0.5f).SetEase(Ease.OutBack).easeOvershootOrAmplitude = overshoot;

			if (selected)
				EndPing();
		}

		public void Toggle(bool enable)
		{
			enableTween.Kill();

			var scale = enable ? Vector3.one : Vector3.zero;

			enableTween = transform.DOScale(scale, .5f).SetEase(Ease.OutBack);
		}

		public void  StartPing()
		{
			if (m_IsPinging)
				return;

			m_IsPinging = true;

			// AnimationData startAnim1 = new AnimationData(AnimationKeyData.PingAnimationsByKeys[AnimationKey.PingStart1], false, 1f);
			// AnimationData startAnim2 = new AnimationData(AnimationKeyData.PingAnimationsByKeys[AnimationKey.PingStart2], false, 1f);
			// AnimationData idleAnim = new AnimationData(AnimationKeyData.PingAnimationsByKeys[AnimationKey.PingIdle], true, 1);
			//
			// AnimationData[] anims = new[]
			// {
			// 	startAnim1,
			// 	startAnim2,
			// 	idleAnim
			// };
			//
			// m_PingAnimationController.SetAnimationState(anims);
			//
			// m_PingAnimationController.transform.localScale = Vector3.one * .2f; 

			//m_PingGraphic.enabled = true;
		}

		public void EndPing()
		{
			if (!m_IsPinging)
				return;

			m_IsPinging = false;
			
			// AnimationData endAnim = new AnimationData(AnimationKeyData.PingAnimationsByKeys[AnimationKey.PingEnd], false, 1f);
			// var animDur = m_PingAnimationController.SetAnimationState(endAnim);
			//
			// Conditional.Wait(animDur).Do((() =>
			// {
			// 	var t = m_PingAnimationController.transform;
			// 	pingTween.Kill();
			// 	pingTween = t.DOScale(0f, 0.5f).SetEase(Ease.InBack);
			//
			// }));

		}
	}
}
