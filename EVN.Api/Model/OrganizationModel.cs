using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class OrganizationModel
    {
        public OrganizationModel(Organization entity)
        {
            orgId = entity.orgId;
            orgCode = entity.orgCode;
            compCode = entity.compCode;
            orgName = entity.orgName;
            parentCode = entity.parentCode;
            address = entity.address;
            phone = entity.phone;
            email = entity.email;
            daiDien = entity.daiDien;
            chucVu = entity.chucVu;
            taxCode = entity.taxCode;
            soTaiKhoan = entity.soTaiKhoan;
            nganHang = entity.nganHang;
        }
        public long orgId { get; set; }
        public string orgCode { get; set; }
        public string compCode { get; set; }
        public string orgName { get; set; }
        public string parentCode { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string daiDien { get; set; }
        public string chucVu { get; set; }
        public string taxCode { get; set; }
        public string soTaiKhoan { get; set; }
        public string nganHang { get; set; }
    }
}