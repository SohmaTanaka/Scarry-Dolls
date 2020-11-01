/// <summary>
/// 動きの状態を示す
/// 
/// 担当者：田中颯馬
/// 作成日時:2019/5/9
/// </summary>
public enum EPlayerMoveState
{
    /// <summary>
    /// 止まる
    /// </summary>
    STOP,

    /// <summary>
    /// 歩き
    /// </summary>
    WALK,

    /// <summary>
    /// 走り
    /// </summary>
    SPRINT,

    /// <summary>
    /// しゃがみ
    /// </summary>
    CROUCH,
}
