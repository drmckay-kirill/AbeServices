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
            var masterKeyFile = LocalHost.GetRandomFilename();
            var publicKeyFile = LocalHost.GetRandomFilename();

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

        public async Task<ISecretKey> Generate(IMasterKey masterKey, IPublicKey publicKey, IAttributes attributes)
        {
            var masterKeyFile = await LocalHost.WriteFileAsync(masterKey.Value);
            var publicKeyFile = await LocalHost.WriteFileAsync(publicKey.Value);
            var privateKeyFile = LocalHost.GetRandomFilename();

            await LocalHost.RunProcessAsync("cpabe-keygen", $"-o {privateKeyFile} {publicKeyFile} {masterKeyFile} {attributes.Get()}");

            var privateKeyBytes = await LocalHost.ReadFileAsync(privateKeyFile);

            var privateKey = new MockSecretKey() {
                Value = privateKeyBytes
            };

            File.Delete(masterKeyFile);
            File.Delete(publicKeyFile);
            File.Delete(privateKeyFile);

            return privateKey;
        }
    }
}