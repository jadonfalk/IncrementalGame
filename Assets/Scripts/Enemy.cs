using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP = 10f;
    public float currentHP;

    // Rewards
    public float beliReward = 5f;
    public float xpReward = 3f;
    public float bountyReward = 1f;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public void Respawn()
    {
        currentHP = maxHP;
        Debug.Log("Enemy respawned!");
    }
}