using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections.Generic;

[Action("AI/DestroyFlowers")]
[Help("Finds and destroys the nearest flower by moving adjacent to it and placing a bomb.")]
public class DestroyFlowers : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("flowerManager")]
    public GameObject flowerManager;

    private BombController bombController;
    private AiAutoPath enemyMovement;
    private FlowerExtractor flowerExtractor;
    private bool bombPlaced;

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

        flowerExtractor = flowerManager.GetComponent<FlowerExtractor>();
        if (flowerExtractor == null || flowerExtractor.flowerPositions == null || flowerExtractor.flowerPositions.Count == 0)
        {
            Debug.LogWarning("No flowers found to destroy.");
        }

        bombPlaced = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null || flowerManager == null || flowerExtractor == null)
        {
            Debug.LogError("AI Agent or FlowerManager is not assigned.");
            return TaskStatus.FAILED;
        }

        Vector3 agentPosition = aiAgent.transform.position;

        if (!bombPlaced)
        {
            // Find the nearest flower
            Vector3 nearestFlower = FindNearestFlower(agentPosition, flowerExtractor.flowerPositions);
            if (nearestFlower == Vector3.zero)
            {
                Debug.LogWarning("No valid flower found.");
                return TaskStatus.FAILED;
            }

            // Find an adjacent walkable position near the flower
            Vector3 targetPosition = FindAdjacentWalkablePosition(nearestFlower);
            if (targetPosition == Vector3.zero)
            {
                Debug.LogWarning("No walkable position found near the flower.");
                return TaskStatus.FAILED;
            }

            // Move the AI towards the target position
            enemyMovement.SetTargetVector(targetPosition);

            // Check if the AI has reached the target position
            if (Vector3.Distance(agentPosition, targetPosition) <= 0.2f)
            {
                // Place a bomb
                bombController.PlaceBombExternally();
                bombPlaced = true;
                return TaskStatus.COMPLETED;
            }

            return TaskStatus.RUNNING;
        }

        return TaskStatus.COMPLETED;
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

    private Vector3 FindAdjacentWalkablePosition(Vector3 flowerPosition)
    {
        // Define directions to check around the flower (up, down, left, right)
        Vector3[] directions = new Vector3[]
        {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 adjacentPosition = flowerPosition + direction;

            // Check if the adjacent position is walkable
            NavMeshHit hit;
            if (NavMesh.SamplePosition(adjacentPosition, out hit, 0.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero; // No valid adjacent position found
    }
}
