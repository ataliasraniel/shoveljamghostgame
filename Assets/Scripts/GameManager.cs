// Exemplo: Um GameManager que conta mortes ou gera itens
using UnityEngine;

public class GameManager : MonoBehaviour
{

  public static GameManager Instance { get; private set; }

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      // DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }
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

  public void SpawnPowerUp(GameObject poweup, Vector3 position)
  {
    print("Spawning powerup at position: " + position);
  }


  #region levelDialogueEvents

  public void onEndDialogueLevelOne()
  {
    Debug.Log("Fim do diálogo do nível 1. Chamando função para avançar para o nível 2.");
  }

  #endregion
}