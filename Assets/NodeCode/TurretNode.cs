using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretNode : NodeClass
{
    TextMeshProUGUI text;
    Image img;
    Image bgIMG;
    GameObject fireFX;
    Transform endPos;
    Transform graphics;
    public override void Init(NodeClass _parent,Vector3 position)
    {
        this.data = new NodeData(Resources.Load<NodeData>("ScriptableObject/Turret"));
        maxHealth = data.health;
        model = GameObject.Instantiate(data.model, position, Quaternion.identity);
        model.GetComponent<NodeReference>().noderef= this;
        model.GetComponent<NodeReference>().canvas.worldCamera = Camera.main;
        text = model.GetComponent<NodeReference>().text;
        bgIMG = model.GetComponent<NodeReference>().bgimg;
        img = model.GetComponent<NodeReference>().img;
        fireFX = model.GetComponent<NodeReference>().fireFX;
        endPos = model.GetComponent<NodeReference>().endPos;
        graphics = model.GetComponent<NodeReference>().model;
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
            
            Parent.Remove(oldParent);
            parents.Remove(oldParent);
            line.SetPosition(0, newParent.model.transform.position);
            graphics.rotation = Quaternion.LookRotation(newParent.model.transform.position - this.model.transform.position);
            Parent.Add(newParent, line);
            parents.Add(newParent);

        }


    }

    public override void AddParentNode(NodeClass _parent)
    {
       graphics.rotation=Quaternion.LookRotation(_parent.model.transform.position-this.model.transform.position);
        _parent.AddChildren(this);
        LineRenderer lineRenderer = GameObject.Instantiate(Resources.Load<GameObject>("Line"),model.transform).GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, _parent.model.transform.position);
        lineRenderer.SetPosition(1, model.transform.position);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth= 0.05f;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.SetMaterials(GameObject.FindFirstObjectByType<GameManager>().materials);
        Parent.Add(_parent, lineRenderer);
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
            if (parents.Count > 0 && parents[0].data.isHacked)
            {
                fireFX.SetActive(true);
                endPos.position = parents[0].model.transform.position;
            }
            else
                fireFX.SetActive(false);
        }
        else
        {
            img.gameObject.SetActive(false);
            bgIMG.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
            text.text = "HACKED";
            fireFX.SetActive(false) ;
        }

        

    }
}
