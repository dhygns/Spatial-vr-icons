using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGroup : SWindow
{
    public enum GROUP
    {
        /// <summary>
        /// this state is before activate
        /// </summary>
        Ready,

        /// <summary>
        /// Group normal mode (minimized)
        /// </summary>
        Normal,

        /// <summary>
        /// Group Spreaded mode
        /// </summary>
        Spread,
    }
    protected GROUP _groupState;

    //group children list
    private List<SWindow> __children;

    protected new void Awake()
    {
        base.Awake();
        __children = new List<SWindow>();

        _groupState = GROUP.Ready;
        ActivateMode(LOGIC.None);

        _collider.enabled = false;

        transform.localScale = _scaleCurr = new Vector3(0.0f, 1.0f, 0.0f);
        _scaleDest = Vector3.one;
    }

    public override void DefaultUpdate()
    {
        base.DefaultUpdate();

        if (_groupState != GROUP.Ready)
        {
            __children.ForEach((window) =>
            {
                Vector3 diff = window.transform.position - transform.position;
                if (diff.magnitude > 1.0f)
                {
                    __children.Remove(window);
                    window.ActivateMode(LOGIC.Basic);
                }
            });

            if(__children.Count < 2)
            {
                Deativate();
            }
        }

        switch (_groupState)
        {
            case GROUP.Normal:
                {
                    float index = -(__children.Count - 1) * 0.5f;
                    __children.ForEach((window) =>
                    {
                        window.SetActivationCollider(false);
                        window.SetTransform(
                            transform.position + transform.right * 0.1f * index,
                            (transform.forward + transform.right).normalized);

                        index += 1.0f;
                    });

                    _scaleDest = Vector3.one;
                }
                break;
            case GROUP.Spread:
                {
                    float index = -(__children.Count - 1) * 0.5f;
                    __children.ForEach((window) =>
                    {
                        window.SetActivationCollider(true);
                        window.SetTransform(
                            transform.position + transform.right * 0.4f * index,
                            transform.forward);

                        index += 1.0f;
                    });

                    _scaleDest = new Vector3(1.5f, 1.0f, 1.5f);
                }
                break;
            default: break;
        }
    }

    protected override void UpdateBasicPressed()
    {
        base.UpdateBasicPressed();
    }

    protected override void UpdateBasicDragged()
    {
        base.UpdateBasicDragged();
    }

    protected override void UpdateBasicReleased()
    {
        base.UpdateBasicReleased();
    }

    public override void OnClicked(int ClickedCount)
    {
        base.OnClicked(ClickedCount);

        //Switch State
        if (ClickedCount == 1)
        {
            _groupState = _groupState == GROUP.Normal ? GROUP.Spread : GROUP.Normal;
        }

    }

    public void Activate()
    {
        _groupState = GROUP.Normal;
        ActivateMode(LOGIC.Basic);

        _collider.enabled = true;
    }

    public void Deativate()
    {
        _groupState = GROUP.Ready;
        _scaleDest = new Vector3(0.0f, 1.0f, 0.0f);
        Destroy(gameObject, 1.0f);
    }

    public void AddChild(SWindow child)
    {
        child.ActivateMode(LOGIC.Group);
        child.SetTransform(transform.position, transform.forward);

        __children.Add(child);
    }

    private void OnDestroy()
    {
        __children.ForEach((window) =>
        {
            window.ActivateMode(LOGIC.Basic);
        });
    }
}
