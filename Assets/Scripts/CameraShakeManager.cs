using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
  public static CameraShakeManager Instance { get; private set; }

  [Header("Configuração do shake")]
  [SerializeField] private CinemachineImpulseSource impulseSource;

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }

  /// <summary>
  /// Dispara um shake de câmera com determinada força
  /// </summary>
  public void Shake(float force = 1f)
  {
    if (impulseSource != null)
    {
      Debug.Log("Shaking by " + force);
      impulseSource.GenerateImpulseWithForce(force);
    }
  }
}
