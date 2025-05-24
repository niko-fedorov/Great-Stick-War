using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MapEditor
{
    namespace UI
    {
        public class ColorSelectPanel : MonoBehaviour
        {
            public event UnityAction<Color> Selected;

            public Color Color { get; private set; }

            [SerializeField]
            private Image _colorImage;
            [SerializeField]
            private InputField _hexadecimalInputField,
                               _redColorInputField,
                               _greenColorInputField,
                               _blueColorInputField;

            [SerializeField]
            private ColorSelector _colorSelector;

            [SerializeField]
            private Slider _toneSlider;

            [SerializeField]
            private Button _eyedropperButton;

            private Texture2D _toneTexture;

            private void Start()
            {
                var colors = new Color[]
                {
            new Color(1, 0, 0),
            new Color(1, 1, 0),
            new Color(0, 1, 0),
            new Color(0, 1, 1),
            new Color(0, 0, 1),
            new Color(1, 0, 1),
                };

                _toneTexture = new Texture2D(1, 240);
                int colorRange = _toneTexture.height / colors.Length;
                for (int y = 0; y < _toneTexture.height; y++)
                    _toneTexture.SetPixel(1, _toneTexture.height - y - 1,
                        Color.Lerp(colors[y / colorRange], colors[(y / colorRange + 1) % colors.Length], y % colorRange / (float)colorRange));

                _toneTexture.Apply();
                _toneSlider.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(_toneTexture, new Rect(0, 0, _toneTexture.width, _toneTexture.height), Vector2.one / 2);

                _toneSlider.onValueChanged.AddListener(value =>
                    _colorSelector.UpdateTexture(_toneTexture.GetPixel(1, (int)((1 - _toneSlider.value) * (_toneTexture.height - 1)))));

                _hexadecimalInputField.onSubmit.AddListener(value =>
                {
                    if (ColorUtility.TryParseHtmlString(value, out Color color))
                        _colorSelector.UpdateTexture(color);
                });

                _redColorInputField.onSubmit.AddListener(value =>
                    _colorSelector.UpdateTexture(new Color(int.Parse(value), Color.b, Color.g)));

                _blueColorInputField.onSubmit.AddListener(value =>
                    _colorSelector.UpdateTexture(new Color(Color.r, int.Parse(value), Color.g)));

                _greenColorInputField.onSubmit.AddListener(value =>
                    _colorSelector.UpdateTexture(new Color(Color.r, Color.b, int.Parse(value))));

                _eyedropperButton.onClick.AddListener(() =>
                {
                    IEnumerator PickColor(UnityAction<Color> picked)
                    {
                        while (!Input.GetMouseButtonDown(0))
                            yield return null;

                        yield return new WaitForEndOfFrame();

                        var texture = new Texture2D(Screen.width, Screen.height);
                        texture.ReadPixels(Camera.main.pixelRect, 0, 0);

                        picked.Invoke(texture.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y));
                    }

                    StartCoroutine(PickColor(color => _colorSelector.UpdateTexture(color)));
                });

                _colorSelector.Selected += (color) =>
                {
                    Color = _colorImage.color = color;
                    _redColorInputField.text = color.r.ToString();
                    _greenColorInputField.text = color.g.ToString();
                    _blueColorInputField.text = color.b.ToString();
                    _hexadecimalInputField.text = ColorUtility.ToHtmlStringRGB(color);

                    Selected.Invoke(color);
                };
            }
        }
    }
}
