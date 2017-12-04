using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPointerLogic
{
    // Primary Hand Object
    private GameObject _primaryHand;

    // Razer Cursor Position
    private Vector3 _position;

    // Razer Cursor Distant
    private float _maxDistance;
    private float _distance;

    // Ray & hit checker
    private Ray _ray;
    private RaycastHit _hit;
    private SWindow _obj;

    // Use this for initialization
    public SPointerLogic()
    {
        _primaryHand = null;
        _position = Vector3.zero;
        _maxDistance = _distance = 10.0f;

        _ray = new Ray();
        _obj = null;
    }

    /// <summary>
    /// 
    /// </summary>
    private void _updateRayAndHit()
    {
        _ray.origin = _primaryHand.transform.position;
        _ray.direction = _primaryHand.transform.forward;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateIdle()
    {
        if (!IsReady) return;
        _updateRayAndHit();

        if (Physics.Raycast(_ray, out _hit))
        {
            _distance = Mathf.Min(_maxDistance, _hit.distance);
        }

        _position = _ray.origin + _ray.direction * _distance;
        Debug.DrawLine(_ray.origin, _position, Color.gray);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdatePress()
    {
        if (!IsReady) return;
        _updateRayAndHit();

        if (Physics.Raycast(_ray, out _hit))
        {
            Vector3 pos = _hit.collider.gameObject.transform.position - _ray.origin; 
            _distance = Mathf.Min(_maxDistance, pos.magnitude);
        }

        if (_hit.collider != null)
        {
            _obj = _hit.collider.GetComponent<SWindow>();
        }

        _position = _ray.origin + _ray.direction * _distance;

        if (_obj != null)
        {
            Vector3 pos = _obj.transform.position - _ray.origin;
            _maxDistance = pos.magnitude;
            _obj.OnClicked(_position, -_ray.direction);
        }
        Debug.DrawLine(_ray.origin, _position, Color.magenta);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateDrag()
    {
        if (!IsReady) return;
        _updateRayAndHit();

        _position = _ray.origin + _ray.direction * _distance;

        if (Physics.Raycast(_ray, out _hit))
        {
            if (_obj != null && _hit.collider != _obj.GetComponent<Collider>())
            {
                //Vector3 pos = _hit.collider.gameObject.transform.position - _ray.origin;
                //_distance = Mathf.Min(_maxDistance, pos.magnitude);
            }
        }
            

        if (_obj != null)
        {
            _obj.OnDraged(_position, -_ray.direction);
        }

        Debug.DrawLine(_ray.origin, _position, Color.red);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateRelease()
    {
        if (!IsReady) return;
        _updateRayAndHit();

        if (Physics.Raycast(_ray, out _hit))
        {
            Vector3 pos = _hit.collider.gameObject.transform.position - _ray.origin; 
            _distance = Mathf.Min(_maxDistance, pos.magnitude);
        }

        _maxDistance = 10.0f;
        _position = _ray.origin + _ray.direction * _distance;

        if (_obj != null)
        {
            _obj.OnReleased(_position, -_ray.direction);
            _obj = null;
        }
        Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction, Color.magenta);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hand"></param>
    public void SetPrimaryHand(GameObject hand)
    {
        _primaryHand = hand;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsReady
    {
        get
        {
            return _primaryHand != null;
        }
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }
    }
}
