using UnityDebug.Log;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 此脚本作为样例 
    /// 为了避免更新通用框架的时候导致脚本被覆盖，建议不要直接使用此脚本，否则后果自负！！！
    /// 开发者可拷贝此脚本然后重命名（拷贝的脚本请移动到自己的目录下!!命名空间也记得改为自己的命名空间）扩展自己的逻辑
    /// ShowDebugInfo用来设置是否打印日志 可在面板进行配置(发布时记得取消勾选!!!!)
    /// useDefaultAudio根据自己需要进行设置
    /// </summary>
    public class BluetoothReceiverSample : BluetoothReceiver
    {
        public override void ReceiveShootInfo()
        {
            base.ReceiveShootInfo();
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiverSample 收到射击消息");
            }
            //下面实现收到消息后的处理逻辑

        }

        public override void ReceiveSwapBulletInfo()
        {
            base.ReceiveSwapBulletInfo();
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiverSample 收到更换子弹消息");
            }
            //下面实现收到消息后的处理逻辑

        }

        public override void ReceiveSwapWappenInfo()
        {
            base.ReceiveSwapWappenInfo();
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiverSample 收到更换武器消息");
            }
            //下面实现收到消息后的处理逻辑

        }

        public override void ReceiveSwapItemInfo()
        {
            base.ReceiveSwapItemInfo();
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiverSample 收到更换道具消息");
            }
            //下面实现收到消息后的处理逻辑

        }

        public override void ReceiveLongSwapItemInfo()
        {
            base.ReceiveLongSwapItemInfo();
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiverSample 收到更换备用道具消息");
            }
            //下面实现收到消息后的处理逻辑

        }
    }
}