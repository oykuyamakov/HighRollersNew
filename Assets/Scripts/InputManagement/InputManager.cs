using System;
using DG.Tweening;
using Events;
using Fate.EventImplementations;
using InputManagement.EventImplementations;
using Rewired;
using Roro.Scripts.Utility;
using SceneManagement;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.Utility;
using UnityCommon.Singletons;
using UnityEngine;


namespace InputManagement
{
    [DefaultExecutionOrder(ExecOrder.InputManager)]
    public class InputManager : SingletonBehaviour<InputManager>
    {
        public Vector3 MoveDirX { get; private set; }
        public Vector3 MoveDirY { get; private set; }

        private Player m_RewiredPlayer;

        private float m_TimeScale = 1;

        private int m_PlayerId = 0;

        // TODO 
        int layerMask = 1 << 8;

        private Tweener m_TimeTween;
        
        #region InputActionNames

        private const string SLIDE = "Slide";
        private const string FIRE = "Fire";
        private const string WEAPON_SWAP = "Weapon Swap";

        private const string FATE_ATTACK = "Fate Attack";

        private const string MOVE_UP = "Move Up";
        private const string MOVE_RIGHT = "Move Right";
        private const string MOVE_LEFT = "Move Left";
        private const string MOVE_DOWN = "Move Down";

        private const string TIME_SLOW = "TimeSlow";

        private const string INVENTORY = "Inventory";

        private readonly Vector3 m_PlainDir = new Vector3(1, 0, 1);

        #endregion

        private void Awake()
        {
            if (!SetupInstance())
                return;

            m_RewiredPlayer = ReInput.players.GetPlayer(m_PlayerId);

            m_RewiredPlayer.AddInputEventDelegate(OnSlideInput, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, SLIDE);

            m_RewiredPlayer.AddInputEventDelegate(OnFirePressedInput, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, FIRE);
            m_RewiredPlayer.AddInputEventDelegate(OnFireContinuous, UpdateLoopType.Update,
                InputActionEventType.ButtonRepeating, FIRE);
            m_RewiredPlayer.AddInputEventDelegate(OnFireReleasedInput, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, FIRE);

            m_RewiredPlayer.AddInputEventDelegate(OnFateAttack, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, FATE_ATTACK);

            m_RewiredPlayer.AddInputEventDelegate(MoveHorizontal, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, MOVE_UP);
            m_RewiredPlayer.AddInputEventDelegate(MoveHorizontal, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, MOVE_DOWN);
            m_RewiredPlayer.AddInputEventDelegate(MoveVertical, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, MOVE_RIGHT);
            m_RewiredPlayer.AddInputEventDelegate(MoveVertical, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, MOVE_LEFT);

            m_RewiredPlayer.AddInputEventDelegate(OnWeaponSwitch, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, WEAPON_SWAP);

            m_RewiredPlayer.AddInputEventDelegate(ReleaseVertical, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, MOVE_LEFT);
            m_RewiredPlayer.AddInputEventDelegate(ReleaseVertical, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, MOVE_RIGHT);
            m_RewiredPlayer.AddInputEventDelegate(ReleaseHorizontal, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, MOVE_DOWN);
            m_RewiredPlayer.AddInputEventDelegate(ReleaseHorizontal, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, MOVE_UP);


            m_RewiredPlayer.AddInputEventDelegate(OnRightMousePressed, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, TIME_SLOW);
            m_RewiredPlayer.AddInputEventDelegate(OnRightMouseReleased, UpdateLoopType.Update,
                InputActionEventType.ButtonJustReleased, TIME_SLOW);

            m_RewiredPlayer.AddInputEventDelegate(OnInventory, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, INVENTORY);
        }

        private void MoveHorizontal(InputActionEventData data)
        {
            Vector3 mH = Vector3.zero;
            if (m_RewiredPlayer.GetButton(MOVE_UP))
                mH += m_PlainDir;
            if (m_RewiredPlayer.GetButton(MOVE_DOWN))
                mH += -m_PlainDir;
            MoveDirX = mH;
        }

        private void MoveVertical(InputActionEventData data)
        {
            Vector3 mH = Vector3.zero;
            if (m_RewiredPlayer.GetButton("Move Right"))
                mH += m_PlainDir.WithZ(-1);
            if (m_RewiredPlayer.GetButton("Move Left"))
                mH += -m_PlainDir.WithZ(-1);
            MoveDirY = mH;
        }

        private void ReleaseHorizontal(InputActionEventData data)
        {
            Vector3 mH = Vector3.zero;
            if (data.actionName == "Move Up")
                mH += m_PlainDir;
            if (data.actionName == "Move Down")
                mH += -m_PlainDir;

            MoveDirX -= mH;
        }

        private void ReleaseVertical(InputActionEventData data)
        {
            Vector3 mH = Vector3.zero;
            if (data.actionName == "Move Right")
                mH += m_PlainDir.WithZ(-1);
            if (data.actionName == "Move Left")
                mH += -m_PlainDir.WithZ(-1);
            MoveDirY -= mH;
        }

        private void OnRightMousePressed(InputActionEventData data)
        {
            m_TimeTween.Kill();
            m_TimeTween = DOTween.To(() => Time.timeScale = 1, x => Time.timeScale = x, 0.55f, 0.12f);

            using var evt = RightMousePressedEvent.Get(GetMousePos()).SendGlobal();
        }

        private void OnRightMouseReleased(InputActionEventData data)
        {
            m_TimeTween.Kill();
            m_TimeTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.12f);
            
            using var evt = RightMouseButtonReleasedEvent.Get().SendGlobal();
        }

        private void OnRightMouseContinuousPressed(InputActionEventData data)
        {
            using var evt = RightMouseButtonContinuousPressEvent.Get(GetMousePos());
            evt.SendGlobal();
        }

        private void OnSlideInput(InputActionEventData data)
        {
            using var evt = SlideEvent.Get();
            evt.SendGlobal();
        }

        public Vector3 GetMousePos()
        {
            var ray = Shared.MainCamera.ScreenPointToRay(m_RewiredPlayer.controllers.Mouse.screenPosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);

            if (Physics.Raycast(ray.origin, ray.direction * 1000, out var hit2, Mathf.Infinity, layerMask))
                return hit2.point;

            var b = Physics.RaycastAll(ray.origin, ray.direction * 1000, layerMask);

            for (int i = 0; i < b.Length; i++)
            {
                var o = b[i];

                if (o.transform.TryGetComponent<Ground>(out var ground))
                {
                    return o.point;
                }
            }

            return !Physics.Raycast(ray.origin, ray.direction * 1000, out var hit, Mathf.Infinity, layerMask)
                ? Shared.MainCamera.ScreenToWorldPoint(m_RewiredPlayer.controllers.Mouse.screenPosition)
                : hit.point;

            // return Physics.Raycast (ray, out var hit, Mathf.Infinity, LayerMask.NameToLayer("Ground")) ? hit.point 
            //     : Camera.main.ScreenToWorldPoint(m_RewiredPlayer.controllers.Mouse.screenPosition);
        }

        private void OnWeaponSwitch(InputActionEventData data)
        {
            using var evt = WeaponWheelEvent.Get();
            evt.SendGlobal();
        }

        private void OnFirePressedInput(InputActionEventData data)
        {
            using var evt = FireButtonPressedEvent.Get(GetMousePos());
            evt.SendGlobal();
        }

        private void OnFireReleasedInput(InputActionEventData data)
        {
            using var evt = FireButtonReleasedEvent.Get(GetMousePos());
            evt.SendGlobal();
        }

        private void OnFireContinuous(InputActionEventData data)
        {
            using var evt = FireButtonRepeatingEvent.Get(GetMousePos());
            evt.SendGlobal();
        }

        private void OnInventory(InputActionEventData data)
        {
            using var evt = ToggleFateEditorUIEvent.Get(true).SendGlobal();
        }

        private void OnFateAttack(InputActionEventData data)
        {
            using var evt = CallFateAttackEvent.Get().SendGlobal();
        }
    }
}