using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialManagement
{
	public class Tutorial : SingletonBehaviour<Tutorial>, ICanvasRaycastFilter
	{
		private static readonly int HashFocus = Shader.PropertyToID("_Focus");
		private static readonly int HashAspectRatio = Shader.PropertyToID("_AspectRatio");

		[SerializeField]
		private Canvas m_Canvas;

		[SerializeField]
		private CanvasGroup m_CanvasGroup;

		[SerializeField]
		private Image m_HandImage;

		private static Transform handTransform;

		//for tutorials with focus on screen area
		[SerializeField]
		private Material m_FocusMaterial;

		//For tutorials with text
		[SerializeField]
		private RectTransform m_BubbleTransform;
		[SerializeField]
		private Text m_BubbleText;

		private float4 m_CurrentFocus;

		private float2 m_ScreenSize;
		private float m_AspectRatio;

		private bool m_FocusVisible;
		private bool m_HandVisible;
		private bool m_BubbleVisible;
		
		private static Sequence handSequence;
		private static Tweener focusTweener;

		public static List<GraphicRaycaster> AllGraphicRaycasters { get; set; }

		private void Awake()
		{
			if (!SetupInstance(false))
				return;
			
			m_ScreenSize = new float2(Screen.width, Screen.height);
			m_AspectRatio = m_ScreenSize.y / m_ScreenSize.x;

			m_Canvas.enabled = false;
			m_CanvasGroup.alpha = 0f;

			m_HandImage.color = Color.white.WithAlpha(0f);
			handTransform = m_HandImage.transform;

			m_CurrentFocus = new float4(0.5f, 0.5f, 3f, 0.1f);
			m_FocusMaterial.SetVector(HashFocus, m_CurrentFocus);
			m_FocusMaterial.SetFloat(HashAspectRatio, Screen.height / (float) Screen.width);

			m_BubbleTransform.localScale = Vector3.zero;
		}

		public static void ShowBubble()
		{
			if (I.m_BubbleVisible)
				return;

			I.m_BubbleVisible = true;

			I.m_BubbleTransform.DOKill();
			I.m_BubbleTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
		}

		public static void HideBubble()
		{
			if (!I.m_BubbleVisible)
				return;

			I.m_BubbleVisible = false;

			I.m_BubbleTransform.DOKill();
			I.m_BubbleTransform.DOScale(0f, 0.5f).SetEase(Ease.InBack).SetDelay(0.25f);
		}

		public static void SetBubbleText(string text)
		{
			ShowBubble();

			I.m_BubbleText.DOKill();
			I.m_BubbleText.text = "";
			I.m_BubbleText.DOText(text, 0.9f, false).SetEase(Ease.Linear).SetDelay(0.25f);
		}

		public static Transform GetHandTransform()
		{
			return I.m_HandImage.transform;
		}

		public static Transform SetHandPosition(Vector3 pos)
		{
			//Debug.Log("hand pos: " + pos);
			var t = handTransform.transform;
			handSequence.Kill();
			t.DOKill();
			t.position = pos;
			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// {
			// }
			// else
			// {
				ShowHand();
			//}

			return t;
		}

		public static void ShowHand()
		{
			if (I.m_HandVisible)
				return;

			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// 	return;

			I.m_HandImage.transform.localScale = Vector3.one;

			I.m_HandVisible = true;

			handSequence.Kill();
			handTransform.DOKill();
			I.m_HandImage.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);

			I.m_CanvasGroup.DOKill();
			I.m_CanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
			I.m_Canvas.enabled = true;
		}

		public static void HideHand()
		{
			if (!I.m_HandVisible)
				return;

			handSequence.Kill();
			I.m_HandVisible = false;

			I.m_HandImage.transform.DOKill();
			I.m_HandImage.DOKill();
			I.m_HandImage.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);

			if (!I.m_FocusVisible)
			{
				HideCanvas();
			}
		}

		public static void ShowFocus()
		{
			if (I.m_FocusVisible)
				return;
			//
			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// 	return;

			I.m_FocusVisible = true;

			I.m_CanvasGroup.DOKill();
			I.m_CanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
			I.m_Canvas.enabled = true;
		}

		public static void HideFocus()
		{
			if (!I.m_FocusVisible)
				return;

			I.m_CurrentFocus.z = 3f;

			I.m_FocusMaterial.DOKill();
			var focus = I.m_CurrentFocus;
			focus.xy = new float2(0.5f, 0.5f);
			focus.z = focus.z * focus.z;
			focus.z = 3f;
			focus.w = 0.003f;
			I.m_FocusMaterial.DOVector(focus, HashFocus, 0.5f).SetEase(Ease.InQuad);
			I.m_FocusVisible = false;

			if (!I.m_HandVisible)
			{
				HideCanvas();
			}
		}

		private static void HideCanvas()
		{
			I.m_CanvasGroup.DOKill();
			I.m_CanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() => { I.m_Canvas.enabled = false; });
		}


		public static void SetFocusTargetWorld(Transform target, float z, float w)
		{
			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// 	return;

			ShowFocus();
			w = 0.01f;
			// I.m_CurrentFocus.z = z;
			I.m_CurrentFocus.w = w;
			I.m_FocusMaterial.DOKill();

			// focusTweener?.Kill();

			float t = 0f;
			var focus0 = I.m_CurrentFocus;
			focusTweener = DOTween.To(() => t, val =>
			{
				t = val;
				var pos = Camera.main.WorldToScreenPoint(target.position);
				pos.x /= I.m_ScreenSize.x;
				pos.y /= I.m_ScreenSize.y;
				var focus = Vector4.Lerp(focus0, new Vector4(pos.x, pos.y, z, w), t);
				I.m_CurrentFocus = focus;
				focus.z = focus.z * focus.z;
				focus.w = 0.003f;
				I.m_FocusMaterial.SetVector(HashFocus, focus);
			}, 1f, 0.6f).SetEase(Ease.InOutQuad).SetTarget(I.m_FocusMaterial);
		}
		public static void SetFocusTarget(Transform target, float z, float w)
		{
			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// 	return;

			ShowFocus();
			w = 0.01f;
			// I.m_CurrentFocus.z = z;
			I.m_CurrentFocus.w = w;
			I.m_FocusMaterial.DOKill();

			// focusTweener?.Kill();

			float t = 0f;
			var focus0 = I.m_CurrentFocus;
			focusTweener = DOTween.To(() => t, val =>
			{
				t = val;
				var pos = target.position;
				pos.x /= I.m_ScreenSize.x;
				pos.y /= I.m_ScreenSize.y;
				var focus = Vector4.Lerp(focus0, new Vector4(pos.x, pos.y, z, w), t);
				I.m_CurrentFocus = focus;
				focus.z = focus.z * focus.z;
				focus.w = 0.003f;
				I.m_FocusMaterial.SetVector(HashFocus, focus);
			}, 1f, 0.6f).SetEase(Ease.InOutQuad).SetTarget(I.m_FocusMaterial);
		}

		public static void SetFocusScreen(Vector4 focus)
		{
			// Debug.Log("focus screen: " + focus);
			// Debug.Log("screen size: " + I.m_ScreenSize);
			focus.x /= I.m_ScreenSize.x;
			focus.y /= I.m_ScreenSize.y;

			// Debug.Log("viewport size: " + focus);
			SetFocus(focus);
		}

		[Button]
		public static void SetFocus(Vector4 focus)
		{
			// if (I.m_GeneralSettings.HideTutorialInEditor && Application.isEditor)
			// 	return;

			ShowFocus();
			focus.w = 0.01f;
			I.m_CurrentFocus = focus;
			// focus.z = focus.z * focus.z;
			// focus.w = 0.003f;
			I.m_FocusMaterial.DOKill();
			focus.z = focus.z * focus.z;
			focus.w = 0.003f;
			I.m_FocusMaterial.DOVector(focus, HashFocus, 0.6f).SetEase(Ease.InOutQuad);
			// I.m_FocusMaterial.SetVector(HashFocus, focus);
		}

		// blocks raycasts outside focus area
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			float2 pos = (float2) sp / m_ScreenSize;
			var r = pos - m_CurrentFocus.xy;

			r.y *= m_AspectRatio;

			var isInFocusArea = math.length(r) < m_CurrentFocus.z;

			return !isInFocusArea;
		}

// New stuff

		private static bool s_GraphicRaycastersDisabled;

		public static void DisableAllGraphicRaycasters()
		{
#if UNITY_EDITOR
			// if (I.m_GeneralSettings.HideTutorialInEditor)
			// 	return;
#endif

			// for (var i = BoardSlots.Count - 1; i >= 0; i--)
			// {
			// 	var slot = BoardSlots[i];
			// 	slot.Raycaster.raycastTarget = false;
			// }
		}

		public static void DisableAllGraphicRaycastersExcept(Transform target)
		{
			DisableAllGraphicRaycasters();

			var graphic = target.GetComponent<Graphic>();
			if (graphic)
				graphic.raycastTarget = true;

			target.GetComponentInParent<GraphicRaycaster>().enabled = true;
		}

		public static void EnableAllGraphicRaycasters()
		{
			s_GraphicRaycastersDisabled = false;
			Conditional.Wait(0.25f).Do(() =>
			{
				if (s_GraphicRaycastersDisabled)
					return;

				for (var i = AllGraphicRaycasters.Count - 1; i >= 0; i--)
				{
					var gr = AllGraphicRaycasters[i];
					if (!gr)
					{
						AllGraphicRaycasters.RemoveAt(i);
						continue;
					}
				
					gr.enabled = true;
				}

				// for (var i = Slots.Count - 1; i >= 0; i--)
				// {
				// 	var slot = Slots[i];
				// 	slot.Raycaster.raycastTarget = true;
				// }
			});
		}

		public static void HideAll()
		{
			HideBubble();
			HideFocus();
			HideHand();
			EnableAllGraphicRaycasters();
		}

		public static void HandClick(Transform target, int loops = -1)
		{
#if UNITY_EDITOR
			// if (I.m_GeneralSettings.HideTutorialInEditor)
			// {
			// 	return;
			// }
#endif
			ShowHand();

			handSequence?.Kill();
			handTransform.DOKill();

			handTransform.localScale = Vector3.one;
			handTransform.position = target.position;
			handSequence = DOTween.Sequence().Append(handTransform.DOScale(0.75f, 0.25f).SetEase(Ease.InQuad))
			                      .AppendInterval(0.0f)
			                      .Append(handTransform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad)).AppendInterval(0.4f)
			                      .SetLoops(loops, LoopType.Restart)
			                      .OnUpdate(
				                      () => { handTransform.position = target.position; }).OnComplete(HideAll);;
		}
		
		public static void HandClick(Vector3 target, int loops = -1)
		{
#if UNITY_EDITOR
			// if (I.m_GeneralSettings.HideTutorialInEditor)
			// {
			// 	return;
			// }
#endif
			ShowHand();

			handSequence?.Kill();
			handTransform.DOKill();

			handTransform.localScale = Vector3.one;
			handTransform.position = target;
			handSequence = DOTween.Sequence().Append(handTransform.DOScale(0.75f, 0.25f).SetEase(Ease.InQuad))
			                      .AppendInterval(0.0f)
			                      .Append(handTransform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad)).AppendInterval(0.4f)
			                      .SetLoops(loops, LoopType.Restart)
			                      .OnUpdate(
				                      () => { handTransform.position = target; }).OnComplete(HideAll);;
		}

		public static void HandDrag(Transform from, Transform to, int loops = -1)
		{
#if UNITY_EDITOR
			// if (I.m_GeneralSettings.HideTutorialInEditor)
			// {
			// 	return;
			// }
#endif
			ShowHand();

			handSequence?.Kill();
			handTransform.DOKill();

			float a = 0f;
			handTransform.localScale = Vector3.one;
			handSequence = DOTween.Sequence().Append(DOTween.To(() => a, val =>
			                      {
				                      a = val;
				                      handTransform.position = Vector3.Lerp(from.position, to.position, a);
			                      }, 1f, 0.8f).SetEase(Ease.InOutQuad))
			                      .AppendInterval(0.4f).SetLoops(loops, LoopType.Restart).OnComplete(HideAll);
		}
		public static void HandDragWorld(Transform from, Transform to, int loops = -1)
		{
#if UNITY_EDITOR
			// if (I.m_GeneralSettings.HideTutorialInEditor)
			// {
			// 	return;
			// }
#endif
			ShowHand();

			handSequence?.Kill();
			handTransform.DOKill();
			var t0Pos = Camera.main.WorldToScreenPoint(to.position);
			var f0Pos = Camera.main.WorldToScreenPoint(from.position);

			float a = 0f;
			handTransform.localScale = Vector3.one;
			handSequence = DOTween.Sequence().Append(DOTween.To(() => a, val =>
			                      {
				                      a = val;
				                      handTransform.position = Vector3.Lerp(f0Pos,
					                      t0Pos, a);
			                      }, 1f, 0.8f).SetEase(Ease.InOutQuad))
			                      .AppendInterval(0.4f).SetLoops(loops, LoopType.Restart).OnComplete(HideAll);
		}
	}
}
