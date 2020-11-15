using MarkOfFlare.Interfaces;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MarkOfFlare.Models
{
  class FlareSigner : IFlareSigner
  {
    private readonly IJSRuntime js;

    public FlareSigner(IJSRuntime js)
    {
      this.js = js;
    }

    public Task<SignedTx> Sign(KeyPair keyPair, uint fee, uint sequence, string flareMessage) =>
      Task.Run(async () => await js.InvokeAsync<SignedTx>("RippleOnFire.SignTransaction", keyPair, fee, sequence, flareMessage));

    public Task<KeyPair> DeriveKeyPair(string mnemonic, string password) =>
      Task.Run(async () => await js.InvokeAsync<KeyPair>("RippleOnFire.DeriveFromMnemonic",
          mnemonic, password));

    public Task<string> GetAddress(string publicKey) =>
      Task.Run(async () => await js.InvokeAsync<string>("RippleOnFire.GetAddress", publicKey));

    public Task<bool> IsEthereumAdressValid(string ethereumAddress) =>
      Task.Run(async () => await js.InvokeAsync<bool>("RippleOnFire.IsValidAddress", ethereumAddress)); 

    public Task<KeyPair> DeriveFromSeed(string seed) =>
      Task.Run(async () => await js.InvokeAsync<KeyPair>("RippleOnFire.DeriveFromSeed", seed));
  }
}
