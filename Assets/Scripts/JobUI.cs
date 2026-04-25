using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JobUI : MonoBehaviour
{
    public TMP_Text activeJobText;
    public TMP_Text nextJobText;
    public Button promoteButton;

    void Start()
    {
        // ONLY wiring we need
        if (promoteButton != null)
            promoteButton.onClick.AddListener(OnPromoteClicked);
    }

    void Update()
    {
        if (JobManager.Instance == null)
            return;

        UpdateUI();
    }

    void UpdateUI()
    {
        JobManager jm = JobManager.Instance;

        Job current = jm.GetActiveJob();
        Job next = jm.GetNextJob();

        activeJobText.text = $"Job: {current.jobName}";

        if (next != null)
        {
            nextJobText.text =
                $"Next: {next.jobName}\n" +
                $"Req Lvl {next.requiredLevel} | Bounty {next.requiredBounty}";

            promoteButton.interactable = jm.CanPromote();
        }
        else
        {
            nextJobText.text = "Max Job Reached";
            promoteButton.interactable = false;
        }
    }

    void OnPromoteClicked()
    {
        bool success = JobManager.Instance.TryPromoteJob();

        if (success)
        {
            Debug.Log("Job promoted via UI");
        }
    }
}