using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class JobManager : MonoBehaviour
{
    [Header("Core References")]
    public ResourceManager resourceManager;
    public UpgradeManager upgradeManager;
    //public CombatManager combatManager;
    public StoryManager storyManager;

    [Header("Jobs")]
    public List<Job> jobs = new List<Job>();
    public int activeJobIndex = 0;

    // Generators
    private Generator xpGenerator;
    private Generator beliGenerator;

    [Header("UI")]
    public TMP_Text activeJobText;
    public TMP_Text nextJobRequirement;
    public Button upgradeJobButton;

    public GameObject jobPanel;

    void Start()
    {
        // ----------------------------
        // JOB DEFINITIONS (STORY BASED)
        // ----------------------------
        jobs.Add(new Job { jobName = "Dishwasher", xpPerSecond = 1f, beliPerSecond = 2f, enemiesRequiredToUnlock = 0, isUnlocked = true });
        jobs.Add(new Job { jobName = "Pirate Hunter", xpPerSecond = 3f, beliPerSecond = 5f, enemiesRequiredToUnlock = 1 });
        jobs.Add(new Job { jobName = "Treasure Diver", xpPerSecond = 7f, beliPerSecond = 10f, enemiesRequiredToUnlock = 3 });

        xpGenerator = new XPGenerator();
        beliGenerator = new BeliGenerator();

        if (upgradeJobButton != null)
        {
            upgradeJobButton.onClick.AddListener(TryPromoteJob);
        }

        UpdateActiveJobUI();
    }

    void Update()
    {
        if (jobs.Count == 0)
            return;

        Job activeJob = jobs[activeJobIndex];

        // ----------------------------
        // FIXED MULTIPLIER + FLAT BONUS PIPELINE
        // ----------------------------

        float xpGain =
            (activeJob.xpPerSecond + upgradeManager.GetFlatBonus(ResourceType.XP)) *
            upgradeManager.GetMultiplier(ResourceType.XP);

        float beliGain =
            (activeJob.beliPerSecond + upgradeManager.GetFlatBonus(ResourceType.Beli)) *
            upgradeManager.GetMultiplier(ResourceType.Beli);

        float xp = 0f;
        float beli = 0f;

        ApplyGeneration(xpGenerator, ref xp, xpGain * Time.deltaTime);
        ApplyGeneration(beliGenerator, ref beli, beliGain * Time.deltaTime);

        resourceManager.AddResource(ResourceType.XP, xp);
        resourceManager.AddResource(ResourceType.Beli, beli);

        UpdateActiveJobUI();
    }

    void ApplyGeneration(Generator generator, ref float resource, float amount)
    {
        generator.Produce(ref resource, amount);
    }

    public void TryPromoteJob()
    {
        if (activeJobIndex + 1 >= jobs.Count)
            return;

        int currentStoryIndex = storyManager.GetCurrentIndex();
        Job nextJob = jobs[activeJobIndex + 1];

        if (currentStoryIndex >= nextJob.enemiesRequiredToUnlock)
        {
            activeJobIndex++;
            Debug.Log($"Promoted to {jobs[activeJobIndex].jobName}");
            UpdateActiveJobUI();
        }
    }

    // ----------------------------
    // UI
    // ----------------------------
    void UpdateActiveJobUI()
    {
        if (jobs.Count == 0)
            return;

        Job activeJob = jobs[activeJobIndex];

        activeJobText.text = $"Active Job: {activeJob.jobName}";

        if (activeJobIndex + 1 < jobs.Count)
        {
            Job nextJob = jobs[activeJobIndex + 1];

            int storyIndex = storyManager.GetCurrentIndex();
            int remaining = nextJob.enemiesRequiredToUnlock - storyIndex;

            remaining = Mathf.Max(0, remaining);

            nextJobRequirement.text =
                $"Story Progress Needed: {remaining} enemies";

            upgradeJobButton.interactable = remaining <= 0;
        }
        else
        {
            nextJobRequirement.text = "Max Job Reached";
            upgradeJobButton.interactable = false;
        }
    }
}