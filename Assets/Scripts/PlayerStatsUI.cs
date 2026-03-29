using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public ResourceManager resourceManager;
    public UpgradeManager upgradeManager;

    public TMP_Text beliText;
    public TMP_Text xpText;
    public TMP_Text bountyText;
    public TMP_Text hpText;
    public TMP_Text damageText;

    public float baseHP = 100f;
    public float baseDamage = 2f;

    void Update()
    {
        // Resources
        beliText.text = "Beli: " + resourceManager.GetResource(ResourceType.Beli).ToString("F1");
        xpText.text = "XP: " + resourceManager.GetResource(ResourceType.XP).ToString("F1");
        bountyText.text = "Bounty: " + resourceManager.GetResource(ResourceType.Bounty).ToString("F1");

        // Stats with upgrades applied
        float totalHP = baseHP * upgradeManager.GetMultiplier(ResourceType.HP);
        float totalDamage = baseDamage * upgradeManager.GetMultiplier(ResourceType.Damage);

        hpText.text = "HP: " + totalHP.ToString("F1");
        damageText.text = "Damage: " + totalDamage.ToString("F1");
    }
}