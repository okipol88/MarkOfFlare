using System;
using System.Collections.Generic;
using MarkOfFlare.Interfaces;
using MarkOfFlare.Messages;
using MarkOfFlare.Services;
using MvvmBlazor.ViewModel;

namespace MarkOfFlare.ViewModel
{
  public class FlareClaimViewModel : ViewModelBase, IFlareWizarClaimViewModel
  {
    private readonly int MaxSteps = 2;
    private readonly IMessenger messenger;

    public FlareClaimViewModel(IMessenger messenger)
    {
      this.messenger = messenger;
    }

    private int progress = 0;
    private bool previousButtonDisabled = true;
    private bool nextButtonDisabled = true;
    private int currentStep = 1;
    private readonly Dictionary<int, bool> _stepNumberToNavigationMap = new Dictionary<int, bool>()
        {
            {0, false },
            {1, true },
            {2, false },
        };

    public override void OnInitialized()
    {
      base.OnInitialized();

      messenger.Register<NavigationAvailabilityMessage>(OnNavigationAvailabilityMessageReceived);
    }

    private void OnNavigationAvailabilityMessageReceived(NavigationAvailabilityMessage obj)
    {
      _stepNumberToNavigationMap[obj.StepNumber] = obj.CanNavigate;

      UpdateNavigationPossibility();
    }

    public int Progress
    {
      get => progress;
      set => Set(ref progress, value);
    }

    public bool PreviousButtonDisabled
    {
      get => previousButtonDisabled;
      set => Set(ref previousButtonDisabled, value);
    }

    public bool NextButtonDisabled
    {
      get => nextButtonDisabled;
      set => Set(ref nextButtonDisabled, value);
    }

    public int CurrentStep
    {
      get => currentStep;
      set => Set(ref currentStep, value);
    }

    public void GoToPreviousStep()
    {
      CurrentStep = 1;
      UpdateOnNavigate();
    }

    private void UpdateOnNavigate()
    {
      Progress = (int)(((double)(CurrentStep -1 )/ (double)(MaxSteps - 1)) * 100);

      UpdateNavigationPossibility();
    }

    private void UpdateNavigationPossibility()
    {
      _stepNumberToNavigationMap.TryGetValue(CurrentStep - 1, out bool canPrevious);
      _stepNumberToNavigationMap.TryGetValue(CurrentStep + 1, out bool canNext);

      PreviousButtonDisabled = !canPrevious || CurrentStep == 1;
      NextButtonDisabled = !canNext || CurrentStep == MaxSteps;

      Console.WriteLine($"Previous disabled: {PreviousButtonDisabled}; Next disabled: {NextButtonDisabled}");
    }

    public void GoToNextStep()
    {
      CurrentStep = 2;
      UpdateOnNavigate();
    }
  }
}
