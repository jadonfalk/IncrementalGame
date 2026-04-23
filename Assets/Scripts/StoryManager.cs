using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public List<EnemyData> enemies = new List<EnemyData>();

    public int currentEnemyIndex = 0;

    private const string SAVE_KEY = "StoryIndex";
    private const string COMPLETE_KEY = "StoryComplete";

    public bool IsStoryComplete { get; private set; } = false;

    void Awake()
    {
        // Singleton protection
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // CRITICAL FIX: Load state immediately (not in Start)
        LoadProgress();
    }

    // ----------------------------
    // GET CURRENT ENEMY
    // ----------------------------
    public EnemyData GetCurrentEnemy()
    {
        if (IsStoryComplete)
            return null;

        if (currentEnemyIndex >= enemies.Count)
            return null;

        return enemies[currentEnemyIndex];
    }

    // ----------------------------
    // COMPLETE ENEMY
    // ----------------------------
    public void CompleteCurrentEnemy()
    {
        currentEnemyIndex++;

        if (currentEnemyIndex >= enemies.Count)
        {
            IsStoryComplete = true;

            PlayerPrefs.SetInt(COMPLETE_KEY, 1);
            PlayerPrefs.Save();

            Debug.Log("Story Completed!");
        }

        SaveProgress();
    }

    // ----------------------------
    // SAVE
    // ----------------------------
    void SaveProgress()
    {
        PlayerPrefs.SetInt(SAVE_KEY, currentEnemyIndex);
        PlayerPrefs.Save();
    }

    // ----------------------------
    // LOAD
    // ----------------------------
    void LoadProgress()
    {
        currentEnemyIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);
        IsStoryComplete = PlayerPrefs.GetInt(COMPLETE_KEY, 0) == 1;
    }

    // ----------------------------
    // RESET
    // ----------------------------
    public void ResetStoryProgress()
    {
        currentEnemyIndex = 0;
        IsStoryComplete = false;

        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(COMPLETE_KEY);
        PlayerPrefs.Save();

        Debug.Log("Story fully reset.");
    }

    public bool IsComplete()
    {
        return IsStoryComplete || currentEnemyIndex >= enemies.Count;
    }

    public int GetCurrentIndex()
    {
        return currentEnemyIndex;
    }
}