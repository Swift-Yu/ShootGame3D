using UnityEngine;
using System.Collections;
namespace Showbaby.UIComponent
{
    /// <summary>
    /// 自动旋转
    /// </summary>
    public class AutoRotate : MonoBehaviour
    {
        /// <summary>
        /// 旋转方向
        /// </summary>
        [SerializeField]
        private Vector3 direction = Vector3.forward;

        /// <summary>
        /// 每帧旋转角度
        /// </summary>
        [SerializeField]
        private float angle = 3f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.Rotate(direction, angle);
        }
    }
}
