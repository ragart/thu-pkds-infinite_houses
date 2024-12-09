using System;
using System.Collections;
using UnityEngine;
using PKDS.Managers;
using PKDS.Properties;

namespace PKDS.Entities
{

    /// <summary>
    /// Class <c>HouseSet</c> represents a house set object.
    /// </summary>
    public class HouseSet : Interactable
    {
        #region Behaviour Properties

            /// <value>Property <c>loopBehaviour</c> represents the behaviour of the house set loop.</value>
            [Header("Behaviour Properties")]
            [SerializeField]
            private Loop.Behaviour loopBehaviour;

            /// <value>Property <c>childLoopBehaviour</c> represents the behaviour of the child house set loop.</value>
            [SerializeField]
            private Loop.Behaviour childLoopBehaviour;

        #endregion

        #region House Set Properties

            /// <value>Property <c>houseSetPlaceholder</c> represents the placeholder for the house set prefab.</value>
            [Header("House Set Properties")]
            [SerializeField]
            private Transform houseSetPlaceholder;

            /// <value>Property <c>houseSetPrefab</c> represents the prefab of the house set.</value>
            [SerializeField]
            private GameObject houseSetPrefab;

            /// <value>Property <c>parentHouseSet</c> represents the parent house set.</value>
            [SerializeField]
            private HouseSet parentHouseSet;

            /// <value>Property <c>childHouseSet</c> represents the child house set.</value>
            [SerializeField]
            private HouseSet childHouseSet;

        #endregion

        #region Key & Switch Properties

            /// <value>Property <c>switchPlaceholderContainer</c> represents the container for the switch placeholders.</value>
            [Header("Key & Switch Properties")]
            [SerializeField]
            private Transform switchPlaceholderContainer;

            /// <value>Property <c>keyPlaceholderContainer</c> represents the container for the key placeholders.</value>
            [SerializeField]
            private Transform keyPlaceholderContainer;

            /// <value>Property <c>switchPrefab</c> represents the prefab of the switch.</value>
            [SerializeField]
            private GameObject switchPrefab;

            /// <value>Property <c>keyPrefab</c> represents the prefab of the key.</value>
            [SerializeField]
            private GameObject keyPrefab;

            /// <value>Property <c>switchComponent</c> represents the switch of the house set.</value>
            [SerializeField]
            private Switch switchComponent;
            
            /// <value>Property <c>keyComponent</c> represents the key of the house set.</value>
            [SerializeField]
            private Key keyComponent;
            
        #endregion

        #region Zoom Properties

            /// <value>Property <c>zoomDelay</c> represents the velocity of the zoom.</value>
            [Header("Zoom Properties")]
            [SerializeField]
            private float zoomDelay = 5f;
            
        #endregion
        
        #region Outline Properties
            
            /// <value>Property <c>outlineTarget</c> represents the target of the outline.</value>
            [Header("Outline Properties")]
            [SerializeField]
            private Transform outlineTarget;

        #endregion

        #region Round Properties
        
            /// <value>Property <c>_roundTime</c> represents the time of the round.</value>
            private float _roundTime;
            
            /// <value>Property <c>_isRoundStarted</c> represents if the round is started.</value>
            private bool _isRoundStarted;
        
        #endregion
        
        #region Unity Event Methods

            /// <summary>
            /// Method <c>Awake</c> is called when the script instance is being loaded.
            /// </summary>
            protected override void Awake()
            {
                // Remove any existing switches or keys
                DestroyExistingSwitchesAndKeys();

                // Call the base method
                base.Awake();
            }

            /// <summary>
            /// Method <c>Start</c> is called before the first frame update.
            /// </summary>
            private void Start()
            {
                if (parentHouseSet == null && childLoopBehaviour != GameManager.Instance.LoopBehaviour)
                {
                    if (GameManager.Instance.Round > 0)
                        throw new Exception($"Something went wrong: {gameObject.name} has no parent house set in round {GameManager.Instance.Round}");
                    loopBehaviour = Loop.Behaviour.Start;
                    childLoopBehaviour = GameManager.Instance.LoopBehaviour;
                }
                if (loopBehaviour != Loop.Behaviour.Start)
                    return;
                CreateChildHouseSet(childLoopBehaviour);
                DelayedStart();
            }
            
            private void DelayedStart()
            {
                SetRoundTime();
                Debug.Log($"({gameObject.name}) Round time: {_roundTime}");
                Debug.Log($"({gameObject.name}) Loop behaviour: {loopBehaviour}");
                var targetLoopBehaviour = (loopBehaviour == Loop.Behaviour.Start) ? childLoopBehaviour : loopBehaviour;
                switch (targetLoopBehaviour)
                {
                    case Loop.Behaviour.None:
                    case Loop.Behaviour.Start:
                        break;
                    case Loop.Behaviour.Auto:
                        StartCoroutine(childHouseSet.DelayedZoomIn(_roundTime));
                        break;
                    case Loop.Behaviour.Click:
                        RoundStart();
                        break;
                    case Loop.Behaviour.SwitchKey:
                        CreateSwitch();
                        RoundStart();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                loopBehaviour = Loop.Behaviour.None;
            }
            
            /// <summary>
            /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
            /// </summary>
            protected override void Update()
            {
                base.Update();
                RoundUpdate();
            }
        
        #endregion
        
        #region Input Event Methods

            /// <summary>
            /// Method <c>HandleLeftClickUp</c> handles the left click up event.
            /// </summary>
            protected override void HandleLeftClickUp()
            {
                if (loopBehaviour != Loop.Behaviour.Click)
                    return;
                parentHouseSet.RoundEnd();
            }
            
        #endregion

        #region Zoom Methods

            /// <summary>
            /// Method <c>ZoomIn</c> is called to zoom in the house set.
            /// </summary>
            private void ZoomIn()
            {
                if (parentHouseSet == null)
                    return;
                switch (loopBehaviour)
                {
                    case Loop.Behaviour.None:
                    case Loop.Behaviour.Start:
                    case Loop.Behaviour.Auto:
                        break;
                    case Loop.Behaviour.Click:
                        OutlineComponent.enabled = false;
                        break;
                    case Loop.Behaviour.SwitchKey:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                CreateChildHouseSet();
                StartCoroutine(ZoomInCoroutine());
            }
            
            /// <summary>
            /// Method <c>DelayedZoomIn</c> is called to delay the zoom in the house set.
            /// </summary>
            /// <param name="delay">The delay of the zoom.</param>
            /// <returns>IEnumerator</returns>
            private IEnumerator DelayedZoomIn(float delay = 0f)
            {
                Debug.Log("DELAYED ZOOM IN");
                yield return new WaitForSeconds(delay);
                ZoomIn();
            }

            /// <summary>
            /// Method <c>ZoomInCoroutine</c> is called to zoom in the house set.
            /// </summary>
            private IEnumerator ZoomInCoroutine()
            {
                // Scale the game object progressively
                var targetScale = new Vector3(50f, 50f, 50f);
                var initialScale = parentHouseSet.transform.localScale;
                for (var elapsedTime = 0f; elapsedTime < zoomDelay; elapsedTime += Time.deltaTime)
                {
                    parentHouseSet.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / zoomDelay);
                    yield return null;
                }
                parentHouseSet.transform.localScale = targetScale;

                // Move the game object to the root of the hierarchy
                transform.SetParent(null);

                // Destroy the parent house set
                Destroy(parentHouseSet.gameObject);
                parentHouseSet = null;

                // Assign the correct behaviour to the child house set
                childHouseSet.loopBehaviour = childLoopBehaviour;
                
                // Invoke the start after zoom method
                DelayedStart();
            }

        #endregion
        
        #region Create and Destroy Methods

            /// <summary>
            /// Method <c>CreateChildHouseSet</c> is called to create a child house set.
            /// </summary>
            /// <param name="overrideLoopBehaviour">An override loop behaviour.</param>
            private void CreateChildHouseSet(Loop.Behaviour overrideLoopBehaviour = Loop.Behaviour.None)
            {
                // Create and configure the child house set
                childHouseSet = Instantiate(houseSetPrefab, houseSetPlaceholder.position, houseSetPlaceholder.rotation)
                    .GetComponent<HouseSet>();
                childHouseSet.transform.SetParent(houseSetPlaceholder);
                childHouseSet.transform.localScale = new Vector3(1f, 1f, 1f);
                childHouseSet.gameObject.name = "HouseSet" + GameManager.Instance.Round;
                childHouseSet.loopBehaviour = overrideLoopBehaviour;
                childHouseSet.childLoopBehaviour = childLoopBehaviour;
                childHouseSet.parentHouseSet = this;
            }
            
            /// <summary>
            /// Method <c>CreateSwitch</c> is called to create a switch.
            /// </summary>
            private void CreateSwitch()
            {
                // Create a switch on a random placeholder of the child house set
                var randomPlaceholder = switchPlaceholderContainer
                    .GetChild(UnityEngine.Random.Range(0, switchPlaceholderContainer.childCount));
                switchComponent = Instantiate(switchPrefab, randomPlaceholder.position, randomPlaceholder.rotation)
                    .GetComponent<Switch>();
                switchComponent.transform.SetParent(randomPlaceholder);
                switchComponent.transform.localScale = new Vector3(1f, 1f, 1f);
                switchComponent.gameObject.name = "Switch";
                switchComponent.houseSet = this;
            }
            
            /// <summary>
            /// Method <c>CreateKey</c> is called to create a key.
            /// </summary>
            public void CreateKey()
            {
                // Create a key on a random placeholder of the child house set
                var randomPlaceholder = keyPlaceholderContainer
                    .GetChild(UnityEngine.Random.Range(0, keyPlaceholderContainer.childCount));
                keyComponent = Instantiate(keyPrefab, randomPlaceholder.position, randomPlaceholder.rotation)
                    .GetComponent<Key>();
                keyComponent.transform.SetParent(randomPlaceholder);
                keyComponent.transform.localScale = new Vector3(1f, 1f, 1f);
                keyComponent.gameObject.name = "Key";
                keyComponent.houseSet = this;
            }
            
            /// <summary>
            /// Method <c>DestroyExistingSwitchesAndKeys</c> is called to destroy existing switches and keys.
            /// </summary>
            private void DestroyExistingSwitchesAndKeys()
            {
                var existingSwitches = GetComponentsInChildren<Switch>();
                foreach (var existingSwitch in existingSwitches)
                    Destroy(existingSwitch.gameObject);
                var existingKeys = GetComponentsInChildren<Key>();
                foreach (var existingKey in existingKeys)
                    Destroy(existingKey.gameObject);
            }
        
        #endregion

        #region Outline Methods

            /// <summary>
            /// Method <c>SetOutline</c> sets the outline of the object.
            /// </summary>
            protected override void SetOutlineTarget()
            {
                OutlineTarget = outlineTarget;
            }

            /// <summary>
            /// Method <c>Highlight</c> highlights the object.
            /// </summary>
            protected override void Highlight()
            {
                if (loopBehaviour != Loop.Behaviour.Click)
                    return;
                base.Highlight();
            }
        
        #endregion
        
        #region Round Methods
        
            /// <summary>
            /// Method <c>SetRoundTime</c> sets the round time.
            /// </summary>
            private void SetRoundTime()
            {
                var gameDuration = GameManager.Instance.GameTime;
                var elapsedTime = gameDuration - GameManager.Instance.GameTimeLeft;
                var timeRatio = Mathf.Clamp01(elapsedTime / (gameDuration * 0.75f));
                _roundTime = Mathf.Lerp(GameManager.Instance.MaxRoundTime, GameManager.Instance.MinRoundTime, timeRatio);
            }
            
            /// <summary>
            /// Method <c>RoundStart</c> starts the round.
            /// </summary>
            private void RoundStart()
            {
                GameManager.Instance.Round++;
                PreventInteraction(false, ScopeGlobal);
                _isRoundStarted = true;
            }
            
            /// <summary>
            /// Method <c>RoundUpdate</c> updates the round.
            /// </summary>
            private void RoundUpdate()
            {
                if (!_isRoundStarted)
                    return;
                // Update the timer
                _roundTime -= Time.deltaTime;
                _roundTime = Mathf.Clamp(_roundTime, 0f, GameManager.Instance.MaxRoundTime);
                // If the time is up, end the round in failure
                if (_roundTime > 0f)
                    return;
                RoundEnd(false);
            }
            
            /// <summary>
            /// Method <c>EndRound</c> ends the round.
            /// </summary>
            /// <param name="success">Whether the round was successful.</param>
            public void RoundEnd(bool success = true)
            {
                _isRoundStarted = false;
                PreventInteraction(true, ScopeBoth);
                if (success)
                    GameManager.Instance.IncreaseWinCount();
                else
                    GameManager.Instance.IncreaseLossCount();
                childHouseSet.ZoomIn();
            }
        
        #endregion
    }
}