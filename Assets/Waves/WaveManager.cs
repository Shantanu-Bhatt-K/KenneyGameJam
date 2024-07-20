using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<GameObject> _enemyPrefabs;
    private Wave _currentWave;
    GameObject _waveObject;
    private int _currentWaveIndex;
    // Random number generator to release
    System.Random random;
    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        _currentWave = null;
        _waveObject = null;
        _currentWaveIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentWaveIndex % 5 == 0)
        {
            // Add another random entry point
        }

    }

    public void StartWaves()
    {
        // Creates the first wave
        CreateWave();
    }
    public void CreateWave()
    {
        List<NodeClass> list = new List<NodeClass>();
        _waveObject = new GameObject("Wave");
        _currentWave = gameObject.AddComponent<Wave>();
        _currentWave.Create(list, _enemyPrefabs, 5, 2000, 3);
        _currentWave.OnFinish += WaveComplete;
        _currentWaveIndex++;
    }
    private void WaveComplete(object sender, EventArgs eventArgs)
    {
        CreateWave();

    }
}
