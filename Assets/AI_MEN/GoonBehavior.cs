using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonBehavior : MonoBehaviour
{

    [SerializeField] GameControl gameControl;

    //BASE STATS
    [SerializeField] Stats stats;
    [SerializeField] float baseHP;
    [SerializeField] float baseMovementSpeed;

    //GAMEPLAY STATS
    [SerializeField] float damage;
    [SerializeField] float attackSpeed;
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
    public LayerMask targetLayer;

    //UTILITY
    [SerializeField] GameObject weaponHolder;
    float distanceToTarget;
    bool canAttack = true;

    // WEAPON BASED STATS
    float attackRange;


    private void OnEnable()
    {
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();

        weaponStats = gameObject.GetComponentInChildren<WeaponStats>();
        armorStats = gameObject.GetComponentInChildren<ArmorStats>();

        ApplyStats();
        hitpoints = maxHitpoints; // NEED TO ADD BONUS HP FROM ARMOR EQUIP
        attackRange = weaponStats.range;

        weaponAnimator = weapon.GetComponent<Animator>();
        weaponAnimator.SetFloat("attackSpeed", attackSpeed);

    }

    void Update()
    {
        if (gameControl.waveRunning && target == null)
        {
            target = Physics2D.CircleCast(transform.position, perception, Vector2.zero, perception, targetLayer).collider.gameObject;
        }

        if (target != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            MoveToRange(attackRange, moveSpeed);
            weaponHolder.transform.up = target.transform.position - transform.position;
        }

        if (distanceToTarget < attackRange && canAttack)
        {
            weaponAnimator.SetTrigger("Attack");
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }
    }

    void MoveToRange(float attackRangeParam, float moveSpeedParam)
    {
        if (distanceToTarget > attackRangeParam)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * moveSpeedParam);
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
        Debug.Log("STR: " + stats.STR);
        Debug.Log("DEX: " + stats.DEX);
        Debug.Log("CON: " + stats.CON);
        Debug.Log("AGL: " + stats.AGL);
        stats.STR += armorStats.bonusSTR;
        stats.DEX += armorStats.bonusDEX;
        stats.CON += armorStats.bonusCON;
        stats.AGL += armorStats.bonusAGL;
        Debug.Log("STR: " + stats.STR);
        Debug.Log("DEX: " + stats.DEX);
        Debug.Log("CON: " + stats.CON);
        Debug.Log("AGL: " + stats.AGL);

        damage = stats.STR + weaponStats.weaponDamage;
        attackSpeed = ((1.2f * stats.DEX) / 2) * 1 + weaponStats.weaponAttackSpeed;
        maxHitpoints = (stats.CON * 1.2f) + baseHP;
        moveSpeed = ((1.2f * stats.AGL) / 2) * 1 + baseMovementSpeed;
    }

    public void targetHit(GoonBehavior targetStats)
    {
        targetStats.TakeHit(damage);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

}
