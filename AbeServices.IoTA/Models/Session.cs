using System;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.IoTA.Models
{
    public class Session
    {
        public Guid Id { get; set;}
        public DateTime CreateDate { get; set; }

        public AbeAuthSteps ProtocolStep { get; set; }
        
        public byte[] Z { get; set; }

        public byte[] SharedKey { get; set; }

        public Session()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
            ProtocolStep = AbeAuthSteps.GetAccessPolicy;
        }

        public Session SetStep(AbeAuthSteps step)
        {
            ProtocolStep = step;
            return this;
        }
    }
}