using MvvmBlazor.ViewModel;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MarkOfFlare.Interfaces
{

  public interface IFlareWizarClaimViewModel: INotifyPropertyChanged
  {
    public int Progress { get; set; }

    public bool PreviousButtonDisabled { get; set; }

    public bool NextButtonDisabled { get; set; }

    public int CurrentStep { get; set; }

    public void GoToPreviousStep();

    public void GoToNextStep();
  }

}
