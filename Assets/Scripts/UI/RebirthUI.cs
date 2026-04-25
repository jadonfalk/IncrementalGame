using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebirthUI : MonoBehaviour
{
    private PlayerProgression progression;
    private ResourceManager resourceManager;
    private UpgradeManager upgradeManager;
    private JobManager jobManager;

    public TMP_Text rebirthCount;
    public Button rebirthButton;
    public TMP_Text rebirthRequirement;

    void OnEnable()
    {
        BindManagers();
    }

    void Start()
    {
        BindManagers();
    }

    void Update()
    {
        if (RebirthManager.Instance == null || progression == null) { return; }

        bool canRebirth = RebirthManager.Instance.CanRebirth(progression);

        rebirthButton.interactable = canRebirth;

        rebirthRequirement.text =
            "Requires:\n" +
            "• Level " + RebirthManager.Instance.requiredLevel + "\n" +
            "• Story Completed";

        UpdateRebirthUI();
    }

    void BindManagers()
    {
        progression = PlayerProgression.Instance ?? FindFirstObjectByType<PlayerProgression>();
        resourceManager = ResourceManager.Instance ?? FindFirstObjectByType<ResourceManager>();
        upgradeManager = UpgradeManager.Instance ?? FindFirstObjectByType<UpgradeManager>();
        jobManager = JobManager.Instance ?? FindFirstObjectByType<JobManager>();
    }

    public void OnRebirthClicked()
    {
        BindManagers(); // extra safety before button press

        if (RebirthManager.Instance == null)
        {
            Debug.LogError("RebirthManager missing!");
            return;
        }

        if (progression == null || resourceManager == null || upgradeManager == null || jobManager == null)
        {
            Debug.LogError("RebirthUI: Missing references after rebinding.");
            return;
        }

        RebirthManager.Instance.Rebirth(
            progression,
            resourceManager,
            upgradeManager,
            jobManager
        );

        UpdateRebirthUI();

    }

    void UpdateRebirthUI()
    {
        if (RebirthManager.Instance == null || rebirthCount == null)
            return;

        rebirthCount.text = $"Rebirths: {RebirthManager.Instance.rebirthCount}";
    }
}