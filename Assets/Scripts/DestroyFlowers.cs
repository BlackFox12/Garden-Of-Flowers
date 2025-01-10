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
    private Vector3 targetFlowerPosition;
    private Vector3 selectedAdjacentPosition;
    private bool hasSelectedTarget = false;

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
        hasSelectedTarget = false;
        targetFlowerPosition = Vector3.zero;
        selectedAdjacentPosition = Vector3.zero;
    }

    public override TaskStatus OnUpdate()
    {
        if (aiAgent == null || flowerManager == null || flowerExtractor == null)
        {
            Debug.LogError("AI Agent or FlowerManager is not assigned.");
            return TaskStatus.FAILED;
        }

        Vector3 agentPosition = aiAgent.transform.position;

        // If we haven't selected a target yet, update flower positions and find one
        if (!hasSelectedTarget)
        {
            // Update flower positions before selecting target
            flowerExtractor.flowerPositions = flowerExtractor.ExtractFlowerPositions();
            
            if (flowerExtractor.flowerPositions.Count == 0)
            {
                Debug.Log("No flowers remaining.");
                return TaskStatus.COMPLETED;
            }

            targetFlowerPosition = FindNearestFlower(agentPosition, flowerExtractor.flowerPositions);
            if (targetFlowerPosition == Vector3.zero)
            {
                Debug.LogWarning("No valid flower found.");
                return TaskStatus.FAILED;
            }

            selectedAdjacentPosition = FindAdjacentWalkablePosition(targetFlowerPosition);
            if (selectedAdjacentPosition == Vector3.zero)
            {
                Debug.LogWarning("No walkable position found near the flower.");
                return TaskStatus.FAILED;
            }

            hasSelectedTarget = true;
            enemyMovement.SetTargetVector(selectedAdjacentPosition);
        }

        // Check if we've reached our selected adjacent position
        if (Vector3.Distance(agentPosition, selectedAdjacentPosition) < 0.1f)
        {
            if (!bombPlaced)
            {
                bombController.PlaceBombExternally();
                bombPlaced = true;
            }
            return TaskStatus.COMPLETED;
        }

        return TaskStatus.RUNNING;
    }

    private bool IsAdjacentToFlower(Vector3 position, Vector3 flowerPosition)
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (Vector3 direction in directions)
        {
            if (Vector3.Distance(position + direction, flowerPosition) < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    private bool CanWalkToPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(aiAgent.transform.position, position, NavMesh.AllAreas, path);
        return hasPath && path.status == NavMeshPathStatus.PathComplete;
    }

    private Vector3 FindAdjacentWalkablePosition(Vector3 flowerPosition)
    {
        // Round the flower position to ensure we're working with exact grid positions
        Vector3 exactFlowerPos = new Vector3(
            Mathf.Round(flowerPosition.x),
            Mathf.Round(flowerPosition.y),
            flowerPosition.z
        );

        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (Vector3 direction in directions)
        {
            // Calculate exact adjacent position (should be exactly 1 unit away)
            Vector3 adjacentPosition = exactFlowerPos + direction;
            
            // Check if position is orthogonally adjacent (using Manhattan distance)
            float xDiff = Mathf.Abs(adjacentPosition.x - exactFlowerPos.x);
            float yDiff = Mathf.Abs(adjacentPosition.y - exactFlowerPos.y);
            
            // Ensure exactly one coordinate differs by 1, and the other by 0
            if (!((xDiff == 1 && yDiff == 0) || (xDiff == 0 && yDiff == 1)))
            {
                continue;
            }

            // Check if we can actually reach this position
            if (!CanWalkToPosition(adjacentPosition))
            {
                continue;
            }

            // Check if position is on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(adjacentPosition, out hit, 0.1f, NavMesh.AllAreas))
            {
                // Ensure the NavMesh position hasn't shifted us off our exact position
                if (Vector3.Distance(hit.position, adjacentPosition) < 0.1f)
                {
                    // Check if the position is clear of obstacles
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(adjacentPosition, 0.1f);
                    bool isBlocked = false;
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
                            collider.gameObject.layer == LayerMask.NameToLayer("Brick") ||
                            collider.gameObject.layer == LayerMask.NameToLayer("Flower"))
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                    
                    if (!isBlocked)
                    {
                        return adjacentPosition;
                    }
                }
            }
        }

        Debug.LogWarning($"No valid orthogonal adjacent position found for flower at {exactFlowerPos}");
        return Vector3.zero;
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
