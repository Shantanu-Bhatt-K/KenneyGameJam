using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInformation : MonoBehaviour
{
    public float _health;
    public float _value;
    public float _attackSpeed;
    public float _attackDamage;
    // Level of difficulty of the enemy - used in generating waves
    public int _level;
    // The current Node of the enemy
    public NodeClass _currentNode;
    // The node that the enemy is going to attack
    public NodeClass _nextNode;

    public bool _isActive;
    public bool _isDead;


    public float GetHealth()
    {
        return _health;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
