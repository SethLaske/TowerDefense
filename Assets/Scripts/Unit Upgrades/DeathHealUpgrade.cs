using UnityEngine;

[CreateAssetMenu(fileName = "DeathHeal", menuName = "Unit Upgrades/DeathHeal")]
public class DeathHealUpgrade : DeathUnitUpgrade
{
    [SerializeField, Tooltip("Percent of this enemy's max HP to heal nearby enemies")]
    public float healPercent;

    [SerializeField, Tooltip("Radius around this enemy to heal other enemies")] 
    public float healRadius = 1;
    
    public override void OnDeath(UnitBaseController argUnitBaseController)
    {
        int hpToHeal = (int)(argUnitBaseController.maxHP * healPercent);
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(argUnitBaseController.transform.position, healRadius);

        foreach (Collider2D col in colliders)
        {
            EnemyBaseController enemy = col.GetComponent<EnemyBaseController>();
            if (enemy != null)
            {
                float originalHP = enemy.currentHP;
                enemy.HealUnit(hpToHeal);
                //Debug.Log($"Healing enemy {enemy.gameObject.name} from {originalHP} to {enemy.currentHP}");
            }
        }
    }
}
