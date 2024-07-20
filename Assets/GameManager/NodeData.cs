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
    public bool isHacked = false;
    public int maxChildren;
    public GameObject model;
    public int cost;

    public NodeData()
    {
    }

    public NodeData(NodeData other)
    {
        this.health = other.health;
        this.type = other.type;
        this.dmg = other.dmg;
        this.damagePerHit = other.damagePerHit;
        this.hitsPerSecond = other.hitsPerSecond;
        this.coinCount = other.coinCount;
        this.isHacked = other.isHacked;
        this.maxChildren = other.maxChildren;
        this.model = other.model;
        this.cost = other.cost;
    }

}
