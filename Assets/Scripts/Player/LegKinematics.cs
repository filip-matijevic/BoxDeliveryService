using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegKinematics : MonoBehaviour
{
    [SerializeField] private Transform _kinematicTarget;
    [SerializeField] private Transform _kinematicSource;
    [SerializeField] private float _legThresholdDistance = 0.7f;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField, Range(0, 1f)] private float _legDistance = 0.4f;
    [SerializeField] private float _timeToMoveTheLeg = 0.5f;

    private Coroutine _legMovementCoroutine;

    private bool _isInMovement = false;

    [SerializeField] private AnimationCurve _upriseAC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(_kinematicSource.transform.position.y - _kinematicTarget.transform.position.y) > _legThresholdDistance)
        {
            FindNewIdlePosition();
        }
    }

    void FindNewIdlePosition()
    {
        Debug.Log(CharacterController2D.CurrentVelocity);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(_legDistance * Mathf.Sign(CharacterController2D.CurrentVelocity.x), -1f), float.PositiveInfinity, _layerMask);

        if (hit)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            MoveLeg(hit.point);
        }


    }

    
    void MoveLeg(Vector2 position)
    {
        if (_isInMovement)
        {
            return;
            
        }
        if (_legMovementCoroutine != null)
        {
            StopCoroutine(_legMovementCoroutine);
        }

        _legMovementCoroutine = StartCoroutine(MoveLegToPositionCoroutine(position));

    }

    IEnumerator MoveLegToPositionCoroutine(Vector2 newPosition)
    {
        _isInMovement = true;
        Debug.Log("Moving to " + newPosition);
        float currentTime = 0f;
        Vector2 startingPosition = _kinematicTarget.position;
        float startHor = _kinematicTarget.position.x;
        float startVert = _kinematicTarget.position.y;

        while (currentTime < _timeToMoveTheLeg)
        {
            currentTime += Time.deltaTime;
            yield return null;
            _kinematicTarget.position = Vector2.Lerp(startingPosition, newPosition, currentTime / _timeToMoveTheLeg);
        }

        _isInMovement = false;
    }
}
