using UnityEngine;
using DG.Tweening;
public class EnemyLifedObject : LifedBaseGameObject
{

  [Header("Configurações do Inimigo")]
  public int scoreValue = 100;
  private Material material;
  protected override void Start()
  {
    material = sprite.material;
    // material.SetColor("_Color", Color.red);
  }

  protected override void OnTakeDamage()
  {
    base.OnTakeDamage();
    material.DOColor(Color.black, 0.1f).SetLoops(2, LoopType.Yoyo);

  }

  protected override void OnDeathAction()
  {
    base.OnDeathAction(); // Chama a implementação base para instanciar efeitos, etc.
    Debug.Log($"Inimigo {gameObject.name} derrotado! Ganhos {scoreValue} pontos.");
    // Adicione sua lógica específica de inimigo aqui (ex: desabilitar IA, tocar som de morte)
    // GameManager.Instance.AddScore(scoreValue); // Exemplo
    // GetComponent<Collider2D>().enabled = false; // Desativa o collider do inimigo
    // GetComponent<SpriteRenderer>().color = Color.red; // Muda a cor para indicar morte temporariamente
    Destroy(gameObject); // Destrói o inimigo
  }
}