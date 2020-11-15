using GameDevUtils;
using UnityEngine;
using UnityEngine.Audio;


namespace GameDevUtils.SoundSystem
{


	public class SoundManager : SingletonLocal<SoundManager>
	{

		[SerializeField] SoundSettings Settings;


		protected override void Awake()
		{
			base.Awake();
			Settings.Initialize(this);
		}

		public void Register(Sound sound)
		{
			GameObject g = new GameObject(sound.name, typeof(AudioSource));
			g.transform.SetParent(transform, false);
			sound.source                       = g.GetComponent<AudioSource>();
			sound.source.clip                  = (sound.playRandomClip) ? sound.audioClips[Random.Range(0, sound.audioClips.Length)] : sound.audioClip;
			sound.source.volume                = sound.volume;
			sound.source.pitch                 = sound.pitch;
			sound.source.loop                  = sound.loop;
			sound.source.outputAudioMixerGroup = Settings.GetAudioMixerGroup(sound.type);
			if (sound.playOnAwake) sound.source.Play();
			Settings.Add(sound);
		}

		public void Register(string soundName, AudioSource source, SoundType type)
		{
			Sound sound = new Sound {name = soundName, type = type, audioClip = source.clip};
			sound.volume                 = sound.volume;
			sound.playOnAwake            = source.playOnAwake;
			sound.source                 = source;
			sound.loop                   = source.loop;
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

			s.source.Play();
		}

		public void Play(string nameOfSound, Vector3 position)
		{
			Sound s = Settings.Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			s.source.Play();
			s.source.transform.position = position;
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

			if (s.source.isPlaying)
				s.source.Stop();
		}

	}


}