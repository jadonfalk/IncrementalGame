using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    private ResourceManager resourceManager;

    public TMP_Text xpText;
    public TMP_Text beliText;
    public TMP_Text bountyText;

    void OnEnable()
    {
        Bind();
    }

    void Bind()
    {
        resourceManager = ResourceManager.Instance;

        if (resourceManager == null)
        {
            resourceManager = FindFirstObjectByType<ResourceManager>();
        }
    }

    void Update()
    {
        if (resourceManager == null)
        {
            Bind();
            return;
        }

        beliText.text = "Beli: " + resourceManager.GetResource(ResourceType.Beli).ToString("F1");
        bountyText.text = "Bounty: " + resourceManager.GetResource(ResourceType.Bounty).ToString("F1");
        xpText.text = "Total XP: " + resourceManager.GetResource(ResourceType.XP).ToString("F1");
    }
}