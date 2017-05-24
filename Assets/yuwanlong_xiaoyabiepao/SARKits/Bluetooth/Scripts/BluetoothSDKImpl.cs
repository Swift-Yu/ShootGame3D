namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 蓝牙的抽象类
    /// 供android和ios各自实现
    /// </summary>
    public abstract class BluetoothSDKImpl
    {
        /// <summary>
        /// 开始扫描
        /// </summary>
        public abstract void StartScan();

        /// <summary>
        /// 停止扫描
        /// 内部会判断当前是否在扫描
        /// 如果在扫描就停止扫描，如果没扫描就不处理
        /// 避免出现本来就没在扫描的情况下去调用底层的停止扫描
        /// </summary>
        public abstract void StopScan();
        
        /// <summary>
        /// unity主动连接指定mac地址的蓝牙设备（ios的address与name相同）
        /// </summary>
        /// <param name="address"></param>
        public abstract void Connect(string address);

        /// <summary>
        /// unity主动断开连接
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 获取蓝牙权限
        /// </summary>
        public abstract void RequestPermission();

        /// <summary>
        /// 蓝牙是否已开启
        /// </summary>
        /// <returns></returns>
        public abstract bool IsEnabled();

        /// <summary>
        /// 开启蓝牙
        /// </summary>
        public abstract void OpenBluetooth();
    }
}