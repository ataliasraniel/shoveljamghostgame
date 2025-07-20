using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRotatingLaserAttack", menuName = "Boss Attacks/Rotating Laser")]
public class RotatingLaserAttack : BossAttackPattern
{
  public enum RotationDirection { Clockwise, CounterClockwise }

  [Header("Específico: Laser Giratório")]
  public GameObject laserLinePrefab; // Prefab de um GameObject com LineRenderer para o laser
  public int numberOfLasers = 1;
  public float rotationSpeed = 90f; // Graus por segundo
  public RotationDirection rotationDirection = RotationDirection.Clockwise;
  public float laserLength = 10f;
  public float chargeUpTime = 0.5f; // Tempo antes do laser realmente aparecer
  public float laserActiveDuration = 3f; // Duração que o laser fica girando

  public override IEnumerator Execute(BossAI bossAI)
  {
    Debug.Log($"{bossAI.name} iniciando ataque: {attackName}");
    if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, bossAI.transform.position);

    List<GameObject> activeLasers = new List<GameObject>();

    // --- CÁLCULO DA POSIÇÃO E ROTAÇÃO INICIAL DOS LASERS ---
    float angleStep = 360f / numberOfLasers; // Ângulo entre cada laser
    for (int i = 0; i < numberOfLasers; i++)
    {
      float initialAngle = i * angleStep; // Calcula o ângulo inicial para cada laser
      Quaternion initialRotation = Quaternion.Euler(0, 0, initialAngle);

      GameObject laserGO = Instantiate(laserLinePrefab, bossAI.transform.position, initialRotation);
      laserGO.transform.parent = bossAI.transform; // Faz o laser seguir o boss e herdar sua rotação

      activeLasers.Add(laserGO);

      // Configura o LineRenderer (assumindo que o prefab tem um)
      LineRenderer lr = laserGO.GetComponent<LineRenderer>();
      if (lr != null)
      {
        lr.SetPosition(0, Vector3.zero); // Ponto de origem relativo ao boss
        lr.SetPosition(1, Vector3.right * laserLength); // Ponto final, será rotacionado pelo transform do laserGO
      }
      // Anexa o script de tratamento de dano
      LaserDamageHandler ldh = laserGO.GetComponent<LaserDamageHandler>();
      if (ldh != null)
      {
        ldh.SetLaserDamage(damage);
      }
    }

    yield return new WaitForSeconds(chargeUpTime); // Espera o charge up

    float timer = 0f;
    while (timer < laserActiveDuration)
    {
      float currentRotationAmount = (rotationDirection == RotationDirection.Clockwise ? -1 : 1) * rotationSpeed * Time.deltaTime;
      for (int i = 0; i < activeLasers.Count; i++)
      {
        if (activeLasers[i] != null)
        {
          // Cada laser gira em torno do seu próprio pivô (que é o Boss.transform.position)
          // Não precisamos mais de RotateAround se o laser é filho do boss e a rotação é aplicada ao próprio transform do laser.
          // Aplicar rotação diretamente ao transform do laser filho
          activeLasers[i].transform.Rotate(Vector3.forward, currentRotationAmount);
        }
      }
      timer += Time.deltaTime;
      yield return null; // Espera o próximo frame
    }

    // Destroi os lasers após a duração
    foreach (GameObject laser in activeLasers)
    {
      if (laser != null) Destroy(laser);
    }

    yield return new WaitForSeconds(cooldownAfterAttack - laserActiveDuration); // Cooldown restante
  }
}