using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using System.Collections.Generic;


[Condition("AI/IsInDanger")]
[Help("Checks if the aiAigent is in danger from any active bombs.")]
public class IsInDangerCondition : ConditionBase
{
    private const float hitboxThickness = 0.9f; // Match the thickness used in Survive.cs

    [InParam("aiAgent")]
    [Help("The player object to check for danger.")]
    public GameObject aiAgent;

    [InParam("waitingForBomb")]
    [Help("Whether the AI is currently waiting for a bomb to explode.")]
    public bool waitingForBomb;

    private static bool dangerLocked = false;  // Shared static variable to maintain state

    public static void LockDanger()
    {
        dangerLocked = true;
    }

    public static void UnlockDanger()
    {
        dangerLocked = false;
    }

    public override bool Check()
    {
        if (aiAgent == null)
            return false;

        if (dangerLocked || waitingForBomb)
            return true;

        List<Vector2> activeBombs = BombManager.Instance.GetActiveBombs();
        if (activeBombs == null || activeBombs.Count == 0)
        {
            return false;
        }
            
        BombController bombController = aiAgent.GetComponent<BombController>();
        if (bombController == null)
        {
            Debug.LogError("BombController missing on aiAgent");
            return false;
        }

        Vector2 playerPosition = aiAgent.transform.position;
        float explosionRadius = bombController.explosionRadius + 1.0f; // Added safety buffer

        foreach (var bombPosition in activeBombs)
        {
            if (bombPosition == null)
                continue;

            // Check horizontal danger: same column but within the explosion radius
            if (Mathf.Abs(playerPosition.x - bombPosition.x) <= explosionRadius &&
                Mathf.Abs(playerPosition.y - bombPosition.y) <= hitboxThickness &&
                !IsObstacleBlocking(playerPosition, bombPosition))
            {
                LockDanger();
                return true;
            }

            // Check vertical danger: same row but within the explosion radius
            if (Mathf.Abs(playerPosition.y - bombPosition.y) <= explosionRadius &&
                Mathf.Abs(playerPosition.x - bombPosition.x) <= hitboxThickness &&
                !IsObstacleBlocking(playerPosition, bombPosition))
            {
                LockDanger();
                return true;
            }
        }
        return false;
    }

    private bool IsObstacleBlocking(Vector2 position, Vector2 bombPosition)
    {
        // Perform a raycast to check for obstacles blocking the explosion
        RaycastHit2D hitBrick = Physics2D.Raycast(bombPosition, position - bombPosition, Vector2.Distance(bombPosition, position), LayerMask.GetMask("Brick"));
        RaycastHit2D hitFlower = Physics2D.Raycast(bombPosition, position - bombPosition, Vector2.Distance(bombPosition, position), LayerMask.GetMask("Flower"));
        if (hitBrick.collider != null)
        {
            Debug.Log($"Obstacle detected between bomb and player: {hitBrick.collider.name}");
            return true;
        }
        if (hitFlower.collider != null)
        {
            Debug.Log($"Obstacle detected between bomb and player: {hitFlower.collider.name}");
            return true;
        }
        return (hitBrick.collider != null || hitFlower.collider != null);
    }
}
