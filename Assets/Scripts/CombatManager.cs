using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Core References")]
    public Enemy enemy;

    private PlayerStats playerStats;
    private PlayerProgression playerProgression;

    [Header("Managers (AUTO BIND)")]
    private ResourceManager resourceManager;
    private UpgradeManager upgradeManager;

    [Header("UI")]
    public TMP_Text enemyHPText;
    public TMP_Text enemyNameText;
    public TMP_Text turnText;
    public Button attackButton;

    [Header("Player UI")]
    public Slider hpSlider;
    public TMP_Text hpText;

    [Header("Death UI")]
    public GameObject deathPanel;

    public static event Action OnEnemyKilled;

    private bool playerTurn = true;
    private bool combatActive = true;

    private float baseXPReward;
    private float baseBeliReward;
    private float baseBountyReward;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindUI();
    }

    void Awake()
    {
        BindManagers();
    }

    void Start()
    {
        if (!ValidateManagers())
            return;

        if (StoryManager.Instance == null || StoryManager.Instance.IsComplete())
        {
            combatActive = false;
            if (attackButton != null) attackButton.interactable = false;

            if (turnText != null) turnText.text = "Story Complete";
            if (enemyNameText != null) enemyNameText.text = "All Enemies Defeated";
            return;
        }

        LoadNextEnemy();
    }

    // ----------------------------
    // MANAGER BINDING
    // ----------------------------
    void BindManagers()
    {
        resourceManager = ResourceManager.Instance;
        upgradeManager = UpgradeManager.Instance;

        playerStats = FindFirstObjectByType<PlayerStats>();
        playerProgression = PlayerProgression.Instance;

        if (resourceManager == null)
            resourceManager = FindFirstObjectByType<ResourceManager>();

        if (upgradeManager == null)
            upgradeManager = FindFirstObjectByType<UpgradeManager>();
    }

    bool ValidateManagers()
    {
        if (resourceManager == null || upgradeManager == null)
        {
            Debug.LogError("Missing ResourceManager or UpgradeManager. Ensure Bootstrap scene exists.");
            return false;
        }

        return true;
    }

    void BindUI()
    {
        // Player UI
        GameObject sliderObj = GameObject.FindGameObjectWithTag("PlayerHPBar");
        GameObject textObj = GameObject.FindGameObjectWithTag("PlayerHPText");

        if (sliderObj != null)
            hpSlider = sliderObj.GetComponent<Slider>();

        if (textObj != null)
            hpText = textObj.GetComponent<TMP_Text>();

        UpdatePlayerUI();
    }

    // ----------------------------
    // LOAD ENEMY
    // ----------------------------
    void LoadNextEnemy()
    {
        if (StoryManager.Instance == null || StoryManager.Instance.IsComplete())
        {
            combatActive = false;
            attackButton.interactable = false;
            turnText.text = "Story Complete";
            return;
        }

        EnemyData data = StoryManager.Instance.GetCurrentEnemy();

        if (data == null)
        {
            combatActive = false;
            attackButton.interactable = false;
            turnText.text = "Story Complete";
            return;
        }

        enemy.InitializeFromData(data);
        enemyNameText.text = enemy.enemyName;

        baseXPReward = enemy.xpReward;
        baseBeliReward = enemy.beliReward;
        baseBountyReward = enemy.bountyReward;

        UpdateEnemyUI();
        ResetCombatState();
    }

    // ----------------------------
    // COMBAT RESET
    // ----------------------------
    void ResetCombatState()
    {
        combatActive = true;

        if (playerStats != null)
            playerStats.HealFull();

        UpdatePlayerUI();
        playerTurn = playerStats != null && playerStats.speed >= enemy.speed;

        UpdateTurnUI();

        attackButton.interactable = playerTurn;
    }

    // ----------------------------
    // ATTACK
    // ----------------------------
    public void AttackEnemy()
    {
        if (!playerTurn || !combatActive)
            return;

        enemy.TakeDamage(playerStats.damage);

        UpdateEnemyUI();

        if (enemy.IsDead())
        {
            attackButton.interactable = false;
            combatActive = false;

            HandleEnemyDefeat();
            playerStats.HealFull();
            UpdatePlayerUI();
            return;
        }

        playerTurn = false;
        attackButton.interactable = false;

        UpdateTurnUI();

        Invoke(nameof(EnemyTurn), 1f);
    }

    // ----------------------------
    // ENEMY TURN
    // ----------------------------
    void EnemyTurn()
    {
        playerStats.TakeDamage(enemy.damage);
        UpdatePlayerUI();

        if (playerStats.IsDead())
        {
            HandlePlayerDeath();
            return;
        }

        playerTurn = true;
        attackButton.interactable = true;

        UpdateTurnUI();
    }

    // ----------------------------
    // DEFEAT ENEMY
    // ----------------------------
    void HandleEnemyDefeat()
    {
        float beliMult =
            upgradeManager.GetMultiplier(ResourceType.Beli) *
            RebirthManager.Instance.GetBeliMult();

        float xpMult =
            upgradeManager.GetMultiplier(ResourceType.XP) *
            RebirthManager.Instance.GetXPMult();

        float bountyMult =
            upgradeManager.GetMultiplier(ResourceType.Bounty) *
            RebirthManager.Instance.GetBountyMult();

        float totalBeli = baseBeliReward * beliMult;
        float totalXP = baseXPReward * xpMult;
        float totalBounty = baseBountyReward * bountyMult;

        Debug.Log($"Multipliers -> XP:{xpMult} Beli:{beliMult} Bounty:{bountyMult}");

        resourceManager.AddResource(ResourceType.Beli, totalBeli);
        resourceManager.AddResource(ResourceType.XP, totalXP);
        resourceManager.AddResource(ResourceType.Bounty, totalBounty);

        // ALWAYS recalc progression AFTER XP gain
        if (playerProgression != null)
            playerProgression.RecalculateLevel();

        if (playerStats != null)
            playerStats.RecalculateStats();

        if (RewardPopupManager.Instance != null)
        {
            RewardPopupManager.Instance.ShowRewards(totalBeli, totalXP, totalBounty);
        }

        OnEnemyKilled?.Invoke();

        StoryManager.Instance.CompleteCurrentEnemy();

        Invoke(nameof(LoadNextEnemy), 1f);
    }

    // ----------------------------
    // PLAYER DEATH
    // ----------------------------
    void HandlePlayerDeath()
    {
        combatActive = false;

        if (turnText != null)
            turnText.text = "You Died";

        attackButton.interactable = false;

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void RetryFight()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        playerStats.HealFull();

        LoadNextEnemy();
    }

    // ----------------------------
    // UI
    // ----------------------------
    void UpdateEnemyUI()
    {
        enemyHPText.text =
            $"Enemy HP: {Mathf.Max(enemy.currentHP, 0)} / {enemy.maxHP}";
    }

    void UpdateTurnUI()
    {
        turnText.text = playerTurn ? "Turn: Player" : "Turn: Enemy";
    }

    // Player HP
    void UpdatePlayerUI()
    {
        if (playerStats == null) return;

        if (hpSlider != null)
        {
            hpSlider.maxValue = playerStats.maxHP;
            hpSlider.value = playerStats.currentHP;
        }

        if (hpText != null)
        {
            hpText.text = $"HP: {Mathf.RoundToInt(playerStats.currentHP)} / {Mathf.RoundToInt(playerStats.maxHP)}";
        }
    
    }

    // ----------------------------
    // RESET STORY
    // ----------------------------
    public void ResetStoryAndReload()
    {
        StoryManager.Instance.ResetStoryProgress();

        combatActive = false;

        turnText.text = "Turn: Player";
        enemyNameText.text = "";
        enemyHPText.text = "";

        attackButton.interactable = false;

        LoadNextEnemy();
    }
}