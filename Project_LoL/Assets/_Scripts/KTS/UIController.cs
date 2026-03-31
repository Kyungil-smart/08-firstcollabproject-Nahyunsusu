using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RawImage _rawImage;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private Texture2D _png;
    
    private void Awake()
    {
        _rawImage.texture = _png;
    }
}



