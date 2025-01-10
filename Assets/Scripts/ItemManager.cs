using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public static ItemManager Instance { get { return instance; } }

    private List<Vector2> itemPositions = new List<Vector2>();
    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(GameObject item)
    {
        items.Add(item);
        itemPositions.Add(item.transform.position);
    }

    public void RemoveItem(GameObject item)
    {
        // First try to find by reference
        int index = items.IndexOf(item);
        
        // If not found by reference, try to find by position
        if (index == -1)
        {
            Vector2 itemPosition = item.transform.position;
            index = itemPositions.FindIndex(pos => Vector2.Distance(pos, itemPosition) < 0.1f);
        }

        if (index != -1)
        {
            items.RemoveAt(index);
            itemPositions.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning($"Failed to remove item at position {item.transform.position}. Item not found in lists.");
        }
    }

    public List<Vector2> GetItemPositions()
    {
        return new List<Vector2>(itemPositions); // Return a copy to prevent external modifications
    }

    public GameObject GetItemAtPosition(Vector2 position)
    {
        int index = itemPositions.FindIndex(pos => Vector2.Distance(pos, position) < 0.1f);
        if (index != -1)
        {
            return items[index];
        }
        return null;
    }

    // Optional: Add a method to validate and clean up the lists
    private void Update()
    {
        // Remove any null references from the lists
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                itemPositions.RemoveAt(i);
            }
        }
    }
} 