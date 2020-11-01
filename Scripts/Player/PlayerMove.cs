using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputAssistManager;

/// <summary>
/// プレイヤーの移動関連の動きを司る
/// 
/// 担当者：田中颯馬
/// 作成日時:2019/5/9
/// </summary>
public class PlayerMove : MonoBehaviour
{
    //インプット補助
    InputManager inputManager;

    //移動量
    Vector3 movementVector;

    //プレイヤーのRigidbody
    Rigidbody playerRigid;

    [Tooltip("立ち時のコライダー")] public CapsuleCollider capsuleCollider;
    [Tooltip("しゃがみ時のコライダー")] public SphereCollider sphereCollider;
    [Tooltip("ドアのドールが生きてるとき止める用")] public GameObject instantDollObj;

    //プレイヤーの行動
    public PlayerAction PlayerAction { get; set; }

    //歩行時と走行時のオーディオ
    public AudioSource walk;
    public AudioSource sprint;

    #region トグル・ホールド切り替え用変数

    //設定から受け取る（あとでやる）
    //走りは切り替え式か
    bool isSprintToggle;

    //しゃがみは切り替え式か
    bool isCrouthToggle;

    //匍匐は切り替え式か
    bool isProneToggle;

    #endregion

    /// <summary>
    /// MoveStateの取得用
    /// </summary>
    public EPlayerMoveState EPlayerMoveStateGetSet { get; private set; }

    /// <summary>
    /// 死んでいるかどうか
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    /// イベント中かどうか
    /// </summary>
    public bool IsEvent { get; set; }

    /// <summary>
    /// 走行出来るかどうか
    /// </summary>
    private bool CanSprint { get; set; }

    /// <summary>
    /// しゃがみを強制されているかどうか
    /// </summary>
    private bool CrouchForce { get; set; }

    /// <summary>
    /// しゃがんでいるかどうか
    /// </summary>
    private bool IsCrouth { get; set; }

    //人形が出現してるかどうかを判定する用
    private InstantDoll instantDoll;

    /// <summary>
    /// direction取得用
    /// </summary>
    public Vector3 Direction { get; private set; }

    // Use this for initialization
    void Awake()
    {
        //フェードインの開始
        FadeManager.FadeIn();
        //プレイヤーの行動用
        PlayerAction = GetComponent<PlayerAction>();
        //入力
        inputManager = InputManager.GetInstance;
        //初期化
        movementVector = Vector3.zero;
        CanSprint = false;
        IsDead = false;
        IsEvent = false;
        CrouchForce = false;
        IsCrouth = false;
        playerRigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        instantDoll = instantDollObj.GetComponent<InstantDoll>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //死んでいないなら
        if (!IsDead)
        {
            if (other.transform.CompareTag("EvilDoll"))
            {
                //死ぬかどうかの判定
                EvilDollDeath(other);
            }
        }
    }

    void EvilDollDeath(Collision other)
    {
        DollMaster doll = other.transform.GetComponent<DollMaster>();

        //人形のステータスによって死ぬかどうかが決まる
        if (doll.getState() != DollMaster.DollState.movie
            && doll.getState() != DollMaster.DollState.sit && doll.getState() != DollMaster.DollState.stan)
        {
            IsDead = true;
            if (doll.getMode() == DollMaster.Mode.LastRoom)
            {
                other.transform.GetComponent<PrototypeChase>().setAttack();
            }
            else if (doll.getMode() == DollMaster.Mode.MainEvilDollAI)
            {
                other.transform.GetComponent<EvilDollAI>().setAttack();
            }

            FadeManager.FadeOut(6);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //死んでいないなら
        if (!IsDead)
        {
            if (other.CompareTag("ForceCrouch"))
            {
                //しゃがみを強制する
                CrouchForce = true;
            }

            if (other.CompareTag("CrouchDoll"))
            {
                //しゃがんでるときに人形が出てくるギミックの判定に入ったら呼び出し
                PlayerAction.HitObject(other.transform.GetComponent(typeof(IGimmickActionable)) as IGimmickActionable);
            }

            //ゴールにぶつかったら
            if (other.transform.CompareTag("GOAL"))
            {
                IsDead = true;
                FadeManager.FadeOut(5);
            }
        }
    }

    //判定から出たら
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForceCrouch"))
        {
            CrouchForce = false;
        }
    }

    /// <summary>
    /// イベント時じゃないとき。
    /// Update内に入れる
    /// </summary>
    void NotEvent()
    {
        //イベント中じゃないなら
        if (!IsEvent)
        {
            //状態による当たり判定の変更
            switch (EPlayerMoveStateGetSet)
            {
                case EPlayerMoveState.STOP:
                    capsuleCollider.enabled = true;
                    sphereCollider.enabled = false;
                    break;
                case EPlayerMoveState.WALK:
                    capsuleCollider.enabled = true;
                    sphereCollider.enabled = false;
                    break;
                case EPlayerMoveState.SPRINT:
                    capsuleCollider.enabled = true;
                    sphereCollider.enabled = false;
                    break;
                case EPlayerMoveState.CROUCH:
                    capsuleCollider.enabled = false;
                    sphereCollider.enabled = true;
                    break;
            }

            //移動の計算
            Movement();

            //SEの変更
            if (!walk.isPlaying && EPlayerMoveStateGetSet == EPlayerMoveState.WALK)
            {
                walk.Play();
            }
            else if (EPlayerMoveStateGetSet != EPlayerMoveState.WALK)
            {
                walk.Stop();
            }

            if (!sprint.isPlaying && EPlayerMoveStateGetSet == EPlayerMoveState.SPRINT)
            {
                sprint.Play();
            }
            else if (EPlayerMoveStateGetSet != EPlayerMoveState.SPRINT)
            {
                sprint.Stop();
            }
        }
        else
        {
            //止まってる時
            EPlayerMoveStateGetSet = EPlayerMoveState.STOP;
            playerRigid.velocity = Vector3.zero;
        }
    }

    void Update()
    {
        //ギミックのドールが生成されてる時
        if (instantDollObj != null)
        {
            if (instantDoll.IsInstant)
            {
                Stop();
                return;
            }
        }

        //死んでないとき
        if (!IsDead)
        {
            //イベント中じゃなければ
            NotEvent();

            //プレイヤーの狂気度が100より大きくなったら
            if (PlayerAction.sanManager.SAN >= 100.0f)
            {
                IsDead = true;
                FadeManager.FadeOut(6);
            }
        }
    }

    //停止時
    void Stop()
    {
        playerRigid.velocity = Vector3.zero;
        if (walk.isPlaying)
        {
            walk.Stop();
        }

        if (sprint.isPlaying)
        {
            sprint.Stop();
        }
    }

    #region 移動処理

    /// <summary>
    /// 移動処理
    /// </summary>
    void Movement()
    {
        #region MoveState関連

        //しゃがみを強制されていないなら
        if (!CrouchForce)
        {
            if (inputManager.IsUseGamePad)
            {
                //ゲームパッド使用時の移動
                MoveStateUseGamePad();
            }
            else
            {
                //キーボード処理時の移動
                MoveStateUseKeyBord();
            }

            //しゃがみ
            if (inputManager.GetKeyDown(EInputCode.CROUTH))
            {
                IsCrouth = !IsCrouth;
            }
        }

        //しゃがんでる時
        if (IsCrouth)
        {
            EPlayerMoveStateGetSet = EPlayerMoveState.CROUCH;
        }

        #endregion

        #region 移動方向

        //移動の入力処理
        MoveInput();

        //移動方向の正規化
        Direction = movementVector.normalized;

        //速度の算出
        Move.Instance.MoveCalc(EPlayerMoveStateGetSet, movementVector.normalized, transform, out movementVector);

        //速度をプレイヤーに適用
        //ベクトル版
        playerRigid.velocity = movementVector;

        //リセット
        movementVector = Vector3.zero;

        #endregion
    }

    /// <summary>
    /// GamePadを利用してる時のMoveStateの遷移
    /// </summary>
    void MoveStateUseGamePad()
    {
        //Not Toggle
        //動いてないとき
        if ((inputManager.GetAxisHorizontal < 0.1f && inputManager.GetAxisHorizontal > -0.1f) &&
            (inputManager.GetAxisVertical < 0.1f && inputManager.GetAxisVertical > -0.1f))
        {
            EPlayerMoveStateGetSet = EPlayerMoveState.STOP;
            CanSprint = false;
        }

        //動いてるとき
        if (inputManager.GetAxisHorizontal > 0.1f || inputManager.GetAxisHorizontal < -0.1f ||
            inputManager.GetAxisVertical > 0.1f || inputManager.GetAxisVertical < -0.1f)
        {
            EPlayerMoveStateGetSet = EPlayerMoveState.WALK;

            //走り
            if (inputManager.GetKey(EInputCode.SPRINT) && inputManager.GetAxisVertical > -0.1f)
            {
                EPlayerMoveStateGetSet = EPlayerMoveState.SPRINT;
            }
        }
    }

    /// <summary>
    /// キーボード使用時のMoveStateの遷移
    /// </summary>
    void MoveStateUseKeyBord()
    {
        //動いてないとき
        if (!inputManager.GetKey(EInputCode.FRONT) && !inputManager.GetKey(EInputCode.BACK)
                                                   && !inputManager.GetKey(EInputCode.RIGHT) &&
                                                   !inputManager.GetKey(EInputCode.LEFT))
        {
            EPlayerMoveStateGetSet = EPlayerMoveState.STOP;
        }

        //動いてるとき
        if (inputManager.GetKey(EInputCode.FRONT) || inputManager.GetKey(EInputCode.BACK)
                                                  || inputManager.GetKey(EInputCode.RIGHT) ||
                                                  inputManager.GetKey(EInputCode.LEFT))
        {
            //歩き
            EPlayerMoveStateGetSet = EPlayerMoveState.WALK;

            //走り
            if (inputManager.GetKey(EInputCode.SPRINT) && inputManager.GetKey(EInputCode.FRONT))
            {
                EPlayerMoveStateGetSet = EPlayerMoveState.SPRINT;
            }
        }
    }

    /// <summary>
    /// キー入力系をまとめるメソッド
    /// </summary>
    void MoveInput()
    {
        if (inputManager.IsUseGamePad)
        {
            //移動量は傾きによらず一定
            //前進
            if (inputManager.GetAxisVertical > 0.1f)
                movementVector += gameObject.transform.forward;

            //後進
            if (inputManager.GetAxisVertical < -0.1f)
                movementVector += -gameObject.transform.forward;

            //右
            if (inputManager.GetAxisHorizontal > 0.1f)
                movementVector += gameObject.transform.right;

            //左
            if (inputManager.GetAxisHorizontal < -0.1f)
                movementVector += -gameObject.transform.right;
        }
        else
        {
            //前進
            if (inputManager.GetKey(EInputCode.FRONT))
                movementVector += gameObject.transform.forward;

            //後進
            if (inputManager.GetKey(EInputCode.BACK))
                movementVector += -gameObject.transform.forward;

            //右
            if (inputManager.GetKey(EInputCode.RIGHT))
                movementVector += gameObject.transform.right;

            //左
            if (inputManager.GetKey(EInputCode.LEFT))
                movementVector += -gameObject.transform.right;
        }
    }

    #endregion
}