using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

// Spoon with this script can make the sauce plane of the pizza be non transparent
public class NetworkTomatoDrawer : NetworkBehaviour
{
    // delayed drawing update     
    //private List<Vector2> pendingPixelChanges = new List<Vector2>();
    private List<int> pendingPixelChangesX = new List<int>();
    private List<int> pendingPixelChangesY = new List<int>();
    private float updateInterval = 0.5f; // 500 milliseconds
    private float lastUpdateTime = 0f;

    // Variables to be synced accross the network
    private NetworkVariable<bool> _level1 = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone); 
    private NetworkVariable<bool> _level2= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> _level3= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> _level4= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    //private NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone);
    
    public NetworkVariable<int> pixelsDrawn = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone); // Drawn pixel each time spoon is filled
    public NetworkVariable<int> totalPixelsDrawn = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone); // Total Drawn Pixed
    
    // ===
    public GameObject _tip;
    private int _spoonSize = 20;
    private bool pickedUp = false;
    private Color[] _colors;
    private RaycastHit _touch;
    public TomatoDisplay _pizzaDisplay;// _pizzaDisplay
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    public GameObject spoonSauce1;
    public GameObject spoonSauce2;
    public GameObject spoonSauce3;
    public GameObject spoonSauce4;
    private int pixelStep = 11000;
    
    
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;

    private void Start()
    {
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
    }
    
    //----------------------------------------------------------------------------
    //================================= Network RPCs =============================
    //----------------------------------------------------------------------------
    [ServerRpc(RequireOwnership = false)]
    private void changeTomatoSauceLevelServerRpc(bool reset = false)
    {
        // Spoon is filled to a certain level, this level also plays a roll in how many pixels get set
        // to non transparent in the neighbourhood of the spoon :: spoon size
        if (reset)
        {
            //Debug.Log("Change Tomato level is called Rsert");
            //_taskProgression.SetHapticVibration(0.35f,0.35f);
            _level1.Value = true;
            _level2.Value = true;
            _level3.Value = true;
            _level4.Value = true;
            _spoonSize = 50;
            setSauceLevelClientRpc(_spoonSize);
        }
        else
        {
            //Debug.Log("Change Tomato level is called NORMAL");
            
            if (pixelsDrawn.Value > pixelStep && pixelsDrawn.Value < 2 * pixelStep)
            {
                _level1.Value = false;
                _level2.Value = true;
                _level3.Value = true;
                _level4.Value = true;
                _spoonSize = 45;
                setSauceLevelClientRpc(_spoonSize);
            }
            else if (pixelsDrawn.Value > 2 * pixelStep && pixelsDrawn.Value < 3 * pixelStep)
            {
                _level1.Value = false;
                _level2.Value = false;
                _level3.Value = true;
                _level4.Value = true;
                _spoonSize = 40;
                setSauceLevelClientRpc(_spoonSize);
            }
            else if (pixelsDrawn.Value > 3 * pixelStep && pixelsDrawn.Value < 4 * pixelStep)
            {
                _level1.Value = false;
                _level2.Value = false;
                _level3.Value = false;
                _level4.Value = true;
                _spoonSize = 35;
                setSauceLevelClientRpc(_spoonSize);
            }
            else if (pixelsDrawn.Value > 4 * pixelStep)
            {
                _level1.Value = false;
                _level2.Value = false;
                _level3.Value = false;
                _level4.Value = false;
                _spoonSize = 20;
                setSauceLevelClientRpc(_spoonSize);
            }
            spoonSauce1.SetActive(_level1.Value);
            spoonSauce2.SetActive(_level2.Value);
            spoonSauce3.SetActive(_level3.Value);
            spoonSauce4.SetActive(_level4.Value);
        }
    }

    [ClientRpc]
    public void setSauceLevelClientRpc(int spoonSize)
    {
        //Debug.Log("Change Tomato level is called Client");
        spoonSauce1.SetActive(_level1.Value);
        spoonSauce2.SetActive(_level2.Value);
        spoonSauce3.SetActive(_level3.Value);
        spoonSauce4.SetActive(_level4.Value);
        _spoonSize = spoonSize;
    }

    //----------------------- Tomato pixel drawing ------------------------------
    [ServerRpc(RequireOwnership = false)]
    void SetTomatoPixelsServerRpc(int[] pendinChangesX, int[] pendinChangesY)
    {
        for (int i = 0; i < pendinChangesX.Length; i++)
        {
            int x = pendinChangesX[i];
            int y = pendinChangesY[i];
            // Do something with 'vector'
            SetTomatoPixels( x, y);
            ApplyPixels();
        }
        
        SetTomatoPixelsClientRpc(pendinChangesX, pendinChangesY); // Notify clients
    }
    // void SetTomatoPixelsServerRpc(int x, int y)
    // {
    //     SetTomatoPixels(x, y);
    //     ApplyPixels();
    //     SetTomatoPixelsClientRpc(x, y); // Notify clients
    // }

    [ClientRpc]
    void SetTomatoPixelsClientRpc(int[] pendinChangesX, int[] pendinChangesY)
    {
        for (int i = 0; i < pendinChangesX.Length; i++)
        {
            int x = pendinChangesX[i];
            int y = pendinChangesY[i];
            // Do something with 'vector'
            SetTomatoPixels(x, y);
            ApplyPixels();
        }
        //SetTomatoPixels(x, y);
        //ApplyPixels();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPixelDrawnServerRpc(int pixelValue)
    {
        pixelsDrawn.Value = pixelValue;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void setTotalPixelDrawnServerRpc(int pixelValue)
    {
        totalPixelsDrawn.Value = pixelValue;
        //Debug.Log("TOTAL : ");
        //Debug.Log(totalPixelsDrawn.Value);
    }
    //============================================================================
     
    // Refill the spoon with sauce once it enter the bowl
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TomatoSauce"))
        {
            changeTomatoSauceLevelServerRpc(true);
            SetPixelDrawnServerRpc(0);
            //pixelsDrawn.Value = 0;
        }
    }

    
    public void PickUp()
    {
        //Debug.Log("PICKED UP");
        pickedUp = true;
    }

    public void LetGo()
    {
        pickedUp = false;
    }

    void Update()
    {
        if(IsOwner){
            if (!_networkTaskProgression.sausagePizzaDone.Value)
            {
                Draw();
            }
            if (pendingPixelChangesX.Count > 20)
            {
                SetTomatoPixelsServerRpc( pendingPixelChangesX.ToArray(), pendingPixelChangesY.ToArray());
                //SendBatchedPixelUpdates();
                pendingPixelChangesX.Clear();
                pendingPixelChangesY.Clear();
            }
        }
        
    }
    
    void AccumulatePixelChange(int x, int y)
    {
        pendingPixelChangesX.Add(x);
        pendingPixelChangesY.Add(y);
        //pendingPixelChanges.Add(new Vector2(x, y));
    }

    private void Draw()
    {
        int mask = Physics.AllLayers;
        mask &= ~11;

        // When enough pixels are made non transparent, the pizza is ready for toppings
        if (totalPixelsDrawn.Value > 10000)
        {
            //totalPixelsDrawn.Value = 0;
            //_taskProgression.tomatoSpread = true;
            //_taskProgression.UpdateInstruction();
            
            _networkTaskProgression.tomatoSpreadServerRpc(true);
            _networkTaskProgression.UpdateInstruction();
            
        }

        // Raycast out of the top of the spoon and make it only collide with the tomato sauce plane
        //_taskProgression.doughSpread && 
        if (Physics.Raycast(_tip.transform.position, -transform.up, out _touch, 100000, mask))
        {
            //Debug.Log("Tomato RayCast");
            // if the spoon is close enough to the tomato sauce plane
            //Debug.Log(_touch.transform.tag);
            //Debug.Log(Mathf.Abs(_touch.transform.position.y - _tip.transform.position.y));
            if (_touch.transform.CompareTag("TomatoSaucePizza") &&
                Mathf.Abs(_touch.transform.position.y - _tip.transform.position.y) < 0.035f)
            {
                //Debug.Log("Drawable : ");
                //Debug.Log(_touch.transform.gameObject.GetComponent<TomatoDisplay>().drawable);
                //if (_touch.transform.gameObject.GetComponent<TomatoDisplay>().drawable == false) return;
                //TODO : check for drawble value application
                //Debug.Log("PICKED UP");
                //Debug.Log(pickedUp);
                //if (!pickedUp) return;
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                int x = (int)(_touchPos.x * _pizzaDisplay.textureSize.x - (_spoonSize / 2));
                int y = (int)(_touchPos.y * _pizzaDisplay.textureSize.y - (_spoonSize / 2));
                //if out of drawable area
                if (y < 0 || y > _pizzaDisplay.textureSize.y || x < 0 || x > _pizzaDisplay.textureSize.x) return;
                //_touchedLastFrame && 
                //Debug.Log("ACTIVE : ");
                //Debug.Log(spoonSauce4.activeSelf);
                if (spoonSauce4.activeSelf)
                {
                    float dist = new Vector2(x - _pizzaDisplay.textureSize.x / 2 + (_spoonSize / 2),
                        y - _pizzaDisplay.textureSize.y / 2 + (_spoonSize / 2)).magnitude;
                    // Return if trying to draw outside the drawable area
                    if (dist > _pizzaDisplay.textureSize.y / 2.8) return;
                    // otherwise set the pixels in the neighbourhood of the spoon to non transparent
                    //SetTomatoPixelsServerRpc(x, y);  //_taskProgression.SetHapticVibration(0.1f,0.1f);
                    AccumulatePixelChange(x, y);
                    //Debug.Log("Tomato pixel reutrned");
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                //Debug.Log("Change tomato sause level called");
                changeTomatoSauceLevelServerRpc(false);
                return;
            }
        }

        _touchedLastFrame = false;
    }

    private void SetTomatoPixels(int x, int y)
    {
        try
        {
            _colors = _pizzaDisplay.texture.GetPixels(x, y, _spoonSize, _spoonSize);
        }
        catch (Exception e)
        {
            Debug.Log("Check get pixel");
        }
        
        for (int i = 0; i < _colors.Length; i++)
        {
            if (_colors[i].a == 0)
            {
                _colors[i].a = 1;
                if (IsServer || IsHost)
                {
                    SetPixelDrawnServerRpc(pixelsDrawn.Value+ 1);
                    setTotalPixelDrawnServerRpc(totalPixelsDrawn.Value + 1);
                }
            }
        }
        try
        {
            _pizzaDisplay.texture.SetPixels(x, y, _spoonSize, _spoonSize, _colors);
        }
        catch (Exception e)
        {
            Debug.Log("Check set pixel");
        }
    }

    private void ApplyPixels()
    {
        _pizzaDisplay.texture.Apply();
    }

    
}