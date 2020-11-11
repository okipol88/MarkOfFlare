using System;
using System.Collections.Generic;
using MarkOfFlare.Messages;
using MarkOfFlare.Services;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;

namespace MarkOfFlare.ViewModel
{
    public interface IFlareWizarClaimViewModel
    {
        public int Progress { get; set; }

        public bool PreviousButtonDisabled { get; set; } 

        public bool NextButtonDisabled { get; set; }

        public int CurrentStep { get; set; }

        public void GoToPreviousStep();

        public void GoToNextStep();
    }

    public class FlareClaimViewModel: ViewModelBase, IFlareWizarClaimViewModel
    {
        private readonly int MaxSteps = 2;
        private readonly IMessenger messenger;

        public FlareClaimViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        private int progress = 50;
        private bool previousButtonDisabled = true;
        private bool nextButtonDisabled = true;
        private int currentStep = 1;
        private readonly Dictionary<int, bool> _stepNumberToNavigationMap = new Dictionary<int, bool>()
        {
            {0, false },
            {1, false },
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
            Progress = (CurrentStep / MaxSteps) * 100;

            UpdateNavigationPossibility();
        }

        private void UpdateNavigationPossibility()
        {
            if (_stepNumberToNavigationMap.TryGetValue(CurrentStep - 1, out bool canPrevious))
            {
                PreviousButtonDisabled = !canPrevious;
            }
            if (_stepNumberToNavigationMap.TryGetValue(CurrentStep + 1, out bool canNext))
            {
                NextButtonDisabled = !canNext;
            }

            PreviousButtonDisabled |= CurrentStep == 1;
            NextButtonDisabled |= CurrentStep == MaxSteps;
        }

        public void GoToNextStep()
        {
            CurrentStep = 2;
            UpdateOnNavigate();
        }
    }
}
