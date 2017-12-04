using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SWindow : MonoBehaviour
{



    //SWindow Components
    protected BoxCollider _boxColider;
    protected SphereCollider _sphereColider;
    protected Rigidbody _rigidBody;

    //SWindow Logics
    protected delegate void UpdateLogic();
    protected UpdateLogic _updateLogic = null;

    //SWindow positions
    protected Vector3 _targetPosition;
    protected Vector3 _currentPosition;

    //SWindow lookers
    protected Vector3 _targetLooker;
    protected Vector3 _currentLooker;

    //List of SWindows
    protected List<SWindow> _listCollidedSWindows;

    void Awake()
    {
        //init objects
        _listCollidedSWindows = new List<SWindow>();

        //Position &  Looker Init
        _currentPosition = _targetPosition = transform.position;
        _currentLooker = _targetLooker = transform.position + transform.forward;

        //added components
        InitComponent<Rigidbody>((cmpt) =>
        {
            _rigidBody = cmpt;

            cmpt.constraints = RigidbodyConstraints.FreezeAll;
        });

        InitComponent<BoxCollider>((cmpt) =>
        {
            _boxColider = cmpt;

            cmpt.isTrigger = true;
            cmpt.enabled = true;
        });

        InitComponent<SphereCollider>((cmpt) =>
        {
            _sphereColider = cmpt;

            cmpt.isTrigger = true;
            cmpt.enabled = false;
        });
    }

    protected virtual void Update()
    {
        if(_updateLogic != null)
        {
            _updateLogic();
        }

        _listCollidedSWindows.ForEach((SWindow sw) =>
        {
            sw.TargetPosition = sw.TargetPosition + (sw.TargetPosition - TargetPosition).normalized * 0.25f;
        });
    }

    public void updatePosition()
    {
        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;
    }

    public void updateLooker()
    {
        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        transform.LookAt(_currentLooker, Vector3.up);
    }

    public virtual void OnClicked(Vector3 pos, Vector3 forward)
    {
        _sphereColider.enabled = true;
        _boxColider.enabled = false;

        _targetPosition = pos;
        _targetLooker = pos + forward;
        
        _updateLogic = null;
        _updateLogic += updatePosition;
        _updateLogic += updateLooker;
    }

    public virtual void OnDraged(Vector3 pos, Vector3 forward)
    {
        _targetPosition = pos;
        _targetLooker = pos + forward;
    }

    public virtual void OnReleased(Vector3 pos, Vector3 forward)
    {
        _boxColider.enabled = true;
        _sphereColider.enabled = false;

        _targetPosition = pos;
        _targetLooker = transform.position + transform.forward;
        
        _listCollidedSWindows.Clear();

        _updateLogic = null;
        _updateLogic += updatePosition;
        //_updateLogic += updateLooker;
    }

    //Listeners

    private void OnTriggerEnter(Collider other)
    {
        if (!_sphereColider.enabled)
        {
            return;
        }

        if (!other.gameObject)
        {
            return;
        }

        SWindow cmpt = other.gameObject.GetComponent<SWindow>();
        if (!cmpt)
        {
            return;
        }

        _listCollidedSWindows.Add(cmpt);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject)
        {
            return;
        }

        SWindow cmpt = other.gameObject.GetComponent<SWindow>();
        if (!cmpt)
        {
            return;
        }

        if (!_listCollidedSWindows.Remove(cmpt))
        {
            return;
        }
    }



    //Interfaces 

    /// <summary>
    /// initComponent functino
    /// </summary>
    /// <typeparam name="T">Component Class in MonoBehaviour.</typeparam>
    /// <param name="callback"> function For initializing.</param>
    private void InitComponent<T>(Action<T> callback)
        where T : Component
    {
        T cmpt = GetComponent<T>();
        if (cmpt == null)
        {

            cmpt = gameObject.AddComponent<T>();
        }

        callback(cmpt);
    }

    public Vector3 TargetPosition
    {
        get
        {
            return _currentPosition;
        }
        set
        {
            _targetPosition = value;
        }
    }

    public Vector3 TargetLooker
    {
        get
        {
            return _currentLooker;
        }

        set
        {
            _targetLooker = value;
        }
    }



}
