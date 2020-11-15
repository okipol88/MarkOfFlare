using MarkOfFlare.Models;

namespace MarkOfFlare.Messages
{
    public class XrpSigningInformationMessage
    {
        public XrpSigningInformationMessage(KeyPair keyPair, string address)
        {
            KeyPair = keyPair;
            Address = address;
        }

        public KeyPair KeyPair { get;  }
        public string Address { get; }
    }
}
