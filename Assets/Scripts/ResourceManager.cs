using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public PlayerProgression progression;

    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    private const string BELI_KEY = "Beli";
    private const string XP_KEY = "XP";
    private const string BOUNTY_KEY = "Bounty";

    void Awake()
    {
        // Singleton + persistence
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    void Initialize()
    {
        if (resources == null)
            resources = new Dictionary<ResourceType, float>();

        // Ensure all keys exist BEFORE loading
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (!resources.ContainsKey(type))
                resources[type] = 0f;
        }

        LoadResources();
    }

    // ----------------------------
    // ADD RESOURCE
    // ----------------------------
    public void AddResource(ResourceType type, float amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0f;

        resources[type] += amount;

        SaveResource(type);
        
        // Trigger levelling when XP changes
        if (type == ResourceType.XP && progression != null)
        {
            progression.RecalculateLevel();
        }
    }

    // ----------------------------
    // GET RESOURCE
    // ----------------------------
    public float GetResource(ResourceType type)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0f;

        return resources[type];
    }

    // ----------------------------
    // SAVE
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
    // LOAD
    // ----------------------------
    public void LoadResources()
    {
        resources[ResourceType.Beli] = PlayerPrefs.GetFloat(BELI_KEY, 0f);
        resources[ResourceType.XP] = PlayerPrefs.GetFloat(XP_KEY, 0f);
        resources[ResourceType.Bounty] = PlayerPrefs.GetFloat(BOUNTY_KEY, 0f);
    }

    // ----------------------------
    // SPEND
    // ----------------------------
    public bool SpendResource(ResourceType type, float amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0f;

        if (resources[type] < amount)
            return false;

        resources[type] -= amount;

        SaveResource(type);
        return true;
    }

    // ----------------------------
    // REBIRTH RESET
    // ----------------------------
    public void ResetForRebirth()
    {
        float bounty = GetResource(ResourceType.Bounty);

        resources[ResourceType.Beli] = 0f;
        resources[ResourceType.XP] = 0f;
        resources[ResourceType.Bounty] = bounty;

        SaveResource(ResourceType.Beli);
        SaveResource(ResourceType.XP);
        SaveResource(ResourceType.Bounty);
    }

    // Optional: force reload manually (useful after scene swap)
    public void Reload()
    {
        LoadResources();
    }
}