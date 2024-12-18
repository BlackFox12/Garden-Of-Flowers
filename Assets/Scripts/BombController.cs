using System.Collections;
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
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = basket.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Destroy(basket);
        bombsRemaining++;

        Explode2(position, explosionRadius);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }



    private void Explode(Vector2 position, int length){
        Quaternion spawnRotation = bunnyPrefab.transform.localRotation;
        for (int i = 0; i < 4; i++) {
            if (i == 1) {
                spawnRotation = Quaternion.Euler(90, 90, -70);
            } else if (i == 2) {
                spawnRotation = Quaternion.Euler(180, 90, -110);
            } else if (i == 3) {
                spawnRotation = Quaternion.Euler(-90, 90, -110);
            } else {
                spawnRotation = bunnyPrefab.transform.localRotation; // Default rotation
        }

        GameObject bunny = Instantiate(bunnyPrefab, position, spawnRotation);
        }
    }


    private void Explode2(Vector2 position, int length)
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

            if (IsBlocked(position + direction))
            {
                continue; // Skip this direction and move to the next one
            }

            // Adjust spawn rotation for visual effect
            Quaternion spawnRotation = Quaternion.identity;
            if (i == 0) { spawnRotation = bunnyPrefab.transform.localRotation; }
            else if (i == 1) { spawnRotation = Quaternion.Euler(90, 90, -70); }
            else if (i == 2) { spawnRotation = Quaternion.Euler(180, 90, -110); }
            else if (i == 3) { spawnRotation = Quaternion.Euler(-90, 90, -110); }
            
            // Instantiate the rabbit
            GameObject bunny = Instantiate(bunnyPrefab, position, spawnRotation);

            // Start coroutine to move the rabbit
            StartCoroutine(MoveRabbit(bunny, position, direction, length));
        }
    }

    private bool IsBlocked(Vector2 position)
    {
        // Check for blocking objects in the given position (e.g., flowers or bricks)
        return Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
    }

    private IEnumerator MoveRabbit(GameObject bunny, Vector2 startPosition, Vector2 direction, int length)
    {
        float moveDuration = 1f;
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < length; i++) // Move for 'length' blocks
        {
            Vector2 targetPosition = currentPosition + direction;
            float elapsedTime = 0f;

            // Smoothly move the rabbit to the target position
            while (elapsedTime < moveDuration)
            {
                bunny.transform.position = Vector2.Lerp(currentPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Snap to the exact target position
            bunny.transform.position = targetPosition;
            
            ClearDestructible(targetPosition);

            // Update the current position for the next block
            currentPosition = targetPosition;
        }

        // Destroy the rabbit after reaching its final position
        Destroy(bunny);
    }

    public void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if(tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }

    public void AddBomb()
    { 
        bombAmount++;
        bombsRemaining++;
    }
}