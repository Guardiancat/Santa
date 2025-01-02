using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Methode zum Neustarten des Spiels
    public void RestartGame()
    {
        Time.timeScale = 1; // Setzt die Zeit wieder auf Normalgeschwindigkeit
        SceneManager.LoadScene("HauptScene"); // Lädt die Hauptszene neu
    }

    // Methode zum Beenden des Spiels
    public void QuitGame()
    {
        Debug.Log("QuitGame-Methode wurde aufgerufen"); // Gibt in der Konsole eine Nachricht aus
        Application.Quit(); // Beendet das Spiel
    }
}
