using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWindowGroup : SWindow
{

    private List<SWindow> childrenList;

    private UpdateLogic _update = null;

    protected override void Awake()
    {
        base.Awake();

        childrenList = new List<SWindow>();

        //Re Init for Grouping Objects

        _boxCollider.isTrigger = true;
        _boxCollider.enabled = false;

        _boxColliderCenterReleased = new Vector3(0.0f, -_boxCollider.size.y * 0.25f, 0.0f);
        _boxColliderSizeReleased = new Vector3(
            _boxCollider.size.x * 0.5f,
            _boxCollider.size.y * 0.5f,
            _boxCollider.size.z * 0.5f);

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

    public override void UpdateGrapedLooker()
    {
        _currentLooker += (_targetLooker - _currentLooker) * 8.0f * Time.deltaTime;
        _currentLooker.y = transform.position.y;

        transform.LookAt(_currentLooker, Vector3.up);
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
        childrenList.Add(child);
        child.DoGrouping(transform);

        Refresh();
    }

    public void Refresh()
    {
        //All children Assignment Index for Sorting
        for (int idx = 0; idx < childrenList.Count; idx++)
        {
            SWindow sw = childrenList[idx];
            sw.SetID(idx, childrenList.Count);
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
            childrenList.ForEach((sw) =>
            {
                if ((sw.TargetPosition - transform.position).sqrMagnitude > 2.0)
                {
                    sw.UnGrouping();
                    childrenList.Remove(sw);
                    Refresh();

                    if (childrenList.Count < 2)
                    {
                        Remove();
                    }
                }  
            });
        };
    }

    public void Remove()
    {
        childrenList.ForEach((sw) =>
        {
            sw.UnGrouping();
            childrenList.Remove(sw);
        });

        _targetScale.Set(0.001f, 1.0f, 0.001f);
        Destroy(gameObject, 1.0f);
    }
}
