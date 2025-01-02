using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// Custom Editor for CursorTypeSO
    /// </summary>
    [CustomEditor(typeof(CursorTypeSO))]
    public class CursorTypeSOEditor : Editor {

        private PreviewCursor previewCursor;
        private MarkerVisual markerVisual;

        private float deltaTime;
        private double lastRealTimeSinceStartup;

        public override void OnInspectorGUI() {
            serializedObject.Update();

            // Calculate Editor DeltaTime
            CalculateDeltaTime();

            // Grab the selected CursorTypeSO
            CursorTypeSO cursorTypeSO = (CursorTypeSO)target;

            if (cursorTypeSO == null) {
                // No cursor selected, disable preview cursor
                previewCursor = null;
            }

            if (previewCursor == null) {
                // There's no previewCursor already built, create one
                previewCursor = new PreviewCursor(cursorTypeSO);
            } else {
                // There's a previewCursor that already exists, replace?
                if (previewCursor.GetCursorTypeSO() != cursorTypeSO) {
                    // Selected CursorTypeSO is different, update PreviewCursor
                    previewCursor = new PreviewCursor(cursorTypeSO);
                }
            }

            // Create the MarkerVisual for the Offset Marker Texture
            if (markerVisual == null) {
                markerVisual = new MarkerVisual();
            }

            // Header Label
            GUILayout.Label("CUSTOM CURSOR", new GUIStyle { fontStyle = FontStyle.Bold });

            if (cursorTypeSO.spriteArray != null && cursorTypeSO.spriteArray.Length > 0) {
                // Show Sprite Array if filled
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteArray"));
            } else {
                // No Sprites, show normal Texture array
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textureArray"));
            }

            // Test if should show frameRate field
            if ((cursorTypeSO.textureArray != null && cursorTypeSO.textureArray.Length > 1) ||
                (cursorTypeSO.spriteArray != null && cursorTypeSO.spriteArray.Length > 1)) {
                // Has more than one frame
                if (cursorTypeSO.frameRate == 0) {
                    // Set default frameRate
                    cursorTypeSO.frameRate = 5;
                }
                // Show property
                EditorGUILayout.PropertyField(serializedObject.FindProperty("frameRate"));
            } else {
                // No frames or a single frame, don't show frameRate
            }

            // Show normal properties
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"));

            // Texture that is used as the preview
            Texture mainTexture = null;

            if (cursorTypeSO.textureArray != null && cursorTypeSO.textureArray.Length > 0 && cursorTypeSO.textureArray[0] != null) {
                // Has Texture, show this as preview
                mainTexture = cursorTypeSO.textureArray[0];
            }

            if (cursorTypeSO.spriteArray != null && cursorTypeSO.spriteArray.Length > 0 && cursorTypeSO.spriteArray[0] != null) {
                // Has Sprite, show this as preview
                mainTexture = GetSpriteTexture(cursorTypeSO.spriteArray[0]);
            }

            // Identify if user set a texture with multiple sprites
            if ((cursorTypeSO.spriteArray == null || cursorTypeSO.spriteArray.Length == 0) && cursorTypeSO.textureArray != null && cursorTypeSO.textureArray.Length > 0 && cursorTypeSO.textureArray[0] != null) {
                mainTexture = cursorTypeSO.textureArray[0];

                // Find child sprites if any
                string spritesheetPath = AssetDatabase.GetAssetPath(mainTexture);
                Object[] childObjectArray = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);

                if (childObjectArray.Length > 1) {
                    // More than one, has child sprites
                    List<Sprite> spriteList = new List<Sprite>();
                    foreach (Object childObject in childObjectArray) {
                        if (childObject != null && childObject is Sprite) {
                            Sprite sprite = (Sprite)childObject;
                            spriteList.Add(sprite);
                        }
                    }

                    // Replace the Texture Array for the Sprite Array
                    if (spriteList.Count > 0) {
                        cursorTypeSO.spriteArray = spriteList.ToArray();
                        cursorTypeSO.textureArray = null; // Clear Texture if there are sprites

                        if (cursorTypeSO.frameRate == 0) {
                            cursorTypeSO.frameRate = 5;
                        }

                        // Grab mainTexture from first Sprite
                        mainTexture = GetSpriteTexture(cursorTypeSO.spriteArray[0]);
                    }
                }
            }

            // Does it have a preview cursor?
            if (previewCursor != null) {
                // Update it, possibly changing current frame
                if (Event.current.type == EventType.Repaint) {
                    // Only Update using deltaTime on Repaint event, not on Layout
                    previewCursor.Update(deltaTime);
                }
                // Get the current preview texture
                mainTexture = previewCursor.GetCursorTexture();
            }

            if (Event.current.type == EventType.Repaint) {
                // Only Update using deltaTime on Repaint event, not on Layout
                markerVisual.Update(deltaTime);
            }

            // Display preview texture if there is one
            if (mainTexture != null) {
                int previewBoxSize = 100;
                GUILayout.Box(mainTexture, GUILayout.Width(previewBoxSize), GUILayout.Height(previewBoxSize));
                Rect rect = GUILayoutUtility.GetLastRect();
                EditorGUI.DrawTextureTransparent(rect, mainTexture);

                // Validate Offset
                cursorTypeSO.offset.x = Mathf.Clamp(cursorTypeSO.offset.x, 0, mainTexture.width);
                cursorTypeSO.offset.y = Mathf.Clamp(cursorTypeSO.offset.y, 0, mainTexture.height);

                Rect markerRect = rect;
                float markerSize = 4f;
                markerRect.x += (cursorTypeSO.offset.x / mainTexture.width) * previewBoxSize - (markerSize / 2f);
                markerRect.y += (cursorTypeSO.offset.y / mainTexture.height) * previewBoxSize - (markerSize / 2f);
                markerRect.width = markerSize;
                markerRect.height = markerSize;

                EditorGUI.DrawPreviewTexture(markerRect, markerVisual.GetTexture());
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>Set Editor to constantly Repaint in order to make the Preview work</summary>
        public override bool RequiresConstantRepaint() { 
            return true; 
        }

        /// <summary>Get Editor DeltaTime</summary>
        private void CalculateDeltaTime() {
            if (Event.current.type == EventType.Repaint) {
                // Only Calculate on Repaint
                deltaTime = (float)(EditorApplication.timeSinceStartup - lastRealTimeSinceStartup);
                lastRealTimeSinceStartup = EditorApplication.timeSinceStartup;
            }
        }

        /// <summary>Convert Sprite to a Texture2D</summary>
        private Texture2D GetSpriteTexture(Sprite sprite) {
            Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixelColorArray = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            spriteTexture.SetPixels(pixelColorArray);
            spriteTexture.Apply();

            return spriteTexture;
        }




        /// <summary>
        /// Handles the PreviewCursor
        /// Runs the Cursor animation logic in the Editor so you can easily see the animation in action
        /// </summary>
        private class PreviewCursor {

            private CursorTypeSO cursorTypeSO;
            private int currentFrame;
            private float frameTimer;

            public PreviewCursor(CursorTypeSO cursorTypeSO) {
                this.cursorTypeSO = cursorTypeSO;
                currentFrame = 0;
                frameTimer = 0f;
            }

            public void Update(float deltaTime) {
                int frameCount = cursorTypeSO.GetFrameCount();

                if (frameCount > 1 && cursorTypeSO.frameRate > 0f) {
                    // Count down timer
                    frameTimer -= deltaTime;
                    while (frameTimer <= 0f) {
                        // Enough time elapsed, reset timer
                        frameTimer += 1f / cursorTypeSO.frameRate;
                        // Select next frame (with looping)
                        currentFrame = (currentFrame + 1) % frameCount;
                    }
                }
            }

            public Texture2D GetCursorTexture() {
                if (cursorTypeSO.IsValid()) {
                    return cursorTypeSO.GetFrameTexture(currentFrame);
                } else {
                    return null;
                }
            }

            public CursorTypeSO GetCursorTypeSO() {
                return cursorTypeSO;
            }

        }



        /// <summary>
        /// Handles the Marker Visual
        /// Changes marker color to make it easy to see the marker on top of any texture
        /// </summary>
        private class MarkerVisual {

            private const float FRAME_RATE = 5f;

            private static Texture2D redTexture;
            private static Texture2D greenTexture;


            private float timer;
            private bool isRed;

            public MarkerVisual() {
                timer = 0f;

                redTexture = GetMarkerTexture(Color.red);
                greenTexture = GetMarkerTexture(Color.green);
            }

            public void Update(float deltaTime) {
                // Count down timer
                timer -= deltaTime;
                while (timer <= 0f) {
                    // Enough time elapsed, reset timer
                    timer += 1f / FRAME_RATE;
                    // Change color
                    isRed = !isRed;
                }
            }

            public Texture2D GetTexture() {
                if (isRed) {
                    return redTexture;
                } else {
                    return greenTexture;
                }
            }

            /// <summary>Helper function to create the marker texture</summary>
            private Texture2D GetMarkerTexture(Color color) {
                int width = 3;
                int height = 2;

                Color[] colorArray = new Color[width * height];

                for (int i = 0; i < colorArray.Length; i++) {
                    colorArray[i] = i % 2 == 0 ? color : Color.grey;
                }

                Texture2D texture2D = new Texture2D(width, height);
                texture2D.SetPixels(colorArray);
                texture2D.Apply();

                return texture2D;
            }

        }
    }

}