using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CuttingStation : MonoBehaviour
{
    [Header("1. Assignments")]
    public Transform robotPlayer;
    public Transform startPoint;
    public Transform endPoint;
    public ShipPart targetPart;

    [Header("2. Pivot Fix")]
    public Transform measurePoint;

    [Header("3. Settings")]
    public float moveSpeed = 2.0f;
    public float magnetForce = 100.0f;
    public float transitionSpeed = 1.0f;
    public float finishDistance = 1.7f;

    [Header("4. Visuals")]
    public GameObject uiPressE;
    public GameObject uiHoldFire;
    public GameObject cutZoneCube;
    public ParticleSystem sparkEffects;
    public TrailRenderer laserScar;
    public GameObject nextStation;

    [Header("5. Audio")]
    public AudioSource cuttingAudio;

    // Internal State
    private bool _inZone = false;
    private bool _isReady = false;
    private Rigidbody _rb;
    private RobotMovement _friendScript;
    private Transform _distCheckObj;

    void Start()
    {
        if (uiPressE) uiPressE.SetActive(false);
        if (uiHoldFire) uiHoldFire.SetActive(false);
        if (sparkEffects) sparkEffects.Stop();
        if (laserScar) laserScar.emitting = false;
        if (cuttingAudio) cuttingAudio.Stop();

        if (cutZoneCube) cutZoneCube.SetActive(true);

        if (robotPlayer)
        {
            _rb = robotPlayer.GetComponent<Rigidbody>();
            _friendScript = robotPlayer.GetComponent<RobotMovement>();
        }

        _distCheckObj = measurePoint ? measurePoint : robotPlayer;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == robotPlayer)
        {
            _inZone = true;
            if (!_isReady && uiPressE)
                uiPressE.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == robotPlayer)
        {
            _inZone = false;
            if (uiPressE) uiPressE.SetActive(false);
            if (uiHoldFire) uiHoldFire.SetActive(false);

            if (sparkEffects) sparkEffects.Stop();
            if (laserScar) laserScar.emitting = false;
            if (cuttingAudio) cuttingAudio.Stop();
        }
    }

    void Update()
    {
        if (_inZone && !_isReady && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(SetupSequence());
        }

        if (_isReady && !Mouse.current.leftButton.isPressed)
        {
            if (_rb) _rb.linearVelocity = Vector3.zero;

            if (uiHoldFire) uiHoldFire.SetActive(true);
            if (sparkEffects) sparkEffects.Stop();
            if (laserScar) laserScar.emitting = false;

            if (cuttingAudio && cuttingAudio.isPlaying)
                cuttingAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        if (_isReady && Mouse.current.leftButton.isPressed)
        {
            if (uiHoldFire) uiHoldFire.SetActive(false);

            if (sparkEffects && !sparkEffects.isPlaying)
                sparkEffects.Play();

            if (laserScar) laserScar.emitting = true;

            if (cuttingAudio && !cuttingAudio.isPlaying)
                cuttingAudio.Play();

            _rb.AddForce(-robotPlayer.up * magnetForce, ForceMode.Acceleration);

            Vector3 direction = (endPoint.position - robotPlayer.position).normalized;
            _rb.linearVelocity = direction * moveSpeed;

            float current = Vector3.Distance(_distCheckObj.position, endPoint.position);
            if (current < finishDistance)
            {
                FinishCut();
            }
        }
    }

    void FinishCut()
    {
        _isReady = false;

        if (uiHoldFire) uiHoldFire.SetActive(false);
        if (sparkEffects) sparkEffects.Stop();

        if (laserScar)
        {
            laserScar.emitting = false;
            laserScar.Clear();
            laserScar.enabled = false;
        }

        if (cuttingAudio) cuttingAudio.Stop();
        if (cutZoneCube) cutZoneCube.SetActive(false);

        if (targetPart) targetPart.Detach();
        if (nextStation) nextStation.SetActive(true);

        if (_rb) _rb.constraints = RigidbodyConstraints.None;
        if (_friendScript)
        {
            _friendScript.enabled = true;
            _friendScript.SetInputEnabled(true);
        }

        gameObject.SetActive(false);
    }

    IEnumerator SetupSequence()
    {
        if (uiPressE) uiPressE.SetActive(false);
        if (cutZoneCube) cutZoneCube.SetActive(false);

        if (_friendScript)
        {
            _friendScript.SetInputEnabled(false);
            _friendScript.enabled = false;
        }

        if (_rb) _rb.isKinematic = true;

        float t = 0f;
        Vector3 startPos = robotPlayer.position;
        Quaternion startRot = robotPlayer.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            robotPlayer.position = Vector3.Lerp(startPos, startPoint.position, t);
            robotPlayer.rotation = Quaternion.Slerp(startRot, startPoint.rotation, t);
            yield return null;
        }

        robotPlayer.position = startPoint.position;
        robotPlayer.rotation = startPoint.rotation;

        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        _isReady = true;
        if (uiHoldFire) uiHoldFire.SetActive(true);
    }
}
