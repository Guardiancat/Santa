using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.5f; // Amplitude der Auf- und Abbewegung
    public float frequency = 1f; // Frequenz der Bewegung

    private Vector3 startPosition; // Startposition des Objekts

    // Wird beim Start aufgerufen
    void Start()
    {
        startPosition = transform.position; // Speichert die Startposition des Objekts
    }

    // Wird einmal pro Frame aufgerufen
    void Update()
    {
        // Aktualisiert die Position des Objekts basierend auf einer Sinuskurve
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }
}
