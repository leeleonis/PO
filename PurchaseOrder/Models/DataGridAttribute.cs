using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    //[System.AttributeUsage(System.AttributeTargets.Parameter, Inherited = false)]
    //[System.Runtime.InteropServices.ComVisible(true)]
    [AttributeUsage(AttributeTargets.All)]
    public class DataGridAttribute : Attribute
    {
        // Private fields.
        private float width;
        private string align;
        private bool sortable;
        private bool formatter;
        private bool frozen;
        private string columnsType;

        /// <summary>
        /// 寛度
        /// </summary>
        public virtual float Widths
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// 位置
        /// </summary>
        public virtual string Align
        {
            get { return align; }
            set { align = value; }
        }
        /// <summary>
        /// 是否排序
        /// </summary>
        public virtual bool Sortable
        {
            get { return sortable; }
            set { sortable = value; }
        }
        /// <summary>
        /// 是否格式
        /// </summary>
        public virtual bool Formatter
        {
            get { return formatter; }
            set { formatter = value; }
        }
        /// <summary>
        /// 是否凍結
        /// </summary>
        public virtual bool Frozen
        {
            get { return frozen; }
            set { frozen = value; }
        }
        /// <summary>
        /// 查詢元件
        /// </summary>
        public virtual string ColumnsType
        {
            get { return columnsType; }
            set { columnsType = value; }
        }
    }
}