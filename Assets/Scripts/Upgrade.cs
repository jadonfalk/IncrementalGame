using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string name;

    public int level = 0;
    public int maxLevel;

    public float baseCost;
    public float costMultiplier;

    public UpgradeState state = UpgradeState.Available;

    public UpgradeEffect effect;

    public float GetCost()
    {
        return baseCost * Mathf.Pow(costMultiplier, level);
    }

    public bool IsMaxLevel()
    {
        return level >= maxLevel;
    }

    public void LevelUp()
    {
        level++;
    }
}