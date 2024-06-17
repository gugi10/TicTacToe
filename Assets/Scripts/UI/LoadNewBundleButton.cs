using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace UI
{
    public class LoadNewBundleButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button loadButton;

        private void Awake()
        {
            loadButton.onClick.AddListener(async () => await HandleLoad());
        }

        private async Task HandleLoad()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                Debug.LogError("Provide bundle name");
                inputField.text = "Provide bundle name";
                return;
            }
            await AssetBundleLoader.LoadNewBundle(inputField.text);
        }
    }
}