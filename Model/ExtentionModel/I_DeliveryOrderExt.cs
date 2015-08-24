using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class I_DeliveryOrder
    {

        public string mName { get; set; }
        public string mCode { get; set; }
        public string DeliveryCode { get; set; }
        public string ConsigneeID { get; set; }
        public string ConsigneeName { get; set; }

        public Nullable<System.DateTime> DeliveryTime { get; set; }
        public string Remark { get; set; }
        public string OperatorName { get; set; }

        public string OperatorCode { get; set; }
        public Nullable<System.DateTime> OperationTime { get; set; }
        public string ReceivingStoreID { get; set; }
        public string ReceivingStoreName { get; set; }
        public string DeliveryType { get; set; }
        public string EntryStorageCode { get; set; }
        public string EntryStorageName { get; set; }
        public string NumMaterial { get; set; }
        public string TotalDelivery { get; set; }
        public string BatchNo { get; set; }
        public float DeliveryCounts { get; set; }
        public string TargetStorageCode { get; set; }

    }
}
