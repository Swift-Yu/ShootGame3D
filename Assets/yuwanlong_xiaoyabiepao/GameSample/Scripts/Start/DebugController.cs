using UnityEngine;
using System.Collections;

namespace Showbaby.DebugLog
{
    /// <summary>
    /// 为了方便控制日志是否打印
    /// 统一在Start场景中挂载此脚本，发布版本时记得取消勾选EnableDebug
    /// </summary>
    public class DebugController : MonoBehaviour
    {
        /// <summary>
        /// debug是否可用 false表示不打印日志 true表示打印日志
        /// </summary>
        public bool EnableDebug = false;

        private void Awake()
        {
            Debug.logger.logEnabled = EnableDebug;
        }
    }
}