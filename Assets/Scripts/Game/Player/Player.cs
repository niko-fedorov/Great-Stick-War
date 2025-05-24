using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public struct PlayerData
{
    public string Name { get; private set; }
    public Sprite Icon { get; private set; }

    public PlayerData(string name, Sprite icon)
    {
        Name = name;
        Icon = icon;
    }
}

namespace Game
{
    public class Player : NetworkBehaviour
    {
        #region Variables 

        #region Events

        public event UnityAction<PlayerController> ControllerSpawned = delegate { };
        public event UnityAction SideSelected = delegate { };
        public event UnityAction ClassSelected = delegate { };
        public event UnityAction Disconnected = delegate { };

        #endregion

        // Interaction Settings
        public bool IsInteractable => true;
        public string ActionText => string.Empty;

        private NetworkVariable<FixedString32Bytes> _name = new NetworkVariable<FixedString32Bytes>();
        public string Name => _name.Value.ToString();

        private NetworkVariable<SideType> _sideType = new NetworkVariable<SideType>();
        public SideType SideType => _sideType.Value;

        private NetworkVariable<ClassType> _classType = new NetworkVariable<ClassType>();
        public ClassData ClassData => GameData.Instance.GetClassData(_classType.Value);

        private float _health = 100;
        public NetworkVariable<float> Health { get; } = new NetworkVariable<float>();

        private NetworkVariable<int> _kills = new NetworkVariable<int>();
        public int Kills => _kills.Value;

        private NetworkVariable<int> _deaths = new NetworkVariable<int>();
        public int Deaths => _deaths.Value;

        public bool IsDead { get; private set; }

        [SerializeField]
        private PlayerController _controllerPrefab;
        public PlayerController Controller { get; private set; }

        #endregion

        #region Methods

        public void Initialize(string name)
        {
            _name.Value = name;
        }

        private void Start()
        {
            Controller = GetComponent<PlayerController>();
        }

        public void Spawn()
        {
            var controller = Instantiate(_controllerPrefab);
            controller.NetworkObject.SpawnWithOwnership(OwnerClientId);
            SpawnClientRpc(controller);
        }
        [ClientRpc]
        private void SpawnClientRpc(NetworkBehaviourReference controller)
        {
            if (controller.TryGet(out PlayerController controllerObject))
                ControllerSpawned.Invoke(controllerObject);
        }

        [ServerRpc]
        public void SelectSideServerRpc(SideType sideType)
        {
            _sideType.Value = sideType;
            SideSelected.Invoke();
        }

        [ServerRpc]
        public void SelectClassServerRpc(ClassType classType)
        {
            _classType.Value = classType;

            ClassSelected.Invoke();
        }

        public void Damage(float damage, RaycastHit hit)
            => DamageClientRpc(damage);

        [ClientRpc]
        public void DamageClientRpc(float value)
        {
            _health -= value;

            //Damaged.Invoke();

            if (_health <= 0)
                Die();
        }

        public void Die()
        {
            //_animator.SetTrigger("Death");
            Controller.enabled = false;

            //Died.Invoke();
        }
    }

    #endregion
}