using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

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
            Debug.Log("HIT TARGET LAYER");
            GoonBehavior goon = collider.GetComponent<GoonBehavior>();
            if (goon)
            {
                Debug.Log("HIT GOON: " + goon.name);
                ownerBehavior.damageTarget(goon);
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
