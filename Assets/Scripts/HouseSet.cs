using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>HouseSet</c> represents the house set.
/// </summary>
public class HouseSet : MonoBehaviour
{
    /// <value>Property <c>houseSetPlaceholder</c> represents the placeholder for the house set prefab.</value>
    [SerializeField]
    private Transform houseSetPlaceholder;
    
    /// <value>Property <c>houseSetPrefab</c> represents the prefab of the house set.</value>
    [SerializeField]
    private GameObject houseSetPrefab;
    
    /// <value>Property <c>parentHouseSet</c> represents the parent house set.</value>
    public HouseSet parentHouseSet;
    
    /// <value>Property <c>zoomDelay</c> represents the velocity of the zoom.</value>
    [SerializeField]
    private float zoomDelay;
    
    /// <value>Property <c>_isZoomingIn</c> represents the flag to check if the zoom is in progress.</value>
    private bool _isZoomingIn;
    
    /// <summary>
    /// Method <c>Awake</c> is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // If no zoom delay is set, set it to 1
        zoomDelay = (zoomDelay <= 0) ? 1 : zoomDelay;
    }
    
    /// <summary>
    /// Method <c>Start</c> is called before the first frame update.
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(zoomDelay);
        ZoomIn();
    }

    /// <summary>
    /// Method <c>ZoomIn</c> is called to zoom in the house set.
    /// </summary>
    public void ZoomIn()
    {
        if (_isZoomingIn || parentHouseSet == null)
            return;
        _isZoomingIn = true;
        CreateChildHouseSet();
        StartCoroutine(ZoomInCoroutine());
    }
    
    /// <summary>
    /// Method <c>CreateChildHouseSet</c> is called to create a child house set.
    /// </summary>
    private void CreateChildHouseSet()
    {
        var childHouseSet = Instantiate(houseSetPrefab, houseSetPlaceholder.position, Quaternion.identity);
        childHouseSet.transform.SetParent(houseSetPlaceholder);
        childHouseSet.transform.localScale = new Vector3(1f, 1f, 1f);
        childHouseSet.gameObject.name = "HouseSet";
        childHouseSet.GetComponent<HouseSet>().parentHouseSet = this;
    }
    
    /// <summary>
    /// Method <c>ZoomInCoroutine</c> is called to zoom in the house set.
    /// </summary>
    private IEnumerator ZoomInCoroutine()
    {
        var targetScale = new Vector3(50f, 50f, 50f);
        var initialScale = parentHouseSet.transform.localScale;
        Debug.Log("Initial scale: " + initialScale + ", Target scale: " + targetScale);
        var elapsedTime = 0f;
        while (elapsedTime < zoomDelay)
        {
            elapsedTime += Time.deltaTime;
            var newScale = Vector3.Lerp(initialScale, targetScale, Mathf.Clamp01(elapsedTime / zoomDelay));
            Debug.Log("New scale: " + newScale);
            parentHouseSet.transform.localScale = newScale;
            yield return null;
        }
        parentHouseSet.transform.localScale = targetScale;
        Debug.Log("Final scale: " + parentHouseSet.transform.localScale);
        transform.SetParent(null);
        Destroy(parentHouseSet.gameObject);
    }
}
