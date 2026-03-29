using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public ResourceManager resourceManager;

    void Update()
    {
        // Just for testing, prints current resources every frame
        Debug.Log("Beli: " + resourceManager.GetResource(ResourceType.Beli) +
                  " | XP: " + resourceManager.GetResource(ResourceType.XP) +
                  " | Bounty: " + resourceManager.GetResource(ResourceType.Bounty));
    }
}