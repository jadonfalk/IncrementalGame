using UnityEngine;
using TMPro;

public class UpgradeShopUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;

    public TMP_Text beliText;
    public TMP_Text xpText;
    public TMP_Text bountyText;

    private Upgrade beliUpgrade;
    private Upgrade xpUpgrade;
    private Upgrade bountyUpgrade;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeManager not assigned.");
            return;
        }

        beliUpgrade = FindUpgrade("Beli Multiplier");
        xpUpgrade = FindUpgrade("XP Gain Boost");
        bountyUpgrade = FindUpgrade("Bounty Gain Boost");

        UpdateUI();
    }

    Upgrade FindUpgrade(string name)
    {
        Upgrade u = upgradeManager.upgrades.Find(x => x.name == name);

        if (u == null)
        {
            Debug.LogError("Upgrade not found: " + name);
        }

        return u;
    }

    // ----------------------------
    // BUTTON FUNCTIONS (ONCLICK)
    // ----------------------------

    public void BuyBeli()
    {
        BuyUpgrade(beliUpgrade);
    }

    public void BuyXP()
    {
        BuyUpgrade(xpUpgrade);
    }

    public void BuyBounty()
    {
        BuyUpgrade(bountyUpgrade);
    }

    void BuyUpgrade(Upgrade upgrade)
    {
        if (upgrade == null || upgradeManager == null)
            return;

        int index = upgradeManager.upgrades.IndexOf(upgrade);

        string message;
        bool success = upgradeManager.TryPurchaseUpgrade(index, out message);

        Debug.Log(message);

        if (success)
        {
            UpdateUI();
        }
    }

    // ----------------------------
    // UI UPDATE
    // ----------------------------
    void UpdateUI()
    {
        if (beliUpgrade != null)
            beliText.text = Format(beliUpgrade);

        if (xpUpgrade != null)
            xpText.text = Format(xpUpgrade);

        if (bountyUpgrade != null)
            bountyText.text = Format(bountyUpgrade);
    }

    string Format(Upgrade u)
    {
        string cost = u.IsMaxLevel()
            ? "MAX"
            : "Cost: " + u.GetCost().ToString("F1");

        return $"{u.name}\nLv {u.level}/{u.maxLevel}\n{cost}";
    }
}