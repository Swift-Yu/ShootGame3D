using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
namespace ShenQiangLieRen
{
    public class Action_OnClickPlayAudio : MonoBehaviour, IPointerDownHandler 
    {
        private  AudioSource source;

        void Start()
        {
            source = GameObject.FindGameObjectWithTag("ButtonSound").GetComponent<AudioSource>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            source.Play();
        }
    }
}