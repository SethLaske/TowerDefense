using UnityEngine;

[CreateAssetMenu(fileName = "DamageSpeedChange", menuName = "Unit Upgrades/DamageSpeedChange")]
public class DamageSpeedChangeUpgrade : DamageUnitUpgrade
{
    [SerializeField, Tooltip("Speed multiplier applied on every hit, exponential growth")]
    public float speedMultiplierChange = 1.05f;
    
    public override void OnTakeDamage(UnitBaseController argUnitBaseController, int argDamageTaken)
    {
        argUnitBaseController.SetMoveSpeedMultiplier(argUnitBaseController.moveSpeedMultiplier * speedMultiplierChange);
    }
}
