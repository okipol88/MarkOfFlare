using System;
using System.Threading.Tasks;

namespace MarkOfFlare.ViewModel
{
    public interface IXrpKeyDeriviationViewModel
    {
        string Mnemonic { get; set; }
        string Password { get; set; }
        string Address { get; }

        Task DeriveKeys();
    }
}