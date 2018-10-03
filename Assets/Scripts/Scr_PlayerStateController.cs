using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerStateController : MonoBehaviour 
{
    public enum State
    {
        Safe, Unsafe, BabyBox
    }

    public State PlayerState = State.Safe;
    public bool IsMomInRoom = false;
    public bool IsHoldingObject = false;

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Mom_In_Room", () => IsMomInRoom = true);
        Scr_EventManager.StartListening("Mom_Leaves", () => IsMomInRoom = false);
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Mom_In_Room", () => IsMomInRoom = true);
        Scr_EventManager.StopListening("Mom_Leaves", () => IsMomInRoom = false);
    }
}
