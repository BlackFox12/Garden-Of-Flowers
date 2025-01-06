using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    private List<Vector2> activeBombs = new List<Vector2>();
    public float delayToRemoveBomb = 0.2f;

    private void Awake()
    {
        // Singleton pattern to ensure one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBombPosition(Vector2 bombPosition)
    {
        if (!activeBombs.Contains(bombPosition))
        {
            activeBombs.Add(bombPosition);
        }
    }

    public void RemoveBomb(Vector2 bombPosition)
    {
        if (activeBombs.Contains(bombPosition))
        {
            StartCoroutine(RemoveBombAfterDelay(bombPosition));
            //activeBombs.Remove(bombPosition);
        }
    }

    private IEnumerator RemoveBombAfterDelay(Vector2 bombPosition)
    {
        yield return new WaitForSeconds(delayToRemoveBomb); // Wait for the specified delay
        if (activeBombs.Contains(bombPosition))
        {
            activeBombs.Remove(bombPosition);
        }
    }

    public List<Vector2> GetActiveBombs()
    {
        return new List<Vector2>(activeBombs); // Return a copy of the list to avoid external modifications
    }
}
