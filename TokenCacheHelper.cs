﻿using Microsoft.Identity.Client;
using System.IO;
using System.Security.Cryptography;

//No idea whats really going on here I just got it from the microsoft docs
static class TokenCacheHelper {
    public static void EnableSerialization(ITokenCache tokenCache) {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }

    public static readonly string CacheFilePath =
       ".msalcache.bin3";

    private static readonly object FileLock = new object();

    private static void BeforeAccessNotification(TokenCacheNotificationArgs args) {
        lock (FileLock) {
            args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                ? File.ReadAllBytes(CacheFilePath)
                : null);
        }
    }

    private static void AfterAccessNotification(TokenCacheNotificationArgs args) {
        // if the access operation resulted in a cache update
        if (args.HasStateChanged) {
            lock (FileLock) {
                // reflect changes in the persistent store
                File.WriteAllBytes(CacheFilePath,
                    args.TokenCache.SerializeMsalV3());
            }
        }
    }
}