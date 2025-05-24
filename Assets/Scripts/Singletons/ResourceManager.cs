using System.IO;
using System.Linq;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField]
    private UIData _UIData;
    public UIData UIData => _UIData;

    private string _path;

    protected override void OnAwake()
    {
        _path = Path.Combine(Application.persistentDataPath, "saves");

        Directory.CreateDirectory(_path);
    }

    public bool CheckFile(string path)
        => File.Exists(Path.Combine(_path, path));

    public string[] GetFiles<T>()
        => Directory.GetFiles(_path)
        .Select(file => file.Replace(_path, ""))
        .ToArray();

    public void Save(string file, object data)
    {
        File.WriteAllText(Path.Combine(_path, file), JsonUtility.ToJson(data));
    }

    public T Load<T>(string fileName)
    {
        return JsonUtility.FromJson<T>
               (File.ReadAllText(Path.Combine(_path, fileName)));

    }
    public T[] Load<T>()
        => Directory.GetFiles(_path)
           .Select(file =>
           JsonUtility.FromJson<T>
           (File.ReadAllText(Path.Combine(_path, file))))
           .ToArray();
}

