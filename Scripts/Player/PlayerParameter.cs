using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリプタブルオブジェクト作成用のクラス
///
/// 担当者：田中颯馬
/// </summary>
[CreateAssetMenu(fileName = "PlayerParameter", menuName = "ScriptableObjects/PlayerParameter", order = 1)]
public class PlayerParameter : ScriptableObject
{
    //初期狂気度
    public int initializeSANValue = 0;
    //最大値
    public int maxSANValue = 100;
    //最小値
    public int minSANValue = 0;

    //共通スピード
    public float speed = 2.0f;
    //スプリント時のスピード倍率
    public float sprintSpeedRate = 1.3f;
    //しゃがみ時のスピード倍率
    public float crouchSpeedRate = 0.8f;
}