using UnityEngine;
using UnityEngine.Audio;
namespace UMGS
{
    public class AudioController : SingletonScene<AudioController>
    {
        public AudioSource BackGroundMusic;
        public AudioSource clickAudio;
        AudioSource[] Audios;
        // Use this for initialization
        void Start ()
        {
            //music..... .. . .
            checkAudio ();
        }

        // Update is called once per frame
        void Update ()
        {
            checkAudio ();
        }
        void checkAudio ()
        {
            if (PlayerPrefs.GetInt ("IsMusicAuio") == 1)
            {
                BackGroundMusic.mute = false;
            }
            else
            {

                BackGroundMusic.mute = true;
            }
            //sound ... .. .. 
            if (PlayerPrefs.GetInt ("IsSoundAuio") == 1)
            {
                SetVolumeMute (isMute: false);
            }
            else
            {
                SetVolumeMute ();
            }
        }
        void SetVolumeMute (bool isMute = true)
        {
            Audios = FindObjectsOfType<AudioSource> () as AudioSource[];
            for (int i = 0; i < Audios.Length; i++)
            {
                if (Audios[i] != BackGroundMusic)
                    Audios[i].mute = isMute;
            }
        }

        public void PlayBackGround (AudioClip clip)
        {
            BackGroundMusic.clip = clip;
            if (BackGroundMusic.isPlaying)
                BackGroundMusic.Stop ();
            BackGroundMusic.Play ();
        }
        public void Click ()
        {
            clickAudio.Play ();
        }
    }
}