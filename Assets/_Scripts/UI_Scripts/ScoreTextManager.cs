using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextManager : MonoBehaviour {
    private static ScoreTextManager instance;

    [SerializeField] private GameObject textPrefab;

    public static ScoreTextManager Instance {
        get {
            return instance;
        }
    }

	void Awake () {
        instance = this;

        SimplePool.Preload(textPrefab, 5); //Preload 5 instances
    }

    public void SpawnText(Vector3 position, Color color, string text) {
        GameObject go = SimplePool.Spawn(textPrefab, position, Quaternion.identity); //Spawn one in
        StartCoroutine(delaySpawn(go, position, color, text, 0.05f));
    }

    private IEnumerator delaySpawn (GameObject go, Vector3 position, Color color, string text, float delay) {
        yield return new WaitForSeconds(delay);
        go.GetComponent<ScoreTextAnimator>().StartAnim(position, color, text); //Set the display properties
    }
}
