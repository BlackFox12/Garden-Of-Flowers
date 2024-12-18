using UnityEngine;

public class Bunny : MonoBehaviour
{
    /// <summary>
    /// public GameObject player1; // Reference to Player 1
    /// </summary>
    //public GameObject player2; // Reference to Player 2
    private GameObject[] players;
    public float offset = 1.0f; // Allowed offset for proximity
    private GameManager gameManager;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gameManager = FindObjectOfType<GameManager>();


    }

    void Update()
    {
        foreach (GameObject player in players) {
            if (IsWithinOffset(transform.position, player.transform.position))
            {
                TriggerDeathSequence(player);
            }
        }
    }

    private bool IsWithinOffset(Vector3 bunnyPosition, Vector3 playerPosition)
    {
        return Mathf.Abs(bunnyPosition.x - playerPosition.x) <= offset &&
               Mathf.Abs(bunnyPosition.y - playerPosition.y) <= offset;
    }

    private void TriggerDeathSequence(GameObject player)
    {
        // Get the Movement script attached to the player and trigger DeathSequence
        Movement movementScript = player.GetComponent<Movement>();
        if (movementScript != null)
        {
            Debug.Log("Triggering DeathSequence for " + player.name);
            movementScript.DeathSequence();
        }
        else
        {
            Debug.LogWarning("No Movement script found on " + player.name);
        }
        if (player.name == "Player2") {
            gameManager.GameOver(true);
        } else {
            gameManager.GameOver(false);
        }
    }
}
