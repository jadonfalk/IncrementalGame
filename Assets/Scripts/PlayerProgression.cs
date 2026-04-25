using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance;

    public ResourceManager resourceManager;

    [Header("Level")]
    public int level = 1;
    public float xpToNextLevel = 100;
    private int lastGrantedLevel = 1;

    private float lastXPValue = 0f;

    public int statPoints = 0;

    [Header("Stats")]
    public int damage = 0;
    public int hp = 0;
    public int defense = 0;
    public int speed = 0;
    public int luck = 0;

    void Awake()
    {
        // ----------------------------
        // SINGLETON + PERSISTENCE FIX
        // ----------------------------
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    void Start()
    {
        ResolveReferences();
        lastXPValue = GetXP();
        RecalculateLevel();
    }

    void Update()
    {
        if (resourceManager == null) { return; }

        float currentXP = GetXP();

        if (Mathf.Abs(currentXP - lastXPValue) > 0.01f)
        {
            lastXPValue = currentXP;
            RecalculateLevel();
        }
    }

    // ----------------------------
    // SAFE REFERENCE RESOLUTION
    // ----------------------------
    void ResolveReferences()
    {
        if (resourceManager == null)
            resourceManager = FindFirstObjectByType<ResourceManager>();
    }

    float GetXP()
    {
        return resourceManager != null
            ? resourceManager.GetResource(ResourceType.XP)
            : 0f;
    }

    // ----------------------------
    // LEVEL CALCULATION
    // ----------------------------
    public void RecalculateLevel()
    {
        ResolveReferences();

        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found!");
            return;
        }

        float xp = resourceManager.GetResource(ResourceType.XP);

        int newLevel = 1;
        float requiredXP = 100;
        float remainingXP = xp;

        while (remainingXP >= requiredXP)
        {
            remainingXP -= requiredXP;
            newLevel++;
            requiredXP *= 1.25f;
        }

        level = newLevel;
        xpToNextLevel = requiredXP;

        // Rebuild SP
        int totalEarnedSP = level * 5;
        int spentPoints =
            damage +
            hp +
            defense +
            speed +
            luck;

        statPoints = totalEarnedSP - spentPoints;

        Debug.Log($"Level recalculated → Level {level}, SP: {statPoints}");

        Save();
    }

    // ----------------------------
    // SPEND STAT POINTS
    // ----------------------------
    public bool SpendPoint(string stat)
    {
        if (statPoints <= 0)
            return false;

        switch (stat)
        {
            case "Damage": damage++; break;
            case "HP": hp++; break;
            case "Defense": defense++; break;
            case "Speed": speed++; break;
            case "Luck": luck++; break;
            default: return false;
        }

        statPoints--;
        Save();
        return true;
    }

    // ----------------------------
    // SAVE / LOAD
    // ----------------------------
    void Save()
    {
        PlayerPrefs.SetInt("LVL", level);
        PlayerPrefs.SetInt("PTS", statPoints);
        PlayerPrefs.SetInt("LAST_LVL", lastGrantedLevel);

        PlayerPrefs.SetInt("STAT_DMG", damage);
        PlayerPrefs.SetInt("STAT_HP", hp);
        PlayerPrefs.SetInt("STAT_DEF", defense);
        PlayerPrefs.SetInt("STAT_SPD", speed);
        PlayerPrefs.SetInt("STAT_LUCK", luck);

        PlayerPrefs.Save();
    }

    void Load()
    {
        level = PlayerPrefs.GetInt("LVL", 1);
        statPoints = PlayerPrefs.GetInt("PTS", 0);
        lastGrantedLevel = PlayerPrefs.GetInt("LAST_LVL", 1);

        damage = PlayerPrefs.GetInt("STAT_DMG", 0);
        hp = PlayerPrefs.GetInt("STAT_HP", 0);
        defense = PlayerPrefs.GetInt("STAT_DEF", 0);
        speed = PlayerPrefs.GetInt("STAT_SPD", 0);
        luck = PlayerPrefs.GetInt("STAT_LUCK", 0);
    }

    // ----------------------------
    // RESET
    // ----------------------------
    public void ResetProgression()
    {
        level = 1;
        statPoints = 0;

        damage = 0;
        hp = 0;
        defense = 0;
        speed = 0;
        luck = 0;

        lastGrantedLevel = 1;

        Save();
    }

    // ----------------------------
    // XP BAR HELPERS
    // ----------------------------
    public float GetCurrentLevelXP()
    {
        float xp = resourceManager.GetResource(ResourceType.XP);

        float tempXP = xp;
        float req = 100;

        for (int i = 1; i < level; i++)
        {
            tempXP -= req;
            req *= 1.25f;
        }

        return Mathf.Clamp(tempXP, 0, req);
    }

    public float GetRequiredXP()
    {
        float req = 100;

        for (int i = 1; i < level; i++)
            req *= 1.25f;

        return req;
    }

    public void ResetStatPoints()
    {
        statPoints = level * 5;

        damage = 0;
        hp = 0;
        defense = 0;
        speed = 0;
        luck = 0;

        Save();
    }
}