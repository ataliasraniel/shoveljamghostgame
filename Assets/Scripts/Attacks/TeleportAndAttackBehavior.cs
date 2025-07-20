using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewTeleportAndAttackBehavior", menuName = "Boss Behaviors/Teleport and Attack")]
public class TeleportAndAttackBehavior : BossBehaviorPattern
{
  [Header("Específico: Teletransporte e Atacar")]
  public float teleportMinRange = 4f; // Mín. distância do jogador para teleportar
  public float teleportMaxRange = 7f; // Máx. distância do jogador para teleportar
  public float teleportCooldown = 2.0f; // Tempo entre teleportes
  public float preTeleportDelay = 0.5f; // Tempo de 'charge up' antes de teleportar
  public GameObject teleportEffectPrefab; // Efeito visual de teletransporte

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando comportamento: {behaviorName}");
    StopBossMovement(bossAI); // Boss para para teleportar

    float timer = 0f;
    float currentBehaviorDuration = Random.Range(behaviorMinDuration, behaviorMaxDuration);
    float teleportTimer = teleportCooldown; // Inicia com o cooldown para o primeiro teletransporte

    while (timer < currentBehaviorDuration)
    {
      teleportTimer += Time.deltaTime;

      if (teleportTimer >= teleportCooldown)
      {
        // Efeito de charge up antes de desaparecer
        if (teleportEffectPrefab != null)
        {
          Instantiate(teleportEffectPrefab, bossAI.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(preTeleportDelay); // Espera um pouco

        Vector3 playerPosition = bossAI.GetPlayerPosition();
        Vector3 randomOffset = Random.insideUnitCircle.normalized * Random.Range(teleportMinRange, teleportMaxRange);
        Vector3 newPosition = playerPosition + randomOffset;

        // Opcional: Clampear a nova posição dentro dos limites da arena
        // newPosition.x = Mathf.Clamp(newPosition.x, minArenaX, maxArenaX);
        // newPosition.y = Mathf.Clamp(newPosition.y, minArenaY, maxArenaY);

        bossAI.transform.position = newPosition; // Teleporta

        // Efeito de reaparecimento
        if (teleportEffectPrefab != null)
        {
          Instantiate(teleportEffectPrefab, bossAI.transform.position, Quaternion.identity);
        }

        // Após teleportar, ele pode atacar imediatamente
        BossAttackPattern selectedAttack = bossAI.GetRandomAttackPattern();
        if (selectedAttack != null)
        {
          yield return bossAI.StartCoroutine(selectedAttack.Execute(bossAI));
        }
        teleportTimer = 0f; // Reseta o timer do teletransporte
      }

      timer += Time.deltaTime;
      yield return null;
    }
    StopBossMovement(bossAI);
  }
}