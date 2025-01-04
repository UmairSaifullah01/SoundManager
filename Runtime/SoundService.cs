using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace THEBADDEST.SoundSystem
{


	[CreateAssetMenu]
	public class SoundService : ScriptableObject, IList<Sound>
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
				GameObject g = new GameObject(s.SoundName, typeof(AudioSource));
				g.transform.SetParent(manager.transform, false);
				s.Source                       = g.GetComponent<AudioSource>();
				s.Source.clip                  = (s.PlayRandomClip) ? s.AudioClips[Random.Range(0, s.AudioClips.Length)] : s.AudioClip;
				s.Source.volume                = s.Volume;
				s.Source.pitch                 = s.Pitch;
				s.Source.loop                  = s.Loop;
				s.Source.outputAudioMixerGroup = GetAudioMixerGroup(s.Type);
				if (s.PlayOnAwake) s.Source.Play();
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
			return Sounds.Find((s => s.SoundName == soundName));
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