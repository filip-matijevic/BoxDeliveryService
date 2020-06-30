using System;
using System.Collections;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;

    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Transform _magnetRoot;

    [SerializeField] private Transform _handleEnd;

    [SerializeField] private float _scanningHalfTime;

    private const float HORIZONTAL_CAMERA_OFFSET = 0.5f;

    [SerializeField] private LayerMask _boxLayer;

    public Action<Box> OnBoxDiscovered;
    public Action<Box> OnBoxPickedUp;
    public Action OnBoxDelivered;

    [SerializeField] private Transform objectHandle;

    [Header("Wiggly Animation")]
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _animationTime = 2f;
    [SerializeField] private Transform _animationRoot;
    private Transform _animationEnd;
    [SerializeField] public Box itemInHand;

    private float currentTime = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        _animationEnd = _animationRoot.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (itemInHand != null)
        {
            currentTime += Time.deltaTime;
            _animationRoot.localEulerAngles = Vector3.Lerp(Vector3.forward * 30f, -Vector3.forward * 30f,
                _curve.Evaluate(currentTime / _animationTime));

            _handleEnd.position = _animationEnd.position;
            _magnetRoot.rotation = _animationRoot.rotation;
            if (currentTime > _animationTime)
            {
                currentTime = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
        }
    }

    public void Find()
    {
        _magnetRoot.gameObject.SetActive(false);
        _cameraRoot.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ScanFieldCoroutine());
    }

    IEnumerator ScanFieldCoroutine()
    {
        yield return null;
        yield return StartCoroutine(ScanFieldSideCoroutine(new Vector2(-HORIZONTAL_CAMERA_OFFSET, 0f),
            new Vector2(-HORIZONTAL_CAMERA_OFFSET, 1f), 350f, 270f));

        yield return StartCoroutine(ScanFieldSideCoroutine(new Vector2(HORIZONTAL_CAMERA_OFFSET, 0f),
            new Vector2(HORIZONTAL_CAMERA_OFFSET, 1f), 10f, 90f));


        OnBoxDiscovered?.Invoke(null);
    }

    IEnumerator ScanFieldSideCoroutine(Vector2 startGimbal, Vector2 endGimbal, float startAngle, float endAngle)
    {
        float currentTime = 0f;

        while (currentTime<_scanningHalfTime)
        {
            currentTime += Time.deltaTime;
            _cameraRoot.localEulerAngles = Vector2.Lerp(new Vector2(startAngle, 270f), new Vector2(endAngle, 270f),
                currentTime / _scanningHalfTime);
            _handleEnd.localPosition = Vector2.Lerp(startGimbal, endGimbal, currentTime / _scanningHalfTime);
            
            TryFindBox();
            yield return null;
        }
        yield return null;
    }

    void TryFindBox()
    {
        RaycastHit2D hit = Physics2D.Raycast(_cameraRoot.position, _cameraRoot.forward, float.PositiveInfinity, _boxLayer);

        if (hit)
        {
            Box foundBox = hit.transform.GetComponent<Box>();

            if (foundBox.State == BoxState.Undelivered)
            {

                StopAllCoroutines();
                OnBoxDiscovered?.Invoke(hit.transform.GetComponent<Box>());
            }
        }
    }

    public void PickUpTheBox(Transform box)
    {
        _magnetRoot.gameObject.SetActive(true);
        _cameraRoot.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(BoxPickupCoroutine(box.GetComponent<Box>()));
    }

    public void ReleaseBox(Vector2 side)
    {

        StopAllCoroutines();
        StartCoroutine(ReleaseBoxCoroutine(side));
    }

    public void ThrowBox(Transform target)
    {
        float requiredDistance = Mathf.Abs(transform.position.x - target.position.x);
        float maxMaxni = Mathf.Sqrt(requiredDistance * -Physics.gravity.y);

        itemInHand.SetAsBalistic(new Vector3(Mathf.Sign(target.position.x), 1f).normalized * maxMaxni);
        itemInHand.transform.SetParent(null);

        //_boxInHand.OnThrow?.Invoke(_boxInHand);
        itemInHand = null;
    }

    IEnumerator BoxPickupCoroutine(Box box)
    {
        float currentTime = 0f;
        Vector2 initPos = _handleEnd.position;

        while (currentTime < _scanningHalfTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
            _handleEnd.position = Vector2.Lerp(initPos, box.transform.position, currentTime / _scanningHalfTime);
        }

        box.transform.SetParent(objectHandle);
        box.transform.localPosition = Vector3.zero;
        box.transform.localEulerAngles = Vector3.zero;
        box.SetKinematic(true);

        itemInHand = box;
        OnBoxPickedUp?.Invoke(box);
    }

    IEnumerator ReleaseBoxCoroutine(Vector2 handleEnd)
    {
        float currentTime = 0f;
        Vector2 initPos = _handleEnd.position;

        while (currentTime < _scanningHalfTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
            _handleEnd.position = Vector2.Lerp(initPos, handleEnd, currentTime / _scanningHalfTime);
        }
        itemInHand.SetKinematic(false);
        itemInHand.State = BoxState.Delivered;
        itemInHand.transform.SetParent(null);
        itemInHand = null;

        OnBoxDelivered?.Invoke();

    }
}
