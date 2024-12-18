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

    public void GameOver()
    {
        gameObject.SetActive(true);
    }

    public void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
