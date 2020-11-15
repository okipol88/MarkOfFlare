using System;
using System.Linq;
using System.Threading.Tasks;
using MarkOfFlare.Interfaces;
using MarkOfFlare.Messages;
using MarkOfFlare.Models;
using MarkOfFlare.Services;
using MvvmBlazor.ViewModel;
using QRCoder;

namespace MarkOfFlare.ViewModel
{
  public class FlareSigningViewModel : ViewModelBase, IFlareSigningViewModel
  {
    private string ethereumAddress;
    private string flareMessage;
    private IMessenger messenger;
    private uint? fee;
    private uint? sequence;
    private SignedTx tx;
    private Exception txException;
    private string base64qrCode;
    private bool isTxSignDisabled = true;
    private string address;
    private KeyPair keyPair;
    private bool isEthereumAddressValid;
    private bool isSigning;
    private readonly IFlareSigner flareSigner;

    public FlareSigningViewModel(IMessenger messenger, IFlareSigner flareSigner)
    {
      this.messenger = messenger;
      this.flareSigner = flareSigner;
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

      var canNavigate = keyPair != null && address != null;
      messenger.Send(new NavigationAvailabilityMessage(2, canNavigate));

      UpdateTxSignEnabled();
    }

    public string EthereumAddress
    {
      get => ethereumAddress;
      set 
      {
        if (Set(ref ethereumAddress, value?.Trim()))
        {
          ValidateEthereumAddress(ethereumAddress);
        }
      }
    }

    private void ValidateEthereumAddress(string value)
    {
      try
      {
        flareSigner.IsEthereumAdressValid(value)
          .ContinueWith(x =>
          {
            if (ethereumAddress == value)
            {
              try
              {
                IsEthereumAddressValid = x.Result;
              }
              catch (Exception ex)
              {
                Console.WriteLine($"Error occured in validation of ethereum address {ex}");
                IsEthereumAddressValid = false;
              }
            }
          });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error occured in validation of ethereum address {ex}");
        IsEthereumAddressValid = false; // asssume false on error
      }
    }

    public bool IsEthereumAddressValid
    {
      get => isEthereumAddressValid;
      set => Set(ref isEthereumAddressValid, value);
    }

    public string FlareMessage
    {
      get => flareMessage;
      set => Set(ref flareMessage, value);
    }

    public uint? Fee
    {
      get => fee;
      set => Set(ref fee, value);
    }

    public uint? Sequence
    {
      get => sequence;
      set => Set(ref sequence, value);
    }

    public override void OnPropertyChanged(string propertyName)
    {
      base.OnPropertyChanged(propertyName);

      if (propertyName == nameof(Fee)
        || propertyName == nameof(Sequence)
        || propertyName == nameof(EthereumAddress))
      {
        UpdateTxSignEnabled();
      }
    }

    private void UpdateTxSignEnabled()
    {
      IsTxSignDisabled = !Fee.HasValue
        || !Sequence.HasValue
        || string.IsNullOrEmpty(EthereumAddress)
        || keyPair == null 
        || address == null;
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
    public bool IsSigning
    {
      get => isSigning;
      set => Set(ref isSigning, value);
    }

    public async Task SignTx()
    {
      if (IsSigning)
      {
        return;
      }
      IsSigning = true;

      await Task.Run(async () =>
      {
        try
        {
          // the signing takes time but usually this somehow does not work without a delay ???: TODO Investigate
          await Task.Delay(TimeSpan.FromMilliseconds(100));

          FlareMessage = MakeFlareMessage(EthereumAddress);
          Tx = await flareSigner.Sign(keyPair, Fee.Value, Sequence.Value, FlareMessage);
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
      });

      IsSigning = false;
    }

    // Creates the flare message as given on Flare site
    public static string MakeFlareMessage(string ethereumAddress)
    {
      var without_0x = ethereumAddress.StartsWith("0x") || ethereumAddress.StartsWith("0X") ? ethereumAddress.Remove(0, 2) : ethereumAddress;

      var upperCase = without_0x.ToUpper();

      var targetAddress = "02" + string.Join("", Enumerable.Range(0, 24).Select(x => "0")) + upperCase;

      return targetAddress;
    }


    public static string GetQrCodeBase64(string txtQRCode)
    {
      QRCodeGenerator _qrCode = new QRCodeGenerator();
      QRCodeData qrCodeData = _qrCode.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
      PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
      var qrCodeImage = qrCode.GetGraphic(20);
      return System.Convert.ToBase64String(qrCodeImage);
    }
  }
}

