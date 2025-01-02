using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 1; // Punktzahl, die bei Einsammeln hinzugefügt wird
    public GameObject collectEffect; // Effekt, der beim Einsammeln abgespielt wird
    public AudioClip collectSound; // Sound, der beim Einsammeln abgespielt wird

    // Wird aufgerufen, wenn ein anderes Objekt den Trigger des Collectibles betritt
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Überprüft, ob der Spieler das Collectible berührt
        {
            Collect(); // Führt die Sammellogik aus
        }
    }

    // Logik für das Einsammeln
    void Collect()
    {
        if (collectEffect != null) // Überprüft, ob ein Sammel-Effekt zugewiesen ist
        {
            // Erstellt den Sammel-Effekt an der Position des Collectibles
            GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Berechnet die Gesamtdauer des Partikeleffekts
                float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(effect, totalDuration); // Zerstört den Effekt nach Ablauf der Dauer
            }
            else
            {
                Destroy(effect, 2f); // Alternative Zerstörungszeit, falls kein ParticleSystem gefunden wird
            }

            if (collectSound != null) // Überprüft, ob ein Sammel-Sound zugewiesen ist
            {
                // Spielt den Sammel-Sound an der Position des Collectibles ab
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
        }

        // Fügt dem Punktestand den Wert des Collectibles hinzu
        GameManager.Instance.AddScore(scoreValue);

        // Zerstört das Collectible nach dem Einsammeln
        Destroy(gameObject);
    }
}