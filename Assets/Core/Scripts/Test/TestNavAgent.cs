using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavAgent : MonoBehaviour
{
    public Camera Cam;
    public bool DrawNonAgentPath = false;
    public LineRenderer NonAgentLineRenderer;
    private NavMeshPath Path;
    private NavMeshAgent TheNavMeshAgent;
    private LineRenderer TheLineRenderer;

    void Start ()
    {
        TheNavMeshAgent = GetComponent<NavMeshAgent>();
        TheLineRenderer = GetComponent<LineRenderer>();
        Path = new NavMeshPath();
    }
	
	void Update ()
    {
        Ray mouseRay = Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity))
        {
            if (Input.GetMouseButton(0))
            {
                TheNavMeshAgent.SetDestination(mouseHit.point);

                if (DrawNonAgentPath)
                {
                    //Non agent path
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(mouseHit.point, out hit, mouseHit.distance, NavMesh.AllAreas))
                    {
                        NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, Path);
                        NonAgentLineRenderer.positionCount = Path.corners.Length;
                        for (int i = 0; i < Path.corners.Length; i++)
                        {
                            NonAgentLineRenderer.SetPosition(i, new Vector3(Path.corners[i].x, Path.corners[i].y + 0.1f, Path.corners[i].z));
                        }
                    }
                }
            }
        }

        //Agent path
        TheLineRenderer.positionCount = TheNavMeshAgent.path.corners.Length;
        for (int i = 0; i < TheNavMeshAgent.path.corners.Length; i++)
        {
            TheLineRenderer.SetPosition(i, new Vector3(TheNavMeshAgent.path.corners[i].x, TheNavMeshAgent.path.corners[i].y + 0.1f, TheNavMeshAgent.path.corners[i].z));
        }

        //DrawClosestEdge();
    }

    void DrawClosestEdge()
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            DrawCircle(transform.position, hit.distance, Color.red);
            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        }
    }

    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }
}
