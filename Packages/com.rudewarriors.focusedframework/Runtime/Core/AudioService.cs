using UnityEngine;
using System.Collections.Generic;

namespace RudeWarriors.Framework.Core
{
    public interface IAudioService
    {
        void PlaySFX(AudioClip clip, float volume = 1f);
        void PlayMusic(AudioClip clip, float fadeTime = 1f);
        void StopMusic(float fadeTime = 1f);
        void SetGlobalVolume(float volume);
    }

    /// <summary>
    /// Centralized audio manager handling music and SFX playback.
    /// Register this with ServiceLocator during bootstrap.
    /// </summary>
    [RequireComponent(typeof(AudioListener))]
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;

        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;

        private readonly List<AudioSource> _activeSFX = new();

        private void Awake()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            if (sfxSourcePrefab == null)
            {
                sfxSourcePrefab = new GameObject("SFX Source").AddComponent<AudioSource>();
                sfxSourcePrefab.playOnAwake = false;
                sfxSourcePrefab.transform.SetParent(transform);
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (!clip) return;

            var sfx = Instantiate(sfxSourcePrefab, transform);
            sfx.clip = clip;
            sfx.volume = volume * masterVolume;
            sfx.Play();

            _activeSFX.Add(sfx);
            Destroy(sfx.gameObject, clip.length + 0.1f);
        }

        public void PlayMusic(AudioClip clip, float fadeTime = 1f)
        {
            if (!clip) return;
            StartCoroutine(FadeMusicIn(clip, fadeTime));
        }

        public void StopMusic(float fadeTime = 1f)
        {
            StartCoroutine(FadeMusicOut(fadeTime));
        }

        public void SetGlobalVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            musicSource.volume = masterVolume;
        }

        private System.Collections.IEnumerator FadeMusicIn(AudioClip clip, float fadeTime)
        {
            if (musicSource.isPlaying)
                yield return FadeMusicOut(fadeTime);

            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();

            float t = 0f;
            while (t < fadeTime)
            {
                musicSource.volume = Mathf.Lerp(0f, masterVolume, t / fadeTime);
                t += Time.deltaTime;
                yield return null;
            }
            musicSource.volume = masterVolume;
        }

        private System.Collections.IEnumerator FadeMusicOut(float fadeTime)
        {
            float startVol = musicSource.volume;
            float t = 0f;
            while (t < fadeTime)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
                t += Time.deltaTime;
                yield return null;
            }
            musicSource.Stop();
            musicSource.volume = masterVolume;
        }
    }
}
