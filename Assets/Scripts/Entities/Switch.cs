namespace PKDS.Entities
{
    /// <summary>
    /// Class <c>Switch</c> represents a switch object.
    /// </summary>
    public class Switch : Interactable
    {
        /// <value>Property <c>targetHouseSet</c> represents the target house set of the key.</value>
        public HouseSet targetHouseSet;

        /// <summary>
        /// Method <c>HandleLeftClickUp</c> handles the left click up event.
        /// </summary>
        protected override void HandleLeftClickUp()
        {
            PreventInteraction();
            targetHouseSet.CreateKey();
        }
    }
}
