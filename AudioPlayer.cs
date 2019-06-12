using UnityEngine;
using UMGS;
public class AudioPlayer : Singleton<AudioPlayer>
{
    private AudioSource _speaker;
    private AudioSource Speaker
    {
        get
        {
            if (_speaker == null)
                _speaker = GetComponent<AudioSource> ();
            if (_speaker == null)
                _speaker = gameObject.AddComponent<AudioSource> ();
            return _speaker;
        }
    }


    public static void Play (AudioClip clip)
    {
        instance.Speaker.PlayOneShot (clip);
    }
}
