using UnityEngine;

public class LightsOutButton : MonoBehaviour
{
    public KMSelectable Selectable = null;
    public KMAudio Audio = null;
    public Animator Animator = null;
    public LightsOutButton[] AdjacentButtons = null;

    public bool CanInteract = false;
    public bool LitState = false;

    private void OnEnable()
    {
        Selectable.OnInteract += OnInteract;
    }

    private void OnDestroy()
    {
        Selectable.OnInteract -= OnInteract;
    }

    private bool OnInteract()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (!CanInteract)
        {
            return false;
        }

        InvertState();
        foreach (LightsOutButton adjacentButton in AdjacentButtons)
        {
            adjacentButton.InvertState();
        }

        return false;
    }

    public void InvertState()
    {
        SetState(!LitState);
    }

    public void SetState(bool litState)
    {
        if (LitState == litState)
        {
            return;
        }

        LitState = litState;
        Animator.SetTrigger(LitState ? "Lit" : "Unlit");
    }
}
