using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace PKDS.Managers
{
    /// <summary>
    /// Class <c>AudioManager</c> handles the audio logic.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the instance of the audio manager.</value>
        public static AudioManager Instance;
        
        #region Audio Properties
        
            /// <value>Property <c>audioSource</c> represents the audio source.</value>
            [FormerlySerializedAs("audioSource")]
            [Header("Audio Properties")]
            [SerializeField]
            private AudioSource musicAudioSource;
            
            /// <value>Property <c>sfxAudioSource</c> represents the sfx audio source.</value>
            [SerializeField]
            private AudioSource sfxAudioSource;
            
            /// <value>Property <c>backgroundMusic</c> represents the background music.</value>
            [SerializeField]
            private AudioClip backgroundMusic;
            
            /// <value>Property <c>fadeDuration</c> represents the fade duration.</value>
            [SerializeField]
            private float fadeDuration = 2.0f;
            
            /// <value>Property <c>musicBuffer</c> represents the music buffer.</value>
            [SerializeField]
            private float musicBuffer = 4.0f;

            /// <value>Property <c>MusicEnabled</c> represents if the music is enabled.</value>
            public bool MusicEnabled { get; set; } = true;
            
            /// <value>Property <c>SfxEnabled</c> represents if the SFX is enabled.</value>
            public bool SfxEnabled { get; set; } = true;    
        
        #endregion

        #region Unity Event Methods

            /// <summary>
            /// Method <c>Awake</c> initializes the audio manager.
            /// </summary>
            private void Awake()
            {
                // Singleton pattern
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
                Instance = this;
            }

        #endregion
        
        #region Audio Methods

            /// <summary>
            /// Method <c>PlayBackgroundMusic</c> plays the background music.
            /// </summary>
            /// <param name="fade">Whether to fade in the audio source.</param>
            public void PlayBackgroundMusic(bool fade = true)
            {
                if (!MusicEnabled)
                    return;
                musicAudioSource.clip = backgroundMusic;
                if (fade)
                    StartCoroutine(FadeIn());
                else
                    musicAudioSource.Play();
            }

            /// <summary>
            /// Method <c>PlayShortenedBackgroundMusic</c> plays the shortened background music.
            /// </summary>
            /// <param name="duration">The duration of the game.</param>
            public void PlayShortenedBackgroundMusic(float duration)
            {
                if (!MusicEnabled)
                    return;
                musicAudioSource.clip = backgroundMusic;
                var time = 0.0f;
                if (duration > 0.0f)
                {
                    time = musicAudioSource.clip.length - duration - musicBuffer;
                    time = Mathf.Clamp(time, 0.0f, musicAudioSource.clip.length);
                }
                musicAudioSource.time = time;
                if (musicAudioSource.time < musicAudioSource.clip.length)
                    StartCoroutine(FadeIn());
                else
                    musicAudioSource.Play();
            }

            /// <summary>
            /// Method <c>StopBackgroundMusic</c> stops the background music.
            /// </summary>
            /// <param name="fade">Whether to fade in the audio source.</param>
            public void StopBackgroundMusic(bool fade = true)
            {
                if (!MusicEnabled)
                    return;
                if (fade)
                    StartCoroutine(FadeOut());
                else
                    musicAudioSource.Stop();
            }
            
            /// <summary>
            /// Method <c>TogglePauseBackgroundMusic</c> toggles the pause of the background music.
            /// </summary>
            public void TogglePauseBackgroundMusic()
            {
                if (!MusicEnabled)
                    return;
                if (musicAudioSource.isPlaying)
                    musicAudioSource.Pause();
                else
                    musicAudioSource.Play();
            }
            
            /// <summary>
            /// Method <c>FadeIn</c> fades in the audio source.
            /// </summary>
            private IEnumerator FadeIn()
            {
                musicAudioSource.volume = 0.0f;
                musicAudioSource.Play();
                while (musicAudioSource.volume < 1.0f)
                {
                    musicAudioSource.volume += Time.deltaTime / fadeDuration;
                    yield return null;
                }
            }
            
            /// <summary>
            /// Method <c>FadeOut</c> fades out the audio source.
            /// </summary>
            private IEnumerator FadeOut()
            {
                while (musicAudioSource.volume > 0.0f)
                {
                    musicAudioSource.volume -= Time.deltaTime / fadeDuration;
                    yield return null;
                }
                musicAudioSource.Stop();
            }

            /// <summary>
            /// Method <c>PlaySfx</c> plays the sfx.
            /// </summary>
            /// <param name="clip">The audio clip.</param>
            public void PlaySfx(AudioClip clip)
            {
                if (!SfxEnabled)
                    return;
                sfxAudioSource.PlayOneShot(clip);
            }

        #endregion
    }
}
