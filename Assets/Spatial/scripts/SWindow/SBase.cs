using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SBase : MonoBehaviour
{

    //SPointer State
    protected enum POINT
    {
        /// <summary>
        /// this window is pressed by pointer.
        /// </summary>
        Pressed,

        /// <summary>
        /// this window is dragged by pointer.
        /// </summary>
        Dragged,

        /// <summary>
        /// this window is released from pointer.
        /// </summary>
        Released,
    }

    //SWindows Logics updator
    protected POINT _pointState;

    protected delegate void UpdateLogic(POINT state);
    protected UpdateLogic _updateLogic = null;

    //State Logics
    protected abstract void _updateBasicLogic(POINT state);
    protected abstract void _updateGroupLogic(POINT state);


    // position moves positionCurr to positionDest 
    protected float _positionMovingSpeed;
    protected Vector3 _positionDest;
    protected Vector3 _positionCurr;

    // direction spins directionCurr to directionDest
    protected float _directionSpiningSpeed;
    protected Vector3 _directionDest;
    protected Vector3 _directionCurr;

    // scale sizes scaleCurr to scaleDest
    protected float _scaleSizingSpeed;
    protected Vector3 _scaleDest;
    protected Vector3 _scaleCurr;


    //Clicked Logics
    private float __clickedTime;
    private float __clickedTerm;
    private int __clickedCount;

    protected void Awake()
    {

        //Setting Default Transform info
        _positionMovingSpeed = 8.0f;
        _directionSpiningSpeed = 8.0f;
        _scaleSizingSpeed = 8.0f;

        _scaleDest = _scaleCurr = transform.localScale;

        //Setting Default State
        _pointState = POINT.Released;

        //Setting Default Logic
        _updateLogic = _updateBasicLogic;
    }

    // Use this for initialization
    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
        //update advenced logics
        if (_updateLogic != null)
        {
            _updateLogic(_pointState);
        }

        //update default logics
        _positionCurr += (_positionDest - _positionCurr) * _positionMovingSpeed * Time.deltaTime;
        _directionCurr += (_directionDest - _directionCurr) * _directionSpiningSpeed * Time.deltaTime;
        _scaleCurr += (_scaleDest - _scaleCurr) * _scaleSizingSpeed * Time.deltaTime;

        //update default
        DefaultUpdate();
    }

    /// <summary>
    /// This Function will work default.
    /// </summary>
    public virtual void DefaultUpdate()
    {

    }

    /// <summary>
    /// The Function Called When it is Focussed by pointer.
    /// </summary>
    public virtual void OnFocused()
    {
    }

    /// <summary>
    /// The Function Called When it is Blurred by pointer.
    /// </summary>
    public virtual void OnBlurred()
    {
    }

    /// <summary>
    /// The Function Called When it is Clicked by pointer.
    /// </summary>
    /// <param name="ClickedCount"> Clicked Count </param>
    public virtual void OnClicked(int ClickedCount)
    {
    }


    /// <summary>
    /// The Function Called When it is Pressed by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnPressed(Vector3 pos, Vector3 forward)
    {
        //Set State
        _pointState = POINT.Pressed;

        //Set Transform
        _positionCurr = transform.position;
        _directionCurr = transform.position + transform.forward;

        _positionDest = pos;
        _directionDest = pos + forward;

        __clickedTerm = 0.0f;
        if (Time.time - __clickedTime > 0.3)
        {
            __clickedCount = 0;
        }
    }

    /// <summary>
    /// The Function Called When it is Dragged by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnDragged(Vector3 pos, Vector3 forward)
    {
        //Set State
        _pointState = POINT.Dragged;

        //Set Transform
        _positionDest = pos;
        _directionDest = pos + forward;

        __clickedTerm += Time.deltaTime;
    }

    /// <summary>
    /// The Function Called When it is Released by pointer.
    /// </summary>
    /// <param name="pos"> Pointer's Position </param>
    /// <param name="forward"> Direction from body to main hand. </param>
    public void OnReleased(Vector3 pos, Vector3 forward)
    {
        //Set State
        _pointState = POINT.Released;

        //Set Transform
        _positionDest = pos;
        _directionDest = pos + forward;

        __clickedTime = Time.time;
        if (__clickedTerm < 0.3f)
        {
            __clickedCount++;
            OnClicked(__clickedCount);
        }
        else
        {
            __clickedCount = 0;
        }
    }

    public Vector3 CurrentPosition
    {
        get
        {
            return _positionCurr;
        }
    }

    public Vector3 CurrentDirection
    {
        get
        {
            return _directionCurr;
        }
    }

    public Vector3 CurrentScale
    {
        get
        {
            return _scaleCurr;
        }
    }
}
