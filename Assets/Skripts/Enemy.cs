using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f; // Gesundheit des Gegners

    // Methode zum Erhalten von Schaden
    public void TakeDamage(float amount)
    {
        health -= amount; // Gesundheit wird um den Schadensbetrag reduziert
        if (health <= 0)
        {
            Die(); // Wenn die Gesundheit kleiner oder gleich null ist, wird die Sterbemethode aufgerufen
        }
    }

    // Methode, die beim Tod des Gegners aufgerufen wird
    private void Die()
    {
        Destroy(gameObject); // Zerstört das Objekt des Gegners
    }
}