using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;

[Condition("AI/HasPathToTarget")]
[Help("Checks if there is a path to the player using NavMesh.")]
public class HasPathToTarget : ConditionBase
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("player")]
    public Transform player;

    public override bool Check()
    {
        if (aiAgent == null || player == null)
        {
            Debug.LogError("AI Agent or Player is not assigned.");
            return false;
        }

        NavMeshAgent navAgent = aiAgent.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on AI Agent.");
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(aiAgent.transform.position, player.position, NavMesh.AllAreas, path);
        Debug.Log("hasPath = " + hasPath);

        return hasPath && path.status == NavMeshPathStatus.PathComplete;
    }
}
