using System;
using System.Collections;
using UnityEngine;
using PKDS.Managers;

namespace PKDS.Entities
{

    /// <summary>
    /// Class <c>HouseSet</c> represents a house set object.
    /// </summary>
    public class HouseSet : Interactable
    {
        #region Behaviour Properties

            /// <value>Enum <c>LoopBehaviour</c> represents the behaviour of the house set loop.</value>
            private enum LoopBehaviour
            {
                None,
                Start,
                Auto,
                Click
            }

            /// <value>Property <c>loopBehaviour</c> represents the behaviour of the house set loop.</value>
            [Header("Behaviour Properties")]
            [SerializeField]
            private LoopBehaviour loopBehaviour;

            /// <value>Property <c>childLoopBehaviour</c> represents the behaviour of the child house set loop.</value>
            [SerializeField]
            private LoopBehaviour childLoopBehaviour;

        #endregion

        #region House Set Properties

            /// <value>Property <c>houseSetPlaceholder</c> represents the placeholder for the house set prefab.</value>
            [Header("House Set Properties")] [SerializeField]
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
            [Header("Zoom Properties")] [SerializeField]
            private float zoomDelay;
            
        #endregion
        
        #region Outline Properties
            
            /// <value>Property <c>outlineTarget</c> represents the target of the outline.</value>
            [Header("Outline Properties")]
            [SerializeField]
            private Transform outlineTarget;

        #endregion

        #region Unity Event Methods

            /// <summary>
            /// Method <c>Awake</c> is called when the script instance is being loaded.
            /// </summary>
            protected override void Awake()
            {
                // If no zoom delay is set, set it to 1
                zoomDelay = (zoomDelay <= 0) ? 1 : zoomDelay;
                
                // Remove any existing switches or keys
                var existingSwitches = GetComponentsInChildren<Switch>();
                foreach (var existingSwitch in existingSwitches)
                    Destroy(existingSwitch.gameObject);
                var existingKeys = GetComponentsInChildren<Key>();
                foreach (var existingKey in existingKeys)
                    Destroy(existingKey.gameObject);

                // Call the base method
                base.Awake();
            }

            /// <summary>
            /// Method <c>Start</c> is called before the first frame update.
            /// </summary>
            private void Start()
            {
                switch (loopBehaviour)
                {
                    case LoopBehaviour.None:
                        break;
                    case LoopBehaviour.Start:
                        CreateChildHouseSet(childLoopBehaviour);
                        CreateSwitch();
                        break;
                    case LoopBehaviour.Auto:
                        StartCoroutine(DelayedZoomIn());
                        break;
                    case LoopBehaviour.Click:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        
        #endregion
        
        #region Input Event Methods

            /// <summary>
            /// Method <c>HandleLeftClickUp</c> handles the left click up event.
            /// </summary>
            protected override void HandleLeftClickUp()
            {
                if (loopBehaviour != LoopBehaviour.Click)
                    return;
                PreventInteraction();
                ZoomIn();
            }
            
        #endregion

        #region Zoom Methods

            /// <summary>
            /// Method <c>ZoomIn</c> is called to zoom in the house set.
            /// </summary>
            public void ZoomIn()
            {
                if (parentHouseSet == null)
                    return;
                GameManager.Instance.areAllInteractionsPrevented = true;
                switch (loopBehaviour)
                {
                    case LoopBehaviour.None:
                    case LoopBehaviour.Start:
                    case LoopBehaviour.Auto:
                        break;
                    case LoopBehaviour.Click:
                        OutlineComponent.enabled = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                loopBehaviour = LoopBehaviour.None;
                CreateChildHouseSet();
                StartCoroutine(ZoomInCoroutine());
            }
            
            /// <summary>
            /// Method <c>DelayedZoomIn</c> is called to delay the zoom in the house set.
            /// </summary>
            /// <returns>IEnumerator</returns>
            private IEnumerator DelayedZoomIn()
            {
                yield return new WaitForSeconds(zoomDelay);
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
                
                // Reset the level transitioning state
                GameManager.Instance.areAllInteractionsPrevented = false;
            }

        #endregion
        
        #region Create Methods

            /// <summary>
            /// Method <c>CreateChildHouseSet</c> is called to create a child house set.
            /// </summary>
            /// <param name="overrideLoopBehaviour">An override loop behaviour.</param>
            private void CreateChildHouseSet(LoopBehaviour overrideLoopBehaviour = LoopBehaviour.None)
            {
                // Create and configure the child house set
                childHouseSet = Instantiate(houseSetPrefab, houseSetPlaceholder.position, houseSetPlaceholder.rotation)
                    .GetComponent<HouseSet>();
                childHouseSet.transform.SetParent(houseSetPlaceholder);
                childHouseSet.transform.localScale = new Vector3(1f, 1f, 1f);
                childHouseSet.gameObject.name = "HouseSet";
                childHouseSet.loopBehaviour = overrideLoopBehaviour;
                childHouseSet.childLoopBehaviour = childLoopBehaviour;
                childHouseSet.parentHouseSet = this;
                
                // Create a switch on a random placeholder of the child house set
                childHouseSet.CreateSwitch();
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
                switchComponent.targetHouseSet = this;
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
                keyComponent.targetHouseSet = childHouseSet;
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
                if (loopBehaviour != LoopBehaviour.Click)
                    return;
                base.Highlight();
            }
        
        #endregion
    }
}