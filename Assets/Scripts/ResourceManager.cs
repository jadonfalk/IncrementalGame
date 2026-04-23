using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{

    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    private const string BELI_KEY = "Beli";
    private const string XP_KEY = "XP";
    private const string BOUNTY_KEY = "Bounty";

    void Start()
    {
        if (resources == null)
            resources = new Dictionary<ResourceType, float>();

        LoadResources();
    }

    // ----------------------------
    // ADD RESOURCE
    // ----------------------------
    public void AddResource(ResourceType type, float amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] += amount;

        SaveResource(type);
    }

    // ----------------------------
    // GET RESOURCE
    // ----------------------------
    public float GetResource(ResourceType type)
    {
        if (!resources.ContainsKey(type))
            return 0;

        return resources[type];
    }

    // ----------------------------
    // SAVE SINGLE RESOURCE
    // ----------------------------
    void SaveResource(ResourceType type)
    {
        float value = GetResource(type);

        switch (type)
        {
            case ResourceType.Beli:
                PlayerPrefs.SetFloat(BELI_KEY, value);
                break;

            case ResourceType.XP:
                PlayerPrefs.SetFloat(XP_KEY, value);
                break;

            case ResourceType.Bounty:
                PlayerPrefs.SetFloat(BOUNTY_KEY, value);
                break;
        }

        PlayerPrefs.Save();
    }

    // ----------------------------
    // LOAD ALL RESOURCES
    // ----------------------------
    void LoadResources()
    {
        resources[ResourceType.Beli] = PlayerPrefs.GetFloat(BELI_KEY, 0);
        resources[ResourceType.XP] = PlayerPrefs.GetFloat(XP_KEY, 0);
        resources[ResourceType.Bounty] = PlayerPrefs.GetFloat(BOUNTY_KEY, 0);
    }
    
    // ---------------------------
    // SPEND RESOURCE (UPGRADES)
    // ---------------------------
    public bool SpendResource(ResourceType type, float amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        if (resources[type] < amount)
            return false;

        resources[type] -= amount;

        SaveResource(type);

        return true;
    }
    // ----------------------------
    // OPTIONAL: RESET (DEBUG)
    // ----------------------------
    public void ResetResources()
    {
        PlayerPrefs.DeleteKey(BELI_KEY);
        PlayerPrefs.DeleteKey(XP_KEY);
        PlayerPrefs.DeleteKey(BOUNTY_KEY);

        resources[ResourceType.Beli] = 0;
        resources[ResourceType.XP] = 0;
        resources[ResourceType.Bounty] = 0;
    }
}