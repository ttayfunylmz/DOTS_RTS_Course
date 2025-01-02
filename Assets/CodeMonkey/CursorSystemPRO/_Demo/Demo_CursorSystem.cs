using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// Demo showcasing some simple logic working with cursors and objects
    /// It handles clicking on the Soldier to select it, like in an RTS game
    /// Then it listens to the CursorSystem event when the cursor changes
    /// It uses the CursorTypeListSO object to compare with the Arrow cursor
    /// If it matches and the Soldier is selected, change it for the Move cursor
    /// 
    /// It's some simple logic to demonstrate how you can easily use the Event
    /// and the CursorTypeListSO to make a dynamic cursor that responds to the Player 
    /// </summary>
    public class Demo_CursorSystem : MonoBehaviour {

        // References to make the Select/Deselect logic work
        [SerializeField] private Collider2D soldierCollider;
        [SerializeField] private GameObject selectedGameObject;

        private void Start() {
            // Listen to event when cursor changes
            CursorSystem.OnCursorChanged += CursorSystem_OnCursorChanged;
        }

        private void Update() {
            if (IsMouseButtonDown()) {
                // Test if clicked on Soldier
                RaycastHit2D raycastHit2D = GetMousePositionRaycastHit2D();
                if (raycastHit2D.collider == soldierCollider) {
                    // Clicked on Soldier
                    selectedGameObject.SetActive(true);
                } else {
                    // Deselect Soldier
                    selectedGameObject.SetActive(false);
                    if (CursorSystem.GetCursorTypeSO() == CursorTypeListSO.Instance.move) {
                        CursorSystem.SetActiveCursorTypeSO(CursorTypeListSO.Instance.arrow);
                    }
                }
            }
        }

        private void CursorSystem_OnCursorChanged(object sender, CursorSystem.OnCursorChangedEventArgs e) {
            // If Unit is Selected swap Arrow cursor for Move Cursor
            if (e.cursorTypeSO == CursorTypeListSO.Instance.arrow) {
                if (selectedGameObject.activeSelf) {
                    CursorSystem.SetActiveCursorTypeSO(CursorTypeListSO.Instance.move);
                }
            }
        }

        private void OnDestroy() {
            // Unsubscribe to the Event
            CursorSystem.OnCursorChanged -= CursorSystem_OnCursorChanged;
        }

        private bool IsMouseButtonDown() {
#if ENABLE_LEGACY_INPUT_MANAGER
            // Using the Legacy Input System
            return Input.GetMouseButtonDown(0);
#elif ENABLE_INPUT_SYSTEM
            // Using the Input System Package
            return Mouse.current.leftButton.isPressed;
#else
            return false;
#endif
        }

        private RaycastHit2D GetMousePositionRaycastHit2D() {
#if ENABLE_LEGACY_INPUT_MANAGER
            // Using the Legacy Input System
            return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
#elif ENABLE_INPUT_SYSTEM 
            // Using the Input System Package
            return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
#else
            return new RaycastHit2D();
#endif
        }

    }

}
