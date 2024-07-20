using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
    public PlacementManager placementManager=new PlacementManager();
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
