using UnityEngine;
using System.Collections;
using ShootingGallery;
using UnityEngine.UI;

public class SetEffect : MonoBehaviour
{
    //[SerializeField]
    //private GameObject AllHitEffect;
    //[SerializeField]
    //private GameObject PerfectHitEffect;
    //[SerializeField]
    //private GameObject FastHitEffect;
    //[SerializeField] private TextMesh ALLHitFont;
    //[SerializeField] private TextMesh PerfectHitFont;
    //[SerializeField] private TextMesh FastHitFont;
    public void ActiveEffectOnce(GameObject effect)
    {
        while(effect.activeSelf)
        {
            effect.SetActive(false);
        }
        effect.SetActive(true);
    }

    public IEnumerator HitEffect(GameObject effect, string score, string origin)
    {
        //ActiveEffectOnce(effect);
        //yield return new WaitForSeconds(1.5f);        
        TextMesh textmesh = effect.GetComponentInChildren<TextMesh>();
        textmesh.text = origin;
        //string originText = textmesh.text;
        textmesh.text = textmesh.text +"\n"+"+" + score;
        ActiveEffectOnce(effect);
        yield return new WaitForSeconds(1.5f);
        //textmesh.text = originText;
    }

    public IEnumerator MulitiEffect(GameObject effect1, GameObject effect2)
    {
        ActiveEffectOnce(effect1);
        yield return new WaitForSeconds(0.75f);
        effect1.gameObject.SetActive(false);
        ActiveEffectOnce(effect2);
    }

    //public void test()
    //{
    //    StartCoroutine(MulitiEffect(AllHitEffect, PerfectHitEffect));
    //}
}
