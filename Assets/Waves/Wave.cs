using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    // List of enemies to be generated in the current wave
    List<GameObject> _activeEnemies;
    List<NodeClass> _entryNodes;
    List<GameObject> _enemyPrefabs;
    int _enemyCount;
    int _spawnRate;
    int _highestEnemyLevel;
    System.Random random;
    float timer;
    bool _isFinished;
    public event EventHandler OnFinish;
    public int GetActiveEnemyCount() { return _activeEnemies.Count; }
    public void Create(List<NodeClass> entryNodes, List<GameObject> enemyPrefabs, int enemyCount, int spawnRate, int highestEnemyLevel)
    {
        _activeEnemies = new List<GameObject>();
        _enemyCount = enemyCount;
        _entryNodes = entryNodes;
        _highestEnemyLevel = highestEnemyLevel;
        _spawnRate = spawnRate;
        _highestEnemyLevel = highestEnemyLevel;
        _isFinished = false;
        random = new System.Random();
        timer = (float)random.Next(0, spawnRate) / 1000;
        _enemyPrefabs = enemyPrefabs;
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
        if (_enemyCount == 0)
        {
            _isFinished = true;
            OnFinish(this, EventArgs.Empty);
            Destroy(this);
        }
        if (timer < 0 && !_isFinished)
        {
            // Choose a random entry node
            NodeClass entryNode = _entryNodes[random.Next(0, _entryNodes.Count)];
            // Choose an enemy type randomly
            GameObject enemyPrefab = _enemyPrefabs[random.Next(0, _enemyPrefabs.Count)];


            // Put the eneiesy in the map
            // instantiate an enemy in the entry node location
            GameObject enemy = Instantiate(enemyPrefab,
                entryNode.model.transform.position, Quaternion.identity);
            EnemyInformation enemyInformation = enemy.GetComponent<EnemyInformation>();
            enemyInformation._nextNodes = entryNode.children;
            enemyInformation._currentNode = entryNode;
            //enemyInformation._id = GameManager._enemyInformationList[GameManager._enemyInformationList.Count]._id + 1;
            GameManager._enemyInformationList.Add(enemy);

            // Remove the enemy from the waiting list
            _enemyCount--;
            // Reset the timer with a new random variable
            ResetTimer();

        }
    }
    private void ResetTimer()
    {
        timer = (float)random.Next(0, _spawnRate) / 1000;
    }
}
