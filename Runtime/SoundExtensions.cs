using UnityEngine;
using System;
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
	}
}