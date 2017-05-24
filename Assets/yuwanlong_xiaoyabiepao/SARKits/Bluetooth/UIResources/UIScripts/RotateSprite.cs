using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Showbaby.UIComponent
{
    /// <summary>
    /// 图片旋转效果
    /// </summary>
    public class RotateSprite : MonoBehaviour
    {
        #region 面板设置
        /// <summary>
        /// 图片
        /// </summary>
        [SerializeField]
        private Image image;

        /// <summary>
        /// 旋转速度
        /// </summary>
        [SerializeField]
        private float speed = 0.1f;

        /// <summary>
        /// 帧图片的行数
        /// </summary>
        [SerializeField]
        private int Row = 2;

        /// <summary>
        /// 帧图片的列数
        /// </summary>
        [SerializeField]
        private int Cloum = 4;

        /// <summary>
        /// 帧图片
        /// </summary>
        [SerializeField]
        private Texture2D texture;
        #endregion

        #region 私有变量
        /// <summary>
        /// 当前帧图片的下标
        /// </summary>
        private int cIndex = 0;

        /// <summary>
        /// 帧图片的列表
        /// </summary>
        private List<Sprite> sprites = new List<Sprite>();

        /// <summary>
        /// 当前时间
        /// </summary>
        private float now = 0;
        #endregion

        // Use this for initialization
        void Start()
        {
            var cw = texture.width / Cloum;
            var ch = texture.height / Row;
            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Cloum; c++)
                {
                    var rc = new Rect((float)cw * c, (float)ch * r, (float)cw, (float)ch);
                    Sprite s = Sprite.Create(texture, rc, Vector2.zero);
                    sprites.Add(s);
                }
            }
            image.sprite = sprites[cIndex];
            now = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - now > speed)
            {
                cIndex = cIndex + 1 >= sprites.Count ? 0 : cIndex + 1;
                now = Time.time;
                image.sprite = sprites[cIndex];
            }
        }
    }
}

