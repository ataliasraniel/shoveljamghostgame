using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewRadialAttack", menuName = "Boss Attacks/Radial")]
public class RadialAttack : BossAttackPattern
{
  [Header("Específico: Ataque Radial/360 Graus")]
  public float projectileSpeed = 6f;
  public int numberOfProjectilesInCircle = 12; // Quantidade para formar o anel
  public float delayBetweenWaves = 0.5f;
  public int numberOfWaves = 1; // Quantos anéis serão lançados
  public float spinSpeed = 0f; // Velocidade de rotação do padrão (opcional)

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando ataque: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    for (int w = 0; w < numberOfWaves; w++)
    {
      float angleStep = 360f / numberOfProjectilesInCircle;
      float currentStartAngle = 0f; // Ou um offset inicial se quiser variar

      if (spinSpeed != 0)
      {
        currentStartAngle += (bossAI.GetTimeSinceAttackStart() * spinSpeed); // Para rotação contínua
      }

      for (int i = 0; i < numberOfProjectilesInCircle; i++)
      {
        float angle = currentStartAngle + (angleStep * i);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        SpawnProjectile(projectilePrefab, bossAI.transform.position, rotation, projectileSpeed);
      }
      yield return new WaitForSeconds(delayBetweenWaves);
    }

    yield return new WaitForSeconds(attackDuration - (numberOfWaves * delayBetweenWaves));
  }
}