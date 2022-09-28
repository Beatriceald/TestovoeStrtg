using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState {
    Idle,
    WalkToEnemy,
    Attack
}

public class Knight : MonoBehaviour {

    public UnitState CurrentUnitState;

    [SerializeField] private int _health;
    private int _maxHealth;

    public Enemy TargetEnemy;
    [SerializeField] private float _distanceToFollow = 100f;
    [SerializeField] private float _distanceToAttack = 1f;

    public NavMeshAgent NavMeshAgent;

    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private float _attackPeriod = 1f;
    private float _timer;

    [SerializeField] private GameObject _healthBarPrefab;
    private HealthBar _healthBar;

    void Start() {
        SetState(UnitState.Idle);
        _maxHealth = _health;
        GameObject healthBar = Instantiate(_healthBarPrefab);
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Setup(transform);
    }

    // Update is called once per frame
    void Update() {
        if (CurrentUnitState == UnitState.Idle) {
            FindClosestEnemy();
        } else if (CurrentUnitState == UnitState.WalkToEnemy) {

            if (TargetEnemy) {
                NavMeshAgent.SetDestination(TargetEnemy.transform.position);
                float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);
                if (distance > _distanceToFollow) {
                    SetState(UnitState.Idle);
                }
                if (distance < _distanceToAttack) {
                    SetState(UnitState.Attack);
                }
            }
        } else if (CurrentUnitState == UnitState.Attack) {
            if (TargetEnemy) {
                NavMeshAgent.SetDestination(TargetEnemy.transform.position);

                float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);
                if (distance > _distanceToAttack) {
                    SetState(UnitState.WalkToEnemy);
                }
                _timer += Time.deltaTime;
                if (_timer > _attackPeriod) {
                    _timer = 0;
                    // отнять здоровье юниту
                    TargetEnemy.TakeDamage(_attackDamage);
                }
            } else {
                SetState(UnitState.Idle);
            }

        }
    }

    public void SetState(UnitState unitState) {
        CurrentUnitState = unitState;
        if (CurrentUnitState == UnitState.Attack) {
            _timer = 0;
        }
    }

    public void FindClosestEnemy() {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        float minDistance = Mathf.Infinity;
        Enemy closestEnemy = null;

        for (int i = 0; i < allEnemies.Length; i++) {
            float distance = Vector3.Distance(transform.position, allEnemies[i].transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                closestEnemy = allEnemies[i];
            }
        }
        if (minDistance < _distanceToFollow) {
            TargetEnemy = closestEnemy;
            SetState(UnitState.WalkToEnemy);
        }
    }

    public void TakeDamage(int damageValue) {
        _health -= damageValue;
        _healthBar.SetHealth(_health, _maxHealth);
        if (_health <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        if(_healthBar) {
            Destroy(_healthBar.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, _distanceToAttack);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, _distanceToFollow);
    }
#endif
}
