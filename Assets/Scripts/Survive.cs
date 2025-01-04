using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

[Action("AI/Survive")]
[Help("Moves to the nearest safe zone if the agent is in danger.")]
public class Survive : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("bombRadius")]
    public float bombRadius;

    private NavMeshAgentMovement enemyMovement;

    public override void OnStart()
    {
        base.OnStart();

        enemyMovement = aiAgent.GetComponent<NavMeshAgentMovement>();
        if (enemyMovement == null)
        {
            Debug.LogError("NavMeshAgentMovement component missing on AI Agent.");
        }
    }
    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null)
        {
            Debug.LogError("AI Agent is not assigned.");
            return TaskStatus.FAILED;
        }

        // Check for bombs in radius
        Collider[] bombs = Physics.OverlapSphere(aiAgent.transform.position, bombRadius, LayerMask.GetMask("Bomb"));

        // Move to the safest point (opposite to bomb positions)
        Vector3 safestPoint = aiAgent.transform.position;
        foreach (var bomb in bombs)
        {
            Vector3 directionAway = aiAgent.transform.position - bomb.transform.position;
            safestPoint += directionAway.normalized * bombRadius;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(safestPoint, out hit, bombRadius, NavMesh.AllAreas))
        {
            enemyMovement.SetTargetVector(hit.position);
            Debug.Log("Running away to safe place");
            return TaskStatus.COMPLETED;
        }
        

        return TaskStatus.FAILED;
    }
}
