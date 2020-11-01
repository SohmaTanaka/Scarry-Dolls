using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XInputAssistManager
{
    public class TriggerAsButton
    {

        TriggerStruct triggerStruct;    //Axisの値を利用できる構造体
        float previousTrigger;             //前のフレームのTriggerの状態
        float currentTrigger;               //現在のフレームのTriggerの状態

        /// <summary>
        /// コンストラクタ
        /// 
        /// ※Awakeで必ず呼ぶこと！！
        /// </summary>
        /// <param name="trigger"></param>
        public TriggerAsButton(EButtonList trigger)
        {
            triggerStruct = new TriggerStruct(trigger);
        }

        /// <summary>
        /// 前のフレームに押されておらず、現在のフレームで押されているか？
        /// </summary>
        /// <returns></returns>
        public bool IsTriggerDown
        {
            get { return (previousTrigger == 0) && (currentTrigger == 1); }
        }

        /// <summary>
        /// ボタンは押されていないか
        /// </summary>
        /// <returns></returns>
        public bool IsTriggerUp
        {
            get { return currentTrigger == 0; }
        }

        /// <summary>
        /// ボタンが押されているか？
        /// </summary>
        public bool GetTriggerState
        {
            get { return currentTrigger == 1; }
        }

        /// <summary>
        /// Triggerの状態の更新処理
        /// </summary>
        public void Update()
        {
            //previousTrigger = currentTrigger;
            //currentTrigger = triggerStruct.GetAxis();
        }
    }
}