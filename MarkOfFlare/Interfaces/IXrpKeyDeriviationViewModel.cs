using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MarkOfFlare.Interfaces
{
  public interface IXrpKeyDeriviationViewModel: INotifyPropertyChanged
  {
    string Mnemonic { get; set; }
    string Password { get; set; }
    string Address { get; }

    bool IsKeyDeriviationDisabled { get; }

    Task DeriveKeys();
  }
}