using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int score = 0; // Aktueller Punktestand
    public TextMeshProUGUI scoreText; // Textfeld zur Anzeige des Punktestands
    public GameObject gameOverUI; // Referenz auf das GameOver UI

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Das GameManager-Objekt bleibt zwischen Szenen erhalten
        }
        else
        {
            Destroy(gameObject); // Zerstört das zusätzliche GameManager-Objekt
        }
    }

    void Start()
    {
        UpdateScoreDisplay(); // Aktualisiert die Punktzahl-Anzeige zu Beginn des Spiels
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // Versteckt den GameOver-Bildschirm zu Beginn
        }
    }

    public void AddScore(int value)
    {
        score += value; // Erhöht den Punktestand
        UpdateScoreDisplay(); // Aktualisiert die Punktzahl-Anzeige
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Punkte: " + score; // Setzt den Text der Punktzahl-Anzeige
        }
        else
        {
            Debug.LogWarning("ScoreText ist nicht im GameManager zugewiesen!"); // Warnung, wenn kein Textfeld zugewiesen ist
        }
    }

    public void GameOver()
    {
        Debug.Log("Spiel beendet"); // Ausgabe in der Konsole, wenn das Spiel vorbei ist
        Time.timeScale = 0; // Stoppt die Zeit im Spiel (macht das Spiel pausiert)

        if (GameMusicController.Instance != null)
        {
            GameMusicController.Instance.StopMusic(); // Stoppt die Musik, wenn der GameMusicController vorhanden ist
        }

        SceneManager.LoadScene("GameOver"); // Lädt die GameOver-Szene
    }
}

   