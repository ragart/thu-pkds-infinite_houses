using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PKDS.Managers
{
    /// <summary>
    /// Method <c>TransitionableSceneManager</c> contains the methods and properties needed for the to be continued sequence.
    /// </summary>
    public class TransitionableSceneManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the singleton instance of the class.</value>
        public static TransitionableSceneManager Instance;
        
        #region Delays

            /// <value>Property <c>initialDelay</c> represents the initial delay.</value>
            [Header("Delays")] public float initialDelay;
            
            /// <value>Property <c>startDelay</c> represents the start delay.</value>
            public float startDelay = 2.5f;
            
            /// <value>Property <c>finalDelay</c> represents the final delay.</value>
            public float finalDelay;
        
        #endregion
        
        #region Fade

            /// <value>Property <c>fadingTexts</c> represents the fadable texts.</value>
            [Header("Fade")]
            public TextMeshProUGUI[] fadingTexts;
            
            /// <value>Property <c>fadingImages</c> represents the fadable images.</value>
            public Image[] fadingImages;
            
            /// <value>Property <c>fadeDuration</c> represents the duration of the fade.</value>
            public float fadeDuration = 1.5f;
            
        #endregion
        
        #region Blink

            /// <value>Property <c>blinkingTexts</c> represents the blinking texts.</value>
            [Header("Blink")]
            public TextMeshProUGUI[] blinkingTexts;
            
            /// <value>Property <c>blinkRate</c> represents the rate of the blink.</value>
            public float blinkRate = 0.5f;
            
        #endregion
        
        #region Rotate

            /// <value>Property <c>rotatingObjects</c> represents the rotating objects.</value>
            [Header("Rotate")]
            public GameObject[] rotatingObjects;

            /// <value>Property <c>rotateClockwise</c> represents whether the object should rotate clockwise.</value>
            public bool rotateClockwise = true;

            /// <value>Property <c>rotateRate</c> represents the rate of the rotation.</value>
            public float rotateRate = 0.01f;
            
            /// <value>Property <c>rotateSpeed</c> represents the speed of the rotation.</value>
            public float rotateSpeed = 1.0f;
        
        #endregion
        
        #region Next Level

            /// <value>Property <c>nextScene</c> represents the next scene.</value>
            [Header("Next Scene")]
            public string nextScene;
            
            /// <value>Property <c>requireInput</c> represents whether the scene requires input to continue.</value>
            public bool requireInput;
        
            /// <value>Property <c>_inputReceived</c> represents whether input has been received.</value>
            private bool _inputReceived;
            
        #endregion

        /// <value>Property <c>_started</c> represents whether the scene has started.</value>
        private bool _started;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
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

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private IEnumerator Start()
        {
            _inputReceived = false;
            _started = false;
            yield return new WaitForSeconds(initialDelay);
            if (fadingTexts.Length > 0)
                FadeTexts(fadingTexts, 1.0f, fadeDuration);
            if (fadingImages.Length > 0)
                FadeImages(fadingImages, 1.0f, fadeDuration);
            if (blinkingTexts.Length > 0)
                BlinkTexts(blinkingTexts, blinkRate);
            if (rotatingObjects.Length > 0)
                RotateObjects(rotatingObjects, rotateRate, rotateClockwise, rotateSpeed);
            yield return new WaitForSeconds(startDelay);
            if (requireInput)
            {
                _started = true;
                yield break;
            }
            if (fadingTexts.Length > 0 || fadingImages.Length > 0)
            {
                FadeTexts(fadingTexts, 0.0f, fadeDuration);
                FadeImages(fadingImages, 0.0f, fadeDuration);
                yield return new WaitForSeconds(fadeDuration);
            }
            yield return new WaitForSeconds(finalDelay);
            SceneManager.LoadScene(nextScene);
        }

        /// <summary>
        /// Method <c>Update</c> is called once per frame.
        /// </summary>
        private void Update()
        {
            if (!_started || _inputReceived)
                return;
            if (!UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
                return;
            _inputReceived = true;
            SceneManager.LoadScene(nextScene);
        }

        /// <summary>
        /// Method <c>FadeTexts</c> fades the texts.
        /// </summary>
        /// <param name="texts">The texts to fade.</param>
        /// <param name="targetAlpha">The target alpha.</param>
        /// <param name="duration">The duration of the fade.</param>
        private void FadeTexts(TextMeshProUGUI[] texts, float targetAlpha, float duration)
        {
            foreach (var text in texts)
            {
                if (targetAlpha > 0)
                    text.canvasRenderer.SetAlpha(0.0f);
                text.CrossFadeAlpha(targetAlpha, duration, false);
            }
        }
        
        /// <summary>
        /// Method <c>FadeImages</c> fades the images.
        /// </summary>
        /// <param name="images">The images to fade.</param>
        /// <param name="targetAlpha">The target alpha.</param>
        /// <param name="duration">The duration of the fade.</param>
        private void FadeImages(Image[] images, float targetAlpha, float duration)
        {
            foreach (var image in images)
            {
                if (targetAlpha > 0)
                    image.canvasRenderer.SetAlpha(0.0f);
                image.CrossFadeAlpha(targetAlpha, duration, false);
            }
        }
        
        /// <summary>
        /// Method <c>BlinkTexts</c> blinks the texts.
        /// </summary>
        /// <param name="texts">The texts to blink.</param>
        /// <param name="rate">The rate of the blink.</param>
        private void BlinkTexts(TextMeshProUGUI[] texts, float rate)
        {
            foreach (var text in texts)
            {
                StartCoroutine(BlinkText(text, rate));
            }
        }
        
        /// <summary>
        /// Method <c>BlinkText</c> blinks the text.
        /// </summary>
        /// <param name="text">The text to blink.</param>
        /// <param name="rate">The rate of the blink.</param>
        private IEnumerator BlinkText(TextMeshProUGUI text, float rate)
        {
            while (true)
            {
                text.enabled = !text.enabled;
                yield return new WaitForSeconds(rate);
            }
        }
        
        /// <summary>
        /// Method <c>RotateObjects</c> rotates the objects.
        /// </summary>
        /// <param name="objects">The objects to rotate.</param>
        /// <param name="rate">The rate of the rotation.</param>
        /// <param name="clockwise">Whether the object should rotate clockwise.</param>
        /// <param name="speed">The speed of the rotation.</param>
        private void RotateObjects(GameObject[] objects, float rate, bool clockwise, float speed)
        {
            foreach (var obj in objects)
            {
                StartCoroutine(RotateObject(obj, rate, clockwise, speed));
            }
        }

        /// <summary>
        /// Method <c>RotateObject</c> rotates the object.
        /// </summary>
        /// <param name="obj">The object to rotate.</param>
        /// <param name="rate">The rate of the rotation.</param>
        /// <param name="clockwise">Whether the object should rotate clockwise.</param>
        /// <param name="speed">The speed of the rotation.</param>
        private IEnumerator RotateObject(GameObject obj, float rate, bool clockwise, float speed)
        {
            while (true)
            {
                obj.transform.Rotate(clockwise ? -Vector3.forward : Vector3.forward, speed);
                yield return new WaitForSeconds(rate);
            }
        }
    }
}
