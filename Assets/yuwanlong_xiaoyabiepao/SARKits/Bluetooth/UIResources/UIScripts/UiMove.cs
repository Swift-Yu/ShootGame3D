using UnityEngine;

namespace Showbaby.UIComponent
{
    /// <summary>
    /// 线条上下移动的效果
    /// </summary>
    public class UiMove : MonoBehaviour {
        #region 面板设置
        /// <summary>
        /// X移动曲线
        /// </summary>
        [SerializeField]
        private AnimationCurve XMoveCurve;

        /// <summary>
        /// Y移动曲线
        /// </summary>
        [SerializeField]
        private AnimationCurve YMoveCurve;

        /// <summary>
        /// X Y移动范围
        /// </summary>
        public Vector2 MoveRange;
        #endregion
        private bool isActive = false;
        #region 私有变量
        /// <summary>
        /// 自身的RectTransform
        /// </summary>
        private RectTransform rtf;

        /// <summary>
        /// 起始坐标
        /// </summary>
        private Vector2 src;
        #endregion

        void Start()
        {
            rtf = GetComponent<RectTransform>();
            src = rtf.anchoredPosition;
        }
        void OnEnable()
        {
            isActive = true;
            rtf.anchoredPosition = src;
        }
        void OnDisable()
        {
            isActive = false;
        }
        public void Update()
        {
            if(isActive)
            {
                var t = rtf.anchoredPosition;
                t.x = src.x + MoveRange.x * XMoveCurve.Evaluate(Time.time);
                t.y = src.y + MoveRange.y * YMoveCurve.Evaluate(Time.time);
                rtf.anchoredPosition = t;
            }
           
        }

    }
}