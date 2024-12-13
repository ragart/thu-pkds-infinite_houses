using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PKDS.Managers;

namespace PKDS.Controllers
{
    /// <summary>
    /// Class <c>MenuController</c> handles the menu panel logic.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <value>Property <c>titleText</c> represents the title text.</value>
        [Header("Menu Properties")]
        [SerializeField]
        private TextMeshProUGUI titleText;
        
        /// <value>Property <c>TitleText</c> represents the title text value.</value>
        public string TitleText
        {
            get => titleText.text;
            set => titleText.text = value;
        }
        
        /// <value>Property <c>mainMenuButton</c> represents the main menu button.</value>
        [SerializeField]
        private Button mainMenuButton;
        
        /// <value>Property <c>quitGameButton</c> represents the quit game button.</value>
        [SerializeField]
        private Button quitGameButton;
        
        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
            quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
        }

        /// <summary>
        /// Method <c>OnMainMenuButtonClick</c> handles the main menu button click event.
        /// </summary>
        private static void OnMainMenuButtonClick()
        {
            GameManager.Instance.MainMenu();
        }
        
        /// <summary>
        /// Method <c>OnQuitGameButtonClick</c> handles the quit game button click event.
        /// </summary>
        private static void OnQuitGameButtonClick()
        {
            GameManager.QuitGame();
        }
    }
}
