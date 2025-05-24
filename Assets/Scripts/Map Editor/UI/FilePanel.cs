using UI;
using UnityEngine;
using UnityEngine.Events;

namespace MapEditor.UI
{
    public abstract class FilePanel : Panel
    {
        //public event UnityAction<string> File;

        //[SerializeField]
        //protected Text _titleText;

        //[SerializeField]
        //protected ScrollRect _filesScrollView;

        //[SerializeField]
        //protected FileSlot _fileSlotPrefab;

        //[SerializeField]
        //protected AudioInputField _fileNameInputField;

        //[SerializeField]
        //protected Button _actionButton;

        //[SerializeField]
        //protected ActionPanel _actionPanel;

        //private FileSlot[] _fileSlots;

        protected override void OnInitialize()
        {
            //_actionButton.onClick.AddListener(() =>
            //{
            //    Selected();
            //});
        }

        protected void CreateSlots()
        {
            //if (_fileSlots != null)
            //    foreach (var fileSlot in _fileSlots)
            //        Destroy(fileSlot.gameObject);

            //var files = ResourceManager.Instance.GetFiles<MapData>();

            //_fileSlots = new FileSlot[files.Length];

            //for (int i = 0; i < files.Length; i++)
            //{
            //    var file = Instantiate(_fileSlotPrefab, _filesScrollView.content);
            //    file.Initialize(files[i]);
            //    file.Clicked += () =>
            //    {
            //        _fileNameInputField.Text = file.Text.text;
            //    };
            //    file.DoubleClicked += ()
            //        => Selected();

            //    _fileSlots[i] = file;
            //}
        }

        //protected abstract void Selected();

        //protected void OnFile()
        //    => File.Invoke(_fileNameInputField.Text);
    }
}