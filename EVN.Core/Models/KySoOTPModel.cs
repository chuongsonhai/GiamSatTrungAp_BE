using EVN.Core.IServices;
using FX.Core;
using Newtonsoft.Json;

namespace EVN.Core
{
    public class CreateAndSendOtpCmisCommand
    {
       

        public CreateAndSendOtpCmisCommand(string maDonViQuanLy, string noiDung, string soDienThoai, string _extraInfo = "5")
        {
            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var doConfigs = cfgservice.GetDictionary("passKySoOTP");
            string passKySoOTP = doConfigs["passKySoOTP"];

            brandname = "EVNHANOI";
            type = 1;
            appCode = "app1";
            password = passKySoOTP;
            systemKey = "RVZOSUNUMTFDVUFCQUM=";
            token = "abcxyz";
            extraInfo = _extraInfo;
            regionCode = maDonViQuanLy.ToUpper().Trim();
            content = noiDung;
            userId = soDienThoai;
        }
        public string brandname { get; set; }
        public int type { get; set; }
        public string content { get; set; }
        public string regionCode { get; set; }
        public string appCode { get; set; }
        public string password { get; set; }
        public string systemKey { get; set; }
        public string userId { get; set; }
        public string token { get; set; }
        public string extraInfo { get; set; }

    }

    public class JsonUtilSerializer
    {
        /// To the json string. /// 
        /// The object to serialize.
        /// if set to true [use camel case].
        /// if set to true [ignore nulls].
        /// if set to true [indent json].
        /// 
        public static string ToJsonString(object objectToSerialize, bool ignoreNulls = true, bool indentJson = false)
        {
            var settings = new JsonSerializerSettings();

            if (ignoreNulls)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
            }

            if (indentJson)
            {
                settings.Formatting = Formatting.Indented;
            }

            return JsonConvert.SerializeObject(objectToSerialize, settings);
        }

        public static T FromJsonString<T>(string json)
        {
            var settings = new JsonSerializerSettings();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

    }

    public class BaseOtpWebServiceResultDto
    {
        public string Key { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
    public class VerifyOtpCmisCommand
    {
        public VerifyOtpCmisCommand(string sodt, string otptoken, string maxacnhan, string madonvi)
        {

            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var doConfigs = cfgservice.GetDictionary("passKySoOTP");
            string passKySoOTP = doConfigs["passKySoOTP"];

            appCode = "app1";
            password = passKySoOTP;
            userId = sodt;
            token = otptoken;
            totp = maxacnhan;
            //extraInfo = madonvi.ToUpper().Trim();
        }
        public string appCode { get; set; }
        public string password { get; set; }
        public string userId { get; set; }
        public string token { get; set; }
        public string totp { get; set; }
        public string extraInfo { get; set; }

    }

    public class Constants
    {
        public static string DefaultCorsPolicy = nameof(DefaultCorsPolicy);
        public const string CmisApiClientName = "cmisApiClient";
        public const string KySoTrungApApiClientName = "kySoTrungApApiClientName";
        public const string CmisTestApiClientName = "cmisTestApiClientName";
        public const string DvcClientName = "vcApiClient";
        public const string OtpApiClientName = "otpApiClient";
        public const string CRMApiClientName = "crmApiClient";
        public const string ThanhToanApiClientName = "thanhToanApiClient";
        public const string EvnCmsApiClientName = "evnCmsApiClient";
        public const string CccdApiClientName = "cccdApiClient";
        public const string TrungApApiClientName = "trungApApiClientName";

        public const string DefaultDateFormat = "dd/MM/yyyy";
        public const string DateAndTimeFormat = "dd/MM/yyyy hh:mm:ss";
        public const string DefaultDateFormatFromDb = "dd'/'MM'/'yyyy";
        public const string DefaultCultureName = "vi-VN";

        public const string PasswordPolicyErrorClaim = "passwordPolicyError";
        public const string ErrorCodeClaim = "errorCode";

        public const string ErrorCodePasswordExpired = "pwd_expired";
        public const string ErrorCodePasswordWeak = "pwd_weak";

        public const string KeyNgayLe = "NGAY_LE";
    }

    public class InsertOtpAndTokenCommand
    {
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public string Token { get; set; }
    }
    public class ApiResult
    {
        public string Data { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Code { get; set; }
    }
    public class KySoOTPTrungApCommand 
    {
        public string MaYeuCau { get; set; }
        public string NguoiKy { get; set; }
        public string SoDienThoai { get; set; }
        public string OTP { get; set; }
        public string TuKhoa { get; set; }
        public string Base64File { get; set; }

    }

    public class KySoOTPTrungApResultDto
    {
        public int Code { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}