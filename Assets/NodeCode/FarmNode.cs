using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmNode : NodeClass
{
    public override void Init(NodeClass _parent, Vector3 position)
    {
        this.data = Resources.Load<NodeData>("ScriptableObject/Farm");
        model = GameObject.Instantiate(data.model, position, Quaternion.identity);
        model.GetComponent<NodeReference>().noderef = this;
        AddParentNode(_parent);

    }

    public override void Init(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {

    }

    public override void AddParentNode(NodeClass _parent)
    {

        _parent.AddChildren(this);
        LineRenderer lineRenderer = GameObject.Instantiate(Resources.Load<GameObject>("Line"), model.transform).GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, _parent.model.transform.position);
        lineRenderer.SetPosition(1, model.transform.position);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.SetMaterials(GameObject.FindFirstObjectByType<GameManager>().materials);
        Parent.Add(_parent, lineRenderer);
    }

}
