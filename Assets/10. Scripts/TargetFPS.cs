using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    [SerializeField] int targetFrameRate;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
