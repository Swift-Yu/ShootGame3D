using UnityEngine;
using System.Collections.Generic;
using UnityDebug.Log;
using System;

namespace Showbaby.Music
{
    /// <summary>
    /// 为了便于以后扩展(比如同一音效在不同语言环境下使用不同的声音)，这里使用字符串来标识唯一音乐文件
    /// </summary>
    [Serializable]
    public class MusicInfo
    {
        /// <summary>
        /// 音效文件名
        /// </summary>
        public string MusicName;

        /// <summary>
        /// 音效文件
        /// </summary>
        public AudioClip Audio;

        public MusicInfo(string name, AudioClip audio)
        {
            MusicName = name;
            Audio = audio;
        }
    }

    /// <summary>
    /// 此类用来管理所有的声音播放
    /// MusciManager预制件挂载在Start场景中，不销毁
    /// MusciManager预制件下默认挂载了两个音源，分别用来播放背景音乐和其他音效
    /// 在使用播放函数时如果不想使用默认的音源，请将音源参数带上，
    /// 如果不带音源参数，则默认为使用默认的音源
    /// 注意播放和停止函数的音源组件需要对应（要么都带上参数，要么都不带）!!!
    /// 所有音效请在MusciManager预制件面板进行配置(audioClipList) 否则将无法播放
    /// AudioListener已添加到MusciManager预制件上，请删除其他地方的AudioListener组件！！
    /// </summary>
    public class MusciManager : MonoBehaviour
    {
        private static MusciManager mInstance = null;

        public static MusciManager Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new GameObject("MusciManager").AddComponent<MusciManager>();
                }
                return mInstance;
            }
        }        

        /// <summary>
        /// 默认的背景音乐声源
        /// </summary>
        public AudioSource DefaultBGAudioSource;

        /// <summary>
        /// 默认的音效声源
        /// </summary>
        public AudioSource DefaultEffectAudioSource;

        /// <summary>
        /// 声源列表
        /// </summary>
        public List<AudioSource> audioSourceList = new List<AudioSource>();

        /// <summary>
        /// 音效列表
        /// </summary>
        public List<MusicInfo> musicList = new List<MusicInfo>();

        public Dictionary<string, AudioClip> AudioClipDic = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            mInstance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            foreach (var music in musicList)
            {
                if (null != music && !music.MusicName.Equals("") && null != music.Audio)
                {
                    AudioClipDic.Add(music.MusicName, music.Audio);
                }
                else
                {
                    UnityLog.Error("music is null");
                }
            }
            GameCenter.OnSwitchVoice += GameCenter_OnSwitchVoice;
        }

        private void OnDestroy()
        {
            GameCenter.OnSwitchVoice -= GameCenter_OnSwitchVoice;
        }

        private void GameCenter_OnSwitchVoice(bool state)
        {
            if (state)
            {
                UnityLog.InfoGreen("声音开启");
                AudioListener.volume = 1;
            }
            else
            {
                UnityLog.InfoGreen("声音关闭");
                AudioListener.volume = 0;
            }
        }

        /// <summary>
        /// 此方法用于播放背景音乐一类的 默认循环播放
        /// 如需停止播放 调用对应的StopMusic方法
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioSource"></param>
        /// <param name="loop">是否循环播放</param>
        public void PlayMusic(string audioName, AudioSource audioSource = null, bool loop = true)
        {
            if (null == audioSource)
            {
                UnityLog.InfoPurple("PlayMusic audioSource is null use DefaultBGAudioSource");
                audioSource = DefaultBGAudioSource;
            }
            else
            {
                AddAudioSource(audioSource);
            }

            if (PlayerPrefs.GetInt("CloseAudio", 0) == 0)
            {
                AudioClip ac;
                if (AudioClipDic.TryGetValue(audioName, out ac))
                {
                    audioSource.clip = ac;
                    audioSource.loop = loop;
                    audioSource.Play();
                }
                else
                {
                    UnityLog.Error("PlayMusic audioName is null");
                }
            }
        }

        /// <summary>
        /// 此方法用于停止播放背景音乐一类的
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioSource"></param>
        public void StopMusic(string audioName, AudioSource audioSource = null)
        {
            if (null == audioSource)
            {
                UnityLog.InfoPurple("PlayMusic audioSource is null use DefaultBGAudioSource");
                audioSource = DefaultBGAudioSource;
            }
            
            if (PlayerPrefs.GetInt("CloseAudio", 0) == 0)
            {
                AudioClip ac;
                if (AudioClipDic.TryGetValue(audioName, out ac))
                {
                    audioSource.clip = ac;
                    audioSource.Stop();
                }
                else
                {
                    UnityLog.Error("PlayMusic audioName is null");
                }
            }
        }

        /// <summary>
        /// 暂停/播放音乐
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioSource"></param>
        /// <param name="loop"></param>
        public void PauseMusic(string audioName, AudioSource audioSource = null, bool loop = true)
        {
            if (null == audioSource)
            {
                UnityLog.InfoPurple("PlayMusic audioSource is null use DefaultBGAudioSource");
                audioSource = DefaultBGAudioSource;
            }

            if (PlayerPrefs.GetInt("CloseAudio", 0) == 0)
            {
                AudioClip ac;
                if (AudioClipDic.TryGetValue(audioName, out ac))
                {
                    audioSource.clip = ac;
                    audioSource.loop = loop;
                    if (audioSource.isPlaying)
                    {
                        audioSource.Pause();
                    }
                    else
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    UnityLog.Error("PlayMusic audioName is null");
                }
            }
        }

        /// <summary>
        /// 用于播放短音效（如按钮音效 枪击音效等等）
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioSource"></param>
        public void PlayEffectMusic(string audioName, AudioSource audioSource = null)
        {
            if (null == audioSource)
            {
                UnityLog.InfoPurple("PlayMusic audioSource is null");
                audioSource = DefaultEffectAudioSource;
            }
            else
            {
                AddAudioSource(audioSource);
            }
            if (PlayerPrefs.GetInt("CloseAudio", 0) == 0)
            {
                AudioClip ac;
                if (AudioClipDic.TryGetValue(audioName, out ac))
                {
                    audioSource.clip = ac;
                    audioSource.PlayOneShot(ac);
                    UnityLog.InfoGreen("PlayMusic audioName " + audioName);
                }
                else
                {
                    UnityLog.Error("PlayMusic audioName is null");
                }
            }
        }

        /// <summary>
        /// 提供接口供外部添加AudioSource到audioSourceList列表中
        /// </summary>
        /// <param name="audiosource"></param>
        public void AddAudioSource(AudioSource audiosource)
        {
            if (null != audiosource)
            {
                if (!audioSourceList.Contains(audiosource))
                {
                    audioSourceList.Add(audiosource);
                    UnityLog.InfoGreen(audiosource.name + "Add successed");
                }
                else
                {
                    UnityLog.InfoPurple(audiosource.name + " Add failed,the aduiosource has exist allready");
                }
            }
        }

        /// <summary>
        /// 提供接口供外部添加Music到musicList列表中和AudioClipDic字典中
        /// </summary>
        /// <param name="music"></param>
        public void AddMusic(MusicInfo music)
        {
            if (null != music)
            {
                if (!AudioClipDic.ContainsKey(music.MusicName))
                {
                    musicList.Add(music);
                    AudioClipDic.Add(music.MusicName, music.Audio);
                    UnityLog.InfoGreen(music.MusicName + "Add successed");
                }
                else
                {
                    UnityLog.InfoPurple(music.MusicName + "Add failed,the audioclip has exist allready");
                }
            }
            else
            {
                UnityLog.Error("music is null");
            }
        }

        /// <summary>
        /// 提供接口供外部添加Music到musicList列表中和AudioClipDic字典中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="audio"></param>
        public void AddMusic(string name, AudioClip audio)
        {
            if (!name.Equals("") && null != audio)
            {
                var music = new MusicInfo(name, audio);
                if (!AudioClipDic.ContainsKey(music.MusicName))
                {
                    musicList.Add(music);
                    AudioClipDic.Add(music.MusicName, music.Audio);
                    UnityLog.InfoGreen(music.MusicName + "Add successed");
                }
                else
                {
                    UnityLog.InfoPurple(music.MusicName + "Add failed,the audioclip has exist allready");
                }
            }
            else
            {
                UnityLog.Error("music is null");
            }
        }
    }
}

