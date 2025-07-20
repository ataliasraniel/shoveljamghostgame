using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Para usar List


// Em algum lugar global, ou no namespace do BossAI
public enum BossBehaviorType
{
  ChaseAndAttack,
  TeleportAndAttack,
  Evade
}
public class BossAI : EnemyLifedObject
{
  [System.Serializable]
  public class BossPhase
  {
    public string phaseName = "Fase 1";
    public float healthThreshold = 1.0f; // 1.0 = 100%, 0.5 = 50%

    [Header("Comportamentos Disponíveis Nesta Fase")]
    public List<BossBehaviorType> availableBehaviorTypes; // Tipos de comportamento permitidos na fase

    [Header("Ataques e Movimentos Específicos da Fase")]
    public List<BossAttackPattern> attackPatterns; // Ataques disponíveis para os comportamentos
    public List<BossMovementPattern> movementPatterns; // Movimentos disponíveis para os comportamentos

    // Adicionar referências para padrões específicos de cada comportamento
    // para facilitar a organização no Inspector
    public ChaseAndAttackBehavior chaseAndAttackBehavior;
    public TeleportAndAttackBehavior teleportAndAttackBehavior;
    public EvadeBehavior evadeBehavior;
  }

  [Header("Configurações do Chefe")]
  public List<BossPhase> phases;
  public float bossMoveSpeed = 3f; // Velocidade base de movimento do chefe
  public float behaviorSwitchDelay = 1.0f; // Tempo entre a troca de comportamentos

  private int currentPhaseIndex = 0;
  private bool isPerformingBehavior = false; // Flag para controlar se um comportamento está ativo
  private Coroutine currentBehaviorCoroutine;
  private Transform playerTransform;
  private float timeSincePhaseStart = 0f;

  private Coroutine bossBehaviorLoopCoroutine; // NOVA VARIÁVEL: Para guardar a referência da corrotina do loop principal

  protected override void Start()
  {
    base.Start();

    GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
    if (playerGO != null)
    {
      playerTransform = playerGO.transform;
    }
    else
    {
      Debug.LogError("Player não encontrado na cena! Certifique-se que o jogador tem a tag 'Player'.");
    }

    // INICIE O LOOP PRINCIPAL AQUI E GUARDE A REFERÊNCIA
    bossBehaviorLoopCoroutine = StartCoroutine(BossBehaviorLoop());
  }

  protected override void Update()
  {
    base.Update();
    timeSincePhaseStart += Time.deltaTime;
    CheckPhaseTransition();
  }

  public Vector3 GetPlayerPosition()
  {
    if (playerTransform != null)
    {
      return playerTransform.position;
    }
    return Vector3.zero;
  }

  public float GetBossMoveSpeed()
  {
    return bossMoveSpeed;
  }

  public float GetTimeSinceAttackStart()
  {
    return timeSincePhaseStart;
  }

  // Método para ser usado pelos padrões de comportamento para selecionar um ataque da fase atual
  public BossAttackPattern GetRandomAttackPattern()
  {
    if (phases[currentPhaseIndex].attackPatterns != null && phases[currentPhaseIndex].attackPatterns.Count > 0)
    {
      return phases[currentPhaseIndex].attackPatterns[Random.Range(0, phases[currentPhaseIndex].attackPatterns.Count)];
    }
    return null;
  }

  // Método para ser usado pelos padrões de comportamento para selecionar um movimento da fase atual
  // MÉTODO NOVO/CORRIGIDO: Para ser usado pelos padrões de comportamento para selecionar um movimento da fase atual
  public BossMovementPattern GetRandomMovementPattern()
  {
    if (phases[currentPhaseIndex].movementPatterns != null && phases[currentPhaseIndex].movementPatterns.Count > 0)
    {
      return phases[currentPhaseIndex].movementPatterns[Random.Range(0, phases[currentPhaseIndex].movementPatterns.Count)];
    }
    return null;
  }

  private void CheckPhaseTransition()
  {
    if (currentPhaseIndex < phases.Count - 1)
    {
      BossPhase nextPhase = phases[currentPhaseIndex];
      if (CurrentHealth <= nextPhase.healthThreshold)
      {
        TransitionToNextPhase();
      }
    }
    // if (currentPhaseIndex < phases.Count - 1) // Garante que há uma próxima fase
    // {
    //   BossPhase nextPhase = phases[currentPhaseIndex + 1]; // <--- CORRIGIDO: Pega a PRÓXIMA fase
    //                                                        // A condição de transição geralmente é que a vida ATUAL seja MENOR OU IGUAL ao LIMIAR da próxima fase.
    //                                                        // O threshold é geralmente um PERCENTUAL (0.5 para 50%).
    //                                                        // Então, você compara a vida ATUAL do chefe (CurrentHealth) com o (MaxHealth * nextPhase.healthThreshold).
    //   if (CurrentHealth <= MaxHealth * nextPhase.healthThreshold) // Sugestão para uso de threshold percentual
    //   {
    //     TransitionToNextPhase();
    //   }
    // }
  }

  private void TransitionToNextPhase()
  {
    Debug.Log($"[{Time.time}] --- INICIANDO TRANSIÇÃO DE FASE ---");
    // ... (restante dos seus Debug.Logs existentes) ...

    currentPhaseIndex++;
    if (currentPhaseIndex >= phases.Count)
    {
      Debug.LogWarning($"[{Time.time}] Boss {gameObject.name} tentou transicionar para uma fase além do limite ({currentPhaseIndex + 1}/{phases.Count}). Boss derrotado ou último comportamento.", this);
      Die();
      return;
    }

    Debug.Log($"[{Time.time}] Boss {gameObject.name} transicionando OFICIALMENTE para a Fase {currentPhaseIndex + 1}: {phases[currentPhaseIndex].phaseName}");

    // PARA O COMPORTAMENTO ATUAL DA FASE ANTERIOR
    if (currentBehaviorCoroutine != null)
    {
      StopCoroutine(currentBehaviorCoroutine);
      currentBehaviorCoroutine = null;
      Debug.Log($"[{Time.time}] Corrotina de comportamento anterior parada e zerada.");
    }
    isPerformingBehavior = false;
    Debug.Log($"[{Time.time}] isPerformingBehavior definido como false.");

    // Garante que o Rigidbody2D do boss pare completamente
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.linearVelocity = Vector2.zero;
      Debug.Log($"[{Time.time}] Rigidbody2D.velocity zerado.");
    }
    else
    {
      Debug.LogError($"[{Time.time}] Rigidbody2D não encontrado no BossAI. O boss não poderá se mover!", this);
    }

    timeSincePhaseStart = 0f;
    Debug.Log($"[{Time.time}] timeSincePhaseStart zerado.");

    // NOVO: PARA E REINICIA A CORROTINA PRINCIPAL DO BOSSBEHAVIORLOOP
    if (bossBehaviorLoopCoroutine != null)
    {
      StopCoroutine(bossBehaviorLoopCoroutine);
      Debug.Log($"[{Time.time}] BossBehaviorLoop Coroutine parada para reinício.");
    }
    bossBehaviorLoopCoroutine = StartCoroutine(BossBehaviorLoop());
    Debug.Log($"[{Time.time}] BossBehaviorLoop Coroutine reiniciada.");

    Debug.Log($"[{Time.time}] --- TRANSIÇÃO DE FASE CONCLUÍDA ---");
  }




  // Em BossAI.cs - dentro de BossBehaviorLoop()

  private IEnumerator BossBehaviorLoop()
  {
    while (true) // Loop infinito enquanto o chefe estiver vivo
    {
      // Debug.Log($"[{Time.time}] BossBehaviorLoop: isPerformingBehavior = {isPerformingBehavior}"); // Para depuração contínua

      if (!isPerformingBehavior)
      {
        Debug.Log($"[{Time.time}] BossBehaviorLoop: Não está performando comportamento. Aguardando {behaviorSwitchDelay} segundos.");
        yield return new WaitForSeconds(behaviorSwitchDelay); // Cooldown entre a troca de comportamentos

        BossBehaviorType selectedBehaviorType = SelectBehaviorType();
        Debug.Log($"[{Time.time}] BossBehaviorLoop: Comportamento selecionado: {selectedBehaviorType}");

        BossBehaviorPattern behaviorToExecute = null;
        switch (selectedBehaviorType)
        {
          case BossBehaviorType.ChaseAndAttack:
            behaviorToExecute = phases[currentPhaseIndex].chaseAndAttackBehavior;
            break;
          case BossBehaviorType.TeleportAndAttack:
            behaviorToExecute = phases[currentPhaseIndex].teleportAndAttackBehavior;
            break;
          case BossBehaviorType.Evade:
            behaviorToExecute = phases[currentPhaseIndex].evadeBehavior;
            break;
        }

        if (behaviorToExecute != null)
        {
          Debug.Log($"[{Time.time}] BossBehaviorLoop: Executando comportamento: {behaviorToExecute.behaviorName}");
          isPerformingBehavior = true;
          currentBehaviorCoroutine = StartCoroutine(behaviorToExecute.Execute(this));
          yield return currentBehaviorCoroutine; // Espera a corrotina terminar
          isPerformingBehavior = false;
          Debug.Log($"[{Time.time}] BossBehaviorLoop: Comportamento {behaviorToExecute.behaviorName} concluído.");
        }
        else
        {
          Debug.LogError($"[{Time.time}] BossBehaviorLoop: ERRO! Comportamento '{selectedBehaviorType}' selecionado para a Fase {currentPhaseIndex + 1}, mas o ScriptableObject correspondente NÃO FOI ATRIBUÍDO no Inspector!", this);

          // Fallback para ChaseAndAttack
          if (phases[currentPhaseIndex].chaseAndAttackBehavior != null)
          {
            Debug.LogWarning($"[{Time.time}] BossBehaviorLoop: Tentando fallback para ChaseAndAttack Behavior...", this);
            isPerformingBehavior = true;
            currentBehaviorCoroutine = StartCoroutine(phases[currentPhaseIndex].chaseAndAttackBehavior.Execute(this));
            yield return currentBehaviorCoroutine;
            isPerformingBehavior = false;
            Debug.Log($"[{Time.time}] BossBehaviorLoop: Fallback ChaseAndAttack concluído.");
          }
          else
          {
            Debug.LogError($"[{Time.time}] BossBehaviorLoop: Nenhum comportamento configurado OU fallback disponível para o boss na fase {currentPhaseIndex + 1}! Chefe ficará parado indefinidamente.", this);
            yield return new WaitForSeconds(1f); // Evita loop infinito, mas o boss estará "travado"
          }
        }
      }
      yield return null; // Espera o próximo frame
    }
  }

  private BossBehaviorType SelectBehaviorType()
  {
    if (phases[currentPhaseIndex].availableBehaviorTypes.Count > 0)
    {
      // Seleciona um tipo de comportamento aleatório da lista disponível
      int randomIndex = Random.Range(0, phases[currentPhaseIndex].availableBehaviorTypes.Count);
      return phases[currentPhaseIndex].availableBehaviorTypes[randomIndex];
    }
    // Fallback: se não houver comportamentos configurados, retorna um padrão padrão
    return BossBehaviorType.ChaseAndAttack; // Padrão de perseguição como default
  }

  protected override void OnDeathAction()
  {
    base.OnDeathAction();
    if (currentBehaviorCoroutine != null) StopCoroutine(currentBehaviorCoroutine);
    // IMPORTANTE: Parar o loop principal também na morte
    if (bossBehaviorLoopCoroutine != null) StopCoroutine(bossBehaviorLoopCoroutine);
    Debug.Log($"[{Time.time}] Boss {gameObject.name} morreu. Todas as corrotinas paradas.");
  }
}