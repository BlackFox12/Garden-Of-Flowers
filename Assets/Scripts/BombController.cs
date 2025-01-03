using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
{


    [Header("Bomb")]
    public KeyCode inputKey = KeyCode.Space;
    public GameObject basketPrefab;
    public float bombFuseTime = 3f;
    public int bombAmount = 1;
    private int bombsRemaining;
    public static List<GameObject> ActiveBombs = new List<GameObject>();



    [Header("Explosion")]
    //public Explosion explosionPrefab;
    public GameObject bunnyPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    public float velocity = 5;



    [Header("Destructible")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;



    private void OnEnable()
    {
        bombsRemaining = bombAmount;
    }

    private void Update()
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey))
        {
            StartCoroutine(PlaceBomb());
        }
    }


    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject basket = Instantiate(basketPrefab, position, basketPrefab.transform.rotation);
        ActiveBombs.Add(basket); // Add bomb to the list
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = basket.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Destroy(basket);
        ActiveBombs.Remove(basket); // Remove bomb after it explodes
        bombsRemaining++;

        Explode(position, explosionRadius);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }


    private void Explode(Vector2 position, int length)
    {
        // Define directions: Up, Down, Left, Right
        Vector2[] directions = new Vector2[]
        {
            Vector2.right, Vector2.down, Vector2.left, Vector2.up
        };

        // Loop through all directions
        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = directions[i];

            // Adjust spawn rotation for visual effect
            Quaternion spawnRotation = Quaternion.identity;
            if (i == 0) { spawnRotation = bunnyPrefab.transform.localRotation; }
            else if (i == 1) { spawnRotation = Quaternion.Euler(90, 90, -70); }
            else if (i == 2) { spawnRotation = Quaternion.Euler(180, 90, -110); }
            else if (i == 3) { spawnRotation = Quaternion.Euler(-90, 90, -110); }
            
            // Instantiate the rabbit
            GameObject bunny = Instantiate(bunnyPrefab, position, spawnRotation);
            if (IsBlocked(position + direction) || IsBlocked(position)) {
                Destroy(bunny);
            }

            // Start coroutine to move the rabbit
            StartCoroutine(MoveRabbit(bunny, position, direction, length));
        }
    }

    private bool IsBlocked(Vector2 position)
    {
        // Check for blocking objects in the given position
        return Physics2D.OverlapBox(position, Vector2.one/2f, 0f, explosionLayerMask);
    }

    private IEnumerator MoveRabbit(GameObject bunny, Vector2 startPosition, Vector2 direction, int length)
    {
        
        float moveDuration = 1f;
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < length; i++) // Move for 'length' blocks
        {
            if (bunny != null) {
                Vector2 targetPosition = currentPosition + direction;
                float elapsedTime = 0f;

                // Smoothly move the rabbit to the target position
                while (elapsedTime < moveDuration)
                {
                    if (bunny == null) // Check if bunny was removed during movement
                    {
                        yield break; // Exit the coroutine early if bunny is no longer available
                    }
                    bunny.transform.position = Vector2.Lerp(currentPosition, targetPosition, elapsedTime / moveDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Snap to the exact target position
                bunny.transform.position = targetPosition;
                
                ClearDestructible(targetPosition, direction, bunny);


                // Update the current position for the next block
                currentPosition = targetPosition;
            }
        }
        // Destroy the rabbit after reaching its final position
        Destroy(bunny);
    }

    public void ClearDestructible(Vector2 position, Vector2 direction, GameObject bunny)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);
        if (IsBlocked(position + direction) || IsBlocked(position)) {
            Destroy(bunny);
        }

        if(tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
            Destroy(bunny);
        }
        
    }

    public void AddBomb()
    { 
        bombAmount++;
        bombsRemaining++;
    }
}