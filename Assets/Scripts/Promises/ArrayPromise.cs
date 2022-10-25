using System;
using System.Collections.Generic;

namespace Promises
{
	public struct ArrayPromise<T>
	{
		private static int s_AvailableId = 1;

		private static Dictionary<int, Action<string, int, bool, T>> s_ResultCallbacks =
			new Dictionary<int, Action<string, int, bool, T>>();

		public static ArrayPromise<T> Create()
		{
			var promise = new ArrayPromise<T>
			              {
				              Id = s_AvailableId++
			              };
			InitializeHandle(promise.Id);
			return promise;
		}


		private static void InitializeHandle(int id) => s_ResultCallbacks[id] = delegate { };

		private static void ReleaseHandle(int id) => s_ResultCallbacks.Remove(id);

		private static void InvokeResultCallback(int id, string userId, int index, bool success, T item)
		{
			if (!s_ResultCallbacks.TryGetValue(id, out var callback))
			{
				throw new Exception("Promise handle is not valid.");
			}

			callback.Invoke(userId, index, success, item);
		}

		private static void AddResultCallback(int id, Action<string, int, bool, T> callback)
		{
			if (!s_ResultCallbacks.ContainsKey(id))
			{
				throw new Exception("Promise handle is not valid.");
			}

			s_ResultCallbacks[id] += callback;
		}

		public struct Handle
		{
			public int Id;

			public event Action<string, int, bool, T> OnResultT
			{
				add => AddResultCallback(Id, value);
				remove => throw new NotImplementedException(
					"Removing callback from promise OnResultT is not supported.");
			}
		}

		public int Id;

		public Handle handle => new Handle {Id = Id};

		public void Complete(string userId, int index, T result)
		{
			InvokeResultCallback(Id, userId, index, true, result);
		}

		public void Fail( int index)
		{
			InvokeResultCallback(Id,"", index, false, default);
		}

		public void Release()
		{
			ReleaseHandle(Id);
		}
	}
}
