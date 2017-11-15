using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

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
        NeedyModule.OnNeedyDeactivation += OnDeactivate;
        NeedyModule.OnTimerExpired += OnTimerExpired;

        foreach (KMSelectable selectable in Selectable.Children)
        {
            selectable.OnInteract += OnButtonInteract;
        }
	}

    private void OnDestroy()
    {
        NeedyModule.OnNeedyActivation -= OnActivate;
        NeedyModule.OnNeedyDeactivation -= OnDeactivate;
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

    private void OnDeactivate()
    {
        ResetLights();
        EnableLights(false);
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
            button.SetState(UnityEngine.Random.Range(0, 2) == 1);
        }
    }

    private void EnableLights(bool enable)
    {
        foreach (LightsOutButton button in ChildButtons)
        {
            button.CanInteract = enable;
        }
    }

    private KMSelectable[] ProcessTwitchCommand(string command)
    {
        if (!command.StartsWith("press ", StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        command = command.Substring(5);

        string[] sequence = command.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        List<KMSelectable> selectables = new List<KMSelectable>();
        foreach(string buttonString in sequence)
        {
            int buttonIndex = -1;
            if (int.TryParse(buttonString, out buttonIndex) && buttonIndex >= 1 && buttonIndex <= 9)
            {
                selectables.Add(Selectable.Children[buttonIndex - 1]);
            }
        }

        return selectables.ToArray();
    }
}
