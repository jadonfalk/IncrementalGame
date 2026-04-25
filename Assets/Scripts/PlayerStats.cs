using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public PlayerProgression progression;

    [Header("Base Stats")]
    public float baseHP = 20f;
    public float baseDamage = 2f;
    public float baseSpeed = 5f;

    [Header("Final Stats (Calculated)")]
    public float maxHP;
    public float currentHP;
    public float damage;
    public float speed;

    void Start()
    {
        RecalculateStats();
        HealFull();
    }

    // ----------------------------
    // RECALCULATE FROM PROGRESSION
    // ----------------------------
    public void RecalculateStats()
    {
        if (progression == null)
        {
            Debug.LogError("PlayerProgression not assigned!");
            return;
        }

        // Final stat calculations
        damage = baseDamage + progression.damage;

        maxHP = baseHP + (progression.hp * 10f);

        speed = baseSpeed + progression.speed;

        // Clamp current HP if maxHP changed
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

    }

    // ----------------------------
    // DAMAGE
    // ----------------------------
    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP < 0)
            currentHP = 0;

    }

    // ----------------------------
    // HEAL
    // ----------------------------
    public void HealFull()
    {
        currentHP = maxHP;
    }

    // ----------------------------
    // STATE
    // ----------------------------
    public bool IsDead()
    {
        return currentHP <= 0;
    }
}