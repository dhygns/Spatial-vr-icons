using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SWindow : SBase
{

    //SWindow States
    public enum LOGIC
    {
        /// <summary>
        /// This state is for idle mode
        /// </summary>
        Basic,

        /// <summary>
        /// This state is for grouping mode
        /// </summary>
        Group,

        /// <summary>
        /// NONE
        /// </summary>
        None
    }
    protected LOGIC _logicState;


    //interface for each mesh data
    public Vector3 ReleasedBoxColliderSize = new Vector3(0.33f, 0.33f, 0.1f);
    public Vector3 ReleasedBoxColliderCenter = new Vector3(0.0f, 0.0f, 0.0f);

    public Vector3 PressedBoxColliderSize = new Vector3(0.4f, 0.4f, 0.2f);
    public Vector3 PressedBoxColliderCenter = new Vector3(0.0f, 0.0f, 0.0f);


    //cmpts
    protected BoxCollider _collider;
    protected Rigidbody _rigidbody;

    // position moves positionGroupCurr to positionGroupDest 
    protected float _positionGroupMovingSpeed;
    protected Vector3 _positionGroupDest;
    protected Vector3 _positionGroupCurr;

    // direction spins directionGroupCurr to directionGroupDest
    protected float _directionGroupSpiningSpeed;
    protected Vector3 _directionGroupDest;
    protected Vector3 _directionGroupCurr;

    //near SBase List
    protected List<SWindow> _listNearSWindow;

    //groupping
    protected SWindow _groupingCandidate;
    protected GameObject _groupingPrefab;
    protected SGroup _group;
    protected float _groupingFocuseTime;
    protected Ray _groupingCheckRay;
    protected RaycastHit _groupingCheckHit;

    protected new void Awake()
    {
        base.Awake();

        //init Group Transfrom 
        _positionGroupMovingSpeed = 8.0f;
        _directionGroupSpiningSpeed = 8.0f;
        _positionGroupDest = Vector3.zero;
        _directionGroupDest = Vector3.zero;
        
        //init state
        _logicState = LOGIC.Basic;

        //load Resourecs
        _groupingPrefab = Resources.Load("Prefabs/Group") as GameObject;

        //init list
        _listNearSWindow = new List<SWindow>();

        //init collider box
        _collider = gameObject.GetComponent<BoxCollider>();
        _collider = _collider == null ? gameObject.AddComponent<BoxCollider>() : _collider;
        _collider.isTrigger = true;

        _collider.size = ReleasedBoxColliderSize;
        _collider.center = ReleasedBoxColliderCenter;

        //init rigidbody
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody = _rigidbody == null ? gameObject.AddComponent<Rigidbody>() : _rigidbody;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.useGravity = false;
        _rigidbody.drag = 10.0f;
    }

    public override void DefaultUpdate()
    {
        transform.localScale = CurrentScale;
        _listNearSWindow.ForEach((window) =>
        {
            Vector3 diff = window.transform.position - transform.position;
            if (diff.magnitude > 1.0f)
            {
                _listNearSWindow.Remove(window);
            }
        });
    }

    /// <summary>
    /// this function would be called when window is normal.
    /// </summary>
    /// <param name="state"> point state </param>
    protected override void _updateBasicLogic(POINT state)
    {
        switch (state)
        {
            case POINT.Pressed:
                UpdateBasicPressed();
                break;
            case POINT.Dragged:
                UpdateBasicDragged();
                break;
            case POINT.Released:
                UpdateBasicReleased();
                break;
            default:
                Debug.LogWarning(state);
                break;
        }
    }

    /// <summary>
    /// this function would be called when window is groupped.
    /// </summary>
    /// <param name="state"></param>
    protected override void _updateGroupLogic(POINT state)
    {
        switch (state)
        {
            case POINT.Pressed:
                UpdateGroupPressed();
                break;
            case POINT.Dragged:
                UpdateGroupDragged();
                break;
            case POINT.Released:
                UpdateGroupReleased();
                break;
            default:
                Debug.LogWarning(state);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SWindow target = other.GetComponent<SWindow>();
        if (target != null)
        {
            _listNearSWindow.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SWindow target = other.GetComponent<SWindow>();
        if (target != null)
        {
            _listNearSWindow.Remove(target);
        }
    }

    protected void UpdateGroupingCheck()
    {
        if (_logicState == LOGIC.Group)
        {
            return;
        }

        //Setup Ray
        _groupingCheckRay.origin = transform.position;
        _groupingCheckRay.direction = -transform.forward;

        //Check that Groupping is possible
        Physics.Raycast(_groupingCheckRay, out _groupingCheckHit, 1.0f);

        //Get Candidate
        SWindow candidate = _groupingCheckHit.transform != null ? _groupingCheckHit.transform.GetComponent<SWindow>() : null;

        if (candidate == null || _groupingCandidate == null)
        {
            _groupingFocuseTime = 0.0f;

            if (_groupingCandidate != null)
            {
                //this candidate remove from group
                _groupingCandidate = null;
            }

            if (candidate != null)
            {
                //regist candidate
                _groupingCandidate = candidate;
            }

            if (_group != null)
            {
                _group.Deativate();
                _group = null;
            }
        }
        else
        {

            if (candidate != _groupingCandidate)
            {
                _groupingFocuseTime = 0.0f;
                _groupingCandidate = null;

                if (_group != null)
                {
                    _group.Deativate();
                    _group = null;
                }
            }
            else
            {
                _groupingFocuseTime += Time.deltaTime;


                if (_groupingFocuseTime > 1.0f && _group == null)
                {

                    if (_groupingCandidate is SGroup)
                    {
                        _group = _groupingCandidate as SGroup;
                    }
                    else
                    {
                        //create SGroup
                        Transform parent = GameObject.Find("_Windows").transform;
                        Vector3 position = Vector3.Lerp(transform.position, _groupingCandidate.transform.position, 0.5f);
                        Quaternion rotation = Quaternion.Lerp(transform.rotation, _groupingCandidate.transform.rotation, 0.5f);
                        _group = Instantiate(_groupingPrefab, position, rotation, parent).GetComponent<SGroup>();
                    }
                }
            }
        }
    }

    protected void FinishGroupingCheck()
    {
        if (_logicState == LOGIC.Group)
        {
            return;
        }

        if (_group != null)
        {
            _group.Activate();
            _group.AddChild(this);
            if(_group != _groupingCandidate) _group.AddChild(_groupingCandidate);
            _group = null;
        }
    }

    protected virtual void UpdateBasicPressed()
    {
        //Move to Destination position & direction
        transform.position = CurrentPosition;
        transform.LookAt(CurrentDirection, Vector3.up);
        _collider.size = PressedBoxColliderSize;
        _collider.center = PressedBoxColliderCenter;
    }

    protected virtual void UpdateBasicDragged()
    {
        //Move to Destination position & direction
        transform.position = CurrentPosition;
        transform.LookAt(CurrentDirection, Vector3.up);

        //Push Other Objects 
        _listNearSWindow.ForEach((window) =>
        {
            Vector3 forceDirection = window.transform.position - transform.position;
            window._rigidbody.AddForce(forceDirection * 10.0f);
        });

        UpdateGroupingCheck();
    }

    protected virtual void UpdateBasicReleased()
    {
        _collider.size = ReleasedBoxColliderSize;
        _collider.center = ReleasedBoxColliderCenter;

        FinishGroupingCheck();
    }

    protected virtual void UpdateGroupPressed()
    {
        //Move to Destination position & direction
        transform.position = CurrentPosition;
        transform.LookAt(CurrentDirection, Vector3.up);

        _collider.size = PressedBoxColliderSize;
        _collider.center = PressedBoxColliderCenter;
    }
    protected virtual void UpdateGroupDragged()
    {
        //Move to Destination position & direction
        transform.position = CurrentPosition;
        transform.LookAt(CurrentDirection, Vector3.up);

        _positionGroupCurr = transform.position;
        _directionGroupCurr = transform.position + transform.forward;
    }
    protected virtual void UpdateGroupReleased()
    {
        _positionGroupCurr += (_positionGroupDest - _positionGroupCurr) * _positionGroupMovingSpeed * Time.deltaTime;
        transform.position = _positionGroupCurr;

        _directionGroupCurr += (_directionGroupDest - _directionGroupCurr) * _directionGroupSpiningSpeed * Time.deltaTime;
        transform.LookAt(_directionGroupCurr, Vector3.up);

        _collider.size = ReleasedBoxColliderSize;
        _collider.center = ReleasedBoxColliderCenter;
    }

    public void ActivateMode(LOGIC mode)
    {
        _logicState = mode;

        _positionCurr = transform.position;
        _directionCurr = transform.position + transform.forward;

        _positionGroupCurr = transform.position;
        _directionGroupCurr = transform.position + transform.forward;

        switch (mode)
        {
            case LOGIC.Basic:
                _updateLogic = _updateBasicLogic;
                break;

            case LOGIC.Group:
                _updateLogic = _updateGroupLogic;
                break;

            case LOGIC.None:
            default:
                _updateLogic = null;
                break;
        }
    }

    public void SetTransform(Vector3 position, Vector3 direction)
    {
        _positionGroupDest = position;
        _directionGroupDest = _positionGroupDest + direction;
    }

    public void SetActivationCollider(bool b)
    {
        _collider.enabled = b;
    }
}
