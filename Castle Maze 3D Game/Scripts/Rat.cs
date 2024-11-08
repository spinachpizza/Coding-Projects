using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Enemy
{
    protected override void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-direction);
        }
    }
}
