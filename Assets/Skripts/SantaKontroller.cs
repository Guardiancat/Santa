using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour
{
    public GameObject snowballPrefab; // Prefab für den Schneeball
    public Transform firePoint; // Startpunkt für den Schneeball
    public float snowballSpeed = 10f; // Geschwindigkeit des Schneeballs
    public float fireRate = 0.5f; // Feuerrate (Zeit zwischen den Schüssen in Sekunden)
    private float nextFireTime = 0f; // Zeitpunkt, wann wieder geschossen werden kann
    public float moveSpeed = 5f; // Bewegungsgeschwindigkeit des Helden
    public float jumpForce = 10f; // Sprungkraft
    private bool isGrounded = false; // Prüft, ob der Held auf dem Boden ist
    private bool jumpRequest = false; // Anfrage für einen Sprung
    private SpriteRenderer spriteRenderer; // SpriteRenderer-Komponente für das Sprite
    private Rigidbody2D rb; // Rigidbody2D-Komponente für die Physik
    private Animator animator; // Animator-Komponente für Animationen
    private Coroutine setParentCoroutine; // Coroutine zur Verzögerung des Elternwechsels
    public AudioClip walkSound; // Geräusch beim Gehen
    private AudioSource audioSource; // AudioSource-Komponente für Soundeffekte
    private bool isWalking = false; // Status, ob der Held gerade geht

    public float maxHealth = 100f; // Maximale Gesundheit des Helden
    private float currentHealth; // Aktuelle Gesundheit des Helden
    public float knockbackForce = 5f; // Rückstoßkraft
    public float knockbackDuration = 0.2f; // Dauer des Rückstoßes
    private bool isKnockedBack = false; // Status, ob der Held Rückstoß erfährt

    void Start()
    {
        // Initialisiert die benötigten Komponenten und setzt die Gesundheit
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        // Reduziert die Gesundheit und löst Rückstoß aus, wenn der Held Schaden nimmt
        currentHealth -= damage;
        Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;
        Knockback(knockbackDirection);

        if (currentHealth <= 0)
        {
            Die(); // Der Held stirbt, wenn die Gesundheit null oder weniger beträgt
        }
    }

    private void Knockback(Vector2 direction)
    {
        // Löst eine Coroutine für den Rückstoß aus
        StartCoroutine(KnockbackCoroutine(direction));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        // Behandelt den Rückstoß für eine bestimmte Dauer
        isKnockedBack = true;
        rb.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    void Die()
    {
        // Behandelt den Tod des Helden
        Debug.Log("Der Held ist gestorben");
        GameManager.Instance.GameOver();
    }

    void Update()
    {
        // Verarbeitet Eingaben und führt Bewegungen, Sprünge und Angriffe aus
        if (transform.position.y < -10f)
        {
            TakeDamage(currentHealth, Vector2.zero); // Sofortiger Tod bei Sturz
            return;
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (!isKnockedBack)
        {
            // Steuert die Ausrichtung des Sprites basierend auf der Bewegungsrichtung
            if (moveHorizontal > 0)
            {
                spriteRenderer.flipX = false;
                firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
            }
            else if (moveHorizontal < 0)
            {
                spriteRenderer.flipX = true;
                firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jumpRequest = true;
            }

            animator.SetBool("IsJumping", !isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(moveHorizontal));
            animator.SetBool("IsRunning", Mathf.Abs(moveHorizontal) > 0.1f);

            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                ThrowSnowball();
                nextFireTime = Time.time + fireRate;
            }

            HandleWalkingSound(moveHorizontal);
        }

        Move(moveHorizontal);

        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }

    void HandleWalkingSound(float moveHorizontal)
    {
        // Spielt ein Gehgeräusch ab, wenn sich der Held bewegt
        if (Mathf.Abs(moveHorizontal) > 0.1f && isGrounded)
        {
            if (!isWalking)
            {
                audioSource.PlayOneShot(walkSound);
                isWalking = true;
            }
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
            }
        }
    }

    void FixedUpdate()
    {
        // Behandelt Bewegungen und Sprünge im FixedUpdate (für die Physik)
        if (!isKnockedBack)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            Move(moveHorizontal);

            if (jumpRequest)
            {
                Jump();
                jumpRequest = false;
            }
        }
    }

    void Move(float direction)
    {
        // Bewegt den Helden basierend auf der Eingabe
        Vector2 movement = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
    }

    void Jump()
    {
        // Führt einen Sprung aus
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Prüft, ob der Held mit dem Boden oder einer beweglichen Plattform in Kontakt ist
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("MovingPlatform") && transform.parent != collision.transform)
        {
            if (setParentCoroutine != null)
            {
                StopCoroutine(setParentCoroutine);
            }
            setParentCoroutine = StartCoroutine(SetParentDelayed(collision.transform));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Behandelt das Verlassen der Kollision mit dem Boden oder Plattformen
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            if (setParentCoroutine != null)
            {
                StopCoroutine(setParentCoroutine);
            }
            setParentCoroutine = StartCoroutine(SetParentDelayed(null));
        }
    }

    IEnumerator SetParentDelayed(Transform newParent)
    {
        // Verzögert den Wechsel des Elternobjekts
        yield return new WaitForEndOfFrame();

        if (gameObject.activeInHierarchy && (newParent == null || newParent.gameObject.activeInHierarchy))
        {
            transform.SetParent(newParent);
        }
    }

    void ThrowSnowball()
    {
        // Erstellt und wirft einen Schneeball
        Debug.Log("Schneeball wird geworfen");

        if (snowballPrefab == null)
        {
            Debug.LogError("Schneeball-Prefab ist nicht zugewiesen!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("Startpunkt ist nicht zugewiesen!");
            return;
        }

        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rbSnowball = snowball.GetComponent<Rigidbody2D>();

        if (rbSnowball == null)
        {
            Debug.LogError("Rigidbody2D-Komponente wurde auf dem Schneeball nicht gefunden!");
            return;
        }

        Vector2 throwDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        rbSnowball.linearVelocity = throwDirection * snowballSpeed;

        Destroy(snowball, 1f); // Zerstört den Schneeball nach 1 Sekunde
    }
}
