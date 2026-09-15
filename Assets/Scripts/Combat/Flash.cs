using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _whiteFlashMaterial;
    [SerializeField] private float _flashTime = 0.1f;

    private SpriteRenderer[] _spriterenders;


    private void Awake()
    {
        _spriterenders = GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        foreach (SpriteRenderer sr in _spriterenders)
        {
            sr.material = _whiteFlashMaterial;
            sr.color = Color.white;
        }
        yield return new WaitForSeconds(_flashTime);

        SetDefaultMaterial();
    }

    private void SetDefaultMaterial()
    {
        foreach (SpriteRenderer sr in _spriterenders)
        {
            sr.material = _defaultMaterial;
        }
    }
}
