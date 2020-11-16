using System;
using System.Linq;
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
    private bool isDerivingKeys;
    private KeyMode keyMode;
    private string secret;

    public string Mnemonic
    {
      get => mnemonic;
      set => Set(ref mnemonic, value);
    }

    public string Password
    {
      get => password;
      set => Set(ref password, value);
    }
    public string Secret
    {
      get => secret;
      set => Set(ref secret, value);
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
    public bool IsDerivingKeys
    {
      get => isDerivingKeys;
      set => Set(ref isDerivingKeys, value);
    }
    public KeyMode KeyMode
    {
      get => keyMode;
      set
      {
        if (Set(ref keyMode, value))
        {
          Secret = null;
          Mnemonic = null;
        }
      }
    }

    public override void OnPropertyChanged(string propertyName)
    {
      base.OnPropertyChanged(propertyName);

      var props = new string[] { nameof(KeyMode), nameof(Secret), nameof(Mnemonic) };
      if (!props.Any(prop => prop == propertyName))
      {
        return;
      }
      AssignKeyDeriviationDisabled();
    }

    private void AssignKeyDeriviationDisabled()
    {
      IsKeyDeriviationDisabled = KeyMode == KeyMode.Mnemonic
        ? !(Mnemonic?.Length > 0)
        : !(Secret?.Length > 0);
    }

    public async Task DeriveKeys()
    { 
      if (IsDerivingKeys)
      {
        return;
      }

      IsDerivingKeys = true;
      try
      {
        Models.KeyPair keyPair = null;
        switch (KeyMode)
        {
          case KeyMode.Mnemonic:
            keyPair = await flareSigner.DeriveKeyPair(Mnemonic, Password);
            break;
          case KeyMode.Secret:
            keyPair = await flareSigner.DeriveFromSeed(Secret);
            break;
          case KeyMode.PrivateKey:
            keyPair = await flareSigner.GetPair(Secret);
            break;
          default:
            break;
        }
        Address = await flareSigner.GetAddress(keyPair.@public);
        messenger.Send(new XrpSigningInformationMessage(keyPair, address));
      }
      catch (Exception ex)
      {
        DeriviationError = ex;
        Address = null;
        messenger.Send(new XrpSigningInformationMessage(null, null));
        Console.WriteLine($"Error {ex}");
      }
      finally
      {
        IsDerivingKeys = false;
      }
    }

    public void OnNavigatedTo() => ResetState();

    private void ResetState()
    {
      DeriviationError = null;
      Password = null;
      Mnemonic = null;
      Secret = null;
      Address = null;
      messenger.Send(new XrpSigningInformationMessage(null, null));

      AssignKeyDeriviationDisabled();
    }

    public void OnNavigatedFrom()
    { 
    
    }
  }
}