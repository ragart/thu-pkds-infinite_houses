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
        /// Method <c>HandleLeftClickUp</c> handles the left click up event.
        /// </summary>
        protected override void HandleLeftClickUp()
        {
            PreventInteraction();
            houseSet.CreateKey();
        }
    }
}
