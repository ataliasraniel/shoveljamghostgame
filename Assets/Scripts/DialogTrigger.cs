using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
  [Tooltip("O ID da sequência de diálogo a ser iniciada.")]
  public string dialogueSequenceID;
  [Tooltip("Tag do objeto que pode ativar este diálogo (ex: 'Player').")]
  public string activatorTag = "Player";
  [Tooltip("O diálogo deve ser ativado apenas uma vez?")]
  public bool activateOnce = true;

  private bool hasBeenActivated = false;

  private void OnTriggerEnter2D(Collider2D other) // Para 2D
                                                  // private void OnTriggerEnter(Collider other) // Para 3D
  {
    if (other.CompareTag(activatorTag) && (!activateOnce || !hasBeenActivated))
    {
      if (DialogManager.Instance != null)
      {
        DialogManager.Instance.StartDialogue(dialogueSequenceID);
        hasBeenActivated = true;
        // Opcional: desativar o trigger após o uso
        // gameObject.SetActive(false);
      }
      else
      {
        Debug.LogError("DialogManager não encontrado na cena.");
      }
    }
  }

  // Se você quiser que o diálogo seja ativado por um evento de script sem colisão:
  public void TriggerDialogueManually()
  {
    if (!activateOnce || !hasBeenActivated)
    {
      if (DialogManager.Instance != null)
      {
        DialogManager.Instance.StartDialogue(dialogueSequenceID);
        hasBeenActivated = true;
      }
      else
      {
        Debug.LogError("DialogManager não encontrado na cena.");
      }
    }
  }
}