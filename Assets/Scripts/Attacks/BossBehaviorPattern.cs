using UnityEngine;
using System.Collections;

public abstract class BossBehaviorPattern : ScriptableObject
{
  [Header("Configurações Comuns do Comportamento")]
  public string behaviorName = "Novo Comportamento";
  [TextArea(3, 5)]
  public string description = "Descreve a sequência de ações deste comportamento.";
  public float behaviorMinDuration = 3f; // Duração mínima que este comportamento deve durar
  public float behaviorMaxDuration = 6f; // Duração máxima que este comportamento pode durar

  // Método abstrato que cada comportamento específico DEVE implementar.
  public abstract IEnumerator Execute(BossAI bossAI);

  // Método para parar o Rigidbody2D do chefe
  protected void StopBossMovement(BossAI bossAI)
  {
    Rigidbody2D rb = bossAI.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.linearVelocity = Vector2.zero;
    }
  }
}