// Exemplo: Um GameManager que conta mortes ou gera itens
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private void OnEnable()
  {
    LifedBaseGameObject.OnObjectDied += HandleObjectDeath;
  }

  private void OnDisable()
  {
    LifedBaseGameObject.OnObjectDied -= HandleObjectDeath;
  }

  private void HandleObjectDeath(GameObject diedObject)
  {
    Debug.Log($"O objeto {diedObject.name} acabou de morrer!");
    // Ex: Incrementar pontuação, spawnar um power-up
  }
}