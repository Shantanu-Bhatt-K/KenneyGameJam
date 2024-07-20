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
        this.model = GameObject.Instantiate(data.model,position,Quaternion.identity);
        model.GetComponent<NodeReference>().noderef = this;
    }

    
}
