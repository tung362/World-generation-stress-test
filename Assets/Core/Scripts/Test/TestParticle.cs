using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticle : MonoBehaviour
{
    public ParticleSystem ps;
    public float ShapeRadius = 0.1f;

	void Update ()
    {
        var shape = ps.shape;
        shape.radius = ShapeRadius;
    }
}
