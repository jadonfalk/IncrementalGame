using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public bool isInitialized { get; private set; }

    public ResourceManager resourceManager;

    public List<Upgrade> upgrades = new List<Upgrade>();

    public Dictionary<ResourceType, float> multipliers = new Dictionary<ResourceType, float>();
    public Dictionary<ResourceType, float> flatBonuses = new Dictionary<ResourceType, float>();

    void Awake()
    {
        // ----------------------------
        // SINGLETON + PERSISTENCE
        // ----------------------------
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeRuntimeDictionaries();
        InitializeUpgrades();
        LoadUpgrades();

        isInitialized = true;

        Debug.Log("UpgradeManager initialized.");
    }

    // ----------------------------
    // SAFE INIT
    // ----------------------------
    void InitializeRuntimeDictionaries()
    {
        multipliers = new Dictionary<ResourceType, float>();
        flatBonuses = new Dictionary<ResourceType, float>();

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            multipliers[type] = 1f;
            flatBonuses[type] = 0f;
        }
    }

    // ----------------------------
    // UPGRADE DEFINITIONS
    // ----------------------------
    void InitializeUpgrades()
    {
        upgrades.Clear();

        upgrades.Add(new Upgrade
        {
            name = "Max HP Boost",
            maxLevel = 25,
            baseCost = 20,
            costMultiplier = 1.5f,
            effect = new UpgradeEffect
            {
                value = 0.06f,
                targetResource = ResourceType.HP,
                isMultiplier = true
            }
        });

        upgrades.Add(new Upgrade
        {
            name = "XP Generator",
            maxLevel = 10,
            baseCost = 30,
            costMultiplier = 1.6f,
            effect = new UpgradeEffect
            {
                value = 4f,
                targetResource = ResourceType.XP,
                isMultiplier = false
            }
        });

        upgrades.Add(new Upgrade
        {
            name = "Beli Multiplier",
            maxLevel = 15,
            baseCost = 40,
            costMultiplier = 1.6f,
            effect = new UpgradeEffect
            {
                value = 0.15f,
                targetResource = ResourceType.Beli,
                isMultiplier = true
            }
        });

        upgrades.Add(new Upgrade
        {
            name = "Damage Boost",
            maxLevel = 20,
            baseCost = 35,
            costMultiplier = 1.5f,
            effect = new UpgradeEffect
            {
                value = 0.10f,
                targetResource = ResourceType.Damage,
                isMultiplier = true
            }
        });

        upgrades.Add(new Upgrade
        {
            name = "Bounty Gain Boost",
            maxLevel = 15,
            baseCost = 60,
            costMultiplier = 1.6f,
            effect = new UpgradeEffect
            {
                value = 0.10f,
                targetResource = ResourceType.Bounty,
                isMultiplier = true
            }
        });

        upgrades.Add(new Upgrade
        {
            name = "XP Gain Boost",
            maxLevel = 15,
            baseCost = 45,
            costMultiplier = 1.5f,
            effect = new UpgradeEffect
            {
                value = 0.10f,
                targetResource = ResourceType.XP,
                isMultiplier = true
            }
        });
    }

    // ----------------------------
    // PURCHASE
    // ----------------------------
    public bool TryPurchaseUpgrade(int index, out string message)
    {
        if (resourceManager == null)
            resourceManager = ResourceManager.Instance;

        if (index < 0 || index >= upgrades.Count)
        {
            message = "Invalid upgrade.";
            return false;
        }

        Upgrade upgrade = upgrades[index];

        if (upgrade.IsMaxLevel())
        {
            message = "Upgrade is already maxed.";
            return false;
        }

        float cost = upgrade.GetCost();

        if (!resourceManager.SpendResource(ResourceType.Beli, cost))
        {
            message = "Not enough Beli.";
            return false;
        }

        upgrade.LevelUp();
        ApplyUpgradeEffect(upgrade);

        SaveUpgrades();

        message = upgrade.name + " upgraded!";
        return true;
    }

    // ----------------------------
    // APPLY EFFECT
    // ----------------------------
    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        ResourceType target = upgrade.effect.targetResource;

        if (!multipliers.ContainsKey(target))
            multipliers[target] = 1f;

        if (!flatBonuses.ContainsKey(target))
            flatBonuses[target] = 0f;

        if (upgrade.effect.isMultiplier)
            multipliers[target] += upgrade.effect.value;
        else
            flatBonuses[target] += upgrade.effect.value;
    }

    // ----------------------------
    // GETTERS
    // ----------------------------
    public float GetMultiplier(ResourceType type)
    {
        return multipliers.ContainsKey(type) ? multipliers[type] : 1f;
    }

    public float GetFlatBonus(ResourceType type)
    {
        return flatBonuses.ContainsKey(type) ? flatBonuses[type] : 0f;
    }

    // ----------------------------
    // SAVE / LOAD
    // ----------------------------
    void SaveUpgrades()
    {
        foreach (Upgrade u in upgrades)
            PlayerPrefs.SetInt("UPGRADE_" + u.name, u.level);

        PlayerPrefs.Save();
    }

    void LoadUpgrades()
    {
        foreach (Upgrade u in upgrades)
        {
            string key = "UPGRADE_" + u.name;

            if (PlayerPrefs.HasKey(key))
                u.level = PlayerPrefs.GetInt(key);
        }

        RecalculateAllUpgrades();
    }

    // ----------------------------
    // REBUILD SYSTEM
    // ----------------------------
    void RecalculateAllUpgrades()
    {
        InitializeRuntimeDictionaries();

        foreach (Upgrade u in upgrades)
        {
            for (int i = 0; i < u.level; i++)
            {
                ApplyUpgradeEffect(u);
            }
        }

        Debug.Log("Upgrade multipliers recalculated.");
    }

    // ----------------------------
    // RESET
    // ----------------------------
    public void ResetUpgrades()
    {
        foreach (var u in upgrades)
            u.level = 0;

        SaveUpgrades();
        RecalculateAllUpgrades();
    }
}