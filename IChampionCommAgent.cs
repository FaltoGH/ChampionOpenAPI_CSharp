using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public interface IChampionCommAgent
    {
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(2)]
        void CommTerminate(int bSocketClose);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(3)]
        int CommGetConnectState();

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="nVersionPassKey">버전처리 후 받은 키</param>
        /// <param name="sUserID">사용자 아이디</param>
        /// <param name="sPwd">비밀번호</param>
        /// <param name="sCertPwd">공인인증서 비밀번호</param>
        /// <returns>0 if success; otherwise, non-zero integer.</returns>
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(4)]
        int CommLogin(int nVersionPassKey, [MarshalAs(UnmanagedType.BStr)] string sUserID, [MarshalAs(UnmanagedType.BStr)] string sPwd, [MarshalAs(UnmanagedType.BStr)] string sCertPwd);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(5)]
        int CommLogout([MarshalAs(UnmanagedType.BStr)] string sUserID);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(6)]
        int GetLoginState();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(7)]
        int CommLoginPartner(int nVersionPassKey, [MarshalAs(UnmanagedType.BStr)] string sUserID, [MarshalAs(UnmanagedType.BStr)] string sPwd, [MarshalAs(UnmanagedType.BStr)] string sCertPwd, [MarshalAs(UnmanagedType.BStr)] string sPartnerCode);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(8)]
        [TypeLibFunc(64)]
        int GetLoginMode(int nOption);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(15)]
        int CreateRequestID();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(16)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetCommRecvOptionValue(int nOptionType);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(17)]
        void ReleaseRqId(int nRqId);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(20)]
        int SetTranInputData(int nRqId, [MarshalAs(UnmanagedType.BStr)] string strTrCode, [MarshalAs(UnmanagedType.BStr)] string strRecName, [MarshalAs(UnmanagedType.BStr)] string strItem, [MarshalAs(UnmanagedType.BStr)] string strValue);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(21)]
        int RequestTran(int nRqId, [MarshalAs(UnmanagedType.BStr)] string sTrCode, [MarshalAs(UnmanagedType.BStr)] string sNextKey, int nRequestCount);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(22)]
        int GetTranOutputRowCnt([MarshalAs(UnmanagedType.BStr)] string strTrCode, [MarshalAs(UnmanagedType.BStr)] string strRecName);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(23)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetTranOutputData([MarshalAs(UnmanagedType.BStr)] string strTrCode, [MarshalAs(UnmanagedType.BStr)] string strRecName, [MarshalAs(UnmanagedType.BStr)] string strItemName, int nRow);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(24)]
        int SetTranInputArrayCnt(int nRqId, [MarshalAs(UnmanagedType.BStr)] string strTrCode, [MarshalAs(UnmanagedType.BStr)] string strRecName, int nRecCnt);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(25)]
        int SetTranInputArrayData(int nRqId, [MarshalAs(UnmanagedType.BStr)] string strTrCode, [MarshalAs(UnmanagedType.BStr)] string strRecName, [MarshalAs(UnmanagedType.BStr)] string strItem, [MarshalAs(UnmanagedType.BStr)] string strValue, int nArrayIndex);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(33)]
        int GetFidOutputRowCnt(int nRequestId);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(34)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetFidOutputData(int nRequestId, [MarshalAs(UnmanagedType.BStr)] string strFid, int nRow);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(37)]
        int RequestPortfolioFid(int nRqId, [MarshalAs(UnmanagedType.BStr)] string sFidName, [MarshalAs(UnmanagedType.BStr)] string strOutputFidList, [MarshalAs(UnmanagedType.BStr)] string strCodeList, byte chDelimiter, int nRequestCount);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(40)]
        int RegisterReal(short nRealType, [MarshalAs(UnmanagedType.BStr)] string strRealKey);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(41)]
        int UnRegisterReal(short nRealType, [MarshalAs(UnmanagedType.BStr)] string strRealKey);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(42)]
        int AllUnRegisterReal();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(43)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetRealOutputData(short nRealType, [MarshalAs(UnmanagedType.BStr)] string strItemName);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(50)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLastErrMsg();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(51)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetApiAgentModulePath();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(52)]
        int SetSecuUse(int nSecu);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(54)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string SetOptionalFunction(int nOption, int nValue1, int nValue2, [MarshalAs(UnmanagedType.BStr)] string strValue1, [MarshalAs(UnmanagedType.BStr)] string strValue2);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(60)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetAccInfo();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(61)]
        int GetAccCnt();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(70)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetExpCode([MarshalAs(UnmanagedType.BStr)] string sShCode);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(71)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetShCode([MarshalAs(UnmanagedType.BStr)] string sExpCode);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(72)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetShCodeByName([MarshalAs(UnmanagedType.BStr)] string szName);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(73)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetNameByCode([MarshalAs(UnmanagedType.BStr)] string sCode);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(74)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetUpjongByCode([MarshalAs(UnmanagedType.BStr)] string sCode);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(75)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetFutShCode(int nType, int nIndex);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(76)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetOptionATMPrice();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(77)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetOptShCode(int nMonthIndex, int nCallorPut, int nIndex);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(78)]
        short GetMarketKubun([MarshalAs(UnmanagedType.BStr)] string sShCode, [MarshalAs(UnmanagedType.BStr)] string sMarkets);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(79)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetOverseaStockInfo([MarshalAs(UnmanagedType.BStr)] string sCode, int nItemIndex);

        IReadOnlyList<string> GetCodeList();

        void VersionCheck(Action<int> versionCheckCallback);

    }
}
