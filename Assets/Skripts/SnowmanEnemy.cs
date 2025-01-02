using UnityEngine;

public class SnowmanEnemy : MonoBehaviour
{
    public float moveSpeed = 1f; // Bewegungsgeschwindigkeit des Schneemanns
    public float attackRange = 1f; // Angriffsreichweite
    public float attackDamage = 20f; // Schaden, der beim Angriff verursacht wird
    public float attackCooldown = 1f; // Zeitintervall zwischen Angriffen

    private float lastAttackTime; // Zeitpunkt des letzten Angriffs
    private Transform player; // Referenz auf den Spieler

    // Wird beim Start aufgerufen
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Sucht den Spieler anhand seines Tags

        if (player == null)
        {
            Debug.LogError("Player nicht gefunden!"); // Gibt eine Fehlermeldung aus, wenn der Spieler nicht gefunden wird
        }

        Debug.Log("Player gefunden an Position: " + player.position); // Gibt die Position des Spielers aus
    }

    // Wird einmal pro Frame aufgerufen
    void Update()
    {
        if (player != null)
        {
            // Berechnet die Richtung zum Spieler
            Vector3 direction = (player.position - transform.position).normalized;

            // Bewegt den Schneemann in Richtung des Spielers
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Überprüft, ob der Schneemann in Angriffsreichweite ist
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                AttackPlayer(); // Führt einen Angriff auf den Spieler aus
            }
        }
    }

    // Führt einen Angriff auf den Spieler aus
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return; // Wenn der Angriff noch in der Abkühlphase ist, passiert nichts
        }

        // Holt das HeroController-Skript vom Spieler
        HeroController hero = player.GetComponent<HeroController>();

        if (hero != null)
        {
            Vector2 attackerPosition = transform.position; // Position des Schneemanns für Rückstoßberechnung
            hero.TakeDamage(attackDamage, attackerPosition); // Übermittelt den Schaden und die Position an den Spieler

            Debug.Log("Schneemann greift den Spieler an und verursacht " + attackDamage + " Schaden!");

            lastAttackTime = Time.time; // Aktualisiert den Zeitpunkt des letzten Angriffs
        }
    }

    // Wird aufgerufen, wenn der Schneemann mit einem anderen Objekt kollidiert
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer(); // Führt einen Angriff aus, wenn der Schneemann den Spieler berührt
        }
    }
}