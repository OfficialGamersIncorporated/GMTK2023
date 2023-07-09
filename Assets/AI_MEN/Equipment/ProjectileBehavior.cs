using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    float damage;
    LayerMask layerMask;

    void OnCollisionEnter2D(Collision2D collider)
    {
        GetComponent<Collider2D>().enabled = false;
        gameObject.transform.parent = collider.gameObject.transform;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        if ((layerMask.value & 1 << collider.gameObject.layer) > 0)
        {
            GoonBehavior enemy = collider.gameObject.GetComponent<GoonBehavior>();
            if (enemy)
            {
                enemy.TakeHit(damage, Vector3.zero, 0);
            }
            
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    public void SetStats(int DEXParam, LayerMask layerMaskParam)
    {
        damage = DEXParam + 1;
        layerMask = layerMaskParam;
    }
}
