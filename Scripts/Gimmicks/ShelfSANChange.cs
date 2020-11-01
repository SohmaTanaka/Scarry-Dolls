using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シェルフに出てくる人形での狂気度変更用のクラス
///
/// 担当者：田中颯馬
/// </summary>
public class ShelfSANChange : MonoBehaviour
{
    [Tooltip("SANマネージャー")] public SanValueManager sanManager;
    
    /// <summary>
    /// 狂気度の変更
    /// </summary>
    public void SANChange()
    {
        sanManager.ChangeSAN(this.name);
    }
}
