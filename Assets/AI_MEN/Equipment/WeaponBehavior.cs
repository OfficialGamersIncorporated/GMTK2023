using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public bool isAttacking = false;

    [SerializeField] WeaponStats weaponStats;

    GoonBehavior parentBehavior;

    private void Start()
    {
        parentBehavior = transform.root.GetComponent<GoonBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("COLLISION DETECTED WITH: " + collider.name);
        if (collider.gameObject.layer == parentBehavior.targetLayer)
        {
            GoonBehavior goon = collider.GetComponent<GoonBehavior>();
            if (goon)
            {
                Debug.Log("HIT GOON: " + goon.name);
                parentBehavior.targetHit(goon);
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
