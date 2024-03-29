﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace inventorySKU.Models
{
    /// <summary>
    /// 登入用
    /// </summary>
    public class LoginViewModels
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Display(Name = "Username", ResourceType = typeof(ViewRes.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
        public string Username { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "Password", ResourceType = typeof(ViewRes.Resource))]
        [MinLength(3, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public  class MenuAuth
    {
        public int MenuID { get; set; }
        public List<string> AuthList { get; set; }
    }
}