using Roro.Scripts.Utility;
using UnityCommon.Modules;
using UnityCommon.Singletons;
using UnityCommon.Variables;
using UnityEngine;
using Utility;

namespace Roro.Scripts.GameManagement
{
    [DefaultExecutionOrder(ExecOrder.GameManager)]
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [SerializeField]
        private int m_TargetFrameRate = 60;

        private void Awake()
        {
            if (!SetupInstance())
                return;

            Variable.Initialize();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            Application.targetFrameRate = m_TargetFrameRate;

            Input.multiTouchEnabled = false;

            ConditionalsModule.CreateSingletonInstance();

        }
    }
}