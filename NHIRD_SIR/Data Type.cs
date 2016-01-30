using System;
using System.Collections.Generic;
using System.Linq;
using NHIRD.Extensions;
using NHIRD.PoissonDistribution;
using NHIRD.Actions;

namespace NHIRD.DataTypes
{
    class OrderGroup
    {
        public OrderGroup(string name, List<string> OrderCode)
        {
            str_name = name; list_OrderCode = OrderCode;
        }
        public string str_name { get; set; }
        public List<string> list_OrderCode;
    }
    class SecondDiagnosisGroup
    {
        public string str_name { get; set; }
        public List<string> list_ICD9 { get; set; }
    }


    class NHIRD_DataTypes
    {
        /// <summary>
        /// 可供比較的病人資料(定義ID, Birthday, Gender)
        /// </summary>
        internal class Patient : IComparable
        {

            public string str_ID { get; set; }
            /// <summary>
            /// 生日可輸入舊資料(8位)或新資料(6位，日期設定為該月1日) ，輸出必定為"yyyy-MM-dd"
            /// </summary>
            public string str_Birthday
            {
                get { return _dt_Birthday.DatetimeToFormatString(); }
                set
                {
                    string modify;
                    modify = value.Substring(0, value.Length - 2) + "01";
                    _dt_Birthday = modify.StringToDatetime();
                }
            }
            protected DateTime _dt_Birthday = DateTime.MinValue;

            public string str_Gender { get; set; }
            // 實作IComparable 比較子
            int IComparable.CompareTo(object obj)
            {
                var that = obj as PatientBasedData;
                return (this.str_ID + this.str_Birthday).CompareTo(that.str_ID + that.str_Birthday);
            }
        }
        /// <summary>
        /// 繼承Patient,加入order類別及診斷
        /// </summary>
        internal class ActionBasedData : Patient
        {
            // -- 診斷
            public string[] array_ICD;
            /// <summary>
            /// 判斷是否符合ICD criteria
            /// </summary>
            /// <param name="List_criteriaICD">ICD criteria</param>
            /// <returns></returns>
            public bool IsICDMatch(IEnumerable<string> List_criteriaICD)
            {
                foreach (string str_ICD in array_ICD)
                {
                    if (List_criteriaICD.Any(x => x == str_ICD.Substring(0, x.Length))) return true;
                }
                return false;
            }
            // -- 對應OO或DO檔的變數
            public string str_Fee_YM { get; set; }
            public string str_HospID { get; set; }
            public string str_ApplDate { get; set; }
            public string str_SeqNO { get; set; }
            // -- Order類別以及方法
            /// <summary>
            /// 該筆action所附帶的order類別
            /// </summary>
            public class Order
            {
                /// <summary>
                /// 醫令出現次數
                /// </summary>
                public int int_Count { get; set; }
                /// <summary>
                /// 最早醫令出現時間
                /// </summary>
                public string str_FirstDate
                {
                    get
                    {
                        if (_dt_FirstDate != DateTime.MinValue)
                        {
                            return _dt_FirstDate.DatetimeToFormatString();
                        }
                        return "";
                    }
                    set
                    {
                        if (value.StringToDatetime() < _dt_FirstDate || _dt_FirstDate == DateTime.MinValue)
                        {
                            _dt_FirstDate = value.StringToDatetime();
                        }
                    }
                }
                internal DateTime _dt_FirstDate = DateTime.MinValue;
            }
            public Order[] array_Order;
            /// <summary>
            /// 取得指定order 最早出現時年紀
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public string getOrder_FirstAge(int index)
            {
                return Math.Round((double)array_Order[index]._dt_FirstDate.Subtract(base._dt_Birthday).Days / 365, 1).ToString();
            }
            /// <summary>
            /// 取得指定order計數
            /// </summary>
            public int getOrder_count(int index)
            {
                return array_Order[index].int_Count;
            }
        }
        /// <summary>
        /// 依序比較FEE_YM, HOSP_ID, ApplDate, SEQ_NO
        /// </summary>
        internal class ActionBasedData_OrderComparer : IComparer<ActionBasedData>
        {
            public int Compare(ActionBasedData x, ActionBasedData y)
            {
                return (x.str_Fee_YM + x.str_HospID + x.str_ApplDate + x.str_SeqNO).CompareTo(y.str_Fee_YM + y.str_HospID + y.str_ApplDate + y.str_SeqNO);
            }
        }
        /// <summary>
        /// 繼承ActionBasedData，加入CD會使用到的參數
        /// </summary>
        public sealed class ActionBasedData_CD : ActionBasedData
        {
            // -- contructor
            /// <summary>
            /// 設定order組別數量
            /// </summary>
            /// <param name="int_OrderGroupCount"></param>
            public ActionBasedData_CD(int int_OrderGroupCount)
            {
                base.array_ICD = new string[3];
                if (int_OrderGroupCount > 0)
                    this.array_Order = new Order[int_OrderGroupCount];
            }
            /// <summary>
            /// 應診日期
            /// </summary>
            public string str_FuncDate { get; set; }
            /// <summary>
            /// 應診科別
            /// </summary>
            public string str_FuncType { get; set; }
        }
        /// <summary>
        /// 以FeeYM為索引，製作ActionBasedData CD或DD之Bank class
        /// </summary>
        public sealed class Bank_ActionBasedData<T> : IComparable<T>
        {
            string str_FeeYM { get; }
            List<T> list_ActionBasedData_CD;
            public Bank_ActionBasedData<T>(string str_)
            {

            }

        public int CompareTo(T other)
        {
            return (this.str_FeeYM.CompareTo((other as Bank_ActionBasedData<T>).str_FeeYM));
        }
    }

    /// <summary>
    /// 繼承ActionBasedData，加入DD會使用到的參數
    /// </summary>
    public sealed class ActionBasedData_DD : ActionBasedData
    {
        // -- contructor
        /// <summary>
        /// 設定order組別數量
        /// </summary>
        /// <param name="int_OrderGroupCount"></param>
        public ActionBasedData_DD(int int_OrderGroupCount)
        {
            base.array_ICD = new string[5];
            if (int_OrderGroupCount > 0)
                this.array_Order = new Order[int_OrderGroupCount];
        }
        /// <summary>
        /// 住院日期
        /// </summary>
        public string str_InDate { get; set; }

    }

    /// <summary>
    /// Patient Based Data，合併多筆Action Based Data以及醫囑資料，內含order Group之資料(次數及最早時間)
    /// </summary>
    public sealed class PatientBasedData : Patient
    {
        // -- 主要事件
        /// <summary>
        /// 主要事件日期
        /// </summary>
        public string str_PrimaryFirstDate
        {
            get
            {
                if (_dt_PrimaryFirstDate != DateTime.MinValue)
                {
                    return _dt_PrimaryFirstDate.DatetimeToFormatString();
                }
                return "";
            }
            set
            {
                if (value.StringToDatetime() < _dt_PrimaryFirstDate || _dt_PrimaryFirstDate == DateTime.MinValue)
                {
                    _dt_PrimaryFirstDate = value.StringToDatetime();
                }
            }
        }
        DateTime _dt_PrimaryFirstDate = DateTime.MinValue;
        /// <summary>
        /// 主要事件年紀
        /// </summary>
        public double db_PrimaryFirstAge
        {
            get { return Math.Round((double)_dt_PrimaryFirstDate.Subtract(_dt_Birthday).Days / 365, 1); }
        }
        /// <summary>
        /// 主要事件計數
        /// </summary>
        public int int_PrimaryCount { get; set; }
        /// <summary>
        /// 判斷是否為study group
        /// </summary>
        public bool IsStudyGroup
        {
            get { return _dt_PrimaryFirstDate > DateTime.MinValue; }
        }

        // -- 次要事件 --
        /// <summary>
        /// Patient based data中次診斷類別
        /// </summary>
        public class SecondaryDiagnosis
        {
            /// <summary>
            /// 次診斷在門診出現次數
            /// </summary>
            public int int_CountCD { get; set; }
            /// <summary>
            /// 次診斷在住院出現次數
            /// </summary>
            public int int_CountDD { get; set; }
            /// <summary>
            /// 次診斷的初次日期
            /// </summary>
            public string str_FirstDate
            {
                get
                {
                    if (_dt_FirstDate != DateTime.MinValue)
                    {
                        return _dt_FirstDate.DatetimeToFormatString();
                    }
                    return "";
                }
                set
                {
                    if (value.StringToDatetime() < _dt_FirstDate || _dt_FirstDate == DateTime.MinValue)
                    {
                        _dt_FirstDate = value.StringToDatetime();
                    }
                }
            }
            internal DateTime _dt_FirstDate = DateTime.MinValue;
        }
        /// <summary>
        /// 次診斷
        /// </summary>
        public SecondaryDiagnosis[] array_secondDiagnosis;
        /// <summary>
        /// 取得某次診斷與主要事件的時間差距, 單位：年
        /// </summary>
        public string getSecondary_PostEventTime(int index)
        {
            if (_dt_PrimaryFirstDate != DateTime.MinValue)
                return Math.Round((double)_dt_PrimaryFirstDate.Subtract(array_secondDiagnosis[index]._dt_FirstDate).Days / 365, 1).ToString();
            return "";
        }
        /// <summary>
        ///  取得某次診斷出現的年紀, 單位：年
        /// </summary>
        public string getSecondary_FirstAge(int index)
        {
            if (_dt_PrimaryFirstDate != DateTime.MinValue)
                return Math.Round((double)array_secondDiagnosis[index]._dt_FirstDate.Subtract(_dt_Birthday).Days / 365, 1).ToString();
            return "";
        }
        /// <summary>
        ///  取得某次診斷出現在門診的次數
        /// </summary>
        public int getSecondary_CountCD(int index)
        {
            return array_secondDiagnosis[index].int_CountCD;
        }
        /// <summary>
        ///  取得某次診斷出現在門診的次數
        /// </summary>
        public int getSecondary_CountDD(int index)
        {
            return array_secondDiagnosis[index].int_CountDD;
        }

        // -- 保險時間
        /// <summary>
        /// 最早保險開始時間
        /// </summary>
        public string str_InsuranceFirstStartDate
        {
            get { return _dt_InsuranceFisrtStartDate.DatetimeToFormatString(); }
            set
            {
                _dt_InsuranceFisrtStartDate = value.StringToDatetime();
            }
        }
        DateTime _dt_InsuranceFisrtStartDate = DateTime.MinValue;
        /// <summary>
        /// 最晚保險開始時間
        /// </summary>
        public string str_InsuranceLastStartDate
        {
            get { return _dt_InsuranceLastStartDate.DatetimeToFormatString(); }
            set
            {
                _dt_InsuranceLastStartDate = value.StringToDatetime();
            }
        }
        DateTime _dt_InsuranceLastStartDate = DateTime.MinValue;
        /// <summary>
        /// 最晚保險結束時間(可能為0)
        /// </summary>
        public string str_InsuranceEndDate
        {
            get { return _dt_InsuranceEndDate.DatetimeToFormatString(); }
            set
            {
                //ID summary 初始值為"1900-01-01"(待修改)
                if (value.StringToDatetime() == DateTime.Parse("1900-01-01"))
                {
                    _dt_InsuranceEndDate = DateTime.MaxValue;
                }
                else
                {
                    _dt_InsuranceEndDate = value.StringToDatetime();
                }
            }
        }
        DateTime _dt_InsuranceEndDate = DateTime.MinValue;

        // --輸入年份區段 計算在資料庫時間
        /// <summary>
        /// 輸入年份區段，取得資料開始時年紀，若傳回-1代表無交集。
        /// </summary>
        /// <param name="int_data_start_year">年份開始</param>
        /// <param name="int_data_end_year">年份結束</param>
        /// <returns></returns>
        public double db_data_start_age(int int_data_start_year, int int_data_end_year)
        {
            //此病人在資料庫開始的時間(start_time)為 Max(設定的資料庫最早時間, 病人最早出現時間)
            DateTime dt_data_start_time = DateTime.Parse(int_data_start_year + "-01-01");
            DateTime dt_data_end_time = DateTime.Parse((int_data_end_year + 1) + "-01-01");
            //無交集 傳回-1 =>  最晚時間<data start time或 最早時間>data end time
            if ((_dt_InsuranceLastStartDate < dt_data_start_time &&
                _dt_InsuranceEndDate < dt_data_start_time) ||
                _dt_InsuranceFisrtStartDate > dt_data_end_time)
                return -1;
            //有交集, 取data start time及ins. first date較晚的
            DateTime dt_start_time = _dt_InsuranceFisrtStartDate > dt_data_start_time ?
                _dt_InsuranceFisrtStartDate : dt_data_start_time;
            return Math.Round((double)dt_start_time.Subtract(_dt_Birthday).Days / 365, 1);
        }
        /// <summary>
        /// 輸入年份區段，取得資料結束時年紀，若傳回-1代表無交集。
        /// </summary>
        /// <param name="int_data_start_year">年份開始</param>
        /// <param name="int_data_end_year">年份結束</param>
        /// <returns></returns>
        public double db_data_end_age(int int_data_start_year, int int_data_end_year)
        {
            //此病人在資料庫的結束時間(end_time)為 Min(設定的資料庫最晚時間, 病人最晚出現時間)
            DateTime dt_data_start_time = DateTime.Parse(int_data_start_year + "-01-01");
            DateTime dt_data_end_time = DateTime.Parse((int_data_end_year + 1) + "-01-01");
            //無交集 傳回-1 =>  最晚時間<data start time或 最早時間>data end time
            if ((_dt_InsuranceLastStartDate < dt_data_start_time &&
                _dt_InsuranceEndDate < dt_data_start_time) ||
                _dt_InsuranceFisrtStartDate > dt_data_end_time)
                return -1;
            //有交集, 假使ins last start day > ins end day => 最後一筆為加保 => end time = data end time
            //假使 ins last start day <= ins end day => 最後一筆為退保 
            //                                    => end time = ins end day 及 data end time較早的
            DateTime dt_end_time;
            if (_dt_InsuranceLastStartDate >= _dt_InsuranceEndDate)
            {
                dt_end_time = dt_data_end_time;
            }
            else
            {
                dt_end_time = _dt_InsuranceEndDate < dt_data_end_time ?
                    _dt_InsuranceEndDate : dt_data_end_time;
            }
            return Math.Round((double)dt_end_time.Subtract(_dt_Birthday).Days / 365, 1);
        }
        /// <summary>
        /// 判斷此病人是否在指定時間區段內存在於資料庫
        /// </summary>
        /// <param name="int_data_start_year">年份開始</param>
        /// <param name="int_data_end_year">年份結束</param>
        /// <returns></returns>
        public bool IsPatientAvailable(int int_data_start_year, int int_data_end_year)
        {
            if (db_data_start_age(int_data_start_year, int_data_end_year) >= 0
            && db_data_end_age(int_data_start_year, int_data_end_year) >= 0)
                return true;
            return false;
        }
    }

}
class NHIRD_ResultTables
{

}
class NHIRD_ArgTable
{
    // -- Constructor
    public NHIRD_ArgTable(string DatabaseYear, int DataStartYear, int DataEndYear)
    {
        GroupDefinition.initiallizeOrder(this);
        list_SecondDiagnosisGroupDefinition = new List<SecondDiagnosisGroup>();
        str_year = DatabaseYear;
        int_DataStartYear = DataStartYear;
        int_DataEndYear = DataEndYear;
    }
    /// <summary>
    /// 資料庫抽樣年分
    /// </summary>
    public string str_year { get; set; }
    /// <summary>
    /// 資料起始年分
    /// </summary>
    public int int_DataStartYear { get; set; }
    /// <summary>
    /// 資料結束年分
    /// </summary>
    public int int_DataEndYear { get; set; }

    /// <summary>
    /// 儲存醫囑定義
    /// </summary>
    public List<OrderGroup> list_OrderGroupDefinision;
    /// <summary>
    /// 儲存次要診斷的定義
    /// </summary>
    public List<SecondDiagnosisGroup> list_SecondDiagnosisGroupDefinition;
    /// <summary>
    /// 儲存讀入的CD檔，並且根據OrderDefinition，與OO檔運算
    /// </summary>
    public List<List<NHIRD_DataTypes.ActionBasedData_CD>> list_PrimaryActionBasedData_CD;


    public List<PatientBasedData> list_PtBasedData;
    public List<SumData> list_SumData;
    public List<PatientBasedData_ID> list_PtBaseData_IDtable;
    public List<Age_Specific_Incidence> list_AgeSpecInc_AllPt;
    public List<Age_Specific_Incidence> list_AgeSpecInc_StdGrp;
    public List<SIR> list_SIR;

    public void GarbageCollection()
    {
        list_PtBaseData_IDtable = null;
        list_PtBasedData = null;
        list_SumData = null;
        GC.Collect();
    }
}

// -- 存放pateint based data之類別  給CD 或 DD summarize用
class PatientBasedData : IComparable
{
    public string str_ID { get; set; }
    DateTime _dt_Birthday = DateTime.MinValue;
    public string str_Birthday
    {
        get { return _dt_Birthday.DatetimeToFormatString(); }
        set
        {
            string modify;
            modify = value.Substring(0, value.Length - 2) + "01";
            _dt_Birthday = modify.StringToDatetime();
        }
    }
    public string str_Gender { get; set; }

    public int int_GW_OPDTimes { get; set; }
    public int int_GYN_GU_WART_OPDTimes { get; set; }
    DateTime _dt_FirstDate = DateTime.MinValue;
    public string str_FirstDate
    {
        get
        {
            if (_dt_FirstDate != DateTime.MinValue)
            {
                return _dt_FirstDate.DatetimeToFormatString();
            }
            return "";
        }
        set
        {
            if (value.StringToDatetime() < _dt_FirstDate || _dt_FirstDate == DateTime.MinValue)
            {
                _dt_FirstDate = value.StringToDatetime();
            }
        }
    }
    public double db_FirstAge
    {
        get { return Math.Round((double)_dt_FirstDate.Subtract(_dt_Birthday).Days / 365, 1); }
    }
    public SecondDiagnosis[] array_secondDiagnosis { get; set; }

    // 實作IComparable 比較子
    int IComparable.CompareTo(object obj)
    {
        var that = obj as PatientBasedData;
        return (this.str_ID + this.str_Birthday).CompareTo(that.str_ID + that.str_Birthday);
    }

    // -- 在PateitnBasedData內,次要診斷的欄位
    public class SecondDiagnosis
    {
        public SecondDiagnosis(DateTime birthday, DateTime originalDiagnosisDay)
        {
            _dt_Birthday = birthday;
            _dt_OriginalDiagnosisDate = originalDiagnosisDay;
        }

        DateTime _dt_First_date = DateTime.MinValue;
        public DateTime _dt_OriginalDiagnosisDate;
        public DateTime _dt_Birthday;
        public string str_First_Date
        {
            get
            {
                if (_dt_First_date != DateTime.MinValue)
                {
                    return _dt_First_date.DatetimeToFormatString();
                }
                return "";
            }
            set
            {
                if (value.StringToDatetime() < _dt_First_date || _dt_First_date == DateTime.MinValue)
                {
                    _dt_First_date = value.StringToDatetime();
                }
            }
        }
        public string str_postEventTime
        {
            get
            {
                if (_dt_First_date != DateTime.MinValue)
                    return Math.Round((double)_dt_First_date.Subtract(_dt_OriginalDiagnosisDate).Days / 365, 1).ToString();
                return "";
            }
        }
        public string str_Dx_age
        {
            get
            {
                if (_dt_First_date != DateTime.MinValue)
                    return Math.Round((double)_dt_First_date.Subtract(_dt_Birthday).Days / 365, 1).ToString();
                return "";
            }
        }
    }
}

// 存放 all ID table用
class PatientBasedData_ID : IComparable
{
    // --基本資料
    string _str_ID;
    public string str_ID
    {
        get { return _str_ID; }
        set
        {
            if (value.Length > 32)
            {
                _str_ID = value.Substring(0, 32);
            }
            else
            {
                _str_ID = value;
            }
        }
    }
    DateTime _dt_Birthday = DateTime.MinValue;
    public string str_Birthday
    {
        get { return _dt_Birthday.DatetimeToFormatString(); }
        set
        {
            string modify;
            modify = value.Substring(0, value.Length - 2) + "01";
            _dt_Birthday = modify.StringToDatetime();
        }
    }
    public string str_Gender { get; set; }
    // --保險時間
    DateTime _dt_InsuranceFisrtStartDate = DateTime.MinValue;
    public string str_InsuranceFirstStartDate
    {
        get { return _dt_InsuranceFisrtStartDate.DatetimeToFormatString(); }
        set
        {
            _dt_InsuranceFisrtStartDate = value.StringToDatetime();
        }
    }
    DateTime _dt_InsuranceLastStartDate = DateTime.MinValue;
    public string str_InsuranceLastStartDate
    {
        get { return _dt_InsuranceLastStartDate.DatetimeToFormatString(); }
        set
        {
            _dt_InsuranceLastStartDate = value.StringToDatetime();
        }
    }
    DateTime _dt_InsuranceEndDate = DateTime.MinValue;
    public string str_InsuranceEndDate
    {
        get { return _dt_InsuranceEndDate.DatetimeToFormatString(); }
        set
        {
            if (value.StringToDatetime() == DateTime.Parse("1900-01-01"))
            {
                _dt_InsuranceEndDate = DateTime.MaxValue;
            }
            else
            {
                _dt_InsuranceEndDate = value.StringToDatetime();
            }
        }
    }
    // --在資料庫時間
    public double db_data_start_age(int int_data_start_year, int int_data_end_year)
    {
        //此病人在資料庫開始的時間(start_time)為 Max(設定的資料庫最早時間, 病人最早出現時間)
        DateTime dt_data_start_time = DateTime.Parse(int_data_start_year + "-01-01");
        DateTime dt_data_end_time = DateTime.Parse((int_data_end_year + 1) + "-01-01");
        //無交集 傳回-1 =>  最晚時間<data start time或 最早時間>data end time
        if ((_dt_InsuranceLastStartDate < dt_data_start_time &&
            _dt_InsuranceEndDate < dt_data_start_time) ||
            _dt_InsuranceFisrtStartDate > dt_data_end_time)
            return -1;
        //有交集, 取data start time及ins. first date較晚的
        DateTime dt_start_time = _dt_InsuranceFisrtStartDate > dt_data_start_time ?
            _dt_InsuranceFisrtStartDate : dt_data_start_time;
        return Math.Round((double)dt_start_time.Subtract(_dt_Birthday).Days / 365, 1);
    }
    public double db_data_end_age(int int_data_start_year, int int_data_end_year)
    {
        //此病人在資料庫的結束時間(end_time)為 Min(設定的資料庫最晚時間, 病人最晚出現時間)
        DateTime dt_data_start_time = DateTime.Parse(int_data_start_year + "-01-01");
        DateTime dt_data_end_time = DateTime.Parse((int_data_end_year + 1) + "-01-01");
        //無交集 傳回-1 =>  最晚時間<data start time或 最早時間>data end time
        if ((_dt_InsuranceLastStartDate < dt_data_start_time &&
            _dt_InsuranceEndDate < dt_data_start_time) ||
            _dt_InsuranceFisrtStartDate > dt_data_end_time)
            return -1;
        //有交集, 假使ins last start day > ins end day => 最後一筆為加保 => end time = data end time
        //假使 ins last start day <= ins end day => 最後一筆為退保 
        //                                    => end time = ins end day 及 data end time較早的
        DateTime dt_end_time;
        if (_dt_InsuranceLastStartDate >= _dt_InsuranceEndDate)
        {
            dt_end_time = dt_data_end_time;
        }
        else
        {
            dt_end_time = _dt_InsuranceEndDate < dt_data_end_time ?
                _dt_InsuranceEndDate : dt_data_end_time;
        }
        return Math.Round((double)dt_end_time.Subtract(_dt_Birthday).Days / 365, 1);
    }
    public bool IsPatientAvailable(int int_data_start_year, int int_data_end_year)
    {
        if (db_data_start_age(int_data_start_year, int_data_end_year) >= 0
        && db_data_end_age(int_data_start_year, int_data_end_year) >= 0)
            return true;
        return false;
    }
    // --主診斷
    DateTime _dt_Primary_First_date = DateTime.MinValue;
    public string str_PrimaryDiagnosis_First_Date
    {
        get
        {
            if (_dt_Primary_First_date != DateTime.MinValue)
            {
                return _dt_Primary_First_date.DatetimeToFormatString();
            }
            return "";
        }
        set
        {
            if (value.StringToDatetime() < _dt_Primary_First_date || _dt_Primary_First_date == DateTime.MinValue)
            {
                _dt_Primary_First_date = value.StringToDatetime();
            }
        }
    }
    public double db_PrimaryDiagnosis_First_Age
    {
        get
        {
            var result = Math.Round((double)_dt_Primary_First_date.Subtract(_dt_Birthday).Days / 365, 1);
            return result >= 0 ? result : -1;
        }
    }
    public bool IsStudyGroup
    {
        get { return _dt_Primary_First_date > DateTime.MinValue; }
    }
    // --副診斷
    public Secondary_Diagnosis[] secondary_Diagnosis;
    public double get_secondary_Diagnosis_Event_Age(int index)
    {
        if (secondary_Diagnosis[index].IsSecondaryDiagnosis)
        {
            var result = Math.Round((double)secondary_Diagnosis[index]._dt_Event_Time
                .Subtract(_dt_Birthday).Days / 365, 1);
            return result >= 0 ? result : -1;
        }
        return -1;
    }

    // -- Constructor
    public PatientBasedData_ID(string str_ID, string str_Birthday, List<SecondDiagnosisGroup> second_Diagnosis_Group)
    {
        this.str_ID = str_ID; this.str_Birthday = str_Birthday;
        secondary_Diagnosis = new Secondary_Diagnosis[second_Diagnosis_Group.Count()];
        for (int i = 0; i < secondary_Diagnosis.Length; i++)
        {
            secondary_Diagnosis[i] = new Secondary_Diagnosis();
            secondary_Diagnosis[i].str_Group_Name = second_Diagnosis_Group[i].str_name;
        }
    }
    public PatientBasedData_ID(string str_ID, string str_Birthday) //比較用
    {
        this.str_ID = str_ID; this.str_Birthday = str_Birthday;
    }
    // 實作IComparable
    int IComparable.CompareTo(object obj)
    {
        var that = obj as PatientBasedData_ID;
        return (this.str_ID + this.str_Birthday.Replace("-", "").Substring(0, 6)).CompareTo(that.str_ID + that.str_Birthday.Replace("-", "").Substring(0, 6));
    }

    // -- 存放Secondary_Diagnosis_Event_Time
    public class Secondary_Diagnosis
    {
        public string str_Group_Name;
        public DateTime _dt_Event_Time = DateTime.MinValue;
        public string str_Event_Time
        {
            get
            {
                if (_dt_Event_Time != DateTime.MinValue)
                {
                    return _dt_Event_Time.DatetimeToFormatString();
                }
                return "";
            }
            set
            {
                if (value.StringToDatetime() < _dt_Event_Time || _dt_Event_Time == DateTime.MinValue)
                {
                    _dt_Event_Time = value.StringToDatetime();
                }
            }
        }
        public bool IsSecondaryDiagnosis
        {
            get { return _dt_Event_Time > DateTime.MinValue; }
        }
    }
}

// -- 存放Summarized Paitent Data
class SumData
{
    public double db_Age_Lower_Limit;
    public double db_Age_Upper_Limit;
    public int int_Female_count;
    public int int_Male_count;
    public int int_Total_count;
    public Second_Diagnosis_Count_data[] second_Diagnosis;
    public Second_Diagnosis_Count_data get_Second_Diagnosis_Count(int index)
    {
        second_Diagnosis[index].int_Female_denominotor = int_Female_count;
        second_Diagnosis[index].int_Male_denominotor = int_Male_count;
        second_Diagnosis[index].int_Total_denominotor = int_Total_count;
        return second_Diagnosis[index];
    }

    public SumData(List<SecondDiagnosisGroup> second_Diagnosis_Group)
    {
        second_Diagnosis = new Second_Diagnosis_Count_data[second_Diagnosis_Group.Count()];
        for (int i = 0; i < second_Diagnosis.Length; i++)
        {
            second_Diagnosis[i] = new Second_Diagnosis_Count_data();
            second_Diagnosis[i].str_Group_Name = second_Diagnosis_Group[i].str_name;
        }
    }
    // -- 在Summarized Paitent Data儲存各second Dx Count 以及計算Incidence
    public class Second_Diagnosis_Count_data
    {
        public string str_Group_Name;

        //當外部使用get存取子時，更新
        public int int_Female_denominotor;
        public int int_Male_denominotor;
        public int int_Total_denominotor;

        public int int_Female_count;
        public int int_Male_count;
        public int int_Total_count;

        public double db_Female_incidence
        {
            get { return (double)int_Female_count / (double)int_Female_denominotor; }
        }
        public double db_Male_incidence
        {
            get { return (double)int_Male_count / (double)int_Male_denominotor; }
        }
        public double db_Total_incidence
        {
            get { return (double)int_Total_count / (double)int_Total_denominotor; }
        }
    }
}


// -- 存放Age specific Indidence
class Age_Specific_Incidence
{
    //欄位設定
    public string str_group_Name { get; set; }
    public double db_age_lowerL { get; set; }
    public double db_age_upperL { get; set; }
    double _db_total_patient_year;
    public double db_total_patient_year
    {
        get { return _db_total_patient_year; }
        set { _db_total_patient_year = value; }
    }
    double _db_total_patient_year_Male;
    public double db_total_patient_year_Male
    {
        get { return _db_total_patient_year_Male; }
        set { _db_total_patient_year_Male = value; }
    }
    double _db_total_patient_year_Female;
    public double db_total_patient_year_Female
    {
        get { return _db_total_patient_year_Female; }
        set { _db_total_patient_year_Female = value; }
    }
    //主診斷
    public int int_primary_diagnosis_Event_count { get; set; }
    public int int_primary_diagnosis_Event_count_Male { get; set; }
    public int int_primary_diagnosis_Event_count_Female { get; set; }
    public double db_primary_diagnosis_ASI
    {
        get
        {
            if (db_total_patient_year == 0) return -1;
            return (double)int_primary_diagnosis_Event_count / db_total_patient_year;
        }
    }
    public double db_primary_diagnosis_ASI_Male
    {
        get
        {
            if (db_total_patient_year_Male == 0) return -1;
            return (double)int_primary_diagnosis_Event_count_Male / db_total_patient_year_Male;
        }
    }
    public double db_primary_diagnosis_ASI_Female
    {
        get
        {
            if (db_total_patient_year_Female == 0) return -1;
            return (double)int_primary_diagnosis_Event_count_Female / db_total_patient_year_Female;
        }
    }
    //次診斷
    public Secondary_Diagnosis_Event_Count[] Secondary_Diagnosis;
    public Secondary_Diagnosis_Event_Count[] Secondary_Diagnosis_Male;
    public Secondary_Diagnosis_Event_Count[] Secondary_Diagnosis_Female;
    public double get_Secondary_diagnosis_ASI(int index)
    {
        if (_db_total_patient_year == 0) return -1;
        return (double)Secondary_Diagnosis[index].int_Event_count / _db_total_patient_year;
    }
    public double get_Secondary_diagnosis_ASI_Male(int index)
    {
        if (_db_total_patient_year_Male == 0) return -1;
        return (double)Secondary_Diagnosis_Male[index].int_Event_count / _db_total_patient_year_Male;
    }
    public double get_Secondary_diagnosis_ASI_Female(int index)
    {
        if (_db_total_patient_year_Female == 0) return -1;
        return (double)Secondary_Diagnosis_Female[index].int_Event_count / _db_total_patient_year_Female;
    }
    //constructor
    public Age_Specific_Incidence(string str_group_Name, double db_age_lowerL, double db_age_upperL, List<SecondDiagnosisGroup> second_Diagnosis_Group)
    {
        this.str_group_Name = str_group_Name;
        this.db_age_lowerL = db_age_lowerL;
        this.db_age_upperL = db_age_upperL;
        Secondary_Diagnosis = new Secondary_Diagnosis_Event_Count[second_Diagnosis_Group.Count()];
        Secondary_Diagnosis_Male = new Secondary_Diagnosis_Event_Count[second_Diagnosis_Group.Count()];
        Secondary_Diagnosis_Female = new Secondary_Diagnosis_Event_Count[second_Diagnosis_Group.Count()];
        for (int i = 0; i < Secondary_Diagnosis.Length; i++)
        {
            Secondary_Diagnosis[i] = new Secondary_Diagnosis_Event_Count(second_Diagnosis_Group[i]);
            Secondary_Diagnosis_Male[i] = new Secondary_Diagnosis_Event_Count(second_Diagnosis_Group[i]);
            Secondary_Diagnosis_Female[i] = new Secondary_Diagnosis_Event_Count(second_Diagnosis_Group[i]);
        }

    }
}
// -- Secondary_Diagnosis_Event_Count for ASI
class Secondary_Diagnosis_Event_Count
{
    public string str_Group_Name;
    public int int_Event_count { get; set; }
    public Secondary_Diagnosis_Event_Count(SecondDiagnosisGroup second_dxgroup)
    {
        this.str_Group_Name = second_dxgroup.str_name;
    }
}

// -- SIR type
class SIR
{
    public string str_group_name { get; set; }
    public int int_observed { get; set; }
    public int int_observed_Male { get; set; }
    public int int_observed_Female { get; set; }
    public double db_estimated { get; set; }
    public double db_estimated_Male { get; set; }
    public double db_estimated_Female { get; set; }
    public double db_SIR
    {
        get
        {
            if (db_estimated == 0) return -1;
            return (double)int_observed / db_estimated;
        }
    }
    public double db_SIR_Male
    {
        get
        {
            if (db_estimated_Male == 0) return -1;
            return (double)int_observed_Male / db_estimated_Male;
        }
    }
    public double db_SIR_Female
    {
        get
        {
            if (db_estimated_Female == 0) return -1;
            return (double)int_observed_Female / db_estimated_Female;
        }
    }
    public double db_SIR_95CI_L
    {
        get
        {
            if (db_SIR <= 0) return -1;
            if (int_observed < 100)
                return getPoissonDistribution.LowerLimit(int_observed) / db_estimated;
            return db_SIR * Math.Pow(
                (
                1 -
                1 / (9 * ((double)int_observed)) -
                1.96 / (3 * Math.Pow((double)int_observed, 0.5))
                ), 3);
        }
    }
    public double db_SIR_95CI_L_Male
    {
        get
        {
            if (db_SIR_Male <= 0) return -1;
            if (int_observed_Male < 100)
                return getPoissonDistribution.LowerLimit(int_observed_Male) / db_estimated_Male;
            return db_SIR_Male * Math.Pow(
                (
                1 -
                1 / (9 * ((double)int_observed_Male)) -
                1.96 / (3 * Math.Pow((double)int_observed_Male, 0.5))
                ), 3);
        }
    }
    public double db_SIR_95CI_L_Female
    {
        get
        {
            if (db_SIR_Female <= 0) return -1;
            if (int_observed_Female < 100)
                return getPoissonDistribution.LowerLimit(int_observed_Female) / db_estimated_Female;
            return db_SIR_Female * Math.Pow(
                (
                1 -
                1 / (9 * ((double)int_observed_Female)) -
                1.96 / (3 * Math.Pow((double)int_observed_Female, 0.5))
                ), 3);
        }
    }
    public double db_SIR_95CI_U
    {
        get
        {
            if (db_SIR <= 0) return -1;
            if (int_observed < 100)
                return getPoissonDistribution.UpperLimit(int_observed) / db_estimated;
            return db_SIR * ((int_observed + 1) / int_observed) *
                Math.Pow(
                  (
                  1 -
                  1 / (9 * ((double)int_observed) + 1) +
                  1.96 / (3 * Math.Pow((double)int_observed + 1, 0.5))
                  )
              , 3);
        }
    }
    public double db_SIR_95CI_U_Male
    {
        get
        {
            if (db_SIR_Male <= 0) return -1;
            if (int_observed_Male < 100)
                return getPoissonDistribution.UpperLimit(int_observed_Male) / db_estimated_Male;
            return db_SIR_Male * ((int_observed_Male + 1) / int_observed_Male) *
                Math.Pow(
                  (
                  1 -
                  1 / (9 * ((double)int_observed_Male) + 1) +
                  1.96 / (3 * Math.Pow((double)int_observed_Male + 1, 0.5))
                  )
              , 3);
        }
    }
    public double db_SIR_95CI_U_Female
    {
        get
        {
            if (db_SIR_Female <= 0) return -1;
            if (int_observed_Female < 100)
                return getPoissonDistribution.UpperLimit(int_observed_Female) / db_estimated_Female;
            return db_SIR_Female * ((int_observed_Female + 1) / int_observed_Female) *
                Math.Pow(
                  (
                  1 -
                  1 / (9 * ((double)int_observed_Female) + 1) +
                  1.96 / (3 * Math.Pow((double)int_observed_Female + 1, 0.5))
                  )
              , 3);
        }
    }
    public string str_SIR_95CI
    {
        get
        {
            if (db_SIR <= 0) return "";
            return db_SIR_95CI_L.Round(2) + "-" + db_SIR_95CI_U.Round(2);
        }
    }
    public string str_SIR_95CI_Male
    {
        get
        {
            if (db_SIR_Male <= 0) return "";
            return db_SIR_95CI_L_Male.Round(2) + "-" + db_SIR_95CI_U_Male.Round(2);
        }
    }
    public string str_SIR_95CI_Female
    {
        get
        {
            if (db_SIR_Female <= 0) return "";
            return db_SIR_95CI_L_Female.Round(2) + "-" + db_SIR_95CI_U_Female.Round(2);
        }
    }
    public bool IsSignificant
    {
        get
        {
            if (db_SIR <= 0) return false;
            if (db_SIR_95CI_L > 1 || db_SIR_95CI_U < 1) return true;
            return false;
        }
    }
    public bool IsSignificant_Male
    {
        get
        {
            if (db_SIR_Male <= 0) return false;
            if (db_SIR_95CI_L_Male > 1 || db_SIR_95CI_U_Male < 1) return true;
            return false;
        }
    }
    public bool IsSignificant_Female
    {
        get
        {
            if (db_SIR_Female <= 0) return false;
            if (db_SIR_95CI_L_Female > 1 || db_SIR_95CI_U_Female < 1) return true;
            return false;
        }
    }
}
    // -- 存放診斷條件之類別



}
