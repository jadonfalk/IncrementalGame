using UnityEngine;
using System.Collections.Generic;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;

    [Header("Core References")]
    private ResourceManager resourceManager;
    private UpgradeManager upgradeManager;
    private PlayerProgression progression;

    [Header("Jobs")]
    public List<Job> jobs = new List<Job>();
    public int activeJobIndex = 0;

    private Generator xpGenerator;
    private Generator beliGenerator;

    private bool isReady = false;

    // ----------------------------
    // SINGLETON
    // ----------------------------
    void Awake()
    {
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
        InitializeJobs();
        xpGenerator = new XPGenerator();
        beliGenerator = new BeliGenerator();
    }

    void InitializeJobs()
    {
        if (jobs.Count > 0) return;

        jobs.Add(new Job
        {
            jobName = "Dishwasher",
            xpPerSecond = 1f,
            beliPerSecond = 2f,
            requiredLevel = 1,
            requiredBounty = 0,
            isUnlocked = true
        });

        jobs.Add(new Job
        {
            jobName = "Pirate Hunter",
            xpPerSecond = 3f,
            beliPerSecond = 5f,
            requiredLevel = 5,
            requiredBounty = 50
        });

        jobs.Add(new Job
        {
            jobName = "Treasure Diver",
            xpPerSecond = 7f,
            beliPerSecond = 10f,
            requiredLevel = 10,
            requiredBounty = 150
        });
    }

    // ----------------------------
    // SAFE UPDATE (NO NULL CRASHES)
    // ----------------------------
    void Update()
    {
        if (!TryBindManagers())
            return;

        if (xpGenerator == null || beliGenerator == null)
            return;

        if (jobs.Count == 0)
            return;

        Job activeJob = jobs[activeJobIndex];

        float rebirthXP = RebirthManager.Instance != null ? RebirthManager.Instance.GetXPMult() : 1f;
        float rebirthBeli = RebirthManager.Instance != null ? RebirthManager.Instance.GetBeliMult() : 1f;

        float xpGain =
            (activeJob.xpPerSecond + upgradeManager.GetFlatBonus(ResourceType.XP)) *
            upgradeManager.GetMultiplier(ResourceType.XP) *
            rebirthXP;

        float beliGain =
            (activeJob.beliPerSecond + upgradeManager.GetFlatBonus(ResourceType.Beli)) *
            upgradeManager.GetMultiplier(ResourceType.Beli) *
            rebirthBeli;

        float xp = 0f;
        float beli = 0f;

        xpGenerator.Produce(ref xp, xpGain * Time.deltaTime);
        beliGenerator.Produce(ref beli, beliGain * Time.deltaTime);

        if (resourceManager != null)
        {
            resourceManager.AddResource(ResourceType.XP, xp);
            resourceManager.AddResource(ResourceType.Beli, beli);
        }
    }

    // ----------------------------
    // SAFE BINDING (CRITICAL FIX)
    // ----------------------------
    bool TryBindManagers()
    {
        if (isReady)
            return true;

        resourceManager ??= ResourceManager.Instance;
        upgradeManager ??= UpgradeManager.Instance;
        progression ??= PlayerProgression.Instance;

        if (resourceManager == null ||
            upgradeManager == null ||
            progression == null ||
            RebirthManager.Instance == null)
        {
            return false;
        }

        isReady = true;
        return true;
    }

    // ----------------------------
    // PROMOTION LOGIC
    // ----------------------------
    public bool CanPromote()
    {
        if (!TryBindManagers())
            return false;

        Job next = GetNextJob();
        if (next == null) return false;

        float bounty = resourceManager.GetResource(ResourceType.Bounty);
        int level = progression.level;

        return level >= next.requiredLevel &&
               bounty >= next.requiredBounty;
    }

    public bool TryPromoteJob()
    {
        if (!CanPromote())
            return false;

        activeJobIndex++;
        Debug.Log($"Promoted to {jobs[activeJobIndex].jobName}");
        return true;
    }

    public Job GetActiveJob() => jobs[activeJobIndex];

    public Job GetNextJob()
    {
        if (activeJobIndex + 1 >= jobs.Count)
            return null;

        return jobs[activeJobIndex + 1];
    }

    // ----------------------------
    // RESET (REBIRTH)
    // ----------------------------
    public void ResetJobs()
    {
        activeJobIndex = 0;

        foreach (var job in jobs)
            job.isUnlocked = job.requiredLevel == 1;

        Debug.Log("Jobs reset for rebirth");
    }
}