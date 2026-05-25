using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BYOJoystick.UI.Scripts
{
    public class VehicleSelector : MonoBehaviour
    {
        public string          GameName;
        public string          ShortName;
        public TextMeshProUGUI SelectText;
        public GameObject      SaveLoadPanel;
        public Button          SelectButton;
        public Button          SaveButton;
        public Button          LoadButton;
        public GameObject      CopyFromPanel;
        public Button          CopyFromButton;

        public void Initialise(string gameName, string shortName, Action<string> onSelect,
    Action<string> onSave, Action<string> onLoad, Action<string> onCopyFrom)
        {
            GameName = gameName;
            ShortName = shortName;
            SelectText.text = gameName;

            // Capture local copies to avoid closure issues
            string capturedShortName = shortName;

            SelectButton.onClick.AddListener(() => onSelect(capturedShortName));
            SaveButton.onClick.AddListener(() => onSave(capturedShortName));
            LoadButton.onClick.AddListener(() => onLoad(capturedShortName));
            CopyFromButton.onClick.AddListener(() => onCopyFrom(capturedShortName));
        }

        public void SetSelected(bool selected)
        {
            SaveLoadPanel.SetActive(selected);
            CopyFromPanel.SetActive(!selected);
        }
    }
}