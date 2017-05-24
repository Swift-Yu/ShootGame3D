namespace Showbaby.Bluetooth
{
    /// <summary>
    ///蓝牙设备的信息
    /// </summary>
    public class BluetoothSDKDevInfo
    {
        /// <summary>
        /// 蓝牙设备的mac地址 android是mac地址 ios是名称name
        /// </summary>
        public string address = "";

        /// <summary>
        /// 蓝牙设备的名称
        /// </summary>
        public string name = "";

        /// <summary>
        /// 别名（显示的名字 以后如果有重命名的功能 更改的就是这个名字）
        /// </summary>
        public string alias = "";
    }

    public class GunNameRule
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public string name = "";

        /// <summary>
        /// 范围最小值mac地址
        /// </summary>
        public string minValue = "";

        /// <summary>
        /// 范围最大值mac地址
        /// </summary>
        public string maxValue = "";
    }
}