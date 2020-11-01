using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputAssistManager;

/// <summary>
/// プレイヤーの行動に関する関数用のクラス
///
/// 担当者：田中颯馬
/// </summary>
public class PlayerAction : MonoBehaviour
{
    //インプット補助
    InputManager inputManager;

    [Tooltip("PlayerMove")] public PlayerMove playermove;
    [Tooltip("投げ物のプレハブのリスト")] public GameObject throwPrefab;
    [Tooltip("投げ物のスポーンポイント")] public GameObject spawn;
    [Tooltip("カメラの向きを取るためのトランスフォーム")] public Transform cam;
    [Tooltip("DoolAI")] public EvilDollAI evilDollAi;
    [Tooltip("SAN値取得用マネージャー")] public SanValueManager sanManager;
    [Tooltip("狂気度用オーディオのリスト")] public List<AudioSource> sanAudios;

    //投げ物
    private GameObject throwObject;
    //持ってるかどうか
    public bool Hold { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        inputManager = InputManager.GetInstance;
    }

    // Update is called once per frame
    void Update()
    {
        //イベント中じゃなくて投げ物もってたら
        if (Hold && !playermove.IsEvent)
        {
            if (inputManager.GetKeyDown(EInputCode.THROW))
            {
                //撃つ
                Shot();
            }
        }
    }

    /// <summary>
    /// 狂気度に応じるのSEの選定
    /// </summary>
    private void SANAudioSelecter()
    {
        if (sanManager.SAN < 40.0f)
        {
            foreach (var item in sanAudios)
            {
                item.Stop();
            }
        }
        else if (sanManager.SAN >= 40.0f && sanManager.SAN < 60.0f)
        {
            if (!sanAudios[0].isPlaying)
            {
                sanAudios[0].Play();
            }
        }
        else if (sanManager.SAN >= 60.0f && sanManager.SAN < 80.0f)
        {
            sanAudios[1].Play();
            sanAudios[0].Stop();
            evilDollAi.setColling(transform);
        }
    }

    //SEの再生
    public void Breath()
    {
        SANAudioSelecter();
    }

    //投げ物を打ち出す
    private void Shot()
    {
        //投げ物のオブジェクトをローカル変数に保存
        GameObject shot = throwObject;
        //タグを変更
        shot.tag = "Throw";
        //Rigidbodyの装備
        Rigidbody shootRigit = shot.AddComponent<Rigidbody>();

        //子供にRigidbodyを装備
        for (int i = 0; i < shot.transform.childCount; i++)
        {
            Rigidbody childRigid = shot.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            childRigid.useGravity = true;
            childRigid.isKinematic = true;
        }

        //カプセルコライダーの取得・有効化
        CapsuleCollider[] capsuleColliders = shot.GetComponents<CapsuleCollider>();
        foreach (var item in capsuleColliders)
        {
            item.enabled = true;
        }

        //Rigidbodyへ処理を加える
        shootRigit.useGravity = true;
        shootRigit.isKinematic = false;
        shootRigit.mass = 0.5f;
        shootRigit.AddForce((cam.forward + spawn.transform.forward).normalized * 400);
        
        //親オブジェクトから外す
        shot.transform.parent = null;
        //手放す
        Hold = false;
    }

    /// <summary>
    /// ※PointOfView.csのRayHitで呼び出す
    /// インタラクトが押された時、オブジェクトのインターフェースのメソッドを呼ぶ
    /// </summary>
    public void LookingObject(IGimmickActionable gimmickActionable)
    {
        //インタラクトを押したとき
        if (inputManager.GetKeyDown(EInputCode.INTERACT))
        {
            gimmickActionable.Action();
        }
    }

    /// <summary>
    /// ※PointOfView.csのRayHitで呼び出す
    /// インタラクトが押された時、投げ物を呼び出す
    /// </summary>
    public void ThrowLookingObject(GameObject hitGameObject)
    {
        //インタラクトを押したとき
        if (inputManager.GetKeyDown(EInputCode.INTERACT))
        {
            throwObject = Instantiate(throwPrefab, spawn.transform.position, spawn.transform.rotation,
                spawn.transform);
            Hold = true;
        }
    }

    /// <summary>
    /// プレイヤーがぶつかったら呼び出す
    /// </summary>
    /// <param name="gimmickActionable"></param>
    public void HitObject(IGimmickActionable gimmickActionable)
    {
        gimmickActionable.Action();
    }
}