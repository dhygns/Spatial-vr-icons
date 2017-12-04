using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWindowImage : SWindow
{

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    
    // 
    public override void OnClicked(Vector3 pos, Vector3 forward)
    {
        base.OnClicked(pos, forward);
    }

    public override void OnDraged(Vector3 pos, Vector3 forward)
    {
        base.OnDraged(pos, forward);

    }

    public override void OnReleased(Vector3 pos, Vector3 forward)
    {
        base.OnReleased(pos, forward);

        _updateLogic = null;
        _updateLogic += updatePosition;
    }
}
