using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleItem : ItemBase
{
    /// <summary>ƒvƒŒƒCƒ„[‚ğÁ‚µ‚Ü‚·</summary>
    public override void Activate()
    {
        Destroy(Player);
    }
}
