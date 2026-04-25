using UnityEngine;

public class RebirthManager : MonoBehaviour
{
    public static RebirthManager Instance;

    public int rebirthCount = 0;

    public float xpMultiplier = 1f;
    public float beliMultiplier = 1f;
    public float bountyMultiplier = 1f;

    [Header("Requirements")]
    public int requiredLevel = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    // ----------------------------
    // CAN REBIRTH?
    // ----------------------------
    public bool CanRebirth(PlayerProgression progression)
    {
        return progression.level >= requiredLevel &&
               StoryManager.Instance.IsComplete();
    }

    // ----------------------------
    // REBIRTH
    // ----------------------------
    public void Rebirth(PlayerProgression progression,
                        ResourceManager resourceManager,
                        UpgradeManager upgradeManager,
                        JobManager jobManager)
    {
        if (!CanRebirth(progression))
        {
            Debug.Log("Rebirth requirements not met.");
            return;
        }

        rebirthCount++;

        // Permanent scaling
        xpMultiplier += 0.2f;
        beliMultiplier += 0.2f;
        bountyMultiplier += 0.1f;

        // ----------------------------
        // RESET SYSTEMS
        // ----------------------------

        progression.ResetProgression();

        StoryManager.Instance.ResetStoryProgress();

        upgradeManager.ResetUpgrades();

        jobManager.ResetJobs();

        resourceManager.ResetForRebirth();

        PlayerStats stats = FindFirstObjectByType<PlayerStats>();

        if (stats != null)
        {
            stats.RecalculateStats();
            stats.HealFull();
        }

        Save();

        Debug.Log($"REBIRTH #{rebirthCount} COMPLETE");
    }

    // ----------------------------
    // GET MULTIPLIERS
    // ----------------------------
    public float GetXPMult() => xpMultiplier;
    public float GetBeliMult() => beliMultiplier;
    public float GetBountyMult() => bountyMultiplier;

    // ----------------------------
    // SAVE / LOAD
    // ----------------------------
    void Save()
    {
        PlayerPrefs.SetInt("REBIRTH_COUNT", rebirthCount);
        PlayerPrefs.SetFloat("REBIRTH_XP", xpMultiplier);
        PlayerPrefs.SetFloat("REBIRTH_BELI", beliMultiplier);
        PlayerPrefs.SetFloat("REBIRTH_BOUNTY", bountyMultiplier);
        PlayerPrefs.Save();
    }

    void Load()
    {
        rebirthCount = PlayerPrefs.GetInt("REBIRTH_COUNT", 0);
        xpMultiplier = PlayerPrefs.GetFloat("REBIRTH_XP", 1f);
        beliMultiplier = PlayerPrefs.GetFloat("REBIRTH_BELI", 1f);
        bountyMultiplier = PlayerPrefs.GetFloat("REBIRTH_BOUNTY", 1f);
    }
}