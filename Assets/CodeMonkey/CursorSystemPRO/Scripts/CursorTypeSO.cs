using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// Holds data for each Cursor Type
    /// </summary>
    [CreateAssetMenu(menuName = "CodeMonkey/CursorSystem/CursorType")]
    public class CursorTypeSO : ScriptableObject {

        /// <summary>
        /// All the frames for this cursor, can be just one
        /// If an element is set to null, it uses the default cursor
        /// </summary>
        public Texture2D[] textureArray;

        /// <summary>
        /// The System supports sprites as well
        /// </summary>
        public Sprite[] spriteArray;

        /// <summary>
        /// Sprites need to be converted into Texture2D to be used, cache them instead of constantly wasting memory
        /// </summary>
        private static Dictionary<Sprite, Texture2D> spriteTextureCacheDictionary;

        /// <summary>
        /// How many frames per second for the animation to play (if more than one frame)
        /// </summary>
        public float frameRate;

        /// <summary>
        /// Pixel coordinates to offset visual from the mouse position 
        /// (0, 0) = Top Left corner
        /// Example: If you have a 32x32 cursor texture and you want the center of that texture to be the mouse hotspot
        ///          then you would set the offset to (16, 16)
        /// </summary>
        public Vector2 offset;


        /// <summary>
        /// Validate if this Cursor has at least one frame
        /// </summary>
        public bool IsValid() {
            return (textureArray != null && textureArray.Length > 0) || (spriteArray != null && spriteArray.Length > 0);
        }

        /// <summary>
        /// Get the number of animation frames for this Cursor
        /// </summary>
        public int GetFrameCount() {
            if (IsUsingSprites()) {
                return spriteArray.Length;
            } else {
                return textureArray.Length;
            }
        }

        /// <summary>
        /// Get the specific texture on this index
        /// </summary>
        public Texture2D GetFrameTexture(int frameIndex) {
            if (IsUsingSprites()) {
                frameIndex = Mathf.Clamp(frameIndex, 0, spriteArray.Length - 1);
                return GetSpriteTexture(spriteArray[frameIndex]);
            } else {
                frameIndex = Mathf.Clamp(frameIndex, 0, textureArray.Length - 1);
                return textureArray[frameIndex];
            }
        }

        /// <summary>
        /// This system supports both Textures and Sprites
        /// </summary>
        private bool IsUsingSprites() {
            return textureArray == null || textureArray.Length == 0;
        }

        /// <summary>
        /// Convert Sprite into a Texture2D
        /// </summary>
        private Texture2D GetSpriteTexture(Sprite sprite) {
            if (spriteTextureCacheDictionary == null) {
                // Initialize Dictionary
                spriteTextureCacheDictionary = new Dictionary<Sprite, Texture2D>();
            }

            if (spriteTextureCacheDictionary.ContainsKey(sprite)) {
                // Already created Texture2D for this Sprite
                return spriteTextureCacheDictionary[sprite];
            }

            Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
            Color[] pixelColorArray = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            spriteTexture.SetPixels(pixelColorArray);
#if UNITY_EDITOR
            spriteTexture.alphaIsTransparency = true; // Editor only
#endif
            spriteTexture.Apply();

            spriteTextureCacheDictionary[sprite] = spriteTexture;

            return spriteTexture;
        }

    }

}