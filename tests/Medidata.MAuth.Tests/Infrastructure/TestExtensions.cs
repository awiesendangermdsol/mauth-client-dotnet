﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Medidata.MAuth.Core;

namespace Medidata.MAuth.Tests.Infrastructure
{
    internal static class TestExtensions
    {
        public static readonly Uri TestUri = new Uri("http://localhost");
        public static readonly Guid ClientUuid = new Guid("192cce84-8466-490e-b03e-074f82da3ee2");
        public static readonly Guid ServerUuid = new Guid("beec1db9-6f33-4d75-878b-28a6d14531e1");
        public static readonly string ClientPrivateKey = GetKeyFromResource(nameof(ClientPrivateKey)).Result;
        public static readonly string ClientPublicKey = GetKeyFromResource(nameof(ClientPublicKey)).Result;
        public static readonly string ServerPrivateKey = GetKeyFromResource(nameof(ServerPrivateKey)).Result;
        public static readonly string ServerPublicKey = GetKeyFromResource(nameof(ServerPublicKey)).Result;

        public static MAuthOptionsBase ServerOptions => new MAuthTestOptions()
        {
            ApplicationUuid = ServerUuid,
            MAuthServiceUrl = TestUri,
            PrivateKey = ServerPrivateKey,
            MAuthServiceRetryPolicy = MAuthServiceRetryPolicy.RetryOnce,
            MAuthServerHandler = new MAuthServerHandler()
        };

        public static MAuthSigningOptions ClientOptions(DateTimeOffset signedTime) => new MAuthSigningOptions()
        {
            ApplicationUuid = ClientUuid,
            PrivateKey = ClientPrivateKey,
            SignedTime = signedTime
        };

        public static MAuthOptionsBase GetServerOptionsWithAttempts(MAuthServiceRetryPolicy policy,
            bool shouldSucceedWithin) =>
            new MAuthTestOptions()
            {
                ApplicationUuid = ServerUuid,
                MAuthServiceUrl = TestUri,
                PrivateKey = ServerPrivateKey,
                MAuthServiceRetryPolicy = policy,
                MAuthServerHandler = new MAuthServerHandler()
                {
                    SucceedAfterThisManyAttempts = (int)policy + (shouldSucceedWithin ? 1 : 2)
                }
            };

        public static Task<string> GetStringFromResource(string resourceName)
        {
            var assembly = typeof(TestExtensions).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        private static Task<string> GetKeyFromResource(string keyName)
        {
            return GetStringFromResource($"Medidata.MAuth.Tests.Mocks.Keys.{keyName}.pem");
        }
    }
}
