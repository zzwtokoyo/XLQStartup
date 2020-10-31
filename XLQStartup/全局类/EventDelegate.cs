using System;

namespace XLQStartup.全局类
{
    /// <summary>
    /// 全局事件委托类
    /// </summary>
    public class EventDelegate
    {
        /// <summary>
        /// 搜索设备
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public delegate void bleSearchBtn(object Sender, EventArgs e);
        /// <summary>
        /// 委托对象
        /// </summary>
        public event bleSearchBtn OnBleSearchBtn;
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public delegate void bleConncetBtn(object Sender, EventArgs e);
        /// <summary>
        /// 委托对象
        /// </summary>
        public event bleConncetBtn OnBleConncetBtn;

        /// <summary>
        /// 搜索事件调用
        /// </summary>
        public void OnHandler()//调用
        {
            if (OnBleSearchBtn != null) //不等于null 说明该事件已经注册有函数
            {
                object Sender = null;
                EventArgs e = null;
                this.OnBleSearchBtn(Sender,e);
            }
        }

        /// <summary>
        /// 连接事件调用
        /// </summary>
        public void OnHandlerConnect()//调用
        {
            if (OnBleConncetBtn != null) //不等于null 说明该事件已经注册有函数
            {
                object Sender = null;
                EventArgs e = null;
                this.OnBleConncetBtn(Sender, e);
            }
        }
    }
}
