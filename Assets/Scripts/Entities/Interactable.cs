using System;
using UnityEngine;
using UnityEngine.EventSystems;
using PKDS.Managers;

namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Interactable</c> represents any interactable object.
    /// </summary>
    public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
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

            /// <value>Property <c>_isInteractionPrevented</c> represents if the interaction is prevented.</value>
            private bool _isInteractionPrevented;

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
            private void Update()
            {
                Highlight();
            }
        
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
                        if (_isInteractionPrevented || GameManager.Instance.areAllInteractionsPrevented)
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
                        if (_isInteractionPrevented || GameManager.Instance.areAllInteractionsPrevented)
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
            protected virtual void SetOutline()
            {
                OutlineComponent = OutlineTarget.GetComponent<Outline>()
                                   ?? OutlineTarget.gameObject.AddComponent<Outline>();
            }
            
            /// <summary>
            /// Method <c>ConfigureOutline</c> configures the outline component.
            /// </summary>
            protected virtual void ConfigureOutline()
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
                if (_isKeyPressed || _isInteractionPrevented || GameManager.Instance.areAllInteractionsPrevented)
                    return;
                OutlineComponent.enabled = _isPointerOver;
            }
        
        #endregion

        protected void PreventInteraction()
        {
            _isInteractionPrevented = true;
            OutlineComponent.enabled = false;
        }
    }
}
