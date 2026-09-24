using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private Camera _mainCamera;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _transform.position = mousePosition;
    }
}
