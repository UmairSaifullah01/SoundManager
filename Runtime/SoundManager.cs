using THEBADDEST;
using UnityEngine;
using UnityEngine.Audio;


namespace THEBADDEST.SoundSystem
{


	public class SoundManager : MonoBehaviour
	{

		[SerializeField] SoundService Settings;


		protected  void Awake()
		{
			Settings.Initialize(this);
		}

		public void Register(Sound s)
		{
			GameObject g = new GameObject(s.SoundName, typeof(AudioSource));
			g.transform.SetParent(transform, false);
			s.Source                       = g.GetComponent<AudioSource>();
			s.Source.clip                  = (s.PlayRandomClip) ? s.AudioClips[Random.Range(0, s.AudioClips.Length)] : s.AudioClip;
			s.Source.volume                = s.Volume;
			s.Source.pitch                 = s.Pitch;
			s.Source.loop                  = s.Loop;
			s.Source.outputAudioMixerGroup = Settings.GetAudioMixerGroup(s.Type);
			if (s.PlayOnAwake) s.Source.Play();
			Settings.Add(s);
		}

		public void Register(string soundName, AudioSource source, SoundType type)
		{
			Sound sound = new Sound {SoundName = soundName, Type = type, AudioClip = source.clip};
			sound.Volume                 = sound.Volume;
			sound.PlayOnAwake            = source.playOnAwake;
			sound.Source                 = source;
			sound.Loop                   = source.loop;
			source.outputAudioMixerGroup = SourceOutputAudioMixerGroup(type);
			Settings.Add(sound);
		}

		public AudioMixerGroup SourceOutputAudioMixerGroup(SoundType type)
		{
			return Settings.GetAudioMixerGroup(type);
		}

		public void Unregister(string soundName)
		{
			Settings.Remove(Settings.Find(soundName));
		}

		public void Play(string nameOfSound)
		{
			Sound s = Settings.Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			s.Source.Play();
		}

		public void Play(string nameOfSound, Vector3 position)
		{
			Sound s = Settings.Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			s.Source.Play();
			s.Source.transform.position = position;
		}

		public void SetMute(bool value)
		{
			Settings.isMute = value;
		}

		public void Stop(string nameOfSound)
		{
			Sound s = Settings.Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			if (s.Source.isPlaying)
				s.Source.Stop();
		}

	}


}