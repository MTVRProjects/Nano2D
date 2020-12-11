namespace HMLFramwork
{
    namespace UI_Interface
    {
        interface IRayFunc
        {
            /// <summary>
            /// 射线进入
            /// </summary>
            void RayEnter();
            /// <summary>
            /// 射线滞留
            /// </summary>
            void RayStay();
            /// <summary>
            /// 射线退出
            /// </summary>
            void RayExit();
            /// <summary>
            /// 需要执行的功能
            /// </summary>
            void Excute();
        }

        interface IClose
        {
            /// <summary>
            /// 关闭所有UI，包括子对象（需要判断UI子元素各个状态是否正常）
            /// </summary>
            void CloseChildren();
        }

        /// <summary>
        /// UI数据加载接口
        /// </summary>
        interface ILoadData
        {
            /// <summary>
            /// 数据加载状态
            /// </summary>
            DataLoadState MyDataLoadState { get; set; }
            /// <summary>
            /// 数据加载命令
            /// </summary>
            DataLoadOrder MyDataLoadOrder { set; }
        }

        /// <summary>
        /// UI元素的行为接口
        /// </summary>
        interface IUIElementAction
        {
            /// <summary>
            /// 菜单状态
            /// </summary>
            MenuState MyState { set; get; }
            /// <summary>
            /// 操作命令
            /// </summary>
            MenuOperation MyOperation { set; }
        }

        interface IStepCtrlBase
        {
            /// <summary>
            /// 进入下一个步骤
            /// </summary>
            void Next();

            /// <summary>
            /// 进入上一步骤
            /// </summary>
            void Last();
            /// <summary>
            /// 设置当前UI的操作顺序
            /// </summary>
            void SetCurrentOrder(string ORDER);

            /// <summary>
            /// 初始化
            /// </summary>
            void Init();

            /// <summary>
            /// 打开UI
            /// </summary>
            void Open();
            /// <summary>
            /// 关闭UI
            /// </summary>
            void Close();
        }

        interface IStepBase
        {
            int MyOrder { get;}
            /// <summary>
            /// 进入下一个步骤
            /// </summary>
            void Next();

            /// <summary>
            /// 进入上一步骤
            /// </summary>
            void Last();
            /// <summary>
            /// 初始化
            /// </summary>
            void Init();

            /// <summary>
            /// 打开UI
            /// </summary>
            void Start();
            /// <summary>
            /// 关闭UI
            /// </summary>
            void Stop();
        }
    }
    namespace Init
    {
        interface IT_Init
        {
            void Init();
        }
    }
    interface IBehaviourBase
    {
        void Awake();

        void Start();

        void Update();

        void OnDestroy();
    }

    /// <summary>
    /// UI子节点管理接口
    /// </summary>
    interface IUIChildBase
    {
        MenuOperation MyOperation { get; set; }
        void Open();
        void Close();
        void Init();
    }
    /// <summary>
    /// UI父节点管理接口
    /// </summary>
    interface IUIMgrBase
    {
        MenuOperation MyOperation { get; set; }
        void Open();
        void Close();
        void CloseChild(UI_ID childID);

        void OpenChild(UI_ID childID);
        void Init();

    }
}
