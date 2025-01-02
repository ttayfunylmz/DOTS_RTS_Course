using System;
using UnityEngine;

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// Handles Cursors and their Animations
    /// </summary>
    public class CursorSystem : MonoBehaviour {

        /// <summary>Event fired when Cursor Changes</summary>
        public static event EventHandler<OnCursorChangedEventArgs> OnCursorChanged;
        public class OnCursorChangedEventArgs : EventArgs {
            public CursorTypeSO cursorTypeSO;
        }

        /// <summary>Static Instance with auto creation</summary>
        private static CursorSystem instance;
        /// <summary>Static Instance with auto creation</summary>
        private static CursorSystem Instance {
            get {
                if (instance == null) {
                    GameObject gameObject = new GameObject("CursorSystem", typeof(CursorSystem));
                    instance = gameObject.GetComponent<CursorSystem>();
                }
                return instance;
            }
            set {
                instance = value;
            }
        }


        /// <summary>Drag the reference in the editor if you want a cursor to be set on Start</summary>
        [SerializeField] private CursorTypeSO startingCursorTypeSO;

        private CursorTypeSO cursorTypeSO;
        private int currentFrame;
        private float frameTimer;
        private int frameCount;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            if (startingCursorTypeSO != null) {
                // Set Starting Cursor
                SetActiveCursorTypeSO(startingCursorTypeSO);
            }
        }

        private void Update() {
            // Is cursor animated? Does it have more than one frame and a set frameRate?
            if (frameCount > 1 && cursorTypeSO.frameRate > 0f) {
                // Count down timer
                frameTimer -= Time.unscaledDeltaTime;

                bool newFrame = false;
                while (frameTimer <= 0f) {
                    // Enough time elapsed, reset timer
                    frameTimer += 1f / cursorTypeSO.frameRate;
                    // Select next frame (with looping)
                    currentFrame = (currentFrame + 1) % frameCount;

                    newFrame = true;
                }

                if (newFrame) {
                    // Set Cursor visual
                    UpdateCursor();
                }
            }
        }

        private void SetActiveCursorTypeSO_Instance(CursorTypeSO cursorTypeSO) {
            if (!cursorTypeSO.IsValid()) {
                Debug.LogError("Invalid Cursor (" + cursorTypeSO.name + "), did you add any frames?");
            }

            this.cursorTypeSO = cursorTypeSO;
            currentFrame = 0;
            frameTimer = 0f;
            frameCount = cursorTypeSO.GetFrameCount();

            // Set Cursor visual
            UpdateCursor();

            OnCursorChanged?.Invoke(this, new OnCursorChangedEventArgs { cursorTypeSO = cursorTypeSO });
        }

        private void UpdateCursor() {
            Cursor.SetCursor(cursorTypeSO.GetFrameTexture(currentFrame), cursorTypeSO.offset, CursorMode.Auto);
        }

        private CursorTypeSO GetCursorTypeSO_Instance() {
            return cursorTypeSO;
        }



        /// <summary>Set the currently active Cursor Type</summary>
        public static void SetActiveCursorTypeSO(CursorTypeSO cursorTypeSO) {
            Instance.SetActiveCursorTypeSO_Instance(cursorTypeSO);
        }

        /// <summary>Get the currently active Cursor Type</summary>
        public static CursorTypeSO GetCursorTypeSO() {
            return Instance.cursorTypeSO;
        }

    }

}