using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Info")]
    public string enemyName;

    [Header("Stats")]
    public float maxHP = 10f;
    public float currentHP;
    public float damage = 2f;
    public float speed = 3f;

    [Header("Rewards")]
    public float beliReward = 5f;
    public float xpReward = 3f;
    public float bountyReward = 1f;

    [HideInInspector] public bool isDefeated = false;

    void Start()
    {
        currentHP = maxHP;
    }

    public void InitializeFromData(EnemyData data)
    {
        enemyName = data.enemyName;
        maxHP = data.maxHP;
        damage = data.damage;
        speed = data.speed;

        beliReward = data.beliReward;
        xpReward = data.xpReward;
        bountyReward = data.bountyReward;

        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}