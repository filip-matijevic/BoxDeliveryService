using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxColor
{
    Red, Blue
}

public enum BoxState
{
    Undelivered, Delivered
}
public class Box : MonoBehaviour
{
    public BoxColor Color;
    public BoxState State = BoxState.Undelivered;
    public Rigidbody2D _selfRB;

    public BoxCollider2D _selfCollider;
    // Start is called before the first frame update
    void Awake()
    {
        _selfRB = GetComponent<Rigidbody2D>();
        _selfCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    public void SetKinematic(bool state)
    {
        _selfRB.isKinematic = state;
        _selfCollider.enabled = !state;
    }

    public void SetAsBalistic(Vector2 velocityVector)
    {
        State = BoxState.Delivered;
        _selfRB.isKinematic = false;
        _selfCollider.enabled = true;
        _selfCollider.isTrigger = false;
        _selfRB.velocity = velocityVector;
    }
}
