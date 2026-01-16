using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotHUD : MonoBehaviour
{
    [Header("Robot Reference")]
    public RobotMovement robot;

    [Header("Motor Data Texts (L/R)")]
    public TextMeshProUGUI rpmLeftText;
    public TextMeshProUGUI rpmRightText;
    public TextMeshProUGUI tempLeftText;
    public TextMeshProUGUI tempRightText;
    public TextMeshProUGUI voltLeftText;
    public TextMeshProUGUI voltRightText;
    public TextMeshProUGUI currLeftText;
    public TextMeshProUGUI currRightText;

    [Header("Tool Display")]
    public TextMeshProUGUI barValueText;
    public Image pressureGraphic;
    public Image torchGraphic;

    [Header("Status")]
    public TextMeshProUGUI statusText;
    public Image sensorStatus1;
    public Image sensorStatus2;

    [Header("Orientation Graphics")]
    public RectTransform rollGraphic;
    public RectTransform yawGraphic;
    public RectTransform pitchGraphic;
    public RectTransform yawBubble;

    [Header("Orientation Texts")]
    public TextMeshProUGUI rollText;
    public TextMeshProUGUI yawText;
    public TextMeshProUGUI pitchText;

    [Header("Secondary Camera")]
    public Camera backCamera;
    public RawImage backCameraDisplay;

    // Internal state
    private float missionStartTime;
    private Rigidbody robotRb;

    Quaternion initialRotation; // store in Start()

    void Start()
    {
        missionStartTime = Time.time;

        if (robot != null)
        {
            robotRb = robot.GetComponent<Rigidbody>();
        }

        // Setup back camera render texture
        if (backCamera != null && backCameraDisplay != null)
        {
            RenderTexture rt = new RenderTexture(432, 243, 16);
            backCamera.targetTexture = rt;
            backCameraDisplay.texture = rt;
        }

        // Initial status setup
        UpdateSensorStatus();
        UpdateToolDisplay();
        initialRotation = robot.transform.rotation; // store starting rotation
    }

    void Update()
    {
        if (robot == null || robotRb == null) return;

        UpdateMotorData();
        UpdateToolDisplay();
        UpdateSensorStatus();
        UpdateOrientation();
    }

    void UpdateMotorData()
    {
        if (robot == null || robotRb == null) return;

        const float maxRPM = 250f;
        const float maxTemp = 65f;
        const float nominalVoltage = 23.5f;
        const float maxCurrent = 30f;

        Vector3 horizontalVelocity = new Vector3(robotRb.linearVelocity.x, 0, robotRb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed < 0.01f) speed = 0f;

        float speedFactor = Mathf.Clamp01(speed / robot.MaxMoveSpeed);

        // RPM
        float rpm = speedFactor * maxRPM;
        if (speed > 0.01f) rpm += Random.Range(-3f, 3f);
        rpm = Mathf.Max(0f, rpm);
        rpmLeftText.text = Mathf.RoundToInt(rpm).ToString();
        rpmRightText.text = Mathf.RoundToInt(rpm).ToString();

        // Temperature
        float temp = speedFactor * maxTemp;
        if (speed > 0.01f) temp += Random.Range(-0.5f, 0.5f);
        temp = Mathf.Max(0f, temp);
        tempLeftText.text = temp.ToString("F1");
        tempRightText.text = temp.ToString("F1");

        // Voltage
        float voltage = speedFactor * nominalVoltage;
        if (speed > 0.01f) voltage += Random.Range(-0.1f, 0.1f);
        voltage = Mathf.Max(0f, voltage);
        voltLeftText.text = voltage.ToString("F1");
        voltRightText.text = voltage.ToString("F1");

        // Current
        float current = speedFactor * maxCurrent;
        if (speed > 0.01f) current += Random.Range(-1f, 1f);
        current = Mathf.Max(0f, current);
        currLeftText.text = Mathf.RoundToInt(current).ToString();
        currRightText.text = Mathf.RoundToInt(current).ToString();
    }

    void UpdateToolDisplay()
    {
        if (robot == null) return;

        bool cleaning = robot.cleaningHeadActive;
        bool cutting = robot.plasmaTorchActive;

        if (pressureGraphic != null)
        {
            if (!pressureGraphic.gameObject.activeSelf)
                pressureGraphic.gameObject.SetActive(true);
            pressureGraphic.enabled = true;
        }

        if (torchGraphic != null)
        {
            if (!torchGraphic.gameObject.activeSelf)
                torchGraphic.gameObject.SetActive(true);
            torchGraphic.enabled = true;
        }

        if (barValueText != null)
        {
            if (cleaning)
            {
                float pressure = 120f + Mathf.PerlinNoise(Time.time * 2f, 0f) * 10f;
                barValueText.text = pressure.ToString("F1");
            }
            else if (cutting)
            {
                float torchPower = 85f + Mathf.PerlinNoise(Time.time * 3f, 5f) * 15f;
                barValueText.text = torchPower.ToString("F1");
            }
            else
            {
                barValueText.text = "0.0";
            }
        }
    }

    void UpdateSensorStatus()
    {
        float elapsed = Time.time - missionStartTime;
        bool showStatus1 = elapsed > 5f;

        if (sensorStatus1 != null) sensorStatus1.gameObject.SetActive(showStatus1);
        if (sensorStatus2 != null) sensorStatus2.gameObject.SetActive(!showStatus1);

        if (statusText != null) statusText.text = "normal";
    }

    void UpdateOrientation()
    {
        if (robot == null) return;

        // Relative rotation from initial pose
        Quaternion relativeRotation = Quaternion.Inverse(initialRotation) * robot.transform.rotation;

        // Extract Euler angles in degrees
        Vector3 euler = relativeRotation.eulerAngles;

        // Normalize to -180..180
        float yaw = NormalizeAngle(euler.y);
        float pitch = NormalizeAngle(euler.x);
        float roll = NormalizeAngle(euler.z);

        // Update rotation graphics
        if (rollGraphic != null) rollGraphic.localRotation = Quaternion.Euler(0, 0, -roll);
        if (pitchGraphic != null) pitchGraphic.localRotation = Quaternion.Euler(0, 0, -pitch);
        if (yawGraphic != null) yawGraphic.localRotation = Quaternion.Euler(0, 0, -yaw);
        if (yawBubble != null) yawBubble.localRotation = Quaternion.Euler(0, 0, -yaw);

        // Update text displays
        if (rollText != null) rollText.text = roll.ToString("F0") + "°";
        if (pitchText != null) pitchText.text = pitch.ToString("F0") + "°";
        if (yawText != null) yawText.text = yaw.ToString("F0") + "°";
    }

    // Helper to normalize angles to -180..180
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }


    void OnDestroy()
    {
        if (backCameraDisplay != null && backCameraDisplay.texture != null)
        {
            if (backCameraDisplay.texture is RenderTexture rt)
            {
                rt.Release();
                Destroy(rt);
            }
        }
    }
}
