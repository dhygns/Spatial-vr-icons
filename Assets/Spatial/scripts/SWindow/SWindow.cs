using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SWindow : MonoBehaviour
{
    //SWindow Status
    protected enum LOGIC
    {
        Default, Clicked, Grabed, Released
    };

    protected enum GROUP
    {
        Ready, None, Done 
    };

    //SWindow Grouping
    private int _memberCount, _memberID;
    private float _memberRatio = 0.0f;
    protected GROUP _group;

    private SWindow _groupingCoord = null;
    private float _groupingTimer = 0.0f;

    private GameObject _prefabGroup;
    private SWindowGroup _swTmp;

    private Transform _parent;


    //SWindow Components
    protected BoxCollider _boxCollider;
    protected Rigidbody _rigidBody;

    //SWindow Logics
    protected LOGIC _logic;

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
        //SetStatus
        _logic = LOGIC.Default;
        _group = GROUP.None;

        //Load Resources 
        _prefabGroup = Resources.Load("Prefabs/Group") as GameObject;

        //init objects
        _listCollidedSWindows = new List<SWindow>();
        _ray = new Ray();
        _parent = transform.parent;

        //Position &  Looker Init
        _updateLogic = null;
        _updateLogic += UpdateDefaultLogic;

        _updateGrouping = UpdateCheckingGrouping;

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
            if (sw._group != GROUP.Done)
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

    public virtual void UpdateDefaultLogic()
    {
        _currentScale += (_targetScale - _currentScale) * 8.0f * Time.deltaTime;
        transform.localScale = _currentScale;

        //Position No Worked, Depends on rigidbody.
        //Looker Just Looking forward
    }

    public virtual void UpdateGrapedLogic()
    {
        _currentScale += (_targetScale - _currentScale) * 8.0f * Time.deltaTime;
        transform.localScale = _currentScale;

        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;

        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        transform.LookAt(_currentLooker, Vector3.up);
    }

    public virtual void UpdateGroupingLogic()
    {
        _targetPosition.x = _parent.position.x;
        _targetPosition.y = _parent.position.y + 0.1f;
        _targetPosition.z = _parent.position.z;

        _targetPosition += _parent.right * _memberRatio;
        _targetLooker = _currentPosition + _parent.forward;

        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;
        
        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        transform.LookAt(_currentLooker, Vector3.up);
    }

    public void UpdateCheckingGrouping()
    {
        if (_group == GROUP.Done) return;

        _ray.origin = transform.position;
        _ray.direction = -transform.forward;

        //Search Object For Grouping
        if (_group == GROUP.Ready && 
            Physics.Raycast(_ray, out _hit, 1.0f) && 
            _hit.transform != null && 
            _hit.transform.GetComponent<SWindow>()._group != GROUP.Done)
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
                        Vector3 targetPosition = (transform.position + _groupingCoord.transform.position) * 0.5f;
                        Quaternion targetRotation = Quaternion.Lerp(transform.rotation, _groupingCoord.transform.rotation, 0.5f);

                        Transform parent = GameObject.Find("_Windows").transform;
                        GameObject go = Instantiate(_prefabGroup, targetPosition, targetRotation, parent);

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
        if (_group == GROUP.Ready)
        {
            if (
                Physics.Raycast(_ray, out _hit, 1.0f) && 
                _hit.transform != null && 
                (_hit.transform.GetComponent<SWindow>() == _groupingCoord || _hit.transform.GetComponent<SWindow>() == _swTmp))
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
        _parent = parent;

        _group = GROUP.Done;

        _targetPosition = parent.position;
        _targetLooker = parent.forward;

        _updateLogic = null;
        _updateLogic += UpdateGroupingLogic;
    }

    public void UnGrouping()
    {
        GameObject go = GameObject.Find("_Windows");
        if(go != null)
        {
            _parent = go.transform;
        }

        _group = GROUP.None;

        _updateLogic = null;
        if (_logic == LOGIC.Grabed)
        {
            _updateLogic += UpdateGrapedLogic;
        }
        else
        {
            _updateLogic += UpdateDefaultLogic;
        }
    }

    public virtual void Focusing()
    {

    }

    public virtual void Blurring()
    {

    }

    public virtual void OnClicked(Vector3 pos, Vector3 forward)
    {
        //Set Status
        _logic = LOGIC.Clicked;

        //Set Grouping Status
        if (_group != GROUP.Done)
        {
            _group = GROUP.Ready;
        }

        _currentPosition = transform.position;
        _targetPosition = pos;

        _currentLooker = transform.position + transform.forward;
        _targetLooker = pos + forward;

        _updateLogic = null;
        _updateLogic += UpdateGrapedLogic;

        _boxCollider.size = _boxColliderSizeDraged;
        _boxCollider.center = _boxColliderCenterDraged;
        _boxCollider.isTrigger = true;

    }

    public virtual void OnDraged(Vector3 pos, Vector3 forward)
    {
        //Set Status
        _logic = LOGIC.Grabed;

        //Set Transform
        _targetPosition = pos;
        _targetLooker = pos + forward;

        //Debugging
        Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction, Color.cyan);
    }

    public virtual void OnReleased(Vector3 pos, Vector3 forward)
    {
        //Set Status
        _logic = LOGIC.Released;

        //Set Grouping Status
        if (_group != GROUP.Done)
        {
            _group = GROUP.None;
        }

        //Set Transform
        _targetPosition = pos;
        _targetLooker = transform.position + transform.forward;

        //Clear CollidedWindows List
        _listCollidedSWindows.Clear();


        if (_group == GROUP.Done)
        {
            _updateLogic = null;
            _updateLogic += UpdateGroupingLogic;
        }
        else
        {
            _updateLogic = null;
            _updateLogic += UpdateDefaultLogic;
        }

        _boxCollider.size = _boxColliderSizeReleased;
        _boxCollider.center = _boxColliderCenterReleased;
        _boxCollider.isTrigger = false;
    }

    //Listeners

    /// <summary>
    /// Collider Manager (Enter)
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _parent)
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

    /// <summary>
    /// Collider Manager (Exit)
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _parent)
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

    /// <summary>
    /// Getter / Setter For Position
    /// </summary>
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

    /// <summary>
    /// Getter / Setter For Looker
    /// </summary>
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

    /// <summary>
    /// Getter / Setter For Scale
    /// </summary>
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

    /// <summary>
    /// Setter For Indexing in Group
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    public void SetID(int id, int count)
    {
        _memberID = id;
        _memberCount = count;

        if(count > 1)
        {
            _memberRatio = ((float)_memberID / (float)(_memberCount - 1) - 0.5f) * (float)(_memberCount - 1) * 0.5f;
        }
    }
}
