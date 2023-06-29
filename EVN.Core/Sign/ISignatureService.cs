using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public interface ISignatureService
    {
        SignResponseData KyGiamDoc(string tenNguoiKy,  string chucVuNguoiKy, byte[] pdfdata);

        SignResponseData KyNoiBo(string serial, string chucVuNguoiKy, byte[] pdfdata);

        SignResponseData DSachNVien(string maDVi, string tenNguoiKy);
    }
}
