using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// Add to a Game Object to automatically set a Cursor on Enter and Exit
    /// Works with World Game Objects that have a Collider or UI Elements
    /// </summary>
    public class CursorChangeEnterExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField] private CursorTypeSO onEnterCursorType;
        [SerializeField] private CursorTypeSO onExitCursorType;

        private void Awake() {
            // Validate to ensure fields are set
            if (onEnterCursorType == null) {
                Debug.LogError("onEnterCursorType is not selected! " + transform);
            }
            if (onExitCursorType == null) {
                Debug.LogError("onExitCursorType is not selected! " + transform);
            }
        }

        /// <summary>Triggered if object has a Collider</summary>
        private void OnMouseEnter() {
            CursorSystem.SetActiveCursorTypeSO(onEnterCursorType);
        }

        /// <summary>Triggered if object has a Collider</summary>
        private void OnMouseExit() {
            CursorSystem.SetActiveCursorTypeSO(onExitCursorType);
        }

        /// <summary>
        /// Triggered by the Event System, works on UI objects or/and World objects depending on the Raycaster used 
        /// (GraphicsRaycaster, PhysicsRaycaster, Physics2DRaycaster)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData) {
            CursorSystem.SetActiveCursorTypeSO(onEnterCursorType);
        }

        /// <summary>
        /// Triggered by the Event System, works on UI objects or/and World objects depending on the Raycaster used 
        /// (GraphicsRaycaster, PhysicsRaycaster, Physics2DRaycaster)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData) {
            CursorSystem.SetActiveCursorTypeSO(onExitCursorType);
        }

    }

}