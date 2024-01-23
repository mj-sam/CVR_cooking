using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Spoon with this script can make the sauce plane of the pizza be non transparent
public class TomatoDrawer : MonoBehaviour
{
    [SerializeField] private GameObject _tip;
    private int _spoonSize = 50;
    private Color[] _colors;
    private RaycastHit _touch;
    public TomatoDisplay _pizza;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    public GameObject spoonSauce1;
    public GameObject spoonSauce2;
    public GameObject spoonSauce3;
    public GameObject spoonSauce4;
    private bool pickedUp = false;
    private int pixelsDrawn = 0;
    private int totalPixelsDrawn = 0;
    private int pixelStep = 11000;
    private TaskProgression _taskProgression;

    private void Start(){
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
    }

    // Refill the spoon with sauce
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TomatoSauce"))
        {
            changeTomatoSauceLevel(true);
            pixelsDrawn = 0;
        }
    }

    // Spoon is filled to a certain level, this level also plays a roll in how many pixels get set to non transparent in the neighbourhood of the spoon
    private void changeTomatoSauceLevel(bool reset = false){
        if (reset){
            //_taskProgression.SetHapticVibration(0.35f,0.35f);
            //BroadcastRemoteMethod("setSauceLevel", true, true, true, true, 50);
            setSauceLevel(true, true, true, true, 50);
        }
        else{
            if (pixelsDrawn > pixelStep && pixelsDrawn < 2*pixelStep){
                //BroadcastRemoteMethod("setSauceLevel", false, true, true, true, 46);
                setSauceLevel(false, true, true, true, 46);
            } 
            else if (pixelsDrawn > 2*pixelStep && pixelsDrawn < 3*pixelStep){
                //BroadcastRemoteMethod("setSauceLevel", false, false, true, true, 42);
                setSauceLevel(false, false, true, true, 42);
            }
            else if (pixelsDrawn > 3*pixelStep && pixelsDrawn < 4*pixelStep){
                //BroadcastRemoteMethod("setSauceLevel", false, false, false, true, 38);
                setSauceLevel(false, false, false, true, 38);
            }
            else if (pixelsDrawn > 4*pixelStep)
            {
                //BroadcastRemoteMethod("setSauceLevel", false, false, false, false, 34);
                setSauceLevel( false, false, false, false, 34);
            }
        }
    }

    //[SynchronizableMethod]
    public void setSauceLevel(bool level1, bool level2, bool level3, bool level4, int spoonSize){
        spoonSauce1.SetActive(level1);
        spoonSauce2.SetActive(level2);
        spoonSauce3.SetActive(level3);
        spoonSauce4.SetActive(level4);
        _spoonSize = spoonSize;
    }

    public void PickUp(){
        pickedUp = true;
    }

    public void LetGo(){
        pickedUp = false;
    }

    void Update()
    {
        Draw();
    }


    private void Draw()
    {
        int mask = Physics.AllLayers;
        mask &= ~11;
        // When enough pixels are made non transparent, the pizza is ready for toppings
        if (totalPixelsDrawn > 121000){
            totalPixelsDrawn = 0;
            _taskProgression.tomatoSpread = true;
            _taskProgression.UpdateInstruction();
        }
        // Raycast out of the top of the spoon and make it only collide with the tomato sauce plane
        //_taskProgression.doughSpread && 
        if(Physics.Raycast(_tip.transform.position, -transform.up, out _touch, 100000, mask))
        {   
            
            // if the spoon is close enough to the tomato sauce plane
            if(_touch.transform.CompareTag("TomatoSaucePizza") && Mathf.Abs(_touch.transform.position.y - _tip.transform.position.y) < 0.035f)
            {
                
                if (_touch.transform.gameObject.GetComponent<TomatoDisplay>().drawable == false) return;
                if (!pickedUp) return;
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                int x = (int)(_touchPos.x * _pizza.textureSize.x - (_spoonSize / 2));
                int y = (int)(_touchPos.y * _pizza.textureSize.y - (_spoonSize / 2));
                //if out of drawable area
                if(y < 0 || y > _pizza.textureSize.y || x < 0 || x > _pizza.textureSize.x) return;
                
                //_touchedLastFrame && 
                if (spoonSauce4.activeSelf)
                {
                    float dist = new Vector2(x-_pizza.textureSize.x/2+ (_spoonSize / 2), y-_pizza.textureSize.y/2+ (_spoonSize / 2)).magnitude;
                    // Return if trying to draw outside the drawable area
                    if (dist>_pizza.textureSize.y/2.8) return;
                    // otherwise set the pixels in the neighbourhood of the spoon to non transparent
                    //BroadcastRemoteMethod("SetTomatoPixels", x, y);
                    SetTomatoPixels(x, y);
                    _taskProgression.SetHapticVibration(0.1f,0.1f);
                    
                    ApplyPixels();
                }
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                changeTomatoSauceLevel();
                return;
            }
        }
        _touchedLastFrame = false;
    }

    //[SynchronizableMethod]
    private void SetTomatoPixels(int x, int y){
        _colors = _pizza.texture.GetPixels(x, y, _spoonSize, _spoonSize);
        for (int i = 0; i < _colors.Length; i++){
            if (_colors[i].a == 0){
                _colors[i].a = 1;
                pixelsDrawn++;
                totalPixelsDrawn++;
            }
        }
        _pizza.texture.SetPixels(x, y, _spoonSize, _spoonSize, _colors);
    }

    //[SynchronizableMethod]
    private void ApplyPixels(){
        _pizza.texture.Apply();
    }
}
