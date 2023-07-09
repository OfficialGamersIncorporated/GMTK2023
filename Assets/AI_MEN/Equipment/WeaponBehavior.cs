using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public bool isAttacking = false;

    public WeaponStats weaponStats;

    [SerializeField] GoonBehavior ownerBehavior;

    private void Start()
    {
        ownerBehavior = transform.root.GetComponent<GoonBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((ownerBehavior.targetLayerMask.value & 1 << collider.gameObject.layer) > 0)
        {
            GoonBehavior enemy = collider.GetComponent<GoonBehavior>();
            if (enemy)
            {
                //Debug.Log("HIT GOON: " + goon.name);
                ownerBehavior.damageTarget(enemy);
            }
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
