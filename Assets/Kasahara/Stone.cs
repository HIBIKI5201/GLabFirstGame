using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : ItemBase
{
    public override void Activate()
    {
        transform.position = Player.transform.position;
    }
}
