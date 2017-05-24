using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UITools.FrameAnimation
{
    [RequireComponent(typeof(Image))]
    public class FrameAnimation : MonoBehaviour
    {
        private Image animImage;
        public List<Sprite> Sprites;
        public float FrameRate = 24;
        public bool AutoStart = true;

        #region 私有变量
        private int currectFrame;
        private bool startPlay = false;
        private float playTime = 0f;
        private float frameTime = 0;
        #endregion

        #region 公共接口
        public void StartPlay()
        {
            startPlay = true;
            playTime = 0;
            currectFrame = 0;
        }

        public void StopPlay()
        {
            startPlay = false;
            playTime = 0;
            currectFrame = 0;
        }
        #endregion

        private void Start()
        {
            animImage = GetComponent<Image>();
            if (AutoStart)
            {
                StartPlay();
            }
            frameTime = 1 / FrameRate;
        }

        private void OnDisable()
        {
            StopPlay();
        }

        private void OnEnable()
        {
            if (AutoStart)
                StartPlay();
        }

        private void Update()
        {
            if (startPlay)
            {
                if (playTime > frameTime)
                {
                    animImage.sprite = Sprites[currectFrame++];
                    currectFrame = currectFrame >= Sprites.Count ? 0 : currectFrame;
                    playTime = 0f;
                }
                else
                    playTime += Time.deltaTime;
            }
        }
    }
}
