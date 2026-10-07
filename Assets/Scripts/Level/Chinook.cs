using UnityEngine;

/// <summary>
/// Visuele kant van de Chinook: rotors draaien in een loop,
/// de laadklep gaat open zodra de speler aankomt.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Chinook : MonoBehaviour
{
    private static readonly int OpenHatchTrigger = Animator.StringToHash("OpenHatch");

    private Animator animator;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenHatch()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;
        animator.SetTrigger(OpenHatchTrigger);
    }
}