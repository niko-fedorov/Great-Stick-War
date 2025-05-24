using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource), typeof(AudioSource))]
public abstract class UIManager : Singleton<UIManager>
{
    #region Variables

    private Panel _panel;

    #region Components

    [SerializeField]
    protected AudioSource _musicAudioSource,
                          _soundAudioSource;

    protected AudioListener _audioListener;
    protected UIDocument _UIDocument;

    #endregion

    #endregion

    #region Methods

    //protected override void Initialize()
    //{
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.SetEnabled(true);

    //    _panels = new Stack<Panel>();
    //    Panel.Opened += panel => _panels.Push(panel);
    //}

    private void Start()
    {
        _audioListener = GetComponent<AudioListener>();
        _UIDocument = GetComponent<UIDocument>();

        _UIDocument.rootVisualElement.Query().ForEach(ve => (ve as IInitializeable)?.Initialize());

        Panel.Opened += panel => _panel = panel;

        OnStart();    
    }

    protected virtual void OnStart() { }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    if (_panel != null)
        //    {
        //        _panel.SetEnabled(false);
        //        _panel = null;
        //    }
    }

    //protected virtual void OnUpdate() { }

    public void SetCursorActive(bool value)
    {
        UnityEngine.Cursor.visible = value;
        UnityEngine.Cursor.lockState = value switch
        {
            true => CursorLockMode.None,
            false => CursorLockMode.Locked
        };
    }

    public void OpenActionPanel(string text, UnityAction callback)
    {
        var templateContainer = ResourceManager.Instance.UIData.ActionPanelVisualTreeAsset.Instantiate();
        templateContainer.Q<ActionPanel>().Action += callback;
        _UIDocument.rootVisualElement.Add(templateContainer);
    }

    public void PlayAudio()
    {
        _soundAudioSource.Play();
    }

    #endregion
}