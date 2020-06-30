using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ToolManager _toolsManager;
    [SerializeField] private PlayerMovement _movementManager;

    [Header("Platforms")]
    [SerializeField] private Transform _redPlatform;
    [SerializeField] private Transform _bluePlatform;

    // Start is called before the first frame update
    void Start()
    {
        FindAndPickUpABox();
    }

    // Update is called once per frame


    void FindAndPickUpABox()
    {
        _toolsManager.OnBoxDelivered -= FindAndPickUpABox; 
        
        _toolsManager.OnBoxDiscovered = TravelToTheBox;
        _toolsManager.Find();
    }

    private void TravelToTheBox(Box obj)
    {
        _toolsManager.OnBoxDiscovered -= TravelToTheBox;
        if (obj == null)
        {
            FindAndPickUpABox();
            return;
        }

        Debug.Log("Found Box " + obj.name);
        _movementManager.OnDestinationReached = PickUpTheBox;
        _movementManager.TravelTo(obj.transform);

        _movementManager.OnMovementCompromised = ResolvePathingError;


    }

    private void PickUpTheBox(Transform box)
    {

        Debug.Log("Reached the Box " + box.name);
        _movementManager.OnDestinationReached -= PickUpTheBox;
        _toolsManager.PickUpTheBox(box);

        _toolsManager.OnBoxPickedUp = DeliverItToPlatform;
    }

    private void DeliverItToPlatform(Box obj)
    {

        Debug.Log("Picked up the Box " + obj.name);
        _toolsManager.OnBoxPickedUp = null;

        if (obj.Color == BoxColor.Red)
        {
            _movementManager.TravelTo(_redPlatform);
            _movementManager.OnDestinationReached += ReleaseBox;
        }
        else
        {
            _movementManager.TravelTo(_bluePlatform);
            _movementManager.OnDestinationReached += ReleaseBox;
        }

        _movementManager.OnMovementCompromised = ResolvePathingError;
    }

    private void ResolvePathingError(Transform pathingError)
    {
        _movementManager.OnMovementCompromised -= ResolvePathingError;
        if (_toolsManager.itemInHand != null)
        {
            Debug.Log("<color=yellow>I DO HAVE A BOX, GONNA THROW IT</color>");
            _toolsManager.ThrowBox(_toolsManager.itemInHand.Color == BoxColor.Blue ? _bluePlatform.transform : _redPlatform.transform);
            FindAndPickUpABox();
        }
        else
        {
            Debug.Log("<color=red>I DO NOT HAVE A BOX, BETTER PICK THIS ONE UP</color>");
            PickUpTheBox(pathingError);
        }
    }

    private void ReleaseBox(Transform obj)
    {

        Debug.Log("Destination reached " + obj.name);
        _movementManager.OnDestinationReached -= ReleaseBox;
        _toolsManager.ReleaseBox(obj.position + Vector3.up);

        _toolsManager.OnBoxDelivered += FindAndPickUpABox;
    }
}
