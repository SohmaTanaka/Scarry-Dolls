using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引き出し移動用のクラス
///
/// 担当者：田中颯馬
/// </summary>
public class Drawers : MonoBehaviour, IGimmickActionable
{
    //Z値の初期値
    private float ZStore { get; set; }
    //Z値のターゲット
    private float ZTarget { get; set; }
    //開いてるか
    private bool IsOpen { get; set; }
    //コルーチンが動いているか
    private bool NowCoroutine { get; set; }

    [Tooltip("引き出しの開ける音")] public AudioSource open;
    [Tooltip("引き出しの閉じる音")] public AudioSource close;

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        ZStore = -0.09949294f;
        ZTarget = 0.19f;

        IsOpen = false;
        NowCoroutine = false;
    }

    public void Action()
    {
        //コルーチンが回ってなかったら
        if (!NowCoroutine)
        {
            //開いていないとき
            if (!IsOpen)
            {
                //音声再生
                open.Play();
                //コルーチン
                StartCoroutine(OpenCoroutine());
            }
            else
            {
                //音声再生
                close.Play();
                //コルーチン
                StartCoroutine(CloseCoroutine());
            }
        }
    }

    //開ける用のコルーチン
    private IEnumerator OpenCoroutine()
    {
        NowCoroutine = true;

        float leapTime = 0.0f;

        while (leapTime < 1.0f)
        {
            leapTime += Time.deltaTime;

            transform.localPosition =
                Vector3.Lerp(new Vector3(transform.localPosition.x, transform.localPosition.y, ZStore),
                    new Vector3(transform.localPosition.x, transform.localPosition.y, ZTarget), leapTime);
            
            yield return null;
        }

        NowCoroutine = false;
        IsOpen = true;
    }

    //閉める用のコルーチン
    private IEnumerator CloseCoroutine()
    {
        NowCoroutine = true;

        float leapTime = 0.0f;

        while (leapTime < 1.0f)
        {
            leapTime += Time.deltaTime;

            transform.localPosition =
                Vector3.Lerp(new Vector3(transform.localPosition.x, transform.localPosition.y, ZTarget),
                    new Vector3(transform.localPosition.x, transform.localPosition.y, ZStore), leapTime);

            yield return null;
        }

        NowCoroutine = false;
        IsOpen = false;
    }
}