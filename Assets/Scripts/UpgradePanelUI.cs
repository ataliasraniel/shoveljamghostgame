using TMPro;
using UnityEngine;
using UnityEngine.UI; // Importar para usar componentes de UI

public class UpgradePanelUI : MonoBehaviour
{
  [Header("Elementos de UI")]
  [SerializeField] private GameObject option1Panel; // Painel/GameObject para a primeira opção
  [SerializeField] private TextMeshProUGUI option1Name;       // Texto para o nome da primeira opção
  [SerializeField] private TextMeshProUGUI option1Description; // Texto para a descrição da primeira opção
  [SerializeField] private Button option1Button;    // Botão para selecionar a primeira opção

  [SerializeField] private GameObject option2Panel; // Painel/GameObject para a segunda opção
  [SerializeField] private TextMeshProUGUI option2Name;       // Texto para o nome da segunda opção
  [SerializeField] private TextMeshProUGUI option2Description; // Texto para a descrição da segunda opção
  [SerializeField] private Button option2Button;    // Botão para selecionar a segunda opção

  [SerializeField] private Button rejectButton;     // Botão para recusar todos os upgrades neste baú

  private UpgradeData currentOption1;
  private UpgradeData currentOption2;
  private PlayerWeaponUpgrader upgraderReference; // Referência ao PlayerWeaponUpgrader
  private UpgradeChest parentChest; // Referência ao baú que abriu este painel

  // Método chamado pelo UpgradeChest para configurar as opções
  public void SetupUpgradeOptions(UpgradeData opt1, UpgradeData opt2, PlayerWeaponUpgrader upgrader, UpgradeChest chest)
  {
    currentOption1 = opt1;
    currentOption2 = opt2;
    upgraderReference = upgrader;
    parentChest = chest;

    // Configura a primeira opção
    if (currentOption1 != null)
    {
      option1Panel.SetActive(true);
      option1Name.text = currentOption1.upgradeName;
      option1Description.text = currentOption1.description;
      option1Button.onClick.RemoveAllListeners(); // Limpa listeners anteriores
      option1Button.onClick.AddListener(() => OnOptionSelected(currentOption1));
    }
    else
    {
      option1Panel.SetActive(false); // Esconde se não houver opção
    }

    // Configura a segunda opção
    if (currentOption2 != null)
    {
      option2Panel.SetActive(true);
      option2Name.text = currentOption2.upgradeName;
      option2Description.text = currentOption2.description;
      option2Button.onClick.RemoveAllListeners(); // Limpa listeners anteriores
      option2Button.onClick.AddListener(() => OnOptionSelected(currentOption2));
    }
    else
    {
      option2Panel.SetActive(false); // Esconde se não houver opção
    }

    // Configura o botão de recusar
    if (rejectButton != null)
    {
      rejectButton.onClick.RemoveAllListeners();
      rejectButton.onClick.AddListener(OnRejectAll);
    }
  }

  private void OnOptionSelected(UpgradeData selectedUpgrade)
  {
    upgraderReference.ApplyUpgrade(selectedUpgrade);
    parentChest.CloseAndDestroyChest(); // Fecha o painel e destrói o baú
  }

  private void OnRejectAll()
  {
    Debug.Log("Upgrades recusados. Baú destruído.");
    parentChest.CloseAndDestroyChest(); // Fecha o painel e destrói o baú
  }

  // Opcional: Adicionar um método para fechar o painel se o jogador apertar ESC ou algo parecido
  // private void Update() { if (Input.GetKeyDown(KeyCode.Escape)) OnRejectAll(); }
}