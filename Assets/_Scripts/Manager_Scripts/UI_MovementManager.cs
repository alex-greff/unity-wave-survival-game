/*Alex Greff
19/01/2016
UI_MovementManager
Creates the virtual joystick for the player to move with
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MovementManager : MonoBehaviour {
    private static UI_MovementManager instance;

    public static UI_MovementManager Instance {
        get {
            return instance;
        }
    }

    //Private classes used
    [RequireComponent (typeof(ColorFader))]
    private class UI_Component {
        protected Image image;
        protected GameObject go;
        private ColorFader af;

        private Color initColor;

        protected float default_duration = 0.3f;

        private float t = Mathf.Infinity;

        private Color startColor;
        private Color endColor;

        //Getter and setters
        public Image getImage {
            get {
                return image;
            }
        }

        //Constructors
        public UI_Component (Image image, bool hide) {
            Initialize(image, hide, default_duration);
        }

        public UI_Component (Image image, bool hide, float duration) {
            Initialize(image, hide, duration);
        }

        private void Initialize (Image image, bool hide, float duration) {
            this.image = image;
            go = image.gameObject;
            af = go.GetComponent<ColorFader>();

            initColor = image.color;

            this.default_duration = duration;

            if (hide)
                ClearColor();
        }

        //Smooth fading methods
        public void FadeIn () {
            FadeIn(default_duration);
        }

        public void FadeIn (float duration) {
            af.FadeIn(duration);
        }

        public void FadeOut () {
            FadeOut(default_duration);
        }

        public void FadeOut (float duration) {
            af.FadeOut(duration);   
        }

        //Changes the initial color that the image is defaulted at
        //Doesn't change the color right away
        public void SetInitialColor (Color color) {
            initColor = color;
        }

        //Instantaneous color change methods
        public void ClearColor () {
            af.Cancel(); //Cancel any animations
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }

        public void InitializeColor () {
            af.Cancel(); //Cancel any animations
            image.color = initColor;
        }

        //Changes the postion of the transform
        public void SetPos (Vector3 pos) {
            go.transform.position = pos;
        }
    }

    [RequireComponent(typeof(SmoothSlider))]
    private class Slider_Component : UI_Component {
        private SmoothSlider ss;

        //Constructors
        public Slider_Component (Image image, bool hide) : base (image, hide) {
            Initialize();
        }

        public Slider_Component (Image image, bool hide, float duration) : base (image, hide, duration) {
            Initialize();
        }

        private void Initialize () {
            ss = go.GetComponent<SmoothSlider>();
            image.fillAmount = 0;
        }

        //Sets the amount (in decimal percent) of the slider
        public void SetValue (float amount) {
            ss.StartAnimation(amount, default_duration);
        }

        public void SetValue (float amount, float duration) {
            if (duration <= 0) {
                image.fillAmount = amount;
                return;
            }

            ss.StartAnimation(amount, duration);
        }

        public void SetValue (float amount, bool smooth) {
            if (smooth)
                SetValue(amount);
            else
                image.fillAmount = amount;
        }
    }

    //Display in the inspector
    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private Image[] leftArrows;
    [SerializeField] private Image[] rightArrows;
    [SerializeField] private Image circle;
    [SerializeField] private Image leftLine;
    [SerializeField] private Image rightLine;

    private bool touchActive = false;

    private float xMax;
    private float xMin;

    private Vector2 initialPos = Vector2.zero;

    private UI_Component[] _leftArrows;
    private UI_Component[] _rightArrows;
    private UI_Component _circle;
    private Slider_Component _leftLine;
    private Slider_Component _rightLine;

    private float threshold = 0;
    private float percent = 0;

    //Getter and setters
    public float Percent {
        get {
            return percent;
        }
    }

    void Awake () {
        instance = this;
    }

	void Start () {
        //Associate Component scripts with each UI object for the movement control

        _leftArrows = new UI_Component[leftArrows.Length];
        for (int i = 0; i < leftArrows.Length; i++)
            _leftArrows[i] = new UI_Component(leftArrows[i], true);

        _rightArrows = new UI_Component[rightArrows.Length];
        for (int i = 0; i < rightArrows.Length; i++)
            _rightArrows[i] = new UI_Component(rightArrows[i], true);

        _circle = new UI_Component(circle, true);
        _leftLine = new Slider_Component(leftLine, true);
        _rightLine = new Slider_Component(rightLine, true);
	}

    public void TouchInitialize (Vector2 startPos) { //For the beginning of a touch input
        if (PauseManager.Instance.Paused) return;

        initialPos = startPos;
        touchActive = true;

        parentTransform.position = startPos;

        percent = 0;

        UpdateBoundaryPos();

        //Fade in the base of the movement GUI
        _circle.FadeIn();
        _leftLine.FadeIn();
        _rightLine.FadeIn();

        //Reset the line amounts
        _leftLine.SetValue(0, false);
        _rightLine.SetValue(0, false);

        //Reset the opacity of the arrows
        for (int i = 0; i < _leftArrows.Length; i++)
            _leftArrows[i].ClearColor();

        for (int i = 0; i < _rightArrows.Length; i++)
            _rightArrows[i].ClearColor();
    }

    public void TouchUpdate (Vector2 endPos) { //For the change in position of a touch input
        if (PauseManager.Instance.Paused) return;

        UpdateMovementGUI(endPos);
    }

    public void TouchEnd () { //For the end of a touch
        if (PauseManager.Instance.Paused) return;

        percent = 0;
        touchActive = false;

        _circle.FadeOut();
        _leftLine.FadeOut();
        _rightLine.FadeOut();

        for (int i = 0; i < _leftArrows.Length; i++)
            _leftArrows[i].FadeOut();

        for (int i = 0; i < _rightArrows.Length; i++)
            _rightArrows[i].FadeOut();
    }

    private void UpdateBoundaryPos () { //Gets an updated version of the boundaries of the movement GUI box
        xMax = parentTransform.rect.xMax;
        xMin = parentTransform.rect.xMin;
    }

    private void UpdateMovementGUI (Vector2 pos) { //Updates the GUI 
        //float percent = 0;
        float maxDiff;
        float diff = pos.x - initialPos.x;

        if (diff > threshold) { //If it's to the right of the begining point
            //maxDiff = Mathf.Abs(initialPos.x - xMin) / 4;
            maxDiff = 275f / 2;
            diff = Mathf.Abs(diff);

            percent = diff / maxDiff;

            leftLine.fillAmount = 0;
            rightLine.fillAmount = percent;

            ShowArrows(_rightArrows, _leftArrows, percent);
        }
        else if (diff < -threshold) { //If it's to the left begining point
            //maxDiff = Mathf.Abs(initialPos.x - xMin) / 4;
            maxDiff = 275f / 2;
            diff = Mathf.Abs(diff);

            percent = diff / maxDiff;

            leftLine.fillAmount = percent;
            rightLine.fillAmount = 0;

            ShowArrows(_leftArrows, _rightArrows, percent);
        }
        else {
            percent = 0;
            leftLine.fillAmount = 0;
            rightLine.fillAmount = 0;

            for (int i = 0; i < _leftArrows.Length; i++)
                _leftArrows[i].FadeOut();

            for (int i = 0; i < _rightArrows.Length; i++)
                _rightArrows[i].FadeOut();
        }
    }

    private void ShowArrows (UI_Component[] currentArrows, UI_Component[] otherArrows, float percent) { //Displays the arrows based off the percent
        for (int i = 0; i < otherArrows.Length; i++)
            otherArrows[i].FadeOut();

        int startIndex = 0;

        if (percent > 0.35f) {
            currentArrows[0].FadeIn();
            startIndex = 1;

            if (percent > 0.58f) {
                currentArrows[1].FadeIn();

                startIndex = 2;

                if (percent > 0.77f) {
                    currentArrows[2].FadeIn();

                    startIndex = 3;

                    if (percent > 0.95f) {
                        currentArrows[3].FadeIn();

                        startIndex = 4;
                    }
                }
            }
        }
            
        for (int i = startIndex; i < currentArrows.Length; i++) { //Fade out any arrows that are needed to be faded
            currentArrows[i].FadeOut();
        }
    }
}
