using UnityEngine;
using System.Collections;

public class Action_HideSelf : MonoBehaviour {
    public float t=0.2f;
    private float overT;
    void OnEnable()
    {
        overT = 0;
    }
    void Update()
   {
       overT += Time.deltaTime;
        if(overT>=t)
        {
            gameObject.SetActive(false);
        }
   }
}
