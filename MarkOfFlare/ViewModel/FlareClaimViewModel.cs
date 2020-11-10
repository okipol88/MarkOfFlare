using System;
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

        public EventCallback GoToPreviousStep { get; set; }

        public EventCallback GoToNextStep { get; set; }
    }

    public class FlareClaimViewModel: ViewModelBase, IFlareWizarClaimViewModel
    {
        public FlareClaimViewModel()
        {
        }

        private int progress;
        private bool previousButtonDisabled;
        private bool nextButtonDisabled;
        private int currentStep = 1;

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

        public EventCallback GoToPreviousStep { get; set; }


        public EventCallback GoToNextStep { get; set; }
    }
}
