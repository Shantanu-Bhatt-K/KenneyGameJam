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
    public int _startingNumberOfEnemies;

    public event EventHandler OnNewWave;
    public float _timeBetweenWaves;

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
            OnNewWave(this, EventArgs.Empty);
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
        _currentWave.Create(_entryPoints, _enemyPrefabs, _startingNumberOfEnemies + _currentWaveIndex * 2, 2500, 3);
        //_currentWave.OnFinish += WaveComplete;
        _currentWaveIndex++;
    }
    public void SetTimer(float time)
    {
        _timer = time;
    }
    public void ResetTimer()
    {
        _timer = _timeBetweenWaves;
        _hasActiveWave = false;
    }
}
