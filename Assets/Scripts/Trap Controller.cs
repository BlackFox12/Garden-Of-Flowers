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
        Destroy(explosion.gameObject, explosionDuration);

        Destroy(trap);
        trapsRemaining++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap")){
            other.isTrigger = false;
        }
    }
}
