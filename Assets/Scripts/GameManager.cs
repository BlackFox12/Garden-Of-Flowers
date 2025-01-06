using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameObject[] players;

    public GameObject gameOverPanel; // Reference to the Game Over UI Panel
    public Button restartButton;
    public TextMeshProUGUI  gameOverText;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        gameOverPanel.SetActive(false);
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(NewRound);
        }
    }

    public void CheckWinState()
    {
        int aliveCount = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf)
            {
                aliveCount++;
            }
        }

        if (aliveCount <= 1)
        {
            //Invoke(nameof(NewRound), 3f);
        }
    }

    public void GameOver(bool won)
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        gameOverPanel.SetActive(true);

        // Set the appropriate message based on win/loss
        if (gameOverText != null)
        {

            if (won)
            {
                gameOverText.text = "You Won!";
            }
            else
            {
                gameOverText.text = "Game Over";
            }
        }

        //Debug.Log(won ? "You won!" : "Game over");
    }

    public void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
