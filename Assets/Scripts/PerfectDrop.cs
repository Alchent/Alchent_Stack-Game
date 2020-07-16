using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class PerfectDrop : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float speed = 4.0f;
    public float counter = -1.0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(counter > 0)
        {
            counter -= Time.deltaTime * speed;
            lineRenderer.sharedMaterial.SetColor("_BaseColor", new Color(1, 1, 1, counter));
        }
    }

    public void SetPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        lineRenderer.SetPosition(0, p0);
        lineRenderer.SetPosition(1, p1);
        lineRenderer.SetPosition(2, p2);
        lineRenderer.SetPosition(3, p3);
    }

    public void StartAnim()
    {
        counter = 1.0f;
    }


}
