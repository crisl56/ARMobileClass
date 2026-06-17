using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacedObject : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private float highlightStrength = 0.5f;

    private Renderer[] _renderers;
    private Color[] _originalColors;
    private bool _isSelected;

    private void Awake()
    {
        CacheRenders();
    }

    private void CacheRenders()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].material.color;
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null) continue;
            _renderers[i].material.color =
                _isSelected ? Color.Lerp(_originalColors[i], highlightColor, highlightStrength)
                    : _originalColors[i];
        }
    }

    public void SetMaterial(Material material)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = material;
            _originalColors[i] = material.color;
        }

        if (_isSelected)
            SetSelected(true);
    }
}
