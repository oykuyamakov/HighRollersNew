using UnityCommon.Modules;
using UnityEngine;

namespace Zerosum
{
	[DefaultExecutionOrder(-100)]
	public class ZInputModule : Module<ZInputModule>
	{
		private Vector3 mousePosition;

		public override void OnEnable()
		{
		}

		public override void OnDisable()
		{
		}

		public override void Update()
		{
			/*ZInput.MouseDown = Input.GetMouseButtonDown(0);
			ZInput.MouseUp = Input.GetMouseButtonUp(0);
			ZInput.MouseHold = Input.GetMouseButton(0);

			if (ZInput.MouseDown)
			{
				mousePosition = ZInput.mousePosition;
				ZInput.MousePreviousPosition = mousePosition;
				ZInput.MouseDownPosition = mousePosition;
				ZInput.CanSwipe = true;
				return;
			}

			ZInput.MousePreviousPosition = mousePosition;
			mousePosition = ZInput.mousePosition;*/
		}

		public override void LateUpdate()
		{
		}
	}
}
