// Exemplo: Script de uma bala que causa dano
using UnityEngine;

public class Bullet : MonoBehaviour
{
  public float damage = 20f;
  // Variáveis para efeitos de status (passadas pelo PlayerShooter)
  private float dotChance;
  private float dotDamage;
  private float dotDuration;

  private float markChance;
  private float markDuration;
  private float markDamageBonus;

  // Método para definir o dano do projétil
  public void SetDamage(float amount)
  {
    damage = amount;
  }

  // Novo método para definir os dados dos efeitos de status
  public void SetStatusEffectData(float _dotChance, float _dotDamage, float _dotDuration, float _markChance, float _markDuration, float _markDamageBonus)
  {
    dotChance = _dotChance;
    dotDamage = _dotDamage;
    dotDuration = _dotDuration;
    markChance = _markChance;
    markDuration = _markDuration;
    markDamageBonus = _markDamageBonus;
  }
  // Quando o projétil colide
  // private void OnTriggerEnter2D(Collider2D other)
  // {
  //     if (other.CompareTag("Enemy"))
  //     {
  //         EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
  //         if (enemyHealth != null)
  //         {
  //             enemyHealth.TakeDamage(damage); // Aplica o dano direto

  //             // --- Lógica para aplicar efeitos de status ---
  //             // Aplica DoT se tiver chance e o dado for favorável
  //             if (dotChance > 0f && Random.value < dotChance)
  //             {
  //                 enemyHealth.ApplyDotEffect(dotDamage, dotDuration); // Você precisará criar este método em EnemyHealth
  //             }

  //             // Aplica Marcação se tiver chance e o dado for favorável
  //             if (markChance > 0f && Random.value < markChance)
  //             {
  //                 enemyHealth.ApplyMarkEffect(markDuration, markDamageBonus); // Você precisará criar este método em EnemyHealth
  //             }
  //         }
  //         Destroy(gameObject); // Destroi o projétil após colidir
  //     }
  //     else if (other.CompareTag("Wall")) // Exemplo: se colidir com uma parede
  //     {
  //         Destroy(gameObject);
  //     }
  // }
  // // Lembre-se de adaptar para OnCollisionEnter2D se estiver usando colisões não-trigger
  // private void OnCollisionEnter2D(Collision2D collision)
  // {
  //     if (collision.gameObject.CompareTag("Enemy"))
  //     {
  //         EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
  //         if (enemyHealth != null)
  //         {
  //             enemyHealth.TakeDamage(damage);

  //             // --- Lógica para aplicar efeitos de status ---
  //             if (dotChance > 0f && Random.value < dotChance)
  //             {
  //                 enemyHealth.ApplyDotEffect(dotDamage, dotDuration);
  //             }

  //             if (markChance > 0f && Random.value < markChance)
  //             {
  //                 enemyHealth.ApplyMarkEffect(markDuration, markDamageBonus);
  //             }
  //         }
  //         Destroy(gameObject);
  //     }
  //     else if (collision.gameObject.CompareTag("Wall"))
  //     {
  //         Destroy(gameObject);
  //     }
  // }
  private void OnTriggerEnter2D(Collider2D other) // Ou OnCollisionEnter2D
  {
    LifedBaseGameObject target = other.GetComponent<LifedBaseGameObject>();
    if (target != null)
    {
      target.TakeDamage(damage);
      Destroy(gameObject); // Destrói a bala após atingir
    }
  }


}