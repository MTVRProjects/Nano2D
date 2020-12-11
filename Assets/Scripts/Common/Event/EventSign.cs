
/// <summary>
/// 事件标志（事件ID）
/// </summary>
public enum EventSign
{
    /// <summary>
    /// 绑定鼠标操作等事件（传入InputType；鼠标ID：0,1,2；事件：Action）
    /// </summary>
    BindMouseEvent,
    /// <summary>
    /// 是否启用日志
    /// </summary>
    EnableLog,
    /// <summary>
    /// 清空定时器
    /// </summary>
    ClearTimerEvent,
    /// <summary>
    /// 是否暂停定时器（传bool值，true：暂停，false：暂停后开启）
    /// </summary>
    IsPuaseTimerEvent,

    /// <summary>
    /// 加载场景
    /// </summary>
    LOAD_SCENE,
    /// <summary>
    /// 加载资源
    /// </summary>
    LOAD_AssetsBundle,
    /// <summary>
    /// 是否禁用传送功能（传入bool值）
    /// </summary>
    EnableTeleport,
    /// <summary>
    /// 传送
    /// </summary>
    TELEPORT,
    /// <summary>
    /// 禁用手柄的操作功能（传入bool值）
    /// </summary>
    EnableHandleFunc,

    /// <summary>
    /// 播放视频
    /// </summary>
    PlayVideo,

    /// <summary>
    /// 停止播放几个区域的介绍视频
    /// </summary>
    StopVideo,
    /// <summary>
    /// 全场静音
    /// </summary>
    AllMute,
    /// <summary>
    /// 播放音频
    /// </summary>
    PlayAudio,
    /// <summary>
    /// 关闭音频
    /// </summary>
    StopAudio,
    /// <summary>
    /// 设置玩家状态
    /// </summary>
    SetPlayerState,
    /// <summary>
    /// 设置游戏状态（传入：GameState）
    /// </summary>
    SetGameState,
    /// <summary>
    /// 初始化
    /// </summary>
    Init,
    /// <summary>
    /// 获取玩家状态
    /// </summary>
    GetGameState,
    /// <summary>
    /// 添加传送完成回调（参数为函数名； EventCenter.Instance.Excute(EventSign.AddTeleportCallback, callback);）
    /// </summary>
    AddTeleportCallback,

    /// <summary>
    /// 故障演示 切换场景播放完成
    /// </summary>
    ChangeScenePlayOver,
}