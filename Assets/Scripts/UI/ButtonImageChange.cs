using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ButtonImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField]
    private Sprite _hoverSprite;
    [SerializeField]
    private Sprite _pressedSprite;
    [SerializeField]
    private Sprite _disabledSprite;

    private Sprite _defaultSprite;
    private Image _image;
    private Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _defaultSprite = _image.sprite;
    }

    private void OnMouseEnter()
    {
        if (_hoverSprite != null)
        {
            _image.sprite = _hoverSprite;
        }
    }

    private void OnMouseExit()
    {
        if (_defaultSprite != null)
        {
            _image.sprite = _defaultSprite;
        }
    }

    private void OnMouseDown()
    {
        if (_pressedSprite != null)
        {
            _image.sprite = _pressedSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }

    private void OnMouseUp()
    {
        if (_hoverSprite != null)
        {
            _image.sprite = _hoverSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }

    private void OnDisable()
    {
        if (_disabledSprite != null)
        {
            _image.sprite = _disabledSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverSprite != null)
        {
            _image.sprite = _hoverSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_defaultSprite != null)
        {
            _image.sprite = _defaultSprite;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pressedSprite != null)
        {
            _image.sprite = _pressedSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_hoverSprite != null)
        {
            _image.sprite = _hoverSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }
}