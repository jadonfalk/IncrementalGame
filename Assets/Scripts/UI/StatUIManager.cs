using UnityEngine;
using TMPro;

public class StatUIManager : MonoBehaviour
{
    private PlayerProgression progression;
    private PlayerStats playerStats;
    private ResourceManager resourceManager;

    [Header("Texts")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text statPointsText;

    public TMP_Text damageText;
    public TMP_Text hpText;
    public TMP_Text defenseText;
    public TMP_Text speedText;
    public TMP_Text luckText;

    public TMP_Text playerDamage;
    public TMP_Text playerHP;
    public TMP_Text playerSpeed;

    void OnEnable()
    {
        BindManagers();
    }

    void Start()
    {
        BindManagers();
    }

    void BindManagers()
    {
        progression = PlayerProgression.Instance ?? FindFirstObjectByType<PlayerProgression>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        resourceManager = ResourceManager.Instance ?? FindFirstObjectByType<ResourceManager>();
    }

    void Update()
    {
        if (!IsReady())
            return;

        float currentXP = progression.GetCurrentLevelXP();
        float requiredXP = progression.GetRequiredXP();

        levelText.text = $"Level: {progression.level}";
        xpText.text = $"XP: {Mathf.FloorToInt(currentXP)} / {Mathf.FloorToInt(requiredXP)}";
        statPointsText.text = $"Points: {progression.statPoints}";

        damageText.text = $"Damage: {progression.damage}";
        hpText.text = $"HP: {progression.hp}";
        defenseText.text = $"Defense: {progression.defense}";
        speedText.text = $"Speed: {progression.speed}";
        luckText.text = $"Luck: {progression.luck}";

        if (playerStats != null)
        {
            playerDamage.text = $"Damage: {playerStats.damage}";
            playerHP.text = $"Max HP: {playerStats.maxHP}";
            playerSpeed.text = $"Speed: {playerStats.speed}";
        }
    }

    bool IsReady()
    {
        return progression != null;
    }

    // ----------------------------
    // BUTTONS
    // ----------------------------
    public void AddDamage()
    {
        if (!BindIfNeeded()) return;

        if (progression.SpendPoint("Damage"))
            playerStats.RecalculateStats();
    }

    public void AddHP()
    {
        if (!BindIfNeeded()) return;

        if (progression.SpendPoint("HP"))
            playerStats.RecalculateStats();
    }

    public void AddDefense()
    {
        if (!BindIfNeeded()) return;

        if (progression.SpendPoint("Defense"))
            playerStats.RecalculateStats();
    }

    public void AddSpeed()
    {
        if (!BindIfNeeded()) return;

        if (progression.SpendPoint("Speed"))
            playerStats.RecalculateStats();
    }

    public void AddLuck()
    {
        if (!BindIfNeeded()) return;

        if (progression.SpendPoint("Luck"))
            playerStats.RecalculateStats();
    }

    bool BindIfNeeded()
    {
        if (progression == null || playerStats == null)
            BindManagers();

        return progression != null && playerStats != null;
    }

    public void ResetStats()
    {
        PlayerProgression progression = PlayerProgression.Instance;

        if (progression == null)
        {
            progression = FindFirstObjectByType<PlayerProgression>();
        }

        if (progression != null)
        {
            progression.ResetStatPoints();

            if (playerStats != null)
                playerStats.RecalculateStats();
        }
    }
}