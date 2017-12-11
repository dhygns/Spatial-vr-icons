using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWindowGroup : SWindow
{
    //Children List
    private List<SWindow> _childrenList;

    //Update Logic For Group
    private UpdateLogic _update = null;

    //Title Object
    private Transform _titleTransform;

    private Vector3 _titleCurrentScale;
    private Vector3 _titleTargetScale;

    private Vector3 _titleHoveredScale;
    private Vector3 _titleDefaultScale;

    protected override void Awake()
    {
        base.Awake();

        //Init Children List
        _childrenList = new List<SWindow>();

        //init Title Object
        _titleTransform = transform.Find("Title");

        _titleCurrentScale = _titleTransform.localScale;
        _titleTargetScale = _titleTransform.localScale;

        _titleDefaultScale = _titleTransform.localScale;
        _titleHoveredScale = _titleTransform.localScale * 1.1f;


        //Re Init for Grouping Objects
        _boxCollider.isTrigger = true;
        _boxCollider.enabled = false;

        _boxColliderCenterReleased = new Vector3(0.0f, _boxCollider.size.y * 0.4f, 0.0f);
        _boxColliderSizeReleased = new Vector3(
            1.0f, // _boxCollider.size.x * 0.5f,
            _boxCollider.size.y * 0.1f,
            _boxCollider.size.z * 0.1f);

        _boxColliderCenterDraged = new Vector3(0.0f, 0.0f, 0.0f);
        _boxColliderSizeDraged = new Vector3(
            _boxCollider.size.x,
            _boxCollider.size.y,
            _boxCollider.size.z);

        _boxCollider.center = _boxColliderCenterReleased;
        _boxCollider.size = _boxColliderSizeReleased;

        Ready();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Updates();

        if (_update != null)
        {
            _update();
        }
        
    }


    public override void UpdateDefaultLogic()
    {
        base.UpdateDefaultLogic();

        _titleCurrentScale += (_titleTargetScale - _titleCurrentScale) * 8.0f * Time.deltaTime;
        _titleTransform.localScale = _titleCurrentScale;
    }

    public override void UpdateGrapedLogic()
    {

        _currentScale += (_targetScale - _currentScale) * 8.0f * Time.deltaTime;
        transform.localScale = _currentScale;

        _currentPosition += (_targetPosition - _currentPosition) * 8.0f * Time.deltaTime;
        transform.position = _currentPosition;

        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        _currentLooker.y = transform.position.y;

        transform.LookAt(_currentLooker, Vector3.up);

        _titleCurrentScale += (_titleTargetScale - _titleCurrentScale) * 8.0f * Time.deltaTime;
        _titleTransform.localScale = _titleCurrentScale;
    }

    public override void Focusing()
    {
        base.Focusing();
        _titleTargetScale = _titleHoveredScale;
    }

    public override void Blurring()
    {
        base.Blurring();
        _titleTargetScale = _titleDefaultScale;
    }
    // 
    public override void OnClicked(Vector3 pos, Vector3 forward)
    {
        base.OnClicked(pos, forward);

        //Fixed informations 
        _boxCollider.isTrigger = true;
    }

    public override void OnDraged(Vector3 pos, Vector3 forward)
    {
        base.OnDraged(pos, forward);
    }

    public override void OnReleased(Vector3 pos, Vector3 forward)
    {
        base.OnReleased(pos, forward);

        //Fixed informations 
        _boxCollider.isTrigger = true;
    }

    public void AddSWindow(SWindow child)
    {
        _childrenList.Add(child);
        child.DoGrouping(transform);

        Refresh();
    }

    public void Refresh()
    {
        //All children Assignment Index for Sorting
        for (int idx = 0; idx < _childrenList.Count; idx++)
        {
            SWindow sw = _childrenList[idx];
            sw.SetID(idx, _childrenList.Count);
        }
    }
    public void Ready()
    {
        _currentScale.Set(0.001f, 1.0f, 0.001f);
        transform.localScale = _currentScale;

        _targetScale = Vector3.one;
    }

    public void Setup()
    {
        _boxCollider.enabled = true;

        _update = () =>
        {
            _childrenList.ForEach((sw) =>
            {
                if ((sw.TargetPosition - transform.position).sqrMagnitude > 1.4f)
                {
                    sw.UnGrouping();
                    _childrenList.Remove(sw);
                    Refresh();

                    if (_childrenList.Count < 2)
                    {
                        Remove();
                    }
                }  
            });
        };
    }

    public void Remove()
    {
        //isRemoving = true;
        _targetScale.Set(0.001f, 1.0f, 0.001f);
        Destroy(gameObject, 1.0f);
    }

    public void OnDestroy()
    {
        _childrenList.ForEach((sw) =>
        {
            if (sw != null)
            {
                sw.UnGrouping();
            }
            _childrenList.Remove(sw);
        });
    }
}
