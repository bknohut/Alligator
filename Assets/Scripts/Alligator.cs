using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AlligatorState
{
    idle = 0,
    moving = 1
}

public class Alligator : MonoBehaviour
{   
 #region Variables

    private NavMeshAgent _agent;
    private Animator _animator;

    [SerializeField] private Avatar _idleAvatar;
    [SerializeField] private Avatar _walkAvatar;

    private float _minAcceleration = 1f;
    private float _minSpeed = 1f;
    private float _minIdleTime = 1f;

    private AlligatorState _currentState;

    [SerializeField] private float _randomness;
    [SerializeField] private float _radius;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxAcceleration;
    [SerializeField] private float _maxIdleTime;

    private Coroutine _coroutine;

    public float Randomness
    {
        get { return (_randomness >= 0f ? _randomness : 0f) / 100; }
        set { _randomness = value; }
    }

 #endregion
 #region Methods

    private void Awake()
    {
        _agent    = gameObject.GetComponent<NavMeshAgent>();
        _animator = gameObject.GetComponent<Animator>();
        _currentState = AlligatorState.idle;

        StartCoroutine(Live());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private IEnumerator Live()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _currentState = (AlligatorState)Random.Range(0, 2);

        if (_currentState == AlligatorState.idle)
        {
            _coroutine = StartCoroutine(StayIdle());
        }
        else if (_currentState == AlligatorState.moving)
        {
            _coroutine = StartCoroutine(Move(true));
        }
        yield return null;
    }
    private IEnumerator StayIdle()
    {
        _animator.avatar = _idleAvatar;
        _animator.SetBool("IsIdle", true);

        float interpolation = GetNoise();
        float idleTime = Mathf.Lerp(_minIdleTime, _maxIdleTime, interpolation);
        Debug.Log("I AM IDLE FOR " + idleTime + " BITCH");
        yield return new WaitForSeconds(idleTime);

        StartCoroutine(Live());
    }
    // must be rechecked
    private IEnumerator Move(bool isMovementNew = false)
    {
        if (isMovementNew)
        {
            Debug.Log("I AM MOOving");

            Vector3 randomDirection = Random.insideUnitSphere * _radius + transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, _radius, 1);
            Vector3 finalPosition = hit.position;

            _animator.avatar = _walkAvatar;
            _animator.SetBool("IsIdle", false);
            SetMovementProperties(finalPosition);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
            _coroutine = StartCoroutine(Move());
        }

        else if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                StartCoroutine(Live());
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
                _coroutine = StartCoroutine(Move());
            }
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
            _coroutine = StartCoroutine(Move());
        }

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

#endregion
}
