using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum AlligatorState
{
    idle = 0,
    moving = 1
}
public class Alligator : MonoBehaviour
{
 #region Variables

    // Alligator states' execution functions
    private delegate IEnumerator coroutine();
    private coroutine[] coroutines;
    // Components
    private NavMeshAgent _agent;
    private Animator _animator;

    [Header("Animation")]
    [SerializeField] private Avatar _idleAvatar;
    [SerializeField] private Avatar _walkAvatar;

    [Header("Movement")]
    [Range(0,100)][SerializeField] private float _randomness;
    [SerializeField] private float _radius;

    [Header("Maxiumum Values")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxAcceleration;
    [SerializeField] private float _maxIdleTime;
    [SerializeField] private float _maxRotationTime;

    private float _minSpeed = 1f;
    private float _minAcceleration = 1f;
    private float _minIdleTime = 1f;
    private float _minRotationTime = 1f;

 #endregion
 #region Methods

    private void Awake()
    {
        // All the new states MUST be added to the array.
        // Adding them on the same order with the AlligatorState enum is also a better option.
        coroutines = new coroutine[] { StayIdle, Move };

        _agent = gameObject.GetComponent<NavMeshAgent>();
        _animator = gameObject.GetComponent<Animator>();
    }
    // Starting execution on OnEnable as coroutines stop on OnDisable, allowing to continue to the lifecycle on disable-enable.
    private void OnEnable()
    {
        StartCoroutine(Live());
    }
    // For debug purposes, shows a sphere around the transform.
    // The gameobject chooses a point inside this sphere.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private IEnumerator Live()
    {
        while (true)
        {   
            // Get a random state to execute.
            // It randomly executes a state in the coroutines array.
            yield return StartCoroutine(coroutines[Random.Range(0, System.Enum.GetNames(typeof(AlligatorState)).Length)]());
        }
    }
    private IEnumerator StayIdle()
    {
        // set the idle animation properties
        _animator.avatar = _idleAvatar;
        _animator.SetBool("IsIdle", true);

        // get a random wait time to stay idle
        float interpolation = GetNoise();
        float idleTime = Mathf.Lerp(_minIdleTime, _maxIdleTime, interpolation);
        yield return new WaitForSeconds(idleTime);
    }
    private IEnumerator Move()
    {   
        // get a random point in a sphere with radius = _radius around the transform
        Vector3 randomDirection = Random.insideUnitSphere * _radius + transform.position;
        // sample a navmesh position using the generated random point
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _radius, 1);
        Vector3 finalPosition = hit.position;

        // set the walk animation properties
        _animator.avatar = _walkAvatar;
        _animator.SetBool("IsIdle", false);
        
        // turn to the destination point before moving 
        float tmpSpeed = _agent.speed;
        _agent.speed = 0f;
        yield return StartCoroutine(Turn(finalPosition));
        _agent.speed = tmpSpeed;
        // change animation speed with respect to the movement speed
        _animator.SetFloat("Speed", tmpSpeed);
        // move to the sampled point
        SetMovementProperties(finalPosition);
        while (true)
        {   
            // if the destination is reached, become idle.
            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _animator.avatar = _idleAvatar;
                    _animator.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator Turn(Vector3 finalPosition)
    {   
        // rotation time is randomized between its min and max values.
        float interpolation = GetNoise();
        float rotationTime = Mathf.Lerp(_minRotationTime, _maxRotationTime, interpolation);
        // set the animation speed
        // longer rotation time means that the speed  is slower.
        _animator.SetFloat("Speed", Mathf.Lerp(_minSpeed, _maxSpeed, 1-interpolation));

        transform.DOLookAt(finalPosition, rotationTime, AxisConstraint.Y, transform.up);
        yield return new WaitForSeconds(rotationTime);
        yield break;
    }
    
    // set navmesh movement properties
    private void SetMovementProperties(Vector3 finalPosition)
    {
        // get a random speed value between min and max
        float interpolation = GetNoise();
        _agent.speed = Mathf.Lerp(_minSpeed, _maxSpeed, interpolation);
        // get a random acceleration value between min and max
        interpolation = GetNoise();
        _agent.acceleration = Mathf.Lerp(_minAcceleration, _maxAcceleration, interpolation);
        // set the destination
        _agent.destination = finalPosition;
    }
    // returns perlin noise
    private float GetNoise()
    {
        // get noise from random coordinates of the noise field
        float x = Random.Range(0f, _randomness * _randomness);
        float y = Random.Range(0f, _randomness * _randomness);
        float noise = Mathf.PerlinNoise(x, y);
        // perlin noise values may be slightly below or above 0-1 interval
        if (noise < 0) noise = 0;
        else if (noise > 1) noise = 1;

        return noise;
    }
    
#endregion
}
