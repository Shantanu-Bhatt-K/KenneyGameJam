using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
    // List of active enemies
    public static List<GameObject> _enemyInformationList = new List<GameObject>();
    // time in between each calculation of nodes and enemies
    private const float WORLD_CALCULATION_INTERVAL = 1f;
    private float _calculationTimer = WORLD_CALCULATION_INTERVAL;
    public PlacementManager placementManager = new PlacementManager();

    public List<NodeData> nodeData;
    List<NodeClass> nodeClasses = new List<NodeClass>();
    public List<Material> materials;
    public bool isEditMode;
    public NodeClass parentNode;
    [HideInInspector]
    public ServerNode serverNode;
    Nodetype placementNode = Nodetype.Turret;
    public List<NodeClass> entryNodes = new List<NodeClass>();
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
        }
        _calculationTimer -= Time.deltaTime;
    }
    void CalculateInteractions()
    {
        // List of nodes under attack with the total firepower against
        Dictionary<NodeClass, float> nodesUnderAttack = new Dictionary<NodeClass, float>();
        // List of nodes under attack with the enemy on the front
        Dictionary<NodeClass, GameObject> nodesAndEnemies = new Dictionary<NodeClass, GameObject>();
        // This flag will be toggled every time an enemy chooses a side
        bool branchingFlag = false;
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
                    enemy.transform.position = enemyInformation._currentNode.model.transform.position;
                    enemyInformation._nextNodes = enemyInformation._nextNodes[i].children;
                    // Then set the new target node
                    foreach (var child in enemyInformation._nextNodes)
                    {
                        // If the node we just moved is branching
                        if (enemyInformation._currentNode.data.type == Nodetype.Branch)
                        {
                            targetNode = branchingFlag ? enemyInformation._nextNodes[0] : enemyInformation._nextNodes[1];
                            branchingFlag = !branchingFlag;
                        }
                        else if (!child.data.isHacked)
                        {
                            targetNode = child;
                        }
                    }
                    break;
                }
                // If next node is not hacked set it as target
                if (!enemyInformation._nextNodes[i].data.isHacked)
                {
                    if (enemyInformation._currentNode.data.type == Nodetype.Branch)
                    {
                        targetNode = branchingFlag ? enemyInformation._nextNodes[0] : enemyInformation._nextNodes[1];
                        branchingFlag = !branchingFlag;
                    }
                    else
                    {
                        targetNode = enemyInformation._nextNodes[i];
                    }
                    break;
                }
            }

            Debug.Log(targetNode?.data.type);
            Debug.Log(branchingFlag.ToString());


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
                _enemyInformationList.Remove(nodesAndEnemies[node]);
                GameObject.Destroy(nodesAndEnemies[node]);
                // Remove the key from dictionary (will be replaced in the next calculation)
                nodesAndEnemies.Remove(node);
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
                if (node.data.type == Nodetype.Server)
                {
                    Debug.Log("Server down! You lost!!");
                }
            }
        }
    }


    public void AddEntryNode()
    {
        EntryNode entryNode = new EntryNode();
        entryNode.Init(Random.insideUnitSphere * 10);
        nodeClasses.Add(entryNode);
        entryNodes.Add(entryNode);
        serverNode?.AddParentNode(entryNode);
    }
}
