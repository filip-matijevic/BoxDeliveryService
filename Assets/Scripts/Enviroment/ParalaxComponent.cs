using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxComponent : MonoBehaviour
{
    [SerializeField] private float _paralaxDampner = 0.1f;
    [SerializeField] private Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.right * _player.position.x * _paralaxDampner;
    }
}
