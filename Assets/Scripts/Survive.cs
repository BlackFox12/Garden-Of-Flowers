using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections;
using System.Collections.Generic;

[Action("AI/Survive")]
[Help("Moves to the nearest safe zone if the agent is in danger.")]
public class Survive : BasePrimitiveAction
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    private float explosionRadius;
    private BombController bombController;
    private AiAutoPath enemyMovement;
    private const float hitboxRadius = 0.25f;

    public override void OnStart()
    {
        base.OnStart();

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

        // Perform a circular search for a safe spot
        List<Vector2> activeBombs = BombManager.Instance.GetActiveBombs();
        if (activeBombs == null || activeBombs.Count == 0)
        {
            return TaskStatus.COMPLETED;
        }
        int maxSearchRadius = 10; // Maximum distance to search
        for (int radius = 1; radius <= maxSearchRadius; radius++)
        {
            for (int angle = 0; angle < 360; angle += 45) // Check in increments of 45 degrees
            {
                // Calculate the offset position in the circular pattern
                float radian = angle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * radius;
                Vector2 testPosition = currentPosition + offset;

                // Check if the position is safe, valid, and clear of barriers considering the circular hitbox
                if (IsSafe(testPosition) && CanWalkToPosition(testPosition) && IsClearOfWalls(testPosition))
                {
                    enemyMovement.SetTargetVector(testPosition);
                    return TaskStatus.COMPLETED;
                }
            }
        }
        Debug.Log("Amount of bombs when there is no safe pos: " + activeBombs.Count);
        Debug.Log("No safe position found.");
        return TaskStatus.FAILED; // No safe spot was found
    }

    private bool IsSafe(Vector2 position)
    {
        List<Vector2> activeBombs = BombManager.Instance.GetActiveBombs();
        foreach (Vector2 bombPosition in activeBombs)
        {
            if (Vector2.Distance(position, bombPosition) <= explosionRadius &&
                !IsObstacleBlocking(position, bombPosition))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsObstacleBlocking(Vector2 position, Vector2 bombPosition)
    {
        // Perform a raycast to check for obstacles blocking the explosion
        RaycastHit2D hit = Physics2D.Raycast(bombPosition, position - bombPosition, Vector2.Distance(bombPosition, position), LayerMask.GetMask("Brick", "Flower"));
        return (hit.collider != null);
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
}
