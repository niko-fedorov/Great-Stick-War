using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IDamageable, IInteractable
{
    public event System.Action Damaged = delegate { };
    public event System.Action<Player> Died = delegate { };

    // Interaction Settings
    public bool IsInteractable => true;
    public string ActionText => string.Empty;
    public string Name => _nickname;
    public Color TextColor => GameManager.Instance.GetSideColor(_sideType);

    [SerializeField]
    private string _nickname;

    [SerializeField]
    private SideType _sideType;
    public void SetSide(SideType sideType)
        => _sideType = sideType;

    private float _health = 100;
    public float Health => _health;
 
    public bool IsDead { get; private set; }

    private Animator _animator;
    private AudioSource _audioSource;
    public PlayerController Controller { get; private set; }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        Controller = GetComponent<PlayerController>();
    }

    public void Damage(float damage, RaycastHit hit)
        => DamageClientRpc(damage);

    [ClientRpc]
    public void DamageClientRpc(float value)
    {
        _health -= value;

        Damaged.Invoke();

        if (_health <= 0)
            Die();
    }

    public void Die()
    {
        _animator.SetTrigger("Death");
        Controller.enabled = false;

        //Died.Invoke();
    }
}
