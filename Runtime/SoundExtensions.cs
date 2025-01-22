using UnityEngine;


namespace THEBADDEST.SoundSystem
{


	public static class SoundExtensions
	{

		public static void CreateAudioSource(this Sound sound, Transform parent)
		{
			GameObject g = new GameObject(sound.SoundName, typeof(AudioSource));
			g.transform.SetParent(parent, false);
			sound.Source = g.GetComponent<AudioSource>();
		}

		public static AudioSource CreateAudioSource(string name, Transform parent)
		{
			return CreateAudioSource(name, parent, SoundType.SFX);
		}

		public static AudioSource CreateAudioSource(string name, Transform parent, SoundType type)
		{
			GameObject g = new GameObject(name, typeof(AudioSource));
			g.transform.SetParent(parent, false);
			var source   = g.GetComponent<AudioSource>();
			var settings = Resources.Load<SoundSettings>("SoundSettings");
			source.outputAudioMixerGroup = settings.GetAudioMixerGroup(type);
			return source;
		}

		public static AudioSource CreateAudioSource(string name, AudioClip clip, Transform parent)
		{
			var source = CreateAudioSource(name, parent);
			source.clip = clip;
			return source;
		}

		public static AudioSource Play(this AudioClip clip, Transform position)
		{
			var source = CreateAudioSource(clip.name, clip, position);
			source.Play();
			return source;
		}

		public static AudioSource Play(this AudioClip clip, SoundType type, Transform position)
		{
			var source = CreateAudioSource(clip.name, position, type);
			source.clip = clip;
			source.Play();
			return source;
		}

		public static void PlayOnShot(this AudioClip clip, Transform position)
		{
			var source = CreateAudioSource(clip.name, clip, position);
			source.PlayOneShot(clip);
			Object.Destroy(source.gameObject, clip.length);
		}

	}


}