using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour {
    public float floatingSpeed = 5f;
    public float opacitySpeed = 5f;

    TextMesh textMesh;
	// Use this for initialization
	void Start () {
        textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        // floating the text
        transform.Translate(Vector3.up * floatingSpeed * Time.deltaTime);
        if (textMesh.color.a > 0f) {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b,
                textMesh.color.a - opacitySpeed * Time.deltaTime);
        }
	}
}
