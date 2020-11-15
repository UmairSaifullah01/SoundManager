using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace GameDevUtils.SoundSystem
{


	[CreateAssetMenu]
	public class SoundSettings : ScriptableObject, IList<Sound>
	{

		[SerializeField]                     AudioMixer  audioMixer;
		[SerializeField] [Range(0.0f, 1.0f)] float       masterVolume = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float       sfxVolume    = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float       uiVolume     = 1;
		[SerializeField] [Range(0.0f, 1.0f)] float       musicVolume  = 1;
		[SerializeField]                     bool        mute         = false;
		[SerializeField]                     List<Sound> Sounds;

		public int  Count      => Sounds.Count;
		public bool IsReadOnly => true;
		public bool isMute
		{
			get => mute;
			set
			{
				mute = value;
				audioMixer.SetFloat("volume", -80 + 80 * (mute ? 0 : masterVolume));
			}
		}

		public void Initialize(SoundManager manager)
		{
			foreach (Sound s in Sounds)
			{
				GameObject g = new GameObject(s.name, typeof(AudioSource));
				g.transform.SetParent(manager.transform, false);
				s.source                       = g.GetComponent<AudioSource>();
				s.source.clip                  = (s.playRandomClip) ? s.audioClips[Random.Range(0, s.audioClips.Length)] : s.audioClip;
				s.source.volume                = s.volume;
				s.source.pitch                 = s.pitch;
				s.source.loop                  = s.loop;
				s.source.outputAudioMixerGroup = GetAudioMixerGroup(s.type);
				if (s.playOnAwake) s.source.Play();
			}

			UpdateVolume(manager);
		}

		public void UpdateVolume(SoundManager manager)
		{
			manager.StartCoroutine(SetVolumes());
		}

		IEnumerator SetVolumes()
		{
			yield return new WaitForFixedUpdate();
			SetVolume(masterVolume, sfxVolume, uiVolume, musicVolume);
		}

		public void SetVolume(float master = -1, float sfx = -1, float ui = -1, float music = -1)
		{
			if (master != -1)
				audioMixer.SetFloat("volume", -80 + 80 * master);
			if (sfx   != -1) audioMixer.SetFloat("sfx",   -80 + 80 * sfx);
			if (ui    != -1) audioMixer.SetFloat("ui",    -80 + 80 * ui);
			if (music != -1) audioMixer.SetFloat("music", -80 + 80 * music);
		}

		public int IndexOf(Sound item)
		{
			return Sounds.IndexOf(item);
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

		public void Insert(int index, Sound item)
		{
			Sounds.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Sounds.RemoveAt(index);
		}

		public Sound Find(string soundName)
		{
			return Sounds.Find((s => s.name == soundName));
		}

		public Sound this[int index]
		{
			get => Sounds[index];
			set => Sounds[index] = value;
		}

		public IEnumerator<Sound> GetEnumerator()
		{
			return Sounds.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Sound item)
		{
			Sounds.Add(item);
		}

		public void Clear()
		{
			Sounds.Clear();
		}

		public bool Contains(Sound item)
		{
			return Sounds.Contains(item);
		}

		public void CopyTo(Sound[] array, int arrayIndex)
		{
			Sounds.CopyTo(array, arrayIndex);
		}

		public bool Remove(Sound item)
		{
			return Sounds.Remove(item);
		}

	}


}