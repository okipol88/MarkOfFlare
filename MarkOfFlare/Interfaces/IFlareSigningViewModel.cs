using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using MarkOfFlare.Models;

namespace MarkOfFlare.Interfaces
{
  public interface IFlareSigningViewModel : INotifyPropertyChanged
  {
    string EthereumAddress { get; set; }
    string FlareMessage { get; }
    uint? Fee { get; set; }
    uint? Sequence { get; set; }
    SignedTx Tx { get; }
    Exception TxException { get; }
    string Base64qrCode { get; }
    bool IsTxSignDisabled { get; }
    bool IsSigning { get; }
    string Address { get; }
    bool IsEthereumAddressValid { get; }

    Task SignTx();
  }
}