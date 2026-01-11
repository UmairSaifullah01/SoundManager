using UnityEngine;
using System;
using System.Collections.Generic;
using THEBADDEST.Tasks;


namespace THEBADDEST.SoundSystem
{
	/// <summary>
	/// Extension methods for sound-related functionality
	/// </summary>
	public static class SoundExtensions
	{
		private static SoundSettings CachedSettings;

		/// <summary>
		/// Gets or loads the sound settings
		/// </summary>
		private static SoundSettings Settings
		{
			get
			{
				if (CachedSettings == null)
				{
					CachedSettings = Resources.Load<SoundSettings>("SoundSettings");
					if (CachedSettings == null)
					{
						Debug.LogError("SoundSettings not found in Resources folder!");
					}
				}
				return CachedSettings;
			}
		}

		/// <summary>
		/// Creates an AudioSource for a Sound object
		/// </summary>
		public static void CreateAudioSource(this Sound sound, Transform parent)
		{
			if (sound == null)
			{
				throw new ArgumentNullException(nameof(sound));
			}

			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}

			if (string.IsNullOrEmpty(sound.SoundName))
			{
				throw new ArgumentException("Sound name cannot be empty", nameof(sound));
			}

			GameObject obj = new GameObject(sound.SoundName, typeof(AudioSource));
			obj.transform.SetParent(parent, false);
			sound.Source = ConfigureAudioSource(obj.GetComponent<AudioSource>(), sound.Type);
		}

		/// <summary>
		/// Creates an AudioSource with default SFX type
		/// </summary>
		public static AudioSource CreateAudioSource(string name, Transform parent)
		{
			return CreateAudioSource(name, parent, SoundType.SFX);
		}

		/// <summary>
		/// Creates an AudioSource with specified type
		/// </summary>
		public static AudioSource CreateAudioSource(string name, Transform parent, SoundType type)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be empty", nameof(name));
			}

			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}

			GameObject obj = new GameObject(name, typeof(AudioSource));
			obj.transform.SetParent(parent, false);
			return ConfigureAudioSource(obj.GetComponent<AudioSource>(), type);
		}

		/// <summary>
		/// Creates an AudioSource with a specific clip
		/// </summary>
		public static AudioSource CreateAudioSource(string name, AudioClip clip, Transform parent, SoundType type = SoundType.SFX)
		{
			if (clip == null)
			{
				throw new ArgumentNullException(nameof(clip));
			}

			var source = CreateAudioSource(name, parent, type);
			source.clip = clip;
			return source;
		}

		/// <summary>
		/// Plays an AudioClip at the specified position
		/// </summary>
		public static AudioSource Play(this AudioClip clip, Transform position, float volume = 1f)
		{
			if (clip == null)
			{
				throw new ArgumentNullException(nameof(clip));
			}

			var source = CreateAudioSource(clip.name, clip, position);
			source.volume = volume;
			source.Play();
			return source;
		}

		/// <summary>
		/// Plays an AudioClip with specified type and settings
		/// </summary>
		public static AudioSource Play(this AudioClip clip, SoundType type, Transform position, float volume = 1f, float pitch = 1f, bool loop = false)
		{
			if (clip == null)
			{
				throw new ArgumentNullException(nameof(clip));
			}

			var source = CreateAudioSource(clip.name, position, type);
			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;
			source.loop = loop;
			source.Play();
			return source;
		}

		/// <summary>
		/// Plays an AudioClip as a one-shot sound
		/// </summary>
		public static void PlayOneShot(this AudioClip clip, Transform position, float volume = 1f)
		{
			if (clip == null)
			{
				throw new ArgumentNullException(nameof(clip));
			}

			var source = CreateAudioSource(clip.name, clip, position);
			source.volume = volume;
			source.PlayOneShot(clip);
			UnityEngine.Object.Destroy(source.gameObject, clip.length);
		}

		/// <summary>
		/// Plays a 3D spatialized sound
		/// </summary>
		public static AudioSource PlaySpatialized(this AudioClip clip, Vector3 position, float minDistance = 1f, float maxDistance = 50f, float volume = 1f)
		{
			if (clip == null)
			{
				throw new ArgumentNullException(nameof(clip));
			}

			var source = CreateAudioSource(clip.name, null, null);
			source.clip = clip;
			source.volume = volume;
			source.spatialBlend = 1f; // Full 3D
			source.rolloffMode = AudioRolloffMode.Linear;
			source.minDistance = minDistance;
			source.maxDistance = maxDistance;
			source.transform.position = position;
			source.Play();

			return source;
		}

		/// <summary>
		/// Fades in an AudioSource over time
		/// </summary>
		public static void FadeIn(this AudioSource source, float duration, float targetVolume = 1f)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			source.volume = 0f;
			source.Play();
			FadeCoroutine(source, 0f, targetVolume, duration);
		}

		/// <summary>
		/// Fades out an AudioSource over time
		/// </summary>
		public static void FadeOut(this AudioSource source, float duration, bool destroyWhenDone = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			float startVolume = source.volume;
			FadeCoroutine(source, startVolume, 0f, duration, destroyWhenDone);
		}

		private static async void FadeCoroutine(AudioSource source, float startVolume, float endVolume, float duration, bool destroyWhenDone = false)
		{
			float elapsed = 0f;
			while (elapsed < duration && source != null)
			{
				elapsed += Time.deltaTime;
				float normalizedTime = elapsed / duration;
				source.volume = Mathf.Lerp(startVolume, endVolume, normalizedTime);
				// frame delay
				await UTask.NextFrame();
			}

			if (source != null)
			{
				source.volume = endVolume;
				if (destroyWhenDone && endVolume <= 0f)
				{
					UnityEngine.Object.Destroy(source.gameObject);
				}
			}
		}

		private static AudioSource ConfigureAudioSource(AudioSource source, SoundType type)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			source.playOnAwake = false;
			source.outputAudioMixerGroup = Settings?.GetAudioMixerGroup(type);
			return source;
		}

		#region SoundService ID System Extensions

		/// <summary>
		/// Plays a sound using SoundService and returns the sound ID
		/// </summary>
		public static int PlayWithId(this SoundService soundService, string soundName)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.Play(soundName);
		}

		/// <summary>
		/// Plays a sound at a specific position using SoundService and returns the sound ID
		/// </summary>
		public static int PlayWithId(this SoundService soundService, string soundName, Vector3 position)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.Play(soundName, position);
		}

		/// <summary>
		/// Plays a Sound object using SoundService and returns the sound ID
		/// </summary>
		public static int PlayWithId(this SoundService soundService, Sound sound)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.Play(sound);
		}

		/// <summary>
		/// Plays a one-shot sound using SoundService and returns the sound ID
		/// </summary>
		public static int PlayOneShot(this SoundService soundService, string soundName)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			Sound sound = soundService.Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return -1;
			}

			// Ensure it's non-looping for one-shot
			bool originalLoop = sound.Loop;
			sound.Loop = false;
			int soundId = soundService.Play(sound);
			sound.Loop = originalLoop; // Restore original loop setting

			return soundId;
		}

		/// <summary>
		/// Plays a one-shot sound at a specific position using SoundService and returns the sound ID
		/// </summary>
		public static int PlayOneShot(this SoundService soundService, string soundName, Vector3 position)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			Sound sound = soundService.Find(soundName);
			if (sound == null)
			{
				Debug.LogWarning($"Sound not found: {soundName}");
				return -1;
			}

			// Ensure it's non-looping for one-shot
			bool originalLoop = sound.Loop;
			sound.Loop = false;
			int soundId = soundService.Play(soundName, position);
			sound.Loop = originalLoop; // Restore original loop setting

			return soundId;
		}

		/// <summary>
		/// Plays a one-shot Sound object using SoundService and returns the sound ID
		/// </summary>
		public static int PlayOneShot(this SoundService soundService, Sound sound)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			if (sound == null)
			{
				Debug.LogWarning("Sound is null!");
				return -1;
			}

			// Ensure it's non-looping for one-shot
			bool originalLoop = sound.Loop;
			sound.Loop = false;
			int soundId = soundService.Play(sound);
			sound.Loop = originalLoop; // Restore original loop setting

			return soundId;
		}

		/// <summary>
		/// Stops a sound by ID using SoundService
		/// </summary>
		public static void StopById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.Stop(soundId);
		}

		/// <summary>
		/// Pauses a sound by ID using SoundService
		/// </summary>
		public static void PauseById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.Pause(soundId);
		}

		/// <summary>
		/// Resumes a paused sound by ID using SoundService
		/// </summary>
		public static void ResumeById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.Resume(soundId);
		}

		/// <summary>
		/// Pauses all sounds using SoundService
		/// </summary>
		public static void PauseAll(this SoundService soundService)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.PauseAll();
		}

		/// <summary>
		/// Resumes all paused sounds using SoundService
		/// </summary>
		public static void ResumeAll(this SoundService soundService)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.ResumeAll();
		}

		/// <summary>
		/// Pauses all instances of a sound by name using SoundService
		/// </summary>
		public static void Pause(this SoundService soundService, string soundName)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.Pause(soundName);
		}

		/// <summary>
		/// Resumes all instances of a sound by name using SoundService
		/// </summary>
		public static void Resume(this SoundService soundService, string soundName)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.Resume(soundName);
		}

		/// <summary>
		/// Checks if a sound is paused by ID
		/// </summary>
		public static bool IsPausedById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.IsPaused(soundId);
		}

		/// <summary>
		/// Checks if a sound is playing by ID
		/// </summary>
		public static bool IsPlayingById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.IsPlaying(soundId);
		}

		/// <summary>
		/// Gets the AudioSource by sound ID
		/// </summary>
		public static AudioSource GetAudioSourceById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.GetAudioSource(soundId);
		}

		/// <summary>
		/// Gets the Sound object by sound ID
		/// </summary>
		public static Sound GetSoundById(this SoundService soundService, int soundId)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			return soundService.GetSound(soundId);
		}

		/// <summary>
		/// Sets the volume of a sound by ID
		/// </summary>
		public static void SetVolumeById(this SoundService soundService, int soundId, float volume)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.SetVolume(soundId, volume);
		}

		/// <summary>
		/// Sets the pitch of a sound by ID
		/// </summary>
		public static void SetPitchById(this SoundService soundService, int soundId, float pitch)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}
			soundService.SetPitch(soundId, pitch);
		}

		/// <summary>
		/// Fades in a sound by ID over time
		/// </summary>
		public static void FadeInById(this SoundService soundService, int soundId, float duration, float targetVolume = 1f)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			AudioSource source = soundService.GetAudioSource(soundId);
			if (source != null)
			{
				source.FadeIn(duration, targetVolume);
			}
		}

		/// <summary>
		/// Fades out a sound by ID over time
		/// </summary>
		public static void FadeOutById(this SoundService soundService, int soundId, float duration, bool stopWhenDone = true)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			AudioSource source = soundService.GetAudioSource(soundId);
			if (source != null)
			{
				source.FadeOut(duration, false);
				if (stopWhenDone)
				{
					FadeOutAndStopCoroutine(soundService, soundId, duration);
				}
			}
		}

		private static async void FadeOutAndStopCoroutine(SoundService soundService, int soundId, float duration)
		{
			await UTask.Delay(duration);
			if (soundService != null && soundService.IsPlaying(soundId))
			{
				soundService.Stop(soundId);
			}
		}

		/// <summary>
		/// Plays a sound and fades it in, returning the sound ID
		/// </summary>
		public static int PlayWithFadeIn(this SoundService soundService, string soundName, float fadeDuration, float targetVolume = 1f)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			int soundId = soundService.Play(soundName);
			if (soundId != -1)
			{
				soundService.FadeInById(soundId, fadeDuration, targetVolume);
			}
			return soundId;
		}

		/// <summary>
		/// Plays a sound at position and fades it in, returning the sound ID
		/// </summary>
		public static int PlayWithFadeIn(this SoundService soundService, string soundName, Vector3 position, float fadeDuration, float targetVolume = 1f)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			int soundId = soundService.Play(soundName, position);
			if (soundId != -1)
			{
				soundService.FadeInById(soundId, fadeDuration, targetVolume);
			}
			return soundId;
		}

		/// <summary>
		/// Stops a sound with fade out
		/// </summary>
		public static void StopWithFadeOut(this SoundService soundService, int soundId, float fadeDuration)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			soundService.FadeOutById(soundId, fadeDuration, true);
		}

		/// <summary>
		/// Stops a sound by name with fade out (stops all instances)
		/// </summary>
		public static void StopWithFadeOut(this SoundService soundService, string soundName, float fadeDuration)
		{
			if (soundService == null)
			{
				throw new ArgumentNullException(nameof(soundService));
			}

			List<int> idsToFade = soundService.GetPlayingSoundIds(soundName);
			foreach (int id in idsToFade)
			{
				soundService.FadeOutById(id, fadeDuration, true);
			}
		}

		#endregion
	}
}