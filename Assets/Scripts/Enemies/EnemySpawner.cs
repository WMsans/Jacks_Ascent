using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns a prefab at this transform's location on a random timer.
/// Provides methods to enable and disable the spawning process.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [Tooltip("The enemy prefab to be spawned.")]
    [SerializeField] private GameObject _enemyPrefab;

    [Tooltip("The minimum time (in seconds) between spawns.")]
    [SerializeField] private float _minPeriod = 2.0f;

    [Tooltip("The maximum time (in seconds) between spawns.")]
    [SerializeField] private float _maxPeriod = 5.0f;

    [Header("Initial State")]
    [Tooltip("Should the spawner be active when the game starts?")]
    [SerializeField] private bool _initialEnabled = true;

    private Coroutine _spawnCoroutine;
    private bool _isSpawning = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Start()
    {
        if (_initialEnabled)
        {
            EnableSpawning();
        }
    }

    /// <summary>
    /// Starts the enemy spawning process if it's not already active.
    /// </summary>
    public void EnableSpawning()
    {
        if (_isSpawning) return; // Already spawning, do nothing.

        if (_enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the EnemySpawner.", this);
            return;
        }

        _isSpawning = true;
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Stops the enemy spawning process if it's currently active.
    /// </summary>
    public void DisableSpawning()
    {
        if (!_isSpawning) return; // Not spawning, do nothing.

        _isSpawning = false;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    /// <summary>
    /// The coroutine responsible for the spawn loop.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true) // Loop is managed by starting/stopping the coroutine
        {
            // Spawn the enemy at the spawner's position and rotation.
            Instantiate(_enemyPrefab, transform.position, transform.rotation);
            // Calculate a random delay and wait.
            float waitTime = Random.Range(_minPeriod, _maxPeriod);
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// Validates data in the editor to prevent invalid configurations.
    /// </summary>
    private void OnValidate()
    {
        if (_minPeriod < 0.1f)
        {
            _minPeriod = 0.1f;
        }
        if (_maxPeriod < _minPeriod)
        {
            _maxPeriod = _minPeriod;
        }
    }
}