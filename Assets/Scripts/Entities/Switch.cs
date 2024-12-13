using UnityEngine;

namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Switch</c> represents a switch object.
    /// </summary>
    public class Switch : Interactable
    {
        /// <value>Property <c>houseSet</c> represents the house set of the switch.</value>
        public HouseSet houseSet;

        /// <summary>
        /// Method <c>Initialize</c> initializes the switch object.
        /// </summary>
        /// <param name="setName">The name of the switch object.</param>
        /// <param name="setParent">The parent of the switch object.</param>
        /// <param name="setScale">The scale of the switch object.</param>
        /// <param name="setHouseSet">The house set of the switch object.</param>
        public override void Initialize(string setName, Transform setParent, Vector3 setScale, HouseSet setHouseSet)
        {
            gameObject.name = setName;
            transform.SetParent(setParent);
            transform.localScale = setScale;
            houseSet = setHouseSet;
            PreventInteraction(false);
        }

        /// <summary>
        /// Method <c>HandleLeftClickUp</c> handles the left click up event.
        /// </summary>
        protected override void HandleLeftClickUp()
        {
            base.HandleLeftClickUp();
            PreventInteraction();
            houseSet.CreateKey();
        }
    }
}
