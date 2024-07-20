using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 destination;
    private float _distance;
    private float distanceTraveled = 0;
    private Vector3 _lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTraveled += Vector3.Magnitude(transform.position - _lastPosition);
        if (distanceTraveled > _distance)
        {
            GameObject.Destroy(this.gameObject);
        }
        _lastPosition = transform.position;
    }
    public void SetDestinaion(Vector3 position) { destination = position; }
    public void SetDistance(float distance) { _distance = distance; }
}
