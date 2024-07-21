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


    public override void switchParent(NodeClass oldParent, NodeClass newParent)
    {
        if (oldParent == newParent) return;
        if (!Parent.ContainsKey(oldParent))
            AddParentNode(newParent);
        else
        {
           
            LineRenderer line = Parent[oldParent];
            GameObject temp = ParentAttack[oldParent];
            ParentAttack.Remove(oldParent);
            Parent.Remove(oldParent);
            parents.Remove(oldParent);
            line.SetPosition(0, newParent.model.transform.position);
            Parent.Add(newParent, line);
            parents.Add(newParent);

            temp.transform.GetChild(0).GetChild(3).transform.position = newParent.model.transform.position;
            ParentAttack.Add(newParent, temp);
        }


    }
    public override void AddParentNode(NodeClass _parent)
    {
        
        _parent.AddChildren(this);
        LineRenderer lineRenderer = GameObject.Instantiate(Resources.Load<GameObject>("Line"), model.transform).GetComponent<LineRenderer>();
        GameObject temp = GameObject.Instantiate(Resources.Load<GameObject>("Model"),model.transform);
        temp.transform.GetChild(0).GetChild(3).transform.position=_parent.model.transform.position;
        ParentAttack.Add(_parent, temp);
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
            
            foreach(KeyValuePair<NodeClass,GameObject> kvp in ParentAttack)
            {
               
                if(kvp.Key.data.isHacked)
                {
                    kvp.Value.SetActive(true);
                }
            }
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
        foreach (var kvp in ParentAttack)
        {
            kvp.Value.SetActive(false);
        }
    }
}
