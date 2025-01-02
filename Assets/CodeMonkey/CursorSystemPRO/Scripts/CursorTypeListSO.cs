using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CursorSystemPro {

    /// <summary>
    /// This Scriptable Object holds references to whatever cursors you create
    /// This lets you easily do logic on any other script like testing if the 
    /// current cursor is a specific one or set a specific one
    /// </summary>
    //[CreateAssetMenu(menuName = "CodeMonkey/CursorSystem/CursorTypeList"))] // Uncomment to create, you should only ever create a single one
    public class CursorTypeListSO : ScriptableObject {

        /// <summary>Static Instance with auto creation</summary>
        private static CursorTypeListSO instance;
        public  static CursorTypeListSO Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load<CursorTypeListSO>(nameof(CursorTypeListSO));
                }
                return instance;
            }
            set {
                instance = value;
            }
        }

        /// <summary>Complete list that is intended to be filled with all the CursorTypes in your project for ease of access</summary>
        public List<CursorTypeSO> list;

        // Individual references
        public CursorTypeSO arrow;
        public CursorTypeSO arrowBig;
        public CursorTypeSO attack;
        public CursorTypeSO circle;
        public CursorTypeSO codeMonkeyHeadMinimalist;
        public CursorTypeSO constructionHammer;
        public CursorTypeSO cross;
        public CursorTypeSO cursorHandClick;
        public CursorTypeSO cursorHandUnClick;
        public CursorTypeSO dollar;
        public CursorTypeSO gamepad;
        public CursorTypeSO gear;
        public CursorTypeSO grab;
        public CursorTypeSO happyUnhappy;
        public CursorTypeSO health;
        public CursorTypeSO mouseLeft;
        public CursorTypeSO mouseRight;
        public CursorTypeSO move;
        public CursorTypeSO no;
        public CursorTypeSO royaleWithCheese;
        public CursorTypeSO save;
        public CursorTypeSO squareDashes;
        public CursorTypeSO sword;
        public CursorTypeSO tick;
        public CursorTypeSO unit;

    }

}