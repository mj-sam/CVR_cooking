using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class LocalHandPresence : MonoBehaviour
{
    [SerializeField] private InputDeviceCharacteristics controllerCharacteristics;
    [SerializeField] private GameObject handModelPrefab;

    private InputDevice _targetDevice;
    private GameObject _spawnedController, _spawnedHandModel;
    private Animator _handAnimator;

    private void Start()
    {
        TryInitialize();
    }

    private void TryInitialize()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            _targetDevice = devices[0];
            _spawnedHandModel = Instantiate(handModelPrefab, transform);
            _handAnimator = _spawnedHandModel.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (!_targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            if (_spawnedHandModel)
            {
                _spawnedHandModel.SetActive(true);
            }

            if (_spawnedController)
            {
                _spawnedController.SetActive(false);
            }

            UpdateHandAnimation();
        }
    }

    private static readonly int Trigger = Animator.StringToHash("Trigger");
    private static readonly int Grip = Animator.StringToHash("Grip");

    private void UpdateHandAnimation()
    {
        if (_targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            _handAnimator.SetFloat(Trigger, triggerValue);
        }
        else
        {
            _handAnimator.SetFloat(Trigger, 0);
        }

        if (_targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            _handAnimator.SetFloat(Grip, gripValue);
        }
        else
        {
            _handAnimator.SetFloat(Grip, 0);
        }
    }
}
