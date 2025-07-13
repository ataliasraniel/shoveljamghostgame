using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
  [Header("Referências")]
  [SerializeField] private TextMeshProUGUI ammoText;

  public static HUDManager Instance { get; private set; }

  private void Awake()
  {
    // Singleton básico
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }

  /// <summary>
  /// Atualiza o contador de munição na HUD (ex: 3 / 6)
  /// </summary>
  public void UpdateAmmo(int current, int max)
  {
    if (ammoText != null)
    {
      ammoText.text = $"{current} / {max}";
    }
  }
}
