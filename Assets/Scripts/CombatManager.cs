using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CombatManager : MonoBehaviour
{
    [Header("Core References")]
    public Enemy enemy;
    public PlayerStats playerStats;

    [Header("Managers (ASSIGN IN INSPECTOR)")]
    public ResourceManager resourceManager;
    public UpgradeManager upgradeManager;

    [Header("UI")]
    public TMP_Text enemyHPText;
    public TMP_Text enemyNameText;
    public TMP_Text turnText;
    public Button attackButton;

    [Header("Death UI")]
    public GameObject deathPanel;

    public static event Action OnEnemyKilled;

    private bool playerTurn = true;
    private bool combatActive = true;

    private float baseXPReward;
    private float baseBeliReward;
    private float baseBountyReward;

    void Start()
    {
        // ----------------------------
        // SAFETY CHECKS (NO SINGLETONS)
        // ----------------------------
        if (resourceManager == null || upgradeManager == null)
        {
            Debug.LogError("Missing ResourceManager or UpgradeManager in Inspector.");
            return;
        }

        if (StoryManager.Instance == null)
        {
            Debug.LogError("StoryManager missing.");
            return;
        }

        if (StoryManager.Instance.IsComplete())
        {
            combatActive = false;
            attackButton.interactable = false;
            turnText.text = "Story Complete";
            enemyNameText.text = "All Enemies Defeated";
            return;
        }

        LoadNextEnemy();
    }

    // ----------------------------
    // LOAD ENEMY
    // ----------------------------
    void LoadNextEnemy()
    {
        if (StoryManager.Instance.IsComplete())
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

        // snapshot rewards
        baseXPReward = enemy.xpReward;
        baseBeliReward = enemy.beliReward;
        baseBountyReward = enemy.bountyReward;

        UpdateEnemyUI();
        ResetCombatState();
    }

    // ----------------------------
    // RESET COMBAT
    // ----------------------------
    void ResetCombatState()
    {
        combatActive = true;

        playerStats.HealFull();

        playerTurn = playerStats.speed >= enemy.speed;

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
        float beliMult = upgradeManager.GetMultiplier(ResourceType.Beli);
        float xpMult = upgradeManager.GetMultiplier(ResourceType.XP);
        float bountyMult = upgradeManager.GetMultiplier(ResourceType.Bounty);

        float totalBeli = baseBeliReward * beliMult;
        float totalXP = baseXPReward * xpMult;
        float totalBounty = baseBountyReward * bountyMult;

        Debug.Log($"Multipliers -> XP:{xpMult} Beli:{beliMult} Bounty:{bountyMult}");

        resourceManager.AddResource(ResourceType.Beli, totalBeli);
        resourceManager.AddResource(ResourceType.XP, totalXP);
        resourceManager.AddResource(ResourceType.Bounty, totalBounty);

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

        turnText.text = "You Died";

        attackButton.interactable = false;

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    // ----------------------------
    // RETRY
    // ----------------------------
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