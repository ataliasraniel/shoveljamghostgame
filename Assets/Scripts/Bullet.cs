using UnityEngine;

public class Bullet : MonoBehaviour
{
  [Header("Configurações de Dano")]
  public float damage = 20f;

  [Header("Efeitos de Colisão")]
  [Tooltip("Define as camadas com as quais a bala pode colidir e ser destruída (física ou trigger).")]
  public LayerMask collisionLayers; // Permite selecionar as camadas no Inspector

  [Tooltip("Prefab do efeito visual a ser instanciado no ponto de colisão.")]
  public GameObject hitEffectPrefab; // Prefab do efeito de acerto

  // Flag para evitar que a bala seja destruída múltiplas vezes se colidir com trigger e collider ao mesmo tempo (raro, mas possível)
  private bool hasBeenDestroyed = false;

  // Método para definir o dano do projétil
  public void SetDamage(float amount)
  {
    damage = amount;
  }

  // Método para definir os dados dos efeitos de status (mantido)
  public void SetStatusEffectData(float _dotChance, float _dotDamage, float _dotDuration, float _markChance, float _markDuration, float _markDamageBonus)
  {
    // Se você implementar efeitos de status, descomente e use estes campos
    // dotChance = _dotChance;
    // dotDamage = _dotDamage;
    // dotDuration = _dotDuration;
    // markChance = _markChance;
    // markDuration = _markDuration;
    // markDamageBonus = _markDamageBonus;
  }

  // --- Lida com colisões FÍSICAS (com objetos NÃO-TRIGGER como Ground, paredes) ---
  private void OnCollisionEnter2D(Collision2D collision)
  {
    // Se a bala já foi destruída por outro evento (ex: um trigger), não faça nada
    if (hasBeenDestroyed) return;

    GameObject otherGameObject = collision.gameObject;

    // Verifica se a camada do objeto colidido está incluída nas collisionLayers
    if ((collisionLayers.value & (1 << otherGameObject.layer)) > 0)
    {
      LifedBaseGameObject target = otherGameObject.GetComponent<LifedBaseGameObject>();

      // Se atingiu um alvo com LifedBaseGameObject (que NÃO é trigger neste caso), aplica dano
      if (target != null)
      {
        target.TakeDamage(damage);
        // Opcional: Se quiser instanciar o efeito no ponto exato do impacto,
        // você pode usar collision.contacts[0].point.
      }
      // Destrói a bala e o efeito, seja um LifedBaseGameObject ou um objeto de cenário (Ground)
      DestroyBulletWithEffect();
    }
  }

  // --- Lida com colisões por TRIGGER (com objetos que SÃO TRIGGER como inimigos) ---
  private void OnTriggerEnter2D(Collider2D other)
  {
    // Se a bala já foi destruída por outro evento (ex: uma colisão física), não faça nada
    if (hasBeenDestroyed) return;

    // Verifica se a camada do objeto colidido está incluída nas collisionLayers
    if ((collisionLayers.value & (1 << other.gameObject.layer)) > 0)
    {
      LifedBaseGameObject target = other.GetComponent<LifedBaseGameObject>();

      // Se atingiu um alvo com LifedBaseGameObject (que É trigger neste caso), aplica dano
      if (target != null)
      {
        target.TakeDamage(damage);
        // Destrói a bala e o efeito
        DestroyBulletWithEffect();
      }
      // Se atingiu algo que está na collisionLayers e É um trigger, mas NÃO é um LifedBaseGameObject,
      // a bala também pode ser destruída aqui se desejar.
      // Ex: um trigger de área de efeito que a bala deve parar.
      // else 
      // {
      //     DestroyBulletWithEffect();
      // }
    }
  }

  // Método auxiliar para destruir a bala e instanciar o efeito
  void DestroyBulletWithEffect()
  {
    // Marca a bala como destruída para evitar múltiplas chamadas
    hasBeenDestroyed = true;

    // Instancia o prefab do efeito de acerto na posição da bala
    if (hitEffectPrefab != null)
    {
      Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }

    Destroy(gameObject); // Destrói o projétil
  }
}