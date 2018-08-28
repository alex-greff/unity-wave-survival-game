using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineCanvasManager : ToggleCanvas {
    public static OfflineCanvasManager current;

    protected override void Start() {
        current = this;
        base.Start();
    }
}
