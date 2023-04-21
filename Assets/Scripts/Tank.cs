using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Tank : MonoBehaviour, IDamageable
{
    public event System.Action Damaged = delegate { };
    public event System.Action Died = delegate { };

    [SerializeField]
    private VehicleData _data;
    public VehicleData Data => _data;

    public float Health { get; } = 2500;
    public float CurrentHealth { get; private set; } = 2500;

    public bool IsDead { get; private set; }

    private Slider _healthBar;

    private Camera _camera;

    private AudioSource _audioSource;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        //CurrentHealth = Health;

        _healthBar = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        //_camera.enabled = true;
    }

    private void Update()
    {
        /*
        transform.Translate(0, 0, Input.GetAxis("Vertical") * _data.Speed, Space.Self);
        transform.Rotate(0, Input.GetAxis("Horizontal") * _data.RotationSpeed * Time.deltaTime, 0);

        if(Input.GetMouseButtonDown(0))
        {

        }
        */
    }

    private void OnDisable()
    {
        //_camera.enabled = false;
    }

    public void Damage(float damage, RaycastHit hit)
    {
        CurrentHealth -= damage;

        _healthBar.value = CurrentHealth;

        if (CurrentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Died.Invoke();
    }
}
