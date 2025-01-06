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
    private AiAutoPath enemyMovement;

    public override void OnStart()
    {
        base.OnStart();
        bombController = aiAgent.GetComponent<BombController>();
        if (bombController == null)
        {
            Debug.LogError("BombController component missing on AI Agent.");
        }
        enemyMovement = aiAgent.GetComponent<AiAutoPath>();
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

        Vector2 agentPosition = aiAgent.transform.position;
        Vector2 playerPosition = player.transform.position;


        // Gå mot spelaren
        enemyMovement.SetTargetVector(playerPosition);

        // När tillräckligt nära, placera bomb
        if (Vector2.Distance(agentPosition, playerPosition) <= 1.0f)
        {
            bombController.PlaceBombExternally();
            Debug.Log("Bomb placed. Moving to safety.");
        }

        return TaskStatus.COMPLETED;
    }
}
