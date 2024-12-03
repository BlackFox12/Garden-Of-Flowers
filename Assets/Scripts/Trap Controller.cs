using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrapController : MonoBehaviour
{
   [Header("Trap")]
    public GameObject trapPrefab;
    public KeyCode inputKey = KeyCode.Space;
    public float trapFuseTime = 3f;
    public int trapAmount = 1;
    private int trapsRemaining;

    [Header("Explosion")]
    public Explosion explosionPrefab;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    private void OnEnable()
    {
        trapsRemaining = trapAmount;
    }

    private void Update()
    {
        if (trapsRemaining > 0 && Input.GetKeyDown(inputKey)) {
            StartCoroutine(placeTrap());
        }
    }

    private IEnumerator placeTrap()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject trap = Instantiate(trapPrefab, position, Quaternion.identity);
        trapsRemaining--;

        yield return new WaitForSeconds(trapFuseTime);

        position = trap.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        Destroy(explosion.gameObject, explosionDuration);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        Destroy(trap);
        trapsRemaining++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap")){
            other.isTrigger = false;
        }
    }

    public void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0) {
            return;
        }

        position += direction;

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        Destroy(explosion.gameObject, explosionDuration);

        Explode(position, direction, length-1); 
    }

}
