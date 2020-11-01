using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputAssistManager;

/// <summary>
/// 動きに関する関数用のクラス
///
/// 担当者：田中颯馬
/// 作成日時:2019/5/9
/// </summary>
public class Move
{
    //インスタンス変数
    private static Move instance;
    //プレイヤーの初期パラメータ取得用
    private PlayerParameter playerParameter;

    //速度
    //ScriptableObjectからの取得に変更
    private float speed;
    private float sprintSpeedRate;
    private float crouchSpeedRate;

    /// <summary>
    /// インスタンス生成
    /// </summary>
    public static Move Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Move();
            }

            return instance;
        }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Move()
    {
        //スクリプタブルオブジェクトの読み込み
        playerParameter = Resources.Load<PlayerParameter>("Data/PlayerInitialize/PlayerParameter");
        //速さの取得
        speed = playerParameter.speed;
        sprintSpeedRate = playerParameter.sprintSpeedRate;
        crouchSpeedRate = playerParameter.crouchSpeedRate;
    }

    /// <summary>
    /// 動きの計算をする
    /// </summary>
    /// <param name="state">動きの状態</param>
    /// <param name="direction">動きの方向</param>
    /// <param name="moveVector">出力用変数</param>
    public void MoveCalc(EPlayerMoveState state, Vector3 direction, Transform charaTransform, out Vector3 moveVector)
    {
        //ベクトル保存用変数
        Vector3 storeVector = charaTransform.position;

        //状態によって処理を変更
        switch (state)
        {
            //歩いてるとき
            case EPlayerMoveState.WALK:
                storeVector = direction * speed;
                break;
            //走ってるとき
            case EPlayerMoveState.SPRINT:
                storeVector = direction * speed * sprintSpeedRate;
                break;
            //しゃがんでるとき
            case EPlayerMoveState.CROUCH:
                storeVector = direction * speed * crouchSpeedRate;
                break;
            //停止時
            case EPlayerMoveState.STOP:
                storeVector = Vector3.zero;
                break;
        }

        //出力用変数に代入
        moveVector = storeVector;
    }
}