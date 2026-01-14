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
    private float baseVoltage = 23f;
    private float baseTemp = 40f;
    private Rigidbody robotRb;
    
    // Noise timers
    private float tempNoiseTimer;
    private float voltNoiseTimer;
    
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
        // Get speed factor (0-1 based on current velocity relative to max speed)
        float velocityMagnitude = robotRb.linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(velocityMagnitude / robot.speed);
        
        // RPM: 0 when stationary, higher with speed (max ~300)
        float baseRpm = speedFactor * 300f;
        float rpmL = baseRpm + Random.Range(-5f, 5f);
        float rpmR = baseRpm + Random.Range(-5f, 5f);
        
        if (rpmLeftText != null) rpmLeftText.text = Mathf.RoundToInt(Mathf.Max(0, rpmL)).ToString();
        if (rpmRightText != null) rpmRightText.text = Mathf.RoundToInt(Mathf.Max(0, rpmR)).ToString();
        
        // Temperature: fluctuates slightly during mission
        tempNoiseTimer += Time.deltaTime;
        float tempNoise = Mathf.PerlinNoise(tempNoiseTimer * 0.5f, 0f) * 2f - 1f;
        float tempL = baseTemp + tempNoise + Random.Range(-0.2f, 0.2f);
        float tempR = baseTemp + tempNoise + Random.Range(-0.2f, 0.2f);
        
        if (tempLeftText != null) tempLeftText.text = tempL.ToString("F1");
        if (tempRightText != null) tempRightText.text = tempR.ToString("F1");
        
        // Voltage: constant-ish, only decimal fluctuates (23.0-23.9)
        voltNoiseTimer += Time.deltaTime;
        float voltDecimalL = Mathf.PerlinNoise(voltNoiseTimer * 0.3f, 10f) * 0.9f;
        float voltDecimalR = Mathf.PerlinNoise(voltNoiseTimer * 0.3f, 20f) * 0.9f;
        
        if (voltLeftText != null) voltLeftText.text = (baseVoltage + voltDecimalL).ToString("F1");
        if (voltRightText != null) voltRightText.text = (baseVoltage + voltDecimalR).ToString("F1");
        
        // Current: higher with higher input speed
        float baseCurrent = 15f + speedFactor * 10f;
        float currL = baseCurrent + Random.Range(-0.5f, 0.5f);
        float currR = baseCurrent + Random.Range(-0.5f, 0.5f);
        
        if (currLeftText != null) currLeftText.text = Mathf.RoundToInt(currL).ToString();
        if (currRightText != null) currRightText.text = Mathf.RoundToInt(currR).ToString();
    }
    
    void UpdateToolDisplay()
    {
        bool cleaning = robot.cleaningHeadActive;
        bool cutting = robot.plasmaTorchActive;
        
        if (pressureGraphic != null) pressureGraphic.gameObject.SetActive(cleaning);
        if (torchGraphic != null) torchGraphic.gameObject.SetActive(cutting);
        
        // BAR value - fake number that fluctuates when tool is active
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
        
        // First 5 seconds: show status2, then status1
        bool showStatus1 = elapsed > 5f;
        
        if (sensorStatus1 != null) sensorStatus1.gameObject.SetActive(showStatus1);
        if (sensorStatus2 != null) sensorStatus2.gameObject.SetActive(!showStatus1);
        
        if (statusText != null) statusText.text = "normal";
    }
    
    void UpdateOrientation()
    {
        // Get robot's euler angles
        Vector3 euler = robot.transform.eulerAngles;
        
        // Normalize angles to -180 to 180 range
        float roll = NormalizeAngle(euler.z);
        float yaw = NormalizeAngle(euler.y);
        float pitch = NormalizeAngle(euler.x);
        
        // Update rotation graphics
        if (rollGraphic != null)
        {
            rollGraphic.localRotation = Quaternion.Euler(0, 0, -roll);
        }
        
        if (yawGraphic != null)
        {
            yawGraphic.localRotation = Quaternion.Euler(0, 0, -yaw);
        }
        
        if (yawBubble != null)
        {
            yawBubble.localRotation = Quaternion.Euler(0, 0, -yaw);
        }
        
        if (pitchGraphic != null)
        {
            pitchGraphic.localRotation = Quaternion.Euler(0, 0, -pitch);
        }
        
        // Update text displays
        if (rollText != null) rollText.text = roll.ToString("F0") + "°";
        if (yawText != null) yawText.text = yaw.ToString("F0") + "°";
        if (pitchText != null) pitchText.text = pitch.ToString("F0") + "°";
    }
    
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
    
    void OnDestroy()
    {
        // Cleanup render texture
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
