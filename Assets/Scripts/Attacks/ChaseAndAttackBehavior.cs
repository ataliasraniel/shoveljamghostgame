using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewChaseAndAttackBehavior", menuName = "Boss Behaviors/Chase and Attack")]
public class ChaseAndAttackBehavior : BossBehaviorPattern
{
  [Header("Específico: Perseguir e Atacar")]
  [Tooltip("Distância que o chefe para do jogador para começar a atacar.")]
  public float stopDistance = 3f;
  [Tooltip("Intervalo em segundos entre os ataques enquanto o chefe está neste comportamento.")]
  public float attackInterval = 1.5f;

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando comportamento: {behaviorName}");
    Rigidbody2D rb = bossAI.GetComponent<Rigidbody2D>();
    if (rb == null)
    {
      Debug.LogError("Rigidbody2D não encontrado no BossAI! O chefe não poderá se mover.", bossAI);
      yield break; // Sai da corrotina se não tiver Rigidbody2D
    }

    float timer = 0f;
    float currentBehaviorDuration = Random.Range(behaviorMinDuration, behaviorMaxDuration);
    float attackTimer = 0f;

    while (timer < currentBehaviorDuration)
    {
      Vector3 playerPosition = bossAI.GetPlayerPosition();
      Vector3 bossPosition = bossAI.transform.position; // Posição atual do chefe
      float distanceToPlayer = Vector2.Distance(bossPosition, playerPosition);

      // Lógica de Perseguição
      if (distanceToPlayer > stopDistance)
      {
        // Calcula a direção para o jogador
        Vector2 direction = (playerPosition - bossPosition).normalized;

        // Aplica a velocidade ao Rigidbody2D do chefe
        rb.linearVelocity = direction * bossAI.GetBossMoveSpeed();
        // Debug.Log($"Perseguindo. Dist: {distanceToPlayer:F2}, Vel: {rb.velocity.magnitude:F2}");
      }
      else // Lógica de Parar e Atacar
      {
        StopBossMovement(bossAI); // Garante que o Rigidbody pare
                                  // Debug.Log($"Parado para atacar. Dist: {distanceToPlayer:F2}");

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
          BossAttackPattern selectedAttack = bossAI.GetRandomAttackPattern();
          if (selectedAttack != null)
          {
            // Inicia a corrotina do ataque. O 'yield return' garante que esperamos o ataque terminar.
            yield return bossAI.StartCoroutine(selectedAttack.Execute(bossAI));
          }
          else
          {
            Debug.LogWarning("Nenhum BossAttackPattern disponível para o comportamento ChaseAndAttack na fase atual.", bossAI);
          }
          attackTimer = 0f; // Reseta o timer do ataque
        }
      }

      timer += Time.deltaTime;
      yield return null; // Espera o próximo frame
    }

    StopBossMovement(bossAI); // Garante que o boss pare no final do comportamento
    Debug.Log($"{bossAI.name} finalizou comportamento: {behaviorName}");
  }
}