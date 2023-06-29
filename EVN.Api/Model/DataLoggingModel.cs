using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class DataLoggingModel
    {
        public DataLoggingModel() { }
        public DataLoggingModel(DataLogging entity) : base()
        {
            ID = entity.ID;
            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;
            UserID = entity.UserID;
            UserName = entity.UserName;
            ComputerIP = entity.ComputerIP;
            NgayUpdate = entity.NgayUpdate.ToString("dd/MM/yyyy hh:mm:ss");
            ActionType = entity.ActionType;
            SourceType = entity.SourceType;
            DataLoggingCode = entity.DataLoggingCode;
            DataLoggingDetail = entity.DataLoggingDetail;
        }
        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual string NgayUpdate { get; set; } 
        public virtual string UserID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string ComputerIP { get; set; }
        public virtual string ActionType { get; set; }

        public virtual string SourceType { get; set; }

        public virtual string DataLoggingCode { get; set; }

        public virtual string DataLoggingDetail { get; set; }


    }
}