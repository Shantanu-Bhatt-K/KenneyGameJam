using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeData", order = 1)]
public class NodeData :ScriptableObject
{
    public float health;
    public Nodetype type;
    public dmgType dmg;
    public float damagePerHit;
    public float hitsPerSecond;
    public float coinCount;
    public bool isHacked;
    public int maxChildren;
    public GameObject model;
   

}
