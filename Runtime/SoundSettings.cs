using UnityEngine;
using UnityEngine.Audio;


namespace THEBADDEST.SoundSystem
{
	
	public class SoundSettings : ScriptableObject
	{

		[SerializeField]                     AudioMixer audioMixer;
		[SerializeField] [Range(0.0f, 1.0f)] float      masterVolume = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float      sfxVolume    = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float      uiVolume     = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float      musicVolume  = 1;
		[SerializeField]                     bool       mute         = false;

		public bool IsMute
		{
			get => mute;
			set
			{
				mute = value;
				audioMixer.SetFloat("volume", -80 + 80 * (mute ? 0 : masterVolume));
			}
		}
		public void UpdateVolume()
		{
			audioMixer.SetFloat("volume", -80 + 80 * masterVolume);
			audioMixer.SetFloat("sfx",    -80 + 80 * sfxVolume);
			audioMixer.SetFloat("ui",     -80 + 80 * uiVolume);
			audioMixer.SetFloat("music",  -80 + 80 * musicVolume);
		}
		public void SetMasterVolume(float value)
		{
			masterVolume = value;
			audioMixer.SetFloat("volume", -80 + 80 * masterVolume);
		}
		
		public void SetSFXVolume(float value)
		{
			sfxVolume = value;
			audioMixer.SetFloat("sfx",    -80 + 80 * sfxVolume);
		}
		public void SetUIVolume(float value)  {
			uiVolume = value;
			audioMixer.SetFloat("ui",     -80 + 80 * uiVolume);
		}
		public void SetMusicVolume(float value)  {
			musicVolume = value;
			audioMixer.SetFloat("music",  -80 + 80 * musicVolume);
		}
		public AudioMixerGroup GetAudioMixerGroup(SoundType type)
		{
			switch (type)
			{
				case SoundType.SFX:
					return audioMixer.FindMatchingGroups("SFX")[0];

				case SoundType.UI:
					return audioMixer.FindMatchingGroups("UI")[0];

				case SoundType.Music:
					return audioMixer.FindMatchingGroups("Music")[0];

				default:
					return audioMixer.FindMatchingGroups("SFX")[0];
			}
		}
	}


}