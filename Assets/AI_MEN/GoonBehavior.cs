using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonBehavior : MonoBehaviour
{
    //STATS

    [SerializeField] float damage;
    [SerializeField] float attackSpeed;
    [SerializeField] float hitpoints;
    [SerializeField] float moveSpeed;

    GameObject target;
    LayerMask enemy;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject GetTarget(float range)
    {
        Physics.OverlapSphere(transform.position, range, enemy);


        return target;
    }

}
