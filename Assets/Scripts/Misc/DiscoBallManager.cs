using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{
    public static Action OnDiscoBallHitEvent;

    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = .2f;
    [SerializeField] private Light2D _globalLight;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotlights;
    private float _defaultGlobalLightIntensity;

    private void Awake()
    {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }

    private void Start()
    {
        _allSpotlights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }
    private void OnEnable()
    {
        OnDiscoBallHitEvent += DimTheLights;
    }

    private void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLights;
        
    }

    public void DiscoBallParty()
    {   
        if (_discoCoroutine != null)
            return;
        
        OnDiscoBallHitEvent?.Invoke();
    }

    private void DimTheLights()
    {
        foreach (ColorSpotlight colorSpotlight in _allSpotlights)
        {
            StartCoroutine(colorSpotlight.SpotlightDiscoPartyRoutine(_discoBallPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine()
    {
        _globalLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null; 
    }
}
