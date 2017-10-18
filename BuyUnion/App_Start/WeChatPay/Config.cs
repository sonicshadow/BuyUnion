using System;
using System.Collections.Generic;
using System.Web;
using BuyUnion.Models;
namespace WxPayAPI
{
    /**
    * 	配置账号信息
    */
    public class WxPayConfig
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */

        /// <summary>
        /// 绑定支付的APPID wxea4edf995cf54a63
        /// </summary>
        public static string APPID { get { return "wx3dfee3b9ae34d5ae"; } }

        /// <summary>
        /// 手机端APPID
        /// </summary>
        //public static string APPID_M { get { return "wxc36641238ec6db23"; } }

        /// <summary>
        /// 商户号 1316435601
        /// </summary>
        public static string MCHID { get { return "1279131801"; } }

        /// <summary>
        /// 手机端商户号
        /// </summary>
        //public static string MCHID_M { get { return "1328582701"; } }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string KEY { get { return "Immlm2017BuyUnionWeChatPayApiKey"; } }


        /// <summary>
        /// 公众帐号secert 26bd1106cf7da9c6df1c807322ad18e2
        /// </summary>
        public static string APPSECRET { get { return "0b204016cf40bfa2b5acc82a608712de"; } }

        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public const string SSLCERT_PATH = "cert/apiclient_cert.p12";
        public const string SSLCERT_PASSWORD = "1233410002";



        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public static string NOTIFY_URL { get { return $"http://{HttpContext.Current.Request.Url.Host}/WechatPay/NotifyUrl"; } }

        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public static string IP { get { return "121.40.252.187"; } }


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public const string PROXY_URL = "http://10.152.18.220:8080";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 1;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 3;
    }
}