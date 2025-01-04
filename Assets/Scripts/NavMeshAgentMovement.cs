using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class NavMeshAgentMovement : MonoBehaviour
{
    public Vector3 target;
    public GameObject Navmesh;
    private NavMeshAgent agent;
    private NavMeshSurface surface2D;
    // Start is called before the first frame update
    void Start()
    {
        surface2D = Navmesh.GetComponent<NavMeshSurface>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        surface2D.UpdateNavMesh(surface2D.navMeshData);
        if (target == null) return;
        agent.SetDestination(target);
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget.position == target) return;
        target = newTarget.position;
    }
    public void SetTargetVector(Vector3 newTargetVector)
    {
        target = newTargetVector;
    }
}
