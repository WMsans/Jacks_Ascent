using System.Collections;
using FMOD;
using UnityEngine;

public class RisingMouth : MonoBehaviour
{
    public Transform player;
    public float riseTime = 2f;
    public float chaseSpeed = 3f;

    public bool isFlickered = false;
    public bool isMoved = false;

    public Transform firstPosition;
    public Transform secondPosition;
    public Transform thirdPosition;
    public Transform fourthPosition;

    private bool stopAndChasePlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartMouthBehavior()
    {
        if (!isFlickered)
        {
            StopAllCoroutines();
            SoundManager.Instance.MouthIdle();
            StartCoroutine(RotateThenMove(firstPosition.position, secondPosition.position, thirdPosition.position, fourthPosition.position));
            SoundManager.Instance.MouthDetect();
            isFlickered = true;
        }
    }

    private IEnumerator RotateThenMove(Vector3 Pos1, Vector3 Pos2, Vector3 Pos3, Vector3 Pos4)
    {
        if (isMoved) yield break;
        isMoved = true;

        yield return StartCoroutine(SmoothMove(Pos1, riseTime));
        if (stopAndChasePlayer) { yield return StartCoroutine(FaceAndChasePlayer()); yield break; }

        yield return StartCoroutine(SmoothMove(Pos2, riseTime));
        if (stopAndChasePlayer) { yield return StartCoroutine(FaceAndChasePlayer()); yield break; }

        yield return StartCoroutine(SmoothMove(Pos3, riseTime));
        if (stopAndChasePlayer) { yield return StartCoroutine(FaceAndChasePlayer()); yield break; }

        yield return StartCoroutine(SmoothMove(Pos4, riseTime));
        if (stopAndChasePlayer) { yield return StartCoroutine(FaceAndChasePlayer()); yield break; }

        Destroy(gameObject);
    }

    IEnumerator SmoothMove(Vector3 target, float duration)
    {
        Vector3 dir = transform.position - target;
        transform.rotation = Quaternion.LookRotation(dir);
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (transform.position.y > player.position.y)
            {
                stopAndChasePlayer = true;
                yield break;
            }

            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator FaceAndChasePlayer()
    {
        StartCoroutine(RotateAndFacePlayer(1.5f));
        SoundManager.Instance.MouthAttackSFX();
        StartCoroutine(SmoothMove(player.position, 0.7f));
        yield return null;
    }

    public IEnumerator RotateAndFacePlayer(float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(transform.eulerAngles.x + 90f, transform.eulerAngles.y, transform.eulerAngles.z);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;

        Vector3 direction = transform.position - player.position;
        startRot = transform.rotation;
        elapsed = 0f;
        while (elapsed < 2f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(startRot, lookRotation, elapsed / 2f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            SoundManager.Instance.MouthImpact();
        }
    }
}
