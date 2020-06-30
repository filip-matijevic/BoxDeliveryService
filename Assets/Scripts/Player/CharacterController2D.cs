using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public static Vector2 Velocity;
    public static Vector2 CurrentVelocity;

    [SerializeField] private float _movementDampening = 30f;
    private Rigidbody2D _selfRB;
    // Start is called before the first frame update
    void Awake()
    {
        _selfRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector2 movementDirection)
    {
        _selfRB.velocity = Vector2.SmoothDamp(_selfRB.velocity, movementDirection, ref Velocity, _movementDampening);
        CurrentVelocity = _selfRB.velocity;
    }
}
