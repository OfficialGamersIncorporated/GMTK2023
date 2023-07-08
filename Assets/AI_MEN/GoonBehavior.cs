using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoonBehavior : MonoBehaviour
{

    [SerializeField] GameControl gameControl;

    //BASE STATS
    [SerializeField] UnitStats unitStats;

    //GAMEPLAY STATS
    [SerializeField] int currentSTR;
    [SerializeField] int currentDEX;
    [SerializeField] int currentCON;
    [SerializeField] int currentAGL;
    [SerializeField] float damage;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackRange;
    [SerializeField] float maxHitpoints;
    [SerializeField] float hitpoints;
    [SerializeField] float moveSpeed;


    [SerializeField] float perception; // Make a stat that influences range? Or just tweak until feels good

    //EQUPIMENT
    public GameObject weapon;
    WeaponStats weaponStats;
    Animator weaponAnimator;
    public GameObject armor;
    ArmorStats armorStats;

    //TARGETING
    [SerializeField] GameObject target;
    public LayerMask targetLayerMask;

    //UTILITY
    [SerializeField] GameObject weaponHolder;
    [SerializeField] float distanceToTarget;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool hasTarget = false;


    private void OnEnable()
    {
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();

        weaponStats = weapon.GetComponent<WeaponBehavior>().weaponStats;
        armorStats = armor.GetComponent<ArmorBehavior>().armorStats;

        ApplyStats();
        hitpoints = maxHitpoints;
        

        weaponAnimator = weapon.GetComponent<Animator>();
        weaponAnimator.SetFloat("attackSpeed", attackSpeed);

    }

    void Update()
    {
        if (gameControl.waveRunning && !hasTarget)
        {
            GetTarget();
        }

        if (hasTarget)
        {
            if (!target.activeSelf)
            {
                GetTarget();
            }

            MoveToRange();
            if (distanceToTarget < attackRange && canAttack)
            {
                weaponAnimator.SetTrigger("Attack");
                canAttack = false;
                StartCoroutine(AttackCooldown());
            }
        }
    }

    void MoveToRange()
    {
        distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        weaponHolder.transform.up = target.transform.position - transform.position;
        if (distanceToTarget > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * moveSpeed);
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
        moveSpeed = (((1.2f * currentAGL) / 2) * 1 + unitStats.baseMovementSpeed) / 4;

        attackRange = weaponStats.weaponRange;
    }

    void GetTarget()
    {
        Collider2D targetCollider = Physics2D.CircleCast(transform.position, perception, Vector2.zero, perception, targetLayerMask).collider;
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

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1 / attackSpeed);
        canAttack = true;
        Debug.Log("ATTACK COOLDOWN COMPLETE");
    }

}
