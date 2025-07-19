using UnityEngine;
using DG.Tweening; // Já está no seu script
using System.Collections; // Necessário para Coroutines

public class EnemyLifedObject : LifedBaseGameObject
{
  [Header("Configurações do Inimigo")]
  public int scoreValue = 100;
  private Material material;
  [SerializeField] private GameObject upgradeChestPrefab; // Renomeado para refletir o baú de upgrade

  // --- Variáveis para Status de Dano ao Longo do Tempo (DoT) ---
  private Coroutine dotCoroutine;
  private float currentDotDamagePerTick; // Dano por "tick" do DoT
  private float dotTickInterval = 0.5f; // Intervalo entre cada tick de dano (ajustável)

  // --- Variáveis para Status de Marcação (Vulnerabilidade) ---
  private Coroutine markCoroutine;
  private float currentMarkBonusDamage; // Bônus de dano que o inimigo recebe quando marcado
  private bool isMarked = false; // Flag para indicar se o inimigo está marcado

  protected override void Start()
  {
    base.Start(); // Chama o Start do LifedBaseGameObject
    material = sprite.material; // Assumindo que 'sprite' é definido em LifedBaseGameObject
                                // material.SetColor("_Color", Color.red); // Linha comentada do seu original, mantida assim.
  }

  protected override void OnTakeDamage()
  {
    // Se o inimigo estiver marcado, aplica o bônus de dano antes de chamar o base.OnTakeDamage()
    if (isMarked)
    {
      // Nota: Se TakeDamage no LifedBaseGameObject aceita um parâmetro de dano,
      // você deve passar (damage + currentMarkBonusDamage) para ele.
      // Se ele só subtrai uma quantidade fixa, você precisaria ter o valor do dano
      // vindo do PlayerShooter/Bullet aqui ou recalcular.
      // Por simplicidade, vou assumir que o "bonus" é aplicado *quando* o bullet colide.
      // Para um sistema mais robusto, o Bullet deveria verificar a marcação.
      // Por enquanto, esta parte é mais um lembrete do que uma implementação completa.
      Debug.Log($"Inimigo marcado! Recebendo bônus de dano de {currentMarkBonusDamage}.");
    }

    base.OnTakeDamage(); // Chama a implementação base, que cuida da vida e possível morte
    material.DOColor(Color.black, 0.1f).SetLoops(2, LoopType.Yoyo);
  }

  protected override void OnDeathAction()
  {
    base.OnDeathAction(); // Chama a implementação base para instanciar efeitos, etc.
    Debug.Log($"Inimigo {gameObject.name} derrotado! Ganhos {scoreValue} pontos.");

    // --- Lógica para spawnar o Baú de Upgrade ---
    if (upgradeChestPrefab != null)
    {
      // Instancia o baú na posição do inimigo
      Instantiate(upgradeChestPrefab, transform.position, Quaternion.identity);
      Debug.Log("Baú de upgrade instanciado.");
    }
    else
    {
      Debug.LogWarning("upgradeChestPrefab não atribuído no inimigo! Nenhum baú será spawnado.", this);
    }

    // Para uma game jam, Destroy(gameObject) é uma boa forma rápida de remover o inimigo.
    Destroy(gameObject);
  }

  // --- MÉTODOS PÚBLICOS PARA APLICAR EFEITOS DE STATUS ---

  /// <summary>
  /// Aplica um efeito de Dano ao Longo do Tempo (DoT) ao inimigo.
  /// </summary>
  /// <param name="damagePerSecond">Dano total por segundo.</param>
  /// <param name="duration">Duração total do efeito.</param>
  public void ApplyDotEffect(float damagePerSecond, float duration)
  {
    // Se já existe uma coroutine de DoT, para ela para aplicar uma nova ou refrescar.
    if (dotCoroutine != null)
    {
      StopCoroutine(dotCoroutine);
    }

    // Calcula o dano por tick
    currentDotDamagePerTick = damagePerSecond * dotTickInterval;

    dotCoroutine = StartCoroutine(DOTCoroutine(duration));
    Debug.Log($"DoT aplicado: {damagePerSecond} de dano por segundo por {duration} segundos.");
  }

  private IEnumerator DOTCoroutine(float duration)
  {
    float timer = 0f;
    while (timer < duration)
    {
      // Aplica o dano do tick. Isso chamará OnTakeDamage().
      // Você precisaria de um método em LifedBaseGameObject como TakeDirectDamage(float damage)
      // se não quiser que ele acione o feedback visual de 'hit' a cada tick.
      // Por enquanto, chamaremos TakeDamage do base para simplicidade.
      TakeDamage(currentDotDamagePerTick);

      yield return new WaitForSeconds(dotTickInterval);
      timer += dotTickInterval;
    }
    dotCoroutine = null; // Reinicia a coroutine quando termina
    Debug.Log("DoT finalizado.");
  }

  /// <summary>
  /// Aplica um efeito de Marcação (vulnerabilidade) ao inimigo.
  /// </summary>
  /// <param name="duration">Duração do efeito de marcação.</param>
  /// <param name="bonusDamage">Bônus de dano que o inimigo recebe enquanto marcado.</param>
  public void ApplyMarkEffect(float duration, float bonusDamage)
  {
    // Se já existe uma coroutine de Marcação, para ela para aplicar uma nova ou refrescar.
    if (markCoroutine != null)
    {
      StopCoroutine(markCoroutine);
    }

    currentMarkBonusDamage = bonusDamage;
    isMarked = true;

    // Opcional: Adicionar feedback visual/sonoro para inimigo marcado (ex: sprite fica vermelho, aura)
    material.DOColor(Color.yellow, 0.1f).SetLoops(1, LoopType.Yoyo); // Exemplo de feedback visual

    markCoroutine = StartCoroutine(MarkCoroutine(duration));
    Debug.Log($"Inimigo marcado! Recebe +{bonusDamage} de dano por {duration} segundos.");
  }

  private IEnumerator MarkCoroutine(float duration)
  {
    yield return new WaitForSeconds(duration);
    isMarked = false;
    currentMarkBonusDamage = 0f; // Remove o bônus de dano

    // Opcional: Remover feedback visual/sonoro
    material.DOColor(Color.white, 0.1f); // Volta à cor normal (ou o que for padrão)

    markCoroutine = null;
    Debug.Log("Marcação finalizada.");
  }

  // --- Ajuste para a colisão da bala ---
  // Você vai querer que seu Bullet.cs chame TakeDamage e os métodos de status
  // Se o Bullet.cs está chamando 'TakeDamage(damage)' e 'ApplyDotEffect'/'ApplyMarkEffect'
  // diretamente neste script (EnemyLifedObject), então a lógica está correta.
  // O bonus de dano da marcação (currentMarkBonusDamage) precisa ser somado no Bullet.cs,
  // ou você precisa de um método 'TakeDamageWithBonus(float baseDamage)' aqui.
  // Por simplicidade, assumimos que o Bullet já vai chamar as coisas certas.
}