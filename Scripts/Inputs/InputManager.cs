using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XInputAssistManager
{
    /// <summary>
    /// ※継承禁止、GamePad操作はXInput専用
    /// Inputの管理
    /// 
    /// 作成日時:2018/9/20
    /// </summary>
    public sealed class InputManager : MonoBehaviour
    {
        //インスタンス用フィールド変数
        static InputManager inputManager;

        //JoyStick接続確認用配列
        string[] gamePadNames;

        #region プロパティ

        /// <summary>
        /// ゲームパッドを使用しているか？
        /// </summary>
        public bool IsUseGamePad { get; set; }

        /// <summary>
        /// キーコンフィグの設定を収納するDictionaryプロパティ（ボタン版）
        /// </summary>
        public Dictionary<EInputCode, KeyCode> CodeDictionary { get; private set; }

        /// <summary>
        /// キーコンフィグの設定を収納するDictionaryプロパティ（キーボード版）
        /// </summary>
        public Dictionary<EInputCode, KeyCode> CodeDictionaryOnKeyBoard { get; private set; }

        /// <summary>
        /// ゲームパッドのボタンの名前を収納するDictionaryプロパティ
        /// </summary>
        public Dictionary<EButtonList, KeyCode> ButtonNameDictionary { get; private set; }

        /// <summary>
        /// キーコンフィグ（Trigger）の設定を収納するDictionaryプロパティ
        /// </summary>
        public Dictionary<EInputCode, TriggerAsButton> CodeTriggerDictionary { get; private set; }

        /// <summary>
        /// ゲームパッドのTriggerの名前を収納するDictionaryプロパティ
        /// </summary>
        public Dictionary<EButtonList, TriggerAsButton> TriggerNameDictionary { get; private set; }

        /// <summary>
        /// 再処理までのカウントをするタイマー
        /// </summary>
        float RestartTimer { get; set; }

        /// <summary>
        /// RT用
        /// </summary>
        TriggerAsButton RightTrigger { get; set; }

        /// <summary>
        /// LT用
        /// </summary>
        TriggerAsButton LeftTrigger { get; set; }

        #region Axis処理

        /// <summary>
        /// 水平方向の入力の大きさ取得
        /// </summary>
        /// <returns>水平方向の入力の大きさ</returns>
        public float GetAxisHorizontal
        {
            get { return Input.GetAxis("Horizontal"); }
        }

        /// <summary>
        /// 垂直方向の入力の大きさ取得
        /// </summary>
        /// <returns>垂直方向の入力の大きさ</returns>
        public float GetAxisVertical
        {
            get { return Input.GetAxis("Vertical"); }
        }

        /// <summary>
        /// ※ProjectSettingsのInputに"RightStickHorizontal"をJoyStickAxisの4th axisで追加しておくこと！
        /// 
        /// 右スティック水平方向の入力の大きさ取得
        /// </summary>
        /// <returns>右スティック水平方向の入力の大きさ</returns>
        public float GetAxisRightHorizontal
        {
            get { return Input.GetAxis("RightStickHorizontal"); }
        }

        /// <summary>
        /// ※ProjectSettingsのInputに"RightStickVertical"をJoyStickAxisの5th axisで追加しておくこと！
        /// 
        /// 右スティック垂直方向の入力の大きさ取得
        /// </summary>
        /// <returns>右スティック垂直方向の入力の大きさ</returns>
        public float GetAxisRightVertical
        {
            get { return Input.GetAxis("RightStickVertical"); }
        }

        #endregion

        #endregion

        /// <summary>
        /// インスタンス取得
        /// </summary>
        /// <returns>実体</returns>
        public static InputManager GetInstance
        {
            get
            {
                if (inputManager == null)
                {
                    //GameObjectの生成
                    GameObject gameObject = new GameObject("InputManagerGO");
                    //InputManagerのAdd
                    gameObject.AddComponent<InputManager>();
                    //Play時間をカウントするスクリプトをAdd by安部
                    gameObject.AddComponent<GameTimer>();
                    //設定の保存用スクリプト　by石橋
                    gameObject.AddComponent<ConfigSaveManager>();
                    //破壊不可
                    DontDestroyOnLoad(gameObject);

                    inputManager = gameObject.GetComponent<InputManager>();
                }

                return inputManager;
            }
        }

        /// <summary>
        /// 生成直後に一回だけ実行
        /// </summary>
        void Awake()
        {
            //初期化

            // RightTrigger = new TriggerAsButton(EButtonList.RightTrigger);
            // LeftTrigger = new TriggerAsButton(EButtonList.LeftTrigger);
            CodeDictionary = new Dictionary<EInputCode, KeyCode>();
            CodeDictionaryOnKeyBoard = new Dictionary<EInputCode, KeyCode>();
            ButtonNameDictionary = new Dictionary<EButtonList, KeyCode>();
            CodeTriggerDictionary = new Dictionary<EInputCode, TriggerAsButton>();
            TriggerNameDictionary = new Dictionary<EButtonList, TriggerAsButton>();

            RestartTimer = 0.0f;

            SetButtonName();
            SetCode();
            IsUseGamePad = true;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            //TriggerNameDictionary内の値の更新処理
            // foreach (TriggerAsButton item in TriggerNameDictionary.Values)
            // {
            //     item.Update();
            // }

            //GamePadが接続しているか？
            gamePadNames = Input.GetJoystickNames();

            //キーを押したときはゲームパッド入力を使用しない
             if (!AnyButtonDown())
             {
                 if (Input.anyKeyDown)
                 {
                     IsUseGamePad = false;
                 }
             }
            
             //接続している時
             if (gamePadNames.Length != 0)
             {
                 //どれかのボタンを押したらゲームパッド入力を利用する
                 if (AnyButtonDown())
                 {
                     IsUseGamePad = true;
                 }
             }
            
             //Debug.Log(IsUseGamePad);
        }

        #region Awakeの中身

        /// <summary>
        /// ※Awakeで呼ぶこと
        /// ※事故防止のためfor文などでは回していない
        /// 
        /// ButtonListとKeyCodeを結びつけるメソッド
        /// </summary>
        void SetButtonName()
        {
            //WindowsOS用
            //XInput
            //Button
            ButtonNameDictionary.Add(EButtonList.GamePad_A, KeyCode.JoystickButton0); //Aボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_B, KeyCode.JoystickButton1); //Bボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_X, KeyCode.JoystickButton2); //Xボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_Y, KeyCode.JoystickButton3); //Yボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_L, KeyCode.JoystickButton4); //Lボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_R, KeyCode.JoystickButton5); //Rボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_Back, KeyCode.JoystickButton6); //Backボタン
            ButtonNameDictionary.Add(EButtonList.GamePad_Start, KeyCode.JoystickButton7); //Startボタン
            ButtonNameDictionary.Add(EButtonList.LeftStickPush, KeyCode.JoystickButton8); //左スティック押し込み
            ButtonNameDictionary.Add(EButtonList.RightStickPush, KeyCode.JoystickButton9); //右スティック押し込み
            //Trigger
            //TriggerNameDictionary.Add(EButtonList.LeftTrigger, LeftTrigger); //LT
            //TriggerNameDictionary.Add(EButtonList.RightTrigger, RightTrigger); //RT
        }

        /// <summary>
        /// ※Awakeで呼ぶこと
        /// ※事故防止のためfor文などでは回していない
        /// 
        /// InputCodeとKeyCodeを結びつけるメソッド
        /// </summary>
        void SetCode()
        {
            //デフォルトのキー設定
            //例（ボタン版）:CodeDictionary.Add(InputCode.操作, ButtonNameDictionary[ButtonList.対応させるボタン名]);
            //サンプル用
            //CodeDictionary.Add(InputCode.Attack, ButtonNameDictionary[ButtonList.GamePad_X]);
            //CodeDictionary.Add(InputCode.Jump, ButtonNameDictionary[ButtonList.GamePad_A]);
            //例（キーボード版）:CodeDictionaryOnKeyBoard.Add(InputCode.操作, KeyCode.対応させるキー名);
            //サンプル用
            //CodeDictionaryOnKeyBoard.Add(InputCode.Attack, KeyCode.Mouse0);

            //ボタン
            CodeDictionary.Add(EInputCode.CROUTH, ButtonNameDictionary[EButtonList.GamePad_B]);
            CodeDictionary.Add(EInputCode.INTERACT, ButtonNameDictionary[EButtonList.GamePad_A]);
            CodeDictionary.Add(EInputCode.SPRINT, ButtonNameDictionary[EButtonList.GamePad_R]);
            CodeDictionary.Add(EInputCode.THROW, ButtonNameDictionary[EButtonList.GamePad_L]);
            CodeDictionary.Add(EInputCode.QUIT,ButtonNameDictionary[EButtonList.GamePad_Back]);
            //キーボード
            CodeDictionaryOnKeyBoard.Add(EInputCode.FRONT, KeyCode.W);
            CodeDictionaryOnKeyBoard.Add(EInputCode.BACK, KeyCode.S);
            CodeDictionaryOnKeyBoard.Add(EInputCode.RIGHT, KeyCode.D);
            CodeDictionaryOnKeyBoard.Add(EInputCode.LEFT, KeyCode.A);
            CodeDictionaryOnKeyBoard.Add(EInputCode.SPRINT, KeyCode.LeftShift);
            CodeDictionaryOnKeyBoard.Add(EInputCode.CROUTH, KeyCode.C);
            CodeDictionaryOnKeyBoard.Add(EInputCode.INTERACT, KeyCode.E);
            CodeDictionaryOnKeyBoard.Add(EInputCode.THROW, KeyCode.Mouse0);
            CodeDictionaryOnKeyBoard.Add(EInputCode.QUIT,KeyCode.Escape);
            //CodeDictionaryOnKeyBoard.Add(EInputCode.THROW, KeyCode.Mouse2);
        }

        #endregion

        #region キーの状態取得

        /// <summary>
        /// キーを押してない状態から押したときtrue
        /// </summary>
        /// <param name="eInputCode">操作の名前</param>
        /// <returns>キーの状態</returns>
        public bool GetKeyDown(EInputCode eInputCode)
        {
            switch (IsUseGamePad)
            {
                case true:

                    //現在の入力キーがCodeDictionaryに入っていなければ
                    if (!CodeDictionary.ContainsKey(eInputCode))
                    {
                        //トリガーの状態を返す
                        return CodeTriggerDictionary[eInputCode].IsTriggerDown;
                    }

                    //ボタンもしくはキーのKeyCodeを返す
                    return Input.GetKeyDown(CodeDictionary[eInputCode]);

                default:
                    //キーボード使ってるなら
                    return Input.GetKeyDown(CodeDictionaryOnKeyBoard[eInputCode]);
            }
        }

        /// <summary>
        /// キーを押し続けている時true
        /// </summary>
        /// <param name="eInputCode">操作の名前</param>
        /// <returns>キーの状態</returns>
        public bool GetKey(EInputCode eInputCode)
        {
            switch (IsUseGamePad)
            {
                case true:

                    //現在の入力キーがCodeDictionaryに入っていなければ
                    if (!CodeDictionary.ContainsKey(eInputCode))
                    {
                        //トリガーの状態を返す
                        return CodeTriggerDictionary[eInputCode].GetTriggerState;
                    }

                    //ボタンもしくはキーのKeyCodeを返す
                    return Input.GetKey(CodeDictionary[eInputCode]);

                default:
                    //キーボード使ってるなら
                    return Input.GetKey(CodeDictionaryOnKeyBoard[eInputCode]);
            }
        }

        /// <summary>
        /// キーを押している状態から離した時true
        /// </summary>
        /// <param name="eInputCode">操作の名前</param>
        /// <returns>キーの状態</returns>
        public bool GetKeyUp(EInputCode eInputCode)
        {
            switch (IsUseGamePad)
            {
                case true:

                    //現在の入力キーがCodeDictionaryに入っていなければ
                    if (!CodeDictionary.ContainsKey(eInputCode))
                    {
                        //トリガーの状態を返す
                        return CodeTriggerDictionary[eInputCode].IsTriggerUp;
                    }

                    //ボタンもしくはキーのKeyCodeを返す
                    return Input.GetKeyUp(CodeDictionary[eInputCode]);

                default:
                    //キーボード使ってるなら
                    return Input.GetKeyUp(CodeDictionaryOnKeyBoard[eInputCode]);
            }
        }

        #endregion

        /// <summary>
        /// キーの設定
        /// </summary>
        /// <param name="inputCord">操作の名前</param>
        /// <param name="key">キーコード</param>
        public void SetButton(EInputCode eInputCode, KeyCode key)
        {
            //現在の入力キーがCodeTriggerDictionaryに入っていたら
            if (CodeTriggerDictionary.ContainsKey(eInputCode))
            {
                //消しておく(競合回避)
                CodeTriggerDictionary.Remove(eInputCode);
                CodeDictionary.Add(eInputCode, key);
            }
            else
            {
                CodeDictionary[eInputCode] = key;
            }
        }

        /// <summary>
        /// キーの設定（トリガー版）
        /// 
        /// </summary>
        /// <param name="inputCord">操作の名前</param>
        /// <param name="trigger">キーコード</param>
        public void SetTrigger(EInputCode eInputCode, TriggerAsButton trigger)
        {
            //現在の入力キーがCodeDictionaryに入っていたら
            if (CodeDictionary.ContainsKey(eInputCode))
            {
                //消しておく(競合回避)
                CodeDictionary.Remove(eInputCode);
                CodeTriggerDictionary.Add(eInputCode, trigger);
            }
            else
            {
                CodeTriggerDictionary[eInputCode] = trigger;
            }
        }

        /// <summary>
        /// KeyCodeを返す
        /// </summary>
        /// <param name="eButton"></param>
        /// <returns></returns>
        public KeyCode GetButtonCode(EButtonList eButton)
        {
            //Debug.Log(ButtonNameDictionary[eButton]);
            return ButtonNameDictionary[eButton];
        }

        /// <summary>
        /// ボタン（トリガー）名で直接判定する
        /// 前のフレームに押されていなくて現在のフレームで押されていたらtrue
        /// </summary>
        /// <param name="eButton"></param>
        /// <returns></returns>
        public bool GetKeyCodeButtonDown(EButtonList eButton)
        {
            //ボタンかトリガーかの判別
            if (ButtonNameDictionary.ContainsKey(eButton))
            {
                return Input.GetKeyDown(GetButtonCode(eButton));
            }
            else if (TriggerNameDictionary.ContainsKey(eButton)) return TriggerNameDictionary[eButton].IsTriggerDown;

            return false;
        }

        /// <summary>
        /// ボタン（トリガー）名で直接判定する
        /// キーを押し続けている時true
        /// </summary>
        /// <param name="inputCode">操作の名前</param>
        /// <returns>キーの状態</returns>
        public bool GetKeyCodeButton(EButtonList eButton)
        {
            //ボタンかトリガーかの判別
            if (ButtonNameDictionary.ContainsKey(eButton)) return Input.GetKey(GetButtonCode(eButton));
            else if (TriggerNameDictionary.ContainsKey(eButton)) return TriggerNameDictionary[eButton].GetTriggerState;

            return false;
        }

        /// <summary>
        /// ボタン（トリガー）名で直接判定する
        /// キーを押している状態から離した時true
        /// </summary>
        /// <param name="inputCode">操作の名前</param>
        /// <returns>キーの状態</returns>
        public bool GetKeyCodeButtonUp(EButtonList eButton)
        {
            //ボタンかトリガーかの判別
            if (ButtonNameDictionary.ContainsKey(eButton)) return Input.GetKeyUp(GetButtonCode(eButton));
            else if (TriggerNameDictionary.ContainsKey(eButton)) return TriggerNameDictionary[eButton].IsTriggerUp;

            return false;
        }

        /// <summary>
        /// いずれかのボタン（トリガー）が押されたら
        /// </summary>
        /// <returns></returns>
        public bool AnyButtonDown()
        {
            //ButtonListの数だけ回して、どれかが押されていたらtureを返す
            for (int i = 0; i < (int) EButtonList.End; i++)
            {
                if (inputManager.GetKeyCodeButtonDown((EButtonList) i))
                {
                    return true;
                }
            }

            //Debug.Log("抜けた");
            
            return false;
        }
    }
}