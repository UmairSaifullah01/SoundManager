using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THEBADDEST.SoundSystem
{
	[CreateAssetMenu(fileName = "SoundService", menuName = "THEBADDEST/SoundSystem/SoundService", order = 0)]
	public class SoundService : ScriptableObject, IList<Sound>
	{
		[SerializeField] private List<Sound> sounds = new List<Sound>();
		[SerializeField] private int maxConcurrentSounds = 30;

		public int Count => sounds.Count;
		public bool IsReadOnly => false;

		private Transform parent;
		private SoundSettings cacheSettings;
		private AudioSourcePool audioSourcePool;
		private Dictionary<string, Sound> soundDictionary;
		private HashSet<Sound> playingLoopSounds;
		private bool isInitialized;

		public SoundSettings Settings
		{
			get
			{
				if (!cacheSettings) cacheSettings = Resources.Load<SoundSettings>("SoundSettings");
				if (!cacheSettings)
				{
					Debug.LogError("SoundSettings not found in Resources folder!");
					return null;
				}
				return cacheSettings;
			}
		}

		public void Initialize()
		{
			parent = new GameObject("GameSounds").transform;
			DontDestroyOnLoad(parent.gameObject);

			audioSourcePool = new AudioSourcePool(parent, maxConcurrentSounds);
			soundDictionary = new Dictionary<string, Sound>(sounds.Count);
			playingLoopSounds = new HashSet<Sound>();

			// Build dictionary for faster lookups
			foreach (Sound sound in sounds)
			{
				if (string.IsNullOrEmpty(sound.SoundName))
				{
					Debug.LogError($"Sound name cannot be empty! Skipping sound with clip: {sound.AudioClip}");
					continue;
				}

				if (soundDictionary.ContainsKey(sound.SoundName))
				{
					Debug.LogError($"Duplicate sound name found: {sound.SoundName}. Sound names must be unique!");
					continue;
				}

				soundDictionary.Add(sound.SoundName, sound);

				if (sound.PlayOnAwake)
				{
					PlaySound(sound);
				}
			}

			isInitialized = true;
		}

		private void PlaySound(Sound sound)
		{
			if (!isInitialized)
			{
				Debug.LogError("SoundService is not initialized! Call Initialize() first.");
				return;
			}

			if (sound == null) return;

			AudioSource source = audioSourcePool.Get();
			if (source == null)
			{
				Debug.LogWarning("No available audio sources in pool. Maximum concurrent sounds reached.");
				return;
			}

			ConfigureAudioSource(sound, source);
			source.Play();

			if (sound.Loop)
			{
				playingLoopSounds.Add(sound);
			}
			else
			{
				audioSourcePool.ReturnWhenFinished(source);
			}
		}

		private void ConfigureAudioSource(Sound sound, AudioSource source)
		{
			source.gameObject.name = sound.SoundName;
			sound.Source = source;

			// Set the audio clip (handle random clip selection)
			if (sound.PlayRandomClip && sound.AudioClips != null && sound.AudioClips.Length > 0)
			{
				source.clip = sound.AudioClips[Random.Range(0, sound.AudioClips.Length)];
			}
			else
			{
				source.clip = sound.AudioClip;
			}

			if (source.clip == null)
			{
				Debug.LogError($"No audio clip assigned for sound: {sound.SoundName}");
				return;
			}

			// Configure audio source properties
			source.volume = sound.Volume;
			source.pitch = sound.Pitch;
			source.loop = sound.Loop;
			source.outputAudioMixerGroup = Settings?.GetAudioMixerGroup(sound.Type);
		}

		public void Play(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return;
			}

			PlaySound(sound);
		}

		public void Play(string soundName, Vector3 position)
		{
			Sound sound = Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return;
			}

			AudioSource source = audioSourcePool.Get();
			if (source == null) return;

			ConfigureAudioSource(sound, source);
			source.transform.position = position;
			source.spatialBlend = 1f;
			source.Play();

			if (sound.Loop)
			{
				playingLoopSounds.Add(sound);
			}
			else
			{
				audioSourcePool.ReturnWhenFinished(source);
			}
		}

		public void Stop(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null || sound.Source == null) return;

			sound.Source.Stop();
			audioSourcePool.Return(sound.Source);
			playingLoopSounds.Remove(sound);
			sound.Source = null;
		}

		public void StopAllSounds()
		{
			foreach (Sound sound in playingLoopSounds)
			{
				if (sound.Source != null)
				{
					sound.Source.Stop();
					audioSourcePool.Return(sound.Source);
					sound.Source = null;
				}
			}
			playingLoopSounds.Clear();
		}

		public void SetVolume(string soundName, float volume)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			sound.Volume = volume;
			if (sound.Source != null)
			{
				sound.Source.volume = volume;
			}
		}

		public void SetPitch(string soundName, float pitch)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			sound.Pitch = pitch;
			if (sound.Source != null)
			{
				sound.Source.pitch = pitch;
			}
		}

		public void SetMute(bool value)
		{
			if (Settings != null)
			{
				Settings.IsMute = value;
			}
		}

		public void SetMute(SoundType type, bool value)
		{
			if (Settings == null) return;

			switch (type)
			{
				case SoundType.SFX:
					Settings.SetSFXVolume(value ? 0 : 1);
					break;
				case SoundType.UI:
					Settings.SetUIVolume(value ? 0 : 1);
					break;
				case SoundType.Music:
					Settings.SetMusicVolume(value ? 0 : 1);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public Sound Find(string soundName)
		{
			if (string.IsNullOrEmpty(soundName))
			{
				Debug.LogError("Sound name cannot be null or empty!");
				return null;
			}

			return soundDictionary.TryGetValue(soundName, out Sound sound) ? sound : null;
		}

		public void OnDisable()
		{
			if (parent != null)
			{
				StopAllSounds();
				Destroy(parent.gameObject);
			}
		}

		#region IList Implementation
		public Sound this[int index]
		{
			get => sounds[index];
			set => sounds[index] = value;
		}

		public void Add(Sound sound)
		{
			if (sound == null || string.IsNullOrEmpty(sound.SoundName))
			{
				Debug.LogError("Cannot add null sound or sound with empty name!");
				return;
			}

			if (isInitialized && soundDictionary.ContainsKey(sound.SoundName))
			{
				Debug.LogError($"Sound with name {sound.SoundName} already exists!");
				return;
			}

			sounds.Add(sound);
			if (isInitialized)
			{
				soundDictionary.Add(sound.SoundName, sound);
			}
		}

		public void Clear()
		{
			StopAllSounds();
			sounds.Clear();
			soundDictionary?.Clear();
		}

		public bool Contains(Sound sound) => sounds.Contains(sound);
		public void CopyTo(Sound[] array, int arrayIndex) => sounds.CopyTo(array, arrayIndex);
		public int IndexOf(Sound sound) => sounds.IndexOf(sound);
		public void Insert(int index, Sound sound) => sounds.Insert(index, sound);
		public bool Remove(Sound sound)
		{
			if (sound != null)
			{
				Stop(sound.SoundName);
				soundDictionary?.Remove(sound.SoundName);
				return sounds.Remove(sound);
			}
			return false;
		}
		public void RemoveAt(int index)
		{
			if (index >= 0 && index < sounds.Count)
			{
				Sound sound = sounds[index];
				Stop(sound.SoundName);
				soundDictionary?.Remove(sound.SoundName);
				sounds.RemoveAt(index);
			}
		}
		public IEnumerator<Sound> GetEnumerator() => sounds.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion
	}
}