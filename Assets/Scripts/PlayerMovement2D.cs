using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
    [SerializeField] private InputAction moveAction =
        new InputAction(name: "Move", type: InputActionType.Value, expectedControlType: "Vector2");

    private Rigidbody2D body;
    private Vector2 input;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // Default keys are added only if the serialized action has no bindings.
        if (moveAction.bindings.Count == 0)
            AddDefaultBindings();
    }

    private void OnEnable() => moveAction.Enable();
    private void OnDisable() => moveAction.Disable();

    private void Update()
    {
        input = moveAction.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    private void AddDefaultBindings()
    {
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }
}
