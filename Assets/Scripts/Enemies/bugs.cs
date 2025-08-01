using System.Collections;
using UnityEngine;

//TODO: smooth the movement
public class Bugs : MonoBehaviour
{
    public Transform player;
    public float forwardOffset = 5f;
    public float downOffset = 5f;
    public float teleportInterval = 2f;
    public float flickerOffset = 1.5f;
    public float flickerDelay = 1f;
    public int health = 2;

    public Vector3 finalPos; // To store the final position after flickering

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (player != null)
        {
            StartCoroutine(TeleportTowardsPlayer());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health--;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    } 
    IEnumerator TeleportTowardsPlayer()
    {
        Vector3 dir = transform.forward;
        Vector3 targetPos = player.position + dir * forwardOffset + Vector3.up * 0.5f;

        //Vector3 startPos = targetPos - Vector3.up * downOffset;
        Vector3 startPos = transform.position;
        //transform.position = startPos;

        yield return new WaitForSeconds(teleportInterval);

        Vector3 step1 = Vector3.Lerp(startPos, targetPos, 0.33f);
        step1.y = Mathf.Lerp(startPos.y, targetPos.y, 0.33f);
        yield return StartCoroutine(SmoothMove(step1, teleportInterval/2f));

        Vector3 step2 = Vector3.Lerp(startPos, targetPos, 0.66f);
        step2.y = Mathf.Lerp(startPos.y, targetPos.y, 0.66f);
        yield return StartCoroutine(SmoothMove(step2, teleportInterval));

        finalPos = player.position + dir * forwardOffset;
        finalPos.y = player.position.y + 1.1f;
        yield return StartCoroutine(SmoothMove(finalPos, teleportInterval/0.5f));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FlickerAroundPlayer());
    }

    IEnumerator FlickerAroundPlayer()
    {
        Vector3[] offsets = new Vector3[4]
        {
            Vector3.up * flickerOffset,
            Vector3.down * flickerOffset,
            Vector3.left * flickerOffset,
            Vector3.right * flickerOffset
        };
        int[] copyInt = new int[4];

        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = player.TransformDirection(offsets[i]);
        }

        for (int z = 0; z < 4; z++)
        {
            int randIndex = Random.Range(0, offsets.Length);
            if (randIndex == copyInt[0] || randIndex == copyInt[1] || randIndex == copyInt[2])
            {
                z--;
                continue;
            }
            copyInt[z] = randIndex;
            Vector3 flickerPos = finalPos + offsets[randIndex];
            yield return StartCoroutine(SmoothMove(flickerPos, 0.3f));
            yield return new WaitForSeconds(flickerDelay);
        }

        for (int z = 0; z < 4; z++)
        {
            Vector3 flickerPos = finalPos + offsets[copyInt[z]];
            yield return StartCoroutine(SmoothMove(flickerPos, 0.3f));
            yield return new WaitForSeconds(flickerDelay);
        }

        yield return StartCoroutine(SmoothMove(player.position, 0.3f)); 
    }

    IEnumerator SmoothMove(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            Vector3 dir = player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }
}
