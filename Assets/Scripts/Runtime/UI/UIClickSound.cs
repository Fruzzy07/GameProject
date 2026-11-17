using SteppeBastion.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    public class UIClickSound : MonoBehaviour
    {
        private void Awake()
        {
            foreach (Button button in GetComponentsInChildren<Button>(true))
                button.onClick.AddListener(Click);
            foreach (Toggle toggle in GetComponentsInChildren<Toggle>(true))
                toggle.onValueChanged.AddListener(_ => Click());
        }

        private static void Click() => GameAudio.Play(s => s.uiClick, 0.6f);
    }
}
