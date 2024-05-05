import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CryptingService {

  private key = CryptoJS.enc.Utf8.parse(environment.cryptoSecretKey);
  private iv = CryptoJS.enc.Utf8.parse("");

  constructor() { }

  decryptText(encString: string) {

    var decrypted = CryptoJS.AES.decrypt(encString, this.key, {
      keySize: 128 / 8,
      iv: this.iv,
      // mode: CryptoJS.mode.CBC,
      // padding: CryptoJS.pad.Pkcs7
    });
    
    return decrypted.toString(CryptoJS.enc.Utf8);
  }
}
