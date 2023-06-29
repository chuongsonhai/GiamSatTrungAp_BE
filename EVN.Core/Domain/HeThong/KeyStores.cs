using log4net;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EVN.Core.Domain
{
    public class KeyStores
    {
        public virtual int Id { get; set; }

        public virtual int Type { get; set; } //0: File p12/pfx, 1: HSM

        public virtual string Path { get; set; }

        public virtual string Password { get; set; }

        public virtual string SerialCert { get; set; }
        public virtual string CertData { get; set; }

        public virtual bool IsActive { get; set; } = true;

        private X509Certificate2 cert;

        public virtual X509Certificate2 OpenSession()
        {
            try
            {
                if (Type == 0)
                    cert = new X509Certificate2(Path, Password, X509KeyStorageFlags.Exportable);
                else
                    cert = GetCertificateBySerial(SerialCert);
                return cert;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public virtual void CloseSession()
        {
            cert = null;
        }

        private X509Certificate2 GetCertificateBySerial(String serial)
        {
            ILog log = LogManager.GetLogger(typeof(KeyStores));
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Enumerator enumerator = store.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Certificate2 current = enumerator.Current;
                    if (current.GetSerialNumberString().ToUpper().Equals(serial.ToUpper()))
                    {
                        try
                        {
                            AsymmetricAlgorithm aa = current.PrivateKey;
                            return current;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            throw new Exception("Không lấy được private key, chọn chứng thư khác!");
                        }
                    }
                }
                return null;
            }
            finally { store.Close(); }
        }
    }
}
