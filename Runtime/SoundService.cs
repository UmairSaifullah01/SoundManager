using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace THEBADDEST.SoundSystem
{


	[CreateAssetMenu(fileName = "SoundService", menuName = "THEBADDEST/SoundSystem/SoundService", order = 0)]
	public class SoundService : ScriptableObject, IList<Sound>
	{

		[SerializeField] List<Sound> sounds;

		public int  Count      => sounds.Count;
		public bool IsReadOnly => true;

		private Transform     parent;
		private SoundSettings cacheSettings;
		public SoundSettings Settings
		{
			get
			{
				if (!cacheSettings) cacheSettings = Resources.Load<SoundSettings>("SoundSettings");
				return cacheSettings;
			}
		}

		public void Initialize()
		{
			parent = new GameObject("GameSounds").transform;
			foreach (Sound sound in sounds)
			{
				if (sound.PlayOnAwake)
				{
					InitializeSound(sound);
					sound.Source.Play();
				}
			}
		}

		void InitializeSound(Sound sound)
		{
			GameObject gameObject = new GameObject(sound.SoundName, typeof(AudioSource));
			gameObject.transform.SetParent(parent, false);
			sound.Source                       = gameObject.GetComponent<AudioSource>();
			sound.Source.clip                  = (sound.PlayRandomClip) ? sound.AudioClips[Random.Range(0, sound.AudioClips.Length)] : sound.AudioClip;
			sound.Source.volume                = sound.Volume;
			sound.Source.pitch                 = sound.Pitch;
			sound.Source.loop                  = sound.Loop;
			sound.Source.outputAudioMixerGroup = Settings.GetAudioMixerGroup(sound.Type);
		}

		public void Play(string nameOfSound)
		{
			Sound s = Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			if (s.Source == null) InitializeSound(s);
			s.Source.Play();
		}

		public void Play(string nameOfSound, Vector3 position)
		{
			Sound s = Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			if (s.Source == null) InitializeSound(s);
			s.Source.Play();
			s.Source.transform.position = position;
		}

		public void SetMute(bool value)
		{
			Settings.IsMute = value;
		}

		public void Stop(string nameOfSound)
		{
			Sound s = Find(nameOfSound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + nameOfSound + " not found!");
				return;
			}

			if (s.Source.isPlaying)
				s.Source.Stop();
		}

		public int IndexOf(Sound item)
		{
			return sounds.IndexOf(item);
		}


		public void Insert(int index, Sound item)
		{
			sounds.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			sounds.RemoveAt(index);
		}

		public Sound Find(string soundName)
		{
			return sounds.Find((s => s.SoundName == soundName));
		}

		public Sound this[int index]
		{
			get => sounds[index];
			set => sounds[index] = value;
		}

		public IEnumerator<Sound> GetEnumerator()
		{
			return sounds.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Sound item)
		{
			sounds.Add(item);
		}

		public void Clear()
		{
			sounds.Clear();
		}

		public bool Contains(Sound item)
		{
			return sounds.Contains(item);
		}

		public void CopyTo(Sound[] array, int arrayIndex)
		{
			sounds.CopyTo(array, arrayIndex);
		}

		public bool Remove(Sound item)
		{
			return sounds.Remove(item);
		}

	}


}