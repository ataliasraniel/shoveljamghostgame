using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMinionSummonAttack", menuName = "Boss Attacks/Minion Summon")]
public class MinionSummonAttack : BossAttackPattern
{
  [Header("Específico: Invocação de Minions")]
  public GameObject minionPrefab; // Prefab do minion a ser invocado
  public int numberOfMinions = 1;
  public float spawnDelay = 0.5f; // Atraso entre invocações de múltiplos minions
  public List<Vector2> relativeSpawnLocations; // Locais relativos para o spawn dos minions
  public float minionLifetime = 0f; // 0 para infinito, >0 para tempo de vida

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} invocando minions: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    for (int i = 0; i < numberOfMinions; i++)
    {
      Vector3 spawnPos;
      if (relativeSpawnLocations.Count > 0)
      {
        spawnPos = (Vector3)relativeSpawnLocations[i % relativeSpawnLocations.Count] + bossAI.transform.position;
      }
      else
      {
        spawnPos = bossAI.transform.position; // Spawn na posição do boss como fallback
      }

      GameObject minionGO = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
      if (minionLifetime > 0f)
      {
        Destroy(minionGO, minionLifetime);
      }
      yield return new WaitForSeconds(spawnDelay);
    }

    yield return new WaitForSeconds(attackDuration - (numberOfMinions * spawnDelay));
  }
}