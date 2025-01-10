using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent navAgent;

    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        activeSpriteRenderer = spriteRendererDown;
        
        // Ensure initial sprite is visible
        SetActiveSprite(spriteRendererDown);
    }

    private void Update()
    {
        Vector2 velocity = new Vector2(navAgent.velocity.x, navAgent.velocity.z);

        // Check if we're moving
        if (velocity.magnitude > 0.1f)
        {
            // Determine primary direction of movement
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
            {
                // Horizontal movement
                if (velocity.x > 0)
                {
                    SetActiveSprite(spriteRendererRight);
                }
                else
                {
                    SetActiveSprite(spriteRendererLeft);
                }
            }
            else
            {
                // Vertical movement
                if (velocity.y > 0)
                {
                    SetActiveSprite(spriteRendererUp);
                }
                else
                {
                    SetActiveSprite(spriteRendererDown);
                }
            }
            // Set idle to false when moving
            activeSpriteRenderer.idle = false;
        }
        else
        {
            // Set idle to true when not moving
            activeSpriteRenderer.idle = true;
        }
    }

    private void SetActiveSprite(AnimatedSpriteRenderer newSpriteRenderer)
    {
        // Disable all sprites first
        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;

        // Enable only the new sprite
        newSpriteRenderer.enabled = true;
        
        activeSpriteRenderer = newSpriteRenderer;
    }
}
