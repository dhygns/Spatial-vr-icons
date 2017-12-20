using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrouppingEffect : MonoBehaviour {

    private Vector3 __scaleCurr;
    private Vector3 __scaleDest;


    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

        __scaleCurr += (__scaleDest - __scaleCurr) * 4.0f * Time.deltaTime;
        transform.localScale = __scaleCurr;
	}

    public void Begin()
    {
        __scaleCurr = Vector3.zero;
        __scaleDest = Vector3.one;
    }

    public void End()
    {
        __scaleDest = Vector3.zero;
        Destroy(gameObject, 1.0f);
    }
}
