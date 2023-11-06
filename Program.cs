﻿using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

// validate all arguments supplied
// expected: {path to config} {-e/d} {string}
if (args.Length < 3) return;

var jsonPath = args[0];
var method = args[1];
var value = args[2];

// validate method and value
if (method != "-e" && method != "d") return;
if (string.IsNullOrEmpty(value)) return;

var config = ReadConfig(jsonPath);
var crypter = Create(config);
Func<Aes, string, string> cryptoMethod = method == "-e" ? Encrypt : Decrypt;
var result = cryptoMethod(crypter, value);

Console.WriteLine("Your result:");
Console.WriteLine(result);

static Config ReadConfig(string path)
{
    Config config;

    // Read configuration json
    using (StreamReader r = new StreamReader(path))
    {
        string json = r.ReadToEnd();
        var serialized = JsonConvert.DeserializeObject<Config>(json);
        if (serialized == null) throw new ArgumentException("invalid json");
        config = serialized;
    }

    return config;
}

static Aes Create(Config config)
{
    var rfc2898DeriveBytes = new Rfc2898DeriveBytes(config.Password, Encoding.ASCII.GetBytes(config.Salt), config.Iterations, config.HashAlgorithm);
    var cryptoObject = Aes.Create();
    cryptoObject.Key = rfc2898DeriveBytes.GetBytes(cryptoObject.KeySize / 8);
    cryptoObject.IV = rfc2898DeriveBytes.GetBytes(cryptoObject.BlockSize / 8);

    return cryptoObject;
}

static string Decrypt(Aes crypter, string value)
{
    var transform = crypter.CreateDecryptor(crypter.Key, crypter.IV);
    using MemoryStream memoryStream = new(Convert.FromBase64String(value));
    using CryptoStream cryptoStream = new(memoryStream, transform, CryptoStreamMode.Read);
    using StreamReader streamReader = new(cryptoStream);
    return streamReader.ReadToEnd();
}

static string Encrypt(Aes crypter, string value)
{
    var transform = crypter.CreateEncryptor(crypter.Key, crypter.IV);
    byte[] bytes = Encoding.UTF8.GetBytes(value);
    using MemoryStream memoryStream = new();
    using CryptoStream cryptoStream = new(memoryStream, transform, CryptoStreamMode.Write);
    cryptoStream.Write(bytes, 0, bytes.Length);
    cryptoStream.FlushFinalBlock();
    return Convert.ToBase64String(memoryStream.ToArray());
}

class Config
{
    [JsonProperty("pwd")]
    public required string Password { get; set; }

    [JsonProperty("salt")]
    public required string Salt { get; set; }

    [JsonProperty("iterations")]
    public required int Iterations { get; set; }

    [JsonProperty("hashAlgorithm")]
    public required HashAlgorithmName HashAlgorithm { get => _hashAlgorithm; set => _hashAlgorithm = new HashAlgorithmName(value.ToString()); }

    internal required HashAlgorithmName _hashAlgorithm;

    public Config()
    {
        Password = string.Empty;
        Salt = string.Empty;
        Iterations = 1000;
        _hashAlgorithm = HashAlgorithmName.SHA1;
    }
}