using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EVN.Core.PMIS
{
    public class AF_A_ASSET_ATT_ITEMService : FX.Data.BaseService<AF_A_ASSET_ATT_ITEM, int>, IAF_A_ASSET_ATT_ITEMService
    {
        private class ASSETIDData
        {
            public string ASSETID { get; set; }
        }
        ILog log = LogManager.GetLogger(typeof(AF_A_ASSET_ATT_ITEMService));
        public AF_A_ASSET_ATT_ITEMService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool DongBoPMIS(YCauNghiemThu yeucau)
        {
            try
            {
                if (yeucau.TrangThai != TrangThaiNghiemThu.HoanThanh || yeucau.DienSinhHoat) return false;
                log.Error($"MaTram: {yeucau.MaTram}");
                var assetIDQuery = "SELECT A.ASSETID from A_ASSET A JOIN VIEW_TT_MBA_FULL B ON A.ASSETID = B.OBJID ";
                assetIDQuery += $" WHERE A.ASSETID_CMIS = '{yeucau.MaTram}' AND A.USESTATUS_LAST_ID='USING'";
                var assetIDData = GetbyQuery<ASSETIDData>(assetIDQuery, false).FirstOrDefault();
                if (assetIDData == null)
                {
                    log.Error($"{assetIDQuery}: NULL");
                    return false;
                }
                log.Error($"ASSETID: {assetIDData.ASSETID}");
                IRepository repository = new FileStoreRepository();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IHSKemTheoService hsktservice = IoC.Resolve<IHSKemTheoService>();
                var listhso = hsoservice.GetbyYeuCau(yeucau.MaDViQLy, yeucau.MaYeuCau);
                var listhsokemtheo = hsktservice.Query.Where(p => p.MaYeuCau == yeucau.MaYeuCau).ToList();
                decimal nextNo = -1;
                foreach (var hso in listhso)
                {
                    if (string.IsNullOrWhiteSpace(hso.Data)) continue;
                    var fileData = repository.GetData(hso.Data);
                    var result = GhiDuLieuPMIS(assetIDData.ASSETID, fileData.Length, Path.GetExtension(hso.Data), fileData, nextNo + 1);
                    if (result == -1)
                        return false;
                    nextNo = result + 1;
                }
                SaveTramBienAp(yeucau.MaTram);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        void SaveTramBienAp(string assetid_cmis)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.AppendFormat("update AF_A_ASSET_ATT_ITEM set assetid = (select ASSETID_PARENT from A_ASSET where assetid_cmis = '{0}' and CATEGORYID like '%MBA') ", assetid_cmis);
                str.AppendLine($"WHERE AFFILEID in (select AFFILEID from AF_A_ASSET_ATT_ITEM WHERE ASSETID = (select ASSETID from A_ASSET where assetid_cmis = '{assetid_cmis}' and CATEGORYID like '%MBA'))");
                ExcuteNonQuery(str.ToString());
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }

        decimal GhiDuLieuPMIS(string assetid, decimal filesize, string fileType, byte[] fileData, decimal nextNo = 0)
        {
            try
            {                
                IAF_A_ASSET_ATT_ITEM_FILEService service = IoC.Resolve<IAF_A_ASSET_ATT_ITEM_FILEService>();

                decimal currentNo = nextNo;
                if (nextNo == 0)
                    nextNo = currentNo = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                string AFFILEID = $"TA-{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{currentNo}{CommonUtils.RandomNumber(1, 99)}";
                log.Error($"{AFFILEID}:{currentNo}");                

                AF_A_ASSET_ATT_ITEM item = new AF_A_ASSET_ATT_ITEM();
                AF_A_ASSET_ATT_ITEM_FILE itemfile = new AF_A_ASSET_ATT_ITEM_FILE();
                item.AFFILEID = AFFILEID;

                item.CRDTIME = DateTime.Now;
                item.UPDDTIME = DateTime.Now;
                item.MDFDTIME = DateTime.Now;

                item.FILESIZEB = filesize;
                item.FILENAME = $"{AFFILEID}.{fileType}";
                item.ASSETID = assetid;
                item.SUMDESC = nextNo.ToString();
                string checkImg = ".bmp,.gif,.jpg,.jpeg,.png,.tif";
                string fileTypeID = "DOC";
                if (checkImg.Contains(fileType.ToLower()))
                {
                    fileTypeID = "IMG";
                }
                BeginTran();
                item.FILETYPEID = fileTypeID;
                CreateNew(item);

                itemfile.AFFILEID = AFFILEID;
                itemfile.FILEDATA = fileData;
                service.CreateNew(itemfile);
                CommitTran();                
                return currentNo;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return -1;
            }
        }
    }
}
