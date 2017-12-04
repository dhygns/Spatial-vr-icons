using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPointerEvent
{

    //Event Checker
    delegate void EventChecker();
    private EventChecker _eventChecker = null;


    // Use this for initialization
    public SPointerEvent()
    {
        _eventChecker = _eventCheckerIdle;
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        if (_eventChecker != null) _eventChecker();
    }

    //Event Lists Delegates
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"> Hands Position Type Of (Vector3) </param>
    /// <param name="dir"> Hands Direction Type Of (Vector3) </param>
    public delegate void Callback();

    /// <summary>
    /// Event Checker & Functions
    /// </summary>

    //Event Idle Checker & Logic
    private Callback _eventListIdle = null;
    private void _eventCheckerIdle()
    {
        if (_eventListIdle != null)
        {
            _eventListIdle();
        }

        float force = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger | OVRInput.Axis1D.SecondaryHandTrigger);
        if (force > 0.6f)
        {
            _eventChecker = _eventCheckerPressed;
        }

    }

    //Event Pressed Checker & Logic
    private Callback _eventListPressed = null;
    private void _eventCheckerPressed()
    {
        if (_eventListPressed != null)
        {
            _eventListPressed();
        }
        _eventChecker = _eventCheckerDrag;
    }

    //Event Drag Checker & Logic
    private Callback _eventListDrag = null;
    private void _eventCheckerDrag()
    {
        if (_eventListDrag != null)
        {
            _eventListDrag();
        }

        float force = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger | OVRInput.Axis1D.SecondaryHandTrigger);
        if (force < 0.4f)
        {
            _eventChecker = _eventCheckerRelease;
        }
    }

    //Event Release Checker & Logic
    private Callback _eventListRelease = null;
    private void _eventCheckerRelease()
    {
        if (_eventListRelease != null)
        {
            _eventListRelease();
        }
        _eventChecker = _eventCheckerIdle;
    }

    //Interface for Event Lists
    public void AddListener(string name, Callback callback)
    {
        switch (name)
        {
            case "idle":
                {
                    _eventListIdle += callback;
                }
                break;
            case "press":
                {
                    _eventListPressed += callback;
                }
                break;
            case "drag":
                {
                    _eventListDrag += callback;
                }
                break;
            case "release":
                {
                    _eventListRelease += callback;
                }
                break;
            default:
                {

                }
                break;
        }
    }

    public void RemoveListener(string name, Callback callback)
    {
        switch (name)
        {
            case "idle":
                {
                    if (callback == null) _eventListIdle = null;
                    else _eventListIdle -= callback;
                }
                break;
            case "press":
                {
                    if (callback == null) _eventListIdle = null;
                    else _eventListPressed -= callback;
                }
                break;
            case "drag":
                {
                    if (callback == null) _eventListIdle = null;
                    else _eventListDrag -= callback;
                }
                break;
            case "release":
                {
                    if (callback == null) _eventListIdle = null;
                    else _eventListRelease -= callback;
                }
                break;
            default:
                {

                }
                break;
        }
    }
}
