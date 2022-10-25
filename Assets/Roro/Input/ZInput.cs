// using System.Runtime.CompilerServices;
// using UnityCommon.Modules;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// namespace Zerosum
// {
// 	public static class ZInput
// 	{
// 		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
// 		static void Initialize()
// 		{
// 			var module = ZInputModule.Instance;
// 		}
//
// 		public static bool multiTouchEnabled
// 		{
// 			get => Input.multiTouchEnabled;
// 			set => Input.multiTouchEnabled = value;
// 		}
//
//
// 		public static Vector3 mousePosition
// 		{
// 			get => Input.mousePosition;
// 		}
//
// 		public static bool GetMouseButtonDown(int i) => MouseDown;
// 		public static bool GetMouseButtonUp(int i)   => MouseUp;
// 		public static bool GetMouseButton(int i)     => MouseHold;
//
// 		public static Vector3 MouseDownPosition     { get; internal set; }
// 		public static Vector3 MousePreviousPosition { get; internal set; }
// 		public static Vector3 MouseDelta            { get; internal set; }
//
// 		public static bool MouseDown { get; internal set; }
// 		public static bool MouseUp   { get; internal set; }
// 		public static bool MouseHold { get; internal set; }
//
// 		internal static bool CanSwipe { get; set; }
//
// 		[MethodImpl(MethodImplOptions.AggressiveInlining)]
// 		public static float GetDrag(Vector3 axis, float sensitivity = 12f) =>
// 			GetDrag(MousePreviousPosition, axis, sensitivity);
//
//
// 		[MethodImpl(MethodImplOptions.AggressiveInlining)]
// 		internal static float GetDrag(Vector3 mouseAnchorPosition, Vector3 axis, float sensitivity = 12f)
// 		{
// 			return MouseHold ? Vector3.Dot(axis, mousePosition - mouseAnchorPosition) * sensitivity / Screen.width : 0f;
// 		}
//
//
// 		[MethodImpl(MethodImplOptions.AggressiveInlining)]
// 		public static int GetSwipe(float sensitivity = 9f, bool requireRelease = true) =>
// 			GetSwipe(Vector3.right, sensitivity, requireRelease);
//
// 		[MethodImpl(MethodImplOptions.AggressiveInlining)]
// 		public static int GetSwipe(Vector3 axis, float sensitivity = 9f, bool requireRelease = true)
// 		{
// 			if (requireRelease && !CanSwipe)
// 				return 0;
// 			var swipe = (int) Mathf.Clamp(GetDrag(MouseDownPosition, axis, sensitivity), -1.1f, 1.1f);
// 			if (swipe != 0)
// 			{
// 				CanSwipe = false;
// 			}
//
// 			return swipe;
// 		}
// 	}
// }
