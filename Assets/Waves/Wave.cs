using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    // List of enemies to be generated in the current wave
    List<GameObject> _activeEnemies;
    List<NodeClass> _generateLocations;
    List<GameObject> _enemyPrefabs;
    int _enemyCount;
    int _spawnRate;
    int _highestEnemyLevel;
    System.Random random;
    float timer;
    bool _isFinished;
    public event EventHandler OnFinish;
    public int GetActiveEnemyCount() { return _activeEnemies.Count; }
    public void Create(List<NodeClass> generateLocations, List<GameObject> enemyPrefabs, int enemyCount, int spawnRate, int highestEnemyLevel)
    {
        _activeEnemies = new List<GameObject>();
        _enemyCount = enemyCount;
        _generateLocations = generateLocations;
        _highestEnemyLevel = highestEnemyLevel;
        _spawnRate = spawnRate;
        _highestEnemyLevel = highestEnemyLevel;
        _isFinished = false;
        random = new System.Random();
        timer = (float)random.Next(0, spawnRate) / 1000;
        _enemyPrefabs = enemyPrefabs;
        GenerateActiveEnemies();

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        //Debug.Log(timer);
        if (GetActiveEnemyCount() == 0)
        {
            _isFinished = true;
            OnFinish(this, EventArgs.Empty);
            Destroy(this);
        }
        if (timer < 0 && !_isFinished)
        {
            // Put the enemy in the map
            //GameObject enemy = new GameObject("enemy" + GetActiveEnemyCount().ToString());
            //;
            Instantiate(_enemyPrefabs[random.Next(0, 2)], Vector3.one * random.Next(1, 7), Quaternion.identity);
            //enemy.AddComponent<EnemyBase>();
            ////enemy.GetComponent<EnemyBase>()
            //enemy.transform.position = Vector3.one * random.Next(1, 10);
            // Remove the enemy from the list
            _activeEnemies.Remove(_activeEnemies[GetActiveEnemyCount()-1]);
            // Reset the timer with a new random variable
            ResetTimer();

        }
    }
    private void ResetTimer()
    {
        timer = (float)random.Next(0, _spawnRate) / 1000;
    }
    private void GenerateActiveEnemies()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            GameObject enemybase = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _activeEnemies.Add(enemybase);
        }
    }
}
