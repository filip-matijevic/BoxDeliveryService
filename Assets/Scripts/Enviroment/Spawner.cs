using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Current;
    [SerializeField] private List<Box> _spawnPrefabs;
    [SerializeField] private float _spawnRange;

    [SerializeField] private int _initBoxCount = 3;

    private int boxID;
    // Start is called before the first frame update
    void Awake()
    {
        Current = this;

        for (int i = 0; i < _initBoxCount; i++)
        {
            SpawnRandomBox();
        }
    }

    public void SpawnRandomBox()
    {
        Box spawnedBox = Instantiate(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Count)], Vector3.up * 2f +Vector3.right * Random.Range(-_spawnRange, _spawnRange), Quaternion.identity);
        spawnedBox.name = "BOX " + boxID;
        boxID++;
    }
}
