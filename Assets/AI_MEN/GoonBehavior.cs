using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GoonBehavior : MonoBehaviour
{

    [SerializeField] GameControl gameControl;

    //BASE STATS
    [SerializeField] UnitStats unitStats;

    //GAMEPLAY STATS
    public int currentSTR;
    public int currentDEX;
    public int currentCON;
    public int currentAGL;
    [SerializeField] float damage;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackRange;
    [SerializeField] float maxHitpoints;
    [SerializeField] float hitpoints;
    [SerializeField] float moveSpeed;


    [SerializeField] float perception; // Make a stat that influences range? Or just tweak until feels good

    //EQUPIMENT
    public GameObject weapon;
    WeaponBehavior weaponBehavior;
    WeaponStats weaponStats;
    Animator weaponAnimator;
    public GameObject armor;
    ArmorStats armorStats;

    //TARGETING
    [SerializeField] GameObject target;
    [SerializeField] Vector2 targetVector2;
    public LayerMask targetLayerMask;
    float targetTimer;

    //UTILITY
    float distanceToTarget;
    bool canAttack = true;
    bool hasTarget = false;
    Rigidbody2D rb;
    public Transform projectileOrigin;

    private void Start()
    {
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        weaponBehavior = weapon.GetComponent<WeaponBehavior>();
        weaponStats = weaponBehavior.weaponStats;
        //Debug.Log("AttackSpeed: " + weaponStats.weaponAttackSpeed);
        //Debug.Log("AttackDamage: " + weaponStats.weaponDamage);
        //Debug.Log("AttackRange: " + weaponStats.weaponRange);
        armorStats = armor.GetComponent<ArmorBehavior>().armorStats;
        //Debug.Log("ArmorBonusSTR: " + armorStats.bonusSTR);
        //Debug.Log("ArmorBonusDEX: " + armorStats.bonusDEX);
        //Debug.Log("ArmorBonusCON: " + armorStats.bonusCON);
        //Debug.Log("ArmorBonusAGL: " + armorStats.bonusAGL);

        ApplyStats();
        hitpoints = maxHitpoints;
       
        weaponAnimator.SetFloat("attackSpeed", attackSpeed);
    }

    void Update()
    {
        if (distanceToTarget > 1)
        {
            targetTimer += Time.deltaTime;
        }
        

        if (gameControl.waveRunning && !hasTarget)
        {
            GetTarget();
        }

        if (hasTarget)
        {
            if (targetTimer >= 0.5f)
            {
                GetTarget();
                targetTimer = 0;
            }

            if (!target.activeSelf)
            {
                GetTarget();
            }
            else
            {
                targetVector2 = target.transform.position - transform.position;
            }

            MoveToRange();

            if (distanceToTarget < attackRange && canAttack)
            {
                rb.velocity = Vector2.zero;
                Attack();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            // Return to origin
        }
    }

    void MoveToRange()
    {
        distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > attackRange)
        {
            //rb.velocity = targetVector2.normalized * moveSpeed;
            rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * targetVector2.normalized);
            rb.transform.up = target.transform.position - transform.position;
            //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * moveSpeed);
        }
    }

    public void TakeHit(float incomingDamage)
    {
        hitpoints -= incomingDamage;

        if (hitpoints <= 0)
        {
            //Death animation
            Debug.Log("IM DEAD FR FR");
            gameObject.SetActive(false);
        }
    }

    void ApplyStats()
    {
        currentSTR = unitStats.STR + armorStats.bonusSTR;
        currentDEX = unitStats.DEX + armorStats.bonusDEX;
        currentCON = unitStats.CON + armorStats.bonusCON;
        currentAGL = unitStats.AGL + armorStats.bonusAGL;
        //Debug.Log(name + "'s STR: " + currentSTR);
        //Debug.Log(name + "'s DEX: " + currentDEX);
        //Debug.Log(name + "'s CON: " + currentCON);
        //Debug.Log(name + "'s AGL: " + currentAGL);

        //Debug.Log(weapon.name + "'s damage: " + weaponStats.weaponDamage);
        //Debug.Log(weapon.name + "'s attackSpeed: " + weaponStats.weaponAttackSpeed);
        //Debug.Log(weapon.name + "'s range: " + weaponStats.weaponRange);

        damage = currentSTR + weaponStats.weaponDamage;
        attackSpeed = ((1.2f * currentDEX) / 2) * 1 + weaponStats.weaponAttackSpeed;
        maxHitpoints = (currentCON * 1.2f) + unitStats.baseHP;
        moveSpeed = (((1.2f * currentAGL) / 2) * 1 + unitStats.baseMovementSpeed);

        attackRange = weaponStats.weaponRange;
    }

    void GetTarget()
    {
        Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, perception, targetLayerMask).GetComponent<Collider2D>();
        if (targetCollider != null)
        {
            target = targetCollider.gameObject;
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }
    }

    public void damageTarget(GoonBehavior targetStats)
    {
        targetStats.TakeHit(damage);
    }

    void Attack()
    {
        weaponBehavior.Attack();
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }


    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1 / attackSpeed);
        canAttack = true;
    }

}
