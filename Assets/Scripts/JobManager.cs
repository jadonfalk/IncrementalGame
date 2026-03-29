using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class JobManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public UpgradeManager upgradeManager;

    public List<Job> jobs = new List<Job>();
    public int activeJobIndex = 0;

    public int baseEnemyKills = 0;

    // Generators (abstract system)
    private Generator xpGenerator;
    private Generator beliGenerator;

    // UI references
    public TMP_Text activeJobText;       // "Active Job: Dishwasher"
    public TMP_Text nextJobRequirement;  // "Next Job unlocks at X kills"
    public Button upgradeJobButton;

    public GameObject jobPanel; // parent object containing job buttons (if needed for multiple buttons)

    void Start()
    {
        // Initialize jobs
        jobs.Add(new Job { jobName = "Dishwasher", xpPerSecond = 1f, beliPerSecond = 2f, enemiesRequiredToUnlock = 0, isUnlocked = true });
        jobs.Add(new Job { jobName = "Pirate Hunter", xpPerSecond = 3f, beliPerSecond = 5f, enemiesRequiredToUnlock = 10 });
        jobs.Add(new Job { jobName = "Treasure Diver", xpPerSecond = 7f, beliPerSecond = 10f, enemiesRequiredToUnlock = 30 });

        // Initialize generators
        xpGenerator = new XPGenerator();
        beliGenerator = new BeliGenerator();

        // Hook up Upgrade Job button
        if (upgradeJobButton != null)
        {
            upgradeJobButton.onClick.AddListener(TryPromoteJob);
        }

        // Set UI at start
        UpdateActiveJobUI();
        UpdateJobUI(); // optional if using jobPanel buttons
    }

    void Update()
    {
        if (jobs.Count == 0)
            return;

        Job activeJob = jobs[activeJobIndex];

        // Calculate gains with upgrades
        float xpGain = (activeJob.xpPerSecond + upgradeManager.GetFlatBonus(ResourceType.XP)) *
                        upgradeManager.GetMultiplier(ResourceType.XP);

        float beliGain = activeJob.beliPerSecond *
                         upgradeManager.GetMultiplier(ResourceType.Beli);

        // Temporary resource values (needed for ref)
        float xp = 0f;
        float beli = 0f;

        // Apply generation using abstract system + ref
        ApplyGeneration(xpGenerator, ref xp, xpGain * Time.deltaTime);
        ApplyGeneration(beliGenerator, ref beli, beliGain * Time.deltaTime);

        // Send results to ResourceManager
        resourceManager.AddResource(ResourceType.XP, xp);
        resourceManager.AddResource(ResourceType.Beli, beli);
    }

    // Helper method using ref
    void ApplyGeneration(Generator generator, ref float resource, float amount)
    {
        generator.Produce(ref resource, amount);
    }

    // Call this whenever a base enemy is killed
    public void OnBaseEnemyKilled()
    {
        baseEnemyKills++;

        // Update active job UI to refresh kills remaining and button
        UpdateActiveJobUI();

        // Check for job unlocks for optional job panel
        foreach (Job job in jobs)
        {
            if (!job.isUnlocked && baseEnemyKills >= job.enemiesRequiredToUnlock)
            {
                job.isUnlocked = true;
                Debug.Log($"{job.jobName} unlocked!");
                UpdateJobUI(); // refresh optional job panel UI
            }
        }
    }

    // Try to promote to next job if enough kills
    public void TryPromoteJob()
    {
        if (activeJobIndex + 1 >= jobs.Count)
            return; // No more jobs

        Job nextJob = jobs[activeJobIndex + 1];

        if (baseEnemyKills >= nextJob.enemiesRequiredToUnlock)
        {
            activeJobIndex++;
            Debug.Log($"Promoted to {jobs[activeJobIndex].jobName}!");
            UpdateActiveJobUI();
        }
    }

    // Update the active job text and upgrade button
    void UpdateActiveJobUI()
    {
        if (jobs.Count == 0)
            return;

        activeJobText.text = $"Active Job: {jobs[activeJobIndex].jobName}";

        if (activeJobIndex + 1 < jobs.Count)
        {
            Job nextJob = jobs[activeJobIndex + 1];

            // Calculate remaining kills
            int killsRemaining = nextJob.enemiesRequiredToUnlock - baseEnemyKills;
            if (killsRemaining < 0) killsRemaining = 0;

            nextJobRequirement.text = $"Kills to unlock next job: {killsRemaining}";

            // Enable button only if kills requirement is met
            upgradeJobButton.interactable = killsRemaining == 0;
        }
        else
        {
            nextJobRequirement.text = "No further jobs";
            upgradeJobButton.interactable = false;
        }
    }

    // Optional: update job panel buttons if using multiple buttons
    public void UpdateJobUI()
    {
        if (jobPanel == null) return;

        JobUIButton[] buttons = jobPanel.GetComponentsInChildren<JobUIButton>();
        foreach (JobUIButton b in buttons)
        {
            b.UpdateButton();
        }
    }
}