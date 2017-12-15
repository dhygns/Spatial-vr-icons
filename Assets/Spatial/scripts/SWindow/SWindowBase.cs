using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SWindowBase : MonoBehaviour
{
    //SWindow States
    protected enum STATE
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
        /// This state works when group is spreaded for looking around.
        /// </summary>
        Spread
    }

    //SWindows Logics updator
    protected STATE _logicState = STATE.Basic;

    protected delegate void UpdateLogic();
    protected UpdateLogic _updateLogic = null;

    //State Logics
    protected UpdateLogic _updateBasicLogic = null;
    protected UpdateLogic _updateGroupLogic = null;
    protected UpdateLogic _updateSpreadLogic = null;

    //State Detail Logics
    //** Basic Logics
    protected abstract void _updateBasicPressedLogic();
    protected abstract void _updateBasicDraggedLogic();
    protected abstract void _updateBasicReleasedLogic();

    //** Group Logics
    protected abstract void _updateGroupPressedLogic();
    protected abstract void _updateGroupDraggedLogic();
    protected abstract void _updateGroupReleasedLogic();

    //** Group Logics
    protected abstract void _updateSpreadPressedLogic();
    protected abstract void _updateSpreadDraggedLogic();
    protected abstract void _updateSpreadReleasedLogic();


    //Clicked Logics
    private float _clickedTime;
    private float _clickedTerm;
    private int _clickedCount;

    protected void Awake()
    {
        //Setting Default Logic
        _updateLogic = _updateBasicLogic;

        _updateBasicLogic = _updateBasicReleasedLogic;
        _updateGroupLogic = _updateGroupReleasedLogic;
        _updateSpreadLogic = _updateSpreadReleasedLogic;
    }

    // Use this for initialization
    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
        if (_updateLogic != null) _updateLogic();
    }


    /// <summary>
    /// The Function Called When it is Focussed by pointer.
    /// </summary>
    public void OnFocused()
    {

    }

    /// <summary>
    /// The Function Called When it is Blurred by pointer.
    /// </summary>
    public void OnBlurred()
    {

    }


    /// <summary>
    /// The Function Called When it is Pressed by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnPressed(Vector3 pos, Vector3 forward)
    {
        _clickedTerm = 0.0f;
        if (Time.time - _clickedTime > 0.3)
        {
            _clickedCount = 0;
        }
    }

    /// <summary>
    /// The Function Called When it is Dragged by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnDragged(Vector3 pos, Vector3 forward)
    {
        _clickedTerm += Time.deltaTime;
    }

    /// <summary>
    /// The Function Called When it is Released by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnReleased(Vector3 pos, Vector3 forward)
    {
        _clickedTime = Time.time;
        if (_clickedTerm < 0.3f)
        {
            _clickedCount++;
            OnClicked(_clickedCount);
        }
        else
        {
            _clickedCount = 0;
        }
    }

    /// <summary>
    /// The Function Called When it is Clicked by pointer.
    /// </summary>
    /// <param name="ClickedCount"> Clicked Count </param>
    public void OnClicked(int ClickedCount)
    {

    }
}
