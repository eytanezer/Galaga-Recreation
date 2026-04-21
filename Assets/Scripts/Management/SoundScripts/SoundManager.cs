using System.Collections;
using UnityEngine;

namespace ManagmentScripts.SoundScripts
{
    /// <summary>
    /// singleton class to manage all sound effects and background music
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager >
    {
        [SerializeField] private AudioSource soundFXObject;
        [SerializeField] private AudioClip backgroundMusic;

        private AudioPool _audioPool;
        

        public void Start()
        {
            _audioPool = AudioPool.Instance;
            PlayBackgroundMusic();
        }
        
        /// <summary>
        /// begin playing background music
        /// </summary>
        private void PlayBackgroundMusic()
        {
            // get audio source from pool
            AudioSourcePoolable backgroundAudio = _audioPool.Get();
            AudioSource backgroundSource = backgroundAudio.Source;
            
            // inti background music
            backgroundSource.clip = backgroundMusic;
            backgroundSource.loop = true; 
            backgroundSource.spatialBlend = 0; // 0 = 2D (non-spatial)
            backgroundSource.volume = 0.3f;
        
            backgroundAudio.transform.SetParent(transform);
            backgroundSource.Play();
        }
        
        /// <summary>
        /// play a sound effect clip at a given transform location with specified volume
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="spawnTransform"></param>
        /// <param name="volume"></param>
        public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, float customLength = -1)
        {
            // spawn in gameObject
            AudioSourcePoolable audioSourcePoolable = _audioPool.Get();
            AudioSource source = audioSourcePoolable.Source;
            
            // set position
            audioSourcePoolable.transform.position = spawnTransform.position;
            
            // assign audioClip
            source.clip = clip;
            // assign volume
            source.volume = volume;
            // source.pitch = Random.Range(0.8f, 1.2f);
            source.pitch = 1;
            source.spatialBlend = 0;
            
            source.Play();
            
            //get length of clip
            float clipLength = (customLength > 0) ? customLength : source.clip.length;
            // return to pool after clip finished playing
            StartCoroutine(ReturnToPool(audioSourcePoolable, clipLength));
        }

        private IEnumerator ReturnToPool(AudioSourcePoolable audioSourcePoolable, float delay)
        {
            yield return new WaitForSeconds(delay);
            _audioPool.Return(audioSourcePoolable);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StopAllSounds()
        {
            AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource source in allAudioSources)
            {
                source.Stop();
            }
        }
    }
}