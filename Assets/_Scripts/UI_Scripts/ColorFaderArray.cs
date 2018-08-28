/*Alex Greff
19/01/2016
ColorFaderArray
Similar to the ColorFader class except it handles fading multiple components
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFaderArray : Fader {
    private enum State {
        COLOR_CHANGE, FADE_IN, FADE_OUT, CYCLE, NONE
    }

    [SerializeField] private bool includeComponentMaskableGraphic = false;

    public List<MaskableGraphic> graphics = new List<MaskableGraphic>();
    protected List<Color> initColors = new List<Color>();
    protected List<Color> startColor = new List<Color>();
    protected List<Color> endColor = new List<Color>();

    private State state;

    private IEnumerator cycle;
   

    protected override void Awake () {

        MaskableGraphic[] childGraphics = GetComponentsInChildren<MaskableGraphic>(); //Get all children maskable graphics

        foreach (MaskableGraphic mg in childGraphics) {
            if (mg.transform.GetComponent<ColorFader_Ignore>() == null) //If they don't have the ignore script on them
                if (mg.transform != transform) //If it's the maskable graphic on the parent transform
                    graphics.Add(mg); //Add them to the list
                else //If it is
                    if (includeComponentMaskableGraphic) //Check if it is wanted to be in the list
                    graphics.Add(mg); //If so, add it
        }

        base.Awake();
    }

    protected override void Start() {
        //initColor = graphic.color;

        //Initialize the lists
        for (int i = 0; i < graphics.Count; i++) {
            initColors.Add(graphics[i].color);
            startColor.Add(Color.black);
            endColor.Add(Color.black);
        }

        base.Start(); 
    }

    protected override void UpdateEvent() {
        //graphic.color = Color.Lerp(startColor, endColor, t);
        for (int i = 0; i < graphics.Count; i++)
            graphics[i].color = Color.Lerp(startColor[i], endColor[i], t);

        base.UpdateEvent();
    }

    protected override void CancelEvent() {
        state = State.NONE;

        base.CancelEvent();
    }

    protected override void EndEvent() {
        state = State.NONE;

        for (int i = 0; i < graphics.Count; i++)
            graphics[i].color = endColor[i];

        base.EndEvent();
        
    }

    public void FadeIn () {
        FadeIn(initialDuration);
    }

    public void FadeIn (float duration) {
        if (state != State.FADE_IN) {
            EndColorCycle(); //Make sure no color cycle is running

            //Get end colors of all graphics
            for (int i = 0; i < graphics.Count; i++)
                endColor[i] = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, initColors[i].a);

            Begin(endColor, State.FADE_IN, duration); //Start the fade
        }
    }

    public void FadeOut () {
        FadeOut(initialDuration);
    }

    public void FadeOut (float duration) {
        if (state != State.FADE_OUT) {
            EndColorCycle(); //Make sure no color cycle is running

            for (int i = 0; i < graphics.Count; i++)
                endColor[i] = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, 0);

            Begin(endColor, State.FADE_OUT, duration); //Start the fade
        }
    }

    public void ResetColor () {
        ResetColor(initialDuration);
    }

    public void ResetColor (float duration) { //Resets the components to the initial color
        if (state != State.COLOR_CHANGE) 
            Begin(initColors, State.COLOR_CHANGE, duration);
    }

    public void SetColor (Color to) {
        SetColor(to, initialDuration);
    }

    public void SetColor (Color to, float duration) { //Sets the color of each component
        if (state != State.COLOR_CHANGE || !endColor.Equals(to)) { //If the stage isn't changing color OR the target color changed 
            Begin(to, State.COLOR_CHANGE, duration); //Start fade
        }
    }

    public void SetInitialColor (Color color) { //Sets what the initial color is
        for (int i = 0; i < graphics.Count; i++) 
            initColors[i] = color;
    }

    public void SetColorCycle (Color[] colors, float duration) {
        cycle = run_cycle(colors, duration);
        StartCoroutine(cycle);
    }

    public void EndColorCycle () {
        Cancel();
    }


    private void Begin (List<Color> to, State state, float duration) {
        if (this.state == State.CYCLE && state != State.CYCLE) { //If it was cycling beforehand and the state it's changing to is NOT cycling
            //Stop the cycle
            if (cycle != null) {
                StopCoroutine(cycle);
            }
        }

        if (duration <= 0) { //Instantly set color if duration is under zero
            //Just set the colors
            for (int i = 0; i < graphics.Count; i++)
                graphics[i].color = to[i];

            this.state = State.NONE;
        }
        else {
            //Get the start colors
            for (int i = 0; i < graphics.Count; i++) {
                startColor[i] = graphics[i].color;
            }

            this.state = state; //Set state

            base.StartAnim(duration); //Start animation
        }
    }

    private void Begin (Color to, State state, float duration) {
        if (this.state == State.CYCLE && state != State.CYCLE) { //If it was cycling beforehand and the state it's changing to is NOT cycling
            //Stop the cycle
            if (cycle != null) {
                StopCoroutine(cycle);
            }
        }

        if (duration <= 0) { //Instantly set color if duration is under zero
            for (int i = 0; i < graphics.Count; i++)
                graphics[i].color = to;


            this.state = State.NONE;
        }
        else {
            //Set start color
            for (int i = 0; i < graphics.Count; i++) {
                startColor[i] = graphics[i].color;
                endColor[i] = to;
            }

            this.state = state; //Set state

            base.StartAnim(duration); //Start animation
        }
    }

    private IEnumerator run_cycle (Color[] colors, float duration) {
        //Make sure the properties are valid
        if (colors == null) yield return null; 
        if (colors.Length < 0 || duration <= 0) yield return null;

        int i = 1;

        while (true) {
            i = i % colors.Length; //Get the index based off the counter

            Begin(colors[i], State.CYCLE, duration); //Begin a new fade

            yield return new WaitForSeconds(duration); //Wait until fade is done
            i++;
        }
    }

    public override void Cancel() { //Cancel fade
        if (cycle != null)
            StopCoroutine(cycle);

        base.Cancel();
    }
}
