using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using XInputAssistManager;
using UniRx;

/// <summary>
/// 視点用カメラの動き
/// 
/// 担当者：田中颯馬
/// 作成日時:2019/5/9
/// </summary>
public class PointOfView : MonoBehaviour
{
    [Tooltip("基準オブジェクト")] public Transform cameraParentTrans;
    [Tooltip("基準オブジェクトから見たカメラの相対位置")] public Vector3 cameraLocalPosition = new Vector3(0, 2.0f, 0);
    [Tooltip("インタラクトUI")] public InteractUI interactUI;
    [Tooltip("投げ物のスポーンポイント")] public GameObject spawn;

    //プレイヤーの状態
    private EPlayerMoveState EPlayerMoveState { get; set; }

    //プレイヤーの動きを司るクラス
    private PlayerMove PlayerMove { get; set; }

    //プレイヤーの行動を司るクラス
    private PlayerAction PlayerAction { get; set; }

    //入力管理者
    private InputManager inputManager;

    [Tooltip("マウスのX軸感度"), Range(0.01f, 2.0f)]
    public float mouseSensitivityX = 1.0f;

    [Tooltip("マウスのY軸感度"), Range(0.01f, 2.0f)]
    public float mouseSensitivityY = 1.0f;

    [Tooltip("ゲームパッドのX軸感度"), Range(0.01f, 2.0f)]
    public float gamePadSensitivityX = 1.0f;

    [Tooltip("ゲームパッドのY軸感度"), Range(0.01f, 2.0f)]
    public float gamePadSensitivityY = 1.0f;

    [Tooltip("補間処理の時間")] public float leapTimeSetting = 0.5f;

    public GameObject Evildoll;

    //補間してる経過時間
    private float leapTime;

    //投げ物のスポーン地点が補間移動してる経過時間
    private float leapSpawnTransTime;

    //チュートリアルのUIマネージャー
    public TutorialUIMnager tutorialManager;

    //レイヤーマスク 0でどれにも当たらない。1<<△で△のレイヤーのみに当たる
    //string指定でも問題はない
    private readonly LayerMask camLayerMask = 1 << 8 | 1 << 13 | 1 << 0;

    private void Awake()
    {
        //カーソルの不可視化・自由移動の禁止
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //回転の初期化
        transform.eulerAngles = Vector3.zero;

        //カメラの位置の変更
        transform.position = cameraParentTrans.position + cameraParentTrans.right * cameraLocalPosition.x +
                             cameraParentTrans.up * cameraLocalPosition.y +
                             cameraParentTrans.forward * cameraLocalPosition.z;

        //取得
        PlayerMove = cameraParentTrans.gameObject.GetComponent<PlayerMove>();
        PlayerAction = cameraParentTrans.gameObject.GetComponent<PlayerAction>();
        EPlayerMoveState = PlayerMove.EPlayerMoveStateGetSet;
        inputManager = InputManager.GetInstance;

        //初期化
        leapTime = 0.0f;
        leapSpawnTransTime = 0.0f;
    }

    /// <summary>
    /// 補間処理を非同期で行うコルーチン
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private IEnumerator LeapCoroutine(float y)
    {
        //時間の経過
        leapTime += Time.deltaTime;

        //補間
        cameraLocalPosition = Vector3.Lerp(cameraLocalPosition,
            new Vector3(cameraLocalPosition.x, y, cameraLocalPosition.z), leapTime / leapTimeSetting);

        //OnCompleteが呼ばれる処理でないと無限ループする
        yield return Observable.Timer(TimeSpan.FromSeconds(leapTimeSetting)).ToYieldInstruction();

        //リセット
        leapTime = 0.0f;
    }

    /// <summary>
    /// スポーン地点の補間処理を非同期で行うコルーチン
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private IEnumerator LeapSpawnTransFormCoroutine(float y)
    {
        //時間の経過
        leapSpawnTransTime += Time.deltaTime;

        //補間
        spawn.transform.localPosition = Vector3.Lerp(spawn.transform.localPosition,
            new Vector3(spawn.transform.localPosition.x, y, spawn.transform.localPosition.z),
            leapSpawnTransTime / leapTimeSetting);


        //OnCompleteが呼ばれる処理でないと無限ループする
        yield return Observable.Timer(TimeSpan.FromSeconds(leapTimeSetting)).ToYieldInstruction();

        //リセット
        leapSpawnTransTime = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        //カメラの位置の変更
        transform.position = cameraParentTrans.position + cameraParentTrans.right * cameraLocalPosition.x +
                             cameraParentTrans.up * cameraLocalPosition.y +
                             cameraParentTrans.forward * cameraLocalPosition.z;

        //入力
        float rotationX = 0;
        float rotationY = 0;

        //感度
        float sensitivityX;
        float sensitivityY;

        //感度と入力の条件分岐
        if (inputManager.IsUseGamePad)
        {
            rotationX = inputManager.GetAxisRightHorizontal;
            rotationY = -inputManager.GetAxisRightVertical;
            sensitivityX = gamePadSensitivityX;
            sensitivityY = gamePadSensitivityY;
        }
        else
        {
            rotationX = Input.GetAxis("Mouse X");
            rotationY = Input.GetAxis("Mouse Y");
            sensitivityX = mouseSensitivityX;
            sensitivityY = mouseSensitivityY;
        }

        //角度調整
        if ((transform.localEulerAngles.x > 50) && (transform.localEulerAngles.x <= 180))
        {
            transform.localEulerAngles = new Vector3(50, transform.localEulerAngles.y, 0);
        }
        else if ((transform.localEulerAngles.x < 350) && (transform.localEulerAngles.x > 180))
        {
            transform.localEulerAngles = new Vector3(350, transform.localEulerAngles.y, 0);
        }

        //基準オブジェクトと回転を同期
        cameraParentTrans.Rotate(Vector3.up, rotationX * sensitivityX);
        transform.localEulerAngles += new Vector3(-rotationY * sensitivityY, rotationX * sensitivityX, 0.0f);
        cameraParentTrans.localEulerAngles = new Vector3(cameraParentTrans.localEulerAngles.x,
            cameraParentTrans.localEulerAngles.y, 0.0f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraParentTrans.localEulerAngles.y, 0.0f);


        //状態によって処理の分岐
        switch (PlayerMove.EPlayerMoveStateGetSet)
        {
            //停止時
            case EPlayerMoveState.STOP:
                StartCoroutine(LeapCoroutine(1.5f));
                StartCoroutine(LeapSpawnTransFormCoroutine(1.3f));
                break;
            //歩行時
            case EPlayerMoveState.WALK:
                StartCoroutine(LeapCoroutine(1.5f));
                StartCoroutine(LeapSpawnTransFormCoroutine(1.3f));
                break;
            //走行時
            case EPlayerMoveState.SPRINT:
                StartCoroutine(LeapCoroutine(1.5f));
                StartCoroutine(LeapSpawnTransFormCoroutine(1.3f));
                break;
            //しゃがみ時
            case EPlayerMoveState.CROUCH:
                StartCoroutine(LeapCoroutine(1.0f));
                StartCoroutine(LeapSpawnTransFormCoroutine(0.8f));
                break;
        }

        //視点からRayを飛ばす
        Ray camRay = new Ray(transform.position, transform.forward);

        //Rayの当たり判定
        if (Physics.Raycast(camRay, out RaycastHit hit, 2.0f, camLayerMask))
        {
            #region チュートリアル用の判定

            if (hit.transform.CompareTag("DollDoor") && hit.transform.gameObject.layer == 13)
            {
                hit.transform.GetComponent<HitDoor>().setBool();
            }

            if (hit.transform.CompareTag("Medicine") && hit.transform.gameObject.layer == 8)
            {
                tutorialManager.ActiveText(2);
            }

            if (hit.transform.CompareTag("Oil") && hit.transform.gameObject.layer == 8)
            {
                tutorialManager.ActiveText(1);
            }

            #endregion

            //壺の判定
            if (hit.transform.gameObject.layer == 8)
            {
                //左手に持てるアイテムかどうかの判定
                Hold(hit);
            }
            else
            {
                //UIの消去
                interactUI.ChangeBoolUI(false);
            }
        }
        else
        {
            //UIの消去
            interactUI.ChangeBoolUI(false);
        }

        //カメラの位置の変更
        transform.position = cameraParentTrans.position + cameraParentTrans.right * cameraLocalPosition.x +
                             cameraParentTrans.up * cameraLocalPosition.y +
                             cameraParentTrans.forward * cameraLocalPosition.z;
    }

    //左手に持てるアイテムかどうか
    private void Hold(RaycastHit hit)
    {
        if (hit.transform.CompareTag("CanHold"))
        {
            if (Evildoll.activeSelf && Evildoll != null)
            {
                tutorialManager.ActiveText(4);
                if (!PlayerAction.Hold)
                {
                    interactUI.ChangeBoolUI(true);
                    PlayerAction.ThrowLookingObject(hit.transform.gameObject);
                    PlayerAction.LookingObject(
                        hit.transform.GetComponent(typeof(IGimmickActionable)) as IGimmickActionable);
                }
            }
        }
        else
        {
            interactUI.ChangeBoolUI(true);
            //インターフェースを取得する場合
            PlayerAction.LookingObject(
                hit.transform.GetComponent(typeof(IGimmickActionable)) as IGimmickActionable);
        }
    }
}