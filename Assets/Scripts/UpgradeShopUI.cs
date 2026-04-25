using UnityEngine;
using TMPro;
using System.Collections;

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
        StartCoroutine(WaitForManager());
    }

    // ----------------------------
    // SAFE INITIALIZATION (SCENE SAFE)
    // ----------------------------
    IEnumerator WaitForManager()
    {
        while (UpgradeManager.Instance == null || !UpgradeManager.Instance.isInitialized)
        {
            yield return null;
        }

        upgradeManager = UpgradeManager.Instance;

        Initialize();
    }

    // ----------------------------
    // INIT REFERENCES (NO STRINGS AFTER THIS)
    // ----------------------------
    void Initialize()
    {
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeManager missing.");
            return;
        }

        var list = upgradeManager.upgrades;

        beliUpgrade = list.Find(x => x.name == "Beli Multiplier");
        xpUpgrade = list.Find(x => x.name == "XP Gain Boost");
        bountyUpgrade = list.Find(x => x.name == "Bounty Gain Boost");

        if (beliUpgrade == null)
            Debug.LogError("Beli Multiplier upgrade not found");

        if (xpUpgrade == null)
            Debug.LogError("XP Gain Boost upgrade not found");

        if (bountyUpgrade == null)
            Debug.LogError("Bounty Gain Boost upgrade not found");

        UpdateUI();
    }

    // ----------------------------
    // BUTTONS (ONCLICK)
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
        if (upgradeManager == null || upgrade == null)
            return;

        int index = upgradeManager.upgrades.IndexOf(upgrade);

        string message;
        bool success = upgradeManager.TryPurchaseUpgrade(index, out message);

        Debug.Log(message);

        if (success)
            UpdateUI();
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