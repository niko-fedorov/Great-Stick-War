using Game;
using UnityEngine;

namespace MapEditor
{
    public class CameraController : MonoBehaviour
    {
        public bool IsMoving { get; private set; }

        public Camera Camera { get; private set; }

        [SerializeField]
        private float _cameraMotionSpeed,
                      _cameraSpeedScale,
                      _cameraRotationSpeed,
                      _cameraScrollValue;
        private float _cameraSpeedMultiplier;

        private Vector3 _mousePosition;

        private void Start()
        {
            Camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (MapEditor.Instance.IsEditing)
                return;

            if (Input.GetMouseButtonDown(1))
                IsMoving = true;
            if (Input.GetMouseButton(1))
            {
                transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _cameraMotionSpeed * Time.deltaTime);
                var point = WorldGenerator.Instance.WorldSize;

                transform.RotateAround(
                    new Vector3(1, 0, 1),
                    Vector3.up, Input.GetAxis("Mouse X") * _cameraRotationSpeed * Time.deltaTime);
                //transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * _cameraRotationSpeed * Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(1))
                IsMoving = false;

            if (Input.mouseScrollDelta.y != 0)
                transform.Translate(0, 0, Input.mouseScrollDelta.y * _cameraScrollValue);
            if (Input.GetMouseButtonDown(2))
            {
                _mousePosition = Input.mousePosition;

                IsMoving = true;
            }
            if (Input.GetMouseButton(2))
            {
                transform.Translate((_mousePosition - Input.mousePosition) * _cameraMotionSpeed * Time.deltaTime);
                _mousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(2))
                IsMoving = false;

            if (Input.GetKeyDown(KeyCode.LeftShift))
                _cameraSpeedMultiplier = 1;
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                _cameraSpeedMultiplier = 0;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -WorldGenerator.Instance.WorldSize.x, WorldGenerator.Instance.WorldSize.x * 2),
                Mathf.Clamp(transform.position.y, -WorldGenerator.Instance.WorldSize.y, WorldGenerator.Instance.WorldSize.y * 2),
                Mathf.Clamp(transform.position.z, -WorldGenerator.Instance.WorldSize.z, WorldGenerator.Instance.WorldSize.z * 2));
        }
    }
}

