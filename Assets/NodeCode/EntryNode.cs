using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryNode : NodeClass
{
    public override void AddParentNode(NodeClass _parent)
    {
        throw new System.NotImplementedException();
    }

    public override void Init( NodeClass _parent,Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public override void Init(Vector3 position)
    {
        this.data = new NodeData(Resources.Load<NodeData>("ScriptableObject/Entry"));
        maxHealth = data.health;
        this.model = GameObject.Instantiate(data.model,position,Quaternion.identity);
        model.GetComponent<NodeReference>().noderef = this;
    }

    public override void AddChildren(NodeClass child)
    {
        if (children.Count >= data.maxChildren)
        {
            NodeClass oldChild = GameObject.FindAnyObjectByType<GameManager>().placementManager.childNode;
            children.Remove(oldChild);
            children.Add(child);
            child.AddChildren(oldChild);
            this.model.transform.rotation= Quaternion.LookRotation(child.model.transform.position-this.model.transform.position);
            oldChild.switchParent(this, child);
        }
        else
        {
            children.Add(child);
            this.model.transform.rotation = Quaternion.LookRotation(child.model.transform.position-this.model.transform.position);
        }
            
    }

    public override void Update()
    {
        
    }
}
