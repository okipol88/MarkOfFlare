using System;
using System.Threading.Tasks;
using MarkOfFlare.Interfaces;
using MarkOfFlare.Messages;
using MarkOfFlare.Services;
using MvvmBlazor.ViewModel;

namespace MarkOfFlare.ViewModel
{

  public class XrpKeyDeriviationViewModel : ViewModelBase, IXrpKeyDeriviationViewModel
  {
    private readonly IMessenger messenger;
    private readonly IFlareSigner flareSigner;
    public XrpKeyDeriviationViewModel(IMessenger messenger, IFlareSigner flareSigner)
    {
      this.messenger = messenger;
      this.flareSigner = flareSigner;
    }

    private string mnemonic;
    private string password;
    private string address;
    private Exception deriviationError;
    private bool isKeyDeriviationDisabled = true;

    public string Mnemonic
    {
      get => mnemonic;
      set 
      {
        if (Set(ref mnemonic, value))
        {
          IsKeyDeriviationDisabled = !(Mnemonic?.Length > 0);
        }
      }
    }

    public string Password
    {
      get => password;
      set => Set(ref password, value);
    }

    public string Address
    {
      get => address;
      set => Set(ref address, value);
    }

    public Exception DeriviationError
    {
      get => deriviationError;
      set => Set(ref deriviationError, value);
    }
    public bool IsKeyDeriviationDisabled
    {
      get => isKeyDeriviationDisabled;
      set => Set(ref isKeyDeriviationDisabled, value);
    }

    public async Task DeriveKeys()
    {
      try
      {
        // "attend dinner chat movie brain invite forest quiz bulb taste evidence danger"
        var keyPair = await flareSigner.DeriveKeyPair(mnemonic, password);
        Address = await flareSigner.GetAddress(keyPair.@public);
        messenger.Send(new XrpSigningInformationMessage(keyPair, address));
      }
      catch (Exception)
      {
        Address = null;
        messenger.Send(new XrpSigningInformationMessage(null, null));
      }
    }
  }
}