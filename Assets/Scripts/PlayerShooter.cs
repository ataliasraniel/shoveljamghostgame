using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Certifique-se de que este using existe

public class PlayerShooter : MonoBehaviour
{
  [Header("Tiro - Valores Base (Iniciais)")]
  [SerializeField] private GameObject bulletPrefab;
  [SerializeField] private Transform firePoint;
  [SerializeField] private float baseBulletSpeed = 10f; // Renomeado para baseBulletSpeed
  [SerializeField] private float baseFireRate = 0.25f;  // Renomeado para baseFireRate
  [SerializeField] private float firePointDistance = 1f;
  [SerializeField] private float baseDamage = 1f;       // Dano base inicial da pistola
  [SerializeField] private int basePelletCount = 1;     // Contagem de projéteis base (1 para pistola)


  [Header("Munição - Valores Base (Iniciais)")]
  [SerializeField] private int baseMaxAmmo = 6;         // Renomeado para baseMaxAmmo
  [SerializeField] private float baseReloadTime = 1.5f; // Renomeado para baseReloadTime


  [Header("Laser")]
  [SerializeField] private LineRenderer laserLine;
  [SerializeField] private float laserMaxDistance = 20f;
  [SerializeField] private LayerMask laserCollisionMask;

  [Header("Braço Visual")]
  [SerializeField] private Transform armBase;
  [SerializeField] private LineRenderer armLine;

  [SerializeField] private Light2D shootLight;
  [SerializeField] private Transform gunTransform;

  // --- VARIÁVEIS ATUAIS DA ARMA (MODIFICADAS POR UPGRADES) ---
  private float currentFireRate;    // Tempo entre tiros, menor = mais rápido
  private float currentBulletSpeed;
  private float currentDamage;
  private int currentMaxAmmo;
  private float currentReloadTime;
  private int currentPelletCount;   // Quantidade de projéteis por tiro (para escopeta)

  // --- Variáveis para efeitos de status (Hades Revivido / Twilight Omen) ---
  // Você precisará de lógica no Bullet.cs e no inimigo para usar isso.
  private float currentDoTChance = 0f;    // Chance de aplicar dano ao longo do tempo (DoT)
  private float currentDoTDamage = 0f;    // Dano por segundo do DoT
  private float currentDoTDuration = 0f;  // Duração do DoT

  private float currentMarkChance = 0f;   // Chance de marcar o inimigo (vulnerabilidade)
  private float currentMarkDuration = 0f; // Duração da marcação
  private float currentMarkDamageBonus = 0f; // Bônus de dano contra inimigo marcado

  // Variáveis de estado do jogador
  private int currentAmmo;
  private float nextFireTime = 0f;
  private bool isReloading = false;
  private Camera mainCamera;

  private void Awake()
  {
    mainCamera = Camera.main;
    // Inicializa as variáveis de controle com os valores BASE definidos no Inspector
    currentFireRate = baseFireRate;
    currentBulletSpeed = baseBulletSpeed;
    currentDamage = baseDamage;
    currentMaxAmmo = baseMaxAmmo;
    currentReloadTime = baseReloadTime;
    currentPelletCount = basePelletCount; // Inicia com 1 projétil

    currentAmmo = currentMaxAmmo; // Munição inicial baseada na munição máxima atual
  }

  private void Start()
  {
    // Certifique-se de que HUDManager.Instance não é nulo antes de usar
    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateAmmo(currentAmmo, currentMaxAmmo);
    }
    shootLight.enabled = false;
  }

  private void Update()
  {
    if (isReloading) return;

    AimAtMouse();
    UpdateLaser();
    UpdateVisualArm();
    HandleFlipByMouse();

    // Usa currentFireRate para o controle de tempo
    if (Input.GetMouseButton(0) && Time.time >= nextFireTime && currentAmmo > 0)
    {
      Shoot();
      nextFireTime = Time.time + currentFireRate;
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      StartCoroutine(Reload());
    }
  }

  private void UpdateVisualArm()
  {
    if (armLine == null || armBase == null || gunTransform == null)
      return;

    armLine.SetPosition(0, armBase.position);
    armLine.SetPosition(1, gunTransform.position);
  }

  private void AimAtMouse()
  {
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector3 direction = (mouseWorldPos - transform.position).normalized;
    firePoint.position = transform.position + direction * firePointDistance;

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

    if (gunTransform != null)
    {
      gunTransform.position = firePoint.position;
      gunTransform.rotation = firePoint.rotation;
    }
  }

  private void Shoot()
  {
    // Lógica para atirar múltiplos projéteis (Escopeta) ou um único
    if (currentPelletCount <= 1)
    {
      // Tiro único padrão
      InstantiateBullet(firePoint.position, firePoint.rotation, currentDamage, currentDoTChance, currentDoTDamage, currentDoTDuration, currentMarkChance, currentMarkDuration, currentMarkDamageBonus);
    }
    else // Múltiplos projéteis para efeito de "Escopeta"
    {
      float baseAngle = firePoint.rotation.eulerAngles.z;
      float spreadAngle = 20f; // Ângulo total de dispersão da escopeta (ajuste conforme necessário)

      // Calcula o ângulo inicial para centralizar a dispersão
      float startAngle = baseAngle - (spreadAngle / 2f);

      // Calcula o passo do ângulo entre cada projétil
      float angleStep = currentPelletCount > 1 ? spreadAngle / (currentPelletCount - 1) : 0;

      for (int i = 0; i < currentPelletCount; i++)
      {
        float currentPelletAngle = startAngle + (angleStep * i);
        Quaternion pelletRotation = Quaternion.Euler(0f, 0f, currentPelletAngle);

        InstantiateBullet(firePoint.position, pelletRotation, currentDamage, currentDoTChance, currentDoTDamage, currentDoTDuration, currentMarkChance, currentMarkDuration, currentMarkDamageBonus);
      }
    }

    currentAmmo--;
    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateAmmo(currentAmmo, currentMaxAmmo);
    }
    StartCoroutine(ShootEffect());
    if (CameraShakeManager.Instance != null)
    {
      CameraShakeManager.Instance.Shake(0.5f);
    }
  }

  // Novo método para instanciar e configurar o projétil
  private void InstantiateBullet(Vector3 position, Quaternion rotation, float damage, float dotChance, float dotDamage, float dotDuration, float markChance, float markDuration, float markDamageBonus)
  {
    GameObject bullet = Instantiate(bulletPrefab, position, rotation);
    Bullet bulletScript = bullet.GetComponent<Bullet>();
    if (bulletScript != null)
    {
      bulletScript.SetDamage(damage);
      // Passa informações de efeito de status para o projétil
      bulletScript.SetStatusEffectData(dotChance, dotDamage, dotDuration, markChance, markDuration, markDamageBonus);
    }

    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.linearVelocity = rotation * Vector2.right * currentBulletSpeed;
    }
  }

  private IEnumerator ShootEffect()
  {
    shootLight.enabled = true;
    yield return new WaitForSeconds(0.016f);
    shootLight.enabled = false;
  }

  private System.Collections.IEnumerator Reload()
  {
    isReloading = true;
    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateAmmo(0, currentMaxAmmo); // Mostra 0/MaxAmmo durante a recarga
    }
    yield return new WaitForSeconds(currentReloadTime); // Usa currentReloadTime
    currentAmmo = currentMaxAmmo;
    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateAmmo(currentAmmo, currentMaxAmmo);
    }
    isReloading = false;
  }

  private void UpdateLaser()
  {
    if (laserLine == null) return;

    Vector3 start = firePoint.position;
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector3 direction = (mouseWorldPos - start).normalized;
    Vector3 end = start + direction * laserMaxDistance;

    RaycastHit2D hit = Physics2D.Raycast(start, direction, laserMaxDistance, laserCollisionMask);
    if (hit.collider != null)
    {
      end = hit.point;
    }

    laserLine.SetPosition(0, start);
    laserLine.SetPosition(1, end);
  }

  private void HandleFlipByMouse()
  {
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    bool pointingLeft = mouseWorldPos.x < transform.position.x;

    if (gunTransform != null)
    {
      // Ajusta a escala X para flipar horizontalmente e Y para manter a proporção ou inverter
      // Se a arma deve manter-se "para cima" mesmo virada para esquerda, use Mathf.Abs(gunTransform.localScale.y).
      // Se ela deve virar de cabeça para baixo para a esquerda (como alguns sprites de arma), use -Mathf.Abs(gunTransform.localScale.y).
      gunTransform.localScale = new Vector3(
          pointingLeft ? -Mathf.Abs(gunTransform.localScale.x) : Mathf.Abs(gunTransform.localScale.x), // Flip X
          gunTransform.localScale.y, // Mantém a escala Y original, não inverte
          gunTransform.localScale.z
      );
    }
  }

  // --- MÉTODOS PÚBLICOS PARA APLICAR UPGRADES ---
  // Cada método aplica o modificador e garante que o valor não vá para extremos indesejados.
  public void ApplyFireRateModifier(float modifier)
  {
    // fireRate é tempo entre tiros, então um modificador negativo o torna mais rápido.
    currentFireRate = Mathf.Max(0.05f, currentFireRate + modifier); // Limite mínimo de 0.05s entre tiros
    Debug.Log($"Novo Fire Rate: {currentFireRate}");
  }

  public void ApplyBulletSpeedModifier(float modifier)
  {
    currentBulletSpeed = Mathf.Max(1f, currentBulletSpeed + modifier); // Limite mínimo de 1f
    Debug.Log($"Nova Velocidade do Projétil: {currentBulletSpeed}");
  }

  public void ApplyDamageModifier(float modifier)
  {
    currentDamage = Mathf.Max(0.1f, currentDamage + modifier); // Dano mínimo de 0.1
    Debug.Log($"Novo Dano: {currentDamage}");
  }

  public void ApplyMaxAmmoModifier(int modifier)
  {
    currentMaxAmmo = Mathf.Max(1, currentMaxAmmo + modifier); // Munição máxima mínima de 1
                                                              // Garante que a munição atual não exceda a nova munição máxima
    currentAmmo = Mathf.Min(currentAmmo, currentMaxAmmo);
    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateAmmo(currentAmmo, currentMaxAmmo);
    }
    Debug.Log($"Nova Munição Máxima: {currentMaxAmmo}");
  }

  public void ApplyReloadTimeModifier(float modifier)
  {
    // reloadTime é tempo, então um modificador negativo o torna mais rápido.
    currentReloadTime = Mathf.Max(0.1f, currentReloadTime + modifier); // Limite mínimo de 0.1s
    Debug.Log($"Novo Tempo de Recarga: {currentReloadTime}");
  }

  public void ApplyPelletCountModifier(int modifier)
  {
    currentPelletCount = Mathf.Max(1, currentPelletCount + modifier); // Mínimo de 1 projétil
    Debug.Log($"Novo Contagem de Projéteis (Escopeta): {currentPelletCount}");
  }

  // --- MÉTODOS PÚBLICOS PARA APLICAR MODIFICADORES DE STATUS ---
  // Você precisará de variáveis no UpgradeData para controlar esses modificadores.
  public void ApplyDoTEffectModifier(float chanceModifier, float damageModifier, float durationModifier)
  {
    currentDoTChance = Mathf.Clamp01(currentDoTChance + chanceModifier); // Chance entre 0 e 1
    currentDoTDamage = Mathf.Max(0f, currentDoTDamage + damageModifier);
    currentDoTDuration = Mathf.Max(0f, currentDoTDuration + durationModifier);
    Debug.Log($"Novo DoT: Chance={currentDoTChance}, Dano={currentDoTDamage}, Duração={currentDoTDuration}");
  }

  public void ApplyMarkEffectModifier(float chanceModifier, float durationModifier, float damageBonusModifier)
  {
    currentMarkChance = Mathf.Clamp01(currentMarkChance + chanceModifier);
    currentMarkDuration = Mathf.Max(0f, currentMarkDuration + durationModifier);
    currentMarkDamageBonus = Mathf.Max(0f, currentMarkDamageBonus + damageBonusModifier);
    Debug.Log($"Nova Marcação: Chance={currentMarkChance}, Duração={currentMarkDuration}, Bônus={currentMarkDamageBonus}");
  }
}