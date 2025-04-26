using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnitUpgrade : ScriptableObject
{
    public string upgradeName;
}

public abstract class DeathUnitUpgrade : BaseUnitUpgrade
{
    public abstract void OnDeath(UnitBaseController argUnitBaseController);
}

public abstract class DamageUnitUpgrade : BaseUnitUpgrade
{
    // OnTakeDamage is called after a death blow, if this is true
    public bool callAfterDeathBlow = false;
    
    public abstract void OnTakeDamage(UnitBaseController argUnitBaseController, int argDamageTaken);
}