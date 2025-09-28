using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Vector2Int currentPosition;

    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private Animator animator;
    private int animatorHashSpeed;
    private IEnumerator currentCoroutine;
    private GameMap map;

    const float speed = 3f;

    void Start()
    {
        animator.SetFloat(Animator.StringToHash("MotionSpeed"), 1);
        animatorHashSpeed = Animator.StringToHash("Speed");
        map = FindFirstObjectByType<GameMap>();
    }


    public void ChangeColor(Color color)
    {
        mesh.materials[0].color = color;
    }

    [ContextMenu("Play Animation")]
    public void PlayAnimation(float speed)
    {
        animator.SetFloat(animatorHashSpeed, Mathf.Min(speed,6));
    }

    public void Move(List<Vector2Int> nodes)
    {
        if (currentCoroutine != null) return;
        if (nodes == null || nodes.Count == 0) return;
        if (nodes.Count - 1 > map.moveRange) return;
        currentCoroutine = MoveCoroutine(nodes);
        StartCoroutine(currentCoroutine);
    }

    public IEnumerator MoveCoroutine(List<Vector2Int> nodes)
    {
        float movementAnimSpeed = 0;
        foreach (var node in nodes)
        {
            if (node == currentPosition) continue;
            Vector3 currentPos = transform.position;
            Vector3 newPos = new Vector3(node.x, 0, node.y);

            Vector3 direction = (newPos - currentPos).normalized;
            float currentAngle = transform.eulerAngles.y;
            float newAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            for (float f = 0; f < 1; f+=Time.deltaTime * speed)
            {
                transform.position = Vector3.Lerp(currentPos, newPos, f);

                float lerpedAngle = Mathf.LerpAngle(currentAngle, newAngle, f * 2);
                transform.rotation = Quaternion.Euler(0, lerpedAngle, 0);

                movementAnimSpeed += Time.deltaTime * 20;
                PlayAnimation(movementAnimSpeed);

                yield return null;
            }
            transform.position = newPos;
            
        }
        currentPosition = nodes[^1];

        for (float f = Mathf.Min(6, movementAnimSpeed); f > 0; f-= 40 * Time.deltaTime)
        {
            PlayAnimation(f);
            yield return null;
        }
        PlayAnimation(0);
        map.ResetSearch();
        map.ClearPath();
        currentCoroutine = null;
    }

    private void OnFootstep(AnimationEvent animationEvent) { }
}