using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitBaseController : MonoBehaviour, ISelectable
{
    private bool initialized = false;
    
    public int maxHP = 0;
    public int currentHP = 0;

    public float movementSpeed => baseMoveSpeed * moveSpeedMultiplier;
    
    [FormerlySerializedAs("moveSpeed")] public float baseMoveSpeed = 0;

    protected float _moveSpeedMultiplier = 1;
    public float moveSpeedMultiplier => _moveSpeedMultiplier;
        
    [SerializeField, Tooltip("Speed multiplier that will be reached")]
    private float minimumSpeedUpMultiplier = .1f;
    
    [SerializeField, Tooltip("Speed multiplier that will be reached")]
    private float maximumSpeedUpMultiplier = 2;
    
    public GameObject visualObject = null;

    private HealthBarController healthBar = null;
    
    [SerializeField]
    private List<BaseUnitUpgrade> unitUpgrades = new List<BaseUnitUpgrade>();
    
    [Header ("Attacks")]
    [SerializeField] protected float attackRange = .4f;
    [SerializeField] protected float attackFrequency = .75f;
    [SerializeField] protected int attackDamage = 5;
    [SerializeField] protected bool isSplashDamage = false;
    [SerializeField] protected float splashRange = .15f;
    
    protected UpdateTimer attackTimer = null;
    
    protected virtual void Awake()
    {
        currentHP = maxHP;
        healthBar = GetComponentInChildren<HealthBarController>();
        
        if (healthBar != null)
        {
            healthBar.SetHealthBarActive(false);
            healthBar.SetHealthBarFill(1);
        }
        
        attackTimer = new UpdateTimer(attackFrequency, HandleAttack);
    }

    protected virtual void Initialize()
    {
        initialized = true;
    }

    public virtual void DoUpdate(float argDelta)
    {
        if (initialized == false)
        {
            Initialize();
        }
    }

    public void TakeDamage(int argDamage)
    {
        currentHP -= argDamage;

        UpdateHealthBar();

        bool isDead = currentHP <= 0;
        if (isDead)
        {
            HandleDeath();
        }
        
        foreach (BaseUnitUpgrade upgrade in unitUpgrades)
        {
            if (upgrade is DamageUnitUpgrade damageUpgrade && (isDead == false || damageUpgrade.callAfterDeathBlow))
            {
                damageUpgrade.OnTakeDamage(this, argDamage);
            }
        }
    }
    
    public void HealUnit(int argHealthRegained)
    {
        if (currentHP <= 0)
        {
            return;
        }

        currentHP += argHealthRegained;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }

        float healthPercent = (float)currentHP / maxHP;

        if (healthPercent >= 1)
        {
            healthBar.SetHealthBarActive(false);
        }
        else
        {
            healthBar.SetHealthBarActive(true);
            healthBar.SetHealthBarFill(healthPercent);
        }
    }

    protected virtual void HandleDeath()
    {
        foreach (BaseUnitUpgrade upgrade in unitUpgrades)
        {
            if (upgrade is DeathUnitUpgrade deathUpgrade)
            {
                deathUpgrade.OnDeath(this);
            }
        }
        
        Destroy(gameObject);
    }

    protected virtual void HandleAttack()
    {
    }

    public virtual void Select()
    {
        Debug.Log($"Unit Selected: {gameObject.name} HP stats: {currentHP}/{maxHP}");
    }

    public void SetMoveSpeedMultiplier(float argSpeedMultiplier)
    {
        _moveSpeedMultiplier = Mathf.Clamp(argSpeedMultiplier, minimumSpeedUpMultiplier, maximumSpeedUpMultiplier);
    }
}
