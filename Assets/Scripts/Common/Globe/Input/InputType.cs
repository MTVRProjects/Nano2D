namespace HMLFramwork
{
    /// <summary>
    /// 用户输入类型枚举
    /// </summary>
    public enum InputType
    {
        NONE,
        /// <summary>
        /// 鼠标某键一直按下；需传入键的ID：0（左键）,1（右键）,2（滚轮））
        /// </summary>
        GetMouse,
        /// <summary>
        /// 鼠标某键按下时；需传入键的ID：0（左键）,1（右键）,2（滚轮））
        /// </summary>
        MouseDown,
        /// <summary>
        /// 鼠标某键抬起时；需传入键的ID：0（左键）,1（右键）,2（滚轮））
        /// </summary>
        MouseUP,
        /// <summary>
        /// 某按键按下时；需传入按键的ID：KeyCode
        /// </summary>
        KeyDown,
        /// <summary>
        /// 某按键一直按下；需传入按键的ID：KeyCode
        /// </summary>
        GetKey,
        /// <summary>
        /// 某按键抬起时；需传入按键的ID：KeyCode
        /// </summary>
        KeyUP
    }
}
