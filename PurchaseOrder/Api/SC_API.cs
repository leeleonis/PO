using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Web.Mvc;

namespace PurchaseOrderSys.Api
{
    public class SC_API
    {
        private string UserName = "tim@weypro.com";
        private string Password = "timfromweypro";

        public  List<SelectListItem> SCList()
        {
            var SCList = new List<SelectListItem>();
            SCService.SCServiceSoapClient OS_SellerCloud = new SCService.SCServiceSoapClient();
            SCService.AuthHeader OS_AuthHeader = new SCService.AuthHeader { UserName = UserName, Password = Password };
            SCService.ServiceOptions OS_Options = new SCService.ServiceOptions();
            var data = OS_SellerCloud.GetWarehouses(OS_AuthHeader, OS_Options);
            SCList= data.Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            return SCList;
        }


        /// < summary> 
        /// 動態呼叫web服務 
        /// </summary> 
        /// < param name="url">WSDL服務地址</param> 
        /// < param name="methodname">方法名</param> 
        /// < param name="args">引數</param> 
        /// < returns></returns> 
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return SC_API.InvokeWebService(url, null, methodname, args);
        }
        /// < summary> 
        /// 動態呼叫web服務 
        /// </summary> 
        /// < param name="url">WSDL服務地址</param> 
        /// < param name="classname">類名</param> 
        /// < param name="methodname">方法名</param> 
        /// < param name="args">引數</param> 
        /// < returns></returns> 
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == "")) 
{
                classname = SC_API.GetWsClassName(url);
            }
            try
            { //獲取WSDL 
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客戶端代理類程式碼 
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //設定編譯引數 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //編譯代理類 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理例項，並呼叫方法 
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                return mi.Invoke(obj, args);
                // PropertyInfo propertyInfo = type.GetProperty(propertyname); 
                //return propertyInfo.GetValue(obj, null); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}