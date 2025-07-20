using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Para usar List

[CreateAssetMenu(fileName = "NewFixedPatternAttack", menuName = "Boss Attacks/Fixed Pattern")]
public class FixedPatternAttack : BossAttackPattern
{
  [Header("Específico: Ataque de Padrão Fixo/Geométrico")]
  public float projectileSpeed = 7f;
  public List<Vector2> relativeSpawnPositions; // Posições relativas ao boss ou à arena
  public List<Vector2> projectileDirections; // Direções para cada projétil (mesmo índice que spawnPositions)
  public float intervalBetweenSpawns = 0.1f; // Atraso entre cada projétil no padrão
  public bool relativeToBoss = true; // Se as spawn positions são relativas ao boss

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando ataque: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    for (int i = 0; i < relativeSpawnPositions.Count; i++)
    {
      Vector3 spawnPos = relativeToBoss ? (Vector3)relativeSpawnPositions[i] + bossAI.transform.position : (Vector3)relativeSpawnPositions[i];
      Vector2 direction = projectileDirections.Count > i ? projectileDirections[i].normalized : Vector2.right; // Fallback direction

      Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

      SpawnProjectile(projectilePrefab, spawnPos, rotation, projectileSpeed);
      yield return new WaitForSeconds(intervalBetweenSpawns);
    }

    yield return new WaitForSeconds(attackDuration - (relativeSpawnPositions.Count * intervalBetweenSpawns));
  }
}