/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Thanks!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using UnityEngine;

namespace CodeMonkey.CursorSystemPro {

    //[CreateAssetMenu(fileName = "Readme", menuName = "CodeMonkey/CursorSystem/ReadMe", order = 1)]
    public class Readme_CursorSystemPro : ScriptableObject {

        public Texture2D codeMonkeyHeader;
        public string title;
        public string titlesub;
        public bool loadedLayout;
        public Section[] sections;

        [Serializable]
        public class Section {
            public string heading, linkText, url;
            public string[] textLines;
        }

        /*
           * Hi there!
           * Here is a really easy to use System to make your games stand out!
           * You can use any of the 24 built-in Cursors or draw your own.
           * Load up the Demo Scene to see them all in action.
           * 
           * Video Tutorial
           * - How To + Code Walkthrough
           * - https://www.youtube.com/watch?v=pO27ZnBGy1U
           * 
           * How To Make Cursors (Watch the Video for more detail)
           * - Go to Assets > Create > Code Monkey > Cursor System > CursorType
           * - Draw Texture in any image program
           * - Set Texture Import Settings > Texture Type to Cursor with Read/Write Enabled
           * - Drag Texture onto CursorType Object Texture Array
           * - Set the Offset which is the actual mouse clickable position
           *
           * How To Use
           * - Use the CursorSystem script to set the Starting Cursor
           * - CursorChangeEnter / CursorChangeEnterExit scripts
           * - Call the function CursorSystem.SetActiveCursorTypeSO(cursorType);
           * 
           * 
           * If you find this Asset useful please write a review on the Asset Store page, I'd love to hear your thoughts!
           * Best of luck with your games!
           * - Code Monkey
           * https://youtube.com/c/CodeMonkeyUnity

         * */
    }

}