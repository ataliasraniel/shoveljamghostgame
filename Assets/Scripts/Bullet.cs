// Exemplo: Script de uma bala que causa dano
using UnityEngine;

public class Bullet : MonoBehaviour
{
  public float damage = 20f;

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