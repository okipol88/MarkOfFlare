using System;
using System.Threading.Tasks;
using MarkOfFlare.Models;

namespace MarkOfFlare.ViewModel
{
    public interface IFlareSigningViewModel
    {
        string EthereumAddress { get; set; }
        string FlareMessage { get; }
        uint Fee { get; set; }
        uint Sequence { get; set; }
        SignedTx Tx { get;}
        Exception TxException { get; }
        string Base64qrCode { get; }
        bool IsTxSignDisabled { get; }
        string Address { get; }

        Task SignTx();
    }
}