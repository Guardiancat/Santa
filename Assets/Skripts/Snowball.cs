using UnityEngine;

public class Snowball : MonoBehaviour
{
    public float damage = 10f; // Урон от снежка

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, столкнулся ли снежок с противником
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // Наносим урон противнику
        }

        // Уничтожаем снежок при столкновении
       // Destroy(gameObject);
    }
}