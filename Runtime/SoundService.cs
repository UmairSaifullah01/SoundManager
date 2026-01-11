using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THEBADDEST.SoundSystem
{
	// Helper class to track sound instance data
	public class SoundInstance
	{
		public Sound Sound { get; set; }
		public AudioSource AudioSource { get; set; }
		
		public SoundInstance(Sound sound, AudioSource audioSource)
		{
			Sound = sound;
			AudioSource = audioSource;
		}
	}

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
		
		// ID System
		private Dictionary<int, SoundInstance> soundInstances = new Dictionary<int, SoundInstance>();
		private int nextSoundId = 1;
		private HashSet<int> pausedSounds = new HashSet<int>();

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
			soundInstances = new Dictionary<int, SoundInstance>();
			pausedSounds = new HashSet<int>();
			nextSoundId = 1;
			isInitialized = true;
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

			
		}

		private int GetNextSoundId()
		{
			// Find next available ID (in case of wraparound)
			while (soundInstances.ContainsKey(nextSoundId))
			{
				nextSoundId++;
				if (nextSoundId < 0) nextSoundId = 1; // Reset if overflow
			}
			return nextSoundId++;
		}

		private int PlaySound(Sound sound)
		{
			if (!isInitialized)
			{
				Debug.LogError("SoundService is not initialized! Call Initialize() first.");
				return -1;
			}

			if (sound == null) return -1;

			AudioSource source = audioSourcePool.Get();
			if (source == null)
			{
				Debug.LogWarning("No available audio sources in pool. Maximum concurrent sounds reached.");
				return -1;
			}

			ConfigureAudioSource(sound, source);
			source.Play();

			// Generate unique ID and track this instance
			int soundId = GetNextSoundId();
			soundInstances[soundId] = new SoundInstance(sound, source);

			if (sound.Loop)
			{
				playingLoopSounds.Add(sound);
			}
			else
			{
				audioSourcePool.ReturnWhenFinished(source);
			}

			return soundId;
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

		public int Play(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return -1;
			}

			return PlaySound(sound);
		}

		public int Play(string soundName, Vector3 position)
		{
			Sound sound = Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return -1;
			}

			AudioSource source = audioSourcePool.Get();
			if (source == null) return -1;

			ConfigureAudioSource(sound, source);
			source.transform.position = position;
			source.spatialBlend = 1f;
			source.Play();

			// Generate unique ID and track this instance
			int soundId = GetNextSoundId();
			soundInstances[soundId] = new SoundInstance(sound, source);

			if (sound.Loop)
			{
				playingLoopSounds.Add(sound);
			}
			else
			{
				audioSourcePool.ReturnWhenFinished(source);
			}

			return soundId;
		}

		public int Play(Sound sound)
		{
			if (!isInitialized)
			{
				Debug.LogError("SoundService is not initialized! Call Initialize() first.");
				return -1;
			}

			if (sound == null) return -1;

			AudioSource source = audioSourcePool.Get();
			if (source == null)
			{
				Debug.LogWarning("No available audio sources in pool. Maximum concurrent sounds reached.");
				return -1;
			}

			ConfigureAudioSource(sound, source);
			source.Play();

			// Generate unique ID and track this instance
			int soundId = GetNextSoundId();
			soundInstances[soundId] = new SoundInstance(sound, source);

			if (sound.Loop)
			{
				playingLoopSounds.Add(sound);
			}
			else
			{
				audioSourcePool.ReturnWhenFinished(source);
			}

			return soundId;
		}

		public void Stop(int soundId)
		{
			if (!soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				Debug.LogWarning($"Sound instance with ID {soundId} not found!");
				return;
			}

			if (instance.AudioSource != null)
			{
				if (instance.AudioSource.isPlaying || pausedSounds.Contains(soundId))
				{
					instance.AudioSource.Stop();
				}
				audioSourcePool.Return(instance.AudioSource);
			}

			// Clean up tracking
			playingLoopSounds.Remove(instance.Sound);
			pausedSounds.Remove(soundId);
			if (instance.Sound != null && instance.Sound.Source == instance.AudioSource)
			{
				instance.Sound.Source = null;
			}
			soundInstances.Remove(soundId);
		}

		public void Stop(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			// Find all instances of this sound and stop them
			List<int> idsToRemove = new List<int>();
			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound)
				{
					if (kvp.Value.AudioSource != null)
					{
						if (kvp.Value.AudioSource.isPlaying || pausedSounds.Contains(kvp.Key))
						{
							kvp.Value.AudioSource.Stop();
						}
						audioSourcePool.Return(kvp.Value.AudioSource);
					}
					pausedSounds.Remove(kvp.Key);
					idsToRemove.Add(kvp.Key);
				}
			}

			foreach (int id in idsToRemove)
			{
				soundInstances.Remove(id);
			}

			playingLoopSounds.Remove(sound);
			sound.Source = null;
		}

		public void StopAllSounds()
		{
			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.AudioSource != null && kvp.Value.AudioSource.isPlaying)
				{
					kvp.Value.AudioSource.Stop();
					audioSourcePool.Return(kvp.Value.AudioSource);
				}
				if (kvp.Value.Sound != null)
				{
					kvp.Value.Sound.Source = null;
				}
			}
			soundInstances.Clear();
			playingLoopSounds.Clear();
			pausedSounds.Clear();
		}

		// Pause a specific sound by ID
		public void Pause(int soundId)
		{
			if (!soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				Debug.LogWarning($"Sound instance with ID {soundId} not found!");
				return;
			}

			if (instance.AudioSource != null && instance.AudioSource.isPlaying)
			{
				instance.AudioSource.Pause();
				pausedSounds.Add(soundId);
			}
		}

		// Resume a paused sound by ID
		public void Resume(int soundId)
		{
			if (!soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				Debug.LogWarning($"Sound instance with ID {soundId} not found!");
				return;
			}

			if (instance.AudioSource != null && pausedSounds.Contains(soundId))
			{
				instance.AudioSource.UnPause();
				pausedSounds.Remove(soundId);
			}
		}

		// Pause all sounds
		public void PauseAll()
		{
			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.AudioSource != null && kvp.Value.AudioSource.isPlaying)
				{
					kvp.Value.AudioSource.Pause();
					pausedSounds.Add(kvp.Key);
				}
			}
		}

		// Resume all paused sounds
		public void ResumeAll()
		{
			List<int> pausedIds = new List<int>(pausedSounds);
			foreach (int soundId in pausedIds)
			{
				if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
				{
					if (instance.AudioSource != null)
					{
						instance.AudioSource.UnPause();
					}
				}
			}
			pausedSounds.Clear();
		}

		// Pause all instances of a sound by name
		public void Pause(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound && 
				    kvp.Value.AudioSource != null && 
				    kvp.Value.AudioSource.isPlaying)
				{
					kvp.Value.AudioSource.Pause();
					pausedSounds.Add(kvp.Key);
				}
			}
		}

		// Resume all instances of a sound by name
		public void Resume(string soundName)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound && 
				    kvp.Value.AudioSource != null && 
				    pausedSounds.Contains(kvp.Key))
				{
					kvp.Value.AudioSource.UnPause();
					pausedSounds.Remove(kvp.Key);
				}
			}
		}

		// Check if a sound is paused
		public bool IsPaused(int soundId)
		{
			return pausedSounds.Contains(soundId);
		}

		// Get Sound by ID
		public Sound GetSound(int soundId)
		{
			if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				return instance.Sound;
			}
			return null;
		}

		// Get AudioSource by ID
		public AudioSource GetAudioSource(int soundId)
		{
			if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				return instance.AudioSource;
			}
			return null;
		}

		// Check if sound is still playing (automatically cleans up if finished)
		public bool IsPlaying(int soundId)
		{
			if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				bool isPlaying = instance.AudioSource != null && instance.AudioSource.isPlaying;
				
				// Auto-cleanup if non-looping sound has finished
				if (!isPlaying && instance.Sound != null && !instance.Sound.Loop)
				{
					soundInstances.Remove(soundId);
				}
				
				return isPlaying;
			}
			return false;
		}

		// Get all playing sound IDs for a specific sound name
		public List<int> GetPlayingSoundIds(string soundName)
		{
			List<int> ids = new List<int>();
			Sound sound = Find(soundName);
			if (sound == null) return ids;

			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound && 
				    kvp.Value.AudioSource != null && 
				    kvp.Value.AudioSource.isPlaying)
				{
					ids.Add(kvp.Key);
				}
			}
			return ids;
		}

		// Clean up finished non-looping sounds from tracking
		public void CleanupFinishedSounds()
		{
			List<int> idsToRemove = new List<int>();
			foreach (var kvp in soundInstances)
			{
				// Remove if AudioSource is null, not playing, or was returned to pool
				if (kvp.Value.AudioSource == null || 
				    !kvp.Value.AudioSource.isPlaying || 
				    !kvp.Value.AudioSource.gameObject.activeInHierarchy)
				{
					// Only remove non-looping sounds that have finished
					if (kvp.Value.Sound != null && !kvp.Value.Sound.Loop)
					{
						idsToRemove.Add(kvp.Key);
					}
				}
			}

			foreach (int id in idsToRemove)
			{
				soundInstances.Remove(id);
			}
		}

		public void SetVolume(string soundName, float volume)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			sound.Volume = volume;
			// Update all playing instances of this sound
			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound && kvp.Value.AudioSource != null)
				{
					kvp.Value.AudioSource.volume = volume;
				}
			}
		}

		// Set volume for specific sound instance by ID
		public void SetVolume(int soundId, float volume)
		{
			if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				if (instance.Sound != null)
				{
					instance.Sound.Volume = volume;
				}
				if (instance.AudioSource != null)
				{
					instance.AudioSource.volume = volume;
				}
			}
		}

		public void SetPitch(string soundName, float pitch)
		{
			Sound sound = Find(soundName);
			if (sound == null) return;

			sound.Pitch = pitch;
			// Update all playing instances of this sound
			foreach (var kvp in soundInstances)
			{
				if (kvp.Value.Sound == sound && kvp.Value.AudioSource != null)
				{
					kvp.Value.AudioSource.pitch = pitch;
				}
			}
		}

		// Set pitch for specific sound instance by ID
		public void SetPitch(int soundId, float pitch)
		{
			if (soundInstances.TryGetValue(soundId, out SoundInstance instance))
			{
				if (instance.Sound != null)
				{
					instance.Sound.Pitch = pitch;
				}
				if (instance.AudioSource != null)
				{
					instance.AudioSource.pitch = pitch;
				}
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

		public void SetVolume(SoundType type, float volume)
		{
			if (Settings == null) return;

			switch (type)
			{
				case SoundType.SFX:
					Settings.SetSFXVolume(volume);
					break;
				case SoundType.UI:
					Settings.SetUIVolume(volume);
					break;
				case SoundType.Music:
					Settings.SetMusicVolume(volume);
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