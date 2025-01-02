using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    public static GameMusicController Instance; // Instanz des GameMusicControllers (Singleton)
    public AudioSource musicSource; // Quelle für die Musik

    void Awake()
    {
        // Singleton, um Musik zwischen Szenen beizubehalten
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Das GameMusicController-Objekt bleibt zwischen Szenen erhalten
        }
        else
        {
            Destroy(gameObject); // Zerstört das zusätzliche GameMusicController-Objekt
        }

        musicSource.loop = true; // Setzt die Musikquelle so, dass sie sich wiederholt
        musicSource.Play(); // Startet die Musik
    }

    public void StopMusic()
    {
        // Stoppt die Musik, wenn sie gerade spielt
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop(); // Stoppt die Musikquelle
        }
    }
}