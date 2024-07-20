using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
    private bool _hasActiveWave;
    // List of active enemies
    public static List<GameObject> _enemyInformationList = new List<GameObject>();
    // time in between each calculation of nodes and enemies
    private const float WORLD_CALCULATION_INTERVAL = 1f;
    private float _calculationTimer = WORLD_CALCULATION_INTERVAL;
    // This flag will be toggled every time an enemy chooses a side
    bool branchingFlag = false;

    public PlacementManager placementManager = new PlacementManager();

    public List<NodeData> nodeData;
    public List<NodeClass> nodeClasses = new List<NodeClass>();
    public List<Material> materials;
    public bool isEditMode;
    public NodeClass parentNode;
    [HideInInspector]
    public ServerNode serverNode;
    Nodetype placementNode = Nodetype.Turret;
    public List<NodeClass> entryNodes = new List<NodeClass>();


    private int _enemiesKilled = 0;

    public int gameCoins = 10000;
    public int coinsPerTick = 1;
    // Start is called before the first frame update
    void Start()
    {
        AddEntryNode();
        serverNode = new ServerNode();
        serverNode.Init(entryNodes[0], Vector3.zero);
        Camera.main.GetComponent<CamController>().SetTarget(serverNode.model.transform);
        placementManager.serverNode = serverNode;
        placementManager.gameManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isEditMode)
        {
            Debug.Log("Entered Edit Mode");
            Debug.Log("Current node" + placementNode);
            isEditMode = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isEditMode)
        {
            Debug.Log("Exit Edit Mode");
            parentNode = null;
            isEditMode = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && !isEditMode)
        {
            _waveManager.GetComponent<WaveManager>().StartWaves(entryNodes);
            _hasActiveWave = true;
            _waveManager.OnNewWave += (object sender, EventArgs e) => { isEditMode = false; _hasActiveWave = true; };
        }




        if (isEditMode)
            Editmode();
        else
            Playmode();
    }


    void Editmode()
    {

        placementManager.GetInput();
    }

    void Playmode()
    {
        if (_calculationTimer < 0)
        {
            CalculateInteractions();
            _calculationTimer = WORLD_CALCULATION_INTERVAL;
            UpdateCoins();
            foreach (NodeClass node in nodeClasses)
                node.Update();
        }
        _calculationTimer -= Time.deltaTime;
    }
    void CalculateInteractions()
    {
        // List of nodes under attack with the total firepower against
        Dictionary<NodeClass, float> nodesUnderAttack = new Dictionary<NodeClass, float>();
        // List of nodes under attack with the enemy on the front
        Dictionary<NodeClass, GameObject> nodesAndEnemies = new Dictionary<NodeClass, GameObject>();
        foreach (var enemy in _enemyInformationList)
        {
            EnemyInformation enemyInformation = enemy.GetComponent<EnemyInformation>();
            // Finding target Node
            NodeClass targetNode = null;
            for (int i = 0; i < enemyInformation._nextNodes.Count; i++)
            {
                // If the next Node is hacked change the node
                if (enemyInformation._nextNodes[i].data.isHacked)
                {
                    enemyInformation._currentNode = enemyInformation._nextNodes[i];
                    enemyInformation._nextNodes = enemyInformation._nextNodes[i].children;
                    // Then set the new target node
                    // If the node we just moved is branching
                    if (enemyInformation._currentNode.data.type == Nodetype.Branch)
                    {
                        enemyInformation._currentNode = branchingFlag ? enemyInformation._nextNodes[0] : enemyInformation._nextNodes[1];
                        enemyInformation._nextNodes = branchingFlag ? enemyInformation._nextNodes[0].children : enemyInformation._nextNodes[1].children;
                        branchingFlag = !branchingFlag;
                    }
                    else if (!enemyInformation._nextNodes[0].data.isHacked)
                    {
                        targetNode = enemyInformation._nextNodes[0];
                    }
                    enemy.transform.position = enemyInformation._currentNode.model.transform.position;

                    break;
                }
                // If next node is not hacked set it as target
                if (!enemyInformation._nextNodes[i].data.isHacked)
                {
                    // Branching node is hacked by default
                    targetNode = enemyInformation._nextNodes[i];
                    break;
                }
            }

            if (targetNode != null)
            {
                if (nodesUnderAttack.ContainsKey(targetNode))
                    nodesUnderAttack[targetNode] += enemyInformation._damagePerHit * enemyInformation._hitPerSecond;
                else
                    nodesUnderAttack.Add(targetNode, enemyInformation._damagePerHit * enemyInformation._hitPerSecond);
                // 
                if (nodesAndEnemies.ContainsKey(targetNode))
                {
                    if (nodesAndEnemies[targetNode].GetComponent<EnemyInformation>()._health < 0)
                    {
                        nodesAndEnemies[targetNode] = enemy;
                    }
                }
                else
                    nodesAndEnemies.Add(targetNode, enemy);
            }
        }

        // For each Node under attack
        foreach (var attackedNode in nodesUnderAttack)
        {
            NodeClass node = attackedNode.Key;
            float nodeAttackPower = node.data.damagePerHit * node.data.hitsPerSecond;
            float enemyAttackPower = attackedNode.Value;

            // Apply damages
            node.data.health -= enemyAttackPower;
            nodesAndEnemies[node].GetComponent<EnemyInformation>()._health -= nodeAttackPower;

            // Destroy the front attacker
            if (nodesAndEnemies[node].GetComponent<EnemyInformation>()._health < 0)
            {
                gameCoins += nodesAndEnemies[node].GetComponent<EnemyInformation>()._value;
                _enemyInformationList.Remove(nodesAndEnemies[node]);
                GameObject.Destroy(nodesAndEnemies[node]);
                // Remove the key from dictionary (will be replaced in the next calculation)
                nodesAndEnemies.Remove(node);
                _enemiesKilled++;

                //  One wave finished
                if (_enemyInformationList.Count == 0)
                {
                    _waveManager.ResetTimer();
                    _hasActiveWave = false;
                    foreach(NodeClass resetNode in nodeClasses)
                    {
                        resetNode.ResetNode();
                    }
                    // TODO: add the continue button here
                    // Set the waveManagerTimer
                    isEditMode = true;
                    return;
                }
            }

            // Destroy the node
            if (node.data.health < 0)
            {
                node.data.isHacked = true;
                if (node.data.type == Nodetype.Turret)
                {
                    Debug.Log("Turret down!");
                }
                if (node.data.type == Nodetype.Branch)
                {
                    Debug.Log("Branch down!");
                }
                if (node.data.type == Nodetype.Farm)
                {
                    Debug.Log("Farm Down");
                }
                if (node.data.type == Nodetype.Server)
                {
                    Debug.Log("Server down! You lost!!");
                    isEditMode = true;
                }
            }
        }
    }

    void UpdateCoins()
    {
        gameCoins += coinsPerTick;
        Debug.LogWarning("game Coins=" + gameCoins);
    }
    public void AddEntryNode()
    {
        EntryNode entryNode = new EntryNode();
        entryNode.Init(UnityEngine.Random.insideUnitSphere * 10);
        nodeClasses.Add(entryNode);
        entryNodes.Add(entryNode);
        serverNode?.AddParentNode(entryNode);
    }
}
