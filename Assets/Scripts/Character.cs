using UnityEngine;

public class Character : MonoBehaviour
{
    public Vector2Int currentPosition;

    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private Animator animator;
    private int animatorHashSpeed;

    void Start()
    {
        animator.SetFloat(Animator.StringToHash("MotionSpeed"), 1);
        animatorHashSpeed = Animator.StringToHash("Speed");
    }
    public void ChangeColor(Color color)
    {
        mesh.materials[0].color = color;
    }

    [ContextMenu("Play Animation")]
    public void PlayAnimation()
    {
        animator.SetFloat(animatorHashSpeed, 10);
    }
}