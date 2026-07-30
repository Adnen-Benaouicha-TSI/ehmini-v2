using Ehmini.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Ehmini.Infrastructure.Persistence.Services
{
    public class PkceService : IPkceService
    {
        public bool ValidateCodeVerifier(string codeVerifier, string codeChallenge, string method = "S256")
        {
            if (string.IsNullOrWhiteSpace(codeVerifier) || string.IsNullOrWhiteSpace(codeChallenge))
                return false;

            if (method.ToUpperInvariant() != "S256")
            {
                // La norme OAuth 2.1 impose S256 (SHA-256). "plain" n'est plus recommandé.
                return false;
            }

            // 1. Calcul du SHA-256 du code_verifier
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

            // 2. Encodage en Base64Url (sans padding '=', avec '-' et '_')
            var computedChallenge = Base64UrlEncode(challengeBytes);

            // 3. Comparaison sécurisée (Constante en temps)
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedChallenge),
                Encoding.UTF8.GetBytes(codeChallenge)
            );
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            return base64
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
