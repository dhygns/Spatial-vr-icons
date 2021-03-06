﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWindowImage : SWindow
{

    protected override void Awake()
    {
        base.Awake();
        SetMiniIcon("Prefabs/Web_MiniIcon");
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
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
    }
}
