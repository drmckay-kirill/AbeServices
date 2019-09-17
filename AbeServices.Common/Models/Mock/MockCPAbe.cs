using System;
using System.Threading.Tasks;
using AbeServices.Common.Helpers;
using System.IO;
using AbeServices.Common.Models.Base;

namespace AbeServices.Common.Models.Mock
{
    public class MockCPAbe
    {
        public async Task<SetupResult> Setup()
        {
            var masterKeyFile = LocalHost.RandomFilename();
            var publicKeyFile = LocalHost.RandomFilename();

            await LocalHost.RunProcessAsync("cpabe-setup", $"-p {publicKeyFile} -m {masterKeyFile}");

            var masterKeyBytes = await LocalHost.ReadFileAsync(masterKeyFile);
            var publicKeyBytes = await LocalHost.ReadFileAsync(publicKeyFile);

            var setupResult = new SetupResult() {
                MasterKey = new MockMasterKey() {
                    Value = masterKeyBytes
                },
                PublicKey = new MockPublicKey() {
                    Value = publicKeyBytes
                }
            };

            File.Delete(masterKeyFile);
            File.Delete(publicKeyFile);

            return setupResult;
        }
    }
}