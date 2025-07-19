using UnityEngine;

public class UpgradeChest : MonoBehaviour
{
  [SerializeField] private GameObject upgradePanelPrefab; // O prefab do seu painel de UI de upgrades
  private GameObject instantiatedUpgradePanel; // Para guardar a referência do painel instanciado

  private bool playerInRange = false;

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player")) // Certifique-se de que seu jogador tem a tag "Player"
    {
      playerInRange = true;
      Debug.Log("Jogador perto do baú de upgrade. Pressione 'F' para interagir.");
      // Opcional: Mostrar uma dica de UI para o jogador (ex: "Pressione F")
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = false;
      Debug.Log("Jogador longe do baú de upgrade.");
      // Opcional: Esconder a dica de UI
    }
  }

  private void Update()
  {
    if (playerInRange && Input.GetKeyDown(KeyCode.F)) // Ou o botão que você preferir para interagir
    {
      OpenUpgradePanel();
    }
  }

  private void OpenUpgradePanel()
  {
    if (instantiatedUpgradePanel == null && upgradePanelPrefab != null)
    {
      // Encontra o Canvas na cena para instanciar o painel de UI
      Canvas canvas = FindObjectOfType<Canvas>();
      if (canvas == null)
      {
        Debug.LogError("Nenhum Canvas encontrado na cena para instanciar o painel de upgrade!");
        return;
      }

      instantiatedUpgradePanel = Instantiate(upgradePanelPrefab, canvas.transform);

      // Pega a referência do script do painel e passa as opções
      UpgradePanelUI upgradePanelUI = instantiatedUpgradePanel.GetComponent<UpgradePanelUI>();
      if (upgradePanelUI != null)
      {
        // Pega o PlayerWeaponUpgrader do jogador para obter as opções de upgrade
        PlayerWeaponUpgrader upgrader = FindObjectOfType<PlayerWeaponUpgrader>();
        if (upgrader != null)
        {
          (UpgradeData option1, UpgradeData option2) = upgrader.GetUpgradeOptions();
          upgradePanelUI.SetupUpgradeOptions(option1, option2, upgrader, this);
        }
        else
        {
          Debug.LogError("PlayerWeaponUpgrader não encontrado na cena!");
          Destroy(instantiatedUpgradePanel); // Destrói o painel se não puder configurá-lo
        }
      }
      else
      {
        Debug.LogError("O prefab do painel de upgrade não tem o componente UpgradePanelUI!");
        Destroy(instantiatedUpgradePanel);
      }

      // Desativa a interação com o jogo enquanto o painel está aberto (opcional, mas recomendado)
      Time.timeScale = 0f; // Pausa o jogo
                           // Opcional: Desabilitar o input do jogador aqui
    }
  }

  // Método a ser chamado pelo UpgradePanelUI quando uma escolha é feita ou recusada
  public void CloseAndDestroyChest()
  {
    Time.timeScale = 1f; // Retoma o jogo
    Destroy(instantiatedUpgradePanel); // Destrói o painel de UI
    Destroy(gameObject); // Destrói o baú
  }
}