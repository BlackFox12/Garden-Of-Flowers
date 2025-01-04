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
        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        activeSpriteRenderer = spriteRendererDown; // Standardriktning
    }

    private void Update()
    {
        // Hämta NavMeshAgentens rörelsevektor
        Vector3 velocity = navAgent.velocity;

        // Ignorera rörelser som är mycket små
        if (velocity.magnitude > 0.1f)
        {
            // Bestäm riktningen baserat på hastigheten
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.z))
            {
                if (velocity.x > 0)
                {
                    SetActiveSprite(spriteRendererRight); // Rörelse åt höger
                }
                else
                {
                    SetActiveSprite(spriteRendererLeft); // Rörelse åt vänster
                }
            }
            else
            {
                if (velocity.z > 0)
                {
                    SetActiveSprite(spriteRendererUp); // Rörelse uppåt
                }
                else
                {
                    SetActiveSprite(spriteRendererDown); // Rörelse nedåt
                }
            }
        }
        else
        {
            // Stanna (idle)
            activeSpriteRenderer.idle = true;
        }
    }

    private void SetActiveSprite(AnimatedSpriteRenderer newSpriteRenderer)
    {
        if (activeSpriteRenderer == newSpriteRenderer)
        {
            return; // Om samma sprite redan är aktiv, gör inget
        }

        // Avaktivera alla andra spriteRenderers
        //spriteRendererUp.enabled = newSpriteRenderer == spriteRendererUp;
        //spriteRendererDown.enabled = newSpriteRenderer == spriteRendererDown;
        //spriteRendererLeft.enabled = newSpriteRenderer == spriteRendererLeft;
        //spriteRendererRight.enabled = newSpriteRenderer == spriteRendererRight;
        newSpriteRenderer = spriteRendererDown;

        activeSpriteRenderer = newSpriteRenderer;
        activeSpriteRenderer.idle = false;
    }
}
