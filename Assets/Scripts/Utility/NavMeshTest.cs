using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavMeshTest : MonoBehaviour
{
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();

    public Transform t;
    private void Update()
    {
        Agent.destination = t.position;
    }
}
