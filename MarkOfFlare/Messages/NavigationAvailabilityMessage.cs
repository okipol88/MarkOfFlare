using System;
namespace MarkOfFlare.Messages
{
    public class NavigationAvailabilityMessage
    {
        public NavigationAvailabilityMessage(int stepNumber, bool canNavigate)
        {
            StepNumber = stepNumber;
            CanNavigate = canNavigate;
        }

        public int StepNumber { get; set; }

        public bool CanNavigate { get; set; }
    }
}
