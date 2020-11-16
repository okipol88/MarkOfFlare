using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MarkOfFlare.Interfaces
{
  public enum KeyMode
  { 
    Mnemonic,
    Secret,
    PrivateKey
  }

  public interface IXrpKeyDeriviationViewModel: INavigationViewModel, INotifyPropertyChanged
  {
    KeyMode KeyMode { get; set; }

    string Mnemonic { get; set; }
    string Password { get; set; }

    public string Secret { get; set; }

    string Address { get; }

    bool IsKeyDeriviationDisabled { get; }

    Task DeriveKeys();

    bool IsDerivingKeys { get; }

  }
}