using UnityEngine;

[System.Serializable]
public class Job
{
    public string jobName;
    public float xpPerSecond;
    public float beliPerSecond;

    // Requirement: how many base enemies must be killed to unlock this job
    public int enemiesRequiredToUnlock = 0;

    // Track if job is unlocked
    [HideInInspector]
    public bool isUnlocked = false;
}