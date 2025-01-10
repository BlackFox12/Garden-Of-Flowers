using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class AiAutoPath : MonoBehaviour
{
    public Vector3 target;
    public GameObject Navmesh;
    public NavMeshAgent agent;
    public NavMeshSurface surface2D;

    private bool navMeshNeedsUpdate;

    void Start()
    {
        surface2D = Navmesh.GetComponent<NavMeshSurface>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdateNavMeshWithAgentSettings();
        navMeshNeedsUpdate = false;
    }

    void Update()
    {
        // Update the NavMesh dynamically when requested
        if (navMeshNeedsUpdate)
        {
            UpdateNavMeshWithAgentSettings();
            navMeshNeedsUpdate = false;
        }

        // Set the agent's destination if a target exists
        if (target != null)
        {
            agent.SetDestination(target);
        }
    }

    public void SetTargetVector(Vector3 newTargetVector)
    {
        target = newTargetVector;
    }

    public void RequestNavMeshUpdate()
    {
        navMeshNeedsUpdate = true;
    }

    private void UpdateNavMeshWithAgentSettings()
    {
        // Get the current build settings from the NavMeshSurface
        NavMeshBuildSettings buildSettings = surface2D.GetBuildSettings();

        // Adjust the agent radius to match the NavMeshAgent
        buildSettings.agentRadius = agent.radius;

        // Update the NavMesh with the modified settings
        surface2D.UpdateNavMesh(surface2D.navMeshData);
    }
}
