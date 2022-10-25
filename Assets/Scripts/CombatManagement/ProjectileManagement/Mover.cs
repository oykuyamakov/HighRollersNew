using System;
using CharImplementations.PlayerImplementation;
using DG.Tweening;
using Promises;
using Unity.VisualScripting;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace CombatManagement.ProjectileManagement
{
    public class Mover : MonoBehaviour
    {
        public Action OnDone { get; set; }
        public float RecursiveWaitTime { get; set; }
        public bool Recursive { get; set; }
        private float m_YOffset => Grounded ? 2f : Player.GlobalProjectileY;
        private Vector3 Position => transform.position;

        public AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public Action m_RecursiveAction;

        public Tween m_CurrentTween { get; private set; }

        public Transform Target { get; set; }

        private Conditional m_RecursiveWaitConditional;

        private float m_Range = 1;

        public float MoveDur { get; private set; }

        public float RotateSpeed { get; private set; }

        public bool NavMeshEnabled { get; private set; }

        public bool Enabled { get; set; }

        public bool Rotating { get; private set; }
        
        public bool Grounded { get;  set; }

        public bool Moving { get; private set; }

        private bool m_NavMeshed;

        private bool m_Paused;

        #region ComponentGetters

        private Transform SelfTransform() => GetComponent<Transform>();
        private Rigidbody SelfRigid() => GetComponent<Rigidbody>();
        private NavMeshAgent NavMeshAgent() => GetComponent<NavMeshAgent>();
        private Collider SelfCollider() => GetComponent<Collider>();

        #endregion

        private void Update()
        {
            TryUpdateNavmesh();

            RotateIf();
        }

        private void TryUpdateNavmesh()
        {
            if (NavMeshEnabled && Enabled)
            {
                NavMeshAgent().destination = Target.position.WithY(m_YOffset);
                Moving = true;
            }
        }

        private void RotateIf()
        {
            if (Rotating)
            {
                transform.Rotate(0, 0, 100 * RotateSpeed * Time.deltaTime);
            }
        }

        public void Reset()
        {
            if (!Enabled)
                return;

            Enabled = false;

            if (NavMeshAgent())
            {
                NavMeshAgent().speed = 0;
                NavMeshAgent().enabled = false;
            }

            if (m_CurrentTween != null)
            {
                m_CurrentTween.onComplete = null;
                m_CurrentTween.Kill();
            }

            transform.DOKill();

            m_Curve = AnimationCurve.Linear(0, 0, 1, 1);
            m_Range = 1;
            Rotating = false;
            m_NavMeshed = false;
            NavMeshEnabled = false;
            m_CurrentTween = null;
            Recursive = false;
            m_RecursiveWaitConditional?.Cancel();

            //NavMeshAgent().enabled = false;

            Moving = false;
            OnDone = null;
            RecursiveWaitTime = 0;
            m_RecursiveAction = null;
        }

        public void DisableSelf()
        {
            if(!Enabled)
                return;
            Reset();

            SelfCollider().enabled = false;
            
        }


        public void FollowCharacterController(Transform target, float speed)
        {
            var dir = transform.position - target.position;

            GetCharController().SimpleMove((dir.normalized * (speed * Time.deltaTime)));
        }

        public void MoveToPosCharController(Vector3 targetPos, float speed)
        {
            var dir = transform.position - targetPos;

            Enabled = true;
            Moving = true;

            Conditional.While(() => !CheckIfAlignedHorizontal(targetPos)).Do(() =>
            {
                GetCharController().SimpleMove((dir.normalized * (speed * Time.deltaTime)));
            }).OnComplete(OnComplete);
        }

        public void DashToWall(Vector3 dir, float speed)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Conditional.While(() => !GetCharController().Raycast(ray, out var hit, 10)).Do(() =>
            {
                GetCharController().SimpleMove((dir.normalized * (speed * Time.deltaTime)));
            }).OnComplete(OnComplete);
        }

        public CharacterController GetCharController()
        {
            return TryGetComponent<CharacterController>(out var controller)
                ? controller
                : transform.AddComponent<CharacterController>();
        }

        public void MoveToPos(Vector3 position, float dur, float range = 1)
        {
            Enabled = true;
            m_Range = range != 1 ? range : m_Range;
            var dir = -(transform.position - position).normalized;
            var targetPos = m_Range == 1 ? position : dir * (m_Range * 15) + transform.position;
            m_CurrentTween = transform.DOMove(targetPos.WithY(m_YOffset),
                (targetPos - Position).magnitude / dur);

            Moving = true;

            m_CurrentTween.onComplete = OnComplete;
        }

        public void MoveToTargetRecursive(Transform target, float dur, float recursiveWaitTime = 0, float range = 1)
        {
            Enabled = true;

            Target = target;
            m_Range = range;
            RecursiveWaitTime = recursiveWaitTime;
            Recursive = true;
            MoveDur = dur;
            
            m_RecursiveAction = () => { MoveToPos(Target.position.WithY(m_YOffset), MoveDur, 1); };

            m_RecursiveAction.Invoke();
        }

        public void FollowTarget(Transform target, float speed, float acceleration)
        {
            Enabled = true;


            Target = target;
            NavMeshAgent().baseOffset = m_YOffset / 2;
            NavMeshAgent().speed = speed;
            NavMeshAgent().acceleration = acceleration;
            Moving = true;

            if (m_NavMeshed)
            {
                NavMeshEnabled = true;
                return;
            }

            SelfRigid().velocity = Vector3.zero;

            if (NavMesh.SamplePosition(transform.position, out var closestHit, 2000f, NavMesh.AllAreas))
                transform.position = closestHit.position;
            else
                Debug.Log("Could not find position on NavMesh!");

            NavMeshAgent().enabled = true;
            //having problems with y offset should fix later
            // NavMeshAgent().baseOffset = m_YOffset/2;
            // NavMeshAgent().speed = speed;
            // NavMeshAgent().acceleration = acceleration;

            m_NavMeshed = true;
            NavMeshEnabled = true;
            Moving = true;

            Debug.Log("ee enabled");
        }

        public void Rotate(float speed)
        {
            RotateSpeed = speed;
            Rotating = true;
        }

        public void JumpToPos(Vector3 targetPos, float height)
        {
            Enabled = true;


            SelfRigid().velocity = CalculateVelocity(height, targetPos);
            Moving = true;
        }

        public void JumpToPosRecursive(Transform target, float height, float recursiveWaitTime)
        {
            Enabled = true;


            Target = target;
            Recursive = true;
            RecursiveWaitTime = recursiveWaitTime;

            Conditional.If(() => CheckIfAlignedVertical(target.position.y)).Do(OnComplete);

            m_RecursiveAction = () =>
            {
                JumpToPos(Target.position, height);

                Conditional.If(() => CheckIfAlignedVertical(target.position.y)).Do(OnComplete);
            };

            JumpToPos(Target.position, height);
        }

        public void JumpToPosFixed(Vector3 pos, int jumpPower, int jumpCount, float dur, Action onComplete = null)
        {
            Enabled = true;


            Moving = true;

            m_CurrentTween = transform.DOJump(pos, jumpPower,
                jumpCount, (pos - Position).magnitude / dur);

            //m_CurrentTween.SetEase(m_Curve);

            m_CurrentTween.onComplete = () =>
            {
                OnComplete();
                onComplete?.Invoke();
            };
        }

        public void JumpToPosFixedRecursive(Transform target, int jumpPower, int jumpCount, float dur,
            float recursiveWaitTime = 0)
        {
            Enabled = true;


            Target = target;
            RecursiveWaitTime = recursiveWaitTime;
            Recursive = true;
            MoveDur = dur;

            JumpToPosFixed(Target.position, jumpPower, jumpCount, MoveDur);

            m_RecursiveAction = () => { JumpToPosFixed(Target.position, jumpPower, jumpCount, MoveDur); };
        }

        public void SnuckJumpMovement(Vector3 pos, int jumpPower, int jumpCount, float dur, Action onComplete = null)
        {
            if (m_Paused)
                return;

            m_Paused = true;

            Enabled = true;


            transform.DOPause();
            if (m_NavMeshed)
            {
                NavMeshEnabled = false;
                //NavMeshAgent().enabled = false;
            }

            transform.DOJump(pos, jumpPower,
                jumpCount, dur).OnComplete(() =>
            {
                m_Paused = false;

                onComplete?.Invoke();

                if (m_NavMeshed)
                {
                    NavMeshEnabled = true;
                    //NavMeshAgent().enabled = true;
                }
                else
                {
                    if (Recursive)
                    {
                        OnComplete();
                    }
                    else
                    {
                        transform.DOPlay();
                    }
                }
            });
        }

        public void SnuckMovement(Promise<bool> movementCompletePromise, Action onComplete = null)
        {
            PauseCurrentMovement();

            movementCompletePromise.OnResultT += (b, b1) => { ContinueCurrentMovement(onComplete); };
        }

        public void SnuckKnockbackMovement(Vector3 knockbackDirection, Action onComplete = null)
        {
            PauseCurrentMovement();

            var targetPos = transform.position + knockbackDirection;

            transform.DOMove(targetPos, 0.5f).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                ContinueCurrentMovement(onComplete);
            });
        }

        private void PauseCurrentMovement()
        {
            if (m_Paused)
                return;

            m_Paused = true;

            Enabled = true;

            transform.DOPause();

            if (m_NavMeshed)
            {
                NavMeshEnabled = false;
                NavMeshAgent().enabled = false;
            }

            SelfRigid().constraints = RigidbodyConstraints.FreezeAll;
        }

        private void ContinueCurrentMovement(Action onComplete = null)
        {
            m_Paused = false;

            onComplete?.Invoke();

            if (m_NavMeshed)
            {
                NavMeshEnabled = true;
                NavMeshAgent().enabled = true;
            }
            else
            {
                if (Recursive)
                {
                    OnComplete();
                }
                else
                {
                    transform.DOPlay();
                }
            }

            SelfRigid().constraints = RigidbodyConstraints.None;
        }

        private void OnComplete()
        {
            OnDone?.Invoke();
            m_RecursiveWaitConditional?.Cancel();

            if (!Recursive)
            {
                DisableSelf();
                return;
            }

            if (RecursiveWaitTime > 0)
            {
                Moving = false;

                m_RecursiveWaitConditional = Conditional.Wait(RecursiveWaitTime).Do(() =>
                {
                    Moving = true;
                    m_RecursiveAction.Invoke();
                });
            }
            else
            {
                m_RecursiveAction.Invoke();
            }
        }

        private Vector3 CalculateVelocity(float height, Vector3 targetPos)
        {
            float displacementY = targetPos.y - transform.position.y;
            Vector3 displacementXZ = new Vector3(targetPos.x - SelfTransform().position.x, 0,
                targetPos.z - SelfTransform().position.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * -9.81f * height);
            Vector3 velocityXZ = displacementXZ /
                                 (Mathf.Sqrt(-2 * height / -9.81f) + Mathf.Sqrt(2 * (displacementY - height) / -9.81f));

            return velocityY + velocityXZ;
        }

        private bool CheckIfAlignedVertical(float targetHeight)
        {
            return Math.Abs(SelfTransform().position.y - targetHeight) < 0.5f;
        }

        private bool CheckIfAlignedHorizontal(Vector3 targetPos)
        {
            return (Math.Abs(SelfTransform().position.x - targetPos.x) +
                    Math.Abs(SelfTransform().position.z - targetPos.z)) < 0.5f;
        }
    }
}