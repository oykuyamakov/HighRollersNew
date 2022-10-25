using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalPosSetter : MonoBehaviour
{
    [Button]
    private void Set()
    {
        transform.localPosition = Vector3.zero;
    }
}
