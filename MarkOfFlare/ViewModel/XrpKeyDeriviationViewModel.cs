using System;
using System.Threading.Tasks;
using MarkOfFlare.Messages;
using MarkOfFlare.Models;
using MarkOfFlare.Services;
using Microsoft.JSInterop;
using MvvmBlazor.ViewModel;

namespace MarkOfFlare.ViewModel
{

    public class XrpKeyDeriviationViewModel : ViewModelBase, IXrpKeyDeriviationViewModel
    {
        private readonly IMessenger messenger;
        private readonly IJSRuntime js;

        public XrpKeyDeriviationViewModel(IMessenger messenger, IJSRuntime js)
        {
            this.messenger = messenger;
            this.js = js;
        }

        private string mnemonic;
        private string password;
        private string address;
        private Exception deriviationError;

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

        public async Task DeriveKeys()
        {
            try
            {
                // "attend dinner chat movie brain invite forest quiz bulb taste evidence danger"
                var keyPair = await js.InvokeAsync<KeyPair>("RippleOnFire.DeriveFromMnemonic",
                  mnemonic, password);
                Address = await js.InvokeAsync<string>("RippleOnFire.GetAddress", keyPair.@public);
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