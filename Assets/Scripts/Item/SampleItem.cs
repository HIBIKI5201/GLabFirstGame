using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleItem : ItemBase
{
    /// <summary>�v���C���[�������܂�</summary>
    public override void Activate()
    {
        Destroy(Player);
    }
}
