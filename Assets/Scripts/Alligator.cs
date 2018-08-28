using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Alligator : MonoBehaviour
{
    private NavMeshAgent _agent;

    private float _minAcceleration = 1f;
    private float _minSpeed = 1f;
    private float _minIdleTime = 1f;

    [SerializeField] private float _randomness;
    [SerializeField] private float _radius;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxAcceleration;
    [SerializeField] private float _maxIdleTime;


    public float Randomness
    {
        get { return (_randomness >= 0f ? _randomness : 0f) / 100; }
        set { _randomness = value; }
    }

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        StartCoroutine(Live(_minIdleTime));
    }
    private IEnumerator Live(float time)
    {
        Debug.Log("Waiting for : " + time + " seconds.");
        yield return new WaitForSeconds(time);
        
        Vector3 randomDirection = Random.insideUnitSphere * _radius + transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _radius, 1);
        Vector3 finalPosition = hit.position;

        SetMovementProperties(finalPosition);

        float interpolation = GetNoise();
        float idleTime = Mathf.Lerp(_minIdleTime, _maxIdleTime, interpolation);
        StartCoroutine(Live(idleTime));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
    private void SetMovementProperties(Vector3 finalPosition)
    {
        float interpolation = GetNoise();
        _agent.speed = Mathf.Lerp(_minSpeed, _maxSpeed, interpolation);
        _agent.acceleration = Mathf.Lerp(_minAcceleration, _maxAcceleration, interpolation);
        _agent.destination = finalPosition;
    }

    private float GetNoise()
    {
        // get noise from random coordinates of the noise field
        float x = Random.Range(0f, Randomness);
        float y = Random.Range(0f, Randomness);
        float noise = Mathf.PerlinNoise(x, y);
        // perlin noise values may be slightly below or above 0-1 interval
        if (noise < 0) noise = 0;
        else if (noise > 1) noise = 1;

        return noise;
    }
}
