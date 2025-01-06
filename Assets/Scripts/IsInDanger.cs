using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using System.Collections.Generic;


[Condition("AI/IsInDanger")]
[Help("Checks if the aiAigent is in danger from any active bombs.")]
public class IsInDangerCondition : ConditionBase
{
    [InParam("aiAgent")]
    [Help("The player object to check for danger.")]
    public GameObject aiAgent;

    public override bool Check()
    {
        if (aiAgent == null)
            return false;

        List<Vector2> activeBombs = BombManager.Instance.GetActiveBombs();
        if (activeBombs == null || activeBombs.Count == 0)
        {
            return false;
        }
            
        // Calculate explosion range (radius + spread of rabbits)
        BombController bombController = aiAgent.GetComponent<BombController>();
        if (bombController == null)
        {
            Debug.LogError("BombController missing on aiAgent");
            return false;
        }

        Vector2 playerPosition = aiAgent.transform.position;

        foreach (var bombPosition in activeBombs)
        {
            if (bombPosition == null)
                continue;

            float explosionRadius = bombController.explosionRadius + 1.0f;

            if (Vector2.Distance(playerPosition, bombPosition) <= explosionRadius &&
                !IsObstacleBlocking(playerPosition, bombPosition))
            {

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
