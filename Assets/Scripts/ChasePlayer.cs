using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    protected Transform player;
    protected NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Camera>().transform;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(player.position);
    }
}
