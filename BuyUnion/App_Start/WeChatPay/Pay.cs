using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WxPayAPI;

namespace BuyUnion.WeChatPay
{
    public class WechatPay
    {
        private string HOST = $"{HttpContext.Current.Request.Url.Host}";

        public WechatPay()
        {
            Request = HttpContext.Current.Request;
            Response = HttpContext.Current.Response;
        }
        
        /// <summary>
        /// 分（把元转换成分后赋值）
        /// </summary>
        public int TotalFee { get; set; }

        public string OpenID { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public string Body { get; set; }

        public string Attach { get; set; }

        public string GoodsTag { get; set; }





        /**
       * 
       * 网页授权获取用户基本信息的全部过程
       * 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
       * 第一步：利用url跳转获取code
       * 第二步：利用code去获取openid和access_token
       * 
       */
        public void GetOpenidAndAccessToken()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                //获取code码，以获取openid和access_token
                string code = Request.QueryString["code"];
                WxPayAPI.Log.Debug(this.GetType().ToString(), "Get code : " + code);
                GetOpenidAndAccessTokenFromCode(code);
            }
            else
            {
                //构造网页授权获取code的URL
                string host = HOST /*Request.Url.Host*/;
                string path = Request.Path + "?orderCode=" + Request["orderCode"];

                string redirect_uri = "http://" + host + path;
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_base");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
                WxPayAPI.Log.Debug(this.GetType().ToString(), "Will Redirect to URL : " + url);
                try
                {

                    //触发微信返回code码         
                    Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                }
            }
        }



        /**
	    * 
	    * 通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
	    * {
	    *  "access_token":"ACCESS_TOKEN",
	    *  "expires_in":7200,
	    *  "refresh_token":"REFRESH_TOKEN",
	    *  "openid":"OPENID",
	    *  "scope":"SCOPE",
	    *  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
	    * }
	    * 其中access_token可用于获取共享收货地址
	    * openid是微信支付jsapi支付接口统一下单时必须的参数
        * 更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        * @失败时抛异常WxPayException
	    */
        public void GetOpenidAndAccessTokenFromCode(string code)
        {
            try
            {
                //构造获取openid及access_token的url
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("secret", WxPayConfig.APPSECRET);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();

                //请求url以获取数据
                string result = HttpService.Get(url);

                WxPayAPI.Log.Debug(this.GetType().ToString(), "GetOpenidAndAccessTokenFromCode response : " + result);


                JsonData jd = JsonMapper.ToObject(result);
                //获取用户openid
                OpenID = (string)jd["openid"];

                //保存access_token，用于收货地址获取
                //access_token = (string)jd["access_token"];



                WxPayAPI.Log.Debug(this.GetType().ToString(), "Get openid : " + OpenID);
                //Log.Debug(this.GetType().ToString(), "Get access_token : " + access_token);
            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), ex.ToString());
                throw new WxPayException(ex.ToString());
            }
        }

        /**
         * 调用统一下单，获得下单结果
         * @return 统一下单结果
         * @失败时抛异常WxPayException
         */
        public WxPayData GetUnifiedOrderResult()
        {
            //统一下单
            WxPayData data = new WxPayData();
            //必填字段
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", OpenID);
            data.SetValue("body", Body);
            data.SetValue("out_trade_no", OrderCode);
            data.SetValue("total_fee", TotalFee);
            //选填
            //data.SetValue("attach", Attach);//附加说明
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            //data.SetValue("goods_tag", GoodsTag);//标签，如优惠券、降价


            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            unifiedOrderResult = result;
            return result;
        }

        /**
        *  
        * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        * 微信浏览器调起JSAPI时的输入参数格式如下：
        * {
        *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        *   "signType" : "MD5",         //微信签名方式:    
        *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        * }
        * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        * 
        */
        public string GetJsApiParameters()
        {
            WxPayAPI.Log.Debug(this.GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            WxPayAPI.Log.Debug(this.GetType().ToString(), "Get jsApiParam : " + parameters);
            return parameters;
        }

        public string PayRequest()
        {
            throw new Exception("微信使用JS支付");
        }

    }
}