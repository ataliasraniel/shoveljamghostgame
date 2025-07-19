using UnityEngine;

// Define um novo tipo de Asset que pode ser criado no menu "Assets/Create/Upgrades"
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Weapon Upgrade")]
public class UpgradeData : ScriptableObject
{
  [Header("Informações do Upgrade")]
  public string upgradeName = "Novo Upgrade"; // Nome do upgrade (ex: "Foco Reforçado")
  [TextArea(3, 5)] // Permite múltiplas linhas para a descrição no Inspector
  public string description = "Uma breve descrição do que este upgrade faz."; // Descrição para o jogador

  [Header("Efeitos na Arma")]
  // Modificadores para as variáveis do PlayerShooter
  public float fireRateModifier = 0f;       // Redução no fireRate (ex: -0.05 para tiro mais rápido)
                                            // Quanto MENOR o fireRate, mais rápido o tiro.
  public float bulletSpeedModifier = 0f;    // Aumento na velocidade do projétil (ex: 2f)
  public float damageModifier = 0f;         // Aumento no dano (ex: 1f por tiro)
  public int maxAmmoModifier = 0;           // Aumento na munição máxima (ex: 2)
  public float reloadTimeModifier = 0f;     // Redução no reloadTime (ex: -0.1f para recarga mais rápida)
  public int pelletCountModifier = 0;       // Quantidade de projéteis adicionais (para simular escopeta)

  [Header("Efeitos de Status (Opcional)")]
  // Modificadores para efeitos de Status que o projétil pode aplicar
  public float dotChanceModifier = 0f;      // Aumenta a chance de aplicar Dano ao Longo do Tempo (0-1)
  public float dotDamageModifier = 0f;      // Aumenta o dano por segundo do DoT
  public float dotDurationModifier = 0f;    // Aumenta a duração do DoT

  public float markChanceModifier = 0f;     // Aumenta a chance de marcar o inimigo (0-1)
  public float markDurationModifier = 0f;   // Aumenta a duração da marcação
  public float markDamageBonusModifier = 0f; // Aumenta o bônus de dano contra inimigo marcado
}