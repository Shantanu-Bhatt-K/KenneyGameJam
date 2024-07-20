using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placementNode = Nodetype.Turret;
            Debug.Log("Current node" + placementNode);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placementNode = Nodetype.Branch;
            Debug.Log("Current node" + placementNode);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            placementNode = Nodetype.Farm;
            Debug.Log("Current node" + placementNode);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddEntryNode();
            Debug.Log("Added entry Node");

        }

        if (Input.GetMouseButtonDown(1))
        {
            // Perform a raycast from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Get the GameObject that was hit
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.GetComponent<NodeReference>() != null)
                {
                    if (clickedObject.GetComponent<NodeReference>().noderef != null)
                    {

                        parentNode = clickedObject.GetComponent<NodeReference>().noderef;
                        if (parentNode.GetType() == typeof(ServerNode))
                        {
                            Debug.Log("Cannot make Child of server node");
                            parentNode = null;
                        }
                        else
                            Debug.Log("FoundParent");
                    }

                }
            }
        }

        Vector3 mousePosition = Input.mousePosition;

        // Convert the screen space mouse position to a world space position
        mousePosition.z = 10; // Set the distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (parentNode == null)
            return;
        // Set the object's position to the calculated world position
        if (Input.GetMouseButtonDown(0))
        {
            switch (placementNode)
            {
                case Nodetype.Turret:
                    TurretNode tnode = new TurretNode();
                    tnode.Init(parentNode, worldPosition);
                    parentNode = null;
                    break;
                case Nodetype.Branch:
                    BranchingNode bnode = new BranchingNode();
                    bnode.Init(parentNode, worldPosition);
                    tnode = new TurretNode();
                    tnode.Init(bnode, worldPosition + Random.insideUnitSphere * 4f);
                    serverNode.AddParentNode(tnode);
                    parentNode = null;
                    break;
                case Nodetype.Farm:
                    FarmNode fnode = new FarmNode();
                    fnode.Init(parentNode, worldPosition);
                    parentNode = null;
                    break;

            }
        }

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
