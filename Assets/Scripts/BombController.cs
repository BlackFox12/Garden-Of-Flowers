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

    [Header("Explosion")]
    public GameObject bunnyPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    public float velocity = 5;

    [Header("Bunny Movement")]
    public float bunnySpeed = 1f; // Bunny movement speed multiplier

    [Header("Destructible")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;

    [Header("AI Auto Pathing")]
    public GameObject aiAgent; 
    public AiAutoPath aiAutoPath;

    private void OnEnable()
    {
        bombsRemaining = bombAmount;
        if (aiAgent == null)
        {
            Debug.LogError("aiAgent not assigned");
        }
        if (aiAutoPath == null)
        {
            aiAutoPath = aiAgent.GetComponent<AiAutoPath>();
        }
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
        BombManager.Instance.AddBombPosition(basket.transform.position); // Add bomb to BombManager
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = basket.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        BombManager.Instance.RemoveBomb(basket.transform.position); // Remove bomb from BombManager
        Destroy(basket);
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
        Vector2[] directions = new Vector2[]
        {
            Vector2.right, Vector2.down, Vector2.left, Vector2.up
        };

        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = directions[i];

            Quaternion spawnRotation = Quaternion.identity;
            if (i == 0) { spawnRotation = bunnyPrefab.transform.localRotation; }
            else if (i == 1) { spawnRotation = Quaternion.Euler(90, 90, -70); }
            else if (i == 2) { spawnRotation = Quaternion.Euler(180, 90, -110); }
            else if (i == 3) { spawnRotation = Quaternion.Euler(-90, 90, -110); }

            GameObject bunny = Instantiate(bunnyPrefab, position, spawnRotation);
            if (IsBlocked(position + direction) || IsBlocked(position))
            {
                Destroy(bunny);
            }

            StartCoroutine(MoveRabbit(bunny, position, direction, length));
        }
    }

    private bool IsBlocked(Vector2 position)
    {
        return Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
    }

    private IEnumerator MoveRabbit(GameObject bunny, Vector2 startPosition, Vector2 direction, int length)
    {
        float baseMoveDuration = 1f; // Base duration for moving one block
        float moveDuration = baseMoveDuration / bunnySpeed; // Adjusted by bunnySpeed
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < length; i++) // Move for 'length' blocks
        {
            if (bunny != null)
            {
                Vector2 targetPosition = currentPosition + direction;
                float elapsedTime = 0f;

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

                bunny.transform.position = targetPosition;
                ClearDestructible(targetPosition, direction, bunny);
                currentPosition = targetPosition;
            }
        }

        Destroy(bunny);
    }

    public void ClearDestructible(Vector2 position, Vector2 direction, GameObject bunny)
    {
        if (destructibleTiles == null)
        {
            Debug.LogError("DestructibleTiles is not assigned to BombController.");
            return;
        }

        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (IsBlocked(position + direction) || IsBlocked(position))
        {
            Destroy(bunny);
        }

        if (tile == null)
        {
            return;
        }

        Instantiate(destructiblePrefab, position, Quaternion.identity);
        destructibleTiles.SetTile(cell, null);
        Destroy(bunny);

        if (aiAutoPath != null)
        {
            aiAutoPath.RequestNavMeshUpdate();
        }
        else
        {
            Debug.LogWarning("aiAutoPath is null. NavMesh update not requested.");
        }
    }


    public void PlaceBombExternally()
    {
        if (bombsRemaining > 0)
        {
            StartCoroutine(PlaceBomb());
        }
    }

    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }
}
