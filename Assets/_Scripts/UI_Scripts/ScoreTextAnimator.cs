using UnityEngine;
using System.Collections;

public class ScoreTextAnimator : MonoBehaviour {
    float lerpRate = 1f;

    private TextMesh textMesh;

    private Color initColor;
    private Vector3 initPos;

    private Vector3 displacement = new Vector3(0, 2, 0);
    private Vector3 targetPos;

    private Color targetColor;

    private bool doAnim = false;

    void Awake () {
        textMesh = GetComponent<TextMesh>();
    }

	void Start () {
        initColor = textMesh.color;
        initPos = transform.position;

        textMesh.color = Color.clear;

        //Set the render sorting order to above everything (same as order in layer)
        Renderer rend = GetComponent<Renderer>();
        //rend.sortingLayerID = 30;
        rend.sortingOrder = 30;
	}
	
	void Update () {
        if (doAnim) {
            Anim();
        }
	}

    void Anim() {
        transform.position = Vector3.Slerp(transform.position, targetPos, lerpRate * Time.deltaTime); //Move position
        textMesh.color = Color.Lerp(textMesh.color, targetColor, lerpRate*1.5f * Time.deltaTime); //Change color

        if (textMesh.color.a <= 0.1f) { //When color has faded enough
            textMesh.color = Color.clear; //Clear color completely
        }

        if (Vector3.Distance(targetPos,transform.position) <= 0.5f) {
            SimplePool.Despawn(gameObject); //Despawn itself
            doAnim = false;
        }
    }

    public void StartAnim(Vector2 pos, Color col, string txt) {
        textMesh.text = txt;

        transform.position = pos;

        initPos = transform.position;
        targetPos = initPos + displacement;


        textMesh.color = col;

        targetColor = col;
        targetColor.a = 0;

        doAnim = true;
    }

    public bool isActive () {
        return doAnim;
    }
}
