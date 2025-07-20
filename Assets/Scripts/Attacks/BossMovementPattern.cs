using UnityEngine;
using System.Collections; // Importante para IEnumerator

// Adicione esta linha!
[CreateAssetMenu(fileName = "NewBossMovementPattern", menuName = "Boss Movement Patterns/Base Movement Pattern")]
public abstract class BossMovementPattern : ScriptableObject
{
  [Header("Configurações Comuns de Movimento")]
  public string movementName = "Novo Movimento";
  [TextArea(3, 5)]
  public string description = "Descrição do padrão de movimento.";
  public float movementDuration = 2f; // Duração total deste padrão de movimento
  public float cooldownAfterMovement = 0.5f; // Cooldown antes que o chefe possa iniciar outro movimento ou ataque

  public abstract IEnumerator Execute(BossAI bossAI);

  protected void StopBossMovement(BossAI bossAI)
  {
    Rigidbody2D rb = bossAI.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.linearVelocity = Vector2.zero;
    }
  }
}