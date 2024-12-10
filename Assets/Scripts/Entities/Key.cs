using UnityEngine;

namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Key</c> represents a key object.
    /// </summary>
    public class Key : Interactable
    {
        /// <value>Property <c>houseSet</c> represents the house set of the key.</value>
        public HouseSet houseSet;

        /// <summary>
        /// Method <c>Initialize</c> initializes the key object.
        /// </summary>
        /// <param name="setName">The name of the key object.</param>
        /// <param name="setParent">The parent of the key object.</param>
        /// <param name="setScale">The scale of the key object.</param>
        /// <param name="setHouseSet">The house set of the key object.</param>
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
            PreventInteraction();
            houseSet.RoundEnd();
        }
    }
}