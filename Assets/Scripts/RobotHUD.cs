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

    [Header("Settings")]
    public float maxMoveSpeedOverride = 0f;
    public bool showDebugLogs = false;

    [Header("Smoothing")]
    [Range(0.05f, 0.5f)]
    public float smoothSpeed = 0.2f; // Lower = smoother but slower response
    [Range(0.1f, 1.0f)]
    public float currentSmoothSpeed = 0.3f; // Faster smoothing for current

    private float missionStartTime;
    private Rigidbody robotRb;
    private Quaternion initialRotation;

    // Smoothed values
    private float smoothedRpm;
    private float smoothedTemp;
    private float smoothedVoltage;
    private float smoothedCurrent;

    void Start()
    {
        missionStartTime = Time.time;

        if (robot == null)
        {
            Debug.LogError("RobotHUD: Robot reference is missing!");
            return;
        }

        robotRb = robot.GetComponent<Rigidbody>();
        if (robotRb == null)
        {
            Debug.LogError("RobotHUD: Robot doesn't have a Rigidbody component!");
            return;
        }

        // Store initial rotation
        initialRotation = robot.transform.rotation;

        // Initialize smoothed values
        smoothedRpm = 0f;
        smoothedTemp = 20f;
        smoothedVoltage = 24f;
        smoothedCurrent = 3.5f; // Start at idle current, not 0

        // Setup back camera render texture
        if (backCamera != null && backCameraDisplay != null)
        {
            RenderTexture rt = new RenderTexture(432, 243, 16);
            backCamera.targetTexture = rt;
            backCameraDisplay.texture = rt;
        }

        UpdateSensorStatus();
        UpdateToolDisplay();
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
        const float maxRPM = 250f;
        const float maxTemp = 65f;
        const float baseTemp = 20f;
        const float baseVoltage = 24.0f;
        const float minVoltage = 20.0f;
        const float idleCurrent = 3.5f; // Systems always draw some current
        const float maxLoadCurrent = 28f; // Additional current under load

        // Get velocity (compatible with both old and new Unity)
        Vector3 vel = Vector3.zero;
#if UNITY_2023_1_OR_NEWER
        vel = robotRb.linearVelocity;
#else
        vel = robotRb.velocity;
#endif

        Vector3 horizontalVelocity = new Vector3(vel.x, 0, vel.z);
        float speed = horizontalVelocity.magnitude;

        // Determine max speed
        float maxSpeed = maxMoveSpeedOverride > 0 ? maxMoveSpeedOverride : robot.MaxMoveSpeed;
        if (maxSpeed <= 0.01f) maxSpeed = 5f;

        float speedFactor = Mathf.Clamp01(speed / maxSpeed);

        // Calculate target values
        float targetRpm = speedFactor * maxRPM;
        float targetTemp = baseTemp + (speedFactor * (maxTemp - baseTemp));
        float targetVoltage = baseVoltage - (speedFactor * (baseVoltage - minVoltage));

        // Current has baseline + load-based component + tool usage
        float loadCurrent = speedFactor * maxLoadCurrent;
        float toolCurrent = 0f;

        // Add current for active tools
        try
        {
            if (robot.cleaningHeadActive) toolCurrent += 2.5f;
            if (robot.plasmaTorchActive) toolCurrent += 5.0f;
        }
        catch { }

        float targetCurrent = idleCurrent + loadCurrent + toolCurrent;

        // Smooth the values using Lerp
        smoothedRpm = Mathf.Lerp(smoothedRpm, targetRpm, smoothSpeed);
        smoothedTemp = Mathf.Lerp(smoothedTemp, targetTemp, smoothSpeed);
        smoothedVoltage = Mathf.Lerp(smoothedVoltage, targetVoltage, smoothSpeed);
        smoothedCurrent = Mathf.Lerp(smoothedCurrent, targetCurrent, currentSmoothSpeed); // Faster smoothing for current

        // Add tiny random fluctuations only when moving
        float rpmVariance = 0f;
        float tempVariance = 0f;
        float voltVariance = 0f;
        float currVariance = 0f;

        if (speedFactor > 0.02f) // Lower threshold
        {
            rpmVariance = Random.Range(-1.5f, 1.5f);
            tempVariance = Random.Range(-0.2f, 0.2f);
            voltVariance = Random.Range(-0.05f, 0.05f);
            currVariance = Random.Range(-0.8f, 0.8f);
        }
        else
        {
            // Even when idle, add small fluctuations to current
            currVariance = Random.Range(-0.3f, 0.3f);
        }

        // Final values with variance
        float finalRpm = Mathf.Max(0f, smoothedRpm + rpmVariance);
        float finalTemp = Mathf.Max(baseTemp, smoothedTemp + tempVariance);
        float finalVoltage = Mathf.Clamp(smoothedVoltage + voltVariance, 0f, baseVoltage);
        float finalCurrent = Mathf.Max(idleCurrent * 0.5f, smoothedCurrent + currVariance); // Never below half idle current

        // Update display - round to reduce flicker
        if (rpmLeftText != null) rpmLeftText.text = Mathf.RoundToInt(finalRpm).ToString();
        if (rpmRightText != null) rpmRightText.text = Mathf.RoundToInt(finalRpm).ToString();

        if (tempLeftText != null) tempLeftText.text = finalTemp.ToString("F1");
        if (tempRightText != null) tempRightText.text = finalTemp.ToString("F1");

        if (voltLeftText != null) voltLeftText.text = finalVoltage.ToString("F1");
        if (voltRightText != null) voltRightText.text = finalVoltage.ToString("F1");

        // Display current with 1 decimal place so small values show
        if (currLeftText != null) currLeftText.text = finalCurrent.ToString("F1");
        if (currRightText != null) currRightText.text = finalCurrent.ToString("F1");

        // Debug logging
        if (showDebugLogs && Time.frameCount % 120 == 0)
        {
            Debug.Log($"=== MOTOR DATA DEBUG ===");
            Debug.Log($"Speed: {speed:F2} m/s | SpeedFactor: {speedFactor:F3} | MaxSpeed: {maxSpeed:F2}");
            Debug.Log($"Target Current: {targetCurrent:F1}A | Smoothed: {smoothedCurrent:F1}A | Final: {finalCurrent:F1}A");
            Debug.Log($"RPM: {finalRpm:F0} | Temp: {finalTemp:F1}°C | Volt: {finalVoltage:F1}V");
        }
    }

    void UpdateToolDisplay()
    {
        bool cleaning = false;
        bool cutting = false;

        try
        {
            cleaning = robot.cleaningHeadActive;
            cutting = robot.plasmaTorchActive;
        }
        catch (System.Exception e)
        {
            if (showDebugLogs && Time.frameCount % 120 == 0)
            {
                Debug.LogWarning($"Could not access tool states: {e.Message}");
            }
        }

        if (pressureGraphic != null)
        {
            pressureGraphic.gameObject.SetActive(true);
            pressureGraphic.enabled = cleaning;
        }

        if (torchGraphic != null)
        {
            torchGraphic.gameObject.SetActive(true);
            torchGraphic.enabled = cutting;
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
        bool showStatus1 = elapsed > 4f;

        if (sensorStatus1 != null) sensorStatus1.gameObject.SetActive(showStatus1);
        if (sensorStatus2 != null) sensorStatus2.gameObject.SetActive(!showStatus1);

        if (statusText != null) statusText.text = "NORMAL";
    }

    void UpdateOrientation()
    {
        // Calculate relative rotation from initial pose
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

        // Update text displays
        if (rollText != null) rollText.text = roll.ToString("F0") + "°";
        if (pitchText != null) pitchText.text = pitch.ToString("F0") + "°";
        if (yawText != null) yawText.text = yaw.ToString("F0") + "°";
    }

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