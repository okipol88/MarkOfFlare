using System;
namespace MarkOfFlare.Models
{
    public class SignedTx
    {
        public string signedTransaction { get; set; }
        public string id { get; set; }

        public override string ToString()
        {
            return $"Signed transaction: {signedTransaction}{Environment.NewLine}id: {id}";
        }
    }
}
