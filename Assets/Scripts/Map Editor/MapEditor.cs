using Game;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEditor
{
    public class MapEditor : Singleton<MapEditor>
    {
        public bool IsEditing { get; private set; }

        [SerializeField]
        private CameraController _cameraController;

        private Color _color;
        private ToolType _toolType = ToolType.Place;
        private SelectionType _selectionType;

        private Vector3Int _voxel;
        private int _face;
        private int[] _plane;

        private bool _isEditing;

        private Stack<EditAction> _editActions;

        private void Start()
        {
            _voxel = WorldGenerator.Instance.WorldSize;

            _face = -1;
            _plane = new int[0];

            UIManager.Instance.MenuOpened += () => _isEditing = true;
            UIManager.Instance.MenuClosed += () => _isEditing = false;

            UIManager.Instance.MapSave += (path, mapName) => SaveMap(path, mapName);
        }

        private void SaveStructure()
        {
            var data = ScriptableObject.CreateInstance<StructureData>();
            data.Initialize(name, WorldGenerator.Instance.Data);

            AssetDatabase.CreateAsset(data, "Scriptable Objects/Map Datas");
        }

        public void SaveMap(string path, string name)
        {
            ResourceManager.Instance.Save(path,
                new MapData
                (
                   name,
                   null,
                   Vector2Int.zero,
                   new GameModeData(),
                   WorldGenerator.Instance.Data
                ));
        }

        private void Update()
        {
            if (_isEditing)
                return;

            if (Physics.Raycast(_cameraController.Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit)
                && !EventSystem.current.IsPointerOverGameObject())
            { 
                var index = Vector3Int.FloorToInt(raycastHit.point - raycastHit.normal / 2);
                var face = raycastHit.triangleIndex * 2;

                WorldGenerator.Instance.GetVoxel(index, out Voxel voxel);

                switch (_selectionType)
                {
                    case SelectionType.Face:

                        if (face != _face)
                        {
                            if (_face != -1)
                                WorldGenerator.Instance.ResetFaceColor(_face);

                            WorldGenerator.Instance.SetInverseFaceColor(face);

                            _face = face;
                        }

                        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                        {
                            _editActions.Push(
                                new EditAction(
                                    index + raycastHit.normal * (_toolType == ToolType.Place ? 1 : 0),
                                    Input.GetMouseButtonDown(0) ? _toolType == ToolType.Place ? Color.clear : WorldGenerator.Instance.GetColor(index) : WorldGenerator.Instance.GetColor(index)));

                            WorldGenerator.Instance.SetVoxel(
                                index + raycastHit.normal * (_toolType == ToolType.Place ? 1 : 0), 
                                Input.GetMouseButtonDown(0) ? _color : _toolType == ToolType.Place ? Color.clear : WorldGenerator.Instance.GetColor(index));
                        }

                        break;

                    case SelectionType.Plane:

                        if (!_plane.Contains(face))
                        {
                            if (_plane.Length != 0)
                                WorldGenerator.Instance.ResetPlaneColor(_plane);

                            _plane = WorldGenerator.Instance.GetPlane(index, raycastHit.normal);

                            WorldGenerator.Instance.SetInversePlaneColor(_plane);
                        }

                        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                        {
                            //_editActions.Push(
                            //    new EditAction(
                            //        _plane.Select(voxel => voxel + raycastHit.normal * (_toolType == ToolType.Paint ? 0 : 1)), 
                            //        Input.GetMouseButtonDown(0) ? new Color[_plane.Length] : WorldGenerator.Instance.GetVoxels(_plane).Select(voxel => voxel.Color)));
                            
                            WorldGenerator.Instance.SetPlaneVoxel(
                                _plane, 
                                raycastHit.normal * (_toolType == ToolType.Paint ? 0 : 1),
                                Input.GetMouseButtonDown(0) ? _color : _toolType == ToolType.Place ? Color.clear : WorldGenerator.Instance.GetColor(index));
                        }

                        break;
                };

                for (int i = 0; i < 2; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                        IsEditing = true;
                    else if (Input.GetMouseButtonUp(i))
                        IsEditing = false;
                }
            }
            else
            {
                switch (_selectionType)
                {
                    case SelectionType.Face:
                        if (_face != -1)
                        {
                            WorldGenerator.Instance.ResetFaceColor(_face);
                            _face = -1;
                        }
                        break;
                    case SelectionType.Plane:
                        if (_plane.Length > 0)
                        {
                            WorldGenerator.Instance.ResetPlaneColor(_plane);
                            _plane = new int[0];
                        }
                        break;
                };
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                var editAction = _editActions.Pop();
                WorldGenerator.Instance.SetVoxels(editAction.Voxels, editAction.Colors);
            }
        }

        public void SelectColor(Color value)
            => _color = value;

        public void SelectToolType(ToolType value)
            => _toolType = value;

        public void SelectSelectionType(SelectionType value)
            => _selectionType = value;
    }
}
