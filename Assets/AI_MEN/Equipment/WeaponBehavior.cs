using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public bool isRanged;

    public bool isAttacking = false;

    public WeaponStats weaponStats;

    Transform projectileOrigin;

    Animator anim;
    Animator charAnim;

    [SerializeField] GameObject projectile;
    [SerializeField] float launchVelocity = 100;
    GoonBehavior ownerBehavior;

    private void Start()
    {
        ownerBehavior = GetComponentInParent<GoonBehavior>(); //transform.root.GetComponent<GoonBehavior>(); // using root means the units can't be placed in an empty gameobject.

        anim = gameObject.GetComponent<Animator>();
        anim.SetFloat("attackSpeed", ownerBehavior.attackSpeed);
        if(ownerBehavior.characterVisual) {
            charAnim = ownerBehavior.characterVisual.GetComponent<Animator>();
            charAnim.SetFloat("attackSpeed", ownerBehavior.attackSpeed);
        }

        projectileOrigin = ownerBehavior.projectileOrigin;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((ownerBehavior.targetLayerMask.value & 1 << collider.gameObject.layer) > 0 && isAttacking)
        {
            GoonBehavior enemy = collider.GetComponent<GoonBehavior>();
            if (enemy)
            {
                //Debug.Log("HIT GOON: " + goon.name);
                ownerBehavior.damageTarget(enemy);
            }
        }
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        if (charAnim)
            charAnim.SetTrigger("Attack");
        if (isRanged)
        {
            GameObject newProjectile = Instantiate(projectile, projectileOrigin.position, transform.rotation * Quaternion.Euler(0, 0, -90));
            Destroy(newProjectile, 10f);
            newProjectile.GetComponent<ProjectileBehavior>().SetStats(ownerBehavior.currentDEX, ownerBehavior.targetLayerMask);
            newProjectile.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * launchVelocity);
        }

    }

    void StartedAttack()
    {
        isAttacking = true;
    }

    void FinishedAttack()
    {
        isAttacking = false;
    }
}
