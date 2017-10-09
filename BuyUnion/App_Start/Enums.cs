using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyUnion.Enums
{
    #region 公共枚举
    public enum DebugLog
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 不输出
        /// </summary>
        No,
        /// <summary>
        /// 警告以上
        /// </summary>
        Warning,
        /// <summary>
        /// 错误以上
        /// </summary>
        Error
    }

    public enum DebugLogLevel
    {
        /// <summary>
        /// 普通记录
        /// </summary>
        Normal,
        /// <summary>
        /// 警告级别
        /// </summary>
        Warning,
        /// <summary>
        /// 错误级别
        /// </summary>
        Error
    }

    /// <summary>
    /// 占位图
    /// </summary>
    public enum DummyImage
    {
        [Display(Name = "默认")]
        Default,
        [Display(Name = "头像")]
        Avatar
    }

    public enum ResizerMode
    {
        Pad,
        Crop,
        Max,
    }

    public enum ReszieScale
    {
        Down,
        Both,
        Canvas
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    [Flags]
    public enum DriveType
    {
        Windows = 1,
        IPhone = 2,
        IPad = 4,
        Android = 8,
        WindowsPhone = 16,
    }

    public enum RoleType
    {
        System,
        User,

    }

    public enum FileType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 音频
        /// </summary>
        Audio,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }
    #endregion

    /// <summary>
    /// 用户类别
    /// </summary>
    public enum UserType
    {
        [Display(Name = "系统")]
        System = 0,
        [Display(Name = "商家")]
        Shop = 1,
        [Display(Name = "普通用户")]
        Normal = 2
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        [Display(Name = "待支付")]
        WaitPad = 0,
        [Display(Name = "已支付")]
        Paid = 1,
        [Display(Name = "已取消")]
        Cancel = 2,
        [Display(Name = "已发货")]
        Shipped = 3,
        [Display(Name = "已取消")]
        Complete
    }

    //订单记录类别
    public enum OrderLogType
    {
        Pay,
        SubmitCancel,
        Ship,
        Complete,
        ConfirmCancel
    }

    //支付类别
    public enum PayType
    {
        [Display(Name = "支付宝")]
        Alipay,
        [Display(Name = "微信支付")]
        WeChat
    }


    public enum AmountLogType
    {
        [Display(Name = "收入")]
        Income,
        [Display(Name = "提现")]
        Withdraw
    }

    public enum WithdrawState
    {
        [Display(Name = "待审核")]
        NoCheck,
        [Display(Name = "通过")]
        Pass,
        [Display(Name = "不通过")]
        NoPass
    }
}