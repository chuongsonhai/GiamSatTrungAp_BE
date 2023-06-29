namespace EVN.Api.Model
{
    public class ApproveModel
    {
        public int id { get; set; }
        public string deptId { get; set; }
        public string staffCode { get; set; }
        public string ngayHen { get; set; }
        public string noiDung { get; set; }
        public string maCViec { get; set; }
    }

    public class ChuyenTiepRequest
    {
        public string maYCau { get; set; }
        public string maDViTNhan { get; set; }        
    }

    public class CancelModel
    {
        public string maYCau { get; set; }
        public string noiDung { get; set; }
    }
}
