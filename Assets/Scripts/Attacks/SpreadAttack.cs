using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewSpreadAttack", menuName = "Boss Attacks/Spread")]
public class SpreadAttack : BossAttackPattern
{
  [Header("Específico: Ataque em Leque")]
  public float projectileSpeed = 8f;
  public int numberOfProjectiles = 5;
  public float spreadAngle = 60f; // Ângulo total do leque em graus
  public float delayBetweenBursts = 0.5f;
  public int numberOfBursts = 1; // Quantas rajadas de leque serão lançadas

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando ataque: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    for (int b = 0; b < numberOfBursts; b++)
    {
      Vector3 playerPosition = bossAI.GetPlayerPosition();
      Vector2 directionToPlayer = (playerPosition - bossAI.transform.position).normalized;
      float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

      float startAngle = baseAngle - (spreadAngle / 2f);
      float angleStep = spreadAngle / (numberOfProjectiles - 1);

      if (numberOfProjectiles == 1) // Evita divisão por zero se for apenas 1 projétil
      {
        angleStep = 0;
      }

      for (int i = 0; i < numberOfProjectiles; i++)
      {
        float currentAngle = startAngle + (angleStep * i);
        Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
        SpawnProjectile(projectilePrefab, bossAI.transform.position, rotation, projectileSpeed);
      }
      yield return new WaitForSeconds(delayBetweenBursts);
    }

    yield return new WaitForSeconds(attackDuration - (numberOfBursts * delayBetweenBursts));
  }
}