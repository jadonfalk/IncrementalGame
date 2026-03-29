using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    void Start()
    {
        resources[ResourceType.Beli] = 0;
        resources[ResourceType.XP] = 0;
        resources[ResourceType.Bounty] = 0;
    }

    public void AddResource(ResourceType type, float amount)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] += amount;
        }
    }

    public float GetResource(ResourceType type)
    {
        if (resources.ContainsKey(type))
        {
            return resources[type];
        }

        return 0;
    }

    public bool SpendResource(ResourceType type, float amount)
    {
        if (GetResource(type) >= amount)
        {
            resources[type] -= amount;
            return true;
        }

        return false;
    }
}