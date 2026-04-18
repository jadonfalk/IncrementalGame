using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public ResourceManager resourceManager;

    public List<Upgrade> upgrades = new List<Upgrade>();

    public Dictionary<ResourceType, float> multipliers = new Dictionary<ResourceType, float>();
    public Dictionary<ResourceType, float> flatBonuses = new Dictionary<ResourceType, float>();

    void Start()
    {
        multipliers[ResourceType.Beli] = 1f;
        multipliers[ResourceType.XP] = 1f;
        multipliers[ResourceType.Bounty] = 1f;
        multipliers[ResourceType.Damage] = 1f;
        multipliers[ResourceType.HP] = 1f;

        flatBonuses[ResourceType.XP] = 0f;

        InitializeUpgrades();
    }

    void InitializeUpgrades()
    {
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
                value = 0.15f,      // 15% per level
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
            name = "Bounty Boost",
            maxLevel = 15,
            baseCost = 50,
            costMultiplier = 1.6f,
            effect = new UpgradeEffect
            {
                value = 0.15f,
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
                value = 0.08f,
                targetResource = ResourceType.XP,
                isMultiplier = true
            }
        });
    }

    public bool TryPurchaseUpgrade(int index, out string message)
    {
        try
        {
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

            // Use your existing system
            if (!resourceManager.SpendResource(ResourceType.Beli, cost))
            {
                message = "Not enough Beli.";
                return false;
            }

            // Purchase success
            upgrade.LevelUp();
            ApplyUpgradeEffect(upgrade);

            message = upgrade.name + " upgraded to level " + upgrade.level + "!";
            return true;
        }
        catch (System.Exception e)
        {
            message = "Error purchasing Upgrade.";
            Debug.LogError(e);
            return false;
        }     
        
    }

    /*public void PurchaseUpgrade(int index)
    {
        Upgrade upgrade = upgrades[index];

        if (upgrade.IsMaxLevel())
            return;

        float cost = upgrade.GetCost();

        if (resourceManager.SpendResource(ResourceType.Beli, cost))
        {
            upgrade.LevelUp();
            ApplyUpgradeEffect(upgrade);

            Debug.Log(upgrade.name + " upgraded to level " + upgrade.level);
        }
    }*/

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        ResourceType target = upgrade.effect.targetResource;

        if (upgrade.effect.isMultiplier)
        {
            multipliers[target] += upgrade.effect.value;
        }
        else
        {
            if (!flatBonuses.ContainsKey(target))
                flatBonuses[target] = 0;

            flatBonuses[target] += upgrade.effect.value;
        }
    }

    public float GetMultiplier(ResourceType type)
    {
        if (multipliers.ContainsKey(type))
            return multipliers[type];

        return 1f;
    }

    public float GetFlatBonus(ResourceType type)
    {
        if (flatBonuses.ContainsKey(type))
            return flatBonuses[type];

        return 0f;
    }
}