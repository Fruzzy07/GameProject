using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SteppeBastion.UI
{
    /// <summary>Raises events when the pointer enters or leaves a UI element.</summary>
    public class HoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action Entered;
        public event Action Exited;

        public void OnPointerEnter(PointerEventData eventData) => Entered?.Invoke();
        public void OnPointerExit(PointerEventData eventData) => Exited?.Invoke();
    }
}
