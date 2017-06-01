using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
    private bool isDecal = false;//when playing be sure to press the t key to toggle a cool but unfinished decal effect!!
    public GameObject GunTip;
    private LineRenderer lineRender;
    public GameObject TrueTip;
    public Light GunLight;
    float laserColor = 0f;
    float lastShotTime;
    public float laserTimeout;
    public AudioClip m_LaserGun;
    public GameObject AudioObject;
    private AudioSource m_AudioSource;

    public GameObject bulletDecal;

    public GameObject remote;
    private RemoteAnimator remoteAnim;
    void Start () {
        remoteAnim = remote.GetComponent<RemoteAnimator>();
        m_AudioSource = AudioObject.GetComponent<AudioSource>();


        lineRender = GunTip.GetComponent<LineRenderer>();
        lineRender.startWidth = 0.05f;
        lineRender.endWidth = 0.05f;
        lineRender.enabled = false;
        lastShotTime = -10f;
    }
    private void PlayShootingSound()
    {

        m_AudioSource.Play();
    }
    // Update is called once per frame
    void Update () {
        laserColor = RemoteButtonAnimator.mostForwardColor;
        lineRender.material.color = Color.HSVToRGB(laserColor, 1f, 1f);
        lineRender.material.SetColor("_EmissionColor", Color.HSVToRGB(laserColor, 1f, 1f));
        GunLight.color = lineRender.material.color;
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown("t"))
        {
            isDecal = !isDecal;
        }
        if (Input.GetMouseButtonDown(0))
        {
            remoteAnim.isShot = true;
            remoteAnim.wasShot = false;
            Ray ray;
            RaycastHit hit;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
            {
                AudioObject.transform.position = hit.point;

                lineRender.SetPosition(0, hit.point);
                lineRender.SetPosition(1, TrueTip.transform.position);
                GunLight.transform.position = hit.point;
                lineRender.enabled = true;
                GunLight.enabled = true;
                lastShotTime = Time.time;

                if (hit.collider.CompareTag("AliveLantern"))
                {
                    m_AudioSource.pitch = Random.Range(0.8f, 1.2f);

                    PlayShootingSound();
                    hit.rigidbody.AddForce(Vector3.Normalize(hit.collider.gameObject.transform.position - hit.point) * 20f, ForceMode.Impulse);
                    ChangeLanternColor(hit.collider.gameObject);
                }
                if (hit.collider.CompareTag("MusicalNote"))
                {

                    hit.rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
                    hit.transform.gameObject.GetComponent<AudioSource>().Play();
                }

                if (hit.collider.CompareTag("Museum"))
                {
                    if (isDecal)
                    {// toggle this cool effect with the t key!!
                        //create a bulletdecal at this location
                        GameObject newDecal = Instantiate(bulletDecal);
                        newDecal.transform.position = hit.point;
                        newDecal.transform.localRotation = Quaternion.LookRotation(hit.normal) * Quaternion.AngleAxis(90, Vector3.right);

                    }




                }
            }
        }

        if (lineRender.enabled == true && lastShotTime + laserTimeout <= Time.time)
        {
            lineRender.enabled = false;
            GunLight.enabled = false;
            } else if (lineRender.enabled)
            {
                lineRender.SetPosition(1, TrueTip.transform.position);
            }
        }

        void ChangeLanternColor(GameObject lantern) {
            lantern.GetComponent<LanternAnimator>().SetLanternColors();
        }
    }
