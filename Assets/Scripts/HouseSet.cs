using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class <c>HouseSet</c> represents the house set.
/// </summary>
public class HouseSet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <value>Enum <c>LoopBehaviour</c> represents the behaviour of the house set loop.</value>
    private enum LoopBehaviour
    {
        None,
        Auto,
        Click
    }
    
    /// <value>Property <c>loopBehaviour</c> represents the behaviour of the house set loop.</value>
    [SerializeField]
    private LoopBehaviour loopBehaviour;

    /// <value>Property <c>childLoopBehaviour</c> represents the behaviour of the child house set loop.</value>
    [SerializeField]
    private LoopBehaviour childLoopBehaviour;

    /// <value>Property <c>houseSetPlaceholder</c> represents the placeholder for the house set prefab.</value>
    [SerializeField]
    private Transform houseSetPlaceholder;
    
    /// <value>Property <c>houseSetPrefab</c> represents the prefab of the house set.</value>
    [SerializeField]
    private GameObject houseSetPrefab;
    
    /// <value>Property <c>parentHouseSet</c> represents the parent house set.</value>
    public HouseSet parentHouseSet;
    
    /// <value>Property <c>childHouseSet</c> represents the child house set.</value>
    public HouseSet childHouseSet;
    
    /// <value>Property <c>zoomDelay</c> represents the velocity of the zoom.</value>
    [SerializeField]
    private float zoomDelay;
        
    /// <value>Property <c>_outline</c> represents the outline of the object.</value>
    private Outline _outline;
        
    /// <value>Property <c>_isPointerOver</c> represents if the pointer is over the object.</value>
    private bool _isPointerOver;
        
    /// <value>Property <c>_isKeyPressed</c> represents if a key is pressed over the object.</value>
    private bool _isKeyPressed;
    
    /// <value>Property <c>_isZoomingIn</c> represents the flag to check if the zoom is in progress.</value>
    private bool _isZoomingIn;
    
    /// <summary>
    /// Method <c>Awake</c> is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // If no zoom delay is set, set it to 1
        zoomDelay = (zoomDelay <= 0) ? 1 : zoomDelay;
        
        // If the outline component is not present, add it
        _outline = gameObject.GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineColor = Color.magenta;
        _outline.OutlineWidth = 5f;
        _outline.enabled = false;
    }
    
    /// <summary>
    /// Method <c>Start</c> is called before the first frame update.
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator Start()
    {
        switch (loopBehaviour)
        {
            case LoopBehaviour.None:
                break;
            case LoopBehaviour.Auto:
                yield return new WaitForSeconds(zoomDelay);
                ZoomIn();
                break;
            case LoopBehaviour.Click:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        yield return null;
    }

    /// <summary>
    /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        switch (loopBehaviour)
        {
            case LoopBehaviour.None:
            case LoopBehaviour.Auto:
                break;
            case LoopBehaviour.Click:
                Highlight();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Method <c>ZoomIn</c> is called to zoom in the house set.
    /// </summary>
    private void ZoomIn()
    {
        if (_isZoomingIn || parentHouseSet == null)
            return;
        _isZoomingIn = true;
        switch (loopBehaviour)
        {
            case LoopBehaviour.None:
            case LoopBehaviour.Auto:
                break;
            case LoopBehaviour.Click:
                _outline.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        loopBehaviour = LoopBehaviour.None;
        CreateChildHouseSet();
        StartCoroutine(ZoomInCoroutine());
    }
    
    /// <summary>
    /// Method <c>CreateChildHouseSet</c> is called to create a child house set.
    /// </summary>
    private void CreateChildHouseSet()
    {
        childHouseSet = Instantiate(houseSetPrefab, houseSetPlaceholder.position, Quaternion.identity)
            .GetComponent<HouseSet>();
        childHouseSet.transform.SetParent(houseSetPlaceholder);
        childHouseSet.transform.localScale = new Vector3(1f, 1f, 1f);
        childHouseSet.gameObject.name = "HouseSet";
        childHouseSet.loopBehaviour = LoopBehaviour.None;
        childHouseSet.childLoopBehaviour = childLoopBehaviour;
        childHouseSet.parentHouseSet = this;
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
        
        // Assign the correct behaviour to the child house set
        childHouseSet.loopBehaviour = childLoopBehaviour;
    }
    
    /// <summary>
    /// Method <c>OnPointerEnter</c> is called when the pointer enters the object.
    /// </summary>
    /// <param name="eventData">The pointer event data.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
    }
    
    /// <summary>
    /// Method <c>OnPointerExit</c> is called when the pointer exits the object.
    /// </summary>
    /// <param name="eventData">The pointer event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
    }
    
    /// <summary>
    /// Method <c>OnPointerDown</c> is called when the pointer is down on the object.
    /// </summary>
    /// <param name="eventData">The pointer event data.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                HandleLeftClickDown();
                break;
            case PointerEventData.InputButton.Right:
            case PointerEventData.InputButton.Middle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Method <c>OnPointerUp</c> is called when the pointer is up on the object.
    /// </summary>
    /// <param name="eventData">The pointer event data.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                HandleLeftClickUp();
                break;
            case PointerEventData.InputButton.Right:
            case PointerEventData.InputButton.Middle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
        
    /// <summary>
    /// Method <c>HandleLeftClickDown</c> handles the left click down event.
    /// </summary>
    private void HandleLeftClickDown()
    {
        if (_isZoomingIn || _isKeyPressed)
            return;
        _isKeyPressed = true;
    }
    
    /// <summary>
    /// Method <c>HandleLeftClickUp</c> handles the left click up event.
    /// </summary>
    private void HandleLeftClickUp()
    {
        _isKeyPressed = false;
        if (loopBehaviour == LoopBehaviour.Click)
            ZoomIn();
    }

    /// <summary>
    /// Method <c>Highlight</c> highlights the object.
    /// </summary>
    private void Highlight()
    {
        if (_isZoomingIn || _isKeyPressed)
            return;
        _outline.enabled = _isPointerOver;
    }
}
