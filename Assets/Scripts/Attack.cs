using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

[Action("AI/Attack")]
[Help("Moves towards the player and places a bomb if there is a clear path.")]
public class Attack : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("player")]
    public Transform player;

    [InParam("bombPrefab")]
    public GameObject bombPrefab;

    [OutParam("hasClearPath")]
    public bool hasClearPath;

    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null || player == null || bombPrefab == null)
        {
            Debug.LogError("AI Agent, Player, or Bomb Prefab is not assigned.");
            hasClearPath = false;
            return TaskStatus.FAILED;
        }

        NavMeshAgent navAgent = aiAgent.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on AI Agent.");
            hasClearPath = false;
            return TaskStatus.FAILED;
        }

        // Check for clear path using Raycast
        Vector3 direction = player.position - aiAgent.transform.position;
        if (Physics.Raycast(aiAgent.transform.position, direction, out RaycastHit hit))
        {
            hasClearPath = hit.transform == player;
        }
        else
        {
            hasClearPath = false;
        }

        if (hasClearPath)
        {
            // Move towards the player and place a bomb
            navAgent.SetDestination(player.position);

            Vector3 bombPosition = player.position;
            bombPosition.x = Mathf.Round(bombPosition.x);
            bombPosition.y = Mathf.Round(bombPosition.y);
            GameObject.Instantiate(bombPrefab, bombPosition, Quaternion.identity);

            return TaskStatus.COMPLETED;
        }

        return TaskStatus.FAILED;
    }
}
