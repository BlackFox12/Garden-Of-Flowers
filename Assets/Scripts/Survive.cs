using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections.Generic;

[Action("AI/Survive")]
[Help("Moves to the nearest safe zone if the agent is in danger.")]
public class Survive : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    [InParam("waitingForBomb")]
    public bool waitingForBomb;

    private float explosionRadius;
    private BombController bombController;
    private AiAutoPath enemyMovement;
    private const float hitboxThicknes = 0.9f;
    private const float hitboxRadius = 0.3f;

    // Add new fields to track the target position
    private Vector2? targetSafePosition = null;
    private bool isMovingToSafePosition = false;
    private bool isWaitingAtSafePosition = false;
    private float waitBufferTime = 2.5f; // Additional time to wait after bomb explodes
    private float waitEndTime = 0f;

    public override void OnStart()
    {
        base.OnStart();
        IsInDangerCondition.LockDanger();  // Lock danger state when Survive starts

        if (aiAgent == null)
        {
            Debug.LogError("AI Agent is not assigned.");
        }

        enemyMovement = aiAgent.GetComponent<AiAutoPath>();
        if (enemyMovement == null)
        {
            Debug.LogError("AiAutoPath component missing on AI Agent.");
        }

        bombController = aiAgent.GetComponent<BombController>();
        if (bombController == null)
        {
            Debug.LogError("BombController component missing on AI Agent.");
        }

        explosionRadius = bombController.explosionRadius;
    }

    public override TaskStatus OnUpdate()
    {
        Vector2 currentPosition = aiAgent.transform.position;
        List<Vector2> activeBombs = BombManager.Instance.GetActiveBombs();

        // If we're waiting at a safe position, check if it's safe to move
        if (isWaitingAtSafePosition)
        {
            waitingForBomb = true; // Set the out parameter
            if (Time.time < waitEndTime)
            {
                return TaskStatus.RUNNING; // Keep waiting
            }
            
            // After wait time is over, reset states
            ResetStates();
            IsInDangerCondition.UnlockDanger();  // Only unlock danger when we're done waiting
            return TaskStatus.COMPLETED;
        }

        // If we're moving to a safe position, check if we've reached it
        if (isMovingToSafePosition && targetSafePosition.HasValue)
        {
            float distanceToTarget = Vector2.Distance(currentPosition, targetSafePosition.Value);
            if (distanceToTarget < 0.3f) // Close enough to consider "arrived"
            {
                isMovingToSafePosition = false;
                isWaitingAtSafePosition = true;
                // Wait for the bomb fuse time plus buffer
                waitEndTime = Time.time + waitBufferTime;
                return TaskStatus.RUNNING;
            }
            return TaskStatus.RUNNING; // Keep moving to the target
        }

        // Only search for a new safe position if we're not already moving to one
        if (!isMovingToSafePosition)
        {
            targetSafePosition = FindSafePosition(currentPosition, activeBombs);
            if (targetSafePosition.HasValue)
            {
                enemyMovement.SetTargetVector(targetSafePosition.Value);
                isMovingToSafePosition = true;
                return TaskStatus.RUNNING;
            }
            
            Debug.Log("No safe position found.");
            return TaskStatus.FAILED;
        }

        return TaskStatus.RUNNING;
    }

    private Vector2? FindSafePosition(Vector2 currentPosition, List<Vector2> activeBombs)
    {
        int maxSearchRadius = 10;
        for (int radius = 0; radius <= maxSearchRadius; radius++)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                float radian = angle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * radius;
                Vector2 testPosition = currentPosition + offset;

                if (IsSafe(testPosition, activeBombs) && CanWalkToPosition(testPosition) && IsClearOfWalls(testPosition))
                {
                    return testPosition;
                }
            }
        }
        return null;
    }

    private bool IsSafe(Vector2 position, List<Vector2> activeBombs)
    {
        // Adjust the radius with a safety buffer
        float adjustedRadius = explosionRadius + hitboxRadius + 1.0f;

        foreach (Vector2 bombPosition in activeBombs)
        {
            // Check the horizontal safety: same column but within the danger radius
            if (Mathf.Abs(position.x - bombPosition.x) <= adjustedRadius &&
                Mathf.Abs(position.y - bombPosition.y) <= hitboxThicknes)
            {
                return false;
            }

            // Check the vertical safety: same row but within the danger radius
            if (Mathf.Abs(position.y - bombPosition.y) <= adjustedRadius &&
                Mathf.Abs(position.x - bombPosition.x) <= hitboxThicknes)
            {
                return false;
            }
        }
        return true;
    }


    private bool CanWalkToPosition(Vector2 position)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(aiAgent.transform.position, position, NavMesh.AllAreas, path);
        return hasPath && path.status == NavMeshPathStatus.PathComplete;
    }

    private bool IsClearOfWalls(Vector2 position)
    {
        // Check if the hitbox fits within the cell
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(position, hitboxRadius, LayerMask.GetMask("Wall", "Brick", "Flower"));
        return nearbyColliders.Length == 0; // No walls or barriers within the hitbox radius
    }

    private void ResetStates()
    {
        targetSafePosition = null;
        isMovingToSafePosition = false;
        isWaitingAtSafePosition = false;
        waitEndTime = 0f;
        waitingForBomb = false;
    }
}
