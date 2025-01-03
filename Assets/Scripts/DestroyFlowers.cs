using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

[Action("AI/DestroyFlowers")]
[Help("Default behavior: destroys destructible flowers by placing bombs.")]
public class DestroyFlowers : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("flower")]
    public Transform flower;

    [InParam("bombPrefab")]
    public GameObject bombPrefab;

    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null || flower == null || bombPrefab == null)
        {
            Debug.LogError("AI Agent, Flower, or Bomb Prefab is not assigned.");
            return TaskStatus.FAILED;
        }

        NavMeshAgent navAgent = aiAgent.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on AI Agent.");
            return TaskStatus.FAILED;
        }

        // Move to the flower's position and place a bomb
        navAgent.SetDestination(flower.position);

        Vector3 bombPosition = flower.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);
        GameObject.Instantiate(bombPrefab, bombPosition, Quaternion.identity);

        return TaskStatus.COMPLETED;
    }
}
