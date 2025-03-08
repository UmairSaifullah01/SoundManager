using UnityEngine;
using UnityEngine.Pool;

namespace THEBADDEST.SoundSystem
{
    public class AudioSourcePool
    {
        private readonly Transform parent;
        private readonly IObjectPool<AudioSource> pool;
        private readonly int maxSize;

        public AudioSourcePool(Transform parent, int maxSize = 30)
        {
            this.parent = parent;
            this.maxSize = maxSize;

            // Create the object pool with Unity's built-in pooling system
            pool = new ObjectPool<AudioSource>(
                createFunc: CreateNewAudioSource,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: 1,
                maxSize: maxSize
            );
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject obj = new GameObject("PooledAudioSource", typeof(AudioSource));
            obj.transform.SetParent(parent, false);
            AudioSource source = obj.GetComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private void OnTakeFromPool(AudioSource source)
        {
            source.gameObject.SetActive(true);
            source.volume = 1f;
            source.pitch = 1f;
            source.loop = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = null;
            source.clip = null;
        }

        private void OnReturnToPool(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.outputAudioMixerGroup = null;
                source.gameObject.SetActive(false);
            }
        }

        private void OnDestroyPoolObject(AudioSource source)
        {
            if (source != null)
            {
                Object.Destroy(source.gameObject);
            }
        }

        public AudioSource Get()
        {
            return pool.Get();
        }

        public void Return(AudioSource source)
        {
            if (source != null)
            {
                pool.Release(source);
            }
        }

        public void ReturnWhenFinished(AudioSource source)
        {
            if (source == null) return;

            if (!source.isPlaying)
            {
                Return(source);
            }
            else
            {
                source.gameObject.AddComponent<AutoReturnToPool>().Initialize(this);
            }
        }
    }

    // Helper component to automatically return the AudioSource to pool when finished playing
    public class AutoReturnToPool : MonoBehaviour
    {
        private AudioSourcePool pool;
        private AudioSource audioSource;

        public void Initialize(AudioSourcePool pool)
        {
            this.pool = pool;
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(WaitForSoundToFinish());
        }

        private System.Collections.IEnumerator WaitForSoundToFinish()
        {
            while (audioSource != null && audioSource.isPlaying)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (pool != null && audioSource != null)
            {
                pool.Return(audioSource);
            }

            Destroy(this); // Remove this component when done
        }
    }
}