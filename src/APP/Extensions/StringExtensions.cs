using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace APP.Extensions;

public static partial class StringExtensions
{
     public static string Capitalize(this string text)
     { 
         return string.Join(" ", text.Split(' ').ToList()
             .ConvertAll(word => 
                 word[..1].ToUpper() + word[1..])
         );
     }

    public static string RemoveCharacter(this string text, char character) 
    { 
        return text.Trim().Replace(character, ' ');
    }
    
    public static string ReplaceWhitespace(this string input, string replacement = "_") 
    { 
        var sWhitespace = MyRegex(); 
        return sWhitespace.Replace(input, replacement);
    }
    
    public static string EncryptStringToBytes_Aes(this string plainText, byte[] key, byte[] iv) 
    { 
        // Check arguments.
        if (plainText is not { Length: > 0 }) 
            throw new ArgumentNullException(nameof(plainText));
        if (key is not { Length: > 0 })
                throw new ArgumentNullException(nameof(key));
        if (iv is not { Length: > 0 })
            throw new ArgumentNullException(nameof(iv));
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            // Create an encryptor to perform the stream transform.
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return Convert.ToBase64String(encrypted);
    }


    public static string DecryptStringFromBytes_Aes(this string cipherText, byte[] key, byte[] iv) 
    { 
        if (double.TryParse(cipherText, out _)) return cipherText; 
        // Check arguments.
        if (cipherText is not { Length: > 0 }) 
            throw new ArgumentNullException(nameof(cipherText));
        if (key is not { Length: > 0 })
            throw new ArgumentNullException(nameof(key));
        if (iv is not { Length: > 0 })
            throw new ArgumentNullException(nameof(iv));

        // Declare the string used to hold
        // the decrypted text.
        var plaintext = cipherText;

        // Create an Aes object
        // with the specified key and IV.
        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        // Create a decryptor to perform the stream transform.
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for decryption.
        try
        {
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            plaintext = srDecrypt.ReadToEnd();
        }
        catch (Exception)
        {
            // ignored
        }

        return plaintext;
    }
        
    public static IFormFile ConvertFromBase64(this string base64String) 
    { 
        var parts = base64String.Split(",");
        
        var contentTypeAndExtension = parts[0]; 
        var contentType = contentTypeAndExtension.Split(';')[0]["data:".Length..]; 
        var extension = contentType.Split("/")[1];
        
        var base64Data = parts[1];
        var imageData = Convert.FromBase64String(base64Data);
        var id =  Guid.NewGuid().ToString();
        var filename = $"{id}.{extension}";
        var memoryStream = new MemoryStream(imageData); 
        var file = new FormFile(memoryStream, 0, memoryStream.Length,id, filename) 
        { 
            Headers = new HeaderDictionary(), 
            ContentType = contentType
        };
        
        return file;
    }
    
    public static bool IsValidBase64String(this string base64String) 
    { 
        var parts = base64String.Split(","); 
        return parts.Length == 2;
    }
    
    public static async Task<string> ShortenUrl(this string url) 
    { 
        try 
        {
            var apiKey =  Environment.GetEnvironmentVariable("bitlyToken");
            
            var client = new HttpClient();
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.tinyurl.com/create?api_token={apiKey}");
            
            request.Headers.Add("Authorization", $"Bearer {apiKey}"); 
            var body = new 
            { 
                url
            };
            
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.SendAsync(request).ConfigureAwait(false);
            
            var responseStr = await response.Content.ReadAsStringAsync();
            
            var jsonResponse = JsonSerializer.Deserialize<dynamic>(responseStr); 
            return jsonResponse["data"]["tiny_url"];
        }
        catch (Exception)
        {
            return url;
        }
    }
        
    public static string SeparateByCapitalization(this string input)
    {
        const string pattern = @"(?<!^)(?=[A-Z])|(?<=[a-z])(?=[A-Z][a-z])";
        var words = Regex.Split(input, pattern);
        return string.Join(" ", words);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();
}