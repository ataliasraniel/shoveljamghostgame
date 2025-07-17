using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerShooter : MonoBehaviour
{
  [Header("Tiro")]
  [SerializeField] private GameObject bulletPrefab;
  [SerializeField] private Transform firePoint;
  [SerializeField] private float bulletSpeed = 10f;
  [SerializeField] private float fireRate = 0.25f;
  [SerializeField] private float firePointDistance = 1f;

  [Header("Munição")]
  [SerializeField] private int maxAmmo = 6;
  [SerializeField] private float reloadTime = 1.5f;

  [Header("Laser")]
  [SerializeField] private LineRenderer laserLine;
  [SerializeField] private float laserMaxDistance = 20f;
  [SerializeField] private LayerMask laserCollisionMask;

  [Header("Braço Visual")]
  [SerializeField] private Transform armBase;
  [SerializeField] private LineRenderer armLine;



  [SerializeField] private Light2D shootLight;
  [SerializeField] private Transform gunTransform;


  private int currentAmmo;
  private float nextFireTime = 0f;
  private bool isReloading = false;
  private Camera mainCamera;



  private void Awake()
  {
    mainCamera = Camera.main;
    currentAmmo = maxAmmo;
  }

  private void Start()
  {
    HUDManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    // shootLight.gameObject.SetActive(false);
    shootLight.enabled = false;
  }

  private void Update()
  {
    if (isReloading) return;

    AimAtMouse();
    UpdateLaser();
    UpdateVisualArm();
    HandleFlipByMouse();


    if (Input.GetMouseButton(0) && Time.time >= nextFireTime && currentAmmo > 0)
    {
      Shoot();
      nextFireTime = Time.time + fireRate;
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

    armLine.SetPosition(0, armBase.position);     // ombro
    armLine.SetPosition(1, gunTransform.position);  // mão/arma
  }

  private void AimAtMouse()
  {
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector3 direction = (mouseWorldPos - transform.position).normalized;
    firePoint.position = transform.position + direction * firePointDistance;

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

    // Arma segue a posição e rotação do firePoint
    if (gunTransform != null)
    {
      gunTransform.position = firePoint.position;
      gunTransform.rotation = firePoint.rotation;
    }
  }

  private void Shoot()
  {
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.linearVelocity = firePoint.right * bulletSpeed;
    }

    currentAmmo--;
    HUDManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    StartCoroutine(ShootEffect());
    CameraShakeManager.Instance.Shake(0.5f);


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
    yield return new WaitForSeconds(reloadTime);
    currentAmmo = maxAmmo;
    HUDManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

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

    float gunScaley = Mathf.Abs(gunTransform.localScale.y);

    print(pointingLeft);
    if (gunTransform != null)
    {
      gunTransform.localScale = new Vector3(
          pointingLeft ? -gunScaley : gunScaley,
         pointingLeft ? -gunScaley : gunScaley, gunTransform.localScale.z);
    }
  }

}
