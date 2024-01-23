using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoDisplay : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048,2048);
    public Material sauceTexture;
    private Vector3 origLocalScale;
    public bool drawable = false;

    // The doughball prefab has a plane with the texture of sauce
    // at creation all pixels of this plane are set to be transparent
    // an object with tomatodrawer script can make these pixels become not transparent
    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = Instantiate(sauceTexture.GetTexture("_MainTex") as Texture2D);
        
        Color[] _colors = texture.GetPixels(0,0, (int)textureSize.x, (int)textureSize.y);
        for (int i = 0; i < _colors.Length; i++){
            _colors[i].a = 0;
        }
        texture.SetPixels(0,0, (int)textureSize.x, (int)textureSize.y, _colors);
        texture.Apply();
        r.material.mainTexture = texture;
        origLocalScale = this.transform.localScale;
    }
}
