using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
    // List of active enemies
    public static List<EnemyInformation> _enemyInformationList = new List<EnemyInformation>();
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
        placementManager.gameManager= this;
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
        foreach (var enemy in _enemyInformationList)
        {
            NodeClass targetNode = enemy._nextNodes[0];
            //Debug.Log(enemy._damagePerHit * enemy._hitPerSecond);
            if (nodesUnderAttack.ContainsKey(targetNode))
                nodesUnderAttack[targetNode] += enemy._damagePerHit * enemy._hitPerSecond;
            else
                nodesUnderAttack.Add(targetNode, enemy._damagePerHit * enemy._hitPerSecond);
        }

        foreach (var attackedNode in nodesUnderAttack)
        {
            NodeClass node = attackedNode.Key;
            float attackPower = attackedNode.Value;
            Debug.Log(node.data.damagePerHit * node.data.hitsPerSecond - attackPower);
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
