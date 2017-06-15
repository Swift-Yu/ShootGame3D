using UnityEngine;
using System.Collections;

public class ShootEffect : MonoBehaviour
{

    public static ShootEffect instance;
    public GameObject shotObject;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
    #region 射击特效，供weapons调用
    public void ShowEffectRepeat()
    {
        InvokeRepeating("ShowShootEffect", 0f, 0.1f);
    }

    public void ShowShootEffect()
    {
        StartCoroutine(ShowEffect());
    }

    public void StopShowEffectRepeat()
    {
        CancelInvoke("ShowShootEffect");
    }

    private IEnumerator ShowEffect()
    {
        shotObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        shotObject.gameObject.SetActive(false);
    }
    #endregion
}
