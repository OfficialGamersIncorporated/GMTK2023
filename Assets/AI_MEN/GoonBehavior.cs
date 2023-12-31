using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public float attackSpeed;
    [SerializeField] float attackRange;
    [SerializeField] float maxHitpoints;
    [SerializeField] float hitpoints;
    [SerializeField] public float moveSpeed;
    [SerializeField] float knockback;

    //INDEPENDENT STATS
    [SerializeField] float perception; // Make a stat that influences range? Or just tweak until feels good

    //EQUPIMENT
    public GameObject weapon;
    public SpriteRenderer characterVisual;
    WeaponBehavior weaponBehavior;
    WeaponStats weaponStats;
    public GameObject armor;
    ArmorStats armorStats;

    //TARGETING
    [SerializeField] public GameObject target;
    [SerializeField] Vector2 targetVector2;
    public LayerMask targetLayerMask;
    float targetTimer;

    //UTILITY
    float distanceToTarget;
    bool canAttack = true;
    bool hasTarget = false;
    Rigidbody2D rb;
    public Transform projectileOrigin;
    [SerializeField] GameObject weaponHolder;

    //UI
    [SerializeField] HealthbarBehavior healthbar;

    //AUDIO
    AudioSource audioSource;
    [SerializeField] AudioClip[] footstepSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip strikeSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        healthbar.SetHealth(hitpoints, maxHitpoints);
    }

    void Update() {
        if(distanceToTarget > 1) {
            targetTimer += Time.deltaTime;
        }
        if(gameControl.waveRunning && !hasTarget) {
            ScanForTarget(perception);
        }
        if(hasTarget) {
            AimAtTarget();

            if(targetTimer >= 0.5f) {
                ScanForTarget(perception);
                targetTimer = 0;
            }

            if(!target.activeSelf) {
                ScanForTarget(perception);
            } else {
                targetVector2 = target.transform.position - transform.position;
            }

            MoveToRange();

            if(distanceToTarget < attackRange && canAttack) {
                rb.velocity = Vector2.zero;
                Attack();
            }
        } else {
            rb.velocity = Vector2.zero;
            // Return to origin
        }
    }

    void MoveToRange()
    {
        distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > attackRange)
        {

            int rand = Random.Range(0, footstepSound.Length);
            AudioClip currentFootstep = footstepSound[rand];
            audioSource.PlayOneShot(currentFootstep);
            rb.velocity = targetVector2.normalized * moveSpeed;
            //rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * targetVector2.normalized);
        }
    }

    void AimAtTarget() {
        Vector3 toEnemy = (target.transform.position - transform.position).normalized;
        if(characterVisual)
            characterVisual.flipX = toEnemy.x < 0;
        weaponHolder.transform.up = toEnemy;
    }

    public void TakeHit(float incomingDamage, Vector3 attacker, float knockbackParam)
    {
        hitpoints -= incomingDamage;
        healthbar.SetHealth(hitpoints, maxHitpoints);

        Vector2 targetHeadingAway = (transform.position - attacker).normalized;

        if (knockbackParam > 0)
        {
            rb.AddForce(targetHeadingAway * knockbackParam * 5, ForceMode2D.Impulse);
        }

        if (hitpoints <= 0)
        {
            //Death animation
            Debug.Log("IM DEAD FR FR");
            gameObject.SetActive(false);
            audioSource.PlayOneShot(deathSound);
            return;
        }
        audioSource.PlayOneShot(hurtSound);
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
        moveSpeed = (((1.2f * currentAGL) / 2) * 1 + unitStats.baseMovementSpeed) / 2;

        attackRange = weaponStats.weaponRange;

        knockback = currentSTR;
    }

    void ScanForTarget(float scanRange)
    {
        List<GameObject> targetsInRange = new();
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.root.position, scanRange, targetLayerMask.value);

        if (collidersInRange.Length > 0)
        {
            foreach (Collider2D collider in collidersInRange)
            {
                GameObject toGameObject = collider.gameObject;
                targetsInRange.Add(toGameObject);
            }
            target = GetClosestEnemy(targetsInRange);
            hasTarget = true;
            targetVector2 = target.transform.position;
        }
        if (collidersInRange.Length == 0)
        {
            hasTarget = false;
        }

    }

    GameObject GetClosestEnemy(List<GameObject> targetsInRange)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in targetsInRange)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    public void damageTarget(GoonBehavior targetStats)
    {
        audioSource.PlayOneShot(strikeSound);
        targetStats.TakeHit(damage, transform.position, knockback);
    }

    void Attack()
    {
        audioSource.PlayOneShot(attackSound);
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
