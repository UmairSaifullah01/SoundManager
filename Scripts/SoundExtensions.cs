﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevUtils.SoundSystem
{


	public static class SoundExtensions
	{

		public static void CreateAudioSource(this Sound sound, Transform parent)
		{
			GameObject g = new GameObject(sound.name, typeof(AudioSource));
			g.transform.SetParent(parent, false);
			sound.source = g.GetComponent<AudioSource>();
		}

		public static AudioSource CreateAudioSource(string name, Transform parent)
		{
			GameObject g = new GameObject(name, typeof(AudioSource));
			g.transform.SetParent(parent, false);
			var source = g.GetComponent<AudioSource>();
			return source;
		}

		public static AudioSource CreateAudioSource(string name, Transform parent, SoundType type)
		{
			GameObject g = new GameObject(name, typeof(AudioSource));
			g.transform.SetParent(parent, false);
			var source = g.GetComponent<AudioSource>();
			source.outputAudioMixerGroup = SoundManager.Instance.SourceOutputAudioMixerGroup(type);
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
			source.Play();
			Object.Destroy(source.gameObject, clip.length);
		}

	}


}