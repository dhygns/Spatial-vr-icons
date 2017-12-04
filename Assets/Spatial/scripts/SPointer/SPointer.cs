using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SEvent = SPointerEvent;
using SLogic = SPointerLogic;

public class SPointer : MonoBehaviour
{
    public GameObject RightHand;
    public GameObject LeftHand;

    //SingleTon
    static private SPointer _instance = null;

    // Dictionary for managing updators
    private Dictionary<string, SEvent.Callback> _updatordic;

    private void Awake()
    {
        //init SingleTon
        if (_instance == null) _instance = this;

        //Setup SpointerEvent
        _event = new SEvent();
        _logic = new SLogic();

        //Setup Dics for managing SLogic's updators
        _updatordic = new Dictionary<string, SEvent.Callback>();
        AddEventListener("idle", _updatordic["idle"] = _logic.UpdateIdle);
        AddEventListener("press", _updatordic["press"] = _logic.UpdatePress);
        AddEventListener("drag", _updatordic["drag"] = _logic.UpdateDrag);
        AddEventListener("release", _updatordic["release"] = _logic.UpdateRelease);
    }


    private SEvent _event = null;
    private SLogic _logic = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (OVRInput.GetActiveController())
        {
            case OVRInput.Controller.LTouch:
                {
                    _logic.SetPrimaryHand(LeftHand);
                }
                break;
            case OVRInput.Controller.RTouch:
                {
                    _logic.SetPrimaryHand(RightHand);
                }
                break;
            case OVRInput.Controller.Touch:
                {
                    _logic.SetPrimaryHand(RightHand);
                }
                break;
            case OVRInput.Controller.None:
                {
                    _logic.SetPrimaryHand(null);
                }
                break;
        }

        //Update Event & Logic
        _event.Update(Time.deltaTime);

        transform.position = _logic.Position;
    }


    //Interfaces

    /// <summary>
    /// You can use this function for adding EventListener
    /// </summary>
    /// <param name="eventName"> "idle" | "press" | "drag" | "release" </param>
    /// <param name="callback"> (Vector3 pos) => { ... } </param>
    static public void AddEventListener(string eventName, SEvent.Callback callback)
    {
        _instance._event.AddListener(eventName, callback);
    }

    /// <summary>
    /// You can use this function for removing EventListener
    /// if you want to clear all of EventListener, let it be null.
    /// </summary>
    /// <param name="eventName"> "idle" | "press" | "drag" | "release" </param>
    /// <param name="callback"> (Vector3 pos) => { ... } or Nothing </param>
    static public void RemoveEventListener(string eventName, SEvent.Callback callback = null)
    {
        _instance._event.RemoveListener(eventName, callback);
        if (callback == null) AddEventListener("eventName", _instance._updatordic["eventName"]);
    }
}
