using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Launcher : MonoBehaviour
{

    [SerializeField] private Transform m_Target;

    [SerializeField] private float height;

    [Button]
    private void ProjectileMove()
    {
        GetComponent<Rigidbody>().velocity = CalculateVelocity();
    }

    private Vector3 CalculateVelocity()
    {
        float displacementY = m_Target.transform.position.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(m_Target.transform.position.x - transform.position.x, 0,
            m_Target.transform.position.z - transform.position.z);
            
        Vector3 velocityY =Vector3.up * Mathf.Sqrt(-2 * -9.81f * height);
        Vector3 velocityXZ = displacementXZ /  (Mathf.Sqrt(-2 * height/ -9.81f) + Mathf.Sqrt(2 * (displacementY - height)/-9.81f));

        return velocityY + velocityXZ;
    }

}
