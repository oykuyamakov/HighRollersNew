using System;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CameraManagement
{
    public class IsoCameraFollow : MonoBehaviour
    {
        [SerializeField] 
        private Transform m_Target;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField] 
        private bool m_Limitable;

        [SerializeField] private Vector2 yLimit;
        [SerializeField] private Vector2 xLimit;
        [SerializeField] private Vector2 zLimit;

        private void Update()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            transform.position = (m_Target.transform.position + m_Offset).WithY(transform.position.y);

            if (m_Limitable)
            {
                // var y = yLimit.y > transform.position.y ? 
                //     yLimit.x < transform.position.y ?transform.position.y : yLimit.x: yLimit.y;
                // transform.position.WithY(y);
                var x = xLimit.y > transform.position.x ? 
                    xLimit.x < transform.position.x ?transform.position.x : xLimit.x: xLimit.y;
                transform.position = transform.position.WithX(x);
                var z = zLimit.y > transform.position.z ? 
                    zLimit.x < transform.position.z ?transform.position.z : zLimit.x: zLimit.y;
                transform.position = transform.position.WithZ(z);
                
            }
        }
    }
}
