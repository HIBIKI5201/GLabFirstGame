/// <summary>
/// 敵の状態の列挙型
/// </summary>
public enum EnemyStateType
{
    /// <summary>
    /// 通常
    /// </summary>
    Normal,

    /// <summary>
    /// 気絶中（アイテム：石の効果）
    /// </summary>
    Faint,

    /// <summary>
    /// 食いついている（アイテム：肉の効果）
    /// </summary>
    EatingMeat,

    /// <summary>
    /// 逃げている（アイテム：空き瓶の効果）
    /// </summary>
    Escape,

    /// <summary>
    /// 追跡中
    /// </summary>
    ChasingPlayer,
}