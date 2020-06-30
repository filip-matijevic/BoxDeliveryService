using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D _characterController;
    [SerializeField] private Animator _animationController;

    [SerializeField] private float _speed;

    public Action<Transform> OnDestinationReached;
    public Action<Transform> OnMovementCompromised;

    [SerializeField] private Transform _destinationTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _characterController.Move(Input.GetAxis("Horizontal") * Vector2.right * _speed);

        float blendValue = Mathf.Abs(CharacterController2D.CurrentVelocity.x) / 5f;

        _animationController.SetFloat("val", blendValue < 1f ? blendValue : 1f);
    }

    public void TravelTo(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(TravelToPosition(target));
    }

    IEnumerator TravelToPosition(Transform target)
    {
        if (target == null)
        {
            Debug.Log("<color=red>Something went wrong</color>");
            yield break;
        }
        _destinationTransform = target;
        while (Mathf.Abs(target.position.x - transform.position.x) > 1.5f)
        {
            if (target == null)
            {
                Debug.Log("<color=red>Something went wrong</color>");
                yield break;
            }
            _characterController.Move(Vector2.right * _speed * Mathf.Sign(target.position.x - transform.position.x));
            yield return null;
        }

        OnDestinationReached?.Invoke(target);
        _destinationTransform = null;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //this can only happen if you hit the desired box
        if (_destinationTransform != null)
        {
            if (other.transform == _destinationTransform)
            {
                StopAllCoroutines();
                OnDestinationReached?.Invoke(other.transform);
                _destinationTransform = null;
            }

            if (other.transform != _destinationTransform && other.transform.CompareTag("Box"))
            {
                StopAllCoroutines();
                Debug.Log("<color=red>SOMETHING IS IN THE WAY!</color>");
                OnMovementCompromised?.Invoke(other.transform);
                
            }
        }


    }
}
