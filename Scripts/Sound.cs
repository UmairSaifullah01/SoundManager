using System;
using UnityEngine.Audio;
using UnityEngine;


namespace GameDevUtils.SoundSystem
{


	[Serializable]
	public class Sound
	{

		public string      name;
		public SoundType   type = SoundType.SFX;
		public AudioClip   audioClip;
		public AudioClip[] audioClips;

		public                                bool  loop           = false;
		public                                bool  playRandomClip = false;
		public                                bool  playOnAwake    = false;
		[SerializeField, Range(0, 1)] private float m_volume       = 1;
		public float volume
		{
			get => m_volume;
			set => m_volume = Mathf.Clamp01(value);
		}

		[SerializeField, Range(0.1f, 3)] private float m_pitch = 1;
		public float pitch
		{
			get => m_pitch;
			set => m_pitch = Mathf.Clamp(value, 0.1f, 3);
		}
		[HideInInspector] public AudioSource source;

	}

	public enum SoundType
	{

		SFX,
		UI,
		Music

	}


}