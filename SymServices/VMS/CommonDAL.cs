using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;
using System.Net;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using SymOrdinary;

//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;

namespace SymServices.VMS
{
    public class CommonDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Update Currency

        private static string CurrencyUpdateSQL =
            @"
Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Fils',
CurrencySymbol='.?.?' where CurrencyCode='BHD'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Fils',
CurrencySymbol='?.?' where CurrencyCode='IQD'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Fils',
CurrencySymbol='? or K.D' where CurrencyCode='KWD'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Dirham',
CurrencySymbol='LD' where CurrencyCode='LYD'  
  Update Currencies set CurrencyMajor='rial', CurrencyMinor='Baisa',
CurrencySymbol='?.?.' where CurrencyCode='OMR'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Millime',
CurrencySymbol='DT' where CurrencyCode='TND'  
  Update Currencies set CurrencyMajor='d?ng', CurrencyMinor='Hào',
CurrencySymbol='.' where CurrencyCode='VND'  
  Update Currencies set CurrencyMajor='ruble', CurrencyMinor='Kopek',
CurrencySymbol='' where CurrencyCode='RUB'  
  Update Currencies set CurrencyMajor='afghani', CurrencyMinor='Pul',
CurrencySymbol='None' where CurrencyCode='AFN'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='lek', CurrencyMinor='Qindarkë',
CurrencySymbol='L' where CurrencyCode='ALL'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GGP[N]'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Santeem',
CurrencySymbol='??' where CurrencyCode='DZD'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='kwanza', CurrencyMinor='Cêntimo',
CurrencySymbol='Kz' where CurrencyCode='AOA'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='ARS'  
  Update Currencies set CurrencyMajor='dram', CurrencyMinor='Luma',
CurrencySymbol='None' where CurrencyCode='AMD'  
  Update Currencies set CurrencyMajor='florin', CurrencyMinor='Cent',
CurrencySymbol='ƒ' where CurrencyCode='AWG'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='SHP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='AUD'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='manat', CurrencyMinor='Q?pik',
CurrencySymbol='None' where CurrencyCode='AZN'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='BSD'  
  Update Currencies set CurrencyMajor='taka', CurrencyMinor='Paisa',
CurrencySymbol='?' where CurrencyCode='BDT'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='BBD'  
  Update Currencies set CurrencyMajor='ruble', CurrencyMinor='Kapyeyka',
CurrencySymbol='Br' where CurrencyCode='BYR'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='BZD'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='BMD'  
  Update Currencies set CurrencyMajor='ngultrum', CurrencyMinor='Chetrum',
CurrencySymbol='Nu.' where CurrencyCode='BTN'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Paisa',
CurrencySymbol='INR' where CurrencyCode='INR'  
  Update Currencies set CurrencyMajor='boliviano', CurrencyMinor='Centavo',
CurrencySymbol='Bs.' where CurrencyCode='BOB'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='mark', CurrencyMinor='Fening',
CurrencySymbol='KM' where CurrencyCode='BAM'  
  Update Currencies set CurrencyMajor='pula', CurrencyMinor='Thebe',
CurrencySymbol='P' where CurrencyCode='BWP'  
  Update Currencies set CurrencyMajor='real', CurrencyMinor='Centavo',
CurrencySymbol='R$' where CurrencyCode='BRL'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Sen',
CurrencySymbol='$' where CurrencyCode='BND'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='SGD'  
  Update Currencies set CurrencyMajor='lev', CurrencyMinor='Stotinka',
CurrencySymbol='??' where CurrencyCode='BGN'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='kyat', CurrencyMinor='Pya',
CurrencySymbol='Ks' where CurrencyCode='MMK'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='BIF'  
  Update Currencies set CurrencyMajor='riel', CurrencyMinor='Sen',
CurrencySymbol='?' where CurrencyCode='KHR'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='CAD'  
  Update Currencies set CurrencyMajor='escudo', CurrencyMinor='Centavo',
CurrencySymbol=' $' where CurrencyCode='CVE'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='KYD'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='CLP'  
  Update Currencies set CurrencyMajor='yuan', CurrencyMinor='Fen',
CurrencySymbol='¥ ' where CurrencyCode='CNY'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='AUD'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='COP'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='KMF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='CDF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='NZD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='colón', CurrencyMinor='Céntimo',
CurrencySymbol='¢' where CurrencyCode='CRC'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='kuna', CurrencyMinor='Lipa',
CurrencySymbol='kn' where CurrencyCode='HRK'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='CUC'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='CUP'  
  Update Currencies set CurrencyMajor='guilder', CurrencyMinor='Cent',
CurrencySymbol='ƒ' where CurrencyCode='ANG'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='koruna', CurrencyMinor='Halér',
CurrencySymbol='Kc' where CurrencyCode='CZK'  
  Update Currencies set CurrencyMajor='krone', CurrencyMinor='Øre',
CurrencySymbol='kr' where CurrencyCode='DKK'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='DJF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='DOP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Piastre',
CurrencySymbol='£ ' where CurrencyCode='EGP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='nakfa', CurrencyMinor='Cent',
CurrencySymbol='Nfk' where CurrencyCode='ERN'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='birr', CurrencyMinor='Santim',
CurrencySymbol='Br' where CurrencyCode='ETB'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='FKP'  
  Update Currencies set CurrencyMajor='krone', CurrencyMinor='Øre',
CurrencySymbol='kr' where CurrencyCode='DKK'  
  Update Currencies set CurrencyMajor='króna', CurrencyMinor='Oyra',
CurrencySymbol='kr' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='FJD'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XPF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XAF'  
  Update Currencies set CurrencyMajor='dalasi', CurrencyMinor='Butut',
CurrencySymbol='D' where CurrencyCode='GMD'  
  Update Currencies set CurrencyMajor='lari', CurrencyMinor='Tetri',
CurrencySymbol='.' where CurrencyCode='GEL'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='cedi', CurrencyMinor='Pesewa',
CurrencySymbol='?' where CurrencyCode='GHS'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GIP'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='quetzal', CurrencyMinor='Centavo',
CurrencySymbol='Q' where CurrencyCode='GTQ'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='GNF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='GYD'  
  Update Currencies set CurrencyMajor='gourde', CurrencyMinor='Centime',
CurrencySymbol='G' where CurrencyCode='HTG'  
  Update Currencies set CurrencyMajor='lempira', CurrencyMinor='Centavo',
CurrencySymbol='L' where CurrencyCode='HNL'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='HKD'  
  Update Currencies set CurrencyMajor='forint', CurrencyMinor='Fillér',
CurrencySymbol='Ft' where CurrencyCode='HUF'  
  Update Currencies set CurrencyMajor='króna', CurrencyMinor='Eyrir',
CurrencySymbol='kr' where CurrencyCode='ISK'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Paisa',
CurrencySymbol='INR' where CurrencyCode='INR'  
  Update Currencies set CurrencyMajor='rupiah', CurrencyMinor='Sen',
CurrencySymbol='Rp' where CurrencyCode='IDR'  
  Update Currencies set CurrencyMajor='rial', CurrencyMinor='Dinar',
CurrencySymbol='' where CurrencyCode='IRR'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='IMP[N]'  
  Update Currencies set CurrencyMajor='shekel', CurrencyMinor='Agora',
CurrencySymbol='?' where CurrencyCode='ILS'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='JMD'  
  Update Currencies set CurrencyMajor='yen', CurrencyMinor='Sen',
CurrencySymbol='¥' where CurrencyCode='JPY'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='JEP[N]'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Piastre',
CurrencySymbol='None' where CurrencyCode='JOD'  
  Update Currencies set CurrencyMajor='tenge', CurrencyMinor='Tïin',
CurrencySymbol='?' where CurrencyCode='KZT'  
  Update Currencies set CurrencyMajor='shilling', CurrencyMinor='Cent',
CurrencySymbol='Sh' where CurrencyCode='KES'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='AUD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='won', CurrencyMinor='Chon',
CurrencySymbol='?' where CurrencyCode='KPW'  
  Update Currencies set CurrencyMajor='won', CurrencyMinor='Jeon',
CurrencySymbol='?' where CurrencyCode='KRW'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='som', CurrencyMinor='Tyiyn',
CurrencySymbol='None' where CurrencyCode='KGS'  
  Update Currencies set CurrencyMajor='kip', CurrencyMinor='Att',
CurrencySymbol='? or ?N' where CurrencyCode='LAK'  
  Update Currencies set CurrencyMajor='lats', CurrencyMinor='Santims',
CurrencySymbol='Ls' where CurrencyCode='LVL'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Piastre',
CurrencySymbol='?.?' where CurrencyCode='LBP'  
  Update Currencies set CurrencyMajor='loti', CurrencyMinor='Sente',
CurrencySymbol='L' where CurrencyCode='LSL'  
  Update Currencies set CurrencyMajor='rand', CurrencyMinor='Cent',
CurrencySymbol='R' where CurrencyCode='ZAR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='LRD'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Rappen',
CurrencySymbol='Fr' where CurrencyCode='CHF'  
  Update Currencies set CurrencyMajor='litas', CurrencyMinor='Centas',
CurrencySymbol='Lt' where CurrencyCode='LTL'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='pataca', CurrencyMinor='Avo',
CurrencySymbol='P' where CurrencyCode='MOP'  
  Update Currencies set CurrencyMajor='denar', CurrencyMinor='Deni',
CurrencySymbol='???' where CurrencyCode='MKD'  
  Update Currencies set CurrencyMajor='kwacha', CurrencyMinor='Tambala',
CurrencySymbol='MK' where CurrencyCode='MWK'  
  Update Currencies set CurrencyMajor='ringgit', CurrencyMinor='Sen',
CurrencySymbol='RM' where CurrencyCode='MYR'  
  Update Currencies set CurrencyMajor='rufiyaa', CurrencyMinor='Laari',
CurrencySymbol='MVR' where CurrencyCode='MVR'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Cent',
CurrencySymbol='None' where CurrencyCode='MUR'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='$' where CurrencyCode='MXN'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='leu', CurrencyMinor='Ban',
CurrencySymbol='L' where CurrencyCode='MDL'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='tögrög', CurrencyMinor='Möngö',
CurrencySymbol='?' where CurrencyCode='MNT'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='dirham', CurrencyMinor='Centime',
CurrencySymbol='?.?.' where CurrencyCode='MAD'  
  Update Currencies set CurrencyMajor='metical', CurrencyMinor='Centavo',
CurrencySymbol='MT' where CurrencyCode='MZN'  
  Update Currencies set CurrencyMajor='dram', CurrencyMinor='Luma',
CurrencySymbol='' where CurrencyCode='AMD'  
  Update Currencies set CurrencyMajor='dram', CurrencyMinor='Luma',
CurrencySymbol='None' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='NAD'  
  Update Currencies set CurrencyMajor='rand', CurrencyMinor='Cent',
CurrencySymbol='R' where CurrencyCode='ZAR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='AUD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Paisa',
CurrencySymbol='Rs' where CurrencyCode='NPR'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XPF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='NZD'  
  Update Currencies set CurrencyMajor='córdoba', CurrencyMinor='Centavo',
CurrencySymbol='C$' where CurrencyCode='NIO'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='naira', CurrencyMinor='Kobo',
CurrencySymbol='?' where CurrencyCode='NGN'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='NZD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='lira', CurrencyMinor='Kurus',
CurrencySymbol='None' where CurrencyCode='TRY'  
  Update Currencies set CurrencyMajor='krone', CurrencyMinor='Øre',
CurrencySymbol='kr' where CurrencyCode='NOK'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Paisa',
CurrencySymbol='?' where CurrencyCode='PKR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='shekel', CurrencyMinor='Agora',
CurrencySymbol='?' where CurrencyCode='ILS'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Piastre',
CurrencySymbol='None' where CurrencyCode='JOD'  
  Update Currencies set CurrencyMajor='balboa', CurrencyMinor='Centésimo',
CurrencySymbol='B/.' where CurrencyCode='PAB'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='kina', CurrencyMinor='Toea',
CurrencySymbol='K' where CurrencyCode='PGK'  
  Update Currencies set CurrencyMajor='guaraní', CurrencyMinor='Céntimo',
CurrencySymbol=' (? in unicode)' where CurrencyCode='PYG'  
  Update Currencies set CurrencyMajor='sol', CurrencyMinor='Céntimo',
CurrencySymbol='S/.' where CurrencyCode='PEN'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centavo',
CurrencySymbol='?' where CurrencyCode='PHP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='NZD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='zloty', CurrencyMinor='Grosz',
CurrencySymbol='zl' where CurrencyCode='PLN'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='riyal', CurrencyMinor='Dirham',
CurrencySymbol='QR or ?.?' where CurrencyCode='QAR'  
  Update Currencies set CurrencyMajor='leu', CurrencyMinor='Ban',
CurrencySymbol='lei' where CurrencyCode='RON'  
  Update Currencies set CurrencyMajor='ruble', CurrencyMinor='Kopek',
CurrencySymbol='' where CurrencyCode='RUB'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='RWF'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Santeem',
CurrencySymbol='None' where CurrencyCode='DZD'  
  Update Currencies set CurrencyMajor='dirham', CurrencyMinor='Centime',
CurrencySymbol='None' where CurrencyCode='MAD'  
  Update Currencies set CurrencyMajor='peseta', CurrencyMinor='Centime',
CurrencySymbol='Ptas' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='SHP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='XCD'  
  Update Currencies set CurrencyMajor='tala', CurrencyMinor='Sene',
CurrencySymbol='T' where CurrencyCode='WST'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dobra', CurrencyMinor='Cêntimo',
CurrencySymbol='Db' where CurrencyCode='STD'  
  Update Currencies set CurrencyMajor='riyal', CurrencyMinor='Halala',
CurrencySymbol='None' where CurrencyCode='SAR'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='dinar', CurrencyMinor='Para',
CurrencySymbol='din.' where CurrencyCode='RSD'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Cent',
CurrencySymbol='None' where CurrencyCode='SCR'  
  Update Currencies set CurrencyMajor='leone', CurrencyMinor='Cent',
CurrencySymbol='Le' where CurrencyCode='SLL'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Sen',
CurrencySymbol='$' where CurrencyCode='BND'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='SGD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='guilder', CurrencyMinor='Cent',
CurrencySymbol='ƒ' where CurrencyCode='ANG'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='SBD'  
  Update Currencies set CurrencyMajor='shilling', CurrencyMinor='Cent',
CurrencySymbol='Sh' where CurrencyCode='SOS'  
  Update Currencies set CurrencyMajor='shilling', CurrencyMinor='Cent',
CurrencySymbol='Sh' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='rand', CurrencyMinor='Cent',
CurrencySymbol='R' where CurrencyCode='ZAR'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='ruble', CurrencyMinor='Kopek',
CurrencySymbol='' where CurrencyCode='RUB'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Piastre',
CurrencySymbol='£' where CurrencyCode='SSP'  
  Update Currencies set CurrencyMajor='rupee', CurrencyMinor='Cent',
CurrencySymbol='Rs' where CurrencyCode='LKR'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Piastre',
CurrencySymbol='£' where CurrencyCode='SDG'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='SRD'  
  Update Currencies set CurrencyMajor='lilangeni', CurrencyMinor='Cent',
CurrencySymbol='L' where CurrencyCode='SZL'  
  Update Currencies set CurrencyMajor='krona', CurrencyMinor='Öre',
CurrencySymbol='kr' where CurrencyCode='SEK'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Rappen',
CurrencySymbol='Fr' where CurrencyCode='CHF'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Piastre',
CurrencySymbol='£ ' where CurrencyCode='SYP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='TWD'  
  Update Currencies set CurrencyMajor='somoni', CurrencyMinor='Diram',
CurrencySymbol='None' where CurrencyCode='TJS'  
  Update Currencies set CurrencyMajor='shilling', CurrencyMinor='Cent',
CurrencySymbol='Sh' where CurrencyCode='TZS'  
  Update Currencies set CurrencyMajor='baht', CurrencyMinor='Satang',
CurrencySymbol='?' where CurrencyCode='THB'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XOF'  
  Update Currencies set CurrencyMajor='pa?anga', CurrencyMinor='Seniti',
CurrencySymbol='T$' where CurrencyCode='TOP'  
  Update Currencies set CurrencyMajor='ruble', CurrencyMinor='Kopek',
CurrencySymbol='?.' where CurrencyCode='PRB[N]'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='TTD'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='SHP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='lira', CurrencyMinor='Kurus',
CurrencySymbol='None' where CurrencyCode='TRY'  
  Update Currencies set CurrencyMajor='manat', CurrencyMinor='Tennesi',
CurrencySymbol='m' where CurrencyCode='TMT'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='AUD'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='shilling', CurrencyMinor='Cent',
CurrencySymbol='Sh' where CurrencyCode='UGX'  
  Update Currencies set CurrencyMajor='hryvnia', CurrencyMinor='Kopiyka',
CurrencySymbol='?' where CurrencyCode='UAH'  
  Update Currencies set CurrencyMajor='dirham', CurrencyMinor='Fils',
CurrencySymbol='None' where CurrencyCode='AED'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='peso', CurrencyMinor='Centésimo',
CurrencySymbol='$' where CurrencyCode='UYU'  
  Update Currencies set CurrencyMajor='som', CurrencyMinor='Tiyin',
CurrencySymbol='None' where CurrencyCode='UZS'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='bolívar', CurrencyMinor='Céntimo',
CurrencySymbol='Bs F' where CurrencyCode='VEF'  
  Update Currencies set CurrencyMajor='franc', CurrencyMinor='Centime',
CurrencySymbol='Fr' where CurrencyCode='XPF'  
  Update Currencies set CurrencyMajor='rial', CurrencyMinor='Fils',
CurrencySymbol='None' where CurrencyCode='YER'  
  Update Currencies set CurrencyMajor='kwacha', CurrencyMinor='Ngwee',
CurrencySymbol='ZK' where CurrencyCode='ZMW'  
  Update Currencies set CurrencyMajor='pula', CurrencyMinor='Thebe',
CurrencySymbol='P' where CurrencyCode='BWP'  
  Update Currencies set CurrencyMajor='pound', CurrencyMinor='Penny',
CurrencySymbol='£' where CurrencyCode='GBP'  
  Update Currencies set CurrencyMajor='Euro', CurrencyMinor='Cent',
CurrencySymbol='€' where CurrencyCode='EUR'  
  Update Currencies set CurrencyMajor='rand', CurrencyMinor='Cent',
CurrencySymbol='R' where CurrencyCode='ZAR'  
  Update Currencies set CurrencyMajor='dollar', CurrencyMinor='Cent',
CurrencySymbol='$' where CurrencyCode='USD'  
  Update Currencies set CurrencyMajor='ariary', CurrencyMinor='Iraimbilanja',
CurrencySymbol='Ar' where CurrencyCode='MGA'  
  Update Currencies set CurrencyMajor='ouguiya', CurrencyMinor='Khoums',
CurrencySymbol='UM' where CurrencyCode='MRO'  
  Update Currencies set CurrencyMajor='ouguiya', CurrencyMinor='Khoums',
CurrencySymbol='UM' where CurrencyCode='MRO'  
  Update Currencies set CurrencyMajor='apsar', CurrencyMinor='None',
CurrencySymbol='None' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='None', CurrencyMinor='Centavo',
CurrencySymbol='None' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='None', CurrencyMinor='Centavo',
CurrencySymbol='None' where CurrencyCode='None'  
  Update Currencies set CurrencyMajor='vatu', CurrencyMinor='None',
CurrencySymbol='Vt' where CurrencyCode='VUV'  

";

        private static string CurrencyConvertionUpdateSql =
            @"
UPDATE CurrencyConversion
   SET ConversionDate = '1900/01/01 00:00:00.000'
 WHERE ConversionDate is null
";
        #endregion Update Currency
        #region  Create table script
        private static string CreateSettingsRoleScript =
            @"
/****** Object:  Table [dbo].[SettingsRole]    Script Date: 12/02/14 10:24:55 AM ******/
CREATE TABLE [dbo].[SettingsRole](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](20) NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [varchar](120) NULL,
	[SettingType] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]
";
        private static string CreateBanderolsScript =
            @"
/****** Object:  Table [dbo].[Banderols]    Script Date: 31/03/14 12:56:33 PM ******/
CREATE TABLE [dbo].[Banderols](
	[BanderolID] [varchar](50) NOT NULL,
	[BanderolName] [varchar](120) NULL,
	[BanderolSize] [varchar](50) NULL,
	[UOM] [varchar](120) NULL,
	[OpeningQty] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
	[Description] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Banderols] PRIMARY KEY CLUSTERED 
(
	[BanderolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";
        private static string CreatePackInfoScript =
@"
/****** Object:  Table [dbo].[PackagingInformations]    Script Date: 31/03/14 12:56:33 PM ******/
CREATE TABLE [dbo].[PackagingInformations](
	[PackagingID] [varchar](50) NOT NULL,
	[PackagingNature] [varchar](120) NULL,
	[PackagingCapacity] [varchar](50) NULL,
	[UOM] [varchar](120) NULL,
	[Description] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_PackagingInformations] PRIMARY KEY CLUSTERED 
(
	[PackagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
";
        private static string CreateBandeProScript =

@"
/****** Object:  Table [dbo].[BanderolProducts]    Script Date: 31/03/14 12:56:33 PM ******/
CREATE TABLE [dbo].[BanderolProducts](
	[BandProductId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[BanderolId] [varchar](50) NULL,
	[PackagingId] [varchar](50) NULL,
	[BUsedQty] [decimal](25, 9) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[WastageQty] [decimal](25, 9) NULL,
	[OpeningQty] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
 CONSTRAINT [PK_BanderolProducts] PRIMARY KEY CLUSTERED 
(
	[BandProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";
        private static string CreateDemandHeaderScript =

@"
/****** Object:  Table [dbo].[DemandHeaders]    Script Date: 31/03/14 12:56:33 PM ******/
CREATE TABLE [dbo].[DemandHeaders](
	[DemandNo] [varchar](20) NOT NULL,
	[DemandDateTime] [datetime] NULL,
	[FiscalYear] [varchar](30) NULL,
	[MonthFrom] [varchar](50) NULL,
	[MonthTo] [varchar](50) NULL,
	[TotalQty] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[DemandReceiveID] [varchar](20) NULL,
	[VehicleID] [varchar](20) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[DemandReceiveDate] [datetime] NULL,
	[RefNo] [varchar](20) NULL,
	[RefDate] [datetime] NULL,
 CONSTRAINT [PK_DemandHeaders] PRIMARY KEY CLUSTERED 
(
	[DemandNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";
        private static string CreateDemandDetailsScript =
@"
/****** Object:  Table [dbo].[DemandDetails]    Script Date: 31/03/14 12:56:33 PM ******/

            CREATE TABLE [dbo].[DemandDetails](
	[DemandNo] [varchar](20) NOT NULL,
	[DemandLineNo] [int] NULL,
	[BandProductId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[DemandQty] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[TransactionDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[DemandReceiveID] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[TransactionType] [varchar](50) NULL,
	[VehicleID] [varchar](20) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]


";

        private static string CreateTransferRawHeadersScript =
@"
/****** Object:  Table [dbo].[TransferRawHeaders]    Script Date: 12/1/2014 12:16:05 PM ******/

CREATE TABLE [dbo].[TransferRawHeaders](
	[TransferId] [varchar](20) NOT NULL,
	[TransferDateTime] [datetime] NULL,
	[TransFromItemNo] [varchar](20) NOT NULL,
	[UOM] [varchar](50) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[TransferedQty] [decimal](25, 9) NULL,
	[TransferedAmt] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_TransferRawHeaders] PRIMARY KEY CLUSTERED 
(
	[TransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


";
        private static string CreateTransferRawDetailsScript =
@"
/****** Object:  Table [dbo].[TransferRawDetails]    Script Date: 12/1/2014 12:17:15 PM ******/


CREATE TABLE [dbo].[TransferRawDetails](
	[TransferId] [varchar](20) NOT NULL,
	[TransLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[TransFromItemNo] [varchar](20) NOT NULL,
	[TransferDateTime] [datetime] NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]


";

        #region Bureau
        private static string CreateBureauSaleDetailsScript =
        @"
/****** Object:  Table [dbo].[BureauSalesInvoiceDetails]    Script Date: 6/16/2014 9:43:26 PM ******/

CREATE TABLE [dbo].[BureauSalesInvoiceDetails](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[InvoiceLineNo] [int] NULL,
	[ItemNo] [varchar](20) NULL,
	[InvoiceName] [varchar](120) NOT NULL,
	[InvoiceDateTime] [datetime] NULL,
	[Quantity] [decimal](25, 9) NULL,
	[SalesPrice] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[Type] [varchar](10) NULL,
	[PreviousSalesInvoiceNo] [varchar](200) NULL,
	[ChallanDateTime] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[InvoiceCurrency] [varchar](50) NULL,
	[TransactionType] [varchar](50) NULL,
	[CConversionDate] [datetime] NULL,
	[ReturnTransactionType] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_BureauSalesInvoiceDetails_1] PRIMARY KEY CLUSTERED 
(
	[SalesInvoiceNo] ASC,
	[InvoiceName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";
        #endregion

        private static string CreateVAT7Script =
        @"
        CREATE TABLE [dbo].[VAT7](
	[VAT7No] [varchar](20) NOT NULL,
	[Vat7Date] [datetime] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[FinishUOM] [varchar](50) NULL,
	[Vat7LineNo] [int] NULL,
	[ItemNo] [varchar](20) NULL,
	[UOM] [varchar](50) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[Post]	[varchar] (1) NULL,	
    [CreatedBy]	[varchar](120)	NULL,
    [CreatedOn]	[datetime] NULL,	
    [LastModifiedBy]	[varchar] (120) NULL,	
    [LastModifiedOn]	[datetime]	NULL
) ON [PRIMARY]

";
        // Tracking Table

        private static string CreateTrackingsScript =
@"
/****** Object:  Table [dbo].[Trackings]    Script Date: 2/18/2015 10:55:42 AM ******/

CREATE TABLE [dbo].[Trackings](
	
[ItemNo] [varchar](20) NOT NULL,
	[TrackLineNo] [int] NULL,
	[Heading1] [varchar](200) NOT NULL,
	[Heading2] [varchar](200) NULL,
	[Quantity] [int] NULL,
	[IsPurchase] [varchar](1) NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[IsIssue] [varchar](1) NULL,
	[IssueNo] [varchar](20) NULL,
	[IsReceive] [varchar](1) NULL,
	[ReceiveNo] [varchar](20) NULL,
	[IsSale] [varchar](1) NULL,
	[SaleInvoiceNo] [varchar](20) NULL,
	[FinishItemNo] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceivePost] [varchar](1) NULL,
	[SalePost] [varchar](1) NULL,
	[IssuePost] [varchar](1) NULL,
	[ReceiveDate] [datetime] NULL,
	[UnitPrice] [decimal](25, 9) NULL,
	[ReturnType] [varchar](50) NULL,
	[ReturnPurchase] [varchar](1) NULL,
	[ReturnPurchaseID] [varchar](50) NULL,
	[ReturnReceive] [varchar](1) NULL,
	[ReturnReceiveID] [varchar](50) NULL,
	[ReturnSale] [varchar](1) NULL,
	[ReturnSaleID] [varchar](50) NULL,
	[ReturnPurDate] [datetime] NULL,
	[ReturnReceiveDate] [datetime] NULL,
 CONSTRAINT [PK_Trackings] PRIMARY KEY CLUSTERED 
(
	[Heading1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



";
        #endregion  Create table script
        #region Foreign Key

        private static string AddForeignKeyBandeProduct1 =
            @"
SET ANSI_PADDING OFF


ALTER TABLE [dbo].[BanderolProducts]  WITH CHECK ADD  CONSTRAINT [FK_BanderolProducts_Banderols] FOREIGN KEY([BanderolId])
REFERENCES [dbo].[Banderols] ([BanderolID])


ALTER TABLE [dbo].[BanderolProducts] CHECK CONSTRAINT [FK_BanderolProducts_Banderols]
";

        private static string AddForeignKeyBandeProduct2 = @"
SET ANSI_PADDING OFF

ALTER TABLE [dbo].[BanderolProducts]  WITH CHECK ADD  CONSTRAINT [FK_BanderolProducts_PackagingInformations] FOREIGN KEY([PackagingId])
REFERENCES [dbo].[PackagingInformations] ([PackagingID])


ALTER TABLE [dbo].[BanderolProducts] CHECK CONSTRAINT [FK_BanderolProducts_PackagingInformations]";

        private static string AddForeignKeyDemandDetails1 =
            @"

SET ANSI_PADDING OFF


ALTER TABLE [dbo].[DemandDetails]  WITH CHECK ADD  CONSTRAINT [FK_DemandDetails_BanderolProducts] FOREIGN KEY([BandProductId])
REFERENCES [dbo].[BanderolProducts] ([BandProductId])


ALTER TABLE [dbo].[DemandDetails] CHECK CONSTRAINT [FK_DemandDetails_BanderolProducts]";
        private static string AddForeignKeyDemandDetails2 =
                    @"
SET ANSI_PADDING OFF


ALTER TABLE [dbo].[DemandDetails]  WITH CHECK ADD  CONSTRAINT [FK_DemandDetails_DemandHeaders] FOREIGN KEY([DemandNo])
REFERENCES [dbo].[DemandHeaders] ([DemandNo])


ALTER TABLE [dbo].[DemandDetails] CHECK CONSTRAINT [FK_DemandDetails_DemandHeaders]";

        private static string AddForeignKeyTransferRawDetails1 =
            @"
SET ANSI_PADDING OFF


ALTER TABLE [dbo].[TransferRawDetails]  WITH CHECK ADD  CONSTRAINT [FK_TransferRawDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])

ALTER TABLE [dbo].[TransferRawDetails] CHECK CONSTRAINT [FK_TransferRawDetails_Products]
";

        private static string AddForeignKeyTransferRawDetails2 =
        @"
SET ANSI_PADDING OFF


ALTER TABLE [dbo].[TransferRawDetails]  WITH CHECK ADD  CONSTRAINT [FK_TransferRawDetails_TransferRawHeaders] FOREIGN KEY([TransferId])
REFERENCES [dbo].[TransferRawHeaders] ([TransferId])

ALTER TABLE [dbo].[TransferRawDetails] CHECK CONSTRAINT [FK_TransferRawDetails_TransferRawHeaders]
";

        private static string AddForeignKeyTrackings =
        @"
        SET ANSI_PADDING OFF

       ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_Trackings_Products] FOREIGN KEY([ItemNo])
       REFERENCES [dbo].[Products] ([ItemNo])

       ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_Trackings_Products]
        ";

        #endregion

        public DataTable SearchTransanctionHistoryNew(string TransactionNo, string TransactionType, string TransactionDateFrom, string TransactionDateTo, string ProductName, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("Search Transaction History");

            #endregion

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                            SELECT     dbo.TransactionHistorys.TransactionNo,
                            dbo.TransactionHistorys.TransactionType,
                            convert(varchar,dbo.TransactionHistorys.TransactionDate,120)TransactionDate,
                            dbo.Products.ProductName, 
                            dbo.TransactionHistorys.UOM,
                            dbo.TransactionHistorys.Quantity,
                            dbo.TransactionHistorys.UPrice, 
                            dbo.TransactionHistorys.TradingMarkup,
                            dbo.TransactionHistorys.SDRate,
                            dbo.TransactionHistorys.VATRate,
                            dbo.TransactionHistorys.TCost, 
                            dbo.TransactionHistorys.CreatedBy,
                            convert (varchar,dbo.TransactionHistorys.CreatedOn,120)CreatedOn,
                            dbo.TransactionHistorys.LastModifiedBy,
                            convert (varchar,dbo.TransactionHistorys.LastModifiedOn,120)LastModifiedOn
                            FROM         dbo.TransactionHistorys LEFT OUTER JOIN
                                                    dbo.Products ON dbo.TransactionHistorys.ItemNo = dbo.Products.ItemNo
                                       
                            WHERE 
                                (TransactionNo  =  @TransactionNo OR @TransactionNo IS NULL) 
                            AND (TransactionType = @TransactionType  OR @TransactionType IS NULL)
                            AND (TransactionDate >= @TransactionDateFrom OR @TransactionDateFrom IS NULL)
                            AND (TransactionDate < dateadd(d,1, @TransactionDateTo)  OR @TransactionDateTo IS NULL)
                            AND (ProductName = @ProductName  OR @ProductName IS NULL)
                            and TransactionType is not null
                            order by TransactionDate desc ,TransactionNo asc,ProductName asc
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommTransanctionHistory = new SqlCommand();
                objCommTransanctionHistory.Connection = currConn;

                objCommTransanctionHistory.CommandText = sqlText;
                objCommTransanctionHistory.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (TransactionNo == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionNo"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionNo", System.DBNull.Value); }
                    else { objCommTransanctionHistory.Parameters["@TransactionNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionNo"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionNo", TransactionNo); }
                    else { objCommTransanctionHistory.Parameters["@TransactionNo"].Value = TransactionNo; }
                }
                if (TransactionType == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", System.DBNull.Value); }
                    else { objCommTransanctionHistory.Parameters["@TransactionType"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (TransactionType == "DebitNote")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "Debit"); }
                        else { objCommTransanctionHistory.Parameters["@TransactionType"].Value = "Debit"; }
                    }
                    else if (TransactionType == "CreditNote")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "Credit"); }
                        else { objCommTransanctionHistory.Parameters["@TransactionType"].Value = "Credit"; }
                    }
                    else if (TransactionType == "Sale")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "New"); }
                        else { objCommTransanctionHistory.Parameters["@TransactionType"].Value = "New"; }
                    }
                    else
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", TransactionType); }
                        else { objCommTransanctionHistory.Parameters["@TransactionType"].Value = TransactionType; }
                    }
                }
                if (TransactionDateFrom == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateFrom"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateFrom", System.DBNull.Value); }
                    else { objCommTransanctionHistory.Parameters["@TransactionDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateFrom"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateFrom", TransactionDateFrom); }
                    else { objCommTransanctionHistory.Parameters["@TransactionDateFrom"].Value = TransactionDateFrom; }
                    // Common Filed

                }
                if (TransactionDateTo == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateTo"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateTo", System.DBNull.Value); }
                    else { objCommTransanctionHistory.Parameters["@TransactionDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateTo"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateTo", TransactionDateTo); }
                    else { objCommTransanctionHistory.Parameters["@TransactionDateTo"].Value = TransactionDateTo; }

                }
                if (ProductName == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@ProductName"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@ProductName", System.DBNull.Value); }
                    else { objCommTransanctionHistory.Parameters["@ProductName"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@ProductName"))
                    { objCommTransanctionHistory.Parameters.AddWithValue("@ProductName", ProductName); }
                    else { objCommTransanctionHistory.Parameters["@ProductName"].Value = ProductName; }
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransanctionHistory);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return dataTable;

        }

        public bool TestConnection(string userName, string Password, string Datasource)
        {
            bool result = false;
            SqlConnection conn = null;
            try
            {
                #region open connection and transaction
                string ConnectionString = "Data Source=" + Datasource + ";" +
                              "user id=" + userName + ";password=" + Password + ";connect Timeout=120;";
                conn = new SqlConnection(ConnectionString);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    result = true;
                }


                #endregion open connection and transaction


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

            return result;

        }
        public DataSet CompanyList(string ActiveStatus)
        {

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataTable = new DataSet();
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"

SELECT 
CompanySl,
CompanyID,
CompanyName,
DatabaseName,
ActiveStatus,
Serial
FROM CompanyInformations
where (ActiveStatus = @ActiveStatus)	
and (CompanyName<>'NA')
ORDER BY ISNULl(serial,CompanySL) asc;


";

                SqlCommand objCommCompanyList = new SqlCommand();
                objCommCompanyList.Connection = currConn;
                objCommCompanyList.CommandText = sqlText;
                objCommCompanyList.CommandType = CommandType.Text;


                if (!objCommCompanyList.Parameters.Contains("@ActiveStatus"))
                { objCommCompanyList.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommCompanyList.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCompanyList);

                dataAdapter.Fill(dataTable);

            }
            #region Catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            return dataTable;
        }
        public DataTable SuperAdministrator()
        {

            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SA");
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
                            SELECT miki as [user],mouse as [pwd]
FROM SuperAdministrator";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return dataTable;
        }
        public DataSet SuperDBInformation()
        {
            DataSet superDs = new DataSet();

            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml";
                if (!File.Exists(filePath))
                {
                    return superDs;
                }

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");
                superDs.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");

            }

            catch (Exception exp)
            {
                throw exp;
            }
            return superDs;
        }


        //public string settings(string SettingGroup, string SettingName)
        //{
        //    string SettingValue = string.Empty;
        //    try
        //    {

        //        DataRow[] settingRow =
        //            settingVM.SettingsDT.Select("SettingGroup='" + SettingGroup + "' and SettingName='" + SettingName +
        //                                        "'");
        //        SettingValue = settingRow[0]["SettingValue"].ToString();
        //    }
        //    catch (Exception ex)
        //    {

        //        SettingValue = string.Empty;
        //    }
        //    //finally
        //    //{
        //    return SettingValue;
        //    //}
        //}

        public string settings(string SettingGroup, string SettingName)
        {
            /// firstly check user in settingsrole ,if not exist then check settings table
            #region Objects & Variables
            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";
            string SettingValue = string.Empty;
            DataTable dataTable = new DataTable("SA");
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                #region sql statement
                sqlText = "  ";
                sqlText += " SELECT SettingValue FROM SettingsRole";
                sqlText += " WHERE SettingGroup=@SettingGroup AND SettingName=@SettingName AND UserId='" + UserInfoVM.UserId + "'";

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;

                cmdExist.Parameters.AddWithValue("@SettingName", SettingName);
                cmdExist.Parameters.AddWithValue("@SettingGroup", SettingGroup);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    DataRow[] settingRow =
                    settingVM.SettingsDT.Select("SettingGroup='" + SettingGroup + "' and SettingName='" + SettingName +
                                                "'");
                    SettingValue = settingRow[0]["SettingValue"].ToString();
                }
                else
                {
                    SettingValue = objfoundId.ToString();
                }

            }
                #endregion

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                SettingValue = string.Empty;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                SettingValue = string.Empty;
            }

            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion

            return SettingValue;

        }
        public string SysDBCreate(string Uname, string Pwd, string DBSource)
        {
            string result = string.Empty;
            return result;
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string nextId = "";
            string newID = "";

            #endregion Initializ

            #region Validation

            //if (string.IsNullOrEmpty(databaseName))
            //{
            //    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyName);
            //}
            #endregion Validation

            #region open connection and transaction sys / Master

            SysDBInfoVM.SysDatabaseName = "SymphonyVATSys";

            currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//start
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }

            #endregion open connection and transaction

        }
        public string[] NewDBCreateOld(CompanyProfileVM companyProfiles, string databaseName, List<FiscalYearVM> fiscalDetails)
        {

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string nextId = "";
            string newID = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(databaseName))
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyName);
                }
                if (fiscalDetails.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoFiscalYear);
                }
                if (companyProfiles == null)
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyInformation);
                }

                #endregion Validation

                #region open connection and transaction sys / Master

                SysDBInfoVM.SysDatabaseName = "SymphonyVATSys";

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//start
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region check Database

                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " select COUNT(NAME) from sys.databases where name =@databaseName ";

                SqlCommand cmdDBExist = new SqlCommand(sqlText, currConn);

                cmdDBExist.Parameters.AddWithValue("@databaseName", databaseName);

                transResult = (int)cmdDBExist.ExecuteScalar();
                if (transResult > 0)
                {
                    throw new ArgumentNullException("DeleteDB", MessageVM.dbMsgDBExist);
                }

                #endregion check Database

                #region CreateDatabase

                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " CREATE DATABASE @databaseName ";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                transResult = (int)cmdIdExist.ExecuteNonQuery();
                if (transResult != -1)
                {
                    throw new ArgumentNullException("Create Database('" + databaseName + "')", MessageVM.dbMsgDBNotCreate);
                }

                #endregion CreateDatabase

                #region Change Database for New DB
                currConn.ChangeDatabase(databaseName);
                transaction = currConn.BeginTransaction(MessageVM.dbMsgMethodName);
                #endregion open connection and transaction

                #region TableCreate
                string top1;

                #region CreateTable Back
                //              sqlText = @"
                //
                //";
                #endregion CreateTable
                #region CreateTable Back
                sqlText = @"
                
CREATE TABLE [dbo].[AdjustmentHistorys](
	[AdjHistoryID] [varchar](50) NULL,
	[AdjHistoryNo] [varchar](50) NULL,
	[AdjId] [varchar](50) NULL,
	[AdjDate] [datetime] NULL,
	[AdjInputAmount] [decimal](25, 9) NULL,
	[AdjInputPercent] [decimal](25, 9) NULL,
	[AdjAmount] [decimal](25, 9) NULL,
	[AdjVATRate] [decimal](25, 9) NULL,
	[AdjVATAmount] [decimal](25, 9) NULL,
	[AdjSD] [decimal](25, 9) NULL,
	[AdjSDAmount] [decimal](25, 9) NULL,
	[AdjOtherAmount] [decimal](25, 9) NULL,
	[AdjType] [varchar](50) NULL,
	[AdjDescription] [varchar](500) NULL,
	[AdjReferance] [varchar](500) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdjustmentName]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdjustmentName](
	[AdjId] [varchar](50) NULL,
	[AdjName] [varchar](500) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BankInformations]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BankInformations](
	[BankID] [varchar](20) NOT NULL,
	[BankCode] [varchar](50) NULL,
	[BankName] [varchar](120) NULL,
	[BranchName] [varchar](120) NULL,
	[AccountNumber] [varchar](120) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_BankInformations] PRIMARY KEY CLUSTERED 
(
	[BankID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMCompanyOverhead]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMCompanyOverhead](
	[BOMOverHeadId] [varchar](20) NOT NULL,
	[BOMId] [varchar](20) NOT NULL,
	[OHLineNo] [int] NULL,
	[HeadName] [varchar](150) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[HeadAmount] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info5] [varchar](200) NULL,
	[RebatePercent] [decimal](25, 9) NULL,
	[RebateAmount] [decimal](25, 9) NULL,
	[AdditionalCost] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[HeadID] [varchar](20) NULL,
 CONSTRAINT [PK_BOMCompanyOverhead] PRIMARY KEY CLUSTERED 
(
	[BOMOverHeadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMRaws]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMRaws](
	[BOMRawId] [varchar](20) NOT NULL,
	[BOMId] [varchar](20) NOT NULL,
	[BOMLineNo] [int] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[RawItemNo] [varchar](20) NOT NULL,
	[RawItemType] [varchar](50) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[UseQuantity] [decimal](25, 9) NULL,
	[WastageQuantity] [decimal](25, 9) NULL,
	[Cost] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[RebateRate] [decimal](25, 9) NULL,
	[MarkUpValue] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMUQty] [decimal](25, 9) NULL,
	[UOMWQty] [decimal](25, 9) NULL,
	[TotalQuantity] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[PBOMId] [varchar](20) NULL,
	[PInvoiceNo] [varchar](20) NULL,
 CONSTRAINT [PK_BOMRaws] PRIMARY KEY CLUSTERED 
(
	[BOMRawId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMs]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMs](
	[BOMId] [varchar](20) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[VATRate] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[RawTotal] [decimal](25, 9) NULL,
	[PackingTotal] [decimal](25, 9) NULL,
	[RebateTotal] [decimal](25, 9) NULL,
	[AdditionalTotal] [decimal](25, 9) NULL,
	[RebateAdditionTotal] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[PacketPrice] [decimal](25, 9) NULL,
	[RawOHCost] [decimal](25, 9) NULL,
	[LastNBRPrice] [decimal](25, 9) NULL,
	[LastNBRWithSDAmount] [decimal](25, 9) NULL,
	[TotalQuantity] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[WholeSalePrice] [decimal](25, 9) NULL,
	[NBRWithSDAmount] [decimal](25, 9) NULL,
	[MarkUpValue] [decimal](25, 9) NULL,
	[LastMarkUpValue] [decimal](25, 9) NULL,
	[LastSDAmount] [decimal](25, 9) NULL,
	[LastAmount] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[UOM] [varchar](120) NULL,
 CONSTRAINT [PK_BOMs] PRIMARY KEY CLUSTERED 
(
	[BOMId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMsMas]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMsMas](
	[BOMId] [varchar](20) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[NBRPrice] [decimal](18, 5) NULL,
	[PacketPrice] [decimal](18, 5) NULL,
	[RawOHCost] [decimal](18, 5) NULL,
	[LastNBRPrice] [decimal](18, 5) NULL,
	[LastNBRWithSDAmount] [decimal](18, 5) NULL,
	[TotalQuantity] [decimal](18, 5) NULL,
	[SDAmount] [decimal](18, 5) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[WholeSalePrice] [decimal](18, 5) NULL,
	[NBRWithSDAmount] [decimal](18, 5) NULL,
	[MarkUpValue] [decimal](18, 5) NULL,
	[LastMarkUpValue] [decimal](18, 5) NULL,
	[LastSDAmount] [decimal](18, 5) NULL,
	[LastAmount] [decimal](18, 5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Codes]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Codes](
	[CodeId] [int] IDENTITY(1,1) NOT NULL,
	[CodeGroup] [varchar](120) NULL,
	[CodeName] [varchar](120) NULL,
	[prefix] [varchar](120) NULL,
	[Lenth] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyOverheads]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyOverheads](
	[HeadID] [varchar](20) NULL,
	[HeadName] [varchar](150) NOT NULL,
	[HeadAmount] [decimal](25, 9) NULL,
	[Description] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [nchar](10) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[OHCode] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[RebatePercent] [decimal](25, 9) NULL,
 CONSTRAINT [PK_CompanyOverheads] PRIMARY KEY CLUSTERED 
(
	[HeadName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyOverheadVAT]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyOverheadVAT](
	[HeadName] [varchar](150) NOT NULL,
	[HeadAmount] [decimal](18, 5) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[EffectMonth] [datetime] NOT NULL,
 CONSTRAINT [PK_CompanyOverheadVAT] PRIMARY KEY CLUSTERED 
(
	[HeadName] ASC,
	[EffectMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyProfiles]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyProfiles](
	[CompanyID] [varchar](20) NOT NULL,
	[CompanyName] [varchar](120) NULL,
	[CompanyLegalName] [varchar](120) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[ZipCode] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[VatRegistrationNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[StartDateTime] [datetime] NULL,
	[FYearStart] [datetime] NULL,
	[FYearEnd] [datetime] NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
    [Tom] [varchar](200) NULL,
	[Jary] [varchar](200) NULL,
    [Miki] [varchar](200) NULL,
	[Mouse] [varchar](200) NULL,
 CONSTRAINT [PK_CompanyProfile] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Costing]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Costing](
	[ItemNo] [varchar](20) NULL,
	[InputDate] [datetime] NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[AV] [decimal](25, 9) NULL,
	[CD] [decimal](25, 9) NULL,
	[RD] [decimal](25, 9) NULL,
	[TVB] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVA] [decimal](25, 9) NULL,
	[ATV] [decimal](25, 9) NULL,
	[Other] [decimal](25, 9) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyName] [varchar](500) NULL,
	[CurrencyCode] [varchar](50) NULL,
	[Country] [varchar](500) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CurrencyConversion]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CurrencyConversion](
	[CurrencyConversionId] [varchar](20) NULL,
	[CurrencyCodeFrom] [varchar](50) NOT NULL,
	[CurrencyCodeTo] [varchar](50) NULL,
	[CurrencyRate] [decimal](18, 10) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ConversionDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerGroups]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerGroups](
	[CustomerGroupID] [varchar](20) NOT NULL,
	[CustomerGroupName] [varchar](120) NULL,
	[CustomerGroupDescription] [varchar](120) NULL,
	[GroupType] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_CustomerGroup] PRIMARY KEY CLUSTERED 
(
	[CustomerGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](50) NULL,
	[CustomerName] [varchar](120) NULL,
	[CustomerGroupID] [varchar](20) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[Country] [varchar](200) NULL,
	[VDSPercent] [decimal](25, 9) NULL,
	[BusinessType] [varchar](120) NULL,
	[BusinessCode] [varchar](20) NULL,
 CONSTRAINT [PK_CustomerInformation] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DDBDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DDBDetails](
	[DDBDetailsNo] [varchar](20) NOT NULL,
	[DDBNo] [varchar](20) NOT NULL,
	[DDBDateTime] [datetime] NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[BENumber] [varchar](200) NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[PurcahseItemNo] [varchar](20) NOT NULL,
	[SaleItemNo] [varchar](20) NOT NULL,
	[DDBLineNo] [int] NULL,
	[Quantity] [decimal](18, 5) NULL,
	[UseQuantity] [decimal](18, 5) NULL,
	[CostPrice] [decimal](18, 5) NULL,
	[UOM] [varchar](120) NULL,
	[SubTotal] [decimal](18, 5) NULL,
	[CnFAmount] [decimal](18, 9) NULL,
	[InsuranceAmount] [decimal](18, 9) NULL,
	[AssessableValue] [decimal](18, 9) NULL,
	[CDAmount] [decimal](18, 9) NULL,
	[RDAmount] [decimal](18, 9) NULL,
	[SD] [decimal](18, 2) NULL,
	[SDAmount] [decimal](18, 9) NULL,
	[TVBAmount] [decimal](18, 9) NULL,
	[VATRate] [decimal](18, 2) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[TVAAmount] [decimal](18, 9) NULL,
	[ATVAmount] [decimal](18, 9) NULL,
	[OthersAmount] [decimal](18, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DDBHeader]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DDBHeader](
	[DDBNo] [varchar](20) NOT NULL,
	[DDBDateTime] [datetime] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](18, 5) NULL,
	[UOM] [varchar](120) NULL,
	[TotalAmount] [decimal](18, 5) NULL,
	[CnFAmount] [decimal](18, 9) NULL,
	[InsuranceAmount] [decimal](18, 9) NULL,
	[AssessableValue] [decimal](18, 9) NULL,
	[CDAmount] [decimal](18, 9) NULL,
	[RDAmount] [decimal](18, 9) NULL,
	[SD] [decimal](18, 2) NULL,
	[SDAmount] [decimal](18, 9) NULL,
	[TVBAmount] [decimal](18, 9) NULL,
	[VATRate] [decimal](18, 2) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[TVAAmount] [decimal](18, 9) NULL,
	[ATVAmount] [decimal](18, 9) NULL,
	[OthersAmount] [decimal](18, 9) NULL,
	[Remarks] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Deposits]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Deposits](
	[DepositId] [varchar](20) NOT NULL,
	[TreasuryNo] [varchar](50) NULL,
	[DepositDateTime] [datetime] NULL,
	[DepositType] [varchar](50) NULL,
	[DepositAmount] [decimal](25, 9) NULL,
	[ChequeNo] [varchar](50) NULL,
	[ChequeBank] [varchar](120) NULL,
	[ChequeBankBranch] [varchar](120) NULL,
	[ChequeDate] [datetime] NULL,
	[BankID] [varchar](20) NULL,
	[TreasuryCopy] [varchar](20) NULL,
	[DepositPerson] [varchar](120) NULL,
	[DepositPersonDesignation] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
 CONSTRAINT [PK_Deposit] PRIMARY KEY CLUSTERED 
(
	[DepositId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DisposeDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DisposeDetails](
	[DisposeNumber] [varchar](20) NOT NULL,
	[LineNumber] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[UOM] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[RealPrice] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SaleNumber] [varchar](120) NULL,
	[PurchaseNumber] [varchar](120) NOT NULL,
	[PresentPrice] [decimal](25, 9) NULL,
	[Remarks] [varchar](120) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[DisposeDate] [datetime] NULL,
	[QuantityImport] [decimal](25, 9) NULL,
	[TransactionType] [varchar](120) NULL,
	[FromStock] [varchar](1) NULL,
	[DollarPrice] [decimal](25, 9) NULL,
 CONSTRAINT [PK_DisposeDetails] PRIMARY KEY CLUSTERED 
(
	[DisposeNumber] ASC,
	[ItemNo] ASC,
	[PurchaseNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DisposeHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DisposeHeaders](
	[DisposeNumber] [varchar](20) NOT NULL,
	[DisposeDate] [datetime] NULL,
	[RefNumber] [varchar](120) NULL,
	[Remarks] [varchar](120) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](120) NULL,
	[Post] [varchar](1) NULL,
	[FromStock] [varchar](1) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[ImportVATAmount] [decimal](25, 9) NULL,
	[TotalPrice] [decimal](25, 9) NULL,
	[TotalPriceImport] [decimal](25, 9) NULL,
	[AppVATAmount] [decimal](25, 9) NULL,
	[AppTotalPrice] [decimal](25, 9) NULL,
	[AppVATAmountImport] [decimal](25, 9) NULL,
	[AppTotalPriceImport] [decimal](25, 9) NULL,
	[AppDate] [datetime] NULL,
	[AppRefNumber] [varchar](120) NULL,
	[AppRemarks] [varchar](120) NULL,
 CONSTRAINT [PK_DisposeHeaders] PRIMARY KEY CLUSTERED 
(
	[DisposeNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Duties]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Duties](
	[DutyID] [varchar](50) NOT NULL,
	[DutyName] [varchar](120) NULL,
	[DutyRate] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[DutyType] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Duties] PRIMARY KEY CLUSTERED 
(
	[DutyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBackDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBackDetails](
	[DDBackNo] [varchar](20) NOT NULL,
	[DDBackDate] [datetime] NULL,
	[DDLineNo] [int] NULL,
	[SalesInvoiceNo] [varchar](20) NULL,
	[PurchaseInvoiceNo] [varchar](20) NULL,
	[PurchaseDate] [datetime] NULL,
	[FgItemNo] [varchar](20) NULL,
	[FgQty] [decimal](25, 9) NULL,
	[ItemNo] [varchar](20) NULL,
	[BillOfEntry] [varchar](50) NULL,
	[PurchaseUom] [varchar](10) NULL,
	[PurchaseQuantity] [decimal](25, 9) NULL,
	[UnitPrice] [decimal](25, 9) NULL,
	[AV] [decimal](25, 9) NULL,
	[CD] [decimal](25, 9) NULL,
	[RD] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[VAT] [decimal](25, 9) NULL,
	[CnF] [decimal](25, 9) NULL,
	[Insurance] [decimal](25, 9) NULL,
	[TVB] [decimal](25, 9) NULL,
	[TVA] [decimal](25, 9) NULL,
	[ATV] [decimal](25, 9) NULL,
	[Others] [decimal](25, 9) NULL,
	[UseQuantity] [decimal](25, 9) NULL,
	[ClaimCD] [decimal](25, 9) NULL,
	[ClaimRD] [decimal](25, 9) NULL,
	[ClaimSD] [decimal](25, 9) NULL,
	[ClaimVAT] [decimal](25, 9) NULL,
	[ClaimCnF] [decimal](25, 9) NULL,
	[ClaimInsurance] [decimal](25, 9) NULL,
	[ClaimTVB] [decimal](25, 9) NULL,
	[ClaimTVA] [decimal](25, 9) NULL,
	[ClaimATV] [decimal](25, 9) NULL,
	[ClaimOthers] [decimal](25, 9) NULL,
	[SubTotalDDB] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[UOMCD] [decimal](25, 9) NULL,
	[UOMRD] [decimal](25, 9) NULL,
	[UOMSD] [decimal](25, 9) NULL,
	[UOMVAT] [decimal](25, 9) NULL,
	[UOMCnF] [decimal](25, 9) NULL,
	[UOMInsurance] [decimal](25, 9) NULL,
	[UOMTVB] [decimal](25, 9) NULL,
	[UOMTVA] [decimal](25, 9) NULL,
	[UOMATV] [decimal](25, 9) NULL,
	[UOMOthers] [decimal](25, 9) NULL,
	[UOMSubTotalDDB] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](50) NULL,
	[LastModifiedOn] [datetime] NULL,
	[PurchasetransactionType] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBackHeader]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBackHeader](
	[DDBackNo] [varchar](20) NOT NULL,
	[DDBackDate] [datetime] NOT NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SalesDate] [datetime] NOT NULL,
	[CustormerID] [varchar](20) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[ExpCurrency] [decimal](25, 9) NULL,
	[BDTCurrency] [decimal](25, 9) NULL,
	[FgItemNo] [varchar](20) NOT NULL,
	[TotalClaimCD] [decimal](25, 9) NULL,
	[TotalClaimRD] [decimal](25, 9) NULL,
	[TotalClaimSD] [decimal](25, 9) NULL,
	[TotalDDBack] [decimal](25, 9) NULL,
	[TotalClaimVAT] [decimal](25, 9) NULL,
	[TotalClaimCnFAmount] [decimal](25, 9) NULL,
	[TotalClaimInsuranceAmount] [decimal](25, 9) NULL,
	[TotalClaimTVBAmount] [decimal](25, 9) NULL,
	[TotalClaimTVAAmount] [decimal](25, 9) NULL,
	[TotalClaimATVAmount] [decimal](25, 9) NULL,
	[TotalClaimOthersAmount] [decimal](25, 9) NULL,
	[Comments] [varchar](250) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](20) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
 CONSTRAINT [PK_DutyDrawBackHeader] PRIMARY KEY CLUSTERED 
(
	[DDBackNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBacks]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBacks](
	[DrawBackID] [varchar](20) NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[BENumber] [varchar](200) NULL,
	[PurchaseDateTime] [datetime] NULL,
	[PurchaseReceiveDate] [datetime] NULL,
	[PurchaseItemNo] [varchar](20) NOT NULL,
	[PurchaseDutyAmount] [decimal](18, 6) NULL,
	[DrawBackDutyPercent] [decimal](18, 6) NULL,
	[DrawBackDutyAmount] [decimal](18, 6) NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SaleDateTime] [datetime] NULL,
	[SaleDeliveryDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FiscalYear]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FiscalYear](
	[FiscalYearName] [varchar](30) NULL,
	[CurrentYear] [varchar](4) NOT NULL,
	[PeriodID] [varchar](6) NOT NULL,
	[PeriodName] [varchar](50) NULL,
	[PeriodStart] [datetime] NULL,
	[PeriodEnd] [datetime] NULL,
	[PeriodLock] [varchar](1) NULL,
	[GLLock] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_FiscalYear] PRIMARY KEY CLUSTERED 
(
	[CurrentYear] ASC,
	[PeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ImagesStore]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ImagesStore](
	[OriginalPath] [varchar](500) NULL,
	[ImageData] [binary](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[IssueDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IssueDetails](
	[IssueNo] [varchar](20) NOT NULL,
	[IssueLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceiveNo] [varchar](20) NULL,
	[IssueDateTime] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[Wastage] [decimal](25, 9) NULL,
	[BOMDate] [datetime] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[Post] [varchar](1) NULL,
	[TransactionType] [varchar](50) NULL,
	[IssueReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[BOMId] [varchar](20) NULL,
	[UOMWastage] [decimal](25, 9) NULL,
	[IsProcess] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[IssueHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IssueHeaders](
	[IssueNo] [varchar](20) NOT NULL,
	[IssueDateTime] [datetime] NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceiveNo] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[IssueReturnId] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[ImportIDExcel] [varchar](30) NULL,
 CONSTRAINT [PK_IssueHeader] PRIMARY KEY CLUSTERED 
(
	[IssueNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PriceService]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceService](
	[ItemNo] [varchar](20) NOT NULL,
	[Cost] [decimal](25, 9) NULL,
	[BasePrice] [decimal](25, 9) NULL,
	[OtherRate] [decimal](25, 9) NULL,
	[OtherType] [decimal](25, 9) NULL,
	[OtherAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SalePrice] [decimal](25, 9) NULL,
	[EffectDate] [datetime] NOT NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[CategoryID] [varchar](20) NOT NULL,
	[CategoryName] [varchar](120) NULL,
	[Description] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[IsRaw] [varchar](50) NOT NULL,
	[HSCodeNo] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[PropergatingRate] [varchar](1) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[Trading] [varchar](1) NULL,
	[NonStock] [varchar](1) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Products]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Products](
	[ItemNo] [varchar](20) NOT NULL,
	[ProductCode] [varchar](50) NULL,
	[ProductName] [varchar](120) NULL,
	[ProductDescription] [varchar](120) NULL,
	[CategoryID] [varchar](20) NULL,
	[UOM] [varchar](120) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[SalesPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[ReceivePrice] [decimal](25, 9) NULL,
	[IssuePrice] [decimal](25, 9) NULL,
	[TenderPrice] [decimal](25, 9) NULL,
	[ExportPrice] [decimal](25, 9) NULL,
	[InternalIssuePrice] [decimal](25, 9) NULL,
	[TollIssuePrice] [decimal](25, 9) NULL,
	[TollCharge] [decimal](25, 9) NULL,
	[OpeningBalance] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[HSCodeNo] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[SD] [decimal](25, 9) NULL,
	[PacketPrice] [decimal](25, 9) NULL,
	[Trading] [varchar](1) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[NonStock] [varchar](1) NULL,
	[QuantityInHand] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
	[RebatePercent] [decimal](25, 9) NULL,
	[TVBRate] [decimal](25, 9) NULL,
	[CnFRate] [decimal](25, 9) NULL,
	[InsuranceRate] [decimal](25, 9) NULL,
	[CDRate] [decimal](25, 9) NULL,
	[RDRate] [decimal](25, 9) NULL,
	[AITRate] [decimal](25, 9) NULL,
	[TVARate] [decimal](25, 9) NULL,
	[ATVRate] [decimal](25, 9) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[OpeningTotalCost] [decimal](25, 9) NULL,
 CONSTRAINT [PK_ItemInformation] PRIMARY KEY CLUSTERED 
(
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceDetails](
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[POLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Type] [varchar](200) NULL,
	[ProductType] [varchar](200) NULL,
	[BENumber] [varchar](200) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[ReceiveDate] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[RebateRate] [decimal](25, 9) NULL,
	[RebateAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[CnFAmount] [decimal](25, 9) NULL,
	[InsuranceAmount] [decimal](25, 9) NULL,
	[AssessableValue] [decimal](25, 9) NULL,
	[CDAmount] [decimal](25, 9) NULL,
	[RDAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TVBAmount] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVAAmount] [decimal](25, 9) NULL,
	[ATVAmount] [decimal](25, 9) NULL,
	[OthersAmount] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[PurchaseReturnId] [varchar](20) NULL,
 CONSTRAINT [PK_PurchaseInvoiceDetails] PRIMARY KEY CLUSTERED 
(
	[PurchaseInvoiceNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceDuties]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceDuties](
	[PIDutyID] [varchar](50) NOT NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[CnFInp] [decimal](25, 9) NULL,
	[CnFRate] [decimal](25, 9) NULL,
	[CnFAmount] [decimal](25, 9) NULL,
	[InsuranceInp] [decimal](25, 9) NULL,
	[InsuranceRate] [decimal](25, 9) NULL,
	[InsuranceAmount] [decimal](25, 9) NULL,
	[AssessableInp] [decimal](25, 9) NULL,
	[AssessableValue] [decimal](25, 9) NULL,
	[CDInp] [decimal](25, 9) NULL,
	[CDRate] [decimal](25, 9) NULL,
	[CDAmount] [decimal](25, 9) NULL,
	[RDInp] [decimal](25, 9) NULL,
	[RDRate] [decimal](25, 9) NULL,
	[RDAmount] [decimal](25, 9) NULL,
	[TVBInp] [decimal](25, 9) NULL,
	[TVBRate] [decimal](25, 9) NULL,
	[TVBAmount] [decimal](25, 9) NULL,
	[SDInp] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATInp] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVAInp] [decimal](25, 9) NULL,
	[TVARate] [decimal](25, 9) NULL,
	[TVAAmount] [decimal](25, 9) NULL,
	[ATVInp] [decimal](25, 9) NULL,
	[ATVRate] [decimal](25, 9) NULL,
	[ATVAmount] [decimal](25, 9) NULL,
	[OthersInp] [decimal](25, 9) NULL,
	[OthersRate] [decimal](25, 9) NULL,
	[OthersAmount] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[Remarks] [varchar](200) NULL,
	[ItemNo] [varchar](20) NULL,
	[DutyCompleteQuantity] [decimal](25, 9) NULL,
	[DutyCompleteQuantityPercent] [decimal](25, 9) NULL,
	[LineCost] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceHeaders](
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[VendorID] [varchar](20) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[BENumber] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[ProductType] [varchar](100) NULL,
	[TransactionType] [varchar](50) NULL,
	[ReceiveDate] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[CurrencyID] [varchar](50) NULL,
	[CurrencyRateFromBDT] [decimal](25, 9) NULL,
	[WithVDS] [varchar](1) NULL,
	[PurchaseReturnId] [varchar](20) NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[SerialNo1] [varchar](50) NULL,
 CONSTRAINT [PK_ProductInvoiceHead] PRIMARY KEY CLUSTERED 
(
	[PurchaseInvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReceiveDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceiveDetails](
	[ReceiveNo] [varchar](20) NOT NULL,
	[ReceiveLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[ReceiveDateTime] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[VATName] [varchar](100) NULL,
	[ReceiveReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[BOMId] [varchar](20) NULL,
	[BOMId1] [varchar](20) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[DollerValue] [decimal](25, 9) NULL,
 CONSTRAINT [PK_ReceiveDetails] PRIMARY KEY CLUSTERED 
(
	[ReceiveNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReceiveHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceiveHeaders](
	[ReceiveNo] [varchar](20) NOT NULL,
	[ReceiveDateTime] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[ReceiveReturnId] [varchar](20) NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[ReferenceNo] [varchar](50) NULL,
 CONSTRAINT [PK_ReceiveHead] PRIMARY KEY CLUSTERED 
(
	[ReceiveNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportSales]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportSales](
	[AuditUser] [varchar](10) NULL,
	[OpeQty] [money] NULL,
	[PrdQty] [money] NULL,
	[InvoiceNo] [varchar](20) NULL,
	[CustomerName] [varchar](120) NULL,
	[Address] [varchar](200) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[InvoiceDate] [datetime] NULL,
	[ItemNo] [varchar](20) NULL,
	[ProductName] [varchar](120) NULL,
	[Quantity] [decimal](18, 5) NULL,
	[SalesPrice] [decimal](18, 5) NULL,
	[ClosingQuantity] [decimal](18, 5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceDetails](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[InvoiceLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[SalesPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[AVGPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[SaleType] [varchar](10) NULL,
	[PreviousSalesInvoiceNo] [varchar](200) NULL,
	[Trading] [varchar](1) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[NonStock] [varchar](1) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[Type] [varchar](10) NULL,
	[BENumber] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[VATName] [varchar](100) NULL,
	[SaleReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[PromotionalQuantity] [decimal](25, 9) NULL,
	[FinishItemNo] [varchar](20) NULL,
 CONSTRAINT [PK_SalesInvoiceDetails_1] PRIMARY KEY CLUSTERED 
(
	[SalesInvoiceNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceHeaders](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[CustomerID] [varchar](20) NOT NULL,
	[DeliveryAddress1] [varchar](200) NULL,
	[DeliveryAddress2] [varchar](200) NULL,
	[DeliveryAddress3] [varchar](200) NULL,
	[VehicleID] [varchar](20) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[DeliveryDate] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SaleType] [varchar](20) NULL,
	[PreviousSalesInvoiceNo] [varchar](20) NULL,
	[Trading] [varchar](1) NULL,
	[IsPrint] [varchar](1) NULL,
	[TenderId] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[LCNumber] [varchar](50) NULL,
	[CurrencyID] [varchar](50) NULL,
	[CurrencyRateFromBDT] [decimal](25, 9) NULL,
	[SaleReturnId] [varchar](20) NULL,
	[IsVDS] [varchar](1) NULL,
	[GetVDSCertificate] [varchar](1) NULL,
	[VDSCertificateDate] [datetime] NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[AlReadyPrint] [int] NULL,
 CONSTRAINT [PK_SalesInvoiceHead] PRIMARY KEY CLUSTERED 
(
	[SalesInvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceHeadersExport]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceHeadersExport](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SaleLineNo] [int] NULL,
	[RefNo] [varchar](200) NULL,
	[Description] [varchar](200) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[GrossWeight] [decimal](25, 9) NULL,
	[NetWeight] [decimal](25, 9) NULL,
	[NumberFrom] [varchar](120) NULL,
	[NumberTo] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[PortFrom] [varchar](500) NULL,
	[PortTo] [varchar](500) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SDDeposits]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SDDeposits](
	[DepositId] [varchar](20) NOT NULL,
	[TreasuryNo] [varchar](50) NULL,
	[DepositDateTime] [datetime] NULL,
	[DepositType] [varchar](50) NULL,
	[DepositAmount] [decimal](25, 9) NULL,
	[ChequeNo] [varchar](50) NULL,
	[ChequeBank] [varchar](120) NULL,
	[ChequeBankBranch] [varchar](120) NULL,
	[ChequeDate] [datetime] NULL,
	[BankID] [varchar](20) NULL,
	[TreasuryCopy] [varchar](20) NULL,
	[DepositPerson] [varchar](120) NULL,
	[DepositPersonDesignation] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
 CONSTRAINT [PK_SDDeposit] PRIMARY KEY CLUSTERED 
(
	[DepositId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [varchar](120) NULL,
	[SettingType] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Setup]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Setup](
	[PurchaseP] [varchar](3) NULL,
	[PurchaseIDL] [numeric](10, 0) NULL,
	[PurchaseCID] [numeric](10, 0) NULL,
	[PurchaseNYID] [varchar](1) NULL,
	[PurchaseTradingP] [varchar](3) NULL,
	[PurchaseTradingIDL] [numeric](10, 0) NULL,
	[PurchaseTradingCID] [numeric](10, 0) NULL,
	[PurchaseTradingNYID] [varchar](1) NULL,
	[IssueP] [varchar](3) NULL,
	[IssueIDL] [numeric](10, 0) NULL,
	[IssueCID] [numeric](10, 0) NULL,
	[IssueNYID] [varchar](1) NULL,
	[IssueReturnP] [varchar](3) NULL,
	[IssueReturnIDL] [numeric](10, 0) NULL,
	[IssueReturnCID] [numeric](10, 0) NULL,
	[IssueReturnNYID] [varchar](1) NULL,
	[ReceiveP] [varchar](3) NULL,
	[ReceiveIDL] [numeric](10, 0) NULL,
	[ReceiveCID] [numeric](10, 0) NULL,
	[ReceiveNYID] [varchar](1) NULL,
	[TransferP] [varchar](3) NULL,
	[TransferIDL] [numeric](10, 0) NULL,
	[TransferCID] [numeric](10, 0) NULL,
	[TransferNYID] [varchar](1) NULL,
	[SaleP] [varchar](3) NULL,
	[SaleIDL] [numeric](10, 0) NULL,
	[SaleCID] [numeric](10, 0) NULL,
	[SaleNYID] [varchar](1) NULL,
	[SaleServiceP] [varchar](3) NULL,
	[SaleServiceIDL] [numeric](10, 0) NULL,
	[SaleServiceCID] [numeric](10, 0) NULL,
	[SaleServiceNYID] [varchar](1) NULL,
	[SaleTradingP] [varchar](3) NULL,
	[SaleTradingIDL] [numeric](10, 0) NULL,
	[SaleTradingCID] [numeric](10, 0) NULL,
	[SaleTradingNYID] [varchar](1) NULL,
	[SaleExportP] [varchar](3) NULL,
	[SaleExportIDL] [numeric](10, 0) NULL,
	[SaleExportCID] [numeric](10, 0) NULL,
	[SaleExportNYID] [varchar](1) NULL,
	[SaleTenderP] [varchar](3) NULL,
	[SaleTenderIDL] [numeric](10, 0) NULL,
	[SaleTenderCID] [numeric](10, 0) NULL,
	[SaleTenderNYID] [varchar](1) NULL,
	[DNP] [varchar](3) NULL,
	[DNIDL] [numeric](10, 0) NULL,
	[DNCID] [numeric](10, 0) NULL,
	[DNNYID] [varchar](1) NULL,
	[CNP] [varchar](3) NULL,
	[CNIDL] [numeric](10, 0) NULL,
	[CNCID] [numeric](10, 0) NULL,
	[CNNYID] [varchar](1) NULL,
	[DepositP] [varchar](3) NULL,
	[DepositIDL] [numeric](10, 0) NULL,
	[DepositCID] [numeric](10, 0) NULL,
	[DepositNYID] [varchar](1) NULL,
	[VDSP] [varchar](3) NULL,
	[VDSIDL] [numeric](10, 0) NULL,
	[VDSCID] [numeric](10, 0) NULL,
	[VDSNYID] [varchar](1) NULL,
	[TollIssueP] [varchar](3) NULL,
	[TollIssueIDL] [numeric](10, 0) NULL,
	[TollIssueCID] [numeric](10, 0) NULL,
	[TollIssueNYID] [varchar](1) NULL,
	[TollReceiveP] [varchar](3) NULL,
	[TollReceiveIDL] [numeric](10, 0) NULL,
	[TollReceiveCID] [numeric](10, 0) NULL,
	[TollReceiveNYID] [varchar](1) NULL,
	[DSFP] [varchar](3) NULL,
	[DSFIDL] [numeric](10, 0) NULL,
	[DSFCID] [numeric](10, 0) NULL,
	[DSFNYID] [varchar](1) NULL,
	[DSRP] [varchar](3) NULL,
	[DSRIDL] [numeric](10, 0) NULL,
	[DSRCID] [numeric](10, 0) NULL,
	[DSRNYID] [varchar](1) NULL,
	[IssueFromBOM] [varchar](1) NULL,
	[PrepaidVAT] [varchar](1) NULL,
	[CYear] [varchar](4) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TenderDetails]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TenderDetails](
	[TenderId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[TenderQty] [decimal](25, 9) NULL,
	[SaleQty] [decimal](25, 9) NULL,
	[TenderPrice] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[BOMId] [varchar](20) NULL,
 CONSTRAINT [PK_TenderDetails] PRIMARY KEY CLUSTERED 
(
	[TenderId] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TenderHeaders]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TenderHeaders](
	[TenderId] [varchar](20) NOT NULL,
	[RefNo] [varchar](200) NOT NULL,
	[CustomerId] [varchar](20) NULL,
	[TenderDate] [datetime] NULL,
	[DeleveryDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
 CONSTRAINT [PK_TenderHeaders] PRIMARY KEY CLUSTERED 
(
	[TenderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionHistorys]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionHistorys](
	[TransactionNo] [varchar](20) NULL,
	[TransactionType] [varchar](50) NULL,
	[TransactionDate] [datetime] NULL,
	[ItemNo] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[TradingMarkup] [decimal](25, 9) NULL,
	[SDRate] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[TCost] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionID] [varchar](20) NULL,
	[TransactionType] [varchar](200) NULL,
	[TransactionDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UOMName]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UOMName](
	[UOMId] [int] IDENTITY(1,1) NOT NULL,
	[UOMName] [varchar](500) NULL,
	[UOMCode] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UOMs]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UOMs](
	[UOMId] [varchar](50) NULL,
	[UOMFrom] [varchar](50) NULL,
	[UOMTo] [varchar](50) NULL,
	[Convertion] [decimal](25, 9) NULL,
	[CTypes] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ActiveStatus] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserAuditLogs]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserAuditLogs](
	[LogID] [varchar](50) NULL,
	[ComputerName] [varchar](200) NULL,
	[ComputerLoginUserName] [varchar](200) NULL,
	[ComputerIPAddress] [varchar](200) NULL,
	[SoftwareUserId] [varchar](200) NULL,
	[SessionDate] [datetime] NULL,
	[LogInDateTime] [datetime] NULL,
	[LogOutDateTime] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserGroups]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[UserGroups](
	[GroupID] [varchar](20) NULL,
	[GroupName] [varchar](120) NOT NULL,
	[Comments] [varchar](200) NOT NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_UserGroups] PRIMARY KEY CLUSTERED 
(
	[GroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserInformations]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInformations](
	[UserID] [varchar](20) NULL,
	[UserName] [varchar](120) NOT NULL,
	[UserPassword] [varchar](20) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[LastLoginDateTime] [datetime] NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[GroupID] [varchar](20) NULL,
 CONSTRAINT [PK_UserInformations] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLogs]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserLogs](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](120) NULL,
	[LoginTime] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRolls]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRolls](
	[LineID] [numeric](18, 0) NOT NULL,
	[UserID] [varchar](20) NOT NULL,
	[FormID] [varchar](5) NOT NULL,
	[Access] [varchar](1) NULL,
	[PostAccess] [varchar](1) NULL,
	[FormName] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_UserRolls] PRIMARY KEY CLUSTERED 
(
	[LineID] ASC,
	[UserID] ASC,
	[FormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VDS]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VDS](
	[VDSId] [varchar](20) NULL,
	[VendorId] [varchar](20) NULL,
	[BillAmount] [decimal](25, 9) NULL,
	[BillDate] [datetime] NULL,
	[BillDeductAmount] [decimal](25, 9) NULL,
	[DepositNumber] [varchar](30) NULL,
	[DepositDate] [datetime] NULL,
	[Remarks] [varchar](200) NULL,
	[IssueDate] [datetime] NULL,
	[PurchaseNumber] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[VDSPercent] [decimal](25, 9) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Vehicles]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vehicles](
	[VehicleID] [varchar](20) NOT NULL,
	[VehicleCode] [varchar](50) NULL,
	[VehicleType] [varchar](50) NULL,
	[VehicleNo] [varchar](50) NULL,
	[Description] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VendorGroups]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendorGroups](
	[VendorGroupID] [varchar](20) NOT NULL,
	[VendorGroupName] [varchar](120) NULL,
	[VendorGroupDescription] [varchar](120) NULL,
	[GroupType] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[Info2] [varchar](10) NULL,
 CONSTRAINT [PK_VendorGroup] PRIMARY KEY CLUSTERED 
(
	[VendorGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vendors](
	[VendorID] [varchar](20) NOT NULL,
	[VendorCode] [varchar](50) NULL,
	[VendorName] [varchar](120) NULL,
	[VendorGroupID] [varchar](20) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[ContactPerson] [varchar](150) NULL,
	[ContactPersonDesignation] [varchar](150) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Country] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[VDSPercent] [decimal](25, 9) NULL,
	[BusinessType] [varchar](120) NULL,
	[BusinessCode] [varchar](20) NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[YourTable]    Script Date: 12/23/2013 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[YourTable](
	[BOMId] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Duties] ADD  CONSTRAINT [DF_Duties_Comments]  DEFAULT ('NA') FOR [Comments]
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties] ADD  CONSTRAINT [DF_Table_1_Comments_1]  DEFAULT ('NA') FOR [Remarks]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] ADD  CONSTRAINT [DF_SalesInvoiceDetails_AVGPrice]  DEFAULT ((0)) FOR [AVGPrice]
GO
ALTER TABLE [dbo].[BOMCompanyOverhead]  WITH CHECK ADD  CONSTRAINT [FK_BOMCompanyOverhead_BOMId] FOREIGN KEY([BOMId])
REFERENCES [dbo].[BOMs] ([BOMId])
GO
ALTER TABLE [dbo].[BOMCompanyOverhead] CHECK CONSTRAINT [FK_BOMCompanyOverhead_BOMId]
GO
ALTER TABLE [dbo].[BOMRaws]  WITH CHECK ADD  CONSTRAINT [FK_BOMRaws_BOMId] FOREIGN KEY([BOMId])
REFERENCES [dbo].[BOMs] ([BOMId])
GO
ALTER TABLE [dbo].[BOMRaws] CHECK CONSTRAINT [FK_BOMRaws_BOMId]
GO
ALTER TABLE [dbo].[BOMs]  WITH CHECK ADD  CONSTRAINT [FK_BOMs_FinishItemNo] FOREIGN KEY([FinishItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[BOMs] CHECK CONSTRAINT [FK_BOMs_FinishItemNo]
GO
ALTER TABLE [dbo].[CompanyOverheadVAT]  WITH CHECK ADD  CONSTRAINT [FK_CompanyOverheadVAT_CompanyOverheads] FOREIGN KEY([HeadName])
REFERENCES [dbo].[CompanyOverheads] ([HeadName])
GO
ALTER TABLE [dbo].[CompanyOverheadVAT] CHECK CONSTRAINT [FK_CompanyOverheadVAT_CompanyOverheads]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_CustomerGroups1] FOREIGN KEY([CustomerGroupID])
REFERENCES [dbo].[CustomerGroups] ([CustomerGroupID])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_CustomerGroups1]
GO
ALTER TABLE [dbo].[DisposeDetails]  WITH CHECK ADD  CONSTRAINT [FK_DisposeDetails_DisposeHeaders] FOREIGN KEY([DisposeNumber])
REFERENCES [dbo].[DisposeHeaders] ([DisposeNumber])
GO
ALTER TABLE [dbo].[DisposeDetails] CHECK CONSTRAINT [FK_DisposeDetails_DisposeHeaders]
GO
ALTER TABLE [dbo].[DisposeDetails]  WITH CHECK ADD  CONSTRAINT [FK_DisposeDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[DisposeDetails] CHECK CONSTRAINT [FK_DisposeDetails_Products]
GO
ALTER TABLE [dbo].[IssueDetails]  WITH CHECK ADD  CONSTRAINT [FK_IssueDetails_IssueHeaders] FOREIGN KEY([IssueNo])
REFERENCES [dbo].[IssueHeaders] ([IssueNo])
GO
ALTER TABLE [dbo].[IssueDetails] CHECK CONSTRAINT [FK_IssueDetails_IssueHeaders]
GO
ALTER TABLE [dbo].[IssueDetails]  WITH CHECK ADD  CONSTRAINT [FK_IssueDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[IssueDetails] CHECK CONSTRAINT [FK_IssueDetails_Products]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ProductCategories1] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ProductCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ProductCategories1]
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseInvoiceDuties_PurchaseInvoiceHeaders] FOREIGN KEY([PurchaseInvoiceNo])
REFERENCES [dbo].[PurchaseInvoiceHeaders] ([PurchaseInvoiceNo])
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties] CHECK CONSTRAINT [FK_PurchaseInvoiceDuties_PurchaseInvoiceHeaders]
GO
ALTER TABLE [dbo].[PurchaseInvoiceHeaders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseInvoiceHeaders_Vendors] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendors] ([VendorID])
GO
ALTER TABLE [dbo].[PurchaseInvoiceHeaders] CHECK CONSTRAINT [FK_PurchaseInvoiceHeaders_Vendors]
GO
ALTER TABLE [dbo].[ReceiveDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[ReceiveDetails] CHECK CONSTRAINT [FK_ReceiveDetails_Products]
GO
ALTER TABLE [dbo].[ReceiveDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveDetails_ReceiveHeaders] FOREIGN KEY([ReceiveNo])
REFERENCES [dbo].[ReceiveHeaders] ([ReceiveNo])
GO
ALTER TABLE [dbo].[ReceiveDetails] CHECK CONSTRAINT [FK_ReceiveDetails_ReceiveHeaders]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceDetails_Products1] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] CHECK CONSTRAINT [FK_SalesInvoiceDetails_Products1]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceDetails_SalesInvoiceHeaders1] FOREIGN KEY([SalesInvoiceNo])
REFERENCES [dbo].[SalesInvoiceHeaders] ([SalesInvoiceNo])
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] CHECK CONSTRAINT [FK_SalesInvoiceDetails_SalesInvoiceHeaders1]
GO
ALTER TABLE [dbo].[SalesInvoiceHeaders]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceHeaders_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SalesInvoiceHeaders] CHECK CONSTRAINT [FK_SalesInvoiceHeaders_Customers]
GO
ALTER TABLE [dbo].[SalesInvoiceHeadersExport]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceHeadersExport_SalesInvoiceHeaders] FOREIGN KEY([SalesInvoiceNo])
REFERENCES [dbo].[SalesInvoiceHeaders] ([SalesInvoiceNo])
GO
ALTER TABLE [dbo].[SalesInvoiceHeadersExport] CHECK CONSTRAINT [FK_SalesInvoiceHeadersExport_SalesInvoiceHeaders]
GO
ALTER TABLE [dbo].[TenderDetails]  WITH CHECK ADD  CONSTRAINT [FK_TenderDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[TenderDetails] CHECK CONSTRAINT [FK_TenderDetails_Products]
GO
ALTER TABLE [dbo].[TenderDetails]  WITH CHECK ADD  CONSTRAINT [FK_TenderDetails_TenderHeaders] FOREIGN KEY([TenderId])
REFERENCES [dbo].[TenderHeaders] ([TenderId])
GO
ALTER TABLE [dbo].[TenderDetails] CHECK CONSTRAINT [FK_TenderDetails_TenderHeaders]
GO
ALTER TABLE [dbo].[TenderHeaders]  WITH CHECK ADD  CONSTRAINT [FK_TenderHeaders_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[TenderHeaders] CHECK CONSTRAINT [FK_TenderHeaders_Customers]
GO
ALTER TABLE [dbo].[UserLogs]  WITH CHECK ADD  CONSTRAINT [FK_UserLogs_UserInformations] FOREIGN KEY([UserName])
REFERENCES [dbo].[UserInformations] ([UserName])
GO
ALTER TABLE [dbo].[UserLogs] CHECK CONSTRAINT [FK_UserLogs_UserInformations]
GO
ALTER TABLE [dbo].[Vendors]  WITH CHECK ADD  CONSTRAINT [FK_Vendors_VendorGroups1] FOREIGN KEY([VendorGroupID])
REFERENCES [dbo].[VendorGroups] ([VendorGroupID])
GO
ALTER TABLE [dbo].[Vendors] CHECK CONSTRAINT [FK_Vendors_VendorGroups1]
GO
                ";
                //25-2-2014 User Settings and Banderol
                sqlText += @"
/****** Object:  Table [dbo].[SettingsRole]    Script Date: 12/02/14 10:24:55 AM ******/
CREATE TABLE [dbo].[SettingsRole](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](20) NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [varchar](120) NULL,
	[SettingType] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

";

                #endregion CreateTable

                top1 = "go";

                IEnumerable<string> commandStrings = Regex.Split(sqlText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        SqlCommand cmdIdExist1 = new SqlCommand(commandString, currConn);

                        //new SqlCommand(commandString, currConn).ExecuteNonQuery();
                        cmdIdExist1.Transaction = transaction;
                        transResult = (int)cmdIdExist1.ExecuteNonQuery();
                        if (transResult != -1)
                        {
                            throw new ArgumentNullException("Create Tables to database('" + databaseName + "')", MessageVM.dbMsgTableNotCreate);
                        }
                    }
                }

                #endregion TableCreate

                #region TableDefaultData
                string top2;
                // vendor group, vehicle,UserInformations,CustomerGroups,Vendors,Customers
                //userroll,,settings,ProductTypes,codes,Currencies,CurrencyConversion
                #region TableDefaultData Back
                //             sqlText = @"
                //INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)
                //INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[UserInformations] ([UserID], [UserName], [UserPassword], [ActiveStatus], [LastLoginDateTime], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'10', N'admin', N'J7LJ8+qT64o=', N'Y', CAST(0x0000A04D00B82888 AS DateTime), N'KamrulInsert', CAST(0x0000A01400EF44BC AS DateTime), N'admin', CAST(0x0000A08400D5C30C AS DateTime), N'Info1', N'Info2', N'Info3', N'Info4', N'Info5')
                //INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL)


                //";
                #endregion TableDefaultData Back

                #region TableDefaultData Back
                sqlText = @"
INSERT [BankInformations] ([BankID], [BankCode], [BankName], [BranchName], [AccountNumber], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'0', N'NA', N'NA', N'NA', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'Y', N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[UserInformations] ([UserID], [UserName], [UserPassword], [ActiveStatus], [LastLoginDateTime], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'10', N'admin', N'J7LJ8+qT64o=', N'Y', CAST(0x0000A04D00B82888 AS DateTime), N'KamrulInsert', CAST(0x0000A01400EF44BC AS DateTime), N'admin', CAST(0x0000A08400D5C30C AS DateTime), N'Info1', N'Info2', N'Info3', N'Info4', N'Info5')
INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')

INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)
INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL)


INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'0', N'NA', N'NA', N'NA', N'Overhead', N'0.00', CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'admin', CAST(0x0000A16400F8CA3C AS DateTime), N'admin', CAST(0x0000A1A30106ECFC AS DateTime), CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'NA', N'NA')

INSERT [dbo].[Products] ([ItemNo], [ProductCode], [ProductName], [ProductDescription], [CategoryID], [UOM], [CostPrice], [SalesPrice], [NBRPrice], [ReceivePrice], [IssuePrice], [TenderPrice], [ExportPrice], [InternalIssuePrice], [TollIssuePrice], [TollCharge], [OpeningBalance], [SerialNo], [HSCodeNo], [VATRate], [Comments], [SD], [PacketPrice], [Trading], [TradingMarkUp], [NonStock], [QuantityInHand], [OpeningDate], [RebatePercent], [TVBRate], [CnFRate], [InsuranceRate], [CDRate], [RDRate], [AITRate], [TVARate], [ATVRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [OpeningTotalCost]) VALUES (N'ovh0', N'ovh0', N'Margin', N'-', N'0', N'-', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'-', N'', CAST(0.000000000 AS Decimal(25, 9)), N'', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), CAST(0x0000A1A40105ED84 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Y', N'admin', CAST(0x0000A1A401060044 AS DateTime), N'admin', CAST(0x0000A1A401224A74 AS DateTime), NULL)


INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(1 AS Numeric(18, 0)), N'10', N'1101', N'Y', N'Y', N'Setup/ItemInformation/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(2 AS Numeric(18, 0)), N'10', N'1102', N'Y', N'Y', N'Setup/ItemInformation/Product', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(3 AS Numeric(18, 0)), N'10', N'1103', N'Y', N'Y', N'Setup/ItemInformation/Overhead', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(4 AS Numeric(18, 0)), N'10', N'1201', N'Y', N'Y', N'Setup/Vedor/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(5 AS Numeric(18, 0)), N'10', N'1202', N'Y', N'Y', N'Setup/Vedor/Vendor', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(6 AS Numeric(18, 0)), N'10', N'1301', N'Y', N'Y', N'Setup/Customer/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(7 AS Numeric(18, 0)), N'10', N'1302', N'Y', N'Y', N'Setup/Customer/Customer', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(8 AS Numeric(18, 0)), N'10', N'1401', N'Y', N'Y', N'Setup/Bank/Bank', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(9 AS Numeric(18, 0)), N'10', N'1501', N'Y', N'Y', N'Setup/Vehicle/Vehicle', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(10 AS Numeric(18, 0)), N'10', N'1601', N'Y', N'Y', N'Setup/PriceDeclaration/VAT-1', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(11 AS Numeric(18, 0)), N'10', N'1602', N'Y', N'Y', N'Setup/PriceDeclaration/Service', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(12 AS Numeric(18, 0)), N'10', N'1603', N'Y', N'Y', N'Setup/PriceDeclaration/Tender', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(13 AS Numeric(18, 0)), N'10', N'1701', N'Y', N'Y', N'Setup/Company/Commpany', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(14 AS Numeric(18, 0)), N'10', N'1801', N'Y', N'Y', N'Setup/FiscalYear/FiscalYear', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(15 AS Numeric(18, 0)), N'10', N'1901', N'Y', N'Y', N'Setup/Configuration/Settings', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(16 AS Numeric(18, 0)), N'10', N'1902', N'Y', N'Y', N'Setup/Configuration/Prefix', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(17 AS Numeric(18, 0)), N'10', N'11001', N'Y', N'Y', N'Setup/Import/Import', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(18 AS Numeric(18, 0)), N'10', N'11101', N'Y', N'Y', N'Setup/Conversion/Conversion', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(19 AS Numeric(18, 0)), N'10', N'11201', N'Y', N'Y', N'Setup/Currency/Currency', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(20 AS Numeric(18, 0)), N'10', N'11202', N'Y', N'Y', N'Setup/Currency/Conversion', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(21 AS Numeric(18, 0)), N'10', N'2101', N'Y', N'Y', N'Purchase/Purchase/Local', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(22 AS Numeric(18, 0)), N'10', N'2102', N'Y', N'Y', N'Purchase/Purchase/Trading', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(23 AS Numeric(18, 0)), N'10', N'2103', N'Y', N'Y', N'Purchase/Purchase/Import', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(24 AS Numeric(18, 0)), N'10', N'2104', N'Y', N'Y', N'Purchase/Purchase/InputService', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(25 AS Numeric(18, 0)), N'10', N'2105', N'Y', N'Y', N'Purchase/Purchase/PurchaseReturn', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(26 AS Numeric(18, 0)), N'10', N'3101', N'Y', N'Y', N'Production/Issue/Issue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(27 AS Numeric(18, 0)), N'10', N'3102', N'Y', N'Y', N'Production/Issue/Return', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(28 AS Numeric(18, 0)), N'10', N'3201', N'Y', N'Y', N'Production/Receive/WIP', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(29 AS Numeric(18, 0)), N'10', N'3202', N'Y', N'Y', N'Production/Receive/FGReceive', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(30 AS Numeric(18, 0)), N'10', N'3203', N'Y', N'Y', N'Production/Receive/Return', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(31 AS Numeric(18, 0)), N'10', N'4101', N'Y', N'Y', N'Sale/Sale/Local', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(32 AS Numeric(18, 0)), N'10', N'4102', N'Y', N'Y', N'Sale/Sale/Service', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(33 AS Numeric(18, 0)), N'10', N'4103', N'Y', N'Y', N'Sale/Sale/Trading', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(34 AS Numeric(18, 0)), N'10', N'4104', N'Y', N'Y', N'Sale/Sale/Export', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(35 AS Numeric(18, 0)), N'10', N'4105', N'Y', N'Y', N'Sale/Sale/Tender', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(36 AS Numeric(18, 0)), N'10', N'4201', N'Y', N'Y', N'Sale/Transfer/Transfer', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(37 AS Numeric(18, 0)), N'10', N'5101', N'Y', N'Y', N'Deposit/Treasury/Treasury', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(38 AS Numeric(18, 0)), N'10', N'5201', N'Y', N'Y', N'Deposit/VDS/VDS', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(39 AS Numeric(18, 0)), N'10', N'5301', N'Y', N'Y', N'Deposit/SD/SD', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(40 AS Numeric(18, 0)), N'10', N'6101', N'Y', N'Y', N'Toll/Client/RawIssue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(41 AS Numeric(18, 0)), N'10', N'6102', N'Y', N'Y', N'Toll/Client/FGReceive', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(42 AS Numeric(18, 0)), N'10', N'6201', N'Y', N'Y', N'Toll/Contractor/RawReceive', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(43 AS Numeric(18, 0)), N'10', N'6202', N'Y', N'Y', N'Toll/Contractor/FGProduction', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(44 AS Numeric(18, 0)), N'10', N'6203', N'Y', N'Y', N'Toll/Contractor/FGIssue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(45 AS Numeric(18, 0)), N'10', N'7101', N'Y', N'Y', N'Adjustment/AdjustmentHead/Head', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(46 AS Numeric(18, 0)), N'10', N'7102', N'Y', N'Y', N'Adjustment/AdjustmentHead/Transaction', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(47 AS Numeric(18, 0)), N'10', N'7201', N'Y', N'Y', N'Adjustment/Purchase/DN', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(48 AS Numeric(18, 0)), N'10', N'7202', N'Y', N'Y', N'Adjustment/Purchase/CN', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(49 AS Numeric(18, 0)), N'10', N'7301', N'Y', N'Y', N'Adjustment/Sale/CN', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(50 AS Numeric(18, 0)), N'10', N'7302', N'Y', N'Y', N'Adjustment/Sale/DN', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(51 AS Numeric(18, 0)), N'10', N'7401', N'Y', N'Y', N'Adjustment/Dispose/26', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(52 AS Numeric(18, 0)), N'10', N'7402', N'Y', N'Y', N'Adjustment/Dispose/27', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(53 AS Numeric(18, 0)), N'10', N'7501', N'Y', N'Y', N'Adjustment/DDB/DDB', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(54 AS Numeric(18, 0)), N'10', N'8101', N'Y', N'Y', N'NBRReport/VAT1/BOM', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(55 AS Numeric(18, 0)), N'10', N'8201', N'Y', N'Y', N'NBRReport/VAT16/VAT16', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(56 AS Numeric(18, 0)), N'10', N'8301', N'Y', N'Y', N'NBRReport/VAT17/VAT17', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(57 AS Numeric(18, 0)), N'10', N'8401', N'Y', N'Y', N'NBRReport/VAT18/VAT18', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(58 AS Numeric(18, 0)), N'10', N'8501', N'Y', N'Y', N'NBRReport/VAT19/VAT19', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(59 AS Numeric(18, 0)), N'10', N'8601', N'Y', N'Y', N'NBRReport/SDReport/SDReport', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(60 AS Numeric(18, 0)), N'10', N'9101', N'Y', N'Y', N'MISReport/Purchase/Purchase', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(61 AS Numeric(18, 0)), N'10', N'9102', N'Y', N'Y', N'MISReport/Purchase/Trading', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(62 AS Numeric(18, 0)), N'10', N'9201', N'Y', N'Y', N'MISReport/Production/Issue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(63 AS Numeric(18, 0)), N'10', N'9202', N'Y', N'Y', N'MISReport/Production/IssueReturn', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(64 AS Numeric(18, 0)), N'10', N'9203', N'Y', N'Y', N'MISReport/Production/Receive', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(65 AS Numeric(18, 0)), N'10', N'9204', N'Y', N'Y', N'MISReport/Production/InnerIssue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(66 AS Numeric(18, 0)), N'10', N'9301', N'Y', N'Y', N'MISReport/Toll/Issue', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(67 AS Numeric(18, 0)), N'10', N'9302', N'Y', N'Y', N'MISReport/Toll/Receive', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(68 AS Numeric(18, 0)), N'10', N'9401', N'Y', N'Y', N'MISReport/Sale/Local', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(69 AS Numeric(18, 0)), N'10', N'9402', N'Y', N'Y', N'MISReport/Sale/Service', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(70 AS Numeric(18, 0)), N'10', N'9403', N'Y', N'Y', N'MISReport/Sale/Trading', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(71 AS Numeric(18, 0)), N'10', N'9404', N'Y', N'Y', N'MISReport/Sale/Export', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(72 AS Numeric(18, 0)), N'10', N'9501', N'Y', N'Y', N'MISReport/Stock/Stock', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(73 AS Numeric(18, 0)), N'10', N'9601', N'Y', N'Y', N'MISReport/Deposit/Deposit', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(74 AS Numeric(18, 0)), N'10', N'9701', N'Y', N'Y', N'MISReport/VAT16/VAT16', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(75 AS Numeric(18, 0)), N'10', N'9801', N'Y', N'Y', N'MISReport/VAT17/VAT17', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(76 AS Numeric(18, 0)), N'10', N'9901', N'Y', N'Y', N'MISReport/VAT18/VAT18', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(77 AS Numeric(18, 0)), N'10', N'91001', N'Y', N'Y', N'MISReport/SDDeposit/SDDeposit', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(78 AS Numeric(18, 0)), N'10', N'10101', N'Y', N'Y', N'SetupReport/Product/Type', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(79 AS Numeric(18, 0)), N'10', N'10102', N'Y', N'Y', N'SetupReport/Product/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(80 AS Numeric(18, 0)), N'10', N'10103', N'Y', N'Y', N'SetupReport/Product/Product', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(81 AS Numeric(18, 0)), N'10', N'10201', N'Y', N'Y', N'SetupReport/Customer/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(82 AS Numeric(18, 0)), N'10', N'10202', N'Y', N'Y', N'SetupReport/Customer/Customer', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(83 AS Numeric(18, 0)), N'10', N'10301', N'Y', N'Y', N'SetupReport/Vendor/Group', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(84 AS Numeric(18, 0)), N'10', N'10302', N'Y', N'Y', N'SetupReport/Vendor/Vendor', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(85 AS Numeric(18, 0)), N'10', N'10401', N'Y', N'Y', N'SetupReport/Bank/Bank', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(86 AS Numeric(18, 0)), N'10', N'10501', N'Y', N'Y', N'SetupReport/Vehicle/Vehicle', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(87 AS Numeric(18, 0)), N'10', N'20101', N'Y', N'Y', N'UserAccount/NewAccount/NewAccount', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(88 AS Numeric(18, 0)), N'10', N'20201', N'Y', N'Y', N'UserAccount/PasswordChange/PasswordChange', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))
INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (CAST(89 AS Numeric(18, 0)), N'10', N'43', N'Y', N'Y', N'UserAccount/UserRole/UserRole', N'admin', CAST(0x0000A1E700BBA328 AS DateTime), N'admin', CAST(0x0000A1E700BBA328 AS DateTime))

/****** Object:  Table [dbo].[Settings]    Script Date: 04/18/2013 12:35:04 ******/
SET IDENTITY_INSERT [dbo].[Settings] ON
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'Purchase', N'TotalPrice', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'Purchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (3, N'Sale', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'Issue', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'Sale', N'QuantityDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'Sale', N'TakaDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'Sale', N'DollerDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'Sale', N'RateDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (9, N'Sale', N'InvoiceDate', N'2013-01-04', N'date', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (10, N'Sale', N'CustomerName', N'-', N'string', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'ImportPurchase', N'FixedCnF', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'ImportPurchase', N'FixedInsurance', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'ImportPurchase', N'CalculativeAV', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'ImportPurchase', N'FixedCD', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'ImportPurchase', N'FixedRD', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'ImportPurchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'ImportPurchase', N'FixedTVB', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'ImportPurchase', N'FixedTVA', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'ImportPurchase', N'FixedATV', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'ImportPurchase', N'FixedOthers', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'ImportPurchase', N'FixedSD', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'AutoCode', N'Item', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'AutoCode', N'Customer', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'AutoCode', N'Vendor', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'AutoCode', N'Bank', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'AutoCode', N'OverHead', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'IssueFromBOM', N'IssueFromBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'PrepaidVAT', N'PrepaidVAT', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'Sale', N'ItemNature', N'ELECTRIC WIRE/CABLE', N'string', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'BOM', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'BOM', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (39, N'BOM', N'ItemNature', N'SHAFIQKAMRUL', N'string', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (40, N'BOM', N'IntermediateProduction', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (42, N'Sale', N'NumberOfItems', N'15', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (43, N'Production', N'ProductionWithoutBOM', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (44, N'IssueFromBOM', N'IssueAutoPost', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[Settings] OFF


/****** Object:  Table [dbo].[CurrencyConversion]    Script Date: 04/18/2013 12:35:04 ******/
INSERT [dbo].[CurrencyConversion] ([CurrencyConversionId], [CurrencyCodeFrom], [CurrencyCodeTo], [CurrencyRate], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [ConversionDate]) VALUES (N'1', N'249', N'260', CAST(80.0000000000 AS Decimal(18, 10)), N'NA', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime), CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[CurrencyConversion] ([CurrencyConversionId], [CurrencyCodeFrom], [CurrencyCodeTo], [CurrencyRate], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [ConversionDate]) VALUES (N'2', N'260', N'260', CAST(1.0000000000 AS Decimal(18, 10)), N'NA', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime), CAST(0x0000000000000000 AS DateTime))
/****** Object:  Table [dbo].[Currencies]    Script Date: 04/18/2013 12:35:04 ******/
SET IDENTITY_INSERT [dbo].[Currencies] ON
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (246, N'Afghanistan Afghani', N'AFN', N'Afghanistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (247, N'Albanian Lek', N'ALL', N'Albania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (248, N'Algerian Dinar', N'DZD', N'Algeria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (249, N'US Dollar', N'USD', N'American Samoa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (250, N'Euro', N'EUR', N'Andorra', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (251, N'Angolan Kwanza', N'AOA', N'Angola', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (252, N'East Caribbean Dollar', N'XCD', N'Anguilla', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (253, N'Argentine Peso', N'ARS', N'Argentina', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (254, N'Armenian Dram', N'AMD', N'Armenia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (255, N'Aruban Guilder', N'AWG', N'Aruba', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (256, N'Australian Dollar', N'AUD', N'Australia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (257, N'Azerbaijan New Manat', N'AZN', N'Azerbaijan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (258, N'Bahamian Dollar', N'BSD', N'Bahamas', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (259, N'Bahraini Dinar', N'BHD', N'Bahrain', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (260, N'Bangladeshi Taka', N'BDT', N'Bangladesh', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (261, N'Barbados Dollar', N'BBD', N'Barbados', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (262, N'Belarussian Ruble', N'BYR', N'Belarus', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (263, N'Belize Dollar', N'BZD', N'Belize', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (264, N'CFA Franc BCEAO', N'XOF', N'Benin', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (265, N'Bermudian Dollar', N'BMD', N'Bermuda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (266, N'Bhutan Ngultrum', N'BTN', N'Bhutan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (267, N'Boliviano', N'BOB', N'Bolivia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (268, N'Marka', N'BAM', N'Bosnia-Herzegovina', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (269, N'Botswana Pula', N'BWP', N'Botswana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (270, N'Norwegian Krone', N'NOK', N'Bouvet Island', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (271, N'Brazilian Real', N'BRL', N'Brazil', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (272, N'Brunei Dollar', N'BND', N'Brunei Darussalam', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (273, N'Bulgarian Lev', N'BGN', N'Bulgaria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (274, N'Burundi Franc', N'BIF', N'Burundi', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (275, N'Kampuchean Riel', N'KHR', N'Cambodia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (276, N'CFA Franc BEAC', N'XAF', N'Cameroon', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (277, N'Canadian Dollar', N'CAD', N'Canada', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (278, N'Cape Verde Escudo', N'CVE', N'Cape Verde', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (279, N'Cayman Islands Dollar', N'KYD', N'Cayman Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (280, N'Chilean Peso', N'CLP', N'Chile', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (281, N'Yuan Renminbi', N'CNY', N'China', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (282, N'Colombian Peso', N'COP', N'Colombia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (283, N'Comoros Franc', N'KMF', N'Comoros', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (284, N'Francs', N'CDF', N'Congo, Dem. Republic', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (285, N'New Zealand Dollar', N'NZD', N'Cook Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (286, N'Costa Rican Colon', N'CRC', N'Costa Rica', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (287, N'Croatian Kuna', N'HRK', N'Croatia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (288, N'Cuban Peso', N'CUP', N'Cuba', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (289, N'Czech Koruna', N'CZK', N'Czech Rep.', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (290, N'Danish Krone', N'DKK', N'Denmark', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (291, N'Djibouti Franc', N'DJF', N'Djibouti', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (292, N'Dominican Peso', N'DOP', N'Dominican Republic', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (293, N'Ecuador Sucre', N'ECS', N'Ecuador', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (294, N'Egyptian Pound', N'EGP', N'Egypt', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (295, N'El Salvador Colon', N'SVC', N'El Salvador', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (296, N'Eritrean Nakfa', N'ERN', N'Eritrea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (297, N'Ethiopian Birr', N'ETB', N'Ethiopia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (298, N'Falkland Islands Pound', N'FKP', N'Falkland Islands (Malvinas)', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (299, N'Fiji Dollar', N'FJD', N'Fiji', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (300, N'Gambian Dalasi', N'GMD', N'Gambia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (301, N'Georgian Lari', N'GEL', N'Georgia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (302, N'Ghanaian Cedi', N'GHS', N'Ghana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (303, N'Gibraltar Pound', N'GIP', N'Gibraltar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (304, N'Pound Sterling', N'GBP', N'Great Britain', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (305, N'East Carribean Dollar', N'XCD', N'Grenada', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (306, N'Guatemalan Quetzal', N'QTQ', N'Guatemala', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (307, N'Pound Sterling', N'GGP', N'Guernsey', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (308, N'Guinea Franc', N'GNF', N'Guinea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (309, N'Guinea-Bissau Peso', N'GWP', N'Guinea Bissau', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (310, N'Guyana Dollar', N'GYD', N'Guyana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (311, N'Haitian Gourde', N'HTG', N'Haiti', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (312, N'Honduran Lempira', N'HNL', N'Honduras', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (313, N'Hong Kong Dollar', N'HKD', N'Hong Kong', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (314, N'Hungarian Forint', N'HUF', N'Hungary', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (315, N'Iceland Krona', N'ISK', N'Iceland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (316, N'Indian Rupee', N'INR', N'India', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (317, N'Indonesian Rupiah', N'IDR', N'Indonesia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (318, N'Iranian Rial', N'IRR', N'Iran', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (319, N'Iraqi Dinar', N'IQD', N'Iraq', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (320, N'Israeli New Shekel', N'ILS', N'Israel', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (321, N'Jamaican Dollar', N'JMD', N'Jamaica', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (322, N'Japanese Yen', N'JPY', N'Japan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (323, N'Jordanian Dinar', N'JOD', N'Jordan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (324, N'Kazakhstan Tenge', N'KZT', N'Kazakhstan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (325, N'Kenyan Shilling', N'KES', N'Kenya', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (326, N'North Korean Won', N'KPW', N'Korea-North', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (327, N'Korean Won', N'KRW', N'Korea-South', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (328, N'Kuwaiti Dinar', N'KWD', N'Kuwait', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (329, N'Som', N'KGS', N'Kyrgyzstan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (330, N'Lao Kip', N'LAK', N'Laos', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (331, N'Latvian Lats', N'LVL', N'Latvia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (332, N'Lebanese Pound', N'LBP', N'Lebanon', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (333, N'Lesotho Loti', N'LSL', N'Lesotho', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (334, N'Liberian Dollar', N'LRD', N'Liberia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (335, N'Libyan Dinar', N'LYD', N'Libya', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (336, N'Swiss Franc', N'CHF', N'Liechtenstein', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (337, N'Lithuanian Litas', N'LTL', N'Lithuania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (338, N'Macau Pataca', N'MOP', N'Macau', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (339, N'Denar', N'MKD', N'Macedonia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (340, N'Malagasy Franc', N'MGF', N'Madagascar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (341, N'Malawi Kwacha', N'MWK', N'Malawi', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (342, N'Malaysian Ringgit', N'MYR', N'Malaysia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (343, N'Maldive Rufiyaa', N'MVR', N'Maldives', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (344, N'Mauritanian Ouguiya', N'MRO', N'Mauritania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (345, N'Mauritius Rupee', N'MUR', N'Mauritius', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
GO
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (346, N'Mexican Nuevo Peso', N'MXN', N'Mexico', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (347, N'Moldovan Leu', N'MDL', N'Moldova', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (348, N'Mongolian Tugrik', N'MNT', N'Mongolia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (349, N'Moroccan Dirham', N'MAD', N'Morocco', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (350, N'Mozambique Metical', N'MZN', N'Mozambique', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (351, N'Myanmar Kyat', N'MMK', N'Myanmar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (352, N'Namibian Dollar', N'NAD', N'Namibia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (353, N'Nepalese Rupee', N'NPR', N'Nepal', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (354, N'Netherlands Antillean Guilder', N'ANG', N'Netherlands Antilles', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (355, N'CFP Franc', N'XPF', N'New Caledonia (French)', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (356, N'Nicaraguan Cordoba Oro', N'NIO', N'Nicaragua', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (357, N'Nigerian Naira', N'NGN', N'Nigeria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (358, N'Omani Rial', N'OMR', N'Oman', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (359, N'Pakistan Rupee', N'PKR', N'Pakistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (360, N'Panamanian Balboa', N'PAB', N'Panama', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (361, N'Papua New Guinea Kina', N'PGK', N'Papua New Guinea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (362, N'Paraguay Guarani', N'PYG', N'Paraguay', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (363, N'Peruvian Nuevo Sol', N'PEN', N'Peru', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (364, N'Philippine Peso', N'PHP', N'Philippines', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (365, N'Polish Zloty', N'PLN', N'Poland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (366, N'Qatari Rial', N'QAR', N'Qatar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (367, N'Romanian New Leu', N'RON', N'Romania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (368, N'Russian Ruble', N'RUB', N'Russia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (369, N'Rwanda Franc', N'RWF', N'Rwanda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (370, N'St. Helena Pound', N'SHP', N'Saint Helena', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (371, N'Samoan Tala', N'WST', N'Samoa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (372, N'Dobra', N'STD', N'Sao Tome and Principe', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (373, N'Saudi Riyal', N'SAR', N'Saudi Arabia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (374, N'Dinar', N'RSD', N'Serbia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (375, N'Seychelles Rupee', N'SCR', N'Seychelles', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (376, N'Sierra Leone Leone', N'SLL', N'Sierra Leone', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (377, N'Singapore Dollar', N'SGD', N'Singapore', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (378, N'Solomon Islands Dollar', N'SBD', N'Solomon Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (379, N'Somali Shilling', N'SOS', N'Somalia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (380, N'South African Rand', N'ZAR', N'South Africa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (381, N'South Sudan Pound', N'SSP', N'South Sudan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (382, N'Sri Lanka Rupee', N'LKR', N'Sri Lanka', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (383, N'Sudanese Pound', N'SDG', N'Sudan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (384, N'Surinam Dollar', N'SRD', N'Suriname', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (385, N'Swaziland Lilangeni', N'SZL', N'Swaziland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (386, N'Swedish Krona', N'SEK', N'Sweden', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (387, N'Syrian Pound', N'SYP', N'Syria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (388, N'Taiwan Dollar', N'TWD', N'Taiwan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (389, N'Tajik Somoni', N'TJS', N'Tajikistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (390, N'Tanzanian Shilling', N'TZS', N'Tanzania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (391, N'Thai Baht', N'THB', N'Thailand', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (392, N'Tongan Paanga', N'TOP', N'Tonga', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (393, N'Trinidad and Tobago Dollar', N'TTD', N'Trinidad and Tobago', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (394, N'Tunisian Dollar', N'TND', N'Tunisia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (395, N'Turkish Lira', N'TRY', N'Turkey', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (396, N'Manat', N'TMT', N'Turkmenistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (397, N'Uganda Shilling', N'UGX', N'Uganda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (398, N'Ukraine Hryvnia', N'UAH', N'Ukraine', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (399, N'Arab Emirates Dirham', N'AED', N'United Arab Emirates', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (400, N'Uruguayan Peso', N'UYU', N'Uruguay', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (401, N'Uzbekistan Sum', N'UZS', N'Uzbekistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (402, N'Vanuatu Vatu', N'VUV', N'Vanuatu', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (403, N'Venezuelan Bolivar', N'VEF', N'Venezuela', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (404, N'Vietnamese Dong', N'VND', N'Vietnam', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (405, N'Yemeni Rial', N'YER', N'Yemen', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (406, N'Zambian Kwacha', N'ZMW', N'Zambia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (407, N'Zimbabwe Dollar', N'ZWD', N'Zimbabwe', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[Currencies] OFF
/****** Object:  Table [dbo].[Codes]    Script Date: 04/18/2013 12:35:04 ******/
SET IDENTITY_INSERT [dbo].[Codes] ON
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'Purchase', N'Other', N'PUR', N'8', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'Purchase', N'Trading', N'PTD', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'Receive', N'Other', N'REC', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'TollReceive', N'TollReceive', N'TOR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'Purchase', N'PurchaseReturn', N'PRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'Purchase', N'InputService', N'PIS', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'Purchase', N'Import', N'IMP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'TollReceiveRaw', N'TollReceiveRaw', N'TRW', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (9, N'Issue', N'Other', N'ISU', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (10, N'Issue', N'IssueReturn', N'ISR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'Receive', N'ReceiveReturn', N'RER', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'Receive', N'WIP', N'WIP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'TollFinishReceive', N'TollFinishReceive', N'TFR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'Sale', N'Other', N'INV', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'Sale', N'Trading', N'STP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (17, N'Sale', N'Debit', N'DEN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'Sale', N'Credit', N'CRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'Sale', N'Export', N'STR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'InternalIssue', N'InternalIssue', N'TRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (21, N'Sale', N'Service', N'SER', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (22, N'Sale', N'Tender', N'STN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'TollIssue', N'TollIssue', N'EDF', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'TollFinishIssue', N'TollFinishIssue', N'TFI', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'Deposit', N'Treasury', N'DEP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'Deposit', N'VDS', N'VDS', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'Dispose', N'Raw', N'DSR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'Dispose', N'Finish', N'DSF', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'Adjustment', N'Both', N'ADJ', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'SDDeposit', N'Treasury', N'SDP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'DDB', N'DDB', N'DDB', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'Purchase', N'PurchaseDN', N'PDN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'Purchase', N'PurchaseCN', N'PCN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'Sale', N'ServiceNS', N'SNS', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (35, N'Purchase', N'ServiceNS', N'PSN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (36, N'Purchase', N'Service', N'PSE', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[Codes] OFF


           
                ";
                #endregion TableDefaultData Back


                top2 = "go";

                IEnumerable<string> commandStringsDefaultData = Regex.Split(sqlText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStringsDefaultData)
                {
                    if (commandString.Trim() != "")
                    {
                        SqlCommand cmdIdExist1 = new SqlCommand(commandString, currConn);

                        cmdIdExist1.Transaction = transaction;
                        transResult = (int)cmdIdExist1.ExecuteNonQuery();
                        if (transResult < 0)
                        {
                            throw new ArgumentNullException("Insert Default Data to Database'" + databaseName + "'", MessageVM.dbMsgTableDefaultData);
                        }
                    }
                }

                #endregion TableCreate

                #region Insert Company Profile

                //string NewCompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string tom = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                string jary = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyLegalName);
                string miki = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.VatRegistrationNo);
                //string mouse = Converter.DESEncrypt(PassPhrase, EnKey, GetHardwareID());
                string mouse = Converter.DESEncrypt(PassPhrase, EnKey, GetServerHardwareId());


                sqlText = "";
                sqlText += " insert into CompanyProfiles(";
                sqlText += " CompanyID,";
                sqlText += " CompanyName,";
                sqlText += " CompanyLegalName,";
                sqlText += " Address1,";
                sqlText += " Address2,";
                sqlText += " Address3,";
                sqlText += " City,";
                sqlText += " ZipCode,";
                sqlText += " TelephoneNo,";
                sqlText += " FaxNo,";
                sqlText += " Email,";
                sqlText += " ContactPerson,";
                sqlText += " ContactPersonDesignation,";
                sqlText += " ContactPersonTelephone,";
                sqlText += " ContactPersonEmail,";
                sqlText += " TINNo,";
                sqlText += " VatRegistrationNo,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " StartDateTime,";
                sqlText += " FYearStart,";
                sqlText += " FYearEnd,";
                sqlText += " Tom,";
                sqlText += " Jary,";
                sqlText += " Miki,";
                sqlText += " Mouse)";

                sqlText += " values(";
                sqlText += "@companyProfilesCompanyID ,";
                sqlText += "@companyProfilesCompanyName ,";
                sqlText += "@companyProfilesCompanyLegalName ,";
                sqlText += "@companyProfilesAddress1 ,";
                sqlText += "@companyProfilesAddress2 ,";
                sqlText += "@companyProfilesAddress3 ,";
                sqlText += "@companyProfilesCity ,";
                sqlText += "@companyProfilesZipCode ,";
                sqlText += "@companyProfilesTelephoneNo ,";
                sqlText += "@companyProfilesFaxNo ,";
                sqlText += "@companyProfilesEmail ,";
                sqlText += "@companyProfilesContactPerson ,";
                sqlText += "@companyProfilesContactPersonDesignation ,";
                sqlText += "@companyProfilesContactPersonTelephone ,";
                sqlText += "@companyProfilesContactPersonEmail ,";
                sqlText += "@companyProfilesTINNo ,";
                sqlText += "@companyProfilesVatRegistrationNo ,";
                sqlText += "@companyProfilesComments ,";
                sqlText += "@companyProfilesActiveStatus ,";
                sqlText += "'SuperAdmin' ,";
                sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,";
                sqlText += "'SuperAdmin' ,";
                sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,";
                sqlText += "@companyProfilesStartDateTime ,";
                sqlText += "@companyProfilesFYearStart ,";
                sqlText += "@companyProfilesFYearEnd , ";
                sqlText += "@tom , ";
                sqlText += "@jary , ";
                sqlText += "@miki , ";
                sqlText += "@mouse ";
                sqlText += " )";

                //try
                //{


                SqlCommand cmdCompanyProfile = new SqlCommand(sqlText, currConn);
                cmdCompanyProfile.Transaction = transaction;

                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyID", companyProfiles.CompanyID);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyName", companyProfiles.CompanyName);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyLegalName", companyProfiles.CompanyLegalName);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress1", companyProfiles.Address1);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress2", companyProfiles.Address2);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress3", companyProfiles.Address3);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCity", companyProfiles.City);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesZipCode", companyProfiles.ZipCode);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesTelephoneNo", companyProfiles.TelephoneNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFaxNo", companyProfiles.FaxNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesEmail", companyProfiles.Email);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPerson", companyProfiles.ContactPerson);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonDesignation", companyProfiles.ContactPersonDesignation);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonTelephone", companyProfiles.ContactPersonTelephone);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonEmail", companyProfiles.ContactPersonEmail);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesTINNo", companyProfiles.TINNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesVatRegistrationNo", companyProfiles.VatRegistrationNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesComments", companyProfiles.Comments);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesActiveStatus", companyProfiles.ActiveStatus);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesStartDateTime", companyProfiles.StartDateTime);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFYearStart", companyProfiles.FYearStart);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFYearEnd", companyProfiles.FYearEnd);
                cmdCompanyProfile.Parameters.AddWithValue("@tom", tom);
                cmdCompanyProfile.Parameters.AddWithValue("@jary", jary);
                cmdCompanyProfile.Parameters.AddWithValue("@miki", miki);
                cmdCompanyProfile.Parameters.AddWithValue("@mouse", mouse);


                transResult = (int)cmdCompanyProfile.ExecuteNonQuery();
                if (transResult < 0)
                {

                    throw new ArgumentNullException("Insert company Profile data to Database('" + databaseName + "')", MessageVM.dbMsgCompanyInformationNotSave);
                }
                newID = companyProfiles.CompanyID;


                #endregion Insert Company Profile

                #region Insert Fiscal Year
                foreach (var Item in fiscalDetails.ToList())
                {

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into FiscalYear(";
                    sqlText += " FiscalYearName,";
                    sqlText += " CurrentYear,";
                    sqlText += " PeriodID,";
                    sqlText += " PeriodName,";
                    sqlText += " PeriodStart,";
                    sqlText += " PeriodEnd,";
                    sqlText += " PeriodLock,";
                    sqlText += " GLLock,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@Item.FiscalYearName,";
                    sqlText += "@Item.CurrentYear,";
                    sqlText += "@Item.PeriodID,";
                    sqlText += "@Item.PeriodName,";
                    sqlText += "@Item.PeriodStart,";
                    sqlText += "@Item.PeriodEnd,";
                    sqlText += "@Item.PeriodLock,";
                    sqlText += "@Item.GLLock,";
                    sqlText += "'SuperAdmin',";
                    sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sqlText += "'SuperAdmin',";
                    sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@ItemFiscalYearName", Item.FiscalYearName);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCurrentYear", Item.CurrentYear);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodName", Item.PeriodName);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodStart", Item.PeriodStart);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodEnd", Item.PeriodEnd);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemGLLock", Item.GLLock);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Insert Fiscal Year data to Database('" + databaseName + "')", MessageVM.dbMsgCFiscalYearNotSave);
                    }
                    #endregion Insert only DetailTable
                }



                #endregion Insert Fiscal Year

                #region Insert Sys DB Information

                string CompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string CompanyName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                string DatabaseName = Converter.DESEncrypt(PassPhrase, EnKey, databaseName);
                string ActiveStatus = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.ActiveStatus);
                //string CompanyLegalName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyLegalName);
                sqlText = "";
                sqlText += " INSERT INTO CompanyInformations (CompanyID,CompanyName,DatabaseName,ActiveStatus,Serial)";
                sqlText += " VALUES(" +
                           "@CompanyID," +
                           "@CompanyName," +
                           "@DatabaseName," +
                           "@ActiveStatus," +
                    //"'" + CompanyLegalName + "'," +
                           "(select isnull(max(Serial ),0)+1 FROM  CompanyInformations)" +

                           ")";
                currConn.ChangeDatabase("SymphonyVATSys");
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@CompanyID", CompanyID);
                cmdPrefetch.Parameters.AddWithValue("@CompanyName", CompanyName);
                cmdPrefetch.Parameters.AddWithValue("@DatabaseName", DatabaseName);
                cmdPrefetch.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();
                if (transResult < 0)
                {
                    throw new ArgumentNullException("Insert Company List Information", MessageVM.dbMsgDBInfoInsert);
                }
                #endregion Insert Sys DB Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        #region SuccessResult

                        retResults[0] = "Success";
                        retResults[1] = "Requested Company Created successfully.";
                        retResults[2] = newID;
                        #endregion SuccessResult

                    }

                }

                #endregion Commit
            }
            #endregion Try

            #region Catch and Finall
            catch (ArgumentNullException arg)
            {
                if (arg.ParamName.ToLower() != "deletedb")
                {
                    currConn.Close();
                    currConn.Open();
                    currConn.ChangeDatabase("master");
                    #region check Database and delete
                    sqlText = "";
                    sqlText += " USE [master]";
                    sqlText += " drop DATABASE @databaseName ";

                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;

                    cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                    cmdIdExist.ExecuteNonQuery();
                    #endregion check Database
                }

                throw arg;
            }
            catch (SqlException sqlex)
            {

                currConn.Close();
                currConn.Open();
                currConn.ChangeDatabase("master");


                #region check Database and delete


                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " drop DATABASE @databaseName";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                cmdIdExist.ExecuteNonQuery();


                #endregion check Database

                throw sqlex;
            }

            catch (Exception ex)
            {


                currConn.Close();
                currConn.Open();
                currConn.ChangeDatabase("master");
                #region check Database and delete


                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " drop DATABASE @databaseName";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                cmdIdExist.ExecuteNonQuery();

                #endregion check Database

                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result
        }

        public string[] NewDBCreate(CompanyProfileVM companyProfiles, string databaseName, List<FiscalYearVM> fiscalDetails)
        {

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string nextId = "";
            string newID = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(databaseName))
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyName);
                }
                if (fiscalDetails.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoFiscalYear);
                }
                if (companyProfiles == null)
                {
                    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyInformation);
                }

                #endregion Validation

                #region open connection and transaction sys / Master

                SysDBInfoVM.SysDatabaseName = "SymphonyVATSys";

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//start
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region check Database

                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " select COUNT(NAME) from sys.databases where name = @databaseName";

                SqlCommand cmdDBExist = new SqlCommand(sqlText, currConn);

                cmdDBExist.Parameters.AddWithValue("@databaseName", databaseName);

                transResult = (int)cmdDBExist.ExecuteScalar();
                if (transResult > 0)
                {
                    throw new ArgumentNullException("DeleteDB", MessageVM.dbMsgDBExist);
                }

                #endregion check Database

                #region CreateDatabase

                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " CREATE DATABASE @databaseName ";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                cmdDBExist.Parameters.AddWithValue("@databaseName", databaseName);

                transResult = (int)cmdIdExist.ExecuteNonQuery();


                if (transResult != -1)
                {
                    throw new ArgumentNullException("Create Database('" + databaseName + "')", MessageVM.dbMsgDBNotCreate);
                }

                #endregion CreateDatabase

                #region Change Database for New DB
                currConn.ChangeDatabase(databaseName);
                transaction = currConn.BeginTransaction(MessageVM.dbMsgMethodName);
                #endregion open connection and transaction

                #region TableCreate
                string top1;

                #region CreateTable Back
                //              sqlText = @"
                //
                //";
                #endregion CreateTable
                #region CreateTable Back
                sqlText = @"
                
CREATE TABLE [dbo].[AdjustmentHistorys](
	[AdjHistoryID] [varchar](50) NULL,
	[AdjHistoryNo] [varchar](50) NULL,
	[AdjId] [varchar](50) NULL,
	[AdjDate] [datetime] NULL,
	[AdjInputAmount] [decimal](25, 9) NULL,
	[AdjInputPercent] [decimal](25, 9) NULL,
	[AdjAmount] [decimal](25, 9) NULL,
	[AdjVATRate] [decimal](25, 9) NULL,
	[AdjVATAmount] [decimal](25, 9) NULL,
	[AdjSD] [decimal](25, 9) NULL,
	[AdjSDAmount] [decimal](25, 9) NULL,
	[AdjOtherAmount] [decimal](25, 9) NULL,
	[AdjType] [varchar](50) NULL,
	[AdjDescription] [varchar](500) NULL,
	[AdjReferance] [varchar](500) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReverseAdjHistoryNo] [varchar](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdjustmentName]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdjustmentName](
	[AdjId] [varchar](50) NULL,
	[AdjName] [varchar](500) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BanderolProducts]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BanderolProducts](
	[BandProductId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[BanderolId] [varchar](50) NULL,
	[PackagingId] [varchar](50) NULL,
	[BUsedQty] [decimal](25, 9) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[WastageQty] [decimal](25, 9) NULL,
	[OpeningQty] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
 CONSTRAINT [PK_BanderolProducts] PRIMARY KEY CLUSTERED 
(
	[BandProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Banderols]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Banderols](
	[BanderolID] [varchar](50) NOT NULL,
	[BanderolName] [varchar](120) NULL,
	[BanderolSize] [varchar](50) NULL,
	[UOM] [varchar](120) NULL,
	[OpeningQty] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
	[Description] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Banderols] PRIMARY KEY CLUSTERED 
(
	[BanderolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BankInformations]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BankInformations](
	[BankID] [varchar](20) NOT NULL,
	[BankCode] [varchar](50) NULL,
	[BankName] [varchar](120) NULL,
	[BranchName] [varchar](120) NULL,
	[AccountNumber] [varchar](120) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_BankInformations] PRIMARY KEY CLUSTERED 
(
	[BankID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMCompanyOverhead]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMCompanyOverhead](
	[BOMOverHeadId] [varchar](20) NOT NULL,
	[BOMId] [varchar](20) NOT NULL,
	[OHLineNo] [int] NULL,
	[HeadName] [varchar](150) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[HeadAmount] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info5] [varchar](200) NULL,
	[RebatePercent] [decimal](25, 9) NULL,
	[RebateAmount] [decimal](25, 9) NULL,
	[AdditionalCost] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[HeadID] [varchar](20) NULL,
	[CustomerID] [varchar](20) NULL,
 CONSTRAINT [PK_BOMCompanyOverhead] PRIMARY KEY CLUSTERED 
(
	[BOMOverHeadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMRaws]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMRaws](
	[BOMRawId] [varchar](20) NOT NULL,
	[BOMId] [varchar](20) NOT NULL,
	[BOMLineNo] [int] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[RawItemNo] [varchar](20) NOT NULL,
	[RawItemType] [varchar](50) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[UseQuantity] [decimal](25, 9) NULL,
	[WastageQuantity] [decimal](25, 9) NULL,
	[Cost] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[RebateRate] [decimal](25, 9) NULL,
	[MarkUpValue] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMUQty] [decimal](25, 9) NULL,
	[UOMWQty] [decimal](25, 9) NULL,
	[TotalQuantity] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[PBOMId] [varchar](20) NULL,
	[PInvoiceNo] [varchar](20) NULL,
	[IssueOnProduction] [varchar](1) NULL,
	[CustomerID] [varchar](20) NULL,
	[TransactionType] [varchar](50) NULL,
 CONSTRAINT [PK_BOMRaws] PRIMARY KEY CLUSTERED 
(
	[BOMRawId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMs]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMs](
	[BOMId] [varchar](20) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[VATRate] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[RawTotal] [decimal](25, 9) NULL,
	[PackingTotal] [decimal](25, 9) NULL,
	[RebateTotal] [decimal](25, 9) NULL,
	[AdditionalTotal] [decimal](25, 9) NULL,
	[RebateAdditionTotal] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[PacketPrice] [decimal](25, 9) NULL,
	[RawOHCost] [decimal](25, 9) NULL,
	[LastNBRPrice] [decimal](25, 9) NULL,
	[LastNBRWithSDAmount] [decimal](25, 9) NULL,
	[TotalQuantity] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[WholeSalePrice] [decimal](25, 9) NULL,
	[NBRWithSDAmount] [decimal](25, 9) NULL,
	[MarkUpValue] [decimal](25, 9) NULL,
	[LastMarkUpValue] [decimal](25, 9) NULL,
	[LastSDAmount] [decimal](25, 9) NULL,
	[LastAmount] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[UOM] [varchar](120) NULL,
	[CustomerID] [varchar](20) NULL,
 CONSTRAINT [PK_BOMs] PRIMARY KEY CLUSTERED 
(
	[BOMId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOMsMas]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOMsMas](
	[BOMId] [varchar](20) NOT NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[EffectDate] [datetime] NOT NULL,
	[VATName] [varchar](50) NOT NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[NBRPrice] [decimal](18, 5) NULL,
	[PacketPrice] [decimal](18, 5) NULL,
	[RawOHCost] [decimal](18, 5) NULL,
	[LastNBRPrice] [decimal](18, 5) NULL,
	[LastNBRWithSDAmount] [decimal](18, 5) NULL,
	[TotalQuantity] [decimal](18, 5) NULL,
	[SDAmount] [decimal](18, 5) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[WholeSalePrice] [decimal](18, 5) NULL,
	[NBRWithSDAmount] [decimal](18, 5) NULL,
	[MarkUpValue] [decimal](18, 5) NULL,
	[LastMarkUpValue] [decimal](18, 5) NULL,
	[LastSDAmount] [decimal](18, 5) NULL,
	[LastAmount] [decimal](18, 5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Codes]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Codes](
	[CodeId] [int] IDENTITY(1,1) NOT NULL,
	[CodeGroup] [varchar](120) NULL,
	[CodeName] [varchar](120) NULL,
	[prefix] [varchar](120) NULL,
	[Lenth] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyOverheads]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyOverheads](
	[HeadID] [varchar](20) NULL,
	[HeadName] [varchar](150) NOT NULL,
	[HeadAmount] [decimal](25, 9) NULL,
	[Description] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [nchar](10) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[OHCode] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[RebatePercent] [decimal](25, 9) NULL,
 CONSTRAINT [PK_CompanyOverheads] PRIMARY KEY CLUSTERED 
(
	[HeadName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyOverheadVAT]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyOverheadVAT](
	[HeadName] [varchar](150) NOT NULL,
	[HeadAmount] [decimal](18, 5) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[EffectMonth] [datetime] NOT NULL,
 CONSTRAINT [PK_CompanyOverheadVAT] PRIMARY KEY CLUSTERED 
(
	[HeadName] ASC,
	[EffectMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompanyProfiles]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompanyProfiles](
	[CompanyID] [varchar](20) NOT NULL,
	[CompanyName] [varchar](120) NULL,
	[CompanyLegalName] [varchar](120) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[ZipCode] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[VatRegistrationNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[StartDateTime] [datetime] NULL,
	[FYearStart] [datetime] NULL,
	[FYearEnd] [datetime] NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[Tom] [varchar](200) NULL,
	[Jary] [varchar](200) NULL,
	[Miki] [varchar](200) NULL,
	[Mouse] [varchar](200) NULL,
 CONSTRAINT [PK_CompanyProfile] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Costing]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Costing](
	[ItemNo] [varchar](20) NULL,
	[InputDate] [datetime] NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[AV] [decimal](25, 9) NULL,
	[CD] [decimal](25, 9) NULL,
	[RD] [decimal](25, 9) NULL,
	[TVB] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVA] [decimal](25, 9) NULL,
	[ATV] [decimal](25, 9) NULL,
	[Other] [decimal](25, 9) NULL,
	[Id] [int] NULL,
	[BENumber] [varchar](200) NULL,
	[RefNo] [varchar](200) NULL,
	[SD] [decimal](25, 9) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyName] [varchar](500) NULL,
	[CurrencyCode] [varchar](50) NULL,
	[Country] [varchar](500) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[CurrencyMajor] [varchar](50) NULL,
	[CurrencyMinor] [varchar](50) NULL,
	[CurrencySymbol] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CurrencyConversion]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CurrencyConversion](
	[CurrencyConversionId] [varchar](20) NULL,
	[CurrencyCodeFrom] [varchar](50) NOT NULL,
	[CurrencyCodeTo] [varchar](50) NULL,
	[CurrencyRate] [decimal](18, 10) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ConversionDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerGroups]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerGroups](
	[CustomerGroupID] [varchar](20) NOT NULL,
	[CustomerGroupName] [varchar](120) NULL,
	[CustomerGroupDescription] [varchar](120) NULL,
	[GroupType] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_CustomerGroup] PRIMARY KEY CLUSTERED 
(
	[CustomerGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](50) NULL,
	[CustomerName] [varchar](120) NULL,
	[CustomerGroupID] [varchar](20) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[ContactPerson] [varchar](120) NULL,
	[ContactPersonDesignation] [varchar](120) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[Country] [varchar](200) NULL,
	[VDSPercent] [decimal](25, 9) NULL,
	[BusinessType] [varchar](120) NULL,
	[BusinessCode] [varchar](20) NULL,
 CONSTRAINT [PK_CustomerInformation] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomersAddress]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomersAddress](
	[Id] [int] NULL,
	[CustomerID] [varchar](20) NULL,
	[CustomerVATRegNo] [varchar](20) NULL,
	[CustomerAddress] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DDBDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DDBDetails](
	[DDBDetailsNo] [varchar](20) NOT NULL,
	[DDBNo] [varchar](20) NOT NULL,
	[DDBDateTime] [datetime] NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[BENumber] [varchar](200) NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[PurcahseItemNo] [varchar](20) NOT NULL,
	[SaleItemNo] [varchar](20) NOT NULL,
	[DDBLineNo] [int] NULL,
	[Quantity] [decimal](18, 5) NULL,
	[UseQuantity] [decimal](18, 5) NULL,
	[CostPrice] [decimal](18, 5) NULL,
	[UOM] [varchar](120) NULL,
	[SubTotal] [decimal](18, 5) NULL,
	[CnFAmount] [decimal](18, 9) NULL,
	[InsuranceAmount] [decimal](18, 9) NULL,
	[AssessableValue] [decimal](18, 9) NULL,
	[CDAmount] [decimal](18, 9) NULL,
	[RDAmount] [decimal](18, 9) NULL,
	[SD] [decimal](18, 2) NULL,
	[SDAmount] [decimal](18, 9) NULL,
	[TVBAmount] [decimal](18, 9) NULL,
	[VATRate] [decimal](18, 2) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[TVAAmount] [decimal](18, 9) NULL,
	[ATVAmount] [decimal](18, 9) NULL,
	[OthersAmount] [decimal](18, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DDBHeader]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DDBHeader](
	[DDBNo] [varchar](20) NOT NULL,
	[DDBDateTime] [datetime] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](18, 5) NULL,
	[UOM] [varchar](120) NULL,
	[TotalAmount] [decimal](18, 5) NULL,
	[CnFAmount] [decimal](18, 9) NULL,
	[InsuranceAmount] [decimal](18, 9) NULL,
	[AssessableValue] [decimal](18, 9) NULL,
	[CDAmount] [decimal](18, 9) NULL,
	[RDAmount] [decimal](18, 9) NULL,
	[SD] [decimal](18, 2) NULL,
	[SDAmount] [decimal](18, 9) NULL,
	[TVBAmount] [decimal](18, 9) NULL,
	[VATRate] [decimal](18, 2) NULL,
	[VATAmount] [decimal](18, 5) NULL,
	[TVAAmount] [decimal](18, 9) NULL,
	[ATVAmount] [decimal](18, 9) NULL,
	[OthersAmount] [decimal](18, 9) NULL,
	[Remarks] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DemandDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DemandDetails](
	[DemandNo] [varchar](20) NOT NULL,
	[DemandLineNo] [int] NULL,
	[BandProductId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[DemandQty] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[TransactionDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[DemandReceiveID] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[TransactionType] [varchar](50) NULL,
	[VehicleID] [varchar](20) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DemandHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DemandHeaders](
	[DemandNo] [varchar](20) NOT NULL,
	[DemandDateTime] [datetime] NULL,
	[FiscalYear] [varchar](30) NULL,
	[MonthFrom] [varchar](50) NULL,
	[MonthTo] [varchar](50) NULL,
	[TotalQty] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[DemandReceiveID] [varchar](20) NULL,
	[VehicleID] [varchar](20) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[DemandReceiveDate] [datetime] NULL,
	[RefNo] [varchar](20) NULL,
	[RefDate] [datetime] NULL,
 CONSTRAINT [PK_DemandHeaders] PRIMARY KEY CLUSTERED 
(
	[DemandNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Deposits]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Deposits](
	[DepositId] [varchar](20) NOT NULL,
	[TreasuryNo] [varchar](50) NULL,
	[DepositDateTime] [datetime] NULL,
	[DepositType] [varchar](50) NULL,
	[DepositAmount] [decimal](25, 9) NULL,
	[ChequeNo] [varchar](50) NULL,
	[ChequeBank] [varchar](120) NULL,
	[ChequeBankBranch] [varchar](120) NULL,
	[ChequeDate] [datetime] NULL,
	[BankID] [varchar](20) NULL,
	[TreasuryCopy] [varchar](20) NULL,
	[DepositPerson] [varchar](120) NULL,
	[DepositPersonDesignation] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[ReverseDepositId] [varchar](20) NULL,
 CONSTRAINT [PK_Deposit] PRIMARY KEY CLUSTERED 
(
	[DepositId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DisposeDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DisposeDetails](
	[DisposeNumber] [varchar](20) NOT NULL,
	[LineNumber] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[UOM] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[RealPrice] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SaleNumber] [varchar](120) NULL,
	[PurchaseNumber] [varchar](120) NOT NULL,
	[PresentPrice] [decimal](25, 9) NULL,
	[Remarks] [varchar](120) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[DisposeDate] [datetime] NULL,
	[QuantityImport] [decimal](25, 9) NULL,
	[TransactionType] [varchar](120) NULL,
	[FromStock] [varchar](1) NULL,
	[DollarPrice] [decimal](25, 9) NULL,
 CONSTRAINT [PK_DisposeDetails] PRIMARY KEY CLUSTERED 
(
	[DisposeNumber] ASC,
	[ItemNo] ASC,
	[PurchaseNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DisposeHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DisposeHeaders](
	[DisposeNumber] [varchar](20) NOT NULL,
	[DisposeDate] [datetime] NULL,
	[RefNumber] [varchar](120) NULL,
	[Remarks] [varchar](120) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](120) NULL,
	[Post] [varchar](1) NULL,
	[FromStock] [varchar](1) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[ImportVATAmount] [decimal](25, 9) NULL,
	[TotalPrice] [decimal](25, 9) NULL,
	[TotalPriceImport] [decimal](25, 9) NULL,
	[AppVATAmount] [decimal](25, 9) NULL,
	[AppTotalPrice] [decimal](25, 9) NULL,
	[AppVATAmountImport] [decimal](25, 9) NULL,
	[AppTotalPriceImport] [decimal](25, 9) NULL,
	[AppDate] [datetime] NULL,
	[AppRefNumber] [varchar](120) NULL,
	[AppRemarks] [varchar](120) NULL,
 CONSTRAINT [PK_DisposeHeaders] PRIMARY KEY CLUSTERED 
(
	[DisposeNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Duties]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Duties](
	[DutyID] [varchar](50) NOT NULL,
	[DutyName] [varchar](120) NULL,
	[DutyRate] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[DutyType] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Duties] PRIMARY KEY CLUSTERED 
(
	[DutyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBackDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBackDetails](
	[DDBackNo] [varchar](20) NOT NULL,
	[DDBackDate] [datetime] NULL,
	[DDLineNo] [int] NULL,
	[SalesInvoiceNo] [varchar](20) NULL,
	[PurchaseInvoiceNo] [varchar](20) NULL,
	[PurchaseDate] [datetime] NULL,
	[FgItemNo] [varchar](20) NULL,
	[FgQty] [decimal](25, 9) NULL,
	[ItemNo] [varchar](20) NULL,
	[BillOfEntry] [varchar](50) NULL,
	[PurchaseUom] [varchar](10) NULL,
	[PurchaseQuantity] [decimal](25, 9) NULL,
	[UnitPrice] [decimal](25, 9) NULL,
	[AV] [decimal](25, 9) NULL,
	[CD] [decimal](25, 9) NULL,
	[RD] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[VAT] [decimal](25, 9) NULL,
	[CnF] [decimal](25, 9) NULL,
	[Insurance] [decimal](25, 9) NULL,
	[TVB] [decimal](25, 9) NULL,
	[TVA] [decimal](25, 9) NULL,
	[ATV] [decimal](25, 9) NULL,
	[Others] [decimal](25, 9) NULL,
	[UseQuantity] [decimal](25, 9) NULL,
	[ClaimCD] [decimal](25, 9) NULL,
	[ClaimRD] [decimal](25, 9) NULL,
	[ClaimSD] [decimal](25, 9) NULL,
	[ClaimVAT] [decimal](25, 9) NULL,
	[ClaimCnF] [decimal](25, 9) NULL,
	[ClaimInsurance] [decimal](25, 9) NULL,
	[ClaimTVB] [decimal](25, 9) NULL,
	[ClaimTVA] [decimal](25, 9) NULL,
	[ClaimATV] [decimal](25, 9) NULL,
	[ClaimOthers] [decimal](25, 9) NULL,
	[SubTotalDDB] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[UOMCD] [decimal](25, 9) NULL,
	[UOMRD] [decimal](25, 9) NULL,
	[UOMSD] [decimal](25, 9) NULL,
	[UOMVAT] [decimal](25, 9) NULL,
	[UOMCnF] [decimal](25, 9) NULL,
	[UOMInsurance] [decimal](25, 9) NULL,
	[UOMTVB] [decimal](25, 9) NULL,
	[UOMTVA] [decimal](25, 9) NULL,
	[UOMATV] [decimal](25, 9) NULL,
	[UOMOthers] [decimal](25, 9) NULL,
	[UOMSubTotalDDB] [decimal](25, 9) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](50) NULL,
	[LastModifiedOn] [datetime] NULL,
	[PurchasetransactionType] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBackHeader]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBackHeader](
	[DDBackNo] [varchar](20) NOT NULL,
	[DDBackDate] [datetime] NOT NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SalesDate] [datetime] NOT NULL,
	[CustormerID] [varchar](20) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[ExpCurrency] [decimal](25, 9) NULL,
	[BDTCurrency] [decimal](25, 9) NULL,
	[FgItemNo] [varchar](20) NOT NULL,
	[TotalClaimCD] [decimal](25, 9) NULL,
	[TotalClaimRD] [decimal](25, 9) NULL,
	[TotalClaimSD] [decimal](25, 9) NULL,
	[TotalDDBack] [decimal](25, 9) NULL,
	[TotalClaimVAT] [decimal](25, 9) NULL,
	[TotalClaimCnFAmount] [decimal](25, 9) NULL,
	[TotalClaimInsuranceAmount] [decimal](25, 9) NULL,
	[TotalClaimTVBAmount] [decimal](25, 9) NULL,
	[TotalClaimTVAAmount] [decimal](25, 9) NULL,
	[TotalClaimATVAmount] [decimal](25, 9) NULL,
	[TotalClaimOthersAmount] [decimal](25, 9) NULL,
	[Comments] [varchar](250) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](20) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
 CONSTRAINT [PK_DutyDrawBackHeader] PRIMARY KEY CLUSTERED 
(
	[DDBackNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DutyDrawBacks]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DutyDrawBacks](
	[DrawBackID] [varchar](20) NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[BENumber] [varchar](200) NULL,
	[PurchaseDateTime] [datetime] NULL,
	[PurchaseReceiveDate] [datetime] NULL,
	[PurchaseItemNo] [varchar](20) NOT NULL,
	[PurchaseDutyAmount] [decimal](18, 6) NULL,
	[DrawBackDutyPercent] [decimal](18, 6) NULL,
	[DrawBackDutyAmount] [decimal](18, 6) NULL,
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SaleDateTime] [datetime] NULL,
	[SaleDeliveryDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FiscalYear]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FiscalYear](
	[FiscalYearName] [varchar](30) NULL,
	[CurrentYear] [varchar](4) NOT NULL,
	[PeriodID] [varchar](6) NOT NULL,
	[PeriodName] [varchar](50) NULL,
	[PeriodStart] [datetime] NULL,
	[PeriodEnd] [datetime] NULL,
	[PeriodLock] [varchar](1) NULL,
	[GLLock] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_FiscalYear] PRIMARY KEY CLUSTERED 
(
	[CurrentYear] ASC,
	[PeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ImagesStore]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ImagesStore](
	[OriginalPath] [varchar](500) NULL,
	[ImageData] [binary](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[IssueDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IssueDetails](
	[IssueNo] [varchar](20) NOT NULL,
	[IssueLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceiveNo] [varchar](20) NULL,
	[IssueDateTime] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[Wastage] [decimal](25, 9) NULL,
	[BOMDate] [datetime] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[Post] [varchar](1) NULL,
	[TransactionType] [varchar](50) NULL,
	[IssueReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[BOMId] [varchar](20) NULL,
	[UOMWastage] [decimal](25, 9) NULL,
	[IsProcess] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[IssueHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IssueHeaders](
	[IssueNo] [varchar](20) NOT NULL,
	[IssueDateTime] [datetime] NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceiveNo] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[IssueReturnId] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[ImportIDExcel] [varchar](30) NULL,
 CONSTRAINT [PK_IssueHeader] PRIMARY KEY CLUSTERED 
(
	[IssueNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PackagingInformations]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PackagingInformations](
	[PackagingID] [varchar](50) NOT NULL,
	[PackagingNature] [varchar](120) NULL,
	[PackagingCapacity] [varchar](50) NULL,
	[UOM] [varchar](120) NULL,
	[Description] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_PackagingInformations] PRIMARY KEY CLUSTERED 
(
	[PackagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PriceService]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceService](
	[ItemNo] [varchar](20) NOT NULL,
	[Cost] [decimal](25, 9) NULL,
	[BasePrice] [decimal](25, 9) NULL,
	[OtherRate] [decimal](25, 9) NULL,
	[OtherType] [decimal](25, 9) NULL,
	[OtherAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SalePrice] [decimal](25, 9) NULL,
	[EffectDate] [datetime] NOT NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[CategoryID] [varchar](20) NOT NULL,
	[CategoryName] [varchar](120) NULL,
	[Description] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[IsRaw] [varchar](50) NOT NULL,
	[HSCodeNo] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[PropergatingRate] [varchar](1) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[Trading] [varchar](1) NULL,
	[NonStock] [varchar](1) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Products]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Products](
	[ItemNo] [varchar](20) NOT NULL,
	[ProductCode] [varchar](50) NULL,
	[ProductName] [varchar](120) NULL,
	[ProductDescription] [varchar](120) NULL,
	[CategoryID] [varchar](20) NULL,
	[UOM] [varchar](120) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[SalesPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[ReceivePrice] [decimal](25, 9) NULL,
	[IssuePrice] [decimal](25, 9) NULL,
	[TenderPrice] [decimal](25, 9) NULL,
	[ExportPrice] [decimal](25, 9) NULL,
	[InternalIssuePrice] [decimal](25, 9) NULL,
	[TollIssuePrice] [decimal](25, 9) NULL,
	[TollCharge] [decimal](25, 9) NULL,
	[OpeningBalance] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[HSCodeNo] [varchar](50) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[SD] [decimal](25, 9) NULL,
	[PacketPrice] [decimal](25, 9) NULL,
	[Trading] [varchar](1) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[NonStock] [varchar](1) NULL,
	[QuantityInHand] [decimal](25, 9) NULL,
	[OpeningDate] [datetime] NULL,
	[RebatePercent] [decimal](25, 9) NULL,
	[TVBRate] [decimal](25, 9) NULL,
	[CnFRate] [decimal](25, 9) NULL,
	[InsuranceRate] [decimal](25, 9) NULL,
	[CDRate] [decimal](25, 9) NULL,
	[RDRate] [decimal](25, 9) NULL,
	[AITRate] [decimal](25, 9) NULL,
	[TVARate] [decimal](25, 9) NULL,
	[ATVRate] [decimal](25, 9) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[OpeningTotalCost] [decimal](25, 9) NULL,
	[TollProduct] [varchar](1) NULL,
	[Banderol] [varchar](1) NULL,
 CONSTRAINT [PK_ItemInformation] PRIMARY KEY CLUSTERED 
(
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceDetails](
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[POLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Type] [varchar](200) NULL,
	[ProductType] [varchar](200) NULL,
	[BENumber] [varchar](200) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[ReceiveDate] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[RebateRate] [decimal](25, 9) NULL,
	[RebateAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[CnFAmount] [decimal](25, 9) NULL,
	[InsuranceAmount] [decimal](25, 9) NULL,
	[AssessableValue] [decimal](25, 9) NULL,
	[CDAmount] [decimal](25, 9) NULL,
	[RDAmount] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TVBAmount] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVAAmount] [decimal](25, 9) NULL,
	[ATVAmount] [decimal](25, 9) NULL,
	[OthersAmount] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[PurchaseReturnId] [varchar](20) NULL,
	[ReturnTransactionType] [varchar](50) NULL,
 CONSTRAINT [PK_PurchaseInvoiceDetails] PRIMARY KEY CLUSTERED 
(
	[PurchaseInvoiceNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceDuties]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceDuties](
	[PIDutyID] [varchar](50) NOT NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[CnFInp] [decimal](25, 9) NULL,
	[CnFRate] [decimal](25, 9) NULL,
	[CnFAmount] [decimal](25, 9) NULL,
	[InsuranceInp] [decimal](25, 9) NULL,
	[InsuranceRate] [decimal](25, 9) NULL,
	[InsuranceAmount] [decimal](25, 9) NULL,
	[AssessableInp] [decimal](25, 9) NULL,
	[AssessableValue] [decimal](25, 9) NULL,
	[CDInp] [decimal](25, 9) NULL,
	[CDRate] [decimal](25, 9) NULL,
	[CDAmount] [decimal](25, 9) NULL,
	[RDInp] [decimal](25, 9) NULL,
	[RDRate] [decimal](25, 9) NULL,
	[RDAmount] [decimal](25, 9) NULL,
	[TVBInp] [decimal](25, 9) NULL,
	[TVBRate] [decimal](25, 9) NULL,
	[TVBAmount] [decimal](25, 9) NULL,
	[SDInp] [decimal](25, 9) NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[VATInp] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[TVAInp] [decimal](25, 9) NULL,
	[TVARate] [decimal](25, 9) NULL,
	[TVAAmount] [decimal](25, 9) NULL,
	[ATVInp] [decimal](25, 9) NULL,
	[ATVRate] [decimal](25, 9) NULL,
	[ATVAmount] [decimal](25, 9) NULL,
	[OthersInp] [decimal](25, 9) NULL,
	[OthersRate] [decimal](25, 9) NULL,
	[OthersAmount] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[Remarks] [varchar](200) NULL,
	[ItemNo] [varchar](20) NULL,
	[DutyCompleteQuantity] [decimal](25, 9) NULL,
	[DutyCompleteQuantityPercent] [decimal](25, 9) NULL,
	[LineCost] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseInvoiceHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseInvoiceHeaders](
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[VendorID] [varchar](20) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[BENumber] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[ProductType] [varchar](100) NULL,
	[TransactionType] [varchar](50) NULL,
	[ReceiveDate] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[CurrencyID] [varchar](50) NULL,
	[CurrencyRateFromBDT] [decimal](25, 9) NULL,
	[WithVDS] [varchar](1) NULL,
	[PurchaseReturnId] [varchar](20) NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[SerialNo1] [varchar](50) NULL,
	[CustomHouse] [varchar](500) NULL,
	[LCNumber] [varchar](50) NULL,
	[LCDate] [datetime] NULL,
	[LandedCost] [decimal](25, 9) NULL,
 CONSTRAINT [PK_ProductInvoiceHead] PRIMARY KEY CLUSTERED 
(
	[PurchaseInvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReceiveDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceiveDetails](
	[ReceiveNo] [varchar](20) NOT NULL,
	[ReceiveLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[ReceiveDateTime] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[VATName] [varchar](100) NULL,
	[ReceiveReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[BOMId] [varchar](20) NULL,
	[BOMId1] [varchar](20) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[ReturnTransactionType] [varchar](50) NULL,
 CONSTRAINT [PK_ReceiveDetails] PRIMARY KEY CLUSTERED 
(
	[ReceiveNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReceiveHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceiveHeaders](
	[ReceiveNo] [varchar](20) NOT NULL,
	[ReceiveDateTime] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[ReceiveReturnId] [varchar](20) NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[ReferenceNo] [varchar](50) NULL,
	[WithToll] [varchar](1) NULL,
	[CustomerID] [varchar](20) NULL,
 CONSTRAINT [PK_ReceiveHead] PRIMARY KEY CLUSTERED 
(
	[ReceiveNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportSales]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportSales](
	[AuditUser] [varchar](10) NULL,
	[OpeQty] [money] NULL,
	[PrdQty] [money] NULL,
	[InvoiceNo] [varchar](20) NULL,
	[CustomerName] [varchar](120) NULL,
	[Address] [varchar](200) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[InvoiceDate] [datetime] NULL,
	[ItemNo] [varchar](20) NULL,
	[ProductName] [varchar](120) NULL,
	[Quantity] [decimal](18, 5) NULL,
	[SalesPrice] [decimal](18, 5) NULL,
	[ClosingQuantity] [decimal](18, 5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceDetails](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[InvoiceLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[SalesPrice] [decimal](25, 9) NULL,
	[NBRPrice] [decimal](25, 9) NULL,
	[AVGPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[VATAmount] [decimal](25, 9) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SD] [decimal](25, 9) NULL,
	[SDAmount] [decimal](25, 9) NULL,
	[SaleType] [varchar](10) NULL,
	[PreviousSalesInvoiceNo] [varchar](200) NULL,
	[Trading] [varchar](1) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[NonStock] [varchar](1) NULL,
	[TradingMarkUp] [decimal](25, 9) NULL,
	[Type] [varchar](50) NULL,
	[BENumber] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[DollerValue] [decimal](25, 9) NULL,
	[CurrencyValue] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[VATName] [varchar](100) NULL,
	[SaleReturnId] [varchar](20) NULL,
	[DiscountAmount] [decimal](25, 9) NULL,
	[DiscountedNBRPrice] [decimal](25, 9) NULL,
	[PromotionalQuantity] [decimal](25, 9) NULL,
	[FinishItemNo] [varchar](20) NULL,
	[ValueOnly] [varchar](1) NULL,
	[CConversionDate] [datetime] NULL,
	[ReturnTransactionType] [varchar](50) NULL,
	[Weight] [varchar](120) NULL,
 CONSTRAINT [PK_SalesInvoiceDetails_1] PRIMARY KEY CLUSTERED 
(
	[SalesInvoiceNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceHeaders](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[CustomerID] [varchar](20) NOT NULL,
	[DeliveryAddress1] [varchar](200) NULL,
	[DeliveryAddress2] [varchar](200) NULL,
	[DeliveryAddress3] [varchar](200) NULL,
	[VehicleID] [varchar](20) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[DeliveryDate] [datetime] NULL,
	[TotalAmount] [decimal](25, 9) NULL,
	[TotalVATAmount] [decimal](25, 9) NULL,
	[SerialNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[SaleType] [varchar](20) NULL,
	[PreviousSalesInvoiceNo] [varchar](20) NULL,
	[Trading] [varchar](1) NULL,
	[IsPrint] [varchar](1) NULL,
	[TenderId] [varchar](200) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[LCNumber] [varchar](50) NULL,
	[CurrencyID] [varchar](50) NULL,
	[CurrencyRateFromBDT] [decimal](25, 9) NULL,
	[SaleReturnId] [varchar](20) NULL,
	[IsVDS] [varchar](1) NULL,
	[GetVDSCertificate] [varchar](1) NULL,
	[VDSCertificateDate] [datetime] NULL,
	[ImportIDExcel] [varchar](30) NULL,
	[AlReadyPrint] [int] NULL,
	[LCBank] [varchar](200) NULL,
	[LCDate] [datetime] NULL,
	[DeliveryChallanNo] [varchar](50) NULL,
	[IsGatePass] [varchar](3) NULL,
	[CompInvoiceNo] [varchar](50) NULL,
 CONSTRAINT [PK_SalesInvoiceHead] PRIMARY KEY CLUSTERED 
(
	[SalesInvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesInvoiceHeadersExport]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SalesInvoiceHeadersExport](
	[SalesInvoiceNo] [varchar](20) NOT NULL,
	[SaleLineNo] [int] NULL,
	[RefNo] [varchar](200) NULL,
	[Description] [varchar](200) NULL,
	[Quantity] [varchar](120) NULL,
	[GrossWeight] [varchar](120) NULL,
	[NetWeight] [varchar](120) NULL,
	[NumberFrom] [varchar](120) NULL,
	[NumberTo] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[PortFrom] [varchar](500) NULL,
	[PortTo] [varchar](500) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SDDeposits]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SDDeposits](
	[DepositId] [varchar](20) NOT NULL,
	[TreasuryNo] [varchar](50) NULL,
	[DepositDateTime] [datetime] NULL,
	[DepositType] [varchar](50) NULL,
	[DepositAmount] [decimal](25, 9) NULL,
	[ChequeNo] [varchar](50) NULL,
	[ChequeBank] [varchar](120) NULL,
	[ChequeBankBranch] [varchar](120) NULL,
	[ChequeDate] [datetime] NULL,
	[BankID] [varchar](20) NULL,
	[TreasuryCopy] [varchar](20) NULL,
	[DepositPerson] [varchar](120) NULL,
	[DepositPersonDesignation] [varchar](120) NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[ReverseDepositId] [varchar](20) NULL,
 CONSTRAINT [PK_SDDeposit] PRIMARY KEY CLUSTERED 
(
	[DepositId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [varchar](120) NULL,
	[SettingType] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SettingsRole]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[SettingsRole](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](20) NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [varchar](120) NULL,
	[SettingType] [varchar](120) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Setup]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Setup](
	[PurchaseP] [varchar](3) NULL,
	[PurchaseIDL] [numeric](10, 0) NULL,
	[PurchaseCID] [numeric](10, 0) NULL,
	[PurchaseNYID] [varchar](1) NULL,
	[PurchaseTradingP] [varchar](3) NULL,
	[PurchaseTradingIDL] [numeric](10, 0) NULL,
	[PurchaseTradingCID] [numeric](10, 0) NULL,
	[PurchaseTradingNYID] [varchar](1) NULL,
	[IssueP] [varchar](3) NULL,
	[IssueIDL] [numeric](10, 0) NULL,
	[IssueCID] [numeric](10, 0) NULL,
	[IssueNYID] [varchar](1) NULL,
	[IssueReturnP] [varchar](3) NULL,
	[IssueReturnIDL] [numeric](10, 0) NULL,
	[IssueReturnCID] [numeric](10, 0) NULL,
	[IssueReturnNYID] [varchar](1) NULL,
	[ReceiveP] [varchar](3) NULL,
	[ReceiveIDL] [numeric](10, 0) NULL,
	[ReceiveCID] [numeric](10, 0) NULL,
	[ReceiveNYID] [varchar](1) NULL,
	[TransferP] [varchar](3) NULL,
	[TransferIDL] [numeric](10, 0) NULL,
	[TransferCID] [numeric](10, 0) NULL,
	[TransferNYID] [varchar](1) NULL,
	[SaleP] [varchar](3) NULL,
	[SaleIDL] [numeric](10, 0) NULL,
	[SaleCID] [numeric](10, 0) NULL,
	[SaleNYID] [varchar](1) NULL,
	[SaleServiceP] [varchar](3) NULL,
	[SaleServiceIDL] [numeric](10, 0) NULL,
	[SaleServiceCID] [numeric](10, 0) NULL,
	[SaleServiceNYID] [varchar](1) NULL,
	[SaleTradingP] [varchar](3) NULL,
	[SaleTradingIDL] [numeric](10, 0) NULL,
	[SaleTradingCID] [numeric](10, 0) NULL,
	[SaleTradingNYID] [varchar](1) NULL,
	[SaleExportP] [varchar](3) NULL,
	[SaleExportIDL] [numeric](10, 0) NULL,
	[SaleExportCID] [numeric](10, 0) NULL,
	[SaleExportNYID] [varchar](1) NULL,
	[SaleTenderP] [varchar](3) NULL,
	[SaleTenderIDL] [numeric](10, 0) NULL,
	[SaleTenderCID] [numeric](10, 0) NULL,
	[SaleTenderNYID] [varchar](1) NULL,
	[DNP] [varchar](3) NULL,
	[DNIDL] [numeric](10, 0) NULL,
	[DNCID] [numeric](10, 0) NULL,
	[DNNYID] [varchar](1) NULL,
	[CNP] [varchar](3) NULL,
	[CNIDL] [numeric](10, 0) NULL,
	[CNCID] [numeric](10, 0) NULL,
	[CNNYID] [varchar](1) NULL,
	[DepositP] [varchar](3) NULL,
	[DepositIDL] [numeric](10, 0) NULL,
	[DepositCID] [numeric](10, 0) NULL,
	[DepositNYID] [varchar](1) NULL,
	[VDSP] [varchar](3) NULL,
	[VDSIDL] [numeric](10, 0) NULL,
	[VDSCID] [numeric](10, 0) NULL,
	[VDSNYID] [varchar](1) NULL,
	[TollIssueP] [varchar](3) NULL,
	[TollIssueIDL] [numeric](10, 0) NULL,
	[TollIssueCID] [numeric](10, 0) NULL,
	[TollIssueNYID] [varchar](1) NULL,
	[TollReceiveP] [varchar](3) NULL,
	[TollReceiveIDL] [numeric](10, 0) NULL,
	[TollReceiveCID] [numeric](10, 0) NULL,
	[TollReceiveNYID] [varchar](1) NULL,
	[DSFP] [varchar](3) NULL,
	[DSFIDL] [numeric](10, 0) NULL,
	[DSFCID] [numeric](10, 0) NULL,
	[DSFNYID] [varchar](1) NULL,
	[DSRP] [varchar](3) NULL,
	[DSRIDL] [numeric](10, 0) NULL,
	[DSRCID] [numeric](10, 0) NULL,
	[DSRNYID] [varchar](1) NULL,
	[IssueFromBOM] [varchar](1) NULL,
	[PrepaidVAT] [varchar](1) NULL,
	[CYear] [varchar](4) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TenderDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TenderDetails](
	[TenderId] [varchar](20) NOT NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[TenderQty] [decimal](25, 9) NULL,
	[SaleQty] [decimal](25, 9) NULL,
	[TenderPrice] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[BOMId] [varchar](20) NULL,
 CONSTRAINT [PK_TenderDetails] PRIMARY KEY CLUSTERED 
(
	[TenderId] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TenderHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TenderHeaders](
	[TenderId] [varchar](20) NOT NULL,
	[RefNo] [varchar](200) NOT NULL,
	[CustomerId] [varchar](20) NULL,
	[TenderDate] [datetime] NULL,
	[DeleveryDate] [datetime] NULL,
	[Comments] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Post] [varchar](1) NULL,
	[CustomerGroupID] [varchar](20) NULL,
 CONSTRAINT [PK_TenderHeaders] PRIMARY KEY CLUSTERED 
(
	[TenderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Trackings]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Trackings](
	[ItemNo] [varchar](20) NOT NULL,
	[TrackLineNo] [int] NULL,
	[Heading1] [varchar](200) NOT NULL,
	[Heading2] [varchar](200) NULL,
	[Quantity] [int] NULL,
	[IsPurchase] [varchar](1) NULL,
	[PurchaseInvoiceNo] [varchar](20) NOT NULL,
	[IsIssue] [varchar](1) NULL,
	[IssueNo] [varchar](20) NULL,
	[IsReceive] [varchar](1) NULL,
	[ReceiveNo] [varchar](20) NULL,
	[IsSale] [varchar](1) NULL,
	[SaleInvoiceNo] [varchar](20) NULL,
	[FinishItemNo] [varchar](20) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ReceivePost] [varchar](1) NULL,
	[SalePost] [varchar](1) NULL,
	[IssuePost] [varchar](1) NULL,
	[ReceiveDate] [datetime] NULL,
	[UnitPrice] [decimal](25, 9) NULL,
	[ReturnType] [varchar](50) NULL,
	[ReturnPurchase] [varchar](1) NULL,
	[ReturnPurchaseID] [varchar](50) NULL,
	[ReturnReceive] [varchar](1) NULL,
	[ReturnReceiveID] [varchar](50) NULL,
	[ReturnSale] [varchar](1) NULL,
	[ReturnSaleID] [varchar](50) NULL,
	[ReturnPurDate] [datetime] NULL,
	[ReturnReceiveDate] [datetime] NULL,
 CONSTRAINT [PK_Trackings] PRIMARY KEY CLUSTERED 
(
	[Heading1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionHistorys]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionHistorys](
	[TransactionNo] [varchar](20) NULL,
	[TransactionType] [varchar](50) NULL,
	[TransactionDate] [datetime] NULL,
	[ItemNo] [varchar](20) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[TradingMarkup] [decimal](25, 9) NULL,
	[SDRate] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[TCost] [decimal](25, 9) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionID] [varchar](20) NULL,
	[TransactionType] [varchar](200) NULL,
	[TransactionDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransferRawDetails]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransferRawDetails](
	[TransferId] [varchar](20) NOT NULL,
	[TransLineNo] [int] NULL,
	[ItemNo] [varchar](20) NOT NULL,
	[Quantity] [decimal](25, 9) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[UOM] [varchar](120) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMPrice] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[TransFromItemNo] [varchar](20) NOT NULL,
	[TransferDateTime] [datetime] NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransferRawHeaders]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransferRawHeaders](
	[TransferId] [varchar](20) NOT NULL,
	[TransferDateTime] [datetime] NULL,
	[TransFromItemNo] [varchar](20) NOT NULL,
	[UOM] [varchar](50) NULL,
	[CostPrice] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[TransferedQty] [decimal](25, 9) NULL,
	[TransferedAmt] [decimal](25, 9) NULL,
	[TransactionType] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_TransferRawHeaders] PRIMARY KEY CLUSTERED 
(
	[TransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UOMName]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UOMName](
	[UOMId] [int] IDENTITY(1,1) NOT NULL,
	[UOMName] [varchar](500) NULL,
	[UOMCode] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UOMs]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UOMs](
	[UOMId] [varchar](50) NULL,
	[UOMFrom] [varchar](50) NULL,
	[UOMTo] [varchar](50) NULL,
	[Convertion] [decimal](25, 9) NULL,
	[CTypes] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[ActiveStatus] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserAuditLogs]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserAuditLogs](
	[LogID] [varchar](50) NULL,
	[ComputerName] [varchar](200) NULL,
	[ComputerLoginUserName] [varchar](200) NULL,
	[ComputerIPAddress] [varchar](200) NULL,
	[SoftwareUserId] [varchar](200) NULL,
	[SessionDate] [datetime] NULL,
	[LogInDateTime] [datetime] NULL,
	[LogOutDateTime] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserGroups]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[UserGroups](
	[GroupID] [varchar](20) NULL,
	[GroupName] [varchar](120) NOT NULL,
	[Comments] [varchar](200) NOT NULL,
	[ActiveStatus] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_UserGroups] PRIMARY KEY CLUSTERED 
(
	[GroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserInformations]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInformations](
	[UserID] [varchar](20) NULL,
	[UserName] [varchar](120) NOT NULL,
	[UserPassword] [varchar](20) NULL,
	[ActiveStatus] [varchar](1) NULL,
	[LastLoginDateTime] [datetime] NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[GroupID] [varchar](20) NULL,
 CONSTRAINT [PK_UserInformations] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLogs]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserLogs](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](120) NULL,
	[LoginTime] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRolls]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRolls](
	[LineID] [numeric](18, 0) NOT NULL,
	[UserID] [varchar](20) NOT NULL,
	[FormID] [varchar](5) NOT NULL,
	[Access] [varchar](1) NULL,
	[PostAccess] [varchar](1) NULL,
	[FormName] [varchar](200) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[AddAccess] [varchar](1) NULL,
	[EditAccess] [varchar](1) NULL,
 CONSTRAINT [PK_UserRolls] PRIMARY KEY CLUSTERED 
(
	[LineID] ASC,
	[UserID] ASC,
	[FormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VAT7]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VAT7](
	[VAT7No] [varchar](20) NOT NULL,
	[Vat7Date] [datetime] NULL,
	[FinishItemNo] [varchar](20) NOT NULL,
	[FinishUOM] [varchar](50) NULL,
	[Vat7LineNo] [int] NULL,
	[ItemNo] [varchar](20) NULL,
	[UOM] [varchar](50) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UOMQty] [decimal](25, 9) NULL,
	[UOMc] [decimal](25, 9) NULL,
	[UOMn] [varchar](50) NULL,
	[Post] [varchar](1) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VDS]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VDS](
	[VDSId] [varchar](20) NULL,
	[VendorId] [varchar](20) NULL,
	[BillAmount] [decimal](25, 9) NULL,
	[BillDate] [datetime] NULL,
	[BillDeductAmount] [decimal](25, 9) NULL,
	[DepositNumber] [varchar](30) NULL,
	[DepositDate] [datetime] NULL,
	[Remarks] [varchar](200) NULL,
	[IssueDate] [datetime] NULL,
	[PurchaseNumber] [varchar](50) NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[VDSPercent] [decimal](25, 9) NULL,
	[IsPurchase] [varchar](20) NULL,
	[IsPercent] [varchar](1) NULL,
	[ReverseVDSId] [varchar](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Vehicles]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vehicles](
	[VehicleID] [varchar](20) NOT NULL,
	[VehicleCode] [varchar](50) NULL,
	[VehicleType] [varchar](50) NULL,
	[VehicleNo] [varchar](50) NULL,
	[Description] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info1] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[DriverName] [varchar](100) NULL,
 CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VendorGroups]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendorGroups](
	[VendorGroupID] [varchar](20) NOT NULL,
	[VendorGroupName] [varchar](120) NULL,
	[VendorGroupDescription] [varchar](120) NULL,
	[GroupType] [varchar](200) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[Info2] [varchar](10) NULL,
 CONSTRAINT [PK_VendorGroup] PRIMARY KEY CLUSTERED 
(
	[VendorGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vendors](
	[VendorID] [varchar](20) NOT NULL,
	[VendorCode] [varchar](50) NULL,
	[VendorName] [varchar](120) NULL,
	[VendorGroupID] [varchar](20) NULL,
	[Address1] [varchar](200) NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[ContactPerson] [varchar](150) NULL,
	[ContactPersonDesignation] [varchar](150) NULL,
	[ContactPersonTelephone] [varchar](50) NULL,
	[ContactPersonEmail] [varchar](50) NULL,
	[VATRegistrationNo] [varchar](50) NULL,
	[TINNo] [varchar](50) NULL,
	[Comments] [varchar](200) NULL,
	[ActiveStatus] [varchar](1) NOT NULL,
	[CreatedBy] [varchar](120) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](120) NULL,
	[LastModifiedOn] [datetime] NULL,
	[Country] [varchar](200) NULL,
	[Info2] [varchar](200) NULL,
	[Info3] [varchar](200) NULL,
	[Info4] [varchar](200) NULL,
	[Info5] [varchar](200) NULL,
	[VDSPercent] [decimal](25, 9) NULL,
	[BusinessType] [varchar](120) NULL,
	[BusinessCode] [varchar](20) NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[YourTable]    Script Date: 09/13/2018 11:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[YourTable](
	[BOMId] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Duties] ADD  CONSTRAINT [DF_Duties_Comments]  DEFAULT ('NA') FOR [Comments]
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties] ADD  CONSTRAINT [DF_Table_1_Comments_1]  DEFAULT ('NA') FOR [Remarks]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] ADD  CONSTRAINT [DF_SalesInvoiceDetails_AVGPrice]  DEFAULT ((0)) FOR [AVGPrice]
GO
ALTER TABLE [dbo].[BOMCompanyOverhead]  WITH CHECK ADD  CONSTRAINT [FK_BOMCompanyOverhead_BOMId] FOREIGN KEY([BOMId])
REFERENCES [dbo].[BOMs] ([BOMId])
GO
ALTER TABLE [dbo].[BOMCompanyOverhead] CHECK CONSTRAINT [FK_BOMCompanyOverhead_BOMId]
GO
ALTER TABLE [dbo].[BOMRaws]  WITH CHECK ADD  CONSTRAINT [FK_BOMRaws_BOMId] FOREIGN KEY([BOMId])
REFERENCES [dbo].[BOMs] ([BOMId])
GO
ALTER TABLE [dbo].[BOMRaws] CHECK CONSTRAINT [FK_BOMRaws_BOMId]
GO
ALTER TABLE [dbo].[BOMs]  WITH CHECK ADD  CONSTRAINT [FK_BOMs_FinishItemNo] FOREIGN KEY([FinishItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[BOMs] CHECK CONSTRAINT [FK_BOMs_FinishItemNo]
GO
ALTER TABLE [dbo].[CompanyOverheadVAT]  WITH CHECK ADD  CONSTRAINT [FK_CompanyOverheadVAT_CompanyOverheads] FOREIGN KEY([HeadName])
REFERENCES [dbo].[CompanyOverheads] ([HeadName])
GO
ALTER TABLE [dbo].[CompanyOverheadVAT] CHECK CONSTRAINT [FK_CompanyOverheadVAT_CompanyOverheads]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_CustomerGroups1] FOREIGN KEY([CustomerGroupID])
REFERENCES [dbo].[CustomerGroups] ([CustomerGroupID])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_CustomerGroups1]
GO
ALTER TABLE [dbo].[DisposeDetails]  WITH CHECK ADD  CONSTRAINT [FK_DisposeDetails_DisposeHeaders] FOREIGN KEY([DisposeNumber])
REFERENCES [dbo].[DisposeHeaders] ([DisposeNumber])
GO
ALTER TABLE [dbo].[DisposeDetails] CHECK CONSTRAINT [FK_DisposeDetails_DisposeHeaders]
GO
ALTER TABLE [dbo].[DisposeDetails]  WITH CHECK ADD  CONSTRAINT [FK_DisposeDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[DisposeDetails] CHECK CONSTRAINT [FK_DisposeDetails_Products]
GO
ALTER TABLE [dbo].[IssueDetails]  WITH CHECK ADD  CONSTRAINT [FK_IssueDetails_IssueHeaders] FOREIGN KEY([IssueNo])
REFERENCES [dbo].[IssueHeaders] ([IssueNo])
GO
ALTER TABLE [dbo].[IssueDetails] CHECK CONSTRAINT [FK_IssueDetails_IssueHeaders]
GO
ALTER TABLE [dbo].[IssueDetails]  WITH CHECK ADD  CONSTRAINT [FK_IssueDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[IssueDetails] CHECK CONSTRAINT [FK_IssueDetails_Products]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ProductCategories1] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ProductCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ProductCategories1]
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseInvoiceDuties_PurchaseInvoiceHeaders] FOREIGN KEY([PurchaseInvoiceNo])
REFERENCES [dbo].[PurchaseInvoiceHeaders] ([PurchaseInvoiceNo])
GO
ALTER TABLE [dbo].[PurchaseInvoiceDuties] CHECK CONSTRAINT [FK_PurchaseInvoiceDuties_PurchaseInvoiceHeaders]
GO
ALTER TABLE [dbo].[PurchaseInvoiceHeaders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseInvoiceHeaders_Vendors] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendors] ([VendorID])
GO
ALTER TABLE [dbo].[PurchaseInvoiceHeaders] CHECK CONSTRAINT [FK_PurchaseInvoiceHeaders_Vendors]
GO
ALTER TABLE [dbo].[ReceiveDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[ReceiveDetails] CHECK CONSTRAINT [FK_ReceiveDetails_Products]
GO
ALTER TABLE [dbo].[ReceiveDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveDetails_ReceiveHeaders] FOREIGN KEY([ReceiveNo])
REFERENCES [dbo].[ReceiveHeaders] ([ReceiveNo])
GO
ALTER TABLE [dbo].[ReceiveDetails] CHECK CONSTRAINT [FK_ReceiveDetails_ReceiveHeaders]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceDetails_Products1] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] CHECK CONSTRAINT [FK_SalesInvoiceDetails_Products1]
GO
ALTER TABLE [dbo].[SalesInvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceDetails_SalesInvoiceHeaders1] FOREIGN KEY([SalesInvoiceNo])
REFERENCES [dbo].[SalesInvoiceHeaders] ([SalesInvoiceNo])
GO
ALTER TABLE [dbo].[SalesInvoiceDetails] CHECK CONSTRAINT [FK_SalesInvoiceDetails_SalesInvoiceHeaders1]
GO
ALTER TABLE [dbo].[SalesInvoiceHeaders]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceHeaders_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SalesInvoiceHeaders] CHECK CONSTRAINT [FK_SalesInvoiceHeaders_Customers]
GO
ALTER TABLE [dbo].[SalesInvoiceHeadersExport]  WITH CHECK ADD  CONSTRAINT [FK_SalesInvoiceHeadersExport_SalesInvoiceHeaders] FOREIGN KEY([SalesInvoiceNo])
REFERENCES [dbo].[SalesInvoiceHeaders] ([SalesInvoiceNo])
GO
ALTER TABLE [dbo].[SalesInvoiceHeadersExport] CHECK CONSTRAINT [FK_SalesInvoiceHeadersExport_SalesInvoiceHeaders]
GO
ALTER TABLE [dbo].[TenderDetails]  WITH CHECK ADD  CONSTRAINT [FK_TenderDetails_Products] FOREIGN KEY([ItemNo])
REFERENCES [dbo].[Products] ([ItemNo])
GO
ALTER TABLE [dbo].[TenderDetails] CHECK CONSTRAINT [FK_TenderDetails_Products]
GO
ALTER TABLE [dbo].[TenderDetails]  WITH CHECK ADD  CONSTRAINT [FK_TenderDetails_TenderHeaders] FOREIGN KEY([TenderId])
REFERENCES [dbo].[TenderHeaders] ([TenderId])
GO
ALTER TABLE [dbo].[TenderDetails] CHECK CONSTRAINT [FK_TenderDetails_TenderHeaders]
GO
ALTER TABLE [dbo].[TenderHeaders]  WITH CHECK ADD  CONSTRAINT [FK_TenderHeaders_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[TenderHeaders] CHECK CONSTRAINT [FK_TenderHeaders_Customers]
GO
ALTER TABLE [dbo].[UserLogs]  WITH CHECK ADD  CONSTRAINT [FK_UserLogs_UserInformations] FOREIGN KEY([UserName])
REFERENCES [dbo].[UserInformations] ([UserName])
GO
ALTER TABLE [dbo].[UserLogs] CHECK CONSTRAINT [FK_UserLogs_UserInformations]
GO
ALTER TABLE [dbo].[Vendors]  WITH CHECK ADD  CONSTRAINT [FK_Vendors_VendorGroups1] FOREIGN KEY([VendorGroupID])
REFERENCES [dbo].[VendorGroups] ([VendorGroupID])
GO
ALTER TABLE [dbo].[Vendors] CHECK CONSTRAINT [FK_Vendors_VendorGroups1]
GO

                ";
                #endregion CreateTable

                top1 = "go";

                IEnumerable<string> commandStrings = Regex.Split(sqlText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        SqlCommand cmdIdExist1 = new SqlCommand(commandString, currConn);

                        //new SqlCommand(commandString, currConn).ExecuteNonQuery();
                        cmdIdExist1.Transaction = transaction;
                        transResult = (int)cmdIdExist1.ExecuteNonQuery();
                        if (transResult != -1)
                        {
                            throw new ArgumentNullException("Create Tables to database('" + databaseName + "')", MessageVM.dbMsgTableNotCreate);
                        }
                    }
                }

                #endregion TableCreate

                #region TableDefaultData
                string top2;
                // vendor group, vehicle,UserInformations,CustomerGroups,Vendors,Customers
                //userroll,,settings,ProductTypes,codes,Currencies,CurrencyConversion
                #region TableDefaultData Back
                //             sqlText = @"
                //INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)
                //INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[UserInformations] ([UserID], [UserName], [UserPassword], [ActiveStatus], [LastLoginDateTime], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'10', N'admin', N'J7LJ8+qT64o=', N'Y', CAST(0x0000A04D00B82888 AS DateTime), N'KamrulInsert', CAST(0x0000A01400EF44BC AS DateTime), N'admin', CAST(0x0000A08400D5C30C AS DateTime), N'Info1', N'Info2', N'Info3', N'Info4', N'Info5')
                //INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')
                //INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL)


                //";
                #endregion TableDefaultData Back

                #region TableDefaultData Back
                sqlText = @"
               
INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'0', N'NA', N'NA', N'NA', N'Overhead', N'0.00', CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'admin', CAST(0x0000A16400F8CA3C AS DateTime), N'admin', CAST(0x0000A1A30106ECFC AS DateTime), CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'NA', N'NA')

INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'1', N'Finish', N'-', N'-', N'Finish', N'0.00', CAST(15.000000000 AS Decimal(25, 9)), N'N', N'Y', N'admin', CAST(0x0000A95A0185F808 AS DateTime), N'admin', CAST(0x0000A95A0185F808 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), N'N', N'N', NULL, NULL)

INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'2', N'Admin Expense', N'-', N'-', N'Overhead', N'0.00', CAST(0.000000000 AS Decimal(25, 9)), N'N', N'Y', N'admin', CAST(0x0000A95A0186DDB8 AS DateTime), N'admin', CAST(0x0000A95A0186DDB8 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), N'N', N'N', NULL, NULL)

INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'3', N'Raw', N'-', N'-', N'Raw', N'0.00', CAST(15.000000000 AS Decimal(25, 9)), N'N', N'Y', N'admin', CAST(0x0000A95A01870914 AS DateTime), N'admin', CAST(0x0000A95A01870DC4 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), N'N', N'N', NULL, NULL)

INSERT [dbo].[Products] ([ItemNo], [ProductCode], [ProductName], [ProductDescription], [CategoryID], [UOM], [CostPrice], [SalesPrice], [NBRPrice], [ReceivePrice], [IssuePrice], [TenderPrice], [ExportPrice], [InternalIssuePrice], [TollIssuePrice], [TollCharge], [OpeningBalance], [SerialNo], [HSCodeNo], [VATRate], [Comments], [SD], [PacketPrice], [Trading], [TradingMarkUp], [NonStock], [QuantityInHand], [OpeningDate], [RebatePercent], [TVBRate], [CnFRate], [InsuranceRate], [CDRate], [RDRate], [AITRate], [TVARate], [ATVRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [OpeningTotalCost], [TollProduct], [Banderol]) VALUES (N'ovh0', N'ovh0', N'Margin', N'-', N'0', N'-', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'-', N'', CAST(0.000000000 AS Decimal(25, 9)), N'', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), CAST(0x0000A1A40105ED84 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Y', N'admin', CAST(0x0000A1A401060044 AS DateTime), N'admin', CAST(0x0000A1A401224A74 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')

INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'1', N'Local', N'-', N'Local', N'-', N'Y', N'admin', CAST(0x0000A95A01865820 AS DateTime), N'admin', CAST(0x0000A95A01865820 AS DateTime), NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'2', N'Export', N'-', N'Export', N'-', N'Y', N'admin', CAST(0x0000A95A01865F28 AS DateTime), N'admin', CAST(0x0000A95A01865F28 AS DateTime), NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country], [VDSPercent], [BusinessType], [BusinessCode]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL, NULL, NULL, NULL)

INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)

INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'1', N'Local', N'-', N'Local', N'-', N'Y', N'admin', CAST(0x0000A95A01863F84 AS DateTime), N'admin', CAST(0x0000A95A01863F84 AS DateTime), NULL, NULL, NULL, NULL)

INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'2', N'Import', N'-', N'Import', N'-', N'Y', N'admin', CAST(0x0000A95A018647B8 AS DateTime), N'admin', CAST(0x0000A95A018647B8 AS DateTime), NULL, NULL, NULL, NULL)

INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5], [VDSPercent], [BusinessType], [BusinessCode]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', NULL, NULL, NULL)

INSERT [dbo].[UserInformations] ([UserID], [UserName], [UserPassword], [ActiveStatus], [LastLoginDateTime], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5], [GroupID]) VALUES (N'10', N'admin', N'J7LJ8+qT64o=', N'Y', CAST(0x0000A04D00B82888 AS DateTime), N'KamrulInsert', CAST(0x0000A01400EF44BC AS DateTime), N'admin', CAST(0x0000A08400D5C30C AS DateTime), N'Info1', N'Info2', N'Info3', N'Info4', N'Info5', NULL)

INSERT [dbo].[BankInformations] ([BankID], [BankCode], [BankName], [BranchName], [AccountNumber], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'0', N'NA', N'NA', N'NA', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'Y', N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[BankInformations] ([BankID], [BankCode], [BankName], [BranchName], [AccountNumber], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'1', N'SB', N'Sonali Bank Bangladesh', N'Motijhil', N'----', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'Y', N'admin', CAST(0x0000A95A01876B84 AS DateTime), N'admin', CAST(0x0000A95A01876B84 AS DateTime), NULL, NULL, NULL, NULL, NULL)

SET IDENTITY_INSERT [dbo].[Codes] ON 


INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'Purchase', N'Other', N'PUR', N'8', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'Purchase', N'Trading', N'PTD', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'Receive', N'Other', N'REC', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'TollReceive', N'TollReceive', N'TOR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'Purchase', N'PurchaseReturn', N'PRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'Purchase', N'InputService', N'PIS', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'Purchase', N'Import', N'IMP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'TollReceiveRaw', N'TollReceiveRaw', N'TRW', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (9, N'Issue', N'Other', N'ISU', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (10, N'Issue', N'IssueReturn', N'ISR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'Receive', N'ReceiveReturn', N'RER', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'Receive', N'WIP', N'WIP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'TollFinishReceive', N'TollFinishReceive', N'TFR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'Sale', N'Other', N'INV', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'Sale', N'Trading', N'STP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (17, N'Sale', N'Debit', N'DEN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'Sale', N'Credit', N'CRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'Sale', N'Export', N'STR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'InternalIssue', N'InternalIssue', N'TRN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (21, N'Sale', N'Service', N'SER', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (22, N'Sale', N'Tender', N'STN', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'TollIssue', N'TollIssue', N'EDF', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'TollFinishIssue', N'TollFinishIssue', N'TFI', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'Deposit', N'Treasury', N'DEP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'Deposit', N'VDS', N'VDS', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'Dispose', N'Raw', N'DSR', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'Dispose', N'Finish', N'DSF', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'Adjustment', N'Both', N'ADJ', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'SDDeposit', N'Treasury', N'SDP', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'DDB', N'DDB', N'DDB', N'4', N'Y', N'admin', CAST(0x00009FCB00000000 AS DateTime), N'admin', CAST(0x00009FCB00000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'Purchase', N'PurchaseDN', N'PDN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'Purchase', N'PurchaseCN', N'PCN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'Sale', N'ServiceNS', N'SNS', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (35, N'Purchase', N'ServiceNS', N'PSN', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))

INSERT [dbo].[Codes] ([CodeId], [CodeGroup], [CodeName], [prefix], [Lenth], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (36, N'Purchase', N'Service', N'PSE', N'4', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime))

SET IDENTITY_INSERT [dbo].[Codes] OFF

SET IDENTITY_INSERT [dbo].[Currencies] ON 


INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (246, N'Afghanistan Afghani', N'AFN', N'Afghanistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'afghani', N'Pul', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (247, N'Albanian Lek', N'ALL', N'Albania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lek', N'Qindarkë', N'L')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (248, N'Algerian Dinar', N'DZD', N'Algeria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Santeem', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (249, N'US Dollar', N'USD', N'American Samoa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (250, N'Euro', N'EUR', N'Andorra', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Euro', N'Cent', N'€')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (251, N'Anlan Kwanza', N'AOA', N'Anla', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kwanza', N'Cêntimo', N'Kz')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (252, N'East Caribbean Dollar', N'XCD', N'Anguilla', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (253, N'Argentine Peso', N'ARS', N'Argentina', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (254, N'Armenian Dram', N'AMD', N'Armenia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dram', N'Luma', N'')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (255, N'Aruban Guilder', N'AWG', N'Aruba', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'florin', N'Cent', N'ƒ')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (256, N'Australian Dollar', N'AUD', N'Australia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (257, N'Azerbaijan New Manat', N'AZN', N'Azerbaijan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'manat', N'Q?pik', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (258, N'Bahamian Dollar', N'BSD', N'Bahamas', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (259, N'Bahraini Dinar', N'BHD', N'Bahrain', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Fils', N'.?.?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (260, N'Bangladeshi Taka', N'BDT', N'Bangladesh', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'taka', N'Paisa', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (261, N'Barbados Dollar', N'BBD', N'Barbados', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (262, N'Belarussian Ruble', N'BYR', N'Belarus', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'ruble', N'Kapyeyka', N'Br')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (263, N'Belize Dollar', N'BZD', N'Belize', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (264, N'CFA Franc BCEAO', N'XOF', N'Benin', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (265, N'Bermudian Dollar', N'BMD', N'Bermuda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (266, N'Bhutan Ngultrum', N'BTN', N'Bhutan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'ngultrum', N'Chetrum', N'Nu.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (267, N'Boliviano', N'BOB', N'Bolivia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'boliviano', N'Centavo', N'Bs.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (268, N'Marka', N'BAM', N'Bosnia-Herzevina', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'mark', N'Fening', N'KM')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (269, N'Botswana Pula', N'BWP', N'Botswana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pula', N'Thebe', N'P')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (270, N'Norwegian Krone', N'NOK', N'Bouvet Island', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'krone', N'Øre', N'kr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (271, N'Brazilian Real', N'BRL', N'Brazil', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'real', N'Centavo', N'R$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (272, N'Brunei Dollar', N'BND', N'Brunei Darussalam', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Sen', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (273, N'Bulgarian Lev', N'BGN', N'Bulgaria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lev', N'Stotinka', N'??')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (274, N'Burundi Franc', N'BIF', N'Burundi', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (275, N'Kampuchean Riel', N'KHR', N'Cambodia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'riel', N'Sen', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (276, N'CFA Franc BEAC', N'XAF', N'Cameroon', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (277, N'Canadian Dollar', N'CAD', N'Canada', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (278, N'Cape Verde Escudo', N'CVE', N'Cape Verde', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'escudo', N'Centavo', N' $')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (279, N'Cayman Islands Dollar', N'KYD', N'Cayman Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (280, N'Chilean Peso', N'CLP', N'Chile', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (281, N'Yuan Renminbi', N'CNY', N'China', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'yuan', N'Fen', N'¥ ')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (282, N'Colombian Peso', N'COP', N'Colombia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (283, N'Comoros Franc', N'KMF', N'Comoros', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (284, N'Francs', N'CDF', N'Con, Dem. Republic', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (285, N'New Zealand Dollar', N'NZD', N'Cook Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (286, N'Costa Rican Colon', N'CRC', N'Costa Rica', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'colón', N'Céntimo', N'¢')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (287, N'Croatian Kuna', N'HRK', N'Croatia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kuna', N'Lipa', N'kn')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (288, N'Cuban Peso', N'CUP', N'Cuba', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (289, N'Czech Koruna', N'CZK', N'Czech Rep.', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'koruna', N'Halér', N'Kc')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (290, N'Danish Krone', N'DKK', N'Denmark', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'krone', N'Øre', N'kr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (291, N'Djibouti Franc', N'DJF', N'Djibouti', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (292, N'Dominican Peso', N'DOP', N'Dominican Republic', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (293, N'Ecuador Sucre', N'ECS', N'Ecuador', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (294, N'Egyptian Pound', N'EGP', N'Egypt', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Piastre', N'£ ')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (295, N'El Salvador Colon', N'SVC', N'El Salvador', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (296, N'Eritrean Nakfa', N'ERN', N'Eritrea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'nakfa', N'Cent', N'Nfk')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (297, N'Ethiopian Birr', N'ETB', N'Ethiopia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'birr', N'Santim', N'Br')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (298, N'Falkland Islands Pound', N'FKP', N'Falkland Islands (Malvinas)', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Penny', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (299, N'Fiji Dollar', N'FJD', N'Fiji', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (300, N'Gambian Dalasi', N'GMD', N'Gambia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dalasi', N'Butut', N'D')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (301, N'Georgian Lari', N'GEL', N'Georgia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lari', N'Tetri', N'.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (302, N'Ghanaian Cedi', N'GHS', N'Ghana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'cedi', N'Pesewa', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (303, N'Gibraltar Pound', N'GIP', N'Gibraltar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Penny', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (304, N'Pound Sterling', N'GBP', N'Great Britain', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Penny', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (305, N'East Carribean Dollar', N'XCD', N'Grenada', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (306, N'Guatemalan Quetzal', N'QTQ', N'Guatemala', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (307, N'Pound Sterling', N'GGP', N'Guernsey', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (308, N'Guinea Franc', N'GNF', N'Guinea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (309, N'Guinea-Bissau Peso', N'GWP', N'Guinea Bissau', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (310, N'Guyana Dollar', N'GYD', N'Guyana', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (311, N'Haitian urde', N'HTG', N'Haiti', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'urde', N'Centime', N'G')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (312, N'Honduran Lempira', N'HNL', N'Honduras', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lempira', N'Centavo', N'L')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (313, N'Hong Kong Dollar', N'HKD', N'Hong Kong', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (314, N'Hungarian Forint', N'HUF', N'Hungary', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'forint', N'Fillér', N'Ft')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (315, N'Iceland Krona', N'ISK', N'Iceland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'króna', N'Eyrir', N'kr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (316, N'Indian Rupee', N'INR', N'India', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Paisa', N'INR')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (317, N'Indonesian Rupiah', N'IDR', N'Indonesia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupiah', N'Sen', N'Rp')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (318, N'Iranian Rial', N'IRR', N'Iran', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rial', N'Dinar', N'')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (319, N'Iraqi Dinar', N'IQD', N'Iraq', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Fils', N'?.?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (320, N'Israeli New Shekel', N'ILS', N'Israel', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'shekel', N'Ara', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (321, N'Jamaican Dollar', N'JMD', N'Jamaica', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (322, N'Japanese Yen', N'JPY', N'Japan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'yen', N'Sen', N'¥')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (323, N'Jordanian Dinar', N'JOD', N'Jordan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Piastre', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (324, N'Kazakhstan Tenge', N'KZT', N'Kazakhstan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'tenge', N'Tïin', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (325, N'Kenyan Shilling', N'KES', N'Kenya', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'shilling', N'Cent', N'Sh')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (326, N'North Korean Won', N'KPW', N'Korea-North', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'won', N'Chon', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (327, N'Korean Won', N'KRW', N'Korea-South', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'won', N'Jeon', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (328, N'Kuwaiti Dinar', N'KWD', N'Kuwait', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Fils', N'? or K.D')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (329, N'Som', N'KGS', N'Kyrgyzstan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'som', N'Tyiyn', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (330, N'Lao Kip', N'LAK', N'Laos', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kip', N'Att', N'? or ?N')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (331, N'Latvian Lats', N'LVL', N'Latvia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lats', N'Santims', N'Ls')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (332, N'Lebanese Pound', N'LBP', N'Lebanon', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Piastre', N'?.?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (333, N'Lesotho Loti', N'LSL', N'Lesotho', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'loti', N'Sente', N'L')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (334, N'Liberian Dollar', N'LRD', N'Liberia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (335, N'Libyan Dinar', N'LYD', N'Libya', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Dirham', N'LD')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (336, N'Swiss Franc', N'CHF', N'Liechtenstein', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Rappen', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (337, N'Lithuanian Litas', N'LTL', N'Lithuania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'litas', N'Centas', N'Lt')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (338, N'Macau Pataca', N'MOP', N'Macau', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pataca', N'Avo', N'P')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (339, N'Denar', N'MKD', N'Macedonia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'denar', N'Deni', N'???')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (340, N'Malagasy Franc', N'MGF', N'Madagascar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (341, N'Malawi Kwacha', N'MWK', N'Malawi', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kwacha', N'Tambala', N'MK')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (342, N'Malaysian Ringgit', N'MYR', N'Malaysia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'ringgit', N'Sen', N'RM')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (343, N'Maldive Rufiyaa', N'MVR', N'Maldives', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rufiyaa', N'Laari', N'MVR')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (344, N'Mauritanian Ouguiya', N'MRO', N'Mauritania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'ouguiya', N'Khoums', N'UM')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (345, N'Mauritius Rupee', N'MUR', N'Mauritius', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Cent', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (346, N'Mexican Nuevo Peso', N'MXN', N'Mexico', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (347, N'Moldovan Leu', N'MDL', N'Moldova', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'leu', N'Ban', N'L')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (348, N'Monlian Tugrik', N'MNT', N'Monlia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'tögrög', N'Möngö', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (349, N'Moroccan Dirham', N'MAD', N'Morocco', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dirham', N'Centime', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (350, N'Mozambique Metical', N'MZN', N'Mozambique', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'metical', N'Centavo', N'MT')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (351, N'Myanmar Kyat', N'MMK', N'Myanmar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kyat', N'Pya', N'Ks')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (352, N'Namibian Dollar', N'NAD', N'Namibia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (353, N'Nepalese Rupee', N'NPR', N'Nepal', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Paisa', N'Rs')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (354, N'Netherlands Antillean Guilder', N'ANG', N'Netherlands Antilles', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'guilder', N'Cent', N'ƒ')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (355, N'CFP Franc', N'XPF', N'New Caledonia (French)', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (356, N'Nicaraguan Cordoba Oro', N'NIO', N'Nicaragua', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'córdoba', N'Centavo', N'C$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (357, N'Nigerian Naira', N'NGN', N'Nigeria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'naira', N'Kobo', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (358, N'Omani Rial', N'OMR', N'Oman', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rial', N'Baisa', N'?.?.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (359, N'Pakistan Rupee', N'PKR', N'Pakistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Paisa', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (360, N'Panamanian Balboa', N'PAB', N'Panama', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'balboa', N'Centésimo', N'B/.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (361, N'Papua New Guinea Kina', N'PGK', N'Papua New Guinea', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kina', N'Toea', N'K')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (362, N'Paraguay Guarani', N'PYG', N'Paraguay', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'guaraní', N'Céntimo', N' (? in unicode)')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (363, N'Peruvian Nuevo Sol', N'PEN', N'Peru', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'sol', N'Céntimo', N'S/.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (364, N'Philippine Peso', N'PHP', N'Philippines', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centavo', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (365, N'Polish Zloty', N'PLN', N'Poland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'zloty', N'Grosz', N'zl')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (366, N'Qatari Rial', N'QAR', N'Qatar', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'riyal', N'Dirham', N'QR or ?.?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (367, N'Romanian New Leu', N'RON', N'Romania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'leu', N'Ban', N'lei')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (368, N'Russian Ruble', N'RUB', N'Russia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'ruble', N'Kopek', N'')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (369, N'Rwanda Franc', N'RWF', N'Rwanda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'franc', N'Centime', N'Fr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (370, N'St. Helena Pound', N'SHP', N'Saint Helena', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Penny', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (371, N'Samoan Tala', N'WST', N'Samoa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'tala', N'Sene', N'T')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (372, N'Dobra', N'STD', N'Sao Tome and Principe', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dobra', N'Cêntimo', N'Db')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (373, N'Saudi Riyal', N'SAR', N'Saudi Arabia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'riyal', N'Halala', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (374, N'Dinar', N'RSD', N'Serbia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Para', N'din.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (375, N'Seychelles Rupee', N'SCR', N'Seychelles', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Cent', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (376, N'Sierra Leone Leone', N'SLL', N'Sierra Leone', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'leone', N'Cent', N'Le')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (377, N'Singapore Dollar', N'SGD', N'Singapore', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (378, N'Solomon Islands Dollar', N'SBD', N'Solomon Islands', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (379, N'Somali Shilling', N'SOS', N'Somalia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'shilling', N'Cent', N'Sh')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (380, N'South African Rand', N'ZAR', N'South Africa', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rand', N'Cent', N'R')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (381, N'South Sudan Pound', N'SSP', N'South Sudan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Piastre', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (382, N'Sri Lanka Rupee', N'LKR', N'Sri Lanka', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rupee', N'Cent', N'Rs')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (383, N'Sudanese Pound', N'SDG', N'Sudan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Piastre', N'£')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (384, N'Surinam Dollar', N'SRD', N'Suriname', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (385, N'Swaziland Lilangeni', N'SZL', N'Swaziland', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lilangeni', N'Cent', N'L')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (386, N'Swedish Krona', N'SEK', N'Sweden', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'krona', N'Öre', N'kr')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (387, N'Syrian Pound', N'SYP', N'Syria', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pound', N'Piastre', N'£ ')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (388, N'Taiwan Dollar', N'TWD', N'Taiwan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (389, N'Tajik Somoni', N'TJS', N'Tajikistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'somoni', N'Diram', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (390, N'Tanzanian Shilling', N'TZS', N'Tanzania', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'shilling', N'Cent', N'Sh')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (391, N'Thai Baht', N'THB', N'Thailand', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'baht', N'Satang', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (392, N'Tongan Paanga', N'TOP', N'Tonga', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'pa?anga', N'Seniti', N'T$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (393, N'Trinidad and Toba Dollar', N'TTD', N'Trinidad and Toba', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dollar', N'Cent', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (394, N'Tunisian Dollar', N'TND', N'Tunisia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dinar', N'Millime', N'DT')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (395, N'Turkish Lira', N'TRY', N'Turkey', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'lira', N'Kurus', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (396, N'Manat', N'TMT', N'Turkmenistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'manat', N'Tennesi', N'm')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (397, N'Uganda Shilling', N'UGX', N'Uganda', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'shilling', N'Cent', N'Sh')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (398, N'Ukraine Hryvnia', N'UAH', N'Ukraine', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'hryvnia', N'Kopiyka', N'?')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (399, N'Arab Emirates Dirham', N'AED', N'United Arab Emirates', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'dirham', N'Fils', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (400, N'Uruguayan Peso', N'UYU', N'Uruguay', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'peso', N'Centésimo', N'$')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (401, N'Uzbekistan Sum', N'UZS', N'Uzbekistan', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'som', N'Tiyin', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (402, N'Vanuatu Vatu', N'VUV', N'Vanuatu', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'vatu', N'None', N'Vt')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (403, N'Venezuelan Bolivar', N'VEF', N'Venezuela', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'bolívar', N'Céntimo', N'Bs F')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (404, N'Vietnamese Dong', N'VND', N'Vietnam', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'd?ng', N'Hào', N'.')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (405, N'Yemeni Rial', N'YER', N'Yemen', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'rial', N'Fils', N'None')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (406, N'Zambian Kwacha', N'ZMW', N'Zambia', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'kwacha', N'Ngwee', N'ZK')

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyName], [CurrencyCode], [Country], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [CurrencyMajor], [CurrencyMinor], [CurrencySymbol]) VALUES (407, N'Zimbabwe Dollar', N'ZWD', N'Zimbabwe', N'N/A', N'Y', N'Admin', CAST(0x00009FCB00000000 AS DateTime), N'Admin', CAST(0x00009FCB00000000 AS DateTime), NULL, NULL, NULL)

SET IDENTITY_INSERT [dbo].[Currencies] OFF

INSERT [dbo].[CurrencyConversion] ([CurrencyConversionId], [CurrencyCodeFrom], [CurrencyCodeTo], [CurrencyRate], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [ConversionDate]) VALUES (N'1', N'249', N'260', CAST(80.0000000000 AS Decimal(18, 10)), N'NA', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime), CAST(0x0000000000000000 AS DateTime))

INSERT [dbo].[CurrencyConversion] ([CurrencyConversionId], [CurrencyCodeFrom], [CurrencyCodeTo], [CurrencyRate], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [ConversionDate]) VALUES (N'2', N'260', N'260', CAST(1.0000000000 AS Decimal(18, 10)), N'NA', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000000000000000 AS DateTime), CAST(0x0000000000000000 AS DateTime))

SET IDENTITY_INSERT [dbo].[Settings] ON 


INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'Purchase', N'TotalPrice', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'Purchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (3, N'Sale', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'Issue', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'Sale', N'QuantityDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'Sale', N'TakaDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'Sale', N'DollerDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'Sale', N'RateDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (54, N'PriceDeclaration', N'LocalInVAT1', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (55, N'PriceDeclaration', N'LocalInVAT1Ka(Tarrif)', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'ImportPurchase', N'FixedCnF', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'ImportPurchase', N'FixedInsurance', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'ImportPurchase', N'CalculativeAV', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'ImportPurchase', N'FixedCD', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'ImportPurchase', N'FixedRD', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'ImportPurchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'ImportPurchase', N'FixedTVB', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'ImportPurchase', N'FixedTVA', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'ImportPurchase', N'FixedATV', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'ImportPurchase', N'FixedOthers', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'ImportPurchase', N'FixedSD', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'AutoCode', N'Item', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'AutoCode', N'Customer', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'AutoCode', N'Vendor', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'AutoCode', N'Bank', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'AutoCode', N'OverHead', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'IssueFromBOM', N'IssueFromBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'PrepaidVAT', N'PrepaidVAT', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'Sale', N'ItemNature', N'ELECTRIC WIRE/CABLE', N'string', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'BOM', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'BOM', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (39, N'BOM', N'ItemNature', N'SHAFIQKAMRUL', N'string', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (40, N'BOM', N'IntermediateProduction', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (42, N'Sale', N'NumberOfItems', N'15', N'int', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (43, N'Production', N'ProductionWithoutBOM', N'Y', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (44, N'IssueFromBOM', N'IssueAutoPost', N'N', N'bool', N'Y', N'admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (45, N'Sale', N'ATVRate', N'0.0', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (46, N'Sale', N'WareHouseRentPerQuantity', N'0.0', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (47, N'Sale', N'CommercialImporter', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (48, N'DatabaseName', N'DatabaseName', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (49, N'Import', N'SaleExistContinue', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (50, N'Purchase', N'TrackingWithSale', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (51, N'Purchase', N'TrackingWithSaleFIFO', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (52, N'BOM', N'VAT1(TollIssue)WithRaw', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (53, N'VAT18', N'VAT18Digit4', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (56, N'PriceDeclaration', N'TenderInVAT1', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (78, N'Sale', N'Page3Plyer', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (79, N'Sale', N'CreditWithoutTransaction', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (80, N'Sale', N'VAT11A4', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (81, N'Sale', N'VAT11Letter', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (82, N'Sale', N'VAT11Legal', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (83, N'Sale', N'TenderSaleFromBOM', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (84, N'Issue', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (85, N'Issue', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (86, N'Purchase', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (87, N'Purchase', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (88, N'Receive', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (89, N'Receive', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (90, N'Receive', N'PriceDeclarationForImport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (91, N'Purchase', N'RateChangePromote', N'7.5', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (92, N'Banderol', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (93, N'Printer', N'DefaultPrinter', N' ', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (94, N'Printer', N'MaxNoOfPrint', N'10', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (95, N'Printer', N'EmployeeSalary(BDE)', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (96, N'BOM', N'NetCost', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (97, N'TollItemReceive', N'AttachedWithVAT16', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (98, N'ImportTender', N'CustomerGroup', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (99, N'TrackingTrace', N'Tracking', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (100, N'TrackingTrace', N'TrackingNo', N'2', N'int', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (101, N'TrackingTrace', N'TrackingHead_1', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (102, N'TrackingTrace', N'TrackingHead_2', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (103, N'CompanyCode', N'Code', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A0187197C AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (57, N'PriceDeclaration', N'TenderInVAT1(Tender)', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (58, N'PriceDeclaration', N'TenderPriceWithVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (59, N'Purchase', N'ImportCostingIncludeATV', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (60, N'TollReceive', N'WithIssue', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (61, N'Reports', N'VAT11', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (62, N'BOM', N'VAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (63, N'BOM', N'A4VAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (64, N'BOM', N'LegalVAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (65, N'Sale', N'PackingInExport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (66, N'Sale', N'CustomerWiseBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (67, N'Receive', N'CustomerWiseBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (68, N'BOM', N'TollIssueCostWithOthers', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (69, N'BOM', N'RptBOMCostingA4', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (70, N'VAT19', N'ExportInBDT', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (71, N'BOM', N'InputServicePercent', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (72, N'Sale', N'ChangeableNBRPrice', N'N', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (73, N'Production', N'ChangeableNBRPrice', N'N', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (74, N'VAT17', N'AutoAdjustment', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (75, N'VAT16', N'AutoAdjustment', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (76, N'Sale', N'PriceDeclarationForImport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[Settings] ([SettingId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (77, N'Sale', N'ReportNumberOfCopiesPrint', N'1', N'int', N'Y', N'admin', CAST(0x0000A95A01871850 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

SET IDENTITY_INSERT [dbo].[Settings] OFF

SET IDENTITY_INSERT [dbo].[SettingsRole] ON 


INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'10', N'AutoCode', N'Bank', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'10', N'AutoCode', N'Customer', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (3, N'10', N'AutoCode', N'Item', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'10', N'AutoCode', N'OverHead', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'10', N'AutoCode', N'Vendor', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'10', N'Banderol', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'10', N'BOM', N'A4VAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'10', N'BOM', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (9, N'10', N'BOM', N'InputServicePercent', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (10, N'10', N'BOM', N'IntermediateProduction', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'10', N'BOM', N'ItemNature', N'SHAFIQKAMRUL', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'10', N'BOM', N'LegalVAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'10', N'BOM', N'NetCost', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'10', N'BOM', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'10', N'BOM', N'RptBOMCostingA4', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'10', N'BOM', N'TollIssueCostWithOthers', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (17, N'10', N'BOM', N'VAT1(TollIssue)WithRaw', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'10', N'BOM', N'VAT1Digit7', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'10', N'CompanyCode', N'Code', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'10', N'DatabaseName', N'DatabaseName', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (21, N'10', N'Import', N'SaleExistContinue', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (22, N'10', N'ImportPurchase', N'CalculativeAV', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'10', N'ImportPurchase', N'FixedATV', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'10', N'ImportPurchase', N'FixedCD', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'10', N'ImportPurchase', N'FixedCnF', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'10', N'ImportPurchase', N'FixedInsurance', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'10', N'ImportPurchase', N'FixedOthers', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'10', N'ImportPurchase', N'FixedRD', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'10', N'ImportPurchase', N'FixedSD', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'10', N'ImportPurchase', N'FixedTVA', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'10', N'ImportPurchase', N'FixedTVB', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'10', N'ImportPurchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'10', N'ImportTender', N'CustomerGroup', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'10', N'Issue', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (35, N'10', N'Issue', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (36, N'10', N'Issue', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (37, N'10', N'IssueFromBOM', N'IssueAutoPost', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (38, N'10', N'IssueFromBOM', N'IssueFromBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (39, N'10', N'PrepaidVAT', N'PrepaidVAT', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (40, N'10', N'PriceDeclaration', N'LocalInVAT1', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (41, N'10', N'PriceDeclaration', N'LocalInVAT1Ka(Tarrif)', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (42, N'10', N'PriceDeclaration', N'TenderInVAT1', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (43, N'10', N'PriceDeclaration', N'TenderInVAT1(Tender)', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (44, N'10', N'PriceDeclaration', N'TenderPriceWithVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (45, N'10', N'Printer', N'DefaultPrinter', N' ', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (46, N'10', N'Printer', N'EmployeeSalary(BDE)', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (47, N'10', N'Printer', N'MaxNoOfPrint', N'10', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (48, N'10', N'Production', N'ChangeableNBRPrice', N'N', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (49, N'10', N'Production', N'ProductionWithoutBOM', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (50, N'10', N'Purchase', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (51, N'10', N'Purchase', N'FixedVAT', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (52, N'10', N'Purchase', N'ImportCostingIncludeATV', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (53, N'10', N'Purchase', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (54, N'10', N'Purchase', N'RateChangePromote', N'7.5', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (55, N'10', N'Purchase', N'TotalPrice', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (56, N'10', N'Purchase', N'TrackingWithSale', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (57, N'10', N'Purchase', N'TrackingWithSaleFIFO', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (58, N'10', N'Receive', N'Amount', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (59, N'10', N'Receive', N'CustomerWiseBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (60, N'10', N'Receive', N'PriceDeclarationForImport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (61, N'10', N'Receive', N'Quantity', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (62, N'10', N'Reports', N'VAT11', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (63, N'10', N'Sale', N'ATVRate', N'0.0', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (64, N'10', N'Sale', N'ChangeableNBRPrice', N'N', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (65, N'10', N'Sale', N'CommercialImporter', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (66, N'10', N'Sale', N'CreditWithoutTransaction', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (67, N'10', N'Sale', N'CustomerWiseBOM', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (68, N'10', N'Sale', N'DollerDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (69, N'10', N'Sale', N'ItemNature', N'ELECTRIC WIRE/CABLE', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (70, N'10', N'Sale', N'NegStockAllow', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (71, N'10', N'Sale', N'NumberOfItems', N'15', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (72, N'10', N'Sale', N'PackingInExport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (73, N'10', N'Sale', N'Page3Plyer', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (74, N'10', N'Sale', N'PriceDeclarationForImport', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (75, N'10', N'Sale', N'QuantityDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (76, N'10', N'Sale', N'RateDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (77, N'10', N'Sale', N'ReportNumberOfCopiesPrint', N'1', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (78, N'10', N'Sale', N'TakaDecimalPlace', N'4', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (79, N'10', N'Sale', N'TenderSaleFromBOM', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (80, N'10', N'Sale', N'VAT11A4', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (81, N'10', N'Sale', N'VAT11Legal', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (82, N'10', N'Sale', N'VAT11Letter', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (83, N'10', N'Sale', N'WareHouseRentPerQuantity', N'0.0', N'Decimal', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (84, N'10', N'TollItemReceive', N'AttachedWithVAT16', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (85, N'10', N'TollReceive', N'WithIssue', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (86, N'10', N'TrackingTrace', N'Tracking', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (87, N'10', N'TrackingTrace', N'TrackingHead_1', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (88, N'10', N'TrackingTrace', N'TrackingHead_2', N'-', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (89, N'10', N'TrackingTrace', N'TrackingNo', N'2', N'int', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (90, N'10', N'VAT16', N'AutoAdjustment', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (91, N'10', N'VAT17', N'AutoAdjustment', N'Y', N'string', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (92, N'10', N'VAT18', N'VAT18Digit4', N'N', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

INSERT [dbo].[SettingsRole] ([SettingId], [UserID], [SettingGroup], [SettingName], [SettingValue], [SettingType], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (93, N'10', N'VAT19', N'ExportInBDT', N'Y', N'bool', N'Y', N'admin', CAST(0x0000A95A01871AA8 AS DateTime), N'admin', CAST(0x0000A95A01871AA8 AS DateTime))

SET IDENTITY_INSERT [dbo].[SettingsRole] OFF

SET IDENTITY_INSERT [dbo].[UOMName] ON 


INSERT [dbo].[UOMName] ([UOMId], [UOMName], [UOMCode], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'Pcs', N'Pcs', N'-', N'Y', N'admin', CAST(0x0000A95A018611D0 AS DateTime), N'admin', CAST(0x0000A95A018611D0 AS DateTime))

SET IDENTITY_INSERT [dbo].[UOMName] OFF

INSERT [dbo].[UOMs] ([UOMId], [UOMFrom], [UOMTo], [Convertion], [CTypes], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [ActiveStatus]) VALUES (N'1', N'pcs', N'pcs', CAST(1.000000000 AS Decimal(25, 9)), N'-', N'admin', CAST(0x0000A95A01861B30 AS DateTime), N'admin', CAST(0x0000A95A01861B30 AS DateTime), N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(1 AS Numeric(18, 0)), N'10', N'1101', N'Y', N'Y', N'Setup/ItemInformation/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(2 AS Numeric(18, 0)), N'10', N'1102', N'Y', N'Y', N'Setup/ItemInformation/Product', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(3 AS Numeric(18, 0)), N'10', N'1103', N'Y', N'Y', N'Setup/ItemInformation/Overhead', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(4 AS Numeric(18, 0)), N'10', N'1201', N'Y', N'Y', N'Setup/Vedor/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(5 AS Numeric(18, 0)), N'10', N'1202', N'Y', N'Y', N'Setup/Vedor/Vendor', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(6 AS Numeric(18, 0)), N'10', N'1301', N'Y', N'Y', N'Setup/Customer/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(7 AS Numeric(18, 0)), N'10', N'1302', N'Y', N'Y', N'Setup/Customer/Customer', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(8 AS Numeric(18, 0)), N'10', N'1401', N'Y', N'Y', N'Setup/Bank/Bank', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(9 AS Numeric(18, 0)), N'10', N'1501', N'Y', N'Y', N'Setup/Vehicle/Vehicle', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(10 AS Numeric(18, 0)), N'10', N'1601', N'Y', N'Y', N'Setup/PriceDeclaration/VAT-1', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(11 AS Numeric(18, 0)), N'10', N'1602', N'Y', N'Y', N'Setup/PriceDeclaration/Service', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(12 AS Numeric(18, 0)), N'10', N'1603', N'Y', N'Y', N'Setup/PriceDeclaration/Tender', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(13 AS Numeric(18, 0)), N'10', N'1701', N'Y', N'Y', N'Setup/Company/Commpany', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(14 AS Numeric(18, 0)), N'10', N'1801', N'Y', N'Y', N'Setup/FiscalYear/FiscalYear', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(15 AS Numeric(18, 0)), N'10', N'1901', N'Y', N'Y', N'Setup/Configuration/Settings', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(16 AS Numeric(18, 0)), N'10', N'1902', N'Y', N'Y', N'Setup/Configuration/Prefix', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(17 AS Numeric(18, 0)), N'10', N'11001', N'Y', N'Y', N'Setup/Import/Import', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(18 AS Numeric(18, 0)), N'10', N'11101', N'Y', N'Y', N'Setup/Conversion/Conversion', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(19 AS Numeric(18, 0)), N'10', N'11201', N'Y', N'Y', N'Setup/Currency/Currency', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(20 AS Numeric(18, 0)), N'10', N'11202', N'Y', N'Y', N'Setup/Currency/Conversion', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(21 AS Numeric(18, 0)), N'10', N'11301', N'Y', N'Y', N'Setup/Banderol/Banderol', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(22 AS Numeric(18, 0)), N'10', N'11302', N'Y', N'Y', N'Setup/Banderol/Packaging', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(23 AS Numeric(18, 0)), N'10', N'11303', N'Y', N'Y', N'Setup/Banderol/Product', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(24 AS Numeric(18, 0)), N'10', N'2101', N'Y', N'Y', N'Purchase/Purchase/Local', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(25 AS Numeric(18, 0)), N'10', N'2102', N'Y', N'Y', N'Purchase/Purchase/Trading', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(26 AS Numeric(18, 0)), N'10', N'2103', N'Y', N'Y', N'Purchase/Purchase/Import', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(27 AS Numeric(18, 0)), N'10', N'2104', N'Y', N'Y', N'Purchase/Purchase/InputService', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(28 AS Numeric(18, 0)), N'10', N'2105', N'Y', N'Y', N'Purchase/Purchase/PurchaseReturn', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(29 AS Numeric(18, 0)), N'10', N'2106', N'Y', N'Y', N'Purchase/Purchase/Service Stock', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(30 AS Numeric(18, 0)), N'10', N'2107', N'Y', N'Y', N'Purchase/Purchase/Service Non Stock', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(31 AS Numeric(18, 0)), N'10', N'3101', N'Y', N'Y', N'Production/Issue/Issue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(32 AS Numeric(18, 0)), N'10', N'3102', N'Y', N'Y', N'Production/Issue/Return', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(33 AS Numeric(18, 0)), N'10', N'3201', N'Y', N'Y', N'Production/Receive/WIP', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(34 AS Numeric(18, 0)), N'10', N'3202', N'Y', N'Y', N'Production/Receive/FGReceive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(35 AS Numeric(18, 0)), N'10', N'3203', N'Y', N'Y', N'Production/Receive/Return', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(36 AS Numeric(18, 0)), N'10', N'3301', N'Y', N'Y', N'Production/Receive/Package', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(37 AS Numeric(18, 0)), N'10', N'4101', N'Y', N'Y', N'Sale/Sale/Local', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(38 AS Numeric(18, 0)), N'10', N'4102', N'Y', N'Y', N'Sale/Sale/Service Stock', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(39 AS Numeric(18, 0)), N'10', N'41021', N'Y', N'Y', N'Sale/Sale/Service Non Stock', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(40 AS Numeric(18, 0)), N'10', N'4103', N'Y', N'Y', N'Sale/Sale/Trading', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(41 AS Numeric(18, 0)), N'10', N'4104', N'Y', N'Y', N'Sale/Sale/Export', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(42 AS Numeric(18, 0)), N'10', N'4105', N'Y', N'Y', N'Sale/Sale/Tender', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(43 AS Numeric(18, 0)), N'10', N'4201', N'Y', N'Y', N'Sale/Transfer/Transfer', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(44 AS Numeric(18, 0)), N'10', N'5101', N'Y', N'Y', N'Deposit/Treasury/Treasury', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(45 AS Numeric(18, 0)), N'10', N'5201', N'Y', N'Y', N'Deposit/VDS/VDS', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(46 AS Numeric(18, 0)), N'10', N'5301', N'Y', N'Y', N'Deposit/SD/SD', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(47 AS Numeric(18, 0)), N'10', N'5401', N'Y', N'Y', N'Deposit/Reverse/Adjustment', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(48 AS Numeric(18, 0)), N'10', N'6101', N'Y', N'Y', N'Toll/Client/RawIssue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(49 AS Numeric(18, 0)), N'10', N'6102', N'Y', N'Y', N'Toll/Client/FGReceive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(50 AS Numeric(18, 0)), N'10', N'6201', N'Y', N'Y', N'Toll/Contractor/RawReceive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(51 AS Numeric(18, 0)), N'10', N'6202', N'Y', N'Y', N'Toll/Contractor/FGProduction', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(52 AS Numeric(18, 0)), N'10', N'6203', N'Y', N'Y', N'Toll/Contractor/FGIssue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(53 AS Numeric(18, 0)), N'10', N'7101', N'Y', N'Y', N'Adjustment/AdjustmentHead/Head', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(54 AS Numeric(18, 0)), N'10', N'7102', N'Y', N'Y', N'Adjustment/AdjustmentHead/Transaction', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(55 AS Numeric(18, 0)), N'10', N'7201', N'Y', N'Y', N'Adjustment/Purchase/DN', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(56 AS Numeric(18, 0)), N'10', N'7202', N'Y', N'Y', N'Adjustment/Purchase/CN', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(57 AS Numeric(18, 0)), N'10', N'7301', N'Y', N'Y', N'Adjustment/Sale/CN', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(58 AS Numeric(18, 0)), N'10', N'7302', N'Y', N'Y', N'Adjustment/Sale/DN', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(59 AS Numeric(18, 0)), N'10', N'7401', N'Y', N'Y', N'Adjustment/Dispose/26', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(60 AS Numeric(18, 0)), N'10', N'7402', N'Y', N'Y', N'Adjustment/Dispose/27', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(61 AS Numeric(18, 0)), N'10', N'7501', N'Y', N'Y', N'Adjustment/DDB/DDB', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(62 AS Numeric(18, 0)), N'10', N'8101', N'Y', N'Y', N'NBRReport/VAT1/BOM', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(63 AS Numeric(18, 0)), N'10', N'8201', N'Y', N'Y', N'NBRReport/VAT16/VAT16', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(64 AS Numeric(18, 0)), N'10', N'8301', N'Y', N'Y', N'NBRReport/VAT17/VAT17', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(65 AS Numeric(18, 0)), N'10', N'8401', N'Y', N'Y', N'NBRReport/VAT18/VAT18', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(66 AS Numeric(18, 0)), N'10', N'8501', N'Y', N'Y', N'NBRReport/VAT19/VAT19', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(67 AS Numeric(18, 0)), N'10', N'8601', N'Y', N'Y', N'NBRReport/SDReport/SDReport', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(68 AS Numeric(18, 0)), N'10', N'9101', N'Y', N'Y', N'MISReport/Purchase/Purchase', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(69 AS Numeric(18, 0)), N'10', N'9102', N'Y', N'Y', N'MISReport/Purchase/Trading', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(70 AS Numeric(18, 0)), N'10', N'9201', N'Y', N'Y', N'MISReport/Production/Issue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(71 AS Numeric(18, 0)), N'10', N'9202', N'Y', N'Y', N'MISReport/Production/IssueReturn', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(72 AS Numeric(18, 0)), N'10', N'9203', N'Y', N'Y', N'MISReport/Production/Receive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(73 AS Numeric(18, 0)), N'10', N'9204', N'Y', N'Y', N'MISReport/Production/InnerIssue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(74 AS Numeric(18, 0)), N'10', N'9301', N'Y', N'Y', N'MISReport/Toll/Issue', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(75 AS Numeric(18, 0)), N'10', N'9302', N'Y', N'Y', N'MISReport/Toll/Receive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(76 AS Numeric(18, 0)), N'10', N'9401', N'Y', N'Y', N'MISReport/Sale/Local', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(77 AS Numeric(18, 0)), N'10', N'9402', N'Y', N'Y', N'MISReport/Sale/Service', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(78 AS Numeric(18, 0)), N'10', N'9403', N'Y', N'Y', N'MISReport/Sale/Trading', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(79 AS Numeric(18, 0)), N'10', N'9404', N'Y', N'Y', N'MISReport/Sale/Export', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(80 AS Numeric(18, 0)), N'10', N'9501', N'Y', N'Y', N'MISReport/Stock/Stock', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(81 AS Numeric(18, 0)), N'10', N'9601', N'Y', N'Y', N'MISReport/Deposit/Deposit', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(82 AS Numeric(18, 0)), N'10', N'9701', N'Y', N'Y', N'MISReport/VAT16/VAT16', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(83 AS Numeric(18, 0)), N'10', N'9801', N'Y', N'Y', N'MISReport/VAT17/VAT17', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(84 AS Numeric(18, 0)), N'10', N'9901', N'Y', N'Y', N'MISReport/VAT18/VAT18', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(85 AS Numeric(18, 0)), N'10', N'91001', N'Y', N'Y', N'MISReport/SDDeposit/SDDeposit', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(86 AS Numeric(18, 0)), N'10', N'10101', N'Y', N'Y', N'SetupReport/Product/Type', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(87 AS Numeric(18, 0)), N'10', N'10102', N'Y', N'Y', N'SetupReport/Product/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(88 AS Numeric(18, 0)), N'10', N'10103', N'Y', N'Y', N'SetupReport/Product/Product', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(89 AS Numeric(18, 0)), N'10', N'10201', N'Y', N'Y', N'SetupReport/Customer/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(90 AS Numeric(18, 0)), N'10', N'10202', N'Y', N'Y', N'SetupReport/Customer/Customer', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(91 AS Numeric(18, 0)), N'10', N'10301', N'Y', N'Y', N'SetupReport/Vendor/Group', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(92 AS Numeric(18, 0)), N'10', N'10302', N'Y', N'Y', N'SetupReport/Vendor/Vendor', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(93 AS Numeric(18, 0)), N'10', N'10401', N'Y', N'Y', N'SetupReport/Bank/Bank', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(94 AS Numeric(18, 0)), N'10', N'10501', N'Y', N'Y', N'SetupReport/Vehicle/Vehicle', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(95 AS Numeric(18, 0)), N'10', N'20101', N'Y', N'Y', N'UserAccount/NewAccount/NewAccount', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(96 AS Numeric(18, 0)), N'10', N'20201', N'Y', N'Y', N'UserAccount/PasswordChange/PasswordChange', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(97 AS Numeric(18, 0)), N'10', N'43', N'Y', N'Y', N'UserAccount/UserRole/UserRole', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(98 AS Numeric(18, 0)), N'10', N'44', N'Y', N'Y', N'UserAccount/SettingsRole/SettingsRole', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(99 AS Numeric(18, 0)), N'10', N'33101', N'Y', N'Y', N'Banderol/Demand/Demand', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[UserRolls] ([LineID], [UserID], [FormID], [Access], [PostAccess], [FormName], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [AddAccess], [EditAccess]) VALUES (CAST(100 AS Numeric(18, 0)), N'10', N'33201', N'Y', N'Y', N'Banderol/Receive/Receive', N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'admin', CAST(0x0000A95A0187A874 AS DateTime), N'Y', N'Y')

INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5], [DriverName]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', NULL)

INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5], [DriverName]) VALUES (N'1', N'1', N'Truck', N'123', N'-', N'-', N'Y', N'admin', CAST(0x0000A95A0187854C AS DateTime), N'admin', CAST(0x0000A95A0187854C AS DateTime), NULL, NULL, NULL, NULL, NULL, N'')


                ";
                #endregion TableDefaultData Back


                top2 = "go";

                IEnumerable<string> commandStringsDefaultData = Regex.Split(sqlText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStringsDefaultData)
                {
                    if (commandString.Trim() != "")
                    {
                        SqlCommand cmdIdExist1 = new SqlCommand(commandString, currConn);

                        cmdIdExist1.Transaction = transaction;
                        transResult = (int)cmdIdExist1.ExecuteNonQuery();
                        if (transResult < 0)
                        {
                            throw new ArgumentNullException("Insert Default Data to Database'" + databaseName + "'", MessageVM.dbMsgTableDefaultData);
                        }
                    }
                }

                #endregion TableCreate

                #region Insert Company Profile

                //string NewCompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string tom = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                string jary = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyLegalName);
                string miki = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.VatRegistrationNo);
                //string mouse = Converter.DESEncrypt(PassPhrase, EnKey, GetHardwareID());
                string mouse = Converter.DESEncrypt(PassPhrase, EnKey, GetServerHardwareId());


                sqlText = "";
                sqlText += " insert into CompanyProfiles(";
                sqlText += " CompanyID,";
                sqlText += " CompanyName,";
                sqlText += " CompanyLegalName,";
                sqlText += " Address1,";
                sqlText += " Address2,";
                sqlText += " Address3,";
                sqlText += " City,";
                sqlText += " ZipCode,";
                sqlText += " TelephoneNo,";
                sqlText += " FaxNo,";
                sqlText += " Email,";
                sqlText += " ContactPerson,";
                sqlText += " ContactPersonDesignation,";
                sqlText += " ContactPersonTelephone,";
                sqlText += " ContactPersonEmail,";
                sqlText += " TINNo,";
                sqlText += " VatRegistrationNo,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " StartDateTime,";
                sqlText += " FYearStart,";
                sqlText += " FYearEnd,";
                sqlText += " Tom,";
                sqlText += " Jary,";
                sqlText += " Miki,";
                sqlText += " Mouse)";

                sqlText += " values(";
                sqlText += "@companyProfilesCompanyID ,";
                sqlText += "@companyProfilesCompanyName ,";
                sqlText += "@companyProfilesCompanyLegalName ,";
                sqlText += "@companyProfilesAddress1 ,";
                sqlText += "@companyProfilesAddress2 ,";
                sqlText += "@companyProfilesAddress3 ,";
                sqlText += "@companyProfilesCity ,";
                sqlText += "@companyProfilesZipCode ,";
                sqlText += "@companyProfilesTelephoneNo ,";
                sqlText += "@companyProfilesFaxNo ,";
                sqlText += "@companyProfilesEmail ,";
                sqlText += "@companyProfilesContactPerson ,";
                sqlText += "@companyProfilesContactPersonDesignation ,";
                sqlText += "@companyProfilesContactPersonTelephone ,";
                sqlText += "@companyProfilesContactPersonEmail ,";
                sqlText += "@companyProfilesTINNo ,";
                sqlText += "@companyProfilesVatRegistrationNo ,";
                sqlText += "@companyProfilesComments ,";
                sqlText += "@companyProfilesActiveStatus ,";
                sqlText += "'SuperAdmin' ,";
                sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,";
                sqlText += "'SuperAdmin' ,";
                sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,";
                sqlText += "@companyProfilesStartDateTime ,";
                sqlText += "@companyProfilesFYearStart ,";
                sqlText += "@companyProfilesFYearEnd , ";
                sqlText += "@tom , ";
                sqlText += "@jary , ";
                sqlText += "@miki , ";
                sqlText += "@mouse ";
                sqlText += " )";

                //try
                //{


                SqlCommand cmdCompanyProfile = new SqlCommand(sqlText, currConn);
                cmdCompanyProfile.Transaction = transaction;

                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyID", companyProfiles.CompanyID);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyName", companyProfiles.CompanyName);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCompanyLegalName", companyProfiles.CompanyLegalName);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress1", companyProfiles.Address1);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress2", companyProfiles.Address2);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesAddress3", companyProfiles.Address3);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesCity", companyProfiles.City);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesZipCode", companyProfiles.ZipCode);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesTelephoneNo", companyProfiles.TelephoneNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFaxNo", companyProfiles.FaxNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesEmail", companyProfiles.Email);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPerson", companyProfiles.ContactPerson);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonDesignation", companyProfiles.ContactPersonDesignation);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonTelephone", companyProfiles.ContactPersonTelephone);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesContactPersonEmail", companyProfiles.ContactPersonEmail);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesTINNo", companyProfiles.TINNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesVatRegistrationNo", companyProfiles.VatRegistrationNo);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesComments", companyProfiles.Comments);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesActiveStatus", companyProfiles.ActiveStatus);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesStartDateTime", companyProfiles.StartDateTime);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFYearStart", companyProfiles.FYearStart);
                cmdCompanyProfile.Parameters.AddWithValue("@companyProfilesFYearEnd", companyProfiles.FYearEnd);
                cmdCompanyProfile.Parameters.AddWithValue("@tom", tom);
                cmdCompanyProfile.Parameters.AddWithValue("@jary", jary);
                cmdCompanyProfile.Parameters.AddWithValue("@miki", miki);
                cmdCompanyProfile.Parameters.AddWithValue("@mouse", mouse);

                transResult = (int)cmdCompanyProfile.ExecuteNonQuery();
                if (transResult < 0)
                {

                    throw new ArgumentNullException("Insert company Profile data to Database('" + databaseName + "')", MessageVM.dbMsgCompanyInformationNotSave);
                }
                newID = companyProfiles.CompanyID;


                #endregion Insert Company Profile

                #region Insert Fiscal Year
                foreach (var Item in fiscalDetails.ToList())
                {

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into FiscalYear(";
                    sqlText += " FiscalYearName,";
                    sqlText += " CurrentYear,";
                    sqlText += " PeriodID,";
                    sqlText += " PeriodName,";
                    sqlText += " PeriodStart,";
                    sqlText += " PeriodEnd,";
                    sqlText += " PeriodLock,";
                    sqlText += " GLLock,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@ItemFiscalYearName,";
                    sqlText += "@ItemCurrentYear,";
                    sqlText += "@ItemPeriodID,";
                    sqlText += "@ItemPeriodName,";
                    sqlText += "@ItemPeriodStart,";
                    sqlText += "@ItemPeriodEnd,";
                    sqlText += "@ItemPeriodLock,";
                    sqlText += "@ItemGLLock,";
                    sqlText += "'SuperAdmin',";
                    sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sqlText += "'SuperAdmin',";
                    sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@ItemFiscalYearName", Item.FiscalYearName);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCurrentYear", Item.CurrentYear);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodName", Item.PeriodName);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodStart", Item.PeriodStart);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodEnd", Item.PeriodEnd);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemGLLock", Item.GLLock);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Insert Fiscal Year data to Database('" + databaseName + "')", MessageVM.dbMsgCFiscalYearNotSave);
                    }
                    #endregion Insert only DetailTable
                }



                #endregion Insert Fiscal Year

                #region Insert Sys DB Information

                string CompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string CompanyName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                string DatabaseName = Converter.DESEncrypt(PassPhrase, EnKey, databaseName);
                string ActiveStatus = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.ActiveStatus);
                //string CompanyLegalName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyLegalName);
                sqlText = "";
                sqlText += " INSERT INTO CompanyInformations (CompanyID,CompanyName,DatabaseName,ActiveStatus,Serial)";
                sqlText += " VALUES(" +
                           "@CompanyID," +
                           "@CompanyName," +
                           "@DatabaseName," +
                           "@ActiveStatus," +
                    //"'" + CompanyLegalName + "'," +
                           "(select isnull(max(Serial ),0)+1 FROM  CompanyInformations)" +

                           ")";
                currConn.ChangeDatabase("SymphonyVATSys");
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@CompanyID", CompanyID);
                cmdPrefetch.Parameters.AddWithValue("@CompanyName", CompanyName);
                cmdPrefetch.Parameters.AddWithValue("@DatabaseName", DatabaseName);
                cmdPrefetch.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();
                if (transResult < 0)
                {
                    throw new ArgumentNullException("Insert Company List Information", MessageVM.dbMsgDBInfoInsert);
                }
                #endregion Insert Sys DB Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        #region SuccessResult

                        retResults[0] = "Success";
                        retResults[1] = "Requested Company Created successfully.";
                        retResults[2] = newID;
                        #endregion SuccessResult

                    }

                }

                #endregion Commit
            }
            #endregion Try

            #region Catch and Finall
            catch (ArgumentNullException arg)
            {
                if (arg.ParamName.ToLower() != "deletedb")
                {
                    currConn.Close();
                    currConn.Open();
                    currConn.ChangeDatabase("master");
                    #region check Database and delete
                    sqlText = "";
                    sqlText += " USE [master]";
                    sqlText += " drop DATABASE @databaseName ";

                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;

                    cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                    cmdIdExist.ExecuteNonQuery();
                    #endregion check Database
                }

                throw arg;
            }
            catch (SqlException sqlex)
            {

                currConn.Close();
                currConn.Open();
                currConn.ChangeDatabase("master");


                #region check Database and delete


                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " drop DATABASE @databaseName";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                cmdIdExist.ExecuteNonQuery();


                #endregion check Database

                throw sqlex;
            }

            catch (Exception ex)
            {


                currConn.Close();
                currConn.Open();
                currConn.ChangeDatabase("master");
                #region check Database and delete


                sqlText = "";
                sqlText += " USE [master]";
                sqlText += " drop DATABASE @databaseName ";

                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@databaseName", databaseName);

                cmdIdExist.ExecuteNonQuery();

                #endregion check Database

                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result
        }

        #region Old Methods

        public static string CompanyListUpdate1(SqlCommand objCommCustomerGroup, string CompanyID, string CompanyName, string DatabaseName, string ActiveStatus)
        {
            string result = "-1";

            string strSQL = @"
declare @Present numeric;

select @Present = count(CompanyID) from CompanyInformations 
where (CompanyID=@CompanyID)


if(@Present <=0 )
BEGIN

INSERT INTO CompanyInformations
(CompanyID,CompanyName,DatabaseName,ActiveStatus)
VALUES(@CompanyID,@CompanyName,@DatabaseName,@ActiveStatus)

end
else
begin
update CompanyInformations set 

CompanyName=@CompanyName,
ActiveStatus=@ActiveStatus

where (CompanyID=@CompanyID)
;
end


";
            objCommCustomerGroup.CommandText = strSQL;
            objCommCustomerGroup.CommandType = CommandType.Text;

            if (!objCommCustomerGroup.Parameters.Contains("@CompanyID"))
            { objCommCustomerGroup.Parameters.AddWithValue("@CompanyID", CompanyID); }
            else { objCommCustomerGroup.Parameters["@CompanyID"].Value = CompanyID; }
            if (!objCommCustomerGroup.Parameters.Contains("@CompanyName"))
            { objCommCustomerGroup.Parameters.AddWithValue("@CompanyName", CompanyName); }
            else { objCommCustomerGroup.Parameters["@CompanyName"].Value = CompanyName; }

            if (!objCommCustomerGroup.Parameters.Contains("@DatabaseName"))
            { objCommCustomerGroup.Parameters.AddWithValue("@DatabaseName", DatabaseName); }
            else { objCommCustomerGroup.Parameters["@DatabaseName"].Value = DatabaseName; }

            if (!objCommCustomerGroup.Parameters.Contains("@ActiveStatus"))
            { objCommCustomerGroup.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
            else { objCommCustomerGroup.Parameters["@ActiveStatus"].Value = ActiveStatus; }


            try
            {
                result = objCommCustomerGroup.ExecuteNonQuery().ToString();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return "-99";
                }
                else if (ex.Number == 266)
                {
                    return "-266";
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
            finally { }

            return result;

        }

        public string[] SuperAdministratorUpdate(string miki, string mouse)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(miki))
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate", "unable to find Username");
                }
                else if (string.IsNullOrEmpty(mouse))
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate",
                                                    "unable to find password");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("SA");

                #endregion open connection and transaction




                #region update existing row to table

                if (miki == "ADMINISTRATOR")
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(miki) from SuperAdministrator ";
                    sqlText += " where miki='zTvrNxNvP08='";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        sqlText = "";
                        sqlText += " INSERT INTO SuperAdministrator(	miki,	mouse) VALUES('zTvrNxNvP08=','" + mouse +
                                   "')";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                                            "Super Administrator information not Updated");
                        }
                    }
                    else
                    {
                        #region sql statement

                        sqlText = "";
                        sqlText += " update SuperAdministrator set mouse='" + mouse + "'";
                        sqlText += " where miki='zTvrNxNvP08='";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                                            "Super Administrator information not Updated");
                        }

                        #endregion
                    }

                    #endregion Find Transaction Exist
                }
                else if (miki == "SYMPHONY")
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(miki) from SuperAdministrator ";
                    sqlText += " where miki='hV9vFF0OUsptxqpZlnEhrA=='";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        sqlText = "";
                        sqlText += " INSERT INTO SuperAdministrator(	miki,	mouse) VALUES('hV9vFF0OUsptxqpZlnEhrA==','" + mouse +
                                   "')";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                                            "Super Administrator information not Updated");
                        }
                    }
                    else
                    {
                        #region sql statement

                        sqlText = "";
                        sqlText += " update SuperAdministrator set mouse='" + mouse + "'";
                        sqlText += " where miki='hV9vFF0OUsptxqpZlnEhrA=='";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                                            "Super Administrator information not Updated");
                        }

                        #endregion
                    }

                    #endregion Find Transaction Exist
                }

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Requested Information";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Requested Informatioe";

                }

                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }

            }
            #endregion

            return retResults;
        }
        public string[] SuperAdministratorUpdatebACKUP(string mouse)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(mouse))
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate",
                                                    "unable to find password");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("SA");

                #endregion open connection and transaction




                #region update existing row to table
                #region Find Transaction Exist




                #region sql statement

                sqlText = "";
                sqlText += " update SuperAdministrator set mouse='" + mouse + "'";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate", "Super Administrator information not Updated");
                }
                #endregion

                #endregion Find Transaction Exist


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Requested Information";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Requested Informatioe";

                }

                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }

            }
            #endregion

            return retResults;
        }

        public string[] DatabaseInformationUpdate(string Tom, string jary, string mini)
        {

            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(Tom))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate", "unable to find Username");
                }
                else if (string.IsNullOrEmpty(jary))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate",
                                                    "unable to find password");
                }
                else if (string.IsNullOrEmpty(mini))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate", "unable to find Source");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("SA");

                #endregion open connection and transaction

                #region update existing row to table
                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(Tom) from DBInformation ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                int IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist <= 0)
                {
                    sqlText = "";
                    sqlText += " INSERT INTO DBInformation(	Tom,	jary,mini) VALUES('" + Tom + "','" + jary + "','" + mini + "')";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DatabaseInformationUpdate", "Database Information not update");
                    }
                }
                else
                {
                    #region sql statement

                    sqlText = "";
                    sqlText += " update DBInformation set Tom= '" + Tom + "',jary='" + jary + "',mini='" + mini + "'";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DatabaseInformationUpdate", "Database Information not update");
                    }
                    #endregion
                }
                #endregion Find Transaction Exist


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Requested Information";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Requested Informatioe";

                }

                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }

            }
            #endregion

            return retResults;
        }


        public bool UpdateSystemData(string userName, string password, string source)
        {
            bool success = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";


            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(""))
                //{
                //    throw new ArgumentNullException("InsertToBankInformation",
                //                "Could not find requested Bank Id.");
                //}
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("sysdb");
                #endregion open connection and transaction


                sqlText = "delete from DBInformation";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objDel = cmdExist.ExecuteScalar();

                sqlText = "";
                sqlText += "insert into DBInformation";
                sqlText += "(";
                sqlText += "Tom,";
                sqlText += "jary,";
                sqlText += "mini";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@userName,";
                sqlText += "@password,";
                sqlText += "@source";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@userName", userName);
                cmdInsert.Parameters.AddWithValue("@password", password);
                cmdInsert.Parameters.AddWithValue("@source", source);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {

                        currConn.Close();

                    }
                }

            }

            return success;
        }
        #endregion

        public string TransactionCode(string CodeGroup, string CodeName, string tableName,
            string tableIdField, string tableDateField, string tranDate, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            string Prefetch = "";
            int CurrentID = 0;
            int SetupLen = 0;
            string newID = "";
            string n = "";
            int Len = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(CodeGroup))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(CodeName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                else if (string.IsNullOrEmpty(tableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }
                else if (string.IsNullOrEmpty(tableIdField))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("TransactionCode", "Transaction Date not Valid");

                }
                #endregion Validation
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction
                #region Prefetch

                sqlText = "";
                sqlText = sqlText + " SELECT     prefix FROM Codes";
                sqlText = sqlText + " WHERE     (CodeGroup = @CodeGroup) AND (CodeName =@CodeName )";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                cmdPrefetch.Parameters.AddWithValue("@CodeName", CodeName);

                Prefetch = (string)cmdPrefetch.ExecuteScalar();
                if (string.IsNullOrEmpty(Prefetch))
                {
                    throw new ArgumentNullException("TransactionCode", "Could not find prefix.");
                }

                #endregion Prefetch

                #region F year Start Date
                var tranYear = Convert.ToDateTime(tranDate).ToString("yyyy-MM-dd");
                sqlText = "";
                sqlText = sqlText + " SELECT MIN(fy1.PeriodStart) FROM FiscalYear fy1 WHERE fy1.CurrentYear= (";
                ////sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE convert (date, '" + tranDate + "',101)";
                //sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE CONVERT(date,'" + tranDate + "', 101)";
                sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE @tranYear";
                sqlText = sqlText + " between fy.PeriodStart AND fy.PeriodEnd) ";
                SqlCommand cmdFsDate = new SqlCommand(sqlText, currConn);
                cmdFsDate.Transaction = transaction;

                cmdFsDate.Parameters.AddWithValue("@tranYear", tranYear);

                object objFsDate = cmdFsDate.ExecuteScalar();
                //DateTime FsDate = Convert.ToDateTime(objFsDate);
                var FsDate = Convert.ToDateTime(objFsDate).ToString("yyyy-MM-dd HH:mm:ss");
                if (objFsDate == null || FsDate == null)
                {
                    throw new ArgumentNullException("TransactionCode", "Fyscal year Stardate not found");
                }

                #endregion F year Start Date

                #region F year End Date

                sqlText = "";
                sqlText = sqlText + " SELECT  MAX(fy1.PeriodEnd) FROM FiscalYear fy1 WHERE fy1.CurrentYear= (";
                ////sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE convert (date, '" + tranDate + "',101)";
                sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE @tranYear";
                sqlText = sqlText + " between fy.PeriodStart AND fy.PeriodEnd) ";

                SqlCommand cmdFeDate = new SqlCommand(sqlText, currConn);
                cmdFeDate.Transaction = transaction;

                cmdFeDate.Parameters.AddWithValue("@tranYear", tranYear);

                object objFeDate = cmdFeDate.ExecuteScalar();
                var FeDate = Convert.ToDateTime(objFeDate).ToString("yyyy-MM-dd HH:mm:ss");

                if (FeDate == null || objFeDate == null)
                {
                    throw new ArgumentNullException("TransactionCode", "Fyscal year Enddate not found");
                }

                #endregion F year End Date

                #region CurrentID

                sqlText = "";
                sqlText += "  SELECT isnull(max(cast(SUBSTRING ( ih." + tableIdField + " ,5 , LEN(ih." + tableIdField + ")-9 ) AS INT)),0)+1";
                sqlText += " FROM " + tableName + " ih";
                sqlText += " WHERE SUBSTRING ( ih." + tableIdField + " ,1 , 3 )='" + Prefetch + "'";
                sqlText +=
                    " AND ih." + tableDateField + " >= '" + FsDate + "' and ih." + tableDateField + " <DATEADD(d,1,'" + FeDate + "' )";
                sqlText += " AND ih." + tableDateField + " BETWEEN '" + FsDate + "' and DATEADD(d,1,'" + FeDate + "' ) ";



                SqlCommand cmdCurrentID = new SqlCommand(sqlText, currConn);
                cmdCurrentID.Transaction = transaction;
                CurrentID = Convert.ToInt32(cmdCurrentID.ExecuteScalar());

                if (CurrentID < 0)
                {

                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgUnableCreatID);
                }

                #endregion CurrentID
                #region SetupLen

                sqlText = "";
                sqlText = sqlText + " SELECT     Lenth FROM Codes";
                sqlText = sqlText + " WHERE     (CodeGroup = @CodeGroup) AND (CodeName = @CodeName)";
                SqlCommand cmdSetupLen = new SqlCommand(sqlText, currConn);
                cmdSetupLen.Transaction = transaction;

                cmdSetupLen.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                cmdSetupLen.Parameters.AddWithValue("@CodeName", CodeName);

                SetupLen = Convert.ToInt32(cmdSetupLen.ExecuteScalar());
                if (SetupLen < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgUnableCreatID);
                }

                #endregion SetupLen
                #region ID Create

                n = "";
                Len = Convert.ToString(CurrentID).Length;
                for (int i = 0; i < SetupLen - Len; i++)
                {
                    n = n + "0";
                }
                var idYear = Convert.ToDateTime(tranDate);
                //newID = Prefetch + "-" + n + Convert.ToString(CurrentID) + "/" +
                //        Convert.ToString(tranDate.ToString("MMyy"));

                newID = Prefetch + "-" + n + Convert.ToString(CurrentID) + "/" +
                       Convert.ToString(idYear.ToString("MMyy"));

                if (string.IsNullOrEmpty(newID))
                {
                    throw new ArgumentNullException("TransactionCodeGenerator", "Unable to Create ID");
                }
                #endregion ID Create

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion Catch and Finall
            #region Results

            return newID;

            #endregion

        }
        public void DatabaseTableChanges()
        {
            CommonDAL commonDal = new CommonDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);

                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "WareHouseRent", "decimal(25, 9)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "WareHouseVAT", "decimal(25, 9)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "ATVRate", "decimal(25, 9)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "ATVablePrice", "decimal(25, 9)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "ATVAmount", "decimal(25, 9)", currConn, transaction);

                transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "ValueOnly", "varchar(1)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "ValueOnly", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Deposits", "VATCircle", "varchar(20)", currConn, transaction);

                #region TableAdd
                #region 20131231 (yyMMdd)
                //transResult = commonDal.TableAdd("Costing", "Id", "int", currConn, transaction); //tablename,fieldName, datatype
                //transResult = commonDal.TableAdd("CustomersAddress", "Id", "int", currConn, transaction); //tablename,fieldName, datatype
                #endregion 20131231 (yyMMdd)

                #endregion TableAdd
                #region FieldAdd


                transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "CustomHouse", "varchar(500)", currConn, transaction);

                transResult = commonDal.TableFieldAdd("Products", "TollProduct", "varchar(1)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("ReceiveHeaders", "WithToll", "varchar(1)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "LCBank", "varchar(200)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "LCDate", "datetime", currConn, transaction);

                transResult = commonDal.TableFieldAdd("CustomersAddress", "CustomerID", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("CustomersAddress", "CustomerVATRegNo", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("CustomersAddress", "CustomerAddress", "varchar(500)", currConn, transaction);

                transResult = commonDal.TableFieldAdd("BOMs", "CustomerID", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("BOMRaws", "IssueOnProduction", "varchar(1)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("BOMRaws", "CustomerID", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("BOMCompanyOverhead", "CustomerID", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("ReceiveHeaders", "CustomerID", "varchar(20)", currConn, transaction);
                transResult = commonDal.TableFieldAdd("VDS", "IsPurchase", "varchar(20)", currConn, transaction);

                #region 20131231 (yyMMdd)
                //transResult = commonDal.TableFieldAdd("Costing", "Id", "int", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "ItemNo", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "InputDate", "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "Quantity", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "UnitCost", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "AV", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "CD", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "RD", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "TVB", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "SDAmount", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "VATAmount", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "TVA", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "ATV", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "Other", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "CostPrice", "decimal(25, 9)", currConn, transaction);//Total
                //transResult = commonDal.TableFieldAdd("Costing", "CreatedBy", "varchar(120)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "CreatedOn", "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "LastModifiedBy", "varchar(120)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "LastModifiedOn", "datetime", currConn, transaction);

                //transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "ImportIDExcel", "varchar(30)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "AlReadyPrint", "int", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("ReceiveHeaders", "ImportIDExcel", "varchar(30)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("IssueHeaders", "ImportIDExcel", "varchar(30)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "ImportIDExcel", "varchar(30)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("ReceiveHeaders", "ReferenceNo", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "SerialNo1", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("BOMRaws", "PInvoiceNo", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("IssueDetails", "IsProcess", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DisposeDetails", "FromStock", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "FinishItemNo", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("ReceiveDetails", "CurrencyValue", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("ReceiveDetails", "DollerValue", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DisposeDetails", "DollarPrice", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Currencies", "CurrencyMajor", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Currencies", "CurrencyMinor", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Currencies", "CurrencySymbol", "varchar(50)", currConn, transaction);
                #endregion 20131231 (yyMMdd)
                #region 20140102 (yyMMdd)
                //transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "CConversionDate", "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("CompanyProfiles", "Tom", "varchar(200)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("CompanyProfiles", "Jary", "varchar(200)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("CompanyProfiles", "Miki", "varchar(200)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("CompanyProfiles", "Mouse", "varchar(200)", currConn, transaction);
                #endregion 20140102 (yyMMdd)

                #region 20140312
                //transResult = commonDal.TableFieldAdd("VDS", "IsPercent", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "BENumber", "varchar(200)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "RefNo", "varchar(200)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Costing", "SD", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("UserRolls", "AddAccess", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("UserRolls", "EditAccess", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceDetails", "ReturnTransactionType", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "ReturnTransactionType", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("ReceiveDetails", "ReturnTransactionType", "varchar(50)", currConn, transaction);


                #endregion
                #region 20141117
                //transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "DeliveryChallanNo", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "IsGatePass", "varchar(3)", currConn, transaction);
                #endregion
                #region 20150101
                //transResult = commonDal.TableFieldAdd("Deposits", "ReverseDepositId", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("SDDeposits", "ReverseDepositId", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("VDS", "ReverseVDSId", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("AdjustmentHistorys", "ReverseAdjHistoryNo", "varchar(20)", currConn, transaction);
                #endregion
                #region 20150101
                //transResult = commonDal.TableFieldAdd("BOMRaws", "TransactionType", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("TenderHeaders", "CustomerGroupID", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "LCNumber", "varchar(50)", currConn, transaction);

	            #endregion 20150101
                #region 20150806 juwel
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "LCDate",  "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "LandedCost", "decimal(25, 9)", currConn, transaction);
                #endregion
                #region 20151001, Sanofi
                //transResult = commonDal.TableFieldAdd("SalesInvoiceHeaders", "CompInvoiceNo",  "varchar(50)", currConn, transaction);
                #endregion
                #region 20151109, Only for CP
                //transResult = commonDal.TableFieldAdd("SalesInvoiceDetails", "Weight", "varchar(120)", currConn, transaction);
                #endregion
                #endregion FieldAdd
                #region KeyDelete
                #region 20131231 (yyMMdd)
                //transResult = DeleteForeignKey("ProductCategories", "FK_ProductCategories_ProductTypes", currConn, transaction);
                #endregion 20131231 (yyMMdd)
                #endregion KeyDelete
                #region Data type alter
                #region 20131231 (yyMMdd)
                //TableFieldAlter("SalesInvoiceHeadersExport", "Quantity", "varchar(120)", currConn, transaction);
                //TableFieldAlter("SalesInvoiceHeadersExport", "GrossWeight", "varchar(120)", currConn, transaction);
                //TableFieldAlter("SalesInvoiceHeadersExport", "NetWeight", "varchar(120)", currConn, transaction);
                #endregion 20131231 (yyMMdd)
                #region 20140907
                //TableFieldAlter("SalesInvoiceDetails", "Type", "varchar(50)", currConn, transaction);
                #endregion 20140907
                #endregion Data type alter
                #region Update data
                #region 20140101 (yyMMdd)
                //ExecuteUpdateQuery(CurrencyUpdateSQL, currConn, transaction);
                #endregion 20140101 (yyMMdd)
                #region 20140201
                //ExecuteUpdateQuery(CurrencyConvertionUpdateSql, currConn, transaction);
                #endregion 20140201
                #endregion Update data
                #region OldDBUpdate
                //#region TableAdd
                //commonDal.TableAdd("SDDeposits", "DepositId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableAdd("DutyDrawBackHeader", "DDBackNo", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableAdd("DutyDrawBackDetails", "DDBackNo", "varchar(20)", currConn); //tablename,fieldName, datatype

                //#endregion TableAdd
                //#region FieldAdd


                //commonDal.TableFieldAdd("BOMCompanyOverhead", "HeadID", "varchar(20)", currConn);

                //commonDal.TableFieldAdd("SalesInvoiceHeaders", "ImportIDExcel", "varchar(30)", currConn);
                //commonDal.TableFieldAdd("ReceiveHeaders", "ImportIDExcel", "varchar(30)", currConn);
                //commonDal.TableFieldAdd("IssueHeaders", "ImportIDExcel", "varchar(30)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceHeaders", "ImportIDExcel", "varchar(30)", currConn);

                //commonDal.TableFieldAdd("SalesInvoiceDetails", "PromotionalQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositId", "varchar(20)", currConn);

                //commonDal.TableFieldAdd("Customers", "BusinessType", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("Customers", "BusinessCode", "varchar(20)", currConn);

                //commonDal.TableFieldAdd("Vendors", "BusinessType", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("Vendors", "BusinessCode", "varchar(20)", currConn);


                //commonDal.TableFieldAdd("SDDeposits", "TreasuryNo", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositDateTime", "datetime", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositType", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "ChequeNo", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "ChequeBank", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "ChequeBankBranch", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "ChequeDate", "datetime", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "BankID", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "TreasuryCopy", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositPerson", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "DepositPersonDesignation", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "Comments", "varchar(200)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "CreatedBy", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "CreatedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "LastModifiedBy", "varchar(120)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "LastModifiedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "TransactionType", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("SDDeposits", "Post", "varchar(1)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "DDBackNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "DDBackDate", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "SalesInvoiceNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "SalesDate", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "CustormerID", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "CurrencyId", "int", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "ExpCurrency", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "BDTCurrency", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "FgItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalDDBack", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimCnFAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimInsuranceAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimTVBAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimTVAAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimATVAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "TotalClaimOthersAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "Comments", "varchar(250)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "CreatedBy", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "CreatedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "LastModifiedBy", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "LastModifiedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackHeader", "Post", "varchar(1)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "DDBackNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "DDBackDate", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "DDLineNo", "int", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "SalesInvoiceNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "PurchaseInvoiceNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "PurchaseDate", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "FgItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "FgQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "BillOfEntry", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "PurchaseUom", "varchar(10)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "PurchaseQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UnitPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "AV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "CD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "RD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "VAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "CnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "Insurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "TVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "TVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "Others", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UseQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimCnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimInsurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimTVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimTVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "ClaimOthers", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "SubTotalDDB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMn", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMCnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMInsurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMTVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMTVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMOthers", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "UOMSubTotalDDB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "Post", "varchar(1)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "CreatedBy", "varchar(200)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "CreatedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "LastModifiedBy", "varchar(50)", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "LastModifiedOn", "datetime", currConn);
                //commonDal.TableFieldAdd("DutyDrawBackDetails", "PurchasetransactionType", "varchar(50)", currConn);


                //commonDal.TableFieldAdd("ReceiveDetails", "BOMId", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("ReceiveDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("ReceiveDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("ReceiveDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("ReceiveDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype

                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "ItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantityPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "LineCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "Quantity", "decimal(25, 9)", currConn);

                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);

                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);

                //commonDal.TableFieldAdd("TenderDetails", "BOMId", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);
                //#endregion FieldAdd

                //#region AlterFields

                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CnFRate", "decimal(25, 9)", currConn);

                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjInputAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjInputPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjVATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjSDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("AdjustmentHistorys", "AdjOtherAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMCompanyOverhead", "HeadAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMCompanyOverhead", "RebatePercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMCompanyOverhead", "RebateAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMCompanyOverhead", "AdditionalCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UseQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "WastageQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "Cost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "TradingMarkUp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "RebateRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "MarkUpValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UOMPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UOMUQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "UOMWQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMRaws", "TotalQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "TradingMarkUp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "RawTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "PackingTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "RebateTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "AdditionalTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "RebateAdditionTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "PacketPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "RawOHCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "LastNBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "LastNBRWithSDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "TotalQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "WholeSalePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "NBRWithSDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "MarkUpValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "LastMarkUpValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "LastSDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("BOMs", "LastAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Customers", "VDSPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Deposits", "DepositAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "RealPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "PresentPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeDetails", "QuantityImport", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "ImportVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "TotalPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "TotalPriceImport", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "AppVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "AppTotalPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "AppVATAmountImport", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DisposeHeaders", "AppTotalPriceImport", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "FgQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "PurchaseQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UnitPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "AV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "CD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "RD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "VAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "CnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "Insurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "TVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "TVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "Others", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UseQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimCnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimInsurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimTVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimTVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "ClaimOthers", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "SubTotalDDB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMCnF", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMInsurance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMTVB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMTVA", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMATV", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMOthers", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackDetails", "UOMSubTotalDDB", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "ExpCurrency", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "BDTCurrency", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimCD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimRD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimSD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalDDBack", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimVAT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimCnFAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimInsuranceAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimTVBAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimTVAAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimATVAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("DutyDrawBackHeader", "TotalClaimOthersAmount", "decimal(25, 9)", currConn);

                //commonDal.TableFieldAlter("IssueDetails", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "CostPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "SubTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "Wastage", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "UOMQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueHeaders", "TotalVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("IssueHeaders", "TotalAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "Cost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "BasePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "OtherRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "OtherType", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "OtherAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PriceService", "SalePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ProductCategories", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ProductCategories", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "CostPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "SalesPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "ReceivePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "IssuePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TenderPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "ExportPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "InternalIssuePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TollIssuePrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TollCharge", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "OpeningBalance", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "PacketPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TradingMarkUp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "QuantityInHand", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "RebatePercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TVBRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "CnFRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "InsuranceRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "CDRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "RDRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "AITRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "TVARate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "ATVRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Products", "OpeningTotalCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "CostPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "UOMQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "UOMPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "DollerValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "CurrencyValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "RebateRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "RebateAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "SubTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "CnFAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "InsuranceAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "AssessableValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "CDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "RDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "TVBAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "TVAAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "ATVAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDetails", "OthersAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CnFInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CnFRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CnFAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "InsuranceInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "InsuranceRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "InsuranceAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "AssessableInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "AssessableValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CDInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CDRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "CDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "RDInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "RDRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "RDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVBInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVBRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVBAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "SDInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "VATInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVAInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVARate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "TVAAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "ATVInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "ATVRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "ATVAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "OthersInp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "OthersRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "OthersAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "DutyCompleteQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "DutyCompleteQuantityPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "LineCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceDuties", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceHeaders", "TotalAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceHeaders", "TotalVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("PurchaseInvoiceHeaders", "CurrencyRateFromBDT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "CostPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "SubTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "UOMQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "UOMPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveHeaders", "TotalAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("ReceiveHeaders", "TotalVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "SalesPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "NBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "AVGPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "VATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "SubTotal", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "SD", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "SDAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "TradingMarkUp", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "UOMQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "UOMPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "UOMc", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "DollerValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "CurrencyValue", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeaders", "TotalAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeaders", "TotalVATAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeaders", "CurrencyRateFromBDT", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeadersExport", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeadersExport", "GrossWeight", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SalesInvoiceHeadersExport", "NetWeight", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("SDDeposits", "DepositAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TenderDetails", "TenderQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TenderDetails", "SaleQty", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TenderDetails", "TenderPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "Quantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "UPrice", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "TradingMarkup", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "SDRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "VATRate", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("TransactionHistorys", "TCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("UOMs", "Convertion", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("VDS", "BillAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("VDS", "BillDeductAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("VDS", "VDSPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAlter("Vendors", "VDSPercent", "decimal(25, 9)", currConn);
                //#endregion AlterFields
                #endregion OldBUpdate

                #region Banderol bussiness


                #region CreateTable

                //transResult = NewTableAdd("SettingsRole", CreateSettingsRoleScript, currConn, transaction);
                //transResult = NewTableAdd("Banderols", CreateBanderolsScript, currConn, transaction);
                //transResult = NewTableAdd("PackagingInformations", CreatePackInfoScript, currConn, transaction);
                //transResult = NewTableAdd("BanderolProducts", CreateBandeProScript, currConn, transaction);
                //transResult = NewTableAdd("DemandHeaders", CreateDemandHeaderScript, currConn, transaction);
                //transResult = NewTableAdd("DemandDetails", CreateDemandDetailsScript, currConn, transaction);

                #endregion CreateTable
                #region 20140302
                //transResult = commonDal.TableFieldAdd("Products", "Banderol", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Vehicles", "DriverName", "varchar(100)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("BanderolProducts", "BandProductId", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("BanderolProducts", "WastageQty", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DemandHeaders", "DemandReceiveDate", "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DemandHeaders", "RefNo", "varchar(20)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DemandHeaders", "RefDate", "datetime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("DemandDetails", "DemandQty", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("BanderolProducts", "OpeningQty", "decimal(25, 9)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("BanderolProducts", "OpeningDate", "datetime", currConn, transaction);

                #endregion

                #endregion Banderol bussiness

                #region Bureau 20140616
                if (BureauInfoVM.IsBureau == true)
                {
                    //transResult = NewTableAdd("BureauSalesInvoiceDetails", CreateBureauSaleDetailsScript, currConn, transaction);
                    //transResult = commonDal.TableFieldAdd("BureauSalesInvoiceDetails", "CustomerId", "varchar(20)", currConn, transaction);
                    //transResult = commonDal.TableFieldAdd("BureauSalesInvoiceDetails", "BureauType", "varchar(50)", currConn, transaction);
                    //transResult = commonDal.TableFieldAdd("BureauSalesInvoiceDetails", "BureauId", "varchar(50)", currConn, transaction);
                }
                #endregion

                #region Create table

                //transResult = NewTableAdd("VAT7", CreateVAT7Script, currConn, transaction);
                //transResult = NewTableAdd("TransferRawHeaders", CreateTransferRawHeadersScript, currConn, transaction);
                //transResult = NewTableAdd("TransferRawDetails", CreateTransferRawDetailsScript, currConn, transaction);
                //transResult = NewTableAdd("Trackings", CreateTrackingsScript, currConn, transaction);
                #endregion
                #region Tracking
                //transResult = commonDal.TableFieldAdd("Trackings", "ReceivePost", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "SalePost", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "IssuePost", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReceiveDate", "DateTime", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "UnitPrice", "Decimal(25,9)", currConn, transaction);

                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnType", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnPurchase", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnPurchaseID", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnReceive", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnReceiveID", "varchar(50)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnSale", "varchar(1)", currConn, transaction);
                //transResult = commonDal.TableFieldAdd("Trackings", "ReturnSaleID", "varchar(50)", currConn, transaction);
                

                #endregion Tracking


                if (transResult < 0)
                {
                    transaction.Commit();
                }

                #region Foreign Key
                
                
                //transResult = AddForeignKey("BanderolProducts", "FK_BanderolProducts_Banderols", AddForeignKeyBandeProduct1, currConn, null);
                //transResult = AddForeignKey("BanderolProducts", "FK_BanderolProducts_PackagingInformations", AddForeignKeyBandeProduct2, currConn, null);
                //transResult = AddForeignKey("DemandDetails", "FK_DemandDetails_BanderolProducts", AddForeignKeyDemandDetails1, currConn, null);
                //transResult = AddForeignKey("DemandDetails", "FK_DemandDetails_DemandHeaders", AddForeignKeyDemandDetails2, currConn, null);
                //transResult = AddForeignKey("TransferRawDetails", "FK_TransferRawDetails_Products", AddForeignKeyTransferRawDetails1, currConn, null);
                //transResult = AddForeignKey("TransferRawDetails", "FK_TransferRawDetails_TransferRawHeaders", AddForeignKeyTransferRawDetails2, currConn, null);
                //transResult = AddForeignKey("Trackings", "FK_Trackings_Products", AddForeignKeyTrackings, currConn, null);
                #endregion

            }
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {
                transaction.Rollback();

                //throw sqlex;
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + sqlex.Message.ToString());


            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + ex.Message.ToString());


                //throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

        }
        public int TableAdd(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }

                #endregion Validation

                #region Prefetch

                sqlText = "";

                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(@TableName) AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " CREATE TABLE @TableName ( @FieldName @DataType null) ";
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);
                cmdPrefetch.Parameters.AddWithValue("@FieldName", FieldName);
                cmdPrefetch.Parameters.AddWithValue("@DataType", DataType);

                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }
        public int TableFieldAdd(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = @FieldName  and Object_ID = Object_ID(@TableName ))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE @TableName  ADD @FieldName @DataType ;";
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);
                cmdPrefetch.Parameters.AddWithValue("@FieldName", FieldName);
                cmdPrefetch.Parameters.AddWithValue("@DataType", DataType);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }

        public void TableFieldAlter(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " ALTER TABLE @TableName ALTER COLUMN @FieldName @DataType";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);
                cmdPrefetch.Parameters.AddWithValue("@FieldName", FieldName);
                cmdPrefetch.Parameters.AddWithValue("@DataType", DataType);

                cmdPrefetch.ExecuteScalar();
                #endregion Prefetch


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
        }
        public string settingValue(string settingGroup, string settingName)
        {
            #region Initializ

            string retResults = string.Empty;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingValue", "Code system not find");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingValue", "Code system not find");
                }


                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction



                #region Stock

                sqlText = "  ";

                sqlText = " SELECT SettingValue FROM Settings ";
                sqlText += " where ";
                sqlText += " SettingGroup=@settingGroup ";
                sqlText += " and SettingName=@settingName";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@settingName", settingName);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = string.Empty;
                }
                else
                {
                    retResults = (string)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                    //object objDel = cmdDelete.ExecuteScalar();
                }

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public bool TransactionUsed(string tableName, string tableIdField, string FieldValue, SqlConnection currConn)
        {
            #region Initializ

            bool sqlResult = false;
            string sqlText = "";
            int CurrentID = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FieldValue))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");

                }
                else if (string.IsNullOrEmpty(tableIdField))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");

                }
                else if (string.IsNullOrEmpty(FieldValue))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");

                }

                #endregion Validation
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region CurrentID
                sqlText = "";
                sqlText += "  SELECT COUNT(@tableIdField) FROM @tableName  ";
                sqlText += " WHERE @tableIdField=@FieldValue";
                SqlCommand cmdCurrentID = new SqlCommand(sqlText, currConn);

                cmdCurrentID.Parameters.AddWithValue("@tableIdField", tableIdField);
                cmdCurrentID.Parameters.AddWithValue("@tableName", tableName);
                cmdCurrentID.Parameters.AddWithValue("@FieldValue", FieldValue);

                CurrentID = Convert.ToInt32(cmdCurrentID.ExecuteScalar());

                if (CurrentID > 0)
                {
                    sqlResult = true;
                }
                #endregion CurrentID
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return sqlResult;

            #endregion

        }
        public int DataAlreadyUsed(string CompareTable, String CompareField, String CompareWith, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            int retResults = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {


                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction



                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT isnull(COUNT(@CompareField),0) FROM @CompareTable";
                sqlText += "  WHERE @CompareField =@CompareWith";

                SqlCommand cmdDAU = new SqlCommand(sqlText, currConn);

                cmdDAU.Parameters.AddWithValue("@CompareField", CompareField);
                cmdDAU.Parameters.AddWithValue("@CompareTable", CompareTable);
                cmdDAU.Parameters.AddWithValue("@CompareWith", CompareWith);

                cmdDAU.Transaction = transaction;
                if (cmdDAU.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (int)cmdDAU.ExecuteScalar();
                    //object objDel = cmdDelete.ExecuteScalar();

                }

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string decimal259(object input)
        {
            string result = "0";
            int index;
            string tmp = "0", prefetct = "0", postfix = "0";
            if (string.IsNullOrEmpty(input.ToString()))
            {
                prefetct = input.ToString();
                postfix = "0";
            }
            else if (input.ToString().IndexOf('.') == -1)
            {
                prefetct = input.ToString();
                postfix = "0";
            }
            else
            {
                index = input.ToString().IndexOf('.');
                tmp = input.ToString().Substring(index + 1);
                prefetct = input.ToString().Substring(0, index);

                if (tmp.Length > 9)
                {
                    postfix = tmp.Substring(0, 9);

                }
                else
                {
                    postfix = tmp.Substring(0, tmp.Length);

                }

            }
            var tmpR = prefetct + "." + postfix;
            result = tmpR;
            return result;
        }

        public decimal FormatingDecimal(string input)
        {
            string inputValue = input;
            decimal outPutValue = 0;
            string decPointLen = "";
            int DecPlace = 9;
            try
            {

                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }

                if (Convert.ToDecimal(inputValue) < 1000)
                {
                    var a = "0." + decPointLen + "";
                    //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0.0000"));
                    outPutValue = Convert.ToDecimal(inputValue);


                }
                else
                {
                    var a = "0,0." + decPointLen + "";

                    //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0,0.0000"));
                    //outPutValue = Convert.ToDecimal(inputValue).ToString(a);
                    outPutValue = Convert.ToDecimal(inputValue);

                }


            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                throw new Exception(ex.Message);
            }
            #endregion Catch

            return outPutValue;
        }

        public int TableFieldAddInSys(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();//

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction


                #region Prefetch

                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = @FieldName and Object_ID = Object_ID(@TableName))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE @TableName ADD @FieldName @DataType ;";
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                //cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@FieldName", FieldName);
                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);
                cmdPrefetch.Parameters.AddWithValue("@DataType", DataType);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion

            return transResult;
        }

        public int DeleteForeignKey(string TableName, string ForeignKeyName, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(ForeignKeyName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create Foreign Key");

                }


                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " if exists(select * from sys.foreign_keys ";
                sqlText += " where Name = @ForeignKeyName )   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE @TableName DROP CONSTRAINT @ForeignKeyName ";
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@ForeignKeyName", ForeignKeyName);
                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }

        public int ExecuteUpdateQuery(string SqlText, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = SqlText;
            int transResult = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(SqlText))
                {
                    throw new ArgumentNullException("ExecuteUpdateQuery", "No data found for Update.");

                }

                #endregion Validation

                #region Prefetch

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                transResult = (int)cmd.ExecuteNonQuery();
                //transResult = -1;

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }

        public string GetHardwareID()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + "Win32_Processor");
            string processorId = "";
            try
            {

                foreach (ManagementObject share in searcher.Get())
                {
                    if (share.Properties.Count > 0)
                    {
                        foreach (PropertyData PC in share.Properties)
                        {

                            if (PC.Name.ToLower() == "processorid")
                            {
                                processorId = PC.Value.ToString();
                                break;
                            }

                        }
                    }
                }
            }


            catch (Exception exp)
            {
                //MessageBox.Show("can't get data because of the followeing error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return processorId;
        }

        public void SetSecurity(string companyId)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            try
            {
                ReportDSDAL reportDsdal = new ReportDSDAL();
                DataSet ReportResult = reportDsdal.ComapnyProfileSecurity(companyId);

                if (ReportResult.Tables[0].Rows.Count <= 0)
                {
                    return;
                }

                #region Retrive Data

                string cName = ReportResult.Tables[0].Rows[0]["CompanyName"].ToString();
                string cLegalName = ReportResult.Tables[0].Rows[0]["CompanyLegalName"].ToString();
                string vatNo = ReportResult.Tables[0].Rows[0]["VatRegistrationNo"].ToString();
                string hardwareInfo = ReportResult.Tables[0].Rows[0]["Mouse"].ToString();



                string tom = Converter.DESEncrypt(PassPhrase, EnKey, cName);
                string jary = Converter.DESEncrypt(PassPhrase, EnKey, cLegalName);
                string miki = Converter.DESEncrypt(PassPhrase, EnKey, vatNo);
                string mouse = "";

                if (string.IsNullOrEmpty(hardwareInfo))
                {
                    //mouse = GetHardwareID();
                    mouse = GetServerHardwareId();

                    mouse = Converter.DESEncrypt(PassPhrase, EnKey, mouse);
                }
                else
                {
                    return;

                }

                #endregion Retrive Data

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();

                }
                transaction = currConn.BeginTransaction(MessageVM.saleMsgPostNotSelect);

                if (string.IsNullOrEmpty(hardwareInfo))
                {
                    string updateQuer = "";
                    updateQuer += " Update CompanyProfiles set Tom ='" + tom + "',";
                    updateQuer += " Jary ='" + jary + "', Miki ='" + miki + "', Mouse ='" + mouse + "'";
                    updateQuer += "where CompanyID='" + companyId + "'";

                    transResult = ExecuteUpdateQuery(updateQuer, currConn, transaction);

                    if (transResult >= 0)
                    {
                        transaction.Commit();
                    }
                }

            }
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {
                transaction.Rollback();

                throw sqlex;

            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

        }

        public string GetServerHardwareId()
        {
            #region Initializ

            string retResults = string.Empty;
            SqlConnection currConn = null;
            string sqlText = @"EXEC xp_instance_regread
                                'HKEY_LOCAL_MACHINE',
                                'HARDWARE\DESCRIPTION\System\MultifunctionAdapter\0\DiskController\0\DiskPeripheral\0',
                                'Identifier'";
            #endregion Initializ

            #region Try
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                DataTable dt = new DataTable("ServerProcessor");

                //SqlCommand getHardware = new SqlCommand(sqlText, currConn);
                SqlDataAdapter adapter = new SqlDataAdapter(sqlText, currConn);
                adapter.Fill(dt);

                if (dt == null)
                {
                    retResults = string.Empty;
                }
                else if (dt.Columns.Count > 0)
                {
                    retResults = dt.Rows[0][1].ToString();
                }


            }
            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
                //}
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public int NewTableAdd(string TableName, string createQuery, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(createQuery))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                //}

                #endregion Validation

                #region Prefetch

                sqlText = "";

                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(@TableName) AND type in (N'U'))";

                sqlText += " BEGIN";
                sqlText += " " + createQuery;
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;

                cmdPrefetch.Parameters.AddWithValue("@TableName", TableName);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }

        public int AddForeignKey(string TableName, string ForeignKeyName, string query, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;


            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrEmpty(query))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }
                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                //}

                #endregion Validation

                #region Prefetch

                sqlText = "";

                sqlText = "";
                sqlText += " if NOT exists(select * from sys.foreign_keys ";
                sqlText += " where Name = @ForeignKeyName )   ";

                sqlText += " BEGIN";
                sqlText += " " + query;
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                cmdPrefetch.Parameters.AddWithValue("@ForeignKeyName", ForeignKeyName);

                cmdPrefetch.ExecuteNonQuery();
                //cmdPrefetch.Transaction = transaction;
                //transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion

            return transResult;
        }
        public int NextId(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "select isnull(max(cast(id as int)),0)+1 FROM  " + tableName + "";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId;
            #endregion
        }

        public string[] DeleteTableInformation(string Id, string TableName, string FieldName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = FieldName + " from " + TableName + " Delete"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                #region Update Settings
                sqlText = "";
                sqlText = "delete from  " + TableName + " ";
                sqlText += " where " + FieldName + "=@Id";
                //sqlText += " AND IsArchive=0";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", Id);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    //throw new ArgumentNullException(FieldName+ " Delete", Id + " could not Delete.");
                }
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #endregion Update Settings
                #region Commit
                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete Project Information.";
                    throw new ArgumentNullException("", "");
                }
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            return retResults;
        }
        public string[] FieldPost(string tableName, string conditionField, string conditionValue, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("PostTo" + tableName); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                #region Update Settings
                sqlText = "";
                sqlText = "UPDATE " + tableName + " SET";
                sqlText += " Post=@Post";
                sqlText += " WHERE 1=1 AND";
                sqlText += " " + conditionField + "=@ConditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Post", true);
                cmdUpdate.Parameters.AddWithValue("@ConditionValue", conditionValue);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }
                #endregion Commit
                #endregion Update Settings
                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Posted Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            return retResults;
        }
        public int NextIdWithCategory(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "select isnull(max(cast(CategoryID as int)),0)+1 FROM  " + tableName + "";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId;
            #endregion
        }
        public int NextIdWithColumn(string tableName, string columnName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "select isnull(max(cast(" + columnName + " as int)),0)+1 FROM  " + tableName + "";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId;
            #endregion
        }
        public string SelectFieldValue(string tableName, string fieldName, string conditionField, string conditionValue, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            string retFieldValue = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "";
                sqlText += " SELECT Top 1 ";
                sqlText += " Id, ";
                sqlText += fieldName;
                sqlText += " From ";
                sqlText += tableName;
                sqlText += " WHERE 1=1";
                sqlText += " AND " + conditionField + "=@conditionValue";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@conditionValue", conditionValue);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    retFieldValue = dr[fieldName].ToString();
                }
                dr.Close();
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return retFieldValue;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retFieldValue;
            #endregion
        }
        public int NextIdWithType(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "select isnull(max(cast(TypeID as int)),0)+1 FROM  " + tableName + "";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId;
            #endregion
        }
        public DateTime ServerDateTime()
        {
            DateTime result = DateTime.Now;
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                sqlText = "select GETDATE()";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //result = Convert.ToDateTime(cmdIdExist.ExecuteScalar());
                var exeRes = cmdIdExist.ExecuteScalar();
                result = Convert.ToDateTime(exeRes);
            }
            #region Catch
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            return result;
        }

        public string GetTargetId(string tableName, string columnName, string currentId, string btn, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Save
                sqlText = @"SELECT top 1 " + columnName + " From " + tableName + " Where 1=1 ";

                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND " + columnName + " = " + currentId + " ORDER BY " + columnName + " desc";
                }
                if (btn.ToLower() == "next")
                {
                    sqlText += @" AND " + columnName + " > " + currentId + " ORDER BY " + columnName;
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND " + columnName + " < " + currentId + " ORDER BY " + columnName + " desc";
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY " + columnName;
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY " + columnName + " desc";
                }

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId == 0)
                {
                    nextId = Convert.ToInt32(currentId);
                }
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId.ToString();
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId.ToString();
            #endregion
        }
        public string GetTargetIdForTtype(string tableName, string columnName, string currentId, string btn,string ttype, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Save
                sqlText = @"SELECT top 1 " + columnName + " From " + tableName + " Where 1=1 and TransactionType='"+ttype+"' ";

                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND " + columnName + " = " + currentId + " ORDER BY " + columnName + " desc";
                }
                if (btn.ToLower() == "next")
                {
                    sqlText += @" AND " + columnName + " > " + currentId + " ORDER BY " + columnName;
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND " + columnName + " < " + currentId + " ORDER BY " + columnName + " desc";
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY " + columnName;
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY " + columnName + " desc";
                }

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId == 0)
                {
                    nextId = Convert.ToInt32(currentId);
                }
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return nextId.ToString();
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return nextId.ToString();
            #endregion
        }
        public List<IdName> IdNameDropdownOverhead(string Id, string Name, string AllData, string code)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<IdName> VMs = new List<IdName>();
            IdName vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @" SELECT p." + Id + ", p." + Name + ", p." + code + " from Products p left outer join ProductCategories pc on p.CategoryID=pc.CategoryID where pc.IsRaw='Overhead' ";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new IdName();
                    vm.Id = dr[Id].ToString();

                    var name = dr[Name].ToString();
                    var itemCode = dr[code].ToString();
                    if (AllData.ToLower() == "yes")
                    {
                        vm.Name = itemCode;
                    }
                    else
                    {
                        vm.Name = name + "-" + itemCode;
                    }

                    VMs.Add(vm);
                }
                dr.Close();
                #endregion

            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
        public string[] InsertThreads(string value, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = "  ";
                sqlText += @" INSERT INTO Threads(TValue  ) 
                                VALUES ( @TValue ) 
                                        ";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Parameters.AddWithValue("@TValue", value);
                cmdInsert.Transaction = transaction;
                cmdInsert.ExecuteNonQuery();
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = "1";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public List<IdName> IdNameDropdown(string tableName, string Id, string Name, string AllData, string code)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<IdName> VMs = new List<IdName>();
            IdName vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                if (AllData.ToLower() == "yes")
                {
                    sqlText = @" SELECT " + Id + ", " + Name + ", " + code + " from " + tableName + " where 1=1";
                }
                else
                {
                    sqlText = @" SELECT " + Id + ", " + Name + ", " + code + " from " + tableName + " where 1=1 and ActiveStatus='Y'";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new IdName();
                    vm.Id = dr[Id].ToString();

                    var name = dr[Name].ToString();
                    var itemCode = dr[code].ToString();
                    if (Name==code)
                    {
                        vm.Name = itemCode;
                    }
                    else
                    {
                        vm.Name = name + "-" + itemCode;
                    }

                    VMs.Add(vm);
                }
                dr.Close();
                #endregion

            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        public List<IdName> IdNameTtype(string tableName, string Id, string Name, string type, string code)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<IdName> VMs = new List<IdName>();
            IdName vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement

                sqlText = @" SELECT " + Id + ", " + Name + ", " + code + " from " + tableName + " where 1=1 and TransactionType='" + type+"'";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new IdName();
                    vm.Id = dr[Id].ToString();

                    var name = dr[Name].ToString();
                    var itemCode = dr[code].ToString();
                    if (Name == code)
                    {
                        vm.Name = itemCode;
                    }
                    else
                    {
                        vm.Name = name + "-" + itemCode;
                    }

                    VMs.Add(vm);
                }
                dr.Close();
                #endregion

            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        public DataTable DataTableLoad(string tableName, string[] Condition, string DatabaseName)
        {
            DataTable dt = new DataTable("");
            SqlConnection currConn = null;
            try
            {
                #region New open connection and transaction
                #endregion New open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                    currConn.Open();
                if (DatabaseName != null)
                {
                    currConn.ChangeDatabase(DatabaseName);
                }
                string sql = "";
                sql = sql + @" select  * from " + tableName + "";
                if (Condition != null)
                {
                    if (Condition.Count() > 0)
                        sql = sql + @" where ";
                    for (int i = 0; i < Condition.Length; i++)
                    {
                        if (i > 0)
                            sql = sql + @" and ";
                        sql = sql + Condition[i];
                    }
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sql;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dt);
                return dt;
                #region Commit
                #endregion Commit
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }
        public bool FileDelete(string tableName, string field, string id, SqlConnection currConn, SqlTransaction transaction)
        {
            bool returnval = false;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("FileDelete"); }
                string sqlText = "";
                sqlText = "update " + tableName + " set " + field + "=@field";
                sqlText += " where Id=@Id";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", id);
                cmdUpdate.Parameters.AddWithValue("@field", "");
                cmdUpdate.Transaction = transaction;
                cmdUpdate.ExecuteNonQuery();
                returnval = true;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            return returnval;
        }
        public System.Windows.Forms.DataGridView DataGridViewLoad(System.Windows.Forms.DataGridView dgv, string TableName, string[] Column, string[,] SearchColumn)
        {
            //string[,] SearchColumn = new string[3, 2] 
            //{
            //   // { "Search Field", "Search Value" },
            //    { "BodyColor", txtBodyColor.Text },
            //    { "CuffColor", txtCuffColor.Text },
            //    { "Size", txtSize.Text }
            //};
            SqlConnection currConn = null;
            currConn = _dbsqlConnection.GetConnection();//
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }
            string sql = "";
            sql = sql + @" Select ";
            foreach (var item in Column)
            {
                sql = sql + @" " + item + ",";
            }
            sql = sql.Substring(0, sql.Length - 1);
            System.Data.DataTable dt = new System.Data.DataTable("DataTable");
            sql = sql + @" from " + TableName + "";
            sql = sql + @" where IsArchive=0";
            if (SearchColumn.GetUpperBound(0) > 0)
            {
                for (int i = 0; i <= SearchColumn.GetUpperBound(0); i++)
                {
                    string SearchField = SearchColumn[i, 0];
                    string SearchValue = SearchColumn[i, 1];
                    sql = sql + @" and " + SearchField + " like '%" + SearchValue + "%' ";
                }
            }
            SqlDataAdapter adp = new SqlDataAdapter(sql, currConn);
            adp.Fill(dt);
            dgv.DataSource = dt;
            return dgv;
        }

        public System.Windows.Forms.ComboBox ComboBoxLoad(System.Windows.Forms.ComboBox comboBox, string tableName, string valueMember, string displayMember)
        {
            SqlConnection currConn = null;
            currConn = _dbsqlConnection.GetConnection();//
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }
            System.Data.DataTable dt = new System.Data.DataTable("ComboDt");
            string sql = "";
            sql = sql + @"select " + valueMember + ", " + displayMember + " from " + tableName + "";
            SqlDataAdapter adp = new SqlDataAdapter(sql, currConn);
            adp.Fill(dt);
            comboBox.DataSource = dt;
            comboBox.DisplayMember = displayMember.Replace("[", "").Replace("]", "");
            comboBox.ValueMember = valueMember;
            return comboBox;
        }
        public bool AlreadyExist(string tableName, string fieldName, string value)
        {
            #region Variables
            bool Exist = false;
            SqlConnection currConn = null;
            string sqlText = "";
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"SELECT top 1 " + fieldName + " from " + tableName + " where " + fieldName + "= '" + value + "' ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return Exist;
        }
        public System.Windows.Forms.ComboBox AllDB(System.Windows.Forms.ComboBox cmb)
        {
            List<Database> dbList;
            try
            {
                SqlConnection sqlConn = _dbsqlConnection.GetConnectionMaster();
                Server sqlServer = new Server(new ServerConnection(sqlConn));
                dbList = new List<Database>();
                foreach (Database db in sqlServer.Databases)
                {
                    var tt = db;
                    dbList.Add(tt);
                }
                cmb.DataSource = dbList;
                cmb.SelectedIndex = -1;
            }
            catch (Exception exc)
            {
            }
            return cmb;
        }

        #region New Methods

         public bool ExistCheck(string tableName, string selectFieldName, string[] conditionFields, string[] conditionValues, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @" SELECT TOP 1 "+ selectFieldName
                    + " FROM [" + tableName + "]"
                    + " WHERE 1=1  ";
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        _objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                    break;
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return Exist;
        }
        #endregion


    }

}

