using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
  [Header("Menus")]
  [SerializeField] private GameObject mainPanel;
  [SerializeField] private GameObject creditsPanel;

  [Header("Áudio")]
  [SerializeField] private Toggle musicToggle;
  [SerializeField] private Toggle sfxToggle;

  [Header("AudioMixers")]
  [SerializeField] private AudioSource musicSource;
  [SerializeField] private AudioSource sfxSource;

  [SerializeField] private List<GameObject> objectsToDisable;

  private void Start()
  {
    mainPanel.SetActive(true);
    creditsPanel.SetActive(false);
    if (musicSource != null)
      musicToggle.isOn = musicSource.mute == false;
    if (sfxSource != null)
      sfxToggle.isOn = sfxSource.mute == false;
  }

  public void PlayGame()
  {
    // Altere para o nome da cena do jogo
    SceneManager.LoadScene("Game");
  }

  public void ShowCredits()
  {
    mainPanel.SetActive(false);
    creditsPanel.SetActive(true);
    foreach (var obj in objectsToDisable)
    {
      obj.SetActive(false);
    }
  }

  public void BackToMenu()
  {
    mainPanel.SetActive(true);
    creditsPanel.SetActive(false);
    foreach (var obj in objectsToDisable)
    {
      obj.SetActive(true);
    }
  }

  public void QuitGame()
  {
    Application.Quit();
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; // para parar no editor
#endif
  }

  public void ToggleMusic(bool enabled)
  {
    musicSource.mute = !enabled;
  }

  public void ToggleSFX(bool enabled)
  {
    sfxSource.mute = !enabled;
  }
}
