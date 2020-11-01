using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// シェルフに出てくる人形用のクラス
///
/// 担当者：田中颯馬
/// </summary>
public class ShelfDoll : MonoBehaviour, IGimmickActionable
{
    [Tooltip("自身の人形を取得")] public GameObject myDoll;

    [Tooltip("始まりの位置")] public GameObject start;

    [Tooltip("終わりの位置")] public GameObject end;

    [Tooltip("人形のマテリアル")] public Material dollMaterial;

    [Tooltip("人形の出現時間")] public float leapTimeSetting = 1.0f;

    [Tooltip("狂気度設定用")] public ShelfSANChange sanChange;

    [Tooltip("SE")] public List<AudioSource> audios;

    //補間時間
    private float leapTime;
    //発動したことがあるか
    private bool first;

    // Start is called before the first frame update
    void Start()
    {
        leapTime = 0.0f;
        myDoll.SetActive(false);
        first = true;
    }

    public void Action()
    {
        //最初に通過してたなら
        if (first)
        {
            //オーディオを全て再生
            foreach (var item in audios)
            {
                item.Play();
            }

            //狂気度の変更
            sanChange.SANChange();
            myDoll.SetActive(true);
            StartCoroutine(LeapCoroutine());
        }
    }

    //人形の移動の補間処理
    private IEnumerator LeapCoroutine()
    {
        while (leapTime < leapTimeSetting)
        {
            leapTime += Time.deltaTime;

            myDoll.transform.position =
                Vector3.Lerp(start.transform.position, end.transform.position, leapTime / leapTimeSetting);

            dollMaterial.color = new Color(dollMaterial.color.r, dollMaterial.color.g, dollMaterial.color.b,
                1 - (leapTime / leapTimeSetting));

            yield return null;
        }

        leapTime = 0.0f;
        myDoll.SetActive(false);
        first = false;
    }
}