using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeShopUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public ResourceManager resourceManager;

    public Button beliButton;          // assign BeliUpgrade button
    public TMP_Text upgradeInfoText;   // assign UpgradeInfo text child

    private Upgrade beliUpgrade;

    void Start()
    {
        // Find the Beli upgrade in the list
        beliUpgrade = upgradeManager.upgrades.Find(u => u.name == "Beli Multiplier");

        if (beliUpgrade == null)
        {
            Debug.LogError("Beli Multiplier upgrade not found in UpgradeManager.upgrades!");
            return;
        }

        // Hook up button click
        beliButton.onClick.AddListener(OnBeliButtonClick);

        // Update the info text
        UpdateUpgradeInfoText();
    }

    /*void OnBeliButtonClick()
    {
        int index = upgradeManager.upgrades.IndexOf(beliUpgrade);
        if (index == -1) return;

        upgradeManager.PurchaseUpgrade(index);

        UpdateUpgradeInfoText();
    }*/

    void OnBeliButtonClick()
    {
        int index = upgradeManager.upgrades.IndexOf(beliUpgrade);
        if (index == -1) return;

        // Call the new TryPurchaseUpgrade with out parameter
        string message;
        bool success = upgradeManager.TryPurchaseUpgrade(index, out message);

        // Log or display the message
        Debug.Log(message);

        // Only update the UI if purchase succeeded
        if (success)
        {
            UpdateUpgradeInfoText();
        }
    }

    void UpdateUpgradeInfoText()
    {
        if (beliUpgrade == null) return;

        string stateText = beliUpgrade.IsMaxLevel() ? "MAX" : "Cost: " + beliUpgrade.GetCost().ToString("F1");
        upgradeInfoText.text = $"{beliUpgrade.name} Lv {beliUpgrade.level}/{beliUpgrade.maxLevel}\n{stateText}";
    }
}