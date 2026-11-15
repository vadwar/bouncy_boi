using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Rotation Stats")]
    [SerializeField] private float flipYRotationTime = 0.5f;

    private PlayerMovement playerScript;

    private bool isFacingRight;

    private void Awake()
    {
        playerScript = playerTransform.gameObject.GetComponent<PlayerMovement>();

        isFacingRight = !playerScript.isFacingRight;
    }

    private void Update()
    {
        transform.position = playerScript.transform.position;
    }

    public void callTurn()
    {
        LeanTween.rotateY(gameObject, determineEndRotation(), flipYRotationTime).setEaseInOutSine();
    }

    private float determineEndRotation()
    {
        isFacingRight = !isFacingRight;
        if (isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
