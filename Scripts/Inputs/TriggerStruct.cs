using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XInputAssistManager
{
    public struct TriggerStruct
    {
        EButtonList trigger;
        float axis;

        /// <summary>
        /// コンストラクタ 
        /// </summary>
        /// <param name="trigger">※RightTriggerかLeftTriggerを入れること！！</param>
        public TriggerStruct(EButtonList trigger)
        {
            this.trigger = trigger;
            axis = 0;
        }

        /// <summary>
        /// Axisの値を取得し、返すメソッド
        /// 
        /// ※ProjectSettingsのInputに"LeftTriggerAxis"をJoyStickAxisの9th axisで追加しておくこと！
        /// ※ProjectSettingsのInputに"RightTriggerAxis"をJoyStickAxisの10th axisで追加しておくこと！
        /// </summary>
        /// <returns>axis(0か1)</returns>
        // public float GetAxis()
        // {
        //     switch (trigger)
        //     {
        //         case EButtonList.RightTrigger:
        //             axis = Input.GetAxis("RightTriggerAxis");
        //             break;
        //         case EButtonList.LeftTrigger:
        //             axis = Input.GetAxis("LeftTriggerAxis");
        //             break;
        //     }
        //
        //     return axis;
        // }
    }
}