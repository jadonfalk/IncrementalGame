using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public Enemy enemy;
    public ResourceManager resourceManager;
    public UpgradeManager upgradeManager;
    public TMP_Text enemyHPText; // assign in inspector
    public JobManager jobManager; // assign in inspector

    public float playerDamage = 2f;

    public void AttackEnemy()
    {
        if (enemy == null || resourceManager == null)
            return;

        enemy.TakeDamage(playerDamage);

        // Update enemy HP UI
        enemyHPText.text = $"Enemy HP: {Mathf.Max(enemy.currentHP, 0)} / {enemy.maxHP}";

        if (enemy.IsDead())
        {
            // Reward player
            float totalBeli = enemy.beliReward * upgradeManager.GetMultiplier(ResourceType.Beli);
            float totalXP = enemy.xpReward * upgradeManager.GetMultiplier(ResourceType.XP);
            float totalBounty = enemy.bountyReward * upgradeManager.GetMultiplier(ResourceType.Bounty);


            resourceManager.AddResource(ResourceType.Beli, totalBeli);
            resourceManager.AddResource(ResourceType.XP, totalXP);
            resourceManager.AddResource(ResourceType.Bounty, totalBounty);

            // Notify JobManager
            if (jobManager != null)
            {
                jobManager.OnBaseEnemyKilled();
            }

            // Spawn reward popup
            RewardPopupManager.Instance.ShowRewards(totalBeli, totalXP, totalBounty);

            // Respawn enemy
            enemy.Respawn();

            // Update HP UI after respawn
            enemyHPText.text = $"Enemy HP: {enemy.currentHP} / {enemy.maxHP}";
        }
    }
}