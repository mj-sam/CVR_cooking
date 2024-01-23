using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakedPizzaVisual : MonoBehaviour
{
    // Public field to hold the material for the baked version of the pizza. 
    public Material bakedMaterial;
    // When the pizza is for long enough in the oven, all the materials of the pizza will change to their baked material counterpart
    public void Bake(){
        // Access the MeshRenderer component of the GameObject this script is attached to 
        // and set its material to the bakedMaterial.
        GetComponent<MeshRenderer>().material = bakedMaterial;
    }
}
