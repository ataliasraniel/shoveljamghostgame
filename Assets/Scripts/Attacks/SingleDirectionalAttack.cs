using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewSingleDirectionalAttack", menuName = "Boss Attacks/Single Directional")]
public class SingleDirectionalAttack : BossAttackPattern
{
  [Header("Específico: Ataque Direcional Único")]
  public float projectileSpeed = 10f;
  public float delayBetweenShots = 0.5f; // Para rajadas de projéteis únicos
  public int numberOfShots = 1; // Quantidade de projéteis em uma rajada
  public float homingStrength = 0f; // 0 para sem homing, >0 para homing

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando ataque: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    for (int i = 0; i < numberOfShots; i++)
    {
      Vector3 playerPosition = bossAI.GetPlayerPosition();
      Vector2 directionToPlayer = (playerPosition - bossAI.transform.position).normalized;
      Quaternion rotationToPlayer = Quaternion.Euler(0, 0, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);

      // Se houver homing, o projétil precisará de um script próprio para perseguir
      // Por enquanto, ele apenas aponta na direção inicial.
      SpawnProjectile(projectilePrefab, bossAI.transform.position, rotationToPlayer, projectileSpeed);
      yield return new WaitForSeconds(delayBetweenShots);
    }

    yield return new WaitForSeconds(attackDuration - (numberOfShots * delayBetweenShots)); // Espera o restante da duração do ataque
  }
}