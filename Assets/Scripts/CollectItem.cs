using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections.Generic;

[Action("AI/CollectItem")]
[Help("Moves to the nearest reachable item and collects it.")]
public class CollectItem : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    private AiAutoPath enemyMovement;
    private Vector2? targetItemPosition = null;
    private const float collectionDistance = 0.5f;

    public override void OnStart()
    {
        base.OnStart();
        
        if (aiAgent == null)
        {
            Debug.LogError("AI Agent is not assigned.");
            return;
        }

        enemyMovement = aiAgent.GetComponent<AiAutoPath>();
        if (enemyMovement == null)
        {
            Debug.LogError("AiAutoPath component missing on AI Agent.");
            return;
        }

        // Find nearest reachable item
        targetItemPosition = FindNearestReachableItem();
        if (targetItemPosition.HasValue)
        {
            enemyMovement.SetTargetVector(targetItemPosition.Value);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (!targetItemPosition.HasValue)
        {
            return TaskStatus.FAILED;
        }

        float distanceToItem = Vector2.Distance(aiAgent.transform.position, targetItemPosition.Value);
        
        if (distanceToItem <= collectionDistance)
        {
            // Try to collect the item
            GameObject item = ItemManager.Instance.GetItemAtPosition(targetItemPosition.Value);
            if (item != null)
            {
                ItemManager.Instance.RemoveItem(item);
                UnityEngine.Object.Destroy(item);
                targetItemPosition = null;  // Clear the target position after collecting
                return TaskStatus.COMPLETED;
            }
            
            // If we can't find the item at the position, clear the target and fail
            targetItemPosition = null;
            return TaskStatus.FAILED;
        }

        return TaskStatus.RUNNING;
    }

    private Vector2? FindNearestReachableItem()
    {
        List<Vector2> itemPositions = ItemManager.Instance.GetItemPositions();
        Vector2? nearestPosition = null;
        float nearestDistance = float.MaxValue;
        Vector3 agentPosition = aiAgent.transform.position;

        foreach (Vector2 itemPosition in itemPositions)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(agentPosition, itemPosition, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    float distance = Vector2.Distance(agentPosition, itemPosition);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPosition = itemPosition;
                    }
                }
            }
        }

        return nearestPosition;
    }
} 