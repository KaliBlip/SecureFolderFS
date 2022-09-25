﻿using System;
using System.IO;
using System.Linq;

namespace SecureFolderFS.Core.Helpers
{
    internal static class VaultHelpers
    {
        public static char GetUnusedMountLetter()
        {
            var occupiedLetters = Directory.GetLogicalDrives().Select(item => item[0]);
            var availableLetters = Constants.ALPHABET.ToCharArray().Skip(3).Except(occupiedLetters); // Skip C and A, B - these are reserved for floppy disks and should not be used

            // Return first or default letter
            return availableLetters.FirstOrDefault(defaultValue: char.MinValue);
        }

        public static Stream FindVaultFile<TVaultFileDiscoverer>(
            Func<TVaultFileDiscoverer> builtinFileDiscovererInitializationCallback,
            Func<TVaultFileDiscoverer, Stream> openStreamToVaultFile,
            bool useExternalDiscoverer = false,
            TVaultFileDiscoverer? vaultFileDiscoverer = null) where TVaultFileDiscoverer : class
        {
            bool usedDefaultDiscoverer = true;
            TVaultFileDiscoverer appropriateDiscoverer;

            if (useExternalDiscoverer)
            {
                ArgumentNullException.ThrowIfNull(vaultFileDiscoverer);

                usedDefaultDiscoverer = false;
                appropriateDiscoverer = vaultFileDiscoverer;
            }
            else
            {
                appropriateDiscoverer = builtinFileDiscovererInitializationCallback();
            }

            Stream vaultFileStream;
            try
            {
                vaultFileStream = openStreamToVaultFile(appropriateDiscoverer);
            }
            catch
            {
                if (usedDefaultDiscoverer && vaultFileDiscoverer is not null)
                {
                    vaultFileStream = openStreamToVaultFile(appropriateDiscoverer);
                }
                else
                {
                    throw;
                }
            }

            return vaultFileStream;
        }
    }
}
