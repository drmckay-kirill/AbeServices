using System;
using System.Threading.Tasks;
using System.Text;
using AbeServices.Common.Helpers;
using System.IO;
using AbeServices.Common.Models.Base;
using AbeServices.Common.Exceptions;

namespace AbeServices.Common.Models.Mock
{
    public class MockCPAbe: ICPAbe
    {
        public async Task<SetupResult> Setup()
        {
            try
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
            catch (Exception exception)
            {
                throw new ABESchemeException("Error has occured during initialization", exception);
            }
        }

        public async Task<ISecretKey> Generate(IMasterKey masterKey, IPublicKey publicKey, IAttributes attributes)
        {
            try
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
            catch (Exception exception)
            {
                throw new ABESchemeException($"Error has occured during secret key generation: {attributes.Get()}", exception);
            }
        }

        public async Task<ICipherText> Encrypt(byte[] data, IPublicKey publicKey, IAccessPolicy accessPolicy)
        {
            try
            {
                var publicKeyFile = await LocalHost.WriteFileAsync(publicKey.Value);
                var messageFile = await LocalHost.WriteFileAsync(data);
                var encodedDataFile = LocalHost.GetRandomFilename();

                await LocalHost.RunProcessAsync("cpabe-enc", $"-o {encodedDataFile} {publicKeyFile} {messageFile} \"{accessPolicy.AndGate()}\"");

                var encodedDataBytes = await LocalHost.ReadFileAsync(encodedDataFile);

                var encodedData = new MockCipherText() {
                    Value = encodedDataBytes
                };

                File.Delete(publicKeyFile);
                File.Delete(messageFile);
                File.Delete(encodedDataFile);

                return encodedData;
            }
            catch (Exception exception)
            {
                throw new ABESchemeException($"Error has occured during encryption: {accessPolicy.AndGate()}", exception);
            }
        }

        public async Task<ICipherText> Encrypt(string message, IPublicKey publicKey, IAccessPolicy accessPolicy)
        {
            return await Encrypt(Encoding.UTF8.GetBytes(message), publicKey, accessPolicy);
        }

        public async Task<string> Decrypt (ICipherText cipherText, IPublicKey publicKey, ISecretKey secretKey)
        {
            try
            {
                var cipherTextFile = await LocalHost.WriteFileAsync(cipherText.Value);
                var publicKeyFile = await LocalHost.WriteFileAsync(publicKey.Value);
                var secretKeyFile = await LocalHost.WriteFileAsync(secretKey.Value);
                var messageFile = LocalHost.GetRandomFilename();

                await LocalHost.RunProcessAsync("cpabe-dec", $"-o {messageFile} {publicKeyFile} {secretKeyFile} {cipherTextFile}");

                var messageBytes = await LocalHost.ReadFileAsync(messageFile);

                File.Delete(publicKeyFile);
                File.Delete(secretKeyFile);

                return Encoding.UTF8.GetString(messageBytes);
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("cannot decrypt"))
                    throw new UnathorizedAttributesAccessException();
                throw new ABESchemeException("Error has occured during decryption", exception);
            }
        }
    }
}