using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimBehaviour : MonoBehaviour
{
    public bool inSwim;

    private Rigidbody _rb;

    private Transform _swimArea;
    private const float g = 2;

    public float gravityIntensity;
    public float gravityMod;
    
    [SerializeField]
    float submergenceOffset = 0.5f;

    private float submergence;

    public float submergenceRange = 10;
    
    [SerializeField, Range(0f, 10f)]
    float waterDrag = 1f;
    
    [SerializeField, Min(0f)]
    float buoyancy = 1f;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {

        EvaluateSubmergence();
        GravityUpdater();
        if (inSwim) {
            _rb.velocity *= 1f - waterDrag * submergence * Time.deltaTime;

            var d = GetDistance();
            ModifyGravity(d);
        }
    }

    public void GravityUpdater()
    {
        _rb.velocity += Vector3.up * Physics.gravity.y * gravityIntensity * Time.fixedDeltaTime;
    }


    public void ModifyGravity(float d)
    {
        _rb.velocity += Vector3.down * Physics.gravity.y * d * submergence * Time.fixedDeltaTime;
    }

    void EvaluateSubmergence () {
        if (Physics.Raycast(
            transform.position + Vector3.up * submergenceOffset,
            -Vector3.up, out RaycastHit hit,
            1<<LayerMask.NameToLayer("Water")
        )) {
            submergence = 1f - hit.distance / submergenceRange;
        }
        else {
            submergence = 1f;
        }
    }
    
    public float GetDistance()
    {
        var posUp = Vector3.up * transform.position.y;
        float distanceY = _swimArea.position.y - posUp.y;
        return distanceY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1<< other.gameObject.layer == 1 << LayerMask.NameToLayer("Water"))
        {
            gravityIntensity -= gravityMod;
            _swimArea = other.transform;
            inSwim = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (1 << other.gameObject.layer == 1 << LayerMask.NameToLayer("Water"))
        {
            print("Exit");
            gravityIntensity = g;
            _swimArea = null;
            inSwim = false;
        }
    }
    
}
