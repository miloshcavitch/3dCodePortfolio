using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteButtonAnimator : MonoBehaviour {

    private float baseColorPos = 0f;
    private Material[] mats;
    static public float mostForwardColor;
	void Start () {
        mats = new Material[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
        {
            mats[i] = transform.GetChild(i).GetComponent<MeshRenderer>().material;
        }
	}

	// Update is called once per frame
	void Update () {
        baseColorPos += 0.5f * Time.deltaTime;
        if (baseColorPos > 1f)
            baseColorPos -= 1f;
        mostForwardColor = baseColorPos + (1f / 36f * 1f);
        if (mostForwardColor > 1f)
            mostForwardColor -= 1f;

        for (int i = 0; i < transform.childCount; i++)
        {
            float thisColor = baseColorPos + (1f / 36f * i);
            if (thisColor > 1f)
                thisColor -= 1f;
            mats[i].color = Color.HSVToRGB(thisColor, 1f, 1f);
            mats[i].SetColor("_EmissionColor", Color.HSVToRGB(thisColor, 0.6f, 0.6f));

        }
	}
}
