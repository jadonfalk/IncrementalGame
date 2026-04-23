using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 20f;
    public float currentHP;

    public float damage = 2f;
    public float speed = 5f;

    [Header("UI")]
    public Slider hpSlider;
    public TMP_Text hpText;

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP < 0)
            currentHP = 0;

        UpdateUI();
    }

    public void HealFull()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP} / {maxHP}";
        }
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}