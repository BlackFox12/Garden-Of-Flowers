using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

[Action("AI/Attack")]
[Help("Moves towards the player and places a bomb.")]
public class Attack : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("player")]
    public Transform player;

    private BombController bombController;
    private NavMeshAgentMovement enemyMovement;
    public override void OnStart()
    {
        base.OnStart();

        bombController = aiAgent.GetComponent<BombController>();
        if (bombController == null)
        {
            Debug.LogError("BombController component missing on AI Agent.");
        }
        enemyMovement = aiAgent.GetComponent<NavMeshAgentMovement>();
        if (enemyMovement == null)
        {
            Debug.LogError("NavMeshAgentMovement component missing on AI Agent.");
        }
    }
    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null || player == null)
        {
            Debug.LogError("AI Agent or Player is not assigned.");
            return TaskStatus.FAILED;
        }

        // Set destination towards player
        enemyMovement.SetTarget(player);

        // Place a bomb when close to the player
        if (Vector3.Distance(aiAgent.transform.position, player.position) <= 1.0f)
        {
            bombController.PlaceBombExternally();
            return TaskStatus.COMPLETED;
        }

        return TaskStatus.RUNNING;
    }
}
