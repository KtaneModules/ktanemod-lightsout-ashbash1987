using UnityEngine;
using System.Linq;

public class LightsOutModule : MonoBehaviour
{
    public KMSelectable Selectable;
    public KMNeedyModule NeedyModule;

    private LightsOutButton[] ChildButtons
    {
        get
        {
            return GetComponentsInChildren<LightsOutButton>();
        }
    }

    private bool AllLightsOff
    {
        get
        {
            return ChildButtons.All((x) => !x.LitState);
        }
    }

    private void Start()
    {
        NeedyModule.OnNeedyActivation += OnActivate;
        NeedyModule.OnTimerExpired += OnTimerExpired;

        foreach (KMSelectable selectable in Selectable.Children)
        {
            selectable.OnInteract += OnButtonInteract;
        }
	}

    private void OnDestroy()
    {
        NeedyModule.OnNeedyActivation -= OnActivate;
        NeedyModule.OnTimerExpired -= OnTimerExpired;

        foreach (KMSelectable selectable in Selectable.Children)
        {
            selectable.OnInteract -= OnButtonInteract;
        }
    }

    private void OnActivate()
    {
        while (AllLightsOff)
        {
            RandomizeLights();
        }

        EnableLights(true);
    }

    private void OnTimerExpired()
    {
        if (!AllLightsOff)
        {
            NeedyModule.HandleStrike();
        }

        ResetLights();
        EnableLights(false);
    }

    private bool OnButtonInteract()
    {
        if (AllLightsOff)
        {
            NeedyModule.HandlePass();
            EnableLights(false);
        }

        return false;
    }

    private void ResetLights()
    {
        foreach (LightsOutButton button in ChildButtons)
        {
            button.SetState(false);
        }
    }

    private void RandomizeLights()
    {
        foreach (LightsOutButton button in ChildButtons)
        {
            button.SetState(Random.Range(0, 2) == 1);
        }
    }

    private void EnableLights(bool enable)
    {
        foreach (LightsOutButton button in ChildButtons)
        {
            button.CanInteract = enable;
        }
    }
}
