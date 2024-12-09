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
        /// Method <c>HandleLeftClickUp</c> handles the left click up event.
        /// </summary>
        protected override void HandleLeftClickUp()
        {
            PreventInteraction();
            houseSet.RoundEnd();
        }
    }
}