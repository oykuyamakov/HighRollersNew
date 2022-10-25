using System;
using System.Collections.Generic;
using UnityEngine;

namespace Promises
{
	// 
	/// <summary>
	/// Promise is a structure we can use for async operations or for any process that needs
	/// to wait for another process to finish before being executed. You can checkout an example on
	/// how to use it in the class PromiseTest
	/// </summary>
	/// <typeparam name="T"></typeparam>
	
	public struct Promise<T>
	{
		private static int s_AvailableId = 0;

		private static Dictionary<int, Action<bool, T>> s_ResultCallbacks = new Dictionary<int, Action<bool, T>>();

		public static Promise<T> Create()
		{
			var promise = new Promise<T>
			              {
				              m_Id = s_AvailableId++
			              };
			s_ResultCallbacks[promise.m_Id] = delegate { };
			return promise;
		}

		private static bool IsValid(Promise<T> promise)
		{
			return s_ResultCallbacks.ContainsKey(promise.m_Id);
		}

		private int m_Id;
		private bool m_IsDone;
		private bool m_IsSuccessful;

		private T m_Result;

		public event Action<bool, T> OnResultT
		{
			add
			{
				if (m_IsDone)
				{
					value(m_IsSuccessful, m_Result);
					return;
				}

				s_ResultCallbacks[m_Id] += value;
			}
			remove { s_ResultCallbacks[m_Id] -= value; }
		}

		public void Complete(T result)
		{
			m_IsDone = true;
			m_IsSuccessful = true;
			m_Result = result;
			s_ResultCallbacks[m_Id].Invoke(true, result);
		}

		public void Fail()
		{
			m_IsDone = true;
			m_IsSuccessful = false;
			s_ResultCallbacks[m_Id].Invoke(false, default);
		}

		public void Release()
		{
			s_ResultCallbacks.Remove(m_Id);
		}
	}
}
