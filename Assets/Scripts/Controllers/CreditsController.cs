using UnityEngine;
using UnityEngine.UI;
using PKDS.Managers;

namespace PKDS.Controllers
{
    /// <summary>
    /// Class <c>CreditsController</c> handles the credits panel logic.
    /// </summary>
    public class CreditsController : MonoBehaviour
    {
        /// <value>Property <c>backButton</c> represents the back button.</value>
        [SerializeField]
        private Button backButton;

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        /// <summary>
        /// Method <c>OnBackButtonClick</c> handles the back button click event.
        /// </summary>
        private void OnBackButtonClick()
        {
            UIManager.Instance.mainMenuPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
