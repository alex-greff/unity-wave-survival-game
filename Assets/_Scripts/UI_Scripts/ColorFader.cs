/*Alex Greff
19/01/2016
ColorFader
Used to smoothly fade components
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFader : Fader {
    private enum State {
        COLOR_CHANGE, FADE_IN, FADE_OUT, CYCLE, NONE
    }

    protected MaskableGraphic graphic;

    protected Color initColor;
    protected Color startColor, endColor;

    private State state;

    private IEnumerator cycle;
   

    protected override void Awake () {
        graphic = GetComponent<MaskableGraphic>(); //Get maskable graphic attached to the component

        base.Awake();
    }

    protected override void Start() {
        initColor = graphic.color; //Set intitial color value

        base.Start(); 
    }

    protected override void UpdateEvent() { //Called whenever it's fading
        graphic.color = Color.Lerp(startColor, endColor, t); //Lerp the color smoothly

        base.UpdateEvent();
    }

    protected override void CancelEvent() {
        state = State.NONE; //Set state to none

        base.CancelEvent();
    }

    protected override void EndEvent() {
        graphic.color = endColor; //Set end color to the end color

        state = State.NONE; //Set state to none

        base.EndEvent();
    }

    public void FadeIn () {
        FadeIn(initialDuration);
    }

    public void FadeIn (float duration) {
        if (state != State.FADE_IN) {
            EndColorCycle(); //Make sure no color cycle is running

            Begin(new Color (graphic.color.r, graphic.color.g, graphic.color.b, initColor.a), State.FADE_IN, duration);
        }
    }

    public void FadeOut () {
        FadeOut(initialDuration);
    }

    public void FadeOut (float duration) {
        if (state != State.FADE_OUT) {
            EndColorCycle(); //Make sure no color cycle is running

            Color end = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);

            Begin(end, State.FADE_OUT, duration);
        }
    }

    public void ResetColor () {
        ResetColor(initialDuration);
    }

    public void ResetColor (float duration) {
        SetColor(initColor, duration);
    }

    public void SetColor (Color to) {
        SetColor(to, initialDuration);
    }

    public void SetColor (Color to, float duration) {
        if (state != State.COLOR_CHANGE || !endColor.Equals(to)) { //If the stage isn't changing color OR the target color changed 
            Begin(to, State.COLOR_CHANGE, duration);
        }
    }

    public void SetInitialColor (Color color) {
        initColor = color;
    }

    public void SetColorCycle (Color[] colors, float duration) {
        cycle = run_cycle(colors, duration);
        StartCoroutine(cycle);
    }

    public void EndColorCycle () {
        Cancel();
    }

    private void Begin (QueueState queueState) {
        Begin(queueState.color, queueState.state, queueState.duration);
    }

    private void Begin (Color to, State state, float duration) { //Begins the colorfading
        if (this.state == State.CYCLE && state != State.CYCLE) { //If it was cycling beforehand and the state it's changing to is NOT cycling
            //Stop the cycle
            if (cycle != null) {
                StopCoroutine(cycle);
            }
        }

        if (duration <= 0) { //Instantly set color if duration is under zero
            graphic.color = to;

            this.state = State.NONE;
        }
        else {
            startColor = graphic.color;
            endColor = to;

            this.state = state;

            base.StartAnim(duration);
        }
    }

    private IEnumerator run_cycle (Color[] colors, float duration) { //Cycle between colors indefinitely until stopped
        //Make sure the properties are valid
        if (colors == null) yield return null; 
        if (colors.Length < 0 || duration <= 0) yield return null;

        int i = 1;

        while (true) { //Loop through
            i = i % colors.Length; //Get the index based off the iteration count

            Begin(colors[i], State.CYCLE, duration); //Begin a new cycle
             
            yield return new WaitForSeconds(duration); //Wait for that cycle to be over before looping again
            i++;
        }
    }

    public override void Cancel() { //Cancels whatever is color fade is going on
        if (cycle != null)
            StopCoroutine(cycle); //Stop the cycle if its running

        base.Cancel();
    }

    //EVIDENCE: Immutable Object
    private class QueueState { //This class is used for saving states of colors 
        //Instance variables
        public readonly Color color; //The target color
        public readonly State state; //The type of fade behavior it is (Color change, fade in, fade out, cycle, none)
        public readonly float duration; //The duration of the transition

        //Constructor
        public QueueState (Color color, State state, float duration) {
            this.color = color;
            this.state = state;
            this.duration = duration;
        }

        //Methods
        public QueueState transparent () {
            return new QueueState (new Color(color.r, color.g, color.b, 0), state, duration); //Removes the alpha from the color and returns a new instance of it
        }

        public QueueState opaque () {
            return new QueueState (new Color(color.r, color.g, color.b, 1), state, duration); //Sets the alpha to 1 (full) and returns a new instance of it
        }

        public QueueState changeDuration (float duration) { //Makes a new instance of the class with a changed duration value
            return new QueueState (color, state, duration);
        }

        public QueueState changeState (State state) { //Changes the state
            return new QueueState(color, state, duration);
        }
    }
}
