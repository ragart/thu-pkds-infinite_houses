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
        
        #region Outline Properties
            
            /// <value>Property <c>outlineTarget</c> represents the target of the outline.</value>
            [Header("Outline Properties")]
            [SerializeField]
            private Transform outlineTarget;

        #endregion

        #region Round Properties
        
            /// <value>Property <c>_roundTime</c> represents the time of the round.</value>
            private float _roundTime;
            
            /// <value>Property <c>ForcedRoundTime</c> represents the forced time of the round.</value>
            private float ForcedRoundTime { get; set; }
            
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
                if (loopBehaviour == Loop.Behaviour.Start)
                    CreateChildHouseSet();
            }
            
            /// <summary>
            /// Method <c>OnEnable</c> is called when the object becomes enabled and active.
            /// </summary>
            private void OnEnable()
            {
                GameManager.OnGameStart += HandleGameStart;
            }
            
            /// <summary>
            /// Method <c>OnDisable</c> is called when the behaviour becomes disabled or inactive.
            /// </summary>
            private void OnDisable()
            {
                GameManager.OnGameStart -= HandleGameStart;
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
        
        #region Custom Event Methods
        
            /// <summary>
            /// Method <c>Initialize</c> initializes the house set.
            /// </summary>
            /// <param name="setName">The name of the house set.</param>
            /// <param name="setParent">The parent of the house set.</param>
            /// <param name="setScale">The scale of the house set.</param>
            /// <param name="setParentHouseSet">The parent house set of the house set.</param>
            public override void Initialize(string setName, Transform setParent, Vector3 setScale, HouseSet setParentHouseSet)
            {
                gameObject.name = setName;
                transform.SetParent(setParent);
                transform.localScale = setScale;
                parentHouseSet = setParentHouseSet;
                PreventInteraction(false);
            }
            
            /// <summary>
            /// Method <c>HandleGameStart</c> handles the game start event.
            /// </summary>
            private void HandleGameStart()
            {
                if (loopBehaviour == Loop.Behaviour.Start)
                    DelayedStart();
            }
            
            /// <summary>
            /// Method <c>DelayedStart</c> is called to delay the start of the house set.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when an argument is outside the range of valid values.</exception>
            private void DelayedStart()
            {
                // Check if the behaviour of the first child is different from the game loop behaviour
                if (loopBehaviour == Loop.Behaviour.Start && childLoopBehaviour != GameManager.Instance.LoopBehaviour)
                {
                    childLoopBehaviour = GameManager.Instance.LoopBehaviour;
                    childHouseSet.SetLoopBehaviour(childLoopBehaviour, childLoopBehaviour);
                }
                
                // Set the forced round time
                if (childLoopBehaviour == Loop.Behaviour.Random)
                {
                    ForcedRoundTime = childHouseSet.loopBehaviour == Loop.Behaviour.Auto 
                        ? 0.5f 
                        : UnityEngine.Random.Range(GameManager.Instance.MinRoundTime, GameManager.Instance.MaxRoundTime);
                }

                // Set the round time
                SetRoundTime();

                // Check the behaviour of the house set
                if (childHouseSet.loopBehaviour == Loop.Behaviour.SwitchKey)
                    CreateSwitch();

                // Set the current behaviour to none for preventing highlighting and interaction
                loopBehaviour = Loop.Behaviour.None;

                // Start the round
                RoundStart();
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
                if (loopBehaviour == Loop.Behaviour.Click)
                    OutlineComponent.enabled = false;
                CreateChildHouseSet();
                StartCoroutine(ZoomInCoroutine());
            }

            /// <summary>
            /// Method <c>ZoomInCoroutine</c> is called to zoom in the house set.
            /// </summary>
            private IEnumerator ZoomInCoroutine()
            {
                // Scale the game object progressively
                var targetScale = new Vector3(50.0f, 50.0f, 50.0f);
                var initialScale = parentHouseSet.transform.localScale;
                for (var elapsedTime = 0.0f; elapsedTime < GameManager.Instance.ZoomDelay; elapsedTime += Time.deltaTime)
                {
                    parentHouseSet.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / GameManager.Instance.ZoomDelay);
                    yield return null;
                }
                parentHouseSet.transform.localScale = targetScale;

                // Move the game object to the root of the hierarchy
                transform.SetParent(null);

                // Destroy the parent house set
                Destroy(parentHouseSet.gameObject);
                parentHouseSet = null;
                
                // Start the house set
                DelayedStart();
            }

        #endregion
        
        #region Create and Destroy Methods

            /// <summary>
            /// Method <c>CreateChildHouseSet</c> is called to create a child house set.
            /// </summary>
            private void CreateChildHouseSet()
            {
                // Create and configure the child house set
                childHouseSet = Instantiate(houseSetPrefab, houseSetPlaceholder.position, houseSetPlaceholder.rotation)
                    .GetComponent<HouseSet>();
                childHouseSet.Initialize("HouseSet" + GameManager.Instance.Round, houseSetPlaceholder, new Vector3(1.0f, 1.0f, 1.0f), this);
                childHouseSet.SetLoopBehaviour(childLoopBehaviour, childLoopBehaviour);
            }

            /// <summary>
            /// Method <c>SetLoopBehaviour</c> sets the loop behaviour.
            /// </summary>
            /// <param name="setLoopBehaviour"></param>
            /// <param name="setChildLoopBehaviour"></param>
            private void SetLoopBehaviour(Loop.Behaviour setLoopBehaviour, Loop.Behaviour setChildLoopBehaviour)
            {
                loopBehaviour = (setLoopBehaviour == Loop.Behaviour.Random) ? Loop.GetRandomBehaviour() : setLoopBehaviour;
                childLoopBehaviour = setChildLoopBehaviour;
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
                switchComponent.Initialize("Switch", randomPlaceholder, new Vector3(1.0f, 1.0f, 1.0f), this);
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
                keyComponent.Initialize("Key", randomPlaceholder, new Vector3(1.0f, 1.0f, 1.0f), this);
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
                _roundTime = ForcedRoundTime > 0 
                    ? ForcedRoundTime 
                    : gameDuration == 0
                        // If the game duration is 0, set the round time to the average of the max and min round times
                        ? (GameManager.Instance.MaxRoundTime + GameManager.Instance.MinRoundTime) / 2 
                        // Otherwise, set the round time depending on the elapsed time of the game
                        : Mathf.Lerp(GameManager.Instance.MaxRoundTime,
                            GameManager.Instance.MinRoundTime, 
                            Mathf.Clamp01((gameDuration - GameManager.Instance.GameTimeLeft) / (gameDuration * 0.75f)));
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
                if (!_isRoundStarted || !GameManager.Instance.IsGameStarted)
                    return;
                // Update the timer
                _roundTime -= Time.deltaTime;
                _roundTime = Mathf.Clamp(_roundTime, 0.0f, GameManager.Instance.MaxRoundTime);
                // If the time is up, end the round in failure
                if (_roundTime > 0.0f)
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
                if (childHouseSet.loopBehaviour is Loop.Behaviour.Click or Loop.Behaviour.SwitchKey)
                {
                    if (success)
                        GameManager.Instance.IncreaseWinCount();
                    else
                        GameManager.Instance.IncreaseLossCount();
                }
                childHouseSet.ZoomIn();
            }
        
        #endregion
    }
}