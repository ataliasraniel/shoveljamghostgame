using UnityEngine;
using System.Collections; // Para IEnumarator

// Abstract class significa que não podemos criar um "BossAttackPattern" diretamente.
// Precisamos herdar dela para criar padrões específicos (como FanAttackPattern, etc.).
public abstract class BossAttackPattern : ScriptableObject
{
  [Header("Configurações Comuns do Ataque")]
  public string attackName = "Novo Ataque";
  [TextArea(3, 5)]
  public string description = "Descrição do ataque.";
  public float attackDuration = 2f; // Duração total deste padrão de ataque (para AI saber quanto tempo esperar)
  public float cooldownAfterAttack = 1f; // Cooldown antes que o chefe possa iniciar outro ataque
  public float damage = 1f; // Dano padrão de qualquer projétil/laser gerado por este ataque

  [Header("Efeitos Visuais/Sonoros")]
  public GameObject projectilePrefab; // Prefab do projétil/efeito visual específico deste ataque
  public AudioClip attackSound; // Som que toca quando o ataque é iniciado

  // Método abstrato que cada padrão de ataque específico DEVE implementar.
  // Ele será a corrotina que executa a lógica do ataque.
  public abstract IEnumerator Execute(BossAI bossAI);
  // Passamos BossAI para que o padrão possa acessar Rigidbody2D do boss, transform, etc.

  // Método auxiliar para spawnar projéteis (pode ser sobrescrito se precisar de lógica complexa)
  protected GameObject SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation, float projectileSpeed)
  {
    if (prefab == null)
    {
      Debug.LogWarning($"Projectile Prefab is null for attack: {attackName}. Cannot spawn projectile.", this);
      return null;
    }

    GameObject projectileGO = Instantiate(prefab, position, rotation);
    // Assumindo que seu projétil tem um Rigidbody2D e um script Bullet
    Rigidbody2D rb = projectileGO.GetComponent<Rigidbody2D>();
    Bullet bulletScript = projectileGO.GetComponent<Bullet>();

    if (bulletScript != null)
    {
      bulletScript.SetDamage(damage); // Define o dano com base no AttackPattern
                                      // Se precisar de DoT ou Mark, o Bullet deve ter campos para isso
                                      // e o AttackPattern também, para passar os valores aqui.
                                      // Ex: bulletScript.SetStatusEffectData(dotChance, dotDamage, dotDuration, ...);
    }

    if (rb != null)
    {
      rb.linearVelocity = rotation * Vector2.right * projectileSpeed;
    }
    else
    {
      Debug.LogWarning($"Projectile Prefab '{prefab.name}' does not have a Rigidbody2D. It will not move.", prefab);
    }
    return projectileGO;
  }
}