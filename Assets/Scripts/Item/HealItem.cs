using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : ItemBase
{
    [SerializeField] int _recoverLife;
    public override void Activate()
    {
        Player.GetComponent<PlayerController>().FluctuationLife(_recoverLife);
    }
}
