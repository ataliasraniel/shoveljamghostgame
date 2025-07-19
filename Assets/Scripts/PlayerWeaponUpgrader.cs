using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Certifique-se de que o HUDManager está no namespace correto ou remova "using" se não for necessário
// using SeuNamespaceDoHUDManager; 

public class PlayerWeaponUpgrader : MonoBehaviour
{
  [SerializeField] private PlayerShooter playerShooter; // Referência ao seu script PlayerShooter

  // Lista de todos os UpgradesData disponíveis no jogo (você vai preencher isso no Inspector)
  [SerializeField] private List<UpgradeData> allPossibleUpgrades;

  // Lista dos upgrades que já foram aplicados ao jogador
  private List<UpgradeData> appliedUpgrades = new List<UpgradeData>();

  // No início, pegamos a referência ao PlayerShooter
  private void Awake()
  {
    if (playerShooter == null)
    {
      playerShooter = GetComponent<PlayerShooter>();
      if (playerShooter == null)
      {
        Debug.LogError("PlayerShooter não encontrado no mesmo GameObject que PlayerWeaponUpgrader.", this);
      }
    }
  }

  /// <summary>
  /// Aplica um upgrade específico ao PlayerShooter do jogador.
  /// </summary>
  /// <param name="upgradeToApply">O objeto UpgradeData a ser aplicado.</param>
  public void ApplyUpgrade(UpgradeData upgradeToApply)
  {
    if (playerShooter == null)
    {
      Debug.LogError("PlayerShooter não referenciado para aplicar upgrade.", this);
      return;
    }
    if (upgradeToApply == null)
    {
      Debug.LogWarning("Tentando aplicar um upgrade nulo.", this);
      return;
    }

    // Aplica os modificadores ao PlayerShooter
    // Note que algumas variáveis são privadas no PlayerShooter.
    // Precisaremos criar métodos públicos ou propriedades para elas.

    playerShooter.ApplyFireRateModifier(upgradeToApply.fireRateModifier);
    playerShooter.ApplyBulletSpeedModifier(upgradeToApply.bulletSpeedModifier);
    playerShooter.ApplyDamageModifier(upgradeToApply.damageModifier);
    playerShooter.ApplyMaxAmmoModifier(upgradeToApply.maxAmmoModifier);
    playerShooter.ApplyReloadTimeModifier(upgradeToApply.reloadTimeModifier);
    playerShooter.ApplyPelletCountModifier(upgradeToApply.pelletCountModifier);

    // Adiciona o upgrade à lista de upgrades aplicados (para rastreamento, se necessário)
    appliedUpgrades.Add(upgradeToApply);

    Debug.Log($"Upgrade '{upgradeToApply.upgradeName}' aplicado com sucesso!");
    // Opcional: Atualizar HUD ou dar feedback visual/sonoro ao jogador
  }

  /// <summary>
  /// Método para ser chamado quando o jogador encontra um baú de upgrade.
  /// Retorna duas opções de upgrade aleatórias (ou pré-definidas).
  /// </summary>
  /// <returns>Uma tupla contendo duas opções de UpgradeData.</returns>
  public (UpgradeData option1, UpgradeData option2) GetUpgradeOptions()
  {
    if (allPossibleUpgrades == null || allPossibleUpgrades.Count < 2)
    {
      Debug.LogWarning("Não há upgrades suficientes na lista 'allPossibleUpgrades'.", this);
      return (null, null); // Retorna nulo se não houver opções suficientes
    }

    // Para uma game jam, podemos simplesmente pegar duas opções aleatórias
    // Para um controle mais fino por fase, você criaria lógica aqui para determinar as opções
    int randomIndex1 = Random.Range(0, allPossibleUpgrades.Count);
    int randomIndex2 = Random.Range(0, allPossibleUpgrades.Count);

    // Garante que as duas opções sejam diferentes
    while (randomIndex1 == randomIndex2 && allPossibleUpgrades.Count > 1)
    {
      randomIndex2 = Random.Range(0, allPossibleUpgrades.Count);
    }

    return (allPossibleUpgrades[randomIndex1], allPossibleUpgrades[randomIndex2]);
  }
}