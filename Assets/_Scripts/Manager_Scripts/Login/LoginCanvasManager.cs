using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvasManager : ToggleCanvas {
    public static LoginCanvasManager current;

    protected override void Start() {
        current = this;
        base.Start();
    }
}
