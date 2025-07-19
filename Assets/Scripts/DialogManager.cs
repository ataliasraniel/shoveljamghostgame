using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class DialogManager : MonoBehaviour
{
  // Singleton Pattern para fácil acesso
  public static DialogManager Instance { get; private set; }

  [Header("Configurações do Diálogo")]
  [Tooltip("O ScriptableObject que contém todos os diálogos do jogo.")]
  [SerializeField] private DialogueDatabase dialogueDatabase;

  [Header("Elementos de UI (Canvas)")]
  [Tooltip("Painel que contém todos os elementos de UI do diálogo.")]
  [SerializeField] private GameObject dialoguePanel;
  [Tooltip("Componente Text para o nome do falante.")]
  [SerializeField] private TextMeshProUGUI speakerNameText;
  [Tooltip("Componente Text para o texto do diálogo.")]
  [SerializeField] private TextMeshProUGUI dialogueText;
  [Tooltip("Componente Image para o retrato do falante (opcional).")]
  [SerializeField] private Image speakerPortraitImage;
  [Tooltip("Componente AudioSource para tocar clipes de voz (opcional).")]
  [SerializeField] private AudioSource voiceAudioSource;

  [Header("Configurações Avançadas")]
  [Tooltip("Velocidade da digitação do texto (caracteres por segundo).")]
  [SerializeField] private float typingSpeed = 50f;
  // O minLineDisplayTime e a displayDuration da DialogueLine se tornam menos relevantes
  // já que o avanço será manual, mas podemos mantê-los para outras finalidades ou removê-los.
  // [Tooltip("Tempo mínimo que uma linha de diálogo fica visível, mesmo se não tiver áudio.")]
  // [SerializeField] private float minLineDisplayTime = 1f;

  // Estado interno do diálogo
  private Queue<DialogueLine> currentDialogueQueue;
  private DialogueSequence currentSequence;
  private Coroutine typingCoroutine;
  private bool isDialogueActive = false;
  private bool isTyping = false; // Indica se o texto está sendo digitado
  private bool canAdvance = false; // Indica se o jogador pode avançar para a próxima linha
  private Dictionary<string, DialogueSequence> dialogueMap;

  // Eventos para notificar outros sistemas
  public static event Action OnDialogueStarted;
  public static event Action OnDialogueEnded;
  public static event Action<DialogueLine> OnLineDisplayed;

  private void Awake()
  {
    // Implementação do Singleton
    // if (Instance != null && Instance != this)
    // {
    //   Destroy(gameObject);
    // }
    // else
    // {
    //   Instance = this;
    //   DontDestroyOnLoad(gameObject);
    // }

    dialogueMap = new Dictionary<string, DialogueSequence>();
    if (dialogueDatabase != null && dialogueDatabase.dialogueSequences != null)
    {
      foreach (var sequence in dialogueDatabase.dialogueSequences)
      {
        if (!string.IsNullOrEmpty(sequence.sequenceID) && !dialogueMap.ContainsKey(sequence.sequenceID))
        {
          dialogueMap.Add(sequence.sequenceID, sequence);
        }
        else if (string.IsNullOrEmpty(sequence.sequenceID))
        {
          Debug.LogWarning($"Dialogue Sequence sem ID encontrada no {dialogueDatabase.name}. Certifique-se de preencher todos os IDs.");
        }
        else
        {
          Debug.LogWarning($"ID de sequência de diálogo duplicado: {sequence.sequenceID} no {dialogueDatabase.name}. A primeira ocorrência será usada.");
        }
      }
    }
    else
    {
      Debug.LogError("Dialogue Database não atribuída ou vazia no DialogManager.");
    }

    if (dialoguePanel != null)
    {
      dialoguePanel.SetActive(false);
    }
  }

  void Update()
  {
    // Só permite avanço se o diálogo estiver ativo E o texto não estiver digitando E puder avançar
    if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) // Ou outra tecla de avanço que você definir
    {
      if (isTyping)
      {
        // Se estiver digitando, completa a linha imediatamente
        CompleteCurrentLine();
      }
      else if (canAdvance) // Só avança se a linha atual já foi exibida por completo
      {
        DisplayNextLine();
      }
    }
  }

  /// <summary>
  /// Inicia uma sequência de diálogo específica.
  /// Chamado por scripts de terceiros (ex: um Trigger, um NPC).
  /// </summary>
  /// <param name="sequenceID">O ID da sequência de diálogo a ser iniciada.</param>
  public void StartDialogue(string sequenceID)
  {
    if (isDialogueActive)
    {
      Debug.LogWarning("Já existe um diálogo ativo. Não é possível iniciar outro agora.");
      return;
    }

    if (dialogueMap.TryGetValue(sequenceID, out currentSequence))
    {
      currentDialogueQueue = new Queue<DialogueLine>(currentSequence.lines);

      if (currentDialogueQueue.Count == 0)
      {
        Debug.LogWarning($"Sequência de diálogo '{sequenceID}' está vazia.");
        return;
      }

      isDialogueActive = true;
      canAdvance = false; // Garante que não se possa avançar antes da primeira linha
      dialoguePanel.SetActive(true);
      OnDialogueStarted?.Invoke();
      Debug.Log($"Iniciando diálogo: {sequenceID}");

      DisplayNextLine();
    }
    else
    {
      Debug.LogError($"Sequência de diálogo com ID '{sequenceID}' não encontrada no Dialogue Database.");
    }
  }

  /// <summary>
  /// Exibe a próxima linha de diálogo da fila.
  /// </summary>
  private void DisplayNextLine()
  {
    if (currentDialogueQueue.Count == 0)
    {
      EndDialogue(); // Finaliza o diálogo quando não há mais linhas
      return;
    }

    DialogueLine line = currentDialogueQueue.Dequeue();

    speakerNameText.text = line.speakerName;
    if (speakerPortraitImage != null)
    {
      speakerPortraitImage.sprite = line.speakerPortrait;
      speakerPortraitImage.gameObject.SetActive(line.speakerPortrait != null);
    }

    if (typingCoroutine != null)
    {
      StopCoroutine(typingCoroutine);
    }
    typingCoroutine = StartCoroutine(TypeSentence(line));

    canAdvance = false; // Desabilita o avanço enquanto a nova linha está sendo exibida/digitada
    OnLineDisplayed?.Invoke(line);
  }

  /// <summary>
  /// Coroutine para o efeito de digitação do texto.
  /// </summary>
  private IEnumerator TypeSentence(DialogueLine line)
  {
    isTyping = true;
    dialogueText.text = "";

    if (voiceAudioSource != null)
    {
      voiceAudioSource.Stop(); // Para qualquer áudio anterior
      if (line.voiceClip != null)
      {
        voiceAudioSource.clip = line.voiceClip;
        voiceAudioSource.Play();
      }
    }

    foreach (char letter in line.dialogueText.ToCharArray())
    {
      dialogueText.text += letter;
      yield return new WaitForSeconds(1f / typingSpeed);
    }
    isTyping = false;

    // Espera o clipe de voz terminar, se houver
    if (voiceAudioSource != null && voiceAudioSource.isPlaying)
    {
      yield return new WaitWhile(() => voiceAudioSource.isPlaying);
    }

    // AGORA, e SOMENTE AGORA, o jogador pode avançar
    canAdvance = true;
  }

  /// <summary>
  /// Finaliza o diálogo atual.
  /// </summary>
  private void EndDialogue()
  {
    isDialogueActive = false;
    canAdvance = false; // Desativa o avanço
    if (dialoguePanel != null)
    {
      dialoguePanel.SetActive(false);
    }
    if (typingCoroutine != null)
    {
      StopCoroutine(typingCoroutine);
    }
    if (voiceAudioSource != null)
    {
      voiceAudioSource.Stop();
    }
    OnDialogueEnded?.Invoke();
    Debug.Log("Diálogo finalizado.");
    // NOVO: Invoca o UnityEvent se ele estiver atribuído na sequência atual
    if (currentSequence != null && currentSequence.onDialogueEndEvent != null)
    {
      currentSequence.onDialogueEndEvent.Invoke();
      Debug.Log($"UnityEvent de término de diálogo para '{currentSequence.sequenceID}' invocado.");
    }
    currentSequence = null; // Limpa a referência da sequência atual
  }

  // --- Funções Auxiliares para Controle ---

  /// <summary>
  /// Retorna se um diálogo está ativo no momento.
  /// </summary>
  public bool IsDialogueActive()
  {
    return isDialogueActive;
  }

  /// <summary>
  /// Retorna se o texto está sendo digitado no momento.
  /// </summary>
  public bool IsTyping()
  {
    return isTyping;
  }

  /// <summary>
  /// Pula a digitação atual e mostra o texto completo.
  /// </summary>
  public void CompleteCurrentLine()
  {
    if (isDialogueActive && isTyping)
    {
      if (typingCoroutine != null)
      {
        StopCoroutine(typingCoroutine);
      }
      dialogueText.text = currentDialogueQueue.Peek().dialogueText;
      isTyping = false;
      canAdvance = true; // Permite avançar imediatamente após completar a digitação
    }
  }
}