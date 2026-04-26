using UnityEngine;
using TMPro;
using System.Collections;

public class RewardPopupManager : MonoBehaviour
{
    public static RewardPopupManager Instance;

    public GameObject rewardPanel; // assign RewardPopupPanel
    public TMP_Text rewardText;    // assign RewardPopupText

    public float displayTime = 2f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        rewardPanel.SetActive(false);
    }

    public void ShowRewards(float beli, float xp, float bounty)
    {
        rewardText.text = $"+{beli:F2} Beli  +{xp:F2} XP  +{bounty:F2} Bounty";
        rewardPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfterTime(displayTime));
    }

    IEnumerator HideAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        rewardPanel.SetActive(false);
    }
}