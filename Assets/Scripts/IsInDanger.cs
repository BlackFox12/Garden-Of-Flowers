using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using Pada1.BBCore.Framework;

[Condition("AI/IsInDanger")]
[Help("Checks if the player is in danger from any active bombs.")]
public class IsInDangerCondition : ConditionBase
{
    [InParam("player")]
    [Help("The player object to check for danger.")]
    public GameObject player;

    public override bool Check()
    {
        if (player == null || BombController.ActiveBombs == null || BombController.ActiveBombs.Count == 0)
            return false;

        BombController bombController = player.GetComponent<BombController>();
        if (bombController == null)
            return false;

        Vector2 playerPosition = player.transform.position;

        foreach (var bomb in BombController.ActiveBombs)
        {
            if (bomb == null)
                continue;

            Vector2 bombPosition = bomb.transform.position;

            // Use the explosion radius from the BombController
            float distance = Vector2.Distance(playerPosition, bombPosition);
            if (distance <= bombController.explosionRadius)
            {
                Debug.Log("Is In Danger");
                return true; // Player is in danger
            }
        }

        return false; // Player is not in danger
    }
}
