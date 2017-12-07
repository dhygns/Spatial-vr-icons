using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SWindow : MonoBehaviour
{
    //SWindow Grouping
    private int _memberCount, _memberID;
    private float _memberRatio = 0.0f;
    private bool _isReadyForGrouping;
    private bool _isGrouping;
    private GameObject _prefabGroup;
    private SWindowGroup _swTmp;


    //SWindow Components
    protected BoxCollider _boxCollider;
    protected Rigidbody _rigidBody;

    //SWindow Logics
    protected delegate void UpdateLogic();
    protected UpdateLogic _updateLogic = null;
    protected UpdateLogic _updateGrouping = null;

    //SWindow scale
    protected Vector3 _targetScale;
    protected Vector3 _currentScale;

    //SWindow positions
    protected Vector3 _targetPosition;
    protected Vector3 _currentPosition;

    //SWindow lookers
    protected Vector3 _targetLooker;
    protected Vector3 _currentLooker;

    //SWindow vars
    protected Vector3 _boxColliderSizeReleased;
    protected Vector3 _boxColliderSizeDraged;
    protected Vector3 _boxColliderCenterReleased;
    protected Vector3 _boxColliderCenterDraged;

    private Ray _ray;
    private RaycastHit _hit;

    private SWindow _groupingCoord = null;
    private float _groupingTimer = 0.0f;

    //List of SWindows
    protected List<SWindow> _listCollidedSWindows;

    public void InitRigidBody(Rigidbody cmpt)
    {
        _rigidBody = cmpt;

        cmpt.useGravity = false;
        cmpt.constraints = RigidbodyConstraints.FreezeRotation;
        cmpt.drag = 10.0f;
    }

    public void InitBoxCollider(BoxCollider cmpt)
    {
        _boxCollider = cmpt;

        cmpt.enabled = true;

        _boxColliderSizeReleased = cmpt.size;
        _boxColliderCenterReleased = cmpt.center;

        _boxColliderSizeDraged = new Vector3(cmpt.size.x, cmpt.size.y, Mathf.Min(cmpt.size.y, 1.0f));
        _boxColliderCenterDraged = cmpt.center;
    }

    protected virtual void Awake()
    {
        //Load Resources 
        _isReadyForGrouping = false;
        _isGrouping = false;

        _prefabGroup = Resources.Load("Prefabs/Group") as GameObject;
        _updateGrouping = UpdateCheckingGrouping;

        //init objects
        _listCollidedSWindows = new List<SWindow>();
        _ray = new Ray();

        //Position &  Looker Init
        _updateLogic = null;
        _updateLogic += UpdateDefaultScale;
        _updateLogic += UpdateDefaultPosition;
        _updateLogic += UpdateDefaultLooker;

        _currentScale = _targetScale = transform.localScale;
        _currentPosition = _targetPosition = transform.position;
        _currentLooker = _targetLooker = transform.position + transform.forward;

        //added components
        InitComponent<Rigidbody>(InitRigidBody);
        InitComponent<BoxCollider>(InitBoxCollider);
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        Updates();

        _listCollidedSWindows.ForEach((SWindow sw) =>
        {
            if (sw.IsGrouping == false)
            {
                sw._rigidBody.AddForce((sw.TargetPosition - TargetPosition).normalized * 10.0f);
            }
        });
    }

    public virtual void Updates()
    {
        //Update Logic Function
        if (_updateLogic != null)
        {
            _updateLogic();
        }

        //Update Grounping Function
        if (_updateGrouping != null)
        {
            _updateGrouping();
        }
    }

    public virtual void UpdateDefaultScale()
    {
        _currentScale += (_targetScale - _currentScale) * 8.0f * Time.deltaTime;
        transform.localScale = _currentScale;
    }

    public virtual void UpdateDefaultPosition()
    {
        //No Working, Depends on rigidbody.
    }

    public virtual void UpdateDefaultLooker()
    {
        //Just Looking forward
    }

    public virtual void UpdateGrapedScale()
    {
        _currentScale += (_targetScale - _currentScale) * 8.0f * Time.deltaTime;
        transform.localScale = _currentScale;
    }

    public virtual void UpdateGrapedPosition()
    {
        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;
    }

    public virtual void UpdateGrapedLooker()
    {
        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        transform.LookAt(_currentLooker, Vector3.up);
    }

    public virtual void UpdateGroupingScale()
    {

    }

    public virtual void UpdateGroupingPosition()
    {
        _targetPosition.x = transform.parent.position.x;
        _targetPosition.y = transform.parent.position.y + 0.1f;
        _targetPosition.z = transform.parent.position.z;

        _targetPosition += transform.parent.right * _memberRatio;

        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;
    }

    public virtual void UpdateGroupingLooker()
    {
        _targetLooker = _currentPosition + transform.parent.forward;

        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        transform.LookAt(_currentLooker, Vector3.up);
    }

    public void UpdateCheckingGrouping()
    {
        if (_isGrouping == true) return;

        _ray.origin = transform.position;
        _ray.direction = -transform.forward;

        //Search Object For Grouping
        if (_isReadyForGrouping && Physics.Raycast(_ray, out _hit, 1.0f) && _hit.transform != null && _hit.transform.GetComponent<SWindow>().IsGrouping == false)
        {
            SWindow coord = _hit.transform.GetComponent<SWindow>();
            if (coord != null && coord == _groupingCoord)
            {
                _groupingTimer += Time.deltaTime;

                if (_groupingTimer > 1.0f)
                {
                    if(_groupingCoord is SWindowGroup)
                    {
                        _swTmp = _groupingCoord as SWindowGroup;
                    }
                    else
                    {
                        Vector3 target = (transform.position + _groupingCoord.transform.position) * 0.5f;
                        Transform parent = GameObject.Find("_Windows").transform;
                        GameObject go = Instantiate(_prefabGroup, target, Quaternion.identity, parent);
                        _swTmp = go.GetComponent<SWindowGroup>();

                        go.SetActive(true);
                    }
                    _groupingTimer = 0.0f;
                    _updateGrouping = UpdateCreatingGrouping;
                }
            }
            else
            {
                _groupingCoord = coord;
                _groupingTimer = 0.0f;
            }
        }
        else
        {
            _groupingCoord = null;
        }
    }

    public void UpdateCreatingGrouping()
    {
        _ray.origin = transform.position;
        _ray.direction = -transform.forward;

        //Search Object For Grouping
        if (_isReadyForGrouping)
        {
            if (Physics.Raycast(_ray, out _hit, 1.0f) && _hit.transform != null && (_hit.transform.GetComponent<SWindow>() == _groupingCoord || _hit.transform.GetComponent<SWindow>() == _swTmp))
            {
                return;
            }
            else
            {
                _swTmp.Remove();
                _swTmp = null;
                _groupingCoord = null;
                _updateGrouping = UpdateCheckingGrouping;
                return;
            }
        }
        else
        {
            if (_groupingCoord != _swTmp)
            {
                _swTmp.AddSWindow(_groupingCoord);
            }
            _swTmp.AddSWindow(this);
            _swTmp.Setup();

            //Fix & 
            _swTmp = null;
            _groupingCoord = null;
            _updateGrouping = UpdateCheckingGrouping;
            return;
        }

    }
    public void DoGrouping(Transform parent)
    {
        transform.parent = parent;

        _isGrouping = true;
        _targetPosition = parent.position;
        _targetLooker = parent.forward;

        _updateLogic = null;
        _updateLogic += UpdateGroupingScale;
        _updateLogic += UpdateGroupingPosition;
        _updateLogic += UpdateGroupingLooker;
    }

    public void UnGrouping()
    {
        transform.parent = GameObject.Find("_Windows").transform;
        _isGrouping = false;

        /* * * * NEED FIX * * * */
        //_updateLogic to idle or grab
    }

    public virtual void OnClicked(Vector3 pos, Vector3 forward)
    {
        _isReadyForGrouping = true;

        _currentPosition = transform.position;
        _targetPosition = pos;

        _currentLooker = transform.position + transform.forward;
        _targetLooker = pos + forward;

        _updateLogic = null;
        _updateLogic += UpdateGrapedScale;
        _updateLogic += UpdateGrapedPosition;
        _updateLogic += UpdateGrapedLooker;

        _boxCollider.size = _boxColliderSizeDraged;
        _boxCollider.center = _boxColliderCenterDraged;
        _boxCollider.isTrigger = true;

    }

    public virtual void OnDraged(Vector3 pos, Vector3 forward)
    {
        _targetPosition = pos;
        _targetLooker = pos + forward;

        Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction, Color.cyan);
    }

    public virtual void OnReleased(Vector3 pos, Vector3 forward)
    {
        _isReadyForGrouping = false;

        _targetPosition = pos;
        _targetLooker = transform.position + transform.forward;

        _listCollidedSWindows.Clear();


        if (_isGrouping == true)
        {
            _updateLogic = null;
            _updateLogic += UpdateGroupingScale;
            _updateLogic += UpdateGroupingPosition;
            _updateLogic += UpdateGroupingLooker;
        }
        else
        {
            _updateLogic = null;
            _updateLogic += UpdateDefaultScale;
            _updateLogic += UpdateDefaultPosition;
            _updateLogic += UpdateDefaultLooker;
        }

        _boxCollider.size = _boxColliderSizeReleased;
        _boxCollider.center = _boxColliderCenterReleased;
        _boxCollider.isTrigger = false;

    }

    //Listeners

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform.parent)
        {
            return;
        }

        if (_updateLogic == null)
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
        if (other.transform == transform.parent)
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

    public Vector3 TargetScale
    {
        get
        {
            return _currentScale;
        }

        set
        {
            _targetScale = value;
        }
    }
    
    public void SetID(int id, int count)
    {
        _memberID = id;
        _memberCount = count;
        _memberRatio = ((float)_memberID / (float)(_memberCount - 1) - 0.5f) * (float)(_memberCount - 1) * 0.5f;

    }

    public bool IsGrouping
    {
        get
        {
            return _isGrouping;
        }
    }


}
