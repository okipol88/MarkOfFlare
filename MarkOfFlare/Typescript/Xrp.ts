import * as ripple from 'ripple-keypairs'
import * as bip39 from 'bip39'
const bip32 = require('ripple-bip32')
import * as rippleLib from 'ripple-lib'
import { KeyPair as rippleKeyPair, SignOptions } from 'ripple-lib/dist/npm/transaction/types'

const web3Utils = require('web3-utils')
//import { isAddress as isEthereumAddressValid } from 'web3-utils'

import * as elliptic from 'elliptic'

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
            tx.Account = this.GetAddress(keys.public)
            tx.Fee = fee.toString()
            tx.Sequence = sequence
            tx.MessageKey = messageKey

            console.log("private key " + keys.private + "and public " + keys.public)

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

        public DeriveFromSeed(seed: string): IKeyPair
        {
            var keys = ripple.deriveKeypair(seed);
            var keypair = new XrpKeyPair(keys.publicKey, keys.privateKey);

            console.info(keypair.public);
            console.info(keypair.private);

            return keypair;
        }

        public GetPair(privateKeyHex: string): IKeyPair
        {
            var secp256k1 = new elliptic.ec('secp256k1')
            var publicKey = secp256k1.keyFromPrivate(privateKeyHex)
                .getPublic()
                .encodeCompressed();

            var arrayBuff = new Uint8Array(publicKey)
            var buffer = Buffer.from(arrayBuff);

            var keypair = new XrpKeyPair(buffer.toString('hex').toUpperCase(), privateKeyHex);

            console.info(keypair.public);
            console.info(keypair.private);

            return keypair;
        }

        public IsValidAddress(ethereumAddress: string): boolean {
            return web3Utils.isAddress(ethereumAddress);
            //return isEthereumAddressValid(ethereumAddress) // TODO: This caused a copmilation error for 
            //Severity	Code	Description	Project	File	Line	Suppression State
            //Error	TS7016	Build: Could not find a declaration file for module 'bn.js'. '/node_modules/bn.js/lib/bn.js' implicitly has an 'any' type.MarkOfFlare	\MarkOfFlare\node_modules\web3 - utils\types\index.d.ts	23	

        }

        public  GetAddress(publicKey: string) : string
        {
            return ripple.deriveAddress(publicKey);
        }
    }

    export function Load(): void {
        (window as any)['RippleOnFire'] = new KeyPairDerivitaion();
    }
}

RippleOnFire.Load();
