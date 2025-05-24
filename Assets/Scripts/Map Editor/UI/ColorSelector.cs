using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    namespace UI
    {
        public class ColorSelector : MonoBehaviour, IPointerDownHandler, IDragHandler
        {
            public event UnityAction<Color32> Selected;

            [SerializeField]
            private Image _handleImage;

            private Texture2D _texture;

            private Image _image;

            private void Start()
            {
                _image = GetComponent<Image>();

                _texture = new Texture2D(256, 256);
                _image.sprite = Sprite.Create(
                    _texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.up);

                UpdateTexture(Color.red);
            }

            private void SelectColor(Vector2 point)
            {
                _handleImage.transform.position = new Vector3(
                Mathf.Clamp(point.x, transform.position.x, transform.position.x + _image.rectTransform.sizeDelta.x),
                Mathf.Clamp(point.y, transform.position.y, transform.position.y + _image.rectTransform.sizeDelta.y));

                Selected.Invoke(_texture.GetPixel(
                    (int)(_handleImage.transform.localPosition.x / _image.rectTransform.sizeDelta.x * (_texture.width - 1)),
                    (int)(_handleImage.transform.localPosition.y / _image.rectTransform.sizeDelta.y * (_texture.height - 1))));
            }
            public void OnPointerDown(PointerEventData eventData)
                => SelectColor(eventData.position);
            public void OnDrag(PointerEventData eventData)
                => SelectColor(eventData.position);

            public void UpdateTexture(string hexadecimal)
            {
                //if(ColorUtility.TryParseHtmlString)
            }
            public void UpdateTexture(Color tone)
            {
                var colors = new Color[_texture.width * _texture.height];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] =
                    Color.Lerp(Color.white, tone, (float)i % _texture.height / (_texture.width - 1)) *
                    Color.Lerp(Color.black, Color.white, (float)i / _texture.height / (_texture.height - 1));

                _texture.SetPixels(colors.Cast<Color>().ToArray());
                _texture.Apply();

                Selected.Invoke(_texture.GetPixel(
                    (int)(_handleImage.transform.localPosition.x / _image.rectTransform.sizeDelta.x * (_texture.width - 1)),
                    +(int)(_handleImage.transform.localPosition.y / _image.rectTransform.sizeDelta.y * (_texture.height - 1))));
            }
        }
    }
}