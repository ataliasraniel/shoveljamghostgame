using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewEvadeBehavior", menuName = "Boss Behaviors/Evade")]
public class EvadeBehavior : BossBehaviorPattern
{
  [Header("Específico: Esquiva")]
  public float evadeSpeedMultiplier = 1.5f;
  public float evadeDistance = 5f;
  public float minEvadeInterval = 1f;

  // NOVO: Adicione uma referência para o padrão de movimento a ser usado neste comportamento
  // Ou, como fizemos, chame GetRandomMovementPattern() se quiser variar o movimento dentro da ESQUIVA.

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando comportamento: {behaviorName}");
    Rigidbody2D rb = bossAI.GetComponent<Rigidbody2D>();
    if (rb == null) { Debug.LogError("Rigidbody2D não encontrado no BossAI!", bossAI); yield break; }

    float timer = 0f;
    float currentBehaviorDuration = Random.Range(behaviorMinDuration, behaviorMaxDuration);
    float evadeTimer = 0f;
    Vector2 currentEvadeDirection = Vector2.zero;

    while (timer < currentBehaviorDuration)
    {
      Vector3 playerPosition = bossAI.GetPlayerPosition();
      float distanceToPlayer = Vector2.Distance(bossAI.transform.position, playerPosition);

      evadeTimer += Time.deltaTime;

      if (evadeTimer >= minEvadeInterval)
      {
        if (distanceToPlayer < evadeDistance)
        {
          currentEvadeDirection = ((Vector2)bossAI.transform.position - (Vector2)playerPosition).normalized;
        }
        else
        {
          currentEvadeDirection = Random.insideUnitCircle.normalized; // Movimento aleatório
        }
        evadeTimer = 0f;
      }

      rb.linearVelocity = currentEvadeDirection * bossAI.GetBossMoveSpeed() * evadeSpeedMultiplier;

      timer += Time.deltaTime;
      yield return null;
    }
    StopBossMovement(bossAI);
  }
}