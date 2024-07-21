using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerNode :NodeClass
{

    TextMeshProUGUI text;
    Image img;
    Image bgIMG;
    public override void Init( NodeClass _parent,Vector3 position)
    {
        this.data =new NodeData( Resources.Load<NodeData>("ScriptableObject/Server"));
        this.model=GameObject.Instantiate(data.model,position,Quaternion.identity);
        maxHealth = data.health;
        
        model.GetComponent<NodeReference>().noderef = this;
        model.GetComponent<NodeReference>().canvas.worldCamera = Camera.main;
        text = model.GetComponent<NodeReference>().text;
        bgIMG = model.GetComponent<NodeReference>().bgimg;
        img = model.GetComponent<NodeReference>().img;
        img.gameObject.SetActive(true);
        bgIMG.gameObject.SetActive(true);
        text.gameObject.SetActive(false);
        img.fillAmount = (float)data.health / (float)maxHealth;
        AddParentNode(_parent);
       
    }

    public override void Init(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

   

    public override void AddParentNode(NodeClass _parent)
    {
        
        _parent.AddChildren(this);
        LineRenderer lineRenderer = GameObject.Instantiate(Resources.Load<GameObject>("Line"), model.transform).GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, _parent.model.transform.position);
        lineRenderer.SetPosition(1, model.transform.position);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.SetMaterials(GameObject.FindFirstObjectByType<GameManager>().materials);
        Parent.Add(_parent,lineRenderer);
        parents.Add(_parent);
    }

    public override void Update()
    {
        if (data.health > 0)
        {
            img.gameObject.SetActive(true);
            bgIMG.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
            img.fillAmount = (float)data.health / (float)maxHealth;
        }
        else
        {
            img.gameObject.SetActive(false);
            bgIMG.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
            text.text = "HACKED";
        }
    }
    public override void ResetNode()
    {
        
    }
}
