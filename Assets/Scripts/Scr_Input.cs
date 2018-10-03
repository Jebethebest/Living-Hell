using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Scr_Input : MonoBehaviour {

    PlayerIndex m_GamepadIndex;
    GamePadState m_GamepadState;
    private bool m_IsSet = false;

    private string m_HorizontalMove = "Horizontal_Left_";
    private string m_VerticalMove = "Vertical_Left_";
    private string m_JumpButton = "Jump_";
    private string m_GrabButton = "Grab_";
    private string m_FireButton = "Fire_";
    private string m_UseButton = "Use_";

    //Vibration
    private enum Type
    {
        Pulse, Increase, Decrease
    }

    private bool m_IsVibrating = false;
    private float m_Duration = 0.0f;
    private float m_Amount = 0.0f;
    private float m_Min = 0.0f;
    private float m_Max = 0.0f;
    private float m_VibrationTimer = 0.0f;
    private Type m_Type = Type.Pulse;

    private void FixedUpdate()
    {
        if (m_IsVibrating)
        {
            switch (m_Type)
            {
                case Type.Pulse:
                    UpdatePulseVibration();
                    break;

                case Type.Increase:
                    UpdateIncreaseVibration();
                    break;

                case Type.Decrease:
                    UpdateDecreaseVibration();
                    break;
            }
        }
    }

    public void SetGamepadIndex(PlayerIndex index)
    {
        m_GamepadIndex = index;

        Debug.Log("Registered controller with ID: " + index.ToString());
    }

    public void SetInput(string player)
    {
        m_HorizontalMove += player;
        m_VerticalMove += player;
        m_JumpButton += player;
        m_GrabButton += player;
        m_FireButton += player;
        m_UseButton += player;

        m_IsSet = true;

        Debug.Log(player + " has been set");
    }

    public void Vibrate(float amount, float duration)
    {
        m_Amount = amount;
        m_Duration = duration;
        m_IsVibrating = true;

        m_Type = Type.Pulse;
    }

    public void VibrateIncrease(float min, float max, float duration)
    {
        m_Min = min;
        m_Max = max;
        m_Duration = duration;
        m_Amount = min;
        m_VibrationTimer = 0.0f;
        m_IsVibrating = true;

        m_Type = Type.Increase;
    }

    public void VibrateDecrease(float min, float max, float duration)
    {
        m_Min = min;
        m_Max = max;
        m_Duration = duration;
        m_Amount = max;
        m_VibrationTimer = 0.0f;
        m_IsVibrating = true;

        m_Type = Type.Decrease;
    }

    public string GetHorizontalMove() { return m_HorizontalMove; }
    public string GetVerticalMove() { return m_VerticalMove; }
    public string GetJump() { return m_JumpButton; }
    public string GetGrab() { return m_GrabButton; }
    public string GetFire() { return m_FireButton; }
    public string GetUse() { return m_UseButton; }
    public string GetThrow() { return m_GrabButton; }
    public bool IsSet() { return m_IsSet; }

    private void UpdatePulseVibration()
    {
        GamePad.SetVibration(m_GamepadIndex, m_Amount, m_Amount);

        m_Duration -= Time.deltaTime;
        if (m_Duration <= 0.0f)
        {
            GamePad.SetVibration(m_GamepadIndex, 0.0f, 0.0f);
            m_IsVibrating = false;
        }
    }

    private void UpdateIncreaseVibration()
    {
        m_VibrationTimer += Time.deltaTime;

        float lerp = m_VibrationTimer / m_Duration;
        m_Amount = Mathf.Lerp(m_Min, m_Max, lerp);
        GamePad.SetVibration(m_GamepadIndex, m_Amount, m_Amount);

        if (m_VibrationTimer >= m_Duration)
        {
            GamePad.SetVibration(m_GamepadIndex, 0.0f, 0.0f);
            m_IsVibrating = false;
        }
    }

    private void UpdateDecreaseVibration()
    {
        m_VibrationTimer += Time.deltaTime;

        float lerp = 1.0f - (m_VibrationTimer / m_Duration);
        m_Amount = Mathf.Lerp(m_Min, m_Max, lerp);
        GamePad.SetVibration(m_GamepadIndex, m_Amount, m_Amount);

        if (m_VibrationTimer >= m_Duration)
        {
            GamePad.SetVibration(m_GamepadIndex, 0.0f, 0.0f);
            m_IsVibrating = false;
        }
    }
}
