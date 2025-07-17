using UnityEngine;
using System; // Necessário para Action

public class LifedBaseGameObject : MonoBehaviour
{
  [Header("Configurações de Vida")]
  [Tooltip("Vida máxima do objeto.")]
  [SerializeField] private float maxHealth = 100f;
  [Tooltip("Vida atual do objeto.")]
  [SerializeField] private float currentHealth;

  [Header("Configurações de Dano por Colisão")]
  [Tooltip("Habilita ou desabilita o recebimento de dano por colisão.")]
  [SerializeField] private bool enableCollisionDamage = true;
  [Tooltip("Camadas (Layers) dos objetos que podem causar dano a este.")]
  [SerializeField] private LayerMask damageLayers;
  [Tooltip("Dano aplicado por colisão (se a camada for detectada).")]
  [SerializeField] private float collisionDamageAmount = 10f;
  [Tooltip("Tempo em segundos de invulnerabilidade após levar dano.")]
  [SerializeField] private float invulnerabilityDuration = 0.5f;
  public SpriteRenderer sprite;

  [Header("Efeitos de Morte")]
  [Tooltip("Prefabs a serem instanciados na morte (efeitos, itens, etc.).")]
  [SerializeField] private GameObject[] deathEffectPrefabs;
  [Tooltip("Ponto de spawn para os prefabs de morte (se nulo, usa a posição do objeto).")]
  [SerializeField] private Transform deathSpawnPoint;
  [Tooltip("Destruir o GameObject após a morte?")]
  [SerializeField] private bool destroyOnDeath = true;
  [Tooltip("Tempo em segundos para destruir o GameObject após a morte (se destroyOnDeath for true).")]
  [SerializeField] private float destroyDelay = 0f;

  // Estado interno
  private bool isDead = false;
  private bool isInvulnerable = false;
  private float invulnerabilityTimer = 0f;

  // Evento que pode ser assinado por outros scripts quando o objeto morre
  public static event Action<GameObject> OnObjectDied;

  // Propriedades públicas para acesso externo
  public float MaxHealth => maxHealth;
  public float CurrentHealth => currentHealth;
  public bool IsDead => isDead;

  protected virtual void Awake()
  {
    currentHealth = maxHealth;
    // Se deathSpawnPoint não for definido, use a própria transformação
    if (deathSpawnPoint == null)
    {
      deathSpawnPoint = transform;
    }
    sprite = GetComponentInChildren<SpriteRenderer>();
  }

  protected virtual void Start()
  {

  }

  protected virtual void Update()
  {
    // Gerencia o temporizador de invulnerabilidade
    if (isInvulnerable)
    {
      invulnerabilityTimer -= Time.deltaTime;
      if (invulnerabilityTimer <= 0f)
      {
        isInvulnerable = false;
      }
    }
  }

  /// <summary>
  /// Aplica dano ao objeto.
  /// </summary>
  /// <param name="amount">A quantidade de dano a ser aplicada.</param>
  public virtual void TakeDamage(float amount)
  {
    if (isDead || isInvulnerable)
    {
      return; // Não leva dano se já estiver morto ou invulnerável
    }

    currentHealth -= amount;
    Debug.Log($"{gameObject.name} levou {amount} de dano. Vida restante: {currentHealth}");

    // Ativa a invulnerabilidade
    isInvulnerable = true;
    invulnerabilityTimer = invulnerabilityDuration;
    OnTakeDamage();

    if (currentHealth <= 0)
    {
      currentHealth = 0; // Garante que a vida não fique negativa
      Die();
    }
  }



  /// <summary>
  /// Cura o objeto.
  /// </summary>
  /// <param name="amount">A quantidade de vida a ser curada.</param>
  public virtual void Heal(float amount)
  {
    if (isDead)
    {
      return; // Não cura se já estiver morto
    }

    currentHealth += amount;
    if (currentHealth > maxHealth)
    {
      currentHealth = maxHealth; // Garante que a vida não exceda o máximo
    }
    Debug.Log($"{gameObject.name} curou {amount} de vida. Vida atual: {currentHealth}");
  }

  /// <summary>
  /// Lida com o processo de morte do objeto.
  /// </summary>
  protected virtual void Die()
  {
    if (isDead)
    {
      return; // Evita múltiplas chamadas de morte
    }

    isDead = true;
    Debug.Log($"{gameObject.name} morreu!");

    // Dispara a função ou evento de morte
    OnDeathAction();

    // Invoca o evento estático para outros scripts
    OnObjectDied?.Invoke(gameObject);

    // Instancia efeitos de morte, se houver
    InstantiateDeathEffects();

    // Destrói o GameObject, se configurado
    if (destroyOnDeath)
    {
      Destroy(gameObject, destroyDelay);
    }
  }

  /// <summary>
  /// Função virtual para ser sobrescrita por classes filhas
  /// para adicionar comportamentos específicos na morte.
  /// </summary>
  /// 
  protected virtual void OnTakeDamage()
  {

  }
  protected virtual void OnDeathAction()
  {
    // Exemplo: Desativar renderizadores ou colliders, tocar animação
    // GetComponent<SpriteRenderer>()?.enabled = false;
    // GetComponent<Collider2D>()?.enabled = false;
    // GetComponent<Rigidbody2D>()?.simulated = false;
  }

  /// <summary>
  /// Instancia os prefabs de efeito de morte.
  /// </summary>
  private void InstantiateDeathEffects()
  {
    if (deathEffectPrefabs != null && deathEffectPrefabs.Length > 0)
    {
      foreach (GameObject effectPrefab in deathEffectPrefabs)
      {
        if (effectPrefab != null)
        {
          Instantiate(effectPrefab, deathSpawnPoint.position, deathSpawnPoint.rotation);
        }
      }
    }
  }

  // --- Tratamento de Colisões para Dano ---
  // Usaremos OnCollisionEnter2D para 2D e OnCollisionEnter para 3D
  // Certifique-se de que o GameObject tenha um Collider e um Rigidbody (para 2D, Rigidbody2D)
  // para detectar colisões corretamente.

  protected virtual void OnCollisionEnter2D(Collision2D collision)
  {
    if (!enableCollisionDamage || isDead || isInvulnerable) return;

    // Verifica se a camada do objeto colidido está nas camadas que causam dano
    if (((1 << collision.gameObject.layer) & damageLayers) != 0)
    {
      TakeDamage(collisionDamageAmount);
    }
  }

  protected virtual void OnTriggerEnter2D(Collider2D other)
  {
    if (!enableCollisionDamage || isDead || isInvulnerable) return;

    // Verifica se a camada do objeto colidido está nas camadas que causam dano
    if (((1 << other.gameObject.layer) & damageLayers) != 0)
    {
      TakeDamage(collisionDamageAmount);
    }
  }

  // Para projetos 3D, você usaria OnCollisionEnter e OnTriggerEnter:
  /*
  protected virtual void OnCollisionEnter(Collision collision)
  {
      if (!enableCollisionDamage || isDead || isInvulnerable) return;
      if (((1 << collision.gameObject.layer) & damageLayers) != 0)
      {
          TakeDamage(collisionDamageAmount);
      }
  }

  protected virtual void OnTriggerEnter(Collider other)
  {
      if (!enableCollisionDamage || isDead || isInvulnerable) return;
      if (((1 << other.gameObject.layer) & damageLayers) != 0)
      {
          TakeDamage(collisionDamageAmount);
      }
  }
  */
}