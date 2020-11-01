namespace XInputAssistManager
{
    public enum EInputCode
    {
        #region 移動系

        /// <summary>
        /// 前進
        /// </summary>
        FRONT,

        /// <summary>
        /// 後進
        /// </summary>
        BACK,

        /// <summary>
        /// 左
        /// </summary>
        LEFT,

        /// <summary>
        /// 右
        /// </summary>
        RIGHT,

        /// <summary>
        /// 走り
        /// </summary>
        SPRINT,

        /// <summary>
        /// しゃがみ
        /// </summary>
        CROUTH,

        #endregion

        #region アクション

        /// <summary>
        /// インタラクト
        /// </summary>
        INTERACT,

        #endregion

        #region 攻撃系
        
        /// <summary>
        /// 投げる
        /// </summary>
        THROW,

        #endregion

        #region システム・UI系

        /// <summary>
        /// マップ
        /// </summary>
        MAP,

        /// <summary>
        /// メニュー
        /// </summary>
        MENU,
        
        /// <summary>
        /// 進む
        /// </summary>
        ENTERMENU,
        
        /// <summary>
        /// アプリケーション
        /// </summary>
        QUIT,

        #endregion
    }
}