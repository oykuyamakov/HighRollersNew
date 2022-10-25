using System;
using UnityCommon.Modules;
using UnityEngine;

namespace Promises.Promises
{
	public class PromiseTest : MonoBehaviour
	{
		private void Start()
		{
			var promise = Promise<string>.Create();

			promise.OnResultT += (success, str) => { Debug.Log("Received str: " + str); };

			Conditional.Wait(2f).Do(() => { promise.Complete("Heyo"); });
		}
	}
}
