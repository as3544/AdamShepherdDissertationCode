using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR;

public class ShuttlecockLauncher : MonoBehaviour
{
    public GameObject projectile;
    public GameObject target;
    public GameObject qet_ball;
    public GameObject netTape;

    public List<long> shuttleHitPointTimeStamps = new List<long>();

    private GameObject shuttlecock;

    public Animator animationMan;

    public float animationDelay = 0f;
    public float shuttleAirTime = 1f;
    public float qetTrainingTime = 0.3f;

    public float shuttleSpeed = 0.8f;

    public int totalReps = 0;
    public GameObject RepsCounterText;


    public AudioSource shuttleHit;
    public AudioSource shuttleSoftHit;
    public AudioSource badmintonSteps;

    private bool directionChanged = false;
    private bool racketHit = false;
    [NonSerialized]
    public bool playerLeftHanded;
    private int launchesLeft = 0;

    [NonSerialized]
    public bool QET;

    public GameObject leftRacket;
    public GameObject rightRacket;
    public GameControl gameControl;

    public Color[] colors = {new Color(51f/255f, 219f/255f, 0), new Color(0, 91f / 255f, 219f / 255f), new Color(219f / 255f, 0, 22f / 255f), 
        new Color(190f / 255f, 0, 219f / 255f), new Color(219f / 255f, 205f / 255f, 0) };

    //public float launchVelocity = 1f;
    //public Vector3 target = new Vector3(0f, 0f, 0f);

    public float secondsSinceLastLaunch = 0;

    // Start is called before the first frame update
    void Start()
    {
        // frame animation hit = 5.2 seconds
        // animation x4 speed
        // so hit point = 5.2/4 = 1.3

        if (playerLeftHanded)
        {
            leftRacket.SetActive(true);
            rightRacket.SetActive(false);
        }
        else
        {
            leftRacket.SetActive(false);
            rightRacket.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        secondsSinceLastLaunch += Time.deltaTime;

        if (secondsSinceLastLaunch > 6 && launchesLeft > 0)
        {
            launchesLeft--;
            UpdateRepsCounterText();
            FireNewShuttlecock();

            if (launchesLeft == 0)
            {
                Invoke(nameof(CallEndOfRepetition), 10);
            }
        }

        // update any existing shuttlecock position
        if (shuttlecock)
        {
            if (shuttlecock.GetComponent<Rigidbody>().velocity.magnitude > 0.05)
            {
                Quaternion lookRotation = Quaternion.LookRotation(shuttlecock.GetComponent<Rigidbody>().velocity) * Quaternion.Euler(90, 0, 0);
                shuttlecock.transform.rotation = lookRotation;
            }
        }

    }

    void UpdateRepsCounterText()
    {
        RepsCounterText.GetComponent<TextMeshProUGUI>().text = (totalReps - launchesLeft).ToString() + "/" + totalReps.ToString();
    }

    void CallEndOfRepetition()
    {
        gameControl.RepetitionsFinished();
    }

    private IEnumerator CountDownUntilRepetition(int count, bool isQET, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        QET = isQET;
        launchesLeft = count;
        totalReps = count;
        UpdateRepsCounterText();
    }

    public void RunRepetitions(int count, bool isQET)
    {
        StartCoroutine(CountDownUntilRepetition(count, isQET, 5));
    }

    private void FireNewShuttlecock()
    {
        if (shuttlecock) Destroy(shuttlecock); shuttlecock = null;
        directionChanged = false;
        racketHit = false;
        shuttlecock = Instantiate(projectile, transform.position, transform.rotation);
        Invoke(nameof(PlayOverheadHit), animationDelay);
        if (QET) Invoke(nameof(ShowQETBall), shuttleAirTime);
        FireShuttle(target.transform.position, shuttleAirTime);

        secondsSinceLastLaunch = 0;
    }

#nullable enable
    public void ShuttleHitTarget(string hitName, Transform? racketTransform = null, Vector3? collisionNormal = null)
    {
        if (hitName == "Target")
        {
            if (directionChanged == false)
            {
                // find camera
                shuttleHit.Play(0);
                shuttleHitPointTimeStamps.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    FireShuttle(Camera.main.gameObject.transform.position + new Vector3(-1, -1, 0), 0.5f);
                }
                else
                {
                    FireShuttle(Camera.main.gameObject.transform.position + new Vector3(1, -1, 0), 0.5f);
                }
                directionChanged = true;
            }
        }
        else if (hitName == "RacketHitBox" && racketTransform != null && collisionNormal != null)
        {
            if (racketHit == false)
            {
                racketHit = true;
                shuttleSoftHit.Play(0);
                VibrateController();

                // calc z
                float z = netTape.transform.position.z - racketTransform.position.z;

                // find how many collision normal z's are required to reach the net
                float numZs = z / collisionNormal.Value.z;

                // now we know num Xs
                float x = collisionNormal.Value.x * numZs;

                float xdiff = x - racketTransform.position.x;

                // calc x
                //float x = z * (1 / Mathf.Tan(theta));

                // now calc pos over net
                Vector3 positionCrossNet = new Vector3(racketTransform.position.x, netTape.transform.position.y + 0.2f, netTape.transform.position.z);

                FireShuttle(positionCrossNet, shuttleSpeed);
            }
        }
    }
#nullable disable

    private void PlayOverheadHit()
    {
        badmintonSteps.Play(0);
        animationMan.SetTrigger("PlayOverhead");
    }

    private void ShowQETBall()
    {
        qet_ball.GetComponent<Renderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        qet_ball.SetActive(true);
        Invoke(nameof(HideQETBall), qetTrainingTime);
    }

    private void HideQETBall()
    {
        qet_ball.SetActive(false);
    }

    private void FireShuttle(Vector3 TargetPosition, float time)
    {

        Rigidbody rigidShuttle = shuttlecock.GetComponent<Rigidbody>();
        rigidShuttle.freezeRotation = true;

        Vector3 shuttleVelocity = CalculateRequiredVelocityToHitTarget(rigidShuttle.position, TargetPosition, time);
        rigidShuttle.AddForce(shuttleVelocity - new Vector3(rigidShuttle.velocity.x, rigidShuttle.velocity.y, rigidShuttle.velocity.z), ForceMode.VelocityChange);
    }

    private Vector3 CalculateRequiredVelocityToHitTarget(Vector3 startPosition, Vector3 endPosition, float time)
    {
        // u = sqrt(v^2 - 2as)
        Vector3 displacement = endPosition - startPosition;
        Vector3 acceleration = Physics.gravity;

        float initialVelocityx = (displacement.x - 0.5f * acceleration.x * Mathf.Pow(time, 2)) / time;
        float initialVelocityy = (displacement.y - 0.5f * acceleration.y * Mathf.Pow(time, 2)) / time;
        float initialVelocityz = (displacement.z - 0.5f * acceleration.z * Mathf.Pow(time, 2)) / time;

        return new Vector3(initialVelocityx, initialVelocityy, initialVelocityz);
    }

    private void VibrateController()
    {
        UnityEngine.XR.HapticCapabilities capabilitiesR;
        UnityEngine.XR.HapticCapabilities capabilitiesL;
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetHapticCapabilities(out capabilitiesR);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetHapticCapabilities(out capabilitiesL);
        if ((playerLeftHanded && capabilitiesL.supportsImpulse) || (!playerLeftHanded && capabilitiesR.supportsImpulse))
        {
            uint channel = 0;
            float amplitude = 1.0f;
            float duration = 0.1f;
            if (playerLeftHanded)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(channel, amplitude, duration);
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }

}

