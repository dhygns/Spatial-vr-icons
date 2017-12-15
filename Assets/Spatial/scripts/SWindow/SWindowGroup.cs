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

    private Vector3 _titleMinimalizedScale;
    private Vector3 _titleHoveredScale;
    private Vector3 _titleDefaultScale;

    private Vector3 _titleCurrentPosition;
    private Vector3 _titleTargetPosition;

    private Vector3 _titleHoveredPosition;
    private Vector3 _titleMinimalizedPosition;
    private Vector3 _titleDefaultPosition;

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

        _titleCurrentPosition = _titleTransform.localPosition;
        _titleTargetPosition = _titleTransform.localPosition;

        _titleDefaultPosition = _titleTransform.localPosition;
        _titleHoveredPosition = _titleTransform.localPosition;

        //Re Init for Grouping Objects
        _boxCollider.isTrigger = true;
        _boxCollider.enabled = false;

        _boxColliderCenterReleased = new Vector3(0.0f, 0.0f, 0.0f);
        _boxColliderSizeReleased = new Vector3(0.6f, 0.6f, 0.04f);

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
        //Set Main Object Transform
        base.UpdateDefaultLogic();

        //Set Title Object Transform
        _titleCurrentScale += (_titleTargetScale - _titleCurrentScale) * _logicSpeed * Time.deltaTime;
        _titleTransform.localScale = _titleCurrentScale;

        _titleCurrentPosition += (_titleTargetPosition - _titleCurrentPosition) * _logicSpeed * Time.deltaTime;
        _titleTransform.localPosition = _titleCurrentPosition;
    }

    public override void UpdateGrapedLogic()
    {
        //Set Main Object Transform
        _currentScale += (_targetScale - _currentScale) * _logicSpeed * Time.deltaTime;
        transform.localScale = _currentScale;

        _currentPosition += (_targetPosition - _currentPosition) * _logicSpeed * Time.deltaTime;
        transform.position = _currentPosition;

        _currentLooker += (_targetLooker - _currentLooker) * _logicSpeed * Time.deltaTime;
        _currentLooker.y = transform.position.y;

        transform.LookAt(_currentLooker, Vector3.up);

        //Set Title Object Transform
        _titleCurrentScale += (_titleTargetScale - _titleCurrentScale) * _logicSpeed * Time.deltaTime;
        _titleTransform.localScale = _titleCurrentScale;

        _titleCurrentPosition += (_titleTargetPosition - _titleCurrentPosition) * _logicSpeed * Time.deltaTime;
        _titleTransform.localPosition = _titleCurrentPosition;
    }

    public override void UpdateGroupingLogic()
    {
        //Set Main Object Transform
        base.UpdateGroupingLogic();

        //Set Title Object Transform
        _titleCurrentScale += (_titleTargetScale - _titleCurrentScale) * _logicSpeed * Time.deltaTime;
        _titleTransform.localScale = _titleCurrentScale;

        _titleCurrentPosition += (_titleTargetPosition - _titleCurrentPosition) * _logicSpeed * Time.deltaTime;
        _titleTransform.localPosition = _titleCurrentPosition;

    }

    public new void OnFocused()
    {
        _titleTargetScale = _titleHoveredScale;
        _titleTargetPosition = _titleHoveredPosition;
    }

    public new void OnBlurred()
    {
        _titleTargetScale = _titleDefaultScale;
        _titleTargetPosition = _titleDefaultPosition;
    }
    
    public void Binding()
    {
        //Setup Position & Scale for Default & Hovered When Minimalized
        _titleDefaultPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _titleDefaultScale = new Vector3(0.32f, 0.33f, 0.015f);

        _titleHoveredPosition = _titleDefaultPosition;
        _titleHoveredScale = _titleDefaultScale;

        _titleTargetScale = _titleDefaultScale;
        _titleTargetPosition = _titleDefaultPosition;

        //Setup Collider Via Transform
        _boxColliderSizeReleased = _titleDefaultScale + new Vector3(0.02f, 0.02f, 0.1f);
        _boxColliderCenterReleased = _titleTargetPosition;

        _boxCollider.size = _boxColliderSizeReleased;
        _boxCollider.center = _boxColliderCenterReleased;


        _childrenList.ForEach((sw) =>
        {
            if (sw != null)
            {
                sw.Minimalize();
            }
        });
    }

    public void UnBinding()
    {
        //Setup Position & Scale for Default & Hovered When Generalized
        _titleDefaultPosition = new Vector3(0.0f,-0.3f, 0.0f);
        _titleDefaultScale = new Vector3(0.7f, 0.05f, 0.008f);

        _titleHoveredPosition = _titleDefaultPosition;
        _titleHoveredScale = _titleDefaultScale * 1.1f;

        _titleTargetScale = _titleDefaultScale;
        _titleTargetPosition = _titleDefaultPosition;

        //Setup Collider Via Transform
        _boxColliderSizeReleased = _titleDefaultScale + new Vector3(0.02f, 0.02f, 0.1f);
        _boxColliderCenterReleased = _titleTargetPosition;

        _boxCollider.size = _boxColliderSizeReleased;
        _boxCollider.center = _boxColliderCenterReleased;

        _childrenList.ForEach((sw) =>
        {
            if (sw != null)
            {
                sw.Generalize();
            }
        });
    }

    public override void Minimalize()
    {
        base.Minimalize();
        //make child scale zero.
    }

    public override void Generalize()
    {
        base.Generalize();
        //make child scale orin.
    }

    public override void DoGrouping(Transform parent)
    {
        base.DoGrouping(parent);
        Binding();
    }

    public override void UnGrouping()
    {
        base.UnGrouping();
        UnBinding();
    }

    // 
    public override void OnPressed(Vector3 pos, Vector3 forward)
    {
        base.OnPressed(pos, forward);

        //Fixed informations 
        _boxCollider.isTrigger = true;
    }

    public override void OnDragged(Vector3 pos, Vector3 forward)
    {
        base.OnDragged(pos, forward);
    }

    public override void OnReleased(Vector3 pos, Vector3 forward)
    {
        base.OnReleased(pos, forward);

        //Fixed informations 
        _boxCollider.isTrigger = true;
    }
    public override void OnClicked(int ClickCount)
    {
        base.OnClicked(ClickCount);

        switch(ClickCount)
        {
            case 1: break;
            default: break;
        }
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
