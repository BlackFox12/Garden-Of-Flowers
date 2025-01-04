using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections.Generic;

[Action("AI/DestroyFlowers")]
[Help("Finds and destroys the nearest flower using FlowerExtractor.")]
public class DestroyFlowers : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("flowerManager")]
    public GameObject flowerManager;

    private BombController bombController;
    private NavMeshAgentMovement enemyMovement;
    private FlowerExtractor flowerExtractor;
    public override void OnStart()
    {
        base.OnStart();
        if (aiAgent == null || flowerManager == null)
        {
            Debug.LogError("AI Agent or FlowerManager is not assigned.");
        }

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

        flowerExtractor = flowerManager.GetComponent<FlowerExtractor>();
        if (flowerExtractor == null || flowerExtractor.flowerPositions == null || flowerExtractor.flowerPositions.Count == 0)
        {
            Debug.LogWarning("No flowers found to destroy.");
        }
    }

    public override TaskStatus OnUpdate()
    {

        // Get FlowerExtractor component and flower positions
        

        // Find the nearest flower
        Vector3 agentPosition = aiAgent.transform.position;
        Vector3 nearestFlower = FindNearestFlower(agentPosition, flowerExtractor.flowerPositions);

        if (nearestFlower == Vector3.zero)
        {
            Debug.LogWarning("No valid flower found.");
            return TaskStatus.FAILED;
        }

        // Move to the flower's position
        enemyMovement.SetTargetVector(nearestFlower);
        Debug.Log("Moving towards flower");

        // Check if the agent is close enough to place the bomb
        /*if (Vector3.Distance(agentPosition, nearestFlower) <= 1f)
        {
            bombController.PlaceBombExternally();

            // Move the AI to a safe distance
            int safeDistance = bombController.explosionRadius;
            Vector3 safePosition = agentPosition + (agentPosition - nearestFlower).normalized * safeDistance;
            Debug.Log("Moving to safety");
            enemyMovement.SetTargetVector(safePosition);

            return TaskStatus.COMPLETED;
        }*/

        // Continue moving towards the flower
        return TaskStatus.RUNNING;
    }

    private Vector3 FindNearestFlower(Vector3 agentPosition, List<Vector3> flowerPositions)
    {
        Vector3 nearestFlower = Vector3.zero;
        float shortestDistance = Mathf.Infinity;

        foreach (Vector3 flowerPosition in flowerPositions)
        {
            float distance = Vector3.Distance(agentPosition, flowerPosition);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestFlower = flowerPosition;
            }
        }

        return nearestFlower;
    }
}
