/// <summary>
/// 数据加载状态
/// </summary>
public enum DataLoadState
{
    /// <summary>
    /// 未加载
    /// </summary>
    UnLoad,
    /// <summary>
    /// 加载中
    /// </summary>
    Loading,
    /// <summary>
    /// 加载完毕
    /// </summary>
    Loaded
}

public enum DataLoadOrder
{
    /// <summary>
    /// 不加载数据，默认状态
    /// </summary>
    Defualt,
    /// <summary>
    /// 开始加载数据
    /// </summary>
    StartLoad,
    /// <summary>
    /// 停止加载数据
    /// </summary>
    StopLoad
}

/// <summary>
/// 菜单状态
/// </summary>
public enum MenuState
{
    /// <summary>
    /// 执行打开过程中，锁定当前状态
    /// </summary>
    LOCK_Open,
    /// <summary>
    /// 执行关闭过程中，锁定当前状态
    /// </summary>
    LOCK_Close,
    /// <summary>
    /// 打开状态
    /// </summary>
    OPEN,
    /// <summary>
    /// 关闭状态
    /// </summary>
    CLOSE
}

/// <summary>
/// 行为状态枚举
/// </summary>
public enum ActionState
{
    /// <summary>
    /// 默认状态
    /// </summary>
    DEFAULT,
    /// <summary>
    /// 进行中
    /// </summary>
    DOING,
    /// <summary>
    /// 暂停
    /// </summary>
    PAUSE,
    /// <summary>
    /// 完成
    /// </summary>
    DONE
}

/// <summary>
/// 打开或关闭菜单
/// </summary>
public enum MenuOperation
{
    /// <summary>
    /// 不操作，命令收到回复
    /// </summary>
    Default,
    /// <summary>
    /// 打开菜单
    /// </summary>
    OPEN,
    /// <summary>
    /// 关闭菜单
    /// </summary>
    CLOSE
}

/// <summary>
/// 菜单翻页
/// </summary>
public enum PageTurning
{
    /// <summary>
    /// 上一页
    /// </summary>
    UP_Page,
    /// <summary>
    /// 下一页
    /// </summary>
    DOWN_Page,
    /// <summary>
    /// 不操作
    /// </summary>
    Default,
}
/// <summary>
/// 旋转模式
/// </summary>
public enum RotateMode
{
    /// <summary>
    /// 绕自身中心旋转
    /// </summary>
    SELF = 0,
    /// <summary>
    /// 绕指定目标旋转
    /// </summary>
    AROUND
}

/// <summary>
/// 旋转中心轴
/// </summary>
public enum RotateAxle { X, Y, Z }

/// <summary>
/// 通用轴
/// </summary>
public enum ComAxle { X, Y, Z }
/// <summary>
/// 手柄类型
/// </summary>
public enum HandleType
{
    Defualt,
    LEFT,
    RIGHT
}

public enum YesOrNo { YES, NO };

/// <summary>
/// 旋转方向
/// </summary>
public enum RotationDirection
{
    /// <summary>
    /// 顺时针旋转
    /// </summary>
    CLOCKWISE,
    /// <summary>
    /// 逆时针旋转
    /// </summary>
    Anti_CLOCKWISE
}
/// <summary>
/// 时间、日期显示模式
/// </summary>
public enum TimeShowModel { OnlyDATE, OnlyTIME, ALL }

/// <summary>
/// 手柄行为
/// </summary>
public enum HandleAction
{
    /// <summary>
    /// 无操作
    /// </summary>
    Defualt,
    /// <summary>
    /// 按扳机键时
    /// </summary>
    PressTrigger,
    /// <summary>
    /// 按下扳机键
    /// </summary>
    PressTriggerDOWN,
    /// <summary>
    /// 松开扳机键
    /// </summary>
    PressTriggerUP,
    /// <summary>
    /// 按下手柄两侧按钮
    /// </summary>
    PressGripDOWN,
    /// <summary>
    /// 松开手柄两侧按钮
    /// </summary>
    PressGripUP,
    /// <summary>
    /// 按下主菜单键
    /// </summary>
    PressMainMenuDOWN,
    /// <summary>
    /// 松开主菜单键
    /// </summary>
    PressMainMenuUp,
    /// <summary>
    /// 松开触摸板
    /// </summary>
    PressPadUp,
    /// <summary>
    /// 按下触摸板
    /// </summary>
    PressPadDown
}

/// <summary>
/// 手柄操作状态
/// </summary>
public enum HandleOperationState
{
    /// <summary>
    /// 无操作
    /// </summary>
    Default,
    /// <summary>
    /// 传送
    /// </summary>
    Teleport,
    /// <summary>
    /// 射线发射状态
    /// </summary>
    RayEmission,
    /// <summary>
    /// 操作UI
    /// </summary>
    UI_Operate,
    /// <summary>
    /// 操作游戏
    /// </summary>
    GO_Operate,
    /// <summary>
    /// 移动和旋转
    /// </summary>
    MoveAndRotate,
    /// <summary>
    /// 无任何操作
    /// </summary>
    NO_Action

}

/// <summary>
/// 当前操作系统
/// </summary>
public enum CurrentSystem
{
    Default,
    WIN7,
    WIN10
}

/// <summary>
/// 图片格式
/// </summary>
public enum PicFormat
{
    PNG,
    JPG
}
/// <summary>
/// 射线状态
/// </summary>
public enum RayActionState
{
    Defualt,
    /// <summary>
    /// 未射中目标对象
    /// </summary>
    UnShootTarget,
    /// <summary>
    /// 射中目标对象
    /// </summary>
    ShootTarget
}

/// <summary>
/// UI的ID
/// </summary>
public enum UI_ID
{
    Defualt,ONE,TWO,THREE,FOUR,FIVE,SIX
}
public enum RayState { DEFUALT,ENTER,STAY,EXIT}

/// <summary>
/// 正负
/// </summary>
public enum PositiveOrMinus
{
    [EnumExplain("默认")]
    DEFUALT = 0,
    /// <summary>
    /// 正
    /// </summary>
    //[EnumExplain("正")]
    Positive = 1,
    /// <summary>
    /// 负
    /// </summary>
    [EnumExplain("负")]
    Minus = -1
}
/// <summary>
/// 异步/同步
/// </summary>
public enum AsynOrSync
{
    DEFUALT = 0,
    /// <summary>
    /// 异步
    /// </summary>
    ASYN = 1,
    /// <summary>
    /// 同步
    /// </summary>
    SYNC = -1
}

/// <summary>
/// 状态切换
/// </summary>
public enum SWICH
{
    [EnumExplain("默认状态")]
    DEFUALT,
    [EnumExplain("状态1")]
    STATE_1,
    [EnumExplain("状态2")]
    STATE_2,
    [EnumExplain("状态3")]
    STATE_3,
    [EnumExplain("状态4")]
    STATE_4,
    [EnumExplain("状态5")]
    STATE_5,
    [EnumExplain("状态6")]
    STATE_6,
    [EnumExplain("状态7")]
    STATE_7,
    [EnumExplain("状态8")]
    STATE_8,
}

/// <summary>
/// 时间戳类型
/// </summary>
public enum TimeSpanType
{
    /// <summary>
    /// 秒
    /// </summary>
    SEC,
    /// <summary>
    /// 毫秒
    /// </summary>
    MS
}

