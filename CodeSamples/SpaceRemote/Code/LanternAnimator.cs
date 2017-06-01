using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternAnimator : MonoBehaviour {
    private AudioSource m_AudioSource;
    private float sinOffset;
    public float flickerSpeed, flickerWidth, lightRange;
    public GameObject zoneSphere;
    private Vector3 lastPos, newPos;
    private float posLerp = 0f;
    public float waveLength;
    public float wobbleSpeed = 1;
    public float lanternColorSpeed;
    private float lerpVal = 0f;
    private MeshRenderer renderer;
    private Light luz;
    private Rigidbody rigidbody;
    private SphereCollider collider;
    public float currentColorVal = 0f;
    private Color currentColor, currentLightColor;
    private Color newColor, newLightColor;
    private float lastColorSwitchTime;
    private float[] warpOffset;
	void Start () {
        m_AudioSource = GetComponent<AudioSource>();
        warpOffset = new float[3];
        for (int  i = 0; i < 3; i++)
        {
            warpOffset[i] = Random.Range(0f, 1f);
        }
        sinOffset = Random.Range(0, 1f);
        transform.position = zoneSphere.transform.position;

        lastColorSwitchTime = Time.time;
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
        luz = transform.GetChild(0).GetComponent<Light>();
        SetLanternColors();
    }

    void ChangeDirection()
    {
        lastPos = transform.position;
        newPos = (Random.insideUnitSphere * zoneSphere.transform.localScale.x) + zoneSphere.transform.position;
        GetComponent<Rigidbody>().AddForce(Vector3.Normalize(newPos - lastPos) * 150f, ForceMode.Acceleration);
    }

    void SetAudio()
    {
        float velocity = rigidbody.velocity.magnitude;

        m_AudioSource.pitch = 0.8f + (velocity * 0.1f);

        if (CutAudioScript.isCut)
        {
            m_AudioSource.volume = Mathf.Lerp(1f, 0f, Time.time - CutAudioScript.timeStamp);
        } else
        {
            m_AudioSource.volume = Mathf.Lerp(0f, 1f, Time.time - CutAudioScript.timeStamp);
        }
    }
	public void SetLanternColors()
    {
        lastColorSwitchTime = Time.time;
        currentColorVal += Random.Range(0.33f, 0.66f);
        if (currentColorVal >= 1f)
            currentColorVal -= 1f;
        float bizarroVal = currentColorVal - 0.5f;
        if (bizarroVal < 0f)
            bizarroVal += 1;
        currentColor = newColor;
        currentLightColor = newLightColor;
        newColor = Color.HSVToRGB(currentColorVal, 1f, 1f);
        newLightColor = Color.HSVToRGB(bizarroVal, 1f, 1f);
        lerpVal = 0f;
    }
    void LerpLanternColor()
    {
        lerpVal += 0.02f;
        if (lerpVal > 1f)
            lerpVal = 1f;
        luz.color = Color.Lerp(currentLightColor, newLightColor, lerpVal);
        renderer.material.color = Color.Lerp(currentColor, newColor, lerpVal);
        renderer.material.SetColor("_EmissionColor", Color.Lerp(currentColor, newColor, lerpVal));
    }

    void FlickerLantern()
    {
        luz.range = Mathf.Sin( (Time.time + sinOffset) * flickerSpeed) * flickerWidth + lightRange - (flickerWidth/2);
    }

    void WobbleLantern()
    {
        float offset = (Mathf.Sin(Time.time * 1f) * 0.01f) - 0.005f;
        transform.position += new Vector3(offset, offset, offset);

        //below should be very beautiful

        float x = 1f + (Mathf.Sin(Time.time * Vector3.Distance(GetComponent<Rigidbody>().angularVelocity + Vector3.one, Vector3.zero) ) * 0.15f * Mathf.Sin( (Time.time + warpOffset[0])  * waveLength));
        float y = 1f + (Mathf.Cos(Time.time * Vector3.Distance(GetComponent<Rigidbody>().angularVelocity + Vector3.one, Vector3.zero)) * 0.15f * Mathf.Sin((Time.time + warpOffset[1]) * waveLength));
        float z = 1f + (Mathf.Sin(warpOffset[0] + Time.time * Vector3.Distance(GetComponent<Rigidbody>().angularVelocity + Vector3.one, Vector3.zero)) * 0.15f * Mathf.Sin((Time.time + warpOffset[2]) * waveLength));




        transform.localScale = new Vector3(x, y, z);
    }
    // Update is called once per frame
    void Update () {
        SetAudio();
        WobbleLantern();
        FlickerLantern();
        LerpLanternColor();
        if (Random.Range(0, 1f) >= 0.985)//this should be time based
           ChangeDirection();

        if (Random.Range(0, 1f) >= 0.997 && (Time.time - lastColorSwitchTime > 1f))
            SetLanternColors();

            if (Input.GetKeyDown("r"))
              transform.position = zoneSphere.transform.position;
    }



}
