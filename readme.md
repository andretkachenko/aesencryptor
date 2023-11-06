# AES Encryptor
Small console tool to encrypt/decrypt values using AES algorithm.

## Usage
1. Download the latest release version of the app (see the `Release` section on the right, `AesEncryptor.exe`)
2. Place it in an easy-to-find folder
3. Create `config.json` (you can use any name or keep multiple configurations)
4. Copy schema from [below](#configuration-json-schema)
5. Fill in your values
6. Open `Command Prompt` in your folder from step 2
7. Execute generation with the required configuration: 
    - To encrypt: `AesEncryptor.exe config.json -e {value}`
    - To decrypt: `AesEncryptor.exe config.json -d {value}`

## Configuration JSON schema
``` json
{
    "pwd": "your password",
    "salt": "your salt value",
    "iterations": 1000,
    "hashAlgorithm": "SHA1"
}
```
### Explanation
**Password** - can be any text input you want  
**Salt** - random data used to secure the values better. You can read [wiki](https://en.wikipedia.org/wiki/Salt_(cryptography)) for more info.  
**Iterations** - how many times operations will be executed. Set to 1000 by default.  
**Hash Algorithm** - Set to SHA1 by default. If you know what you're doing, you can choose one of the following:  
- MD5  
- SHA1
- SHA256
- SHA384
- SHA512