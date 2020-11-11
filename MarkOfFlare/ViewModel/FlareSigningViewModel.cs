using System;
using System.Linq;
using System.Threading.Tasks;
using MarkOfFlare.Messages;
using MarkOfFlare.Models;
using MarkOfFlare.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MvvmBlazor.ViewModel;
// QrCode generation
using QRCoder;

namespace MarkOfFlare.ViewModel
{
    public class FlareSigningViewModel : ViewModelBase, IFlareSigningViewModel
    {
        private string ethereumAddress;
        private string flareMessage;
        private IMessenger messenger;
        private uint fee;
        private uint sequence;
        private SignedTx tx;
        private Exception txException;
        private string base64qrCode;
        private bool isTxSignDisabled = true;
        private string address;
        private KeyPair keyPair;
        private readonly IJSRuntime js;

        public FlareSigningViewModel(IMessenger messenger, IJSRuntime js)
        {
            this.messenger = messenger;
            this.js = js;
        }

        public override void OnInitialized()
        {
            base.OnInitialized();
            messenger.Register<XrpSigningInformationMessage>(OnXrpSigningInformationMessageReceived);
        }

        private void OnXrpSigningInformationMessageReceived(XrpSigningInformationMessage obj)
        {
            keyPair = obj.KeyPair;
            address = obj.Address;
            IsTxSignDisabled = keyPair != null && address != null;

            //messenger.Send(new NavigationAvailabilityMessage(2, !IsTxSignDisabled));
        }

        public string EthereumAddress
        {
            get => ethereumAddress;
            set => Set(ref ethereumAddress, value);
        }

        public string FlareMessage
        {
            get => flareMessage;
            set => Set(ref flareMessage, value);
        }

        public uint Fee
        {
            get => fee;
            set => Set(ref fee, value);
        }

        public uint Sequence
        {
            get => sequence;
            set => Set(ref sequence, value);
        }

        public SignedTx Tx
        {
            get => tx;
            set => Set(ref tx, value);
        }

        public Exception TxException
        {
            get => txException;
            set => Set(ref txException, value);
        }

        public string Base64qrCode
        {
            get => base64qrCode;
            set => Set(ref base64qrCode, value);
        }

        public bool IsTxSignDisabled
        {
            get => isTxSignDisabled;
            set => Set(ref isTxSignDisabled, value);
        }

        public string Address
        {
            get => address;
            set => Set(ref address, value);
        }

        public async Task SignTx()
        {
            try
            {
                FlareMessage = MakeFlareMessage(EthereumAddress);
                Tx = await js.InvokeAsync<SignedTx>("RippleOnFire.SignTransaction", keyPair, Fee, Sequence, FlareMessage);
                TxException = null;
            }
            catch (Exception ex)
            {
                TxException = ex;
                return;
            }
            try
            {
                if (tx?.signedTransaction is string txString)
                {
                    Base64qrCode = GetQrCodeBase64(txString);
                }
            }
            catch (Exception)
            {
                Base64qrCode = "";
            }
        }

        // Creates the flare message as given on Flare site
        private string MakeFlareMessage(string ethereumAddress)
        {
            var without_0x = ethereumAddress.StartsWith("0x") || ethereumAddress.StartsWith("0X") ? ethereumAddress.Remove(0, 2) : ethereumAddress;

            var upperCase = without_0x.ToUpper();

            var targetAddress = "02" + string.Join("", Enumerable.Range(0, 24).Select(x => "0")) + upperCase;

            return targetAddress;
        }


        public string GetQrCodeBase64(string txtQRCode)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData qrCodeData = _qrCode.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return System.Convert.ToBase64String(qrCodeImage);
        }

    }
}

