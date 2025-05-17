using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleItem : ItemBase
{
    /// <summary>�v���C���[�������܂�</summary>
    protected override void Activate()
    {
        Destroy(Player);
    }
}
