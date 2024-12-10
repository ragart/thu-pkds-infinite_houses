using System;
using UnityEngine;
using UnityEngine.EventSystems;
using PKDS.Managers;

namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Interactable</c> represents any interactable object.
    /// </summary>
    public abstract class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        /// <value>Property <c>ScopeLocal</c> represents the local scope.</value>
        protected const int ScopeLocal = 0;
        
        /// <value>Property <c>ScopeGlobal</c> represents the global scope.</value>
        protected const int ScopeGlobal = 1;
        
        /// <value>Property <c>ScopeBoth</c> represents both local and global scopes.</value>
        protected const int ScopeBoth = 2;
        
        #region Outline Properties

            /// <value>Property <c>OutlineComponent</c> represents the outline of the object.</value>
            protected Outline OutlineComponent;

            /// <value>Property <c>OutlineTarget</c> represents the target of the outline.</value>
            protected Transform OutlineTarget;
        
        #endregion
        
        #region State Properties

            /// <value>Property <c>_isPointerOver</c> represents if the pointer is over the object.</value>
            private bool _isPointerOver;

            /// <value>Property <c>_isKeyPressed</c> represents if a key is pressed over the object.</value>
            private bool _isKeyPressed;

            /// <value>Property <c>_isInteractionEnabled</c> represents if the interaction is enabled.</value>
            private bool _isInteractionEnabled;

        #endregion
        
        #region Unity Event Methods

            /// <summary>
            /// Method <c>Awake</c> is called when the script instance is being loaded.
            /// </summary>
            protected virtual void Awake()
            {
                SetOutlineTarget();
                SetOutline();
                ConfigureOutline();
            }

            /// <summary>
            /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
            /// </summary>
            protected virtual void Update()
            {
                Highlight();
            }
        
        #endregion
        
        #region Custom Event Methods

            /// <summary>
            /// Method <c>Initialize</c> initializes the switch object.
            /// </summary>
            /// <param name="setName">The name of the switch object.</param>
            /// <param name="setParent">The parent of the switch object.</param>
            /// <param name="setScale">The scale of the switch object.</param>
            /// <param name="setHouseSet">The house set of the switch object.</param>
            public abstract void Initialize(string setName, Transform setParent, Vector3 setScale, HouseSet setHouseSet);
        
        #endregion
        
        #region Input Event Methods

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
                        if (_isKeyPressed)
                            break;
                        _isKeyPressed = true;
                        if (!IsInteractionPossible())
                            break;
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
                        _isKeyPressed = false;
                        if (!IsInteractionPossible())
                            break;
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
            protected virtual void HandleLeftClickDown()
            {
            }

            /// <summary>
            /// Method <c>HandleLeftClickUp</c> handles the left click up event.
            /// </summary>
            protected virtual void HandleLeftClickUp()
            {
            }
        
        #endregion
        
        #region Outline Methods

            /// <summary>
            /// Method <c>SetOutlineTarget</c> sets the outline target.
            /// </summary>
            protected virtual void SetOutlineTarget()
            {
                OutlineTarget = transform;
            }
            
            /// <summary>
            /// Method <c>SetOutline</c> sets the outline component.
            /// </summary>
            protected void SetOutline()
            {
                OutlineComponent = OutlineTarget.GetComponent<Outline>()
                                   ?? OutlineTarget.gameObject.AddComponent<Outline>();
            }
            
            /// <summary>
            /// Method <c>ConfigureOutline</c> configures the outline component.
            /// </summary>
            protected void ConfigureOutline()
            {
                OutlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
                OutlineComponent.OutlineColor = Color.magenta;
                OutlineComponent.OutlineWidth = 5f;
                OutlineComponent.enabled = false;
            }

            /// <summary>
            /// Method <c>Highlight</c> highlights the object.
            /// </summary>
            protected virtual void Highlight()
            {
                if (!IsInteractionPossible())
                    OutlineComponent.enabled = false;
                else if (!_isKeyPressed)
                    OutlineComponent.enabled = _isPointerOver;
            }
        
        #endregion
        
        #region State Methods
        
            protected bool IsInteractionPossible()
            {
                return _isInteractionEnabled
                       && GameManager.Instance.IsInteractionEnabled
                       && GameManager.Instance.IsGameStarted;
            }
    
            /// <summary>
            /// Method <c>PreventInteraction</c> prevents the interaction.
            /// </summary>
            /// <param name="prevent">Whether to prevent the interaction.</param>
            /// <param name="scope">The scope of the interaction prevention.</param>
            protected void PreventInteraction(bool prevent = true, int scope = 0)
            {
                switch (scope)
                {
                    case ScopeLocal:
                        _isInteractionEnabled = !prevent;
                        break;
                    case ScopeGlobal:
                        GameManager.Instance.PreventInteraction(prevent);
                        break;
                    case ScopeBoth:
                        _isInteractionEnabled = !prevent;
                        GameManager.Instance.PreventInteraction(prevent);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        #endregion
    }
}
