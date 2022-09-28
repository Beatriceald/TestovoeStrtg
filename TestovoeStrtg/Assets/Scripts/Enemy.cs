using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    Idle,
    WalkToUnit,
    Attack
}

public class Enemy : MonoBehaviour {

    public EnemyState CurrentEnemyState;

    [SerializeField] private int _health;
    private int _maxHealth;

    public Knight TargetUnit;
    [SerializeField] private float _distanceToFollow = 100f;
    [SerializeField] private float _distanceToAttack = 1f;

    public NavMeshAgent NavMeshAgent;

    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private float _attackPeriod = 1f;
    private float _timer;

    [SerializeField] private GameObject _healthBarPrefab;
    private HealthBar _healthBar;

    void Start() {
        SetState(EnemyState.Idle);
        _maxHealth = _health;
        GameObject healthBar = Instantiate(_healthBarPrefab);
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Setup(transform);
    }

    // Update is called once per frame
    void Update() {
        if (CurrentEnemyState == EnemyState.Idle) {
            FindClosestUnit();
        } else if (CurrentEnemyState == EnemyState.WalkToUnit) {

            if (TargetUnit) {
                NavMeshAgent.SetDestination(TargetUnit.transform.position);
                float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);
                if (distance > _distanceToFollow) {
                    SetState(EnemyState.Idle);
                }
                if (distance < _distanceToAttack) {
                    SetState(EnemyState.Attack);
                }
            }
        } else if (CurrentEnemyState == EnemyState.Attack) {
            if (TargetUnit) {
                NavMeshAgent.SetDestination(TargetUnit.transform.position);

                float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);
                if (distance > _distanceToAttack) {
                    SetState(EnemyState.WalkToUnit);
                }
                _timer += Time.deltaTime;
                if (_timer > _attackPeriod) {
                    _timer = 0;
                    // отнять здоровье юниту
                    TargetUnit.TakeDamage(_attackDamage);
                }
            } else {
                SetState(EnemyState.Idle);
            }
            
        }
    }

    public void SetState(EnemyState enemyState) {
        CurrentEnemyState = enemyState;
        if (CurrentEnemyState == EnemyState.Attack) {
            _timer = 0;
        }
    }

    public void FindClosestUnit() {
        Knight[] allUnits = FindObjectsOfType<Knight>();

        float minDistance = Mathf.Infinity;
        Knight closestUnit = null;

        for (int i = 0; i < allUnits.Length; i++) {
            float distance = Vector3.Distance(transform.position, allUnits[i].transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                closestUnit = allUnits[i];
            }
        }
        if (minDistance < _distanceToFollow) {
            TargetUnit = closestUnit;
            SetState(EnemyState.WalkToUnit);
        }
    }

    public void TakeDamage(int damageValue) {
        _health -= damageValue;
        _healthBar.SetHealth(_health, _maxHealth);
        if (_health <= 0) {
            // Die
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        if (_healthBar) {
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
