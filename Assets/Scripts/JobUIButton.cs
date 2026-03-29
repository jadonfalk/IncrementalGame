using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobUIButton : MonoBehaviour
{
    public Job job;
    public Button button;
    public TMP_Text infoText;
    public JobManager jobManager;

    void Start()
    {
        UpdateButton();
        button.onClick.AddListener(() =>
        {
            jobManager.activeJobIndex = jobManager.jobs.IndexOf(job);
        });
    }

    public void UpdateButton()
    {
        infoText.text = $"{job.jobName}\nXP: {job.xpPerSecond}/s\nBeli: {job.beliPerSecond}/s";

        button.interactable = job.isUnlocked;
    }
}