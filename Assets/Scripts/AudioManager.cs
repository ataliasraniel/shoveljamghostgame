using UnityEngine;
using System.Collections.Generic; // Para usar List ou Dictionary

public class AudioManager : MonoBehaviour
{
  // Singleton Instance
  public static AudioManager Instance { get; private set; }

  [Header("Configurações de Volume Globais")]
  [Range(0f, 1f)] public float masterVolume = 1f;
  [Range(0f, 1f)] public float musicVolume = 0.7f;
  [Range(0f, 1f)] public float sfxVolume = 1f;

  [Header("Componentes de Áudio")]
  [SerializeField] private AudioSource musicSource; // Para músicas de fundo
  [SerializeField] private AudioSource sfxSourcePrefab; // Prefab de AudioSource para SFX (dinâmico)

  // Pool de AudioSources para SFX (evita Instantiate/Destroy frequentes)
  private List<AudioSource> sfxPool;
  [SerializeField] private int sfxPoolSize = 10; // Tamanho inicial do pool

  private void Awake()
  {
    // Implementação do Singleton
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject); // Destroi este novo GameObject se já houver um AudioManager
      return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject); // Faz o AudioManager persistir entre as cenas

    // Garante que os AudioSource existam
    if (musicSource == null)
    {
      musicSource = gameObject.AddComponent<AudioSource>();
      musicSource.loop = true; // Músicas geralmente fazem loop
      musicSource.playOnAwake = false;
    }

    // Inicializa o pool de SFX
    sfxPool = new List<AudioSource>();
    for (int i = 0; i < sfxPoolSize; i++)
    {
      CreateNewSfxSource();
    }

    ApplyVolumes(); // Aplica os volumes iniciais
  }

  private AudioSource CreateNewSfxSource()
  {
    // Se usar um prefab, instancia-o
    AudioSource newSource;
    if (sfxSourcePrefab != null)
    {
      newSource = Instantiate(sfxSourcePrefab, transform); // Instancia como filho do AudioManager
    }
    else // Se não houver prefab, cria um novo GameObject com AudioSource
    {
      GameObject sfxGO = new GameObject("SFX_Source");
      sfxGO.transform.parent = transform; // Faz ser filho do AudioManager
      newSource = sfxGO.AddComponent<AudioSource>();
    }

    newSource.playOnAwake = false;
    newSource.loop = false; // SFX geralmente não fazem loop
    sfxPool.Add(newSource);
    return newSource;
  }

  // --- Métodos de Controle de Volume ---
  public void SetMasterVolume(float volume)
  {
    masterVolume = volume;
    ApplyVolumes();
  }

  public void SetMusicVolume(float volume)
  {
    musicVolume = volume;
    ApplyVolumes();
  }

  public void SetSfxVolume(float volume)
  {
    sfxVolume = volume;
    ApplyVolumes();
  }

  private void ApplyVolumes()
  {
    if (musicSource != null)
    {
      musicSource.volume = musicVolume * masterVolume;
    }

    foreach (AudioSource source in sfxPool)
    {
      source.volume = sfxVolume * masterVolume;
    }
    // Se houver SFX tocando em AudioSources não do pool, eles precisariam ser atualizados também.
    // O ideal é que todos os SFX sejam tocados pelo pool ou PlayClipAtPoint.
  }

  // --- Métodos para Tocar Áudios ---

  /// <summary>
  /// Toca uma música de fundo.
  /// </summary>
  public void PlayMusic(AudioClip clip)
  {
    if (musicSource != null && clip != null)
    {
      if (musicSource.isPlaying && musicSource.clip == clip)
      {
        // Já está tocando esta música, não faz nada
        return;
      }

      musicSource.clip = clip;
      musicSource.volume = musicVolume * masterVolume; // Garante o volume correto ao iniciar
      musicSource.Play();
    }
  }

  /// <summary>
  /// Para a música atual.
  /// </summary>
  public void StopMusic()
  {
    if (musicSource != null)
    {
      musicSource.Stop();
    }
  }

  /// <summary>
  /// Toca um clipe de SFX no AudioManager (pool).
  /// </summary>
  public void PlaySFX(AudioClip clip)
  {
    if (clip == null) return;

    // Tenta encontrar um AudioSource livre no pool
    foreach (AudioSource source in sfxPool)
    {
      if (!source.isPlaying)
      {
        source.clip = clip;
        source.volume = sfxVolume * masterVolume;
        source.Play();
        return;
      }
    }

    // Se nenhum AudioSource livre for encontrado, cria um novo (expande o pool)
    Debug.LogWarning("SFX Pool esgotado! Criando novo AudioSource para SFX.");
    AudioSource newSource = CreateNewSfxSource();
    newSource.clip = clip;
    newSource.volume = sfxVolume * masterVolume;
    newSource.Play();
  }

  /// <summary>
  /// Toca um clipe de SFX em uma posição específica no mundo (ignora a posição do ouvinte).
  /// Útil para sons de impacto que não são do jogador.
  /// </summary>
  public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1f)
  {
    if (clip == null) return;
    AudioSource.PlayClipAtPoint(clip, position, sfxVolume * masterVolume * volumeScale);
  }
}