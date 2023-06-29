namespace EVN.Api.Model
{
    public class SignModel
    {
        public int id { get; set; } = 0;
        public string binary_string { get; set; }
    }

    public class SignRemoteModel 
    {
        public int id { get; set; }
        public string deptId { get; set; }
        public string staffCode { get; set; }
        public string ngayHen { get; set; }
        public string noiDung { get; set; }
        public string maCViec { get; set; }
        public string binary_string { get; set; }
    }
}
