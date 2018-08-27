using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Alligator : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField] private float _randomness;
    [SerializeField] private float _radius;

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        StartCoroutine(Live(1f));

    }
    private IEnumerator Live(float time)
    {
        Debug.Log("Waiting for : " + time + " seconds.");
        yield return new WaitForSeconds(time);

        Vector3 randomDirection = Random.insideUnitSphere * _radius;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _radius, 1);
        Vector3 finalPosition = hit.position;
        _agent.destination = finalPosition;

        StartCoroutine(Live(Random.Range(1f, _randomness < 1f ? 1f : _randomness)));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

}
