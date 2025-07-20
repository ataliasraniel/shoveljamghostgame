// Exemplo de LaserDamageHandler.cs (coloque no seu prefab de laser)
using UnityEngine;

public class LaserDamageHandler : MonoBehaviour
{
  private float laserDamage;

  public void SetLaserDamage(float damage)
  {
    laserDamage = damage;
  }

  // Se o laser for um trigger, use OnTriggerStay2D
  private void OnTriggerStay2D(Collider2D other)
  {
    if (other.CompareTag("Player")) // Certifique-se que o Player tem a tag "Player"
    {
      // Assumindo que seu Player tem um componente de saúde
      // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
      // if (playerHealth != null)
      // {
      //   playerHealth.TakeDamage(laserDamage * Time.deltaTime); // Dano contínuo por segundo
      // }
    }
  }

  // Se o laser for um collider sólido, use OnCollisionStay2D
  private void OnCollisionStay2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Player"))
    {
      // PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
      // if (playerHealth != null)
      // {
      //   playerHealth.TakeDamage(laserDamage * Time.deltaTime);
      // }
    }
  }
}