using UnityEngine;

[System.Serializable]
public class Job
{
    public string jobName;
    public float xpPerSecond;
    public float beliPerSecond;

    // Job Requirements
    public int requiredLevel;
    public float requiredBounty;

    // Track if job is unlocked
    [HideInInspector]
    public bool isUnlocked = false;
}