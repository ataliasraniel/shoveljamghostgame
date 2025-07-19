using UnityEngine;
using System;
using UnityEngine.Events; // Necessário para [Serializable]

// Essa classe representa uma única linha de diálogo
[Serializable]
public class DialogueLine
{
  public string speakerName; // Quem está falando
  [TextArea(3, 6)] // Permite múltiplas linhas no Inspector
  public string dialogueText; // O que está sendo falado
  public Sprite speakerPortrait; // Opcional: imagem do personagem
  public AudioClip voiceClip; // Opcional: áudio da fala
  public float displayDuration = 3f; // Duração padrão da linha, se não houver áudio
  public bool autoNext = false;

}

// Essa classe representa uma sequência completa de diálogo (uma "história")
[Serializable]
public class DialogueSequence
{
  public string sequenceID; // ID único para ativar este diálogo (ex: "Level1Intro", "BossFightEnd")
  public DialogueLine[] lines; // Todas as linhas de diálogo desta sequência
  [Tooltip("Eventos a serem disparados ao final desta sequência de diálogo (opcional).")]
  public UnityEvent onDialogueEndEvent; // NOVO: UnityEvent para o callback
}

// Essa classe é um ScriptableObject para armazenar todas as sequências de diálogo
// Isso permite criar um asset de diálogo no seu projeto
[CreateAssetMenu(fileName = "NewDialogueDatabase", menuName = "Dialogue/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
  public DialogueSequence[] dialogueSequences;
}