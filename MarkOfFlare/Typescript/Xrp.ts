import * as ripple from 'ripple-keypairs'
import * as bip39 from 'bip39'
const bip32 = require('ripple-bip32')
import * as rippleLib from 'ripple-lib'
import { KeyPair as rippleKeyPair, SignOptions }  from 'ripple-lib/dist/npm/transaction/types'

namespace RippleOnFire
{
    interface IKeyPair {
        public: string,
        private: string
    }

    class XrpKeyPair implements IKeyPair {
        public: string;
        private: string;

        public constructor(pKey: string, privKey: string)
        {
            this.public = pKey;
            this.private = privKey;
        }
    }

    class AccountSign {
        public TransactionType: string = "AccountSet"
        public Account: string
        public Fee: string
        public  Sequence: number 
        public MessageKey: string //"03AB40A0490F9B7ED8DF29D246BF2D6269820A0EE7742ACDD457BEA7C7D0931EDB" // Example given by Flare
    
    }

    class KeyPairDerivitaion {

        public GetMnemonicEntropy(mnemonicWords: Array<string>, password?: string)
        {
           var words = mnemonicWords.join(" ");

            var seed = bip39.mnemonicToSeedSync(words, password).toString('hex');

            return seed;
        }

        public DeriveFromMnemonic(words:string, password?: string): IKeyPair
        {
            var seed = bip39.mnemonicToSeedSync(words, password);

            const m = bip32.fromSeedBuffer(seed);
            const keyPair = m.derivePath("m/44'/144'/0'/0/0").keyPair.getKeyPairs();

            return new XrpKeyPair(keyPair.publicKey, keyPair.privateKey);
        }

        public SignTransaction(keys: IKeyPair, fee: number, sequence: number, messageKey: string): {
            signedTransaction: string;
            id: string;
        } 
        {
            var api = new rippleLib.RippleAPI()
            var tx = new AccountSign()
            tx.Account = this.GetAddress(keys.private)
            tx.Fee = fee.toString()
            tx.Sequence = sequence
            tx.MessageKey = messageKey

            var keypair = {
                privateKey: keys.private,
                publicKey: keys.public
            } as rippleKeyPair;

            console.log(keypair)

            var txJson = JSON.stringify(tx);
            console.log(txJson)

            var options = { signAs: null } as SignOptions;
            var signed = api.sign(txJson, null, options, keypair);

            console.log(signed)

            return signed;
        }

        public Derive(seedhex: string): IKeyPair
        {
            var mySeed = ripple.generateSeed();
            var x = ripple.deriveKeypair(mySeed);
            var keypair = new XrpKeyPair(x.publicKey, x.privateKey);

            console.info(keypair.public);
            console.info(keypair.private);

            return keypair;
        }

        public  GetAddress(publicKey: string) : string
        {
            return ripple.deriveAddress(publicKey);
        }
    }

    export function Load(): void {
        (window as any)['RippleOnFire'] = new KeyPairDerivitaion();
    }

    export function DeriveKeyPair(seed: string): IKeyPair {
        return new KeyPairDerivitaion().Derive(seed);
    } 
}

RippleOnFire.Load();
