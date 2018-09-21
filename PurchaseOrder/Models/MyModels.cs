using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public class DataGridModels
    {
        /// <summary>
        /// DataGrid的名稱，可ID,class
        /// </summary>
        public string DataGridName { get; set; }
        /// <summary>
        /// 是否顯示checkbox
        /// </summary>
        public bool checkbox { get; set; }
        /// <summary>
        /// 表格名稱
        /// </summary>
        public string title { get; set; }
        public string idField { get; set; }
        /// <summary>
        /// 顯示新增子項目
        /// </summary>
        public bool additem { get; set; }
        /// <summary>
        /// 是否顯修改按鈕
        /// </summary>
        public bool edititem { get; set; }
        /// <summary>
        /// 是否顯示刪除按鈕
        /// </summary>
        public bool delitem { get; set; }
        /// <summary>
        /// 顯示子表
        /// </summary>
        public bool showchilds { get; set; }
        public List<DataGridItemsModels> DataGridItems { get; set; }

    }
    public class DataGridItemsModels
    {
        public string field { get; set; }
        public string title { get; set; }
        public bool frozen { get; set; }
        public float width { get; set; }
        public string align { get; set; }
        public string sortable { get; set; }
        public bool formatter { get; set; }
        public string columnsType { get; set; }
    }
    public class SelectItem
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}