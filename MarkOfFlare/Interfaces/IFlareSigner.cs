using MarkOfFlare.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkOfFlare.Interfaces
{

  public interface IFlareSigner
  {
    Task<KeyPair> DeriveKeyPair(string mnemonic, string password);
    Task<string> GetAddress(string publicKey);
    Task<bool> IsEthereumAdressValid(string ethereumAddress);
    Task<SignedTx> Sign(KeyPair keyPair, uint fee, uint sequence, string flareMessage);
  }
}
