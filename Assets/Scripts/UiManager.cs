using UnityEngine;

public class UiManager : MonoBehaviour
{
  [Header("Menus")]
  [SerializeField] private GameObject pauseMenu;
  [SerializeField] private GameObject gameOverMenu;
  [SerializeField] private GameObject hud;

  private bool isPaused = false;

  public static UiManager Instance { get; private set; }

  private void Awake()
  {
    // Singleton simples
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    // DontDestroyOnLoad(gameObject); // opcional se for trocar de cenas
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      TogglePause();
    }
  }

  public void TogglePause()
  {
    isPaused = !isPaused;

    pauseMenu.SetActive(isPaused);
    hud.SetActive(!isPaused);
    Time.timeScale = isPaused ? 0f : 1f;
    Cursor.visible = isPaused;
    Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
  }

  public void ShowGameOver()
  {
    gameOverMenu.SetActive(true);
    hud.SetActive(false);
    Time.timeScale = 0f;
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
  }

  public void ResumeGame()
  {
    isPaused = false;
    pauseMenu.SetActive(false);
    hud.SetActive(true);
    Time.timeScale = 1f;
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }
}
