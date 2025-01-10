using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using System.Collections.Generic;

[Condition("AI/HasReachableItem")]
[Help("Checks if there are any reachable items on the map.")]
public class HasReachableItem : ConditionBase
{
    [InParam("aiAgent")]
    public GameObject aiAgent;

    public override bool Check()
    {
        if (aiAgent == null || ItemManager.Instance == null)
        {
            Debug.Log("HasReachableItem: Agent or ItemManager is null");
            return false;
        }

        List<Vector2> itemPositions = ItemManager.Instance.GetItemPositions();
        if (itemPositions.Count == 0)
        {
            return false;
        }

        // Check if we can reach any item
        foreach (Vector2 itemPosition in itemPositions)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(aiAgent.transform.position, itemPosition, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return true;
                }
            }
        }

        Debug.Log("HasReachableItem: No reachable items found");
        return false;
    }
} 