using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public enum Nodetype
{
    None,
    Turret,
    Farm,
    Entry,
    Branch,
    Server
}
public enum dmgType
{
    noDamage,
    oneParent,
    allParent,
    oneChild,
    allChild,
    allConnections
}
public abstract class NodeClass
{

    public NodeData data;
    public GameObject model;
    public List<NodeClass> children= new List<NodeClass>();
    public Dictionary<NodeClass,LineRenderer> Parent= new Dictionary<NodeClass,LineRenderer>();
    public GameManager gameManager;
    public abstract void Init( NodeClass _parent,Vector3 position);
    public abstract void Init(Vector3 position );
    public abstract void Update();
   

    public abstract void AddParentNode(NodeClass _parent);

    public virtual void switchParent(NodeClass oldParent, NodeClass newParent)
    {
        if(oldParent == newParent) return;
        if (!Parent.ContainsKey(oldParent))
            AddParentNode(newParent);
        else
        {
            LineRenderer line = Parent[oldParent];
            Parent.Remove(oldParent);
            line.SetPosition(0,newParent.model.transform.position);
            Parent.Add(newParent, line);
            
        }
            

    }
    public virtual void RemoveParent(NodeClass _parent)
    {
        if(!Parent.ContainsKey(_parent))
            throw new System.NotImplementedException();
        else
            Parent.Remove(_parent);
    }
    public virtual void AddChildren(NodeClass child)
    {
        if (children.Count >= data.maxChildren)
        {
            NodeClass oldChild=GameObject.FindAnyObjectByType<GameManager>().placementManager.childNode;
            children.Remove(oldChild);
            children.Add(child);
            child.AddChildren(oldChild);
            oldChild.switchParent(this, child);
        }
        else
            children.Add(child);
    }
    public virtual void RemoveChildren(NodeClass child)
    {
        if (children.Count ==0 || !children.Contains(child))
            throw new System.NotImplementedException();
        else
            children.Remove(child);
    }

    public virtual void ResetNode()
    {
        data = new NodeData(Resources.Load<NodeData>("ScriptableObject/" + data.type));

    }


    
}

