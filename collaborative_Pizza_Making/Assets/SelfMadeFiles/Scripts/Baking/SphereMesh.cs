using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to store the basic unity sphere mesh before altering it by spreading the dough
public class SphereMesh : MonoBehaviour
{
    public Mesh originalMesh;
    public bool first = true;
    public int number = 0;
}