namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Key</c> represents a key object.
    /// </summary>
    public class Key : Interactable
    {
        /// <value>Property <c>targetHouseSet</c> represents the target house set of the key.</value>
        public HouseSet targetHouseSet;

        /// <summary>
        /// Method <c>HandleLeftClickUp</c> handles the left click up event.
        /// </summary>
        protected override void HandleLeftClickUp()
        {
            PreventInteraction();
            targetHouseSet.ZoomIn();
        }
    }
}