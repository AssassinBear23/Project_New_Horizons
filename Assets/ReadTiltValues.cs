using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// 
/// </summary>
public class ReadTiltValues : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;

    // Input Actions
    private InputAction m_moveAction;

    // Control values
    private (float,float,float) m_tiltAmount; // Roll = Z-axis Phone, Pitch = X-Axis Phone, Yaw = Y-axis Phone
    private Vector3 m_accelerometerAmount;
    private (float,float,float) m_calibrationValue; // Roll = Z-axis Phone, Pitch = X-Axis Phone, Yaw = Y-axis Phone
    private bool canTextChange = true;

    // Filtering values
    [Header("Dampening")]
    [SerializeField] [Range(0f, 1f)] private float m_smoothingFactor = 0.2f;
    private Vector3 m_filteredAccel;

    // Internal references
    private TMP_Text m_text;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = inputActions.FindActionMap("Player").FindAction("Move");
        m_text = GetComponent<TMP_Text>();
        m_filteredAccel = Vector3.zero;
    }

    private void FixedUpdate()
    {
        m_accelerometerAmount = m_moveAction.ReadValue<Vector3>();
        // Exponential moving average filter
        m_filteredAccel = Vector3.Lerp(m_filteredAccel, m_accelerometerAmount, m_smoothingFactor);
        m_tiltAmount = GetTilt(m_filteredAccel);
        if(canTextChange)
            m_text.text = "Roll: " + m_tiltAmount.Item1.ToString("F2") + "°";
    }

    private (float,float,float) GetTilt(Vector3 accel)
    {
        Vector3 normAccel = accel.normalized;


        float pitch = Mathf.Atan2(normAccel.y, normAccel.z) * Mathf.Rad2Deg;

        // Roll
        float roll = Mathf.Atan2(-normAccel.x, Mathf.Sqrt(normAccel.y * normAccel.y + normAccel.z * normAccel.z)) * Mathf.Rad2Deg;

        // Applying the calibrated offset
        roll -= m_calibrationValue.Item1;

        roll *= -1f;

        return (roll, pitch, 0f);
    }

    private (float, float, float) GetTilt(Vector3 accel, bool useCalibratedValue)
    {
        Vector3 normAccel = accel.normalized;

        // Roll
        float pitch = Mathf.Atan2(normAccel.y, normAccel.z) * Mathf.Rad2Deg;

        // Pitch
        float roll = Mathf.Atan2(-normAccel.x, Mathf.Sqrt(normAccel.y * normAccel.y + normAccel.z * normAccel.z)) * Mathf.Rad2Deg;

        // Applying the calibrated offset if specified
        if (useCalibratedValue)
        {
            roll -= m_calibrationValue.Item1;
        }

        roll *= -1f;
        pitch *= -1f;

        return (roll, pitch, 0f);
    }

    public void CalibrateTilt(CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        m_calibrationValue = GetTilt(m_filteredAccel, false);

        m_text.text = "Calibration complete. Tilt values set to zero.";
        canTextChange = false;

        StartCoroutine(ChangeCameraColor());
    }

    private IEnumerator ChangeCameraColor()
    {
        
        Camera mainCamera = Camera.main;
        Color targetColor = Color.green;
        Color originalColor = mainCamera.backgroundColor;

        mainCamera.backgroundColor = targetColor;

        yield return new WaitForSeconds(1f);

        //Debug.Log("Resetting camera color to original.");
        Debug.Log("original color: " + originalColor);
        mainCamera.backgroundColor = originalColor;

        yield return new WaitForSeconds(2f);

        canTextChange = true;

        yield return null;
    }
}