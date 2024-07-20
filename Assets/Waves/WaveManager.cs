using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<GameObject> _enemyPrefabs;
    private List<NodeClass> _entryPoints;
    private Wave _currentWave;
    private int _numberOfWaves;
    GameObject _waveObject;
    private int _currentWaveIndex;
    private bool _hasActiveWave;
    // Random number generator to release
    System.Random random;
    private float _timer;
    private const float TIME_BETWEEN_WAVES = 7f;
    // Start is called before the first frame update
    void Start()
    {
        _entryPoints = new List<NodeClass>();
        random = new System.Random();
        _currentWave = null;
        _waveObject = null;
        _currentWaveIndex = 0;
        _timer = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        if(!_hasActiveWave && _timer < 0)
        {
            CreateWave(_entryPoints);
        }
    }

    public void StartWaves(List<NodeClass> list)
    {
        // Creates the first wave
        CreateWave(list);
    }
    public void CreateWave(List<NodeClass> list)
    {
        _hasActiveWave = true;
        _entryPoints = list;
        _waveObject = new GameObject("Wave");
        _currentWave = gameObject.AddComponent<Wave>();
        _currentWave.Create(_entryPoints, _enemyPrefabs, 5, 2000, 3);
        _currentWave.OnFinish += WaveComplete;
        _currentWaveIndex++;
    }
    private void WaveComplete(object sender, EventArgs eventArgs)
    {
        _hasActiveWave = false;
        _timer = TIME_BETWEEN_WAVES;
    }
}
