/*------------------------------------------------------------------------------																								
*			文件名：SetStartImage.CS
*			功 能： N/A
*			
*			VER 2016/12/21   AM11：00  				
*			
*─────────────────────────────────────修 改────────────────────────────────────
*			
*			UPDATE 2017/01/01   AM11：00  
*
*			内容：
*			
*------------------------------------------------------------------------------
*/

using System;
using UnityEngine;
using System.Collections;
using I2.Loc;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Showbaby
{
    public class SetStartImage : MonoBehaviour
    {
        public string MainSceneName;
        [SerializeField]
        private Image images = null;//忠告图片

        void Start()
        {
            if (LocalizationManager.CurrentLanguage == "Chinese")
            {               
                images.gameObject.SetActive(true);
                StartCoroutine(ShowAdviseImage());                
            }
            else
            {
                Invoke("GoMain", 1.0f);
            }
        }
        private float TimesCount = 2f;
        private IEnumerator ShowAdviseImage()
        {
            float times = 0;
            while (times <= TimesCount)
            {
                images.fillAmount = times / TimesCount;
                times += Time.deltaTime;
                yield return null;
            }
            Invoke("GoMain",1.0f);
        }
      
        void GoMain()
        {
            SceneManager.LoadSceneAsync(MainSceneName);
        }

    }
}
