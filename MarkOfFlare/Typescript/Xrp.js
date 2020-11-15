"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const ripple = __importStar(require("ripple-keypairs"));
const bip39 = __importStar(require("bip39"));
const bip32 = require('ripple-bip32');
const rippleLib = __importStar(require("ripple-lib"));
const web3Utils = require('web3-utils');
const elliptic = __importStar(require("elliptic"));
var RippleOnFire;
(function (RippleOnFire) {
    class XrpKeyPair {
        constructor(pKey, privKey) {
            this.public = pKey;
            this.private = privKey;
        }
    }
    class AccountSign {
        constructor() {
            this.TransactionType = "AccountSet";
        }
    }
    class KeyPairDerivitaion {
        GetMnemonicEntropy(mnemonicWords, password) {
            var words = mnemonicWords.join(" ");
            var seed = bip39.mnemonicToSeedSync(words, password).toString('hex');
            return seed;
        }
        DeriveFromMnemonic(words, password) {
            var seed = bip39.mnemonicToSeedSync(words, password);
            const m = bip32.fromSeedBuffer(seed);
            const keyPair = m.derivePath("m/44'/144'/0'/0/0").keyPair.getKeyPairs();
            return new XrpKeyPair(keyPair.publicKey, keyPair.privateKey);
        }
        SignTransaction(keys, fee, sequence, messageKey) {
            var api = new rippleLib.RippleAPI();
            var tx = new AccountSign();
            tx.Account = this.GetAddress(keys.public);
            tx.Fee = fee.toString();
            tx.Sequence = sequence;
            tx.MessageKey = messageKey;
            console.log("private key " + keys.private + "and public " + keys.public);
            var keypair = {
                privateKey: keys.private,
                publicKey: keys.public
            };
            console.log(keypair);
            var txJson = JSON.stringify(tx);
            console.log(txJson);
            var options = { signAs: null };
            var signed = api.sign(txJson, null, options, keypair);
            console.log(signed);
            return signed;
        }
        DeriveFromSeed(seed) {
            var keys = ripple.deriveKeypair(seed);
            var keypair = new XrpKeyPair(keys.publicKey, keys.privateKey);
            console.info(keypair.public);
            console.info(keypair.private);
            return keypair;
        }
        GetPair(privateKeyHex) {
            var secp256k1 = new elliptic.ec('secp256k1');
            var publicKey = secp256k1.keyFromPrivate(privateKeyHex)
                .getPublic()
                .encodeCompressed();
            var arrayBuff = new Uint8Array(publicKey);
            var buffer = Buffer.from(arrayBuff);
            var keypair = new XrpKeyPair(buffer.toString('hex').toUpperCase(), privateKeyHex);
            console.info(keypair.public);
            console.info(keypair.private);
            return keypair;
        }
        IsValidAddress(ethereumAddress) {
            return web3Utils.isAddress(ethereumAddress);
        }
        GetAddress(publicKey) {
            return ripple.deriveAddress(publicKey);
        }
    }
    function Load() {
        window['RippleOnFire'] = new KeyPairDerivitaion();
    }
    RippleOnFire.Load = Load;
})(RippleOnFire || (RippleOnFire = {}));
RippleOnFire.Load();
//# sourceMappingURL=Xrp.js.map