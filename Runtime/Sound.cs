using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace THEBADDEST.SoundSystem
{


	[Serializable]
	public class Sound
	{
		[SerializeField]private string soundName;
		public string SoundName
		{
			get => soundName;
			set => soundName = value;
		}

		[SerializeField]private SoundType type = SoundType.SFX;
		public SoundType Type
		{
			get => type;
			set => type = value;
		}

		[SerializeField]private AudioClip audioClip;
		public AudioClip AudioClip
		{
			get => audioClip;
			set => audioClip = value;
		}

		[SerializeField]private AudioClip[] audioClips;
		public AudioClip[] AudioClips
		{
			get => audioClips;
			set => audioClips = value;
		}

		[SerializeField]private bool loop = false;
		public bool Loop
		{
			get => loop;
			set => loop = value;
		}

		[SerializeField]private bool playRandomClip = false;
		public bool PlayRandomClip
		{
			get => playRandomClip;
			set => playRandomClip = value;
		}

		[SerializeField]private bool playOnAwake = false;
		public bool PlayOnAwake
		{
			get => playOnAwake;
			set => playOnAwake = value;
		}

		[SerializeField, Range(0, 1)] private float volume = 1;
		public float Volume
		{
			get => volume;
			set => volume = Mathf.Clamp01(value);
		}

		[SerializeField, Range(0.1f, 3)] private float pitch = 1;
		public float Pitch
		{
			get => pitch;
			set => pitch = Mathf.Clamp(value, 0.1f, 3);
		}

		[HideInInspector] private AudioSource source;
		public AudioSource Source
		{
			get => source;
			set => source = value;
		}
	}
	public enum SoundType
	{

		SFX,
		UI,
		Music

	}


}