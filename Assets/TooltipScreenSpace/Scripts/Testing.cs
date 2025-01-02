using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    private float timer;

    private void Start() {

        System.Func<string> getTooltipTextFunc = () => {
            return "<color=#00ff00>This is my tooltip!</color>\nThere are many like it but this one is mine!\n" + 
            "<color=#F00>" + timer + "</color>";
        };
        TooltipScreenSpaceUI.ShowTooltip_Static(getTooltipTextFunc, null);

        //TooltipScreenSpaceUI.ShowTooltip_Static("Hello World!");
    }

    private void Update() {
        timer += Time.deltaTime;
    }

}
