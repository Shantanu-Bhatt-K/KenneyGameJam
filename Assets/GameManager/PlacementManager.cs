using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum EditStage
{
    None,
    ParentSelected,
    ReadyToBuild
}
public class PlacementManager
{

    public NodeClass serverNode;
    EditStage editStage=EditStage.None;
    NodeClass parentNode=null;
    public NodeClass childNode=null;
    public GameManager gameManager;

    Nodetype placementNode;
   public void PlaceNode(Nodetype nodetype,Vector3 position,NodeClass parent)
    {
        switch (nodetype)
        {
            case Nodetype.Turret:
                TurretNode tnode = new TurretNode();
                tnode.Init(parent, position);
                
                break;
            case Nodetype.Branch:
                BranchingNode bnode = new BranchingNode();
                bnode.Init(parent, position);
                tnode = new TurretNode();
                tnode.Init(bnode, position + Random.insideUnitSphere * 4f);
                serverNode.AddParentNode(tnode);
                
                break;
            case Nodetype.Farm:
                FarmNode fnode = new FarmNode();
                fnode.Init(parent, position);
                
                break;

        }
        parentNode = null;
        childNode = null;
        editStage = EditStage.None;
    }

    public void GetInput()
    {

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Turret");
            placementNode = Nodetype.Turret;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Branch");
            placementNode = Nodetype.Branch;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Farm");
            placementNode = Nodetype.Farm;
        }

        if(Input.GetMouseButtonDown(0) && editStage==EditStage.None)
        {
           
                gameManager.StartCoroutine(SelectParent());
        }
        if(editStage==EditStage.ParentSelected)
        {
            if (!(parentNode.GetType() == typeof(BranchingNode)))
            {
                childNode = parentNode.children[0];
                editStage = EditStage.ReadyToBuild;
                
            }
               
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    gameManager.StartCoroutine(SelectChild());
                }
            }
        }
        if(editStage==EditStage.ReadyToBuild && Input.GetMouseButtonDown(0))
        {
            gameManager.StartCoroutine(BuildNode());
        }
    }

    IEnumerator SelectParent()
    {
        yield return new WaitForEndOfFrame();
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
                    {
                        Debug.Log("FoundParent");
                        editStage = EditStage.ParentSelected;
                    }

                }

            }

        }
        yield return new WaitForEndOfFrame();

    }


    IEnumerator SelectChild()
    {
        yield return new WaitForEndOfFrame();
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

                   childNode = clickedObject.GetComponent<NodeReference>().noderef;
                    if (!parentNode.children.Contains(childNode))
                    {
                        Debug.Log("isNotChild");
                        parentNode = null;
                        childNode = null;
                        editStage = EditStage.None;
                    }
                    else
                    {
                        Debug.Log("Child Found");
                        editStage = EditStage.ReadyToBuild;
                    }

                }

            }

        }
        yield return new WaitForEndOfFrame();

    }
    IEnumerator BuildNode()
    {
        yield return new WaitForEndOfFrame();
        Vector3 mousePosition = Input.mousePosition;

        // Convert the screen space mouse position to a world space position
        mousePosition.z = 10; // Set the distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        PlaceNode(placementNode, worldPosition,parentNode);
        yield return new WaitForEndOfFrame();
    }

   


}
