using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utility
{
    public class BGObjectMover : MonoBehaviour
    {
        private void Awake()
        {
            var s = Random.insideUnitCircle;
            GetComponent<Rigidbody>().velocity = new Vector3(s.x * Random.Range(1, 10), 0, s.y * UnityEngine.Random.Range(1, 10));
        }
    }
}
