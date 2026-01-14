# Robot HUD Implementation Guide

Complete step-by-step guide to implement the Robot Control HUD in Unity.

---

## Prerequisites

- Unity project with the Robot already set up
- `RobotHUD.cs` script in your Scripts folder
- All HUD sprite assets ready

---

## Required Sprites

| Sprite Name | Size | Purpose |
|-------------|------|---------|
| data sprite.png | 397×360 px | Motor data panel background |
| pressure.png | - | Cleaning mode display |
| torch.png | - | Cutting mode display |
| sensorstatus1.png | - | Normal sensor status |
| sensorstatus2.png | - | Initial sensor status (first 5 sec) |
| rollgraphic.png | 146×146 px | Roll orientation indicator |
| yawgraphic.png | 300×300 px | Yaw orientation indicator |
| yawbubble.png | 90×90 px | Yaw bubble overlay |
| pitchgraphic.png | 146×146 px | Pitch orientation indicator |
| back camera frame.png | 432×243 px | Secondary camera frame |
| bottombar.png | 1920 px wide | Bottom bar background |

---

## Step 1: Import Sprites

1. In Unity Project window, create folder: `Assets/Sprites/HUD`
2. Drag all PNG files into this folder
3. Select each sprite, in Inspector set:
   - **Texture Type:** `Sprite (2D and UI)`
   - Click **Apply**

---

## Step 2: Create the Canvas

1. In Hierarchy window, right-click → **UI** → **Canvas**
2. Rename to `RobotHUDCanvas`
3. Select the Canvas, configure in Inspector:

**Canvas Component:**
- Render Mode: `Screen Space - Overlay`

**Canvas Scaler Component:**
- UI Scale Mode: `Scale With Screen Size`
- Reference Resolution: `1920 x 1080`
- Match: `0.5`

---

## Step 3: Add RobotHUD Script

1. Select `RobotHUDCanvas` in Hierarchy
2. In Inspector, click **Add Component**
3. Search for `RobotHUD` and add it
4. Leave all fields empty for now

---

## Step 4: Create Bottom Bar

1. Right-click `RobotHUDCanvas` → **UI** → **Image**
2. Rename to `BottomBar`

**Inspector Settings:**
- Source Image: `bottombar`
- Image Type: `Simple`
- Preserve Aspect: `Unchecked`

**RectTransform:**
- Click anchor preset (square icon in top-left of RectTransform)
- Hold **Alt** and click **bottom-stretch** option
- Height: `60`
- Pos Y: `0`

---

## Step 5: Create Motor Data Panel (Top-Left)

### 5.1 Create Panel Background

1. Right-click `RobotHUDCanvas` → **UI** → **Image**
2. Rename to `MotorDataPanel`

**Inspector Settings:**
- Source Image: `data sprite`
- Click **Set Native Size** button

**RectTransform:**
- Anchor: Top-Left (click preset, select top-left)
- Pivot: X=`0`, Y=`1`
- Pos X: `30`
- Pos Y: `-30`
- Width: `397`
- Height: `360`

### 5.2 Create Motor Data Text Elements

For each text element below, do the following:

1. Right-click `MotorDataPanel` → **UI** → **Text - TextMeshPro**
2. If prompted, click **Import TMP Essentials**
3. Configure each text:

**Common Settings for All:**
- Font Size: `20`
- Alignment: Center
- Color: White
- Width: `50`
- Height: `30`

**Text Elements to Create:**

| Name | Default Text | Pos X | Pos Y |
|------|--------------|-------|-------|
| RPM_L | 0 | 280 | -85 |
| RPM_R | 0 | 330 | -85 |
| TEMP_L | 40.0 | 280 | -115 |
| TEMP_R | 40.0 | 330 | -115 |
| VOLT_L | 23.0 | 280 | -145 |
| VOLT_R | 23.0 | 330 | -145 |
| CURR_L | 15 | 280 | -175 |
| CURR_R | 15 | 330 | -175 |

> **Note:** Adjust positions to match your data sprite layout

---

## Step 6: Create Tool Panel (Left Side)

### 6.1 Create Panel Container

1. Right-click `RobotHUDCanvas` → **Create Empty**
2. Rename to `ToolPanel`
3. Add **RectTransform** component if not present

**RectTransform:**
- Anchor: Middle-Left
- Pos X: `50`
- Pos Y: `50`
- Width: `200`
- Height: `150`

### 6.2 Create BAR Label

1. Right-click `ToolPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `BAR_Label`
- Text: `BAR`
- Font Size: `24`
- Pos X: `-60`
- Pos Y: `40`

### 6.3 Create BAR Value

1. Right-click `ToolPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `BAR_Value`
- Text: `0.0`
- Font Size: `24`
- Pos X: `40`
- Pos Y: `40`

### 6.4 Create Pressure Graphic

1. Right-click `ToolPanel` → **UI** → **Image**
2. Rename to `PressureGraphic`
- Source Image: `pressure`
- Click **Set Native Size**
- Pos X: `0`
- Pos Y: `-30`

### 6.5 Create Torch Graphic

1. Right-click `ToolPanel` → **UI** → **Image**
2. Rename to `TorchGraphic`
- Source Image: `torch`
- Click **Set Native Size**
- Same position as PressureGraphic
- **Uncheck the checkbox** at top of Inspector to disable

---

## Step 7: Create Status Panel (Bottom-Left)

### 7.1 Create Panel Container

1. Right-click `RobotHUDCanvas` → **Create Empty**
2. Rename to `StatusPanel`

**RectTransform:**
- Anchor: Bottom-Left
- Pos X: `50`
- Pos Y: `100`
- Width: `250`
- Height: `120`

### 7.2 Create STATUS Label

1. Right-click `StatusPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `STATUS_Label`
- Text: `STATUS`
- Font Size: `20`
- Alignment: Left
- Pos X: `-80`
- Pos Y: `40`

### 7.3 Create STATUS Value

1. Right-click `StatusPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `STATUS_Value`
- Text: `normal`
- Font Size: `20`
- Color: Green (`#00FF00`)
- Pos X: `40`
- Pos Y: `40`

### 7.4 Create Sensor Status Graphics

**SensorStatus1:**
1. Right-click `StatusPanel` → **UI** → **Image**
2. Rename to `SensorStatus1`
- Source Image: `sensorstatus1`
- Click **Set Native Size**
- Pos X: `0`
- Pos Y: `-20`

**SensorStatus2:**
1. Right-click `StatusPanel` → **UI** → **Image**
2. Rename to `SensorStatus2`
- Source Image: `sensorstatus2`
- Click **Set Native Size**
- Same position as SensorStatus1

---

## Step 8: Create Orientation Panel (Bottom-Center)

### 8.1 Create Panel Container

1. Right-click `RobotHUDCanvas` → **Create Empty**
2. Rename to `OrientationPanel`

**RectTransform:**
- Anchor: Bottom-Center
- Pos X: `0`
- Pos Y: `150`
- Width: `700`
- Height: `350`

### 8.2 Create Roll Indicator (Left)

**RollGraphic:**
1. Right-click `OrientationPanel` → **UI** → **Image**
2. Rename to `RollGraphic`
- Source Image: `rollgraphic`
- Width: `146`
- Height: `146`
- Pos X: `-220`
- Pos Y: `80`

**Roll_Text:**
1. Right-click `OrientationPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `Roll_Text`
- Text: `0°`
- Font Size: `18`
- Pos X: `-220`
- Pos Y: `-20`

### 8.3 Create Yaw Indicator (Center)

**YawGraphic:**
1. Right-click `OrientationPanel` → **UI** → **Image**
2. Rename to `YawGraphic`
- Source Image: `yawgraphic`
- Width: `300`
- Height: `300`
- Pos X: `0`
- Pos Y: `50`

**YawBubble:**
1. Right-click `OrientationPanel` → **UI** → **Image**
2. Rename to `YawBubble`
- Source Image: `yawbubble`
- Width: `90`
- Height: `90`
- Pos X: `0`
- Pos Y: `180`

**Yaw_Text:**
1. Right-click `OrientationPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `Yaw_Text`
- Text: `0°`
- Font Size: `18`
- Pos X: `0`
- Pos Y: `-120`

### 8.4 Create Pitch Indicator (Right)

**PitchGraphic:**
1. Right-click `OrientationPanel` → **UI** → **Image**
2. Rename to `PitchGraphic`
- Source Image: `pitchgraphic`
- Width: `146`
- Height: `146`
- Pos X: `220`
- Pos Y: `80`

**Pitch_Text:**
1. Right-click `OrientationPanel` → **UI** → **Text - TextMeshPro**
2. Rename to `Pitch_Text`
- Text: `0°`
- Font Size: `18`
- Pos X: `220`
- Pos Y: `-20`

---

## Step 9: Create Back Camera Display (Top-Right)

### 9.1 Create Camera Frame

1. Right-click `RobotHUDCanvas` → **UI** → **Image**
2. Rename to `BackCameraFrame`

**Inspector Settings:**
- Source Image: `back camera frame`

**RectTransform:**
- Anchor: Top-Right
- Pivot: X=`1`, Y=`1`
- Pos X: `-30`
- Pos Y: `-30`
- Width: `432`
- Height: `243`

### 9.2 Create Camera Display

1. Right-click `BackCameraFrame` → **UI** → **Raw Image**
2. Rename to `BackCameraDisplay`

**RectTransform:**
- Anchor: Stretch (click preset, hold Alt, click center stretch option)
- Left: `10`
- Right: `10`
- Top: `10`
- Bottom: `10`

---

## Step 10: Create Back Camera

1. In Hierarchy, find your Robot GameObject
2. Right-click Robot → **Camera**
3. Rename to `BackCamera`

**Position the Camera:**
- Move it to the back of your robot
- Rotate it to face backward (Y rotation: `180`)

**Camera Settings:**
- Clear Flags: `Solid Color` or `Skybox`
- **Uncheck** Audio Listener component

---

## Step 11: Create Camera Toggle Icon (Bottom-Right)

1. Right-click `RobotHUDCanvas` → **UI** → **Image**
2. Rename to `CameraIcon`

**RectTransform:**
- Anchor: Bottom-Right
- Pivot: X=`1`, Y=`0`
- Pos X: `-50`
- Pos Y: `80`
- Width: `60`
- Height: `60`

> Add a camera icon sprite if you have one

---

## Step 12: Wire Up the RobotHUD Component

1. Select `RobotHUDCanvas` in Hierarchy
2. Find the **RobotHUD** component in Inspector
3. Drag and drop the following:

### Robot Reference
| Field | Drag This |
|-------|-----------|
| Robot | Your Robot GameObject (with RobotMovement script) |

### Motor Data Texts
| Field | Drag This |
|-------|-----------|
| Rpm Left Text | RPM_L |
| Rpm Right Text | RPM_R |
| Temp Left Text | TEMP_L |
| Temp Right Text | TEMP_R |
| Volt Left Text | VOLT_L |
| Volt Right Text | VOLT_R |
| Curr Left Text | CURR_L |
| Curr Right Text | CURR_R |

### Tool Display
| Field | Drag This |
|-------|-----------|
| Bar Value Text | BAR_Value |
| Pressure Graphic | PressureGraphic |
| Torch Graphic | TorchGraphic |

### Status
| Field | Drag This |
|-------|-----------|
| Status Text | STATUS_Value |
| Sensor Status 1 | SensorStatus1 |
| Sensor Status 2 | SensorStatus2 |

### Orientation Graphics
| Field | Drag This |
|-------|-----------|
| Roll Graphic | RollGraphic |
| Yaw Graphic | YawGraphic |
| Pitch Graphic | PitchGraphic |
| Yaw Bubble | YawBubble |

### Orientation Texts
| Field | Drag This |
|-------|-----------|
| Roll Text | Roll_Text |
| Yaw Text | Yaw_Text |
| Pitch Text | Pitch_Text |

### Secondary Camera
| Field | Drag This |
|-------|-----------|
| Back Camera | BackCamera |
| Back Camera Display | BackCameraDisplay |

---

## Step 13: Test the HUD

1. Press **Play** in Unity

### Expected Behavior:

| Feature | Expected Result |
|---------|-----------------|
| RPM | Shows `0` when stationary, increases with movement |
| Temperature | Fluctuates slightly around 40°C |
| Voltage | Stays between 23.0-23.9 |
| Current | Increases with speed |
| Sensor Status | Shows status2 for 5 seconds, then switches to status1 |
| Roll/Yaw/Pitch | Rotate based on robot orientation |
| Degree Values | Show real robot rotation angles |
| Back Camera | Shows rear view of robot |
| Press B | Toggles cleaning mode, shows pressure graphic |
| Press Y | Toggles cutting mode, shows torch graphic |

---

## Troubleshooting

### HUD not showing
- Check Canvas Render Mode is `Screen Space - Overlay`
- Ensure Canvas is not disabled

### Text not visible
- Check TextMeshPro is imported (Window → TextMeshPro → Import TMP Essential Resources)
- Verify text color is not transparent

### Back camera not working
- Ensure BackCamera is assigned in RobotHUD
- Check camera is enabled and positioned correctly

### Orientation not updating
- Verify Robot reference is assigned
- Check robot has Rigidbody component

### Graphics not rotating
- Ensure RectTransform references are assigned (not Image references)

---

## Final Hierarchy Structure

```
RobotHUDCanvas
├── BottomBar
├── MotorDataPanel
│   ├── RPM_L
│   ├── RPM_R
│   ├── TEMP_L
│   ├── TEMP_R
│   ├── VOLT_L
│   ├── VOLT_R
│   ├── CURR_L
│   └── CURR_R
├── ToolPanel
│   ├── BAR_Label
│   ├── BAR_Value
│   ├── PressureGraphic
│   └── TorchGraphic
├── StatusPanel
│   ├── STATUS_Label
│   ├── STATUS_Value
│   ├── SensorStatus1
│   └── SensorStatus2
├── OrientationPanel
│   ├── RollGraphic
│   ├── Roll_Text
│   ├── YawGraphic
│   ├── YawBubble
│   ├── Yaw_Text
│   ├── PitchGraphic
│   └── Pitch_Text
├── BackCameraFrame
│   └── BackCameraDisplay
└── CameraIcon
```

---

## Controls Reference

| Key/Button | Action |
|------------|--------|
| W/S | Move forward/backward |
| A/D | Rotate left/right |
| B | Toggle cleaning mode |
| Y | Toggle cutting mode |
| C | Toggle camera view |
| X | Pause game |

---

*Guide created for Robot Control HUD - 1920×1080 Canvas*
