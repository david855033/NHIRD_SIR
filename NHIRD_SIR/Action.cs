using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NHIRD.DataTypes;
using NHIRD.Extensions;
using NHIRD.PoissonDistribution;
namespace NHIRD.Actions
{
    static class GroupDefinition
    {
        /// <summary>
        /// 載入 Order群組定義
        /// </summary>
        /// <param name="Args"></param>
        public static void initiallizeOrder(NHIRD_ArgTable Args)
        {
            Console.WriteLine("Call: initiallizeOrder");
            Args.list_OrderGroupDefinision = new List<OrderGroup>();
            Args.list_OrderGroupDefinision.Add(new OrderGroup("Drug Group 1",
                new List<string>() { "DrugCode1", "DrugCode2" }));
            Console.WriteLine("Return: {0} groups of orders were loaded.\r\n", Args.list_OrderGroupDefinision.Count);
        }
    }
    static class GetActionBasedData
    {
        public static void CD(List<List<NHIRD_DataTypes.ActionBasedData_CD>> ActionBasedData_CD, string str_CDpath, int int_DataGroupCount)
        {
            Console.WriteLine("Call: GetActionBasedData.CD");
            Console.WriteLine(" - CD path: {0}", str_CDpath);
            ActionBasedData_CD = new List<List<NHIRD_DataTypes.ActionBasedData_CD>>();

            //讀取CD檔
            int errorCount = 0;
            using (var sr = new StreamReader(str_CDpath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_FEE_YM_index = title.FindIndex(x => x.IndexOf("FEE_YM") >= 0);
                int int_HOSP_ID_index = title.FindIndex(x => x.IndexOf("HOSP_ID") >= 0);
                int int_APPL_DATE_index = title.FindIndex(x => x.IndexOf("APPL_DATE") >= 0);
                int int_SEQ_NO_index = title.FindIndex(x => x.IndexOf("SEQ_NO") >= 0);
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_FuncDate_index = title.FindIndex(x => x.IndexOf("FUNC_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ACODE_ICD9_1") >= 0);
                int int_FuncType_index = title.FindIndex(x => x.IndexOf("FUNC_TYPE") >= 0);
                // -- streamreader 迴圈
                int datacount = 0;
                while (!sr.EndOfStream)
                {
                    string[] SplitLine = sr.ReadLine().Split('\t');
                    NHIRD_DataTypes.ActionBasedData_CD DataToAdd = new NHIRD_DataTypes.ActionBasedData_CD(int_DataGroupCount);
                    DataToAdd.str_Fee_YM = SplitLine[int_FEE_YM_index];
                    DataToAdd.str_HospID = SplitLine[int_HOSP_ID_index];
                    DataToAdd.str_ApplDate = SplitLine[int_APPL_DATE_index];
                    DataToAdd.str_SeqNO = SplitLine[int_SEQ_NO_index];
                    DataToAdd.str_ID = SplitLine[int_ID_index];
                    DataToAdd.str_Birthday = SplitLine[int_Birthday_index];
                    DataToAdd.str_FuncDate = SplitLine[int_FuncDate_index];
                    DataToAdd.str_Gender = SplitLine[int_Gender_index];
                    DataToAdd.array_ICD = new string[] { SplitLine[int_ICD_index], SplitLine[int_ICD_index + 1], SplitLine[int_ICD_index + 2] };
                    DataToAdd.str_FuncType = SplitLine[int_FuncType_index];
                    //  使用ActionBasedData_OrderComparer 依序存入ActionBasedData List
                    int SearchIndex = ActionBasedData_CD.BinarySearch(
                       DataToAdd, new NHIRD_DataTypes.ActionBasedData_OrderComparer());
                    if (SearchIndex < 0)
                    {
                        ActionBasedData_CD.Insert(-SearchIndex - 1, DataToAdd);
                        datacount++;
                        if (datacount % 10000 == 0)
                            Console.WriteL("Data Readed: {0}\r", datacount);
                    }
                    else { errorCount++; }
                }
            }
            Console.WriteLine("\nRetrun: {0} Action based data was loaded, error count: {1}. \r\n", ActionBasedData_CD.Count(), errorCount);
        }

    }
    static class action
    {
        // -- 初始化 診斷群組
        public static void Initialize_Secondary_Diagnosis(List<SecondDiagnosisGroup> list_second_Diagnosis_Group)
        {
            list_second_Diagnosis_Group.Add(new SecondDiagnosisGroup
            {
                str_name = "Asthma",
                list_ICD9 = new List<string>() { "493" }
            });

            list_second_Diagnosis_Group.Add(new SecondDiagnosisGroup
            {
                str_name = "Atopic dermatitis",
                list_ICD9 = new List<string>() { "691" }
            });

            list_second_Diagnosis_Group.Add(new SecondDiagnosisGroup
            {
                str_name = "Allergic Rhinitis",
                list_ICD9 = new List<string>() { "477" }
            });
        }
        // -- 載入patient based data
        public static void Generate_patient_based_data(List<PatientBasedData> List_PatientBasedData, int int_second_diagnosis_group_count, string str_FilePath)
        {
            using (var sr = new StreamReader(str_FilePath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_FuncDate_index = title.FindIndex(x => x.IndexOf("FUNC_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ACODE_ICD9_1") >= 0);
                int int_FuncType_index = title.FindIndex(x => x.IndexOf("FUNC_TYPE") >= 0);
                // -- Genital wart之診斷
                var ICDcrieteria_GenitalWart = new string[] { "07811" };
                var ICDcriteria_Wart = new string[] { "0781" };
                // -- streamreader 迴圈
                while (sr.Peek() >= 0)
                {
                    string[] str_currentline_split = sr.ReadLine().Split('\t');
                    string str_currentID = str_currentline_split[int_ID_index];
                    string str_currentBirthday = str_currentline_split[int_Birthday_index];
                    string str_currentFuncDate = str_currentline_split[int_FuncDate_index];
                    if (Convert.ToInt32(str_currentFuncDate.Substring(0, 4)) < 2000) continue;  //***年份限制
                    string str_currentGender = str_currentline_split[int_Gender_index];
                    string str_currentFuncType = str_currentline_split[int_FuncType_index];
                    string[] str_currentICDs = new string[] { str_currentline_split[int_ICD_index], str_currentline_split[int_ICD_index + 1], str_currentline_split[int_ICD_index + 2] };
                    // ** 搜尋
                    int int_BinarySearchResult = List_PatientBasedData.BinarySearch(new PatientBasedData { str_ID = str_currentID, str_Birthday = str_currentBirthday });
                    if (int_BinarySearchResult < 0)   //新資料
                    {
                        var new_PatientBasedData = new PatientBasedData
                        {
                            str_ID = str_currentID,
                            str_Birthday = str_currentBirthday,
                            str_Gender = str_currentGender,
                            str_FirstDate = str_currentFuncDate,
                            array_secondDiagnosis = new PatientBasedData.SecondDiagnosis[int_second_diagnosis_group_count]
                        };
                        List_PatientBasedData.Insert(-1 * int_BinarySearchResult - 1, new_PatientBasedData);
                    }
                    else   //舊資料
                    {
                        List_PatientBasedData[int_BinarySearchResult].str_FirstDate = str_currentFuncDate; //更新最早日期
                        // 計算門診次數
                        if (check_ICD(ICDcrieteria_GenitalWart, str_currentICDs))
                        {
                            List_PatientBasedData[int_BinarySearchResult].int_GW_OPDTimes++;
                        }
                        else if (check_ICD(ICDcriteria_Wart, str_currentICDs) && (str_currentFuncType == "08" || str_currentFuncType == "05"))
                        {
                            List_PatientBasedData[int_BinarySearchResult].int_GYN_GU_WART_OPDTimes++;
                        }
                        else
                        {
                            Console.WriteLine("error"); Console.ReadKey();
                        }
                    }
                }
            }
            // -- 建立PatientBasedData內 secondary dianosis之實體物件
            foreach (var pt in List_PatientBasedData)
            {
                for (int i = 0; i < int_second_diagnosis_group_count; i++)
                {
                    pt.array_secondDiagnosis[i] =
                        new PatientBasedData.SecondDiagnosis(Convert.ToDateTime(pt.str_Birthday), Convert.ToDateTime(pt.str_FirstDate));
                }
            }

        }
        // -- 使用在DD抓 2nd 診斷
        public static void Check_Secondary_Diagnosis(List<SecondDiagnosisGroup> list_second_Diagnosis_Group, List<PatientBasedData> List_PatientBasedData, string str_FilePath)
        {
            using (var sr = new StreamReader(str_FilePath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_InDate_index = title.FindIndex(x => x.IndexOf("IN_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ICD9CM_CODE") >= 0);
                // -- sr之迴圈
                while (sr.Peek() >= 0)
                {
                    // 建立該行資料
                    string[] str_currentline_split = sr.ReadLine().Split('\t');
                    string str_currentID = str_currentline_split[int_ID_index];
                    string str_currentBirthday = str_currentline_split[int_Birthday_index];
                    string str_currentInDate = str_currentline_split[int_InDate_index];
                    string str_currentGender = str_currentline_split[int_Gender_index];
                    string[] str_currentICDs = new string[] { str_currentline_split[int_ICD_index],
                        str_currentline_split[int_ICD_index + 1], str_currentline_split[int_ICD_index + 2],
                        str_currentline_split[int_ICD_index + 3], str_currentline_split[int_ICD_index + 4] };

                    int int_BinarySearchResult = List_PatientBasedData.BinarySearch(new PatientBasedData { str_ID = str_currentID, str_Birthday = str_currentBirthday });

                    //各組次要診斷的迴圈
                    for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                    {
                        //   核對該組ICD  符合的話更新病人的第一次診斷時間
                        if (int_BinarySearchResult >= 0 && check_ICD(list_second_Diagnosis_Group[i].list_ICD9, str_currentICDs))
                        {
                            List_PatientBasedData[int_BinarySearchResult].array_secondDiagnosis[i].str_First_Date = str_currentInDate;
                        }
                    }
                }
            }
        }
        // -- 取得年紀分層各組人數
        public static void Summarize_Patient_AgeGrouped(List<PatientBasedData> List_PatientBasedData,
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            List<SumData> list_summarized_Patient_data,
            double db_Age_Start_Age, double db_Age_Group_interval, int int_Age_Group_count)
        {
            //使用list_second_Diagnosis_Group 及 age internval初始化 Summarized_Patient_data_Group

            for (int i = 0; i < int_Age_Group_count; i++)
            {
                list_summarized_Patient_data.Add(new SumData(list_second_Diagnosis_Group));
                list_summarized_Patient_data[i].db_Age_Lower_Limit = db_Age_Group_interval * i + db_Age_Start_Age;
                list_summarized_Patient_data[i].db_Age_Upper_Limit = db_Age_Group_interval * (i + 1) + db_Age_Start_Age;
            }

            //開始計算
            foreach (var currentPatientBaseddata in List_PatientBasedData)
            {
                for (int i = 0; i < list_summarized_Patient_data.Count; i++)
                {
                    if (currentPatientBaseddata.db_FirstAge < list_summarized_Patient_data[i].db_Age_Upper_Limit
                        || i == list_summarized_Patient_data.Count - 1)  //年齡分組
                    {
                        //主診斷人數
                        list_summarized_Patient_data[i].int_Total_count++;
                        if (currentPatientBaseddata.str_Gender == "F")
                        {
                            list_summarized_Patient_data[i].int_Female_count++;
                        }
                        else if (currentPatientBaseddata.str_Gender == "M")
                        {
                            list_summarized_Patient_data[i].int_Male_count++;
                        }
                        //次診斷人數
                        for (int j = 0; j < list_second_Diagnosis_Group.Count; j++)
                        {
                            if (currentPatientBaseddata.array_secondDiagnosis[j].str_postEventTime.Length > 0
                                && Convert.ToDouble(currentPatientBaseddata.array_secondDiagnosis[j].str_postEventTime) > 0)
                            {
                                list_summarized_Patient_data[i].second_Diagnosis[j].int_Total_count++;
                                if (currentPatientBaseddata.str_Gender == "F")
                                {
                                    list_summarized_Patient_data[i].second_Diagnosis[j].int_Female_count++;
                                }
                                else if (currentPatientBaseddata.str_Gender == "M")
                                {
                                    list_summarized_Patient_data[i].second_Diagnosis[j].int_Male_count++;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        // -- 輸出Patient Based Data
        public static void Generate_Report(List<PatientBasedData> list_PatientBasedData, List<SecondDiagnosisGroup> list_second_Diagnosis_Group, string str_path)
        {
            using (var sw = new StreamWriter(str_path))
            {
                string title = "ID\tBirthday\tGender\t初診日期\t初診年齡\t07811 OPD times\t0781婦產泌尿門診 OPD times";
                foreach (var secondDx in list_second_Diagnosis_Group)
                {
                    title += "\t" + secondDx.str_name + ": FirstDate";
                }
                foreach (var secondDx in list_second_Diagnosis_Group)
                {
                    title += "\t" + secondDx.str_name + ": Age";
                }
                foreach (var secondDx in list_second_Diagnosis_Group)
                {
                    title += "\t" + secondDx.str_name + ": PostEventTime";
                }
                sw.WriteLine(title);
                foreach (var pt in list_PatientBasedData)
                {
                    string linetowrite = pt.str_ID + "\t";
                    linetowrite += pt.str_Birthday + "\t";
                    linetowrite += pt.str_Gender + "\t";
                    linetowrite += pt.str_FirstDate + "\t";
                    linetowrite += pt.db_FirstAge + "\t";
                    linetowrite += pt.int_GW_OPDTimes + "\t";
                    linetowrite += pt.int_GYN_GU_WART_OPDTimes;
                    for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                    {
                        linetowrite += "\t" + pt.array_secondDiagnosis[i].str_First_Date;
                    }
                    for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                    {
                        linetowrite += "\t" + pt.array_secondDiagnosis[i].str_Dx_age;
                    }
                    for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                    {
                        linetowrite += "\t" + pt.array_secondDiagnosis[i].str_postEventTime;
                    }
                    sw.WriteLine(linetowrite);
                }
                sw.Flush();
            }
        }
        // --輸出Summarized Data
        public static void Generate_Summary(List<SumData> list_summarized_Patient_data, List<SecondDiagnosisGroup> list_second_Diagnosis_Group, string str_path)
        {
            using (var sw = new StreamWriter(str_path))
            {
                string title = "Patient Char.\t\t\t\t";
                string subtitle = "Age下界\tAge上界\tF\tM\tAll";
                foreach (var secondDxGroup in list_second_Diagnosis_Group)
                {
                    title += "\t" + secondDxGroup.str_name + "\t\t\tIncidence\t\t";
                    subtitle += "\tF\tM\tAll\tF\tM\tAll";
                }
                sw.WriteLine(title);
                sw.WriteLine(subtitle);
                int int_Female_count_sum = 0, int_Male_count_sum = 0, int_Total_count_sum = 0;
                foreach (var current_Summarized_Patient_data in list_summarized_Patient_data)
                {
                    string line = current_Summarized_Patient_data.db_Age_Lower_Limit + "\t" + current_Summarized_Patient_data.db_Age_Upper_Limit;
                    line += "\t" + current_Summarized_Patient_data.int_Female_count.NullforZero();
                    line += "\t" + current_Summarized_Patient_data.int_Male_count.NullforZero();
                    line += "\t" + current_Summarized_Patient_data.int_Total_count.NullforZero();
                    int_Female_count_sum += current_Summarized_Patient_data.int_Female_count;   //SUM尚未完成
                    int_Male_count_sum += current_Summarized_Patient_data.int_Male_count;
                    int_Total_count_sum += current_Summarized_Patient_data.int_Total_count;
                    for (int i = 0; i < current_Summarized_Patient_data.second_Diagnosis.Count(); i++)
                    {
                        line += "\t" + current_Summarized_Patient_data.second_Diagnosis[i].int_Female_count.NullforZero();
                        line += "\t" + current_Summarized_Patient_data.second_Diagnosis[i].int_Male_count.NullforZero();
                        line += "\t" + current_Summarized_Patient_data.second_Diagnosis[i].int_Total_count.NullforZero();
                        line += "\t" + current_Summarized_Patient_data.get_Second_Diagnosis_Count(i).db_Female_incidence.PercentFormat(2);
                        line += "\t" + current_Summarized_Patient_data.get_Second_Diagnosis_Count(i).db_Male_incidence.PercentFormat(2);
                        line += "\t" + current_Summarized_Patient_data.get_Second_Diagnosis_Count(i).db_Total_incidence.PercentFormat(2);
                    }
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
        // -- 載入all patient總表
        public static void Load_All_Patient(List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            List<PatientBasedData_ID> List_PatientBasedData, string str_path)
        {
            using (var sr = new StreamReader(str_path))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("Birthday") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("Sex") >= 0);
                int int_First_Start_Date_index = title.FindIndex(x => x.IndexOf("First Start Date") >= 0);
                int int_Last_Start_Date_index = title.FindIndex(x => x.IndexOf("Last Start Date") >= 0);
                int int_Last_End_Date_index = title.FindIndex(x => x.IndexOf("Last End Date") >= 0);
                //載入資料
                while (sr.Peek() >= 0)
                {
                    var currentlinesplit = sr.ReadLine().Split('\t');
                    var newPatientBasedData = new PatientBasedData_ID(currentlinesplit[int_ID_index], currentlinesplit[int_Birthday_index], list_second_Diagnosis_Group);
                    newPatientBasedData.str_Gender = currentlinesplit[int_Gender_index];
                    newPatientBasedData.str_InsuranceFirstStartDate = currentlinesplit[int_First_Start_Date_index];
                    newPatientBasedData.str_InsuranceLastStartDate = currentlinesplit[int_Last_Start_Date_index];
                    newPatientBasedData.str_InsuranceEndDate = currentlinesplit[int_Last_End_Date_index];
                    var index = List_PatientBasedData.BinarySearch(newPatientBasedData);
                    if (index < 0)
                    {
                        List_PatientBasedData.Insert(-index - 1, newPatientBasedData);
                    }
                    if (List_PatientBasedData.Count() % 10000 == 0)
                        Console.Write("\r - patients: {0} loaded", List_PatientBasedData.Count());
                }
                Console.WriteLine("\r - patients: {0} loaded", List_PatientBasedData.Count());
            }
        }
        // -- 將all-patient 加入 primary diagnosis
        public static void Load_PrimaryDiagnosis_to_All_Patient(
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            List<PatientBasedData_ID> List_PatientBasedData,
            string str_FilePath)
        {
            using (var sr = new StreamReader(str_FilePath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_FuncDate_index = title.FindIndex(x => x.IndexOf("FUNC_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ACODE_ICD9_1") >= 0);
                int int_FuncType_index = title.FindIndex(x => x.IndexOf("FUNC_TYPE") >= 0);
                // -- streamreader 迴圈
                var int_error_count = 0;
                while (sr.Peek() >= 0)
                {
                    string[] str_currentline_split = sr.ReadLine().Split('\t');
                    string str_currentID = str_currentline_split[int_ID_index];
                    string str_currentBirthday = str_currentline_split[int_Birthday_index];
                    string str_currentFuncDate = str_currentline_split[int_FuncDate_index];
                    if (Convert.ToInt32(str_currentFuncDate.Substring(0, 4)) < 2000) continue;  //***年份限制
                    if (Convert.ToInt32(str_currentBirthday.Substring(0, 4)) < 2000) continue;  //***年齡限制
                    string str_currentGender = str_currentline_split[int_Gender_index];
                    string str_currentFuncType = str_currentline_split[int_FuncType_index];
                    string[] str_currentICDs = new string[] { str_currentline_split[int_ICD_index], str_currentline_split[int_ICD_index + 1], str_currentline_split[int_ICD_index + 2] };
                    // ** 搜尋
                    int int_BinarySearchResult = List_PatientBasedData
                        .BinarySearch(new PatientBasedData_ID(str_currentID, str_currentBirthday));
                    if (int_BinarySearchResult < 0)   //查無資料(錯誤)
                    {
                        Console.Write("\rCan not find ID in All patient! :{0}", ++int_error_count);
                    }
                    else   //舊資料
                    {
                        List_PatientBasedData[int_BinarySearchResult]
                            .str_PrimaryDiagnosis_First_Date = str_currentFuncDate; //更新最早日期
                        List_PatientBasedData[int_BinarySearchResult].str_Gender = str_currentGender;//校正性別
                    }
                }
                Console.Write("\n");
            }
        }
        // -- 將all-patient 加入 Secondary diagnosis
        public static void Load_SecondaryDiagnosis_to_All_Patient(
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            List<PatientBasedData_ID> List_PatientBasedData,
            string str_FilePath
            )
        {
            using (var sr = new StreamReader(str_FilePath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_InDate_index = title.FindIndex(x => x.IndexOf("IN_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ICD9CM_CODE") >= 0);
                // -- sr之迴圈
                var int_error_count = 0;
                while (sr.Peek() >= 0)
                {
                    // 建立該行資料
                    string[] str_currentline_split = sr.ReadLine().Split('\t');
                    string str_currentID = str_currentline_split[int_ID_index];
                    string str_currentBirthday = str_currentline_split[int_Birthday_index];
                    string str_currentInDate = str_currentline_split[int_InDate_index];
                    if (Convert.ToInt32(str_currentInDate.Substring(0, 4)) < 2000) continue;  //***年份限制
                    if (Convert.ToInt32(str_currentBirthday.Substring(0, 4)) < 2000) continue;  //***年齡限制
                    string str_currentGender = str_currentline_split[int_Gender_index];
                    string[] str_currentICDs = new string[] { str_currentline_split[int_ICD_index],
                        str_currentline_split[int_ICD_index + 1], str_currentline_split[int_ICD_index + 2],
                        str_currentline_split[int_ICD_index + 3], str_currentline_split[int_ICD_index + 4] };

                    int int_BinarySearchResult = List_PatientBasedData
                        .BinarySearch(new PatientBasedData_ID(str_currentID, str_currentBirthday));
                    if (int_BinarySearchResult < 0)
                    {
                        Console.Write("\rCan not find ID in All patient! :{0}", ++int_error_count);
                    }
                    else
                    {
                        //各組次要診斷的迴圈
                        for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                        {
                            //   核對該組ICD  符合的話更新病人的第一次診斷時間
                            if (int_BinarySearchResult >= 0 && check_ICD(list_second_Diagnosis_Group[i].list_ICD9, str_currentICDs))
                            {
                                List_PatientBasedData[int_BinarySearchResult]
                                    .secondary_Diagnosis[i].str_Event_Time = str_currentInDate;
                            }
                        }
                    }
                }
                Console.Write('\n');
            }
        }
        // -- 將all-patient 加入 Secondary diagnosis
        public static void Load_SecondaryCD_to_All_Patient(
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            List<PatientBasedData_ID> List_PatientBasedData,
            string str_FilePath
            )
        {
            using (var sr = new StreamReader(str_FilePath))
            {
                // -- 取得欄位index
                List<string> title = new List<string>(sr.ReadLine().Split('\t'));
                int int_ID_index = title.FindIndex(x => x == "ID");
                int int_Birthday_index = title.FindIndex(x => x.IndexOf("ID_BIRTHDAY") >= 0);
                int int_InDate_index = title.FindIndex(x => x.IndexOf("FUNC_DATE") >= 0);
                int int_Gender_index = title.FindIndex(x => x.IndexOf("ID_SEX") >= 0);
                int int_ICD_index = title.FindIndex(x => x.IndexOf("ACODE_ICD9_1") >= 0);
                // -- sr之迴圈
                var int_error_count = 0;
                while (sr.Peek() >= 0)
                {
                    // 建立該行資料
                    string[] str_currentline_split = sr.ReadLine().Split('\t');
                    string str_currentID = str_currentline_split[int_ID_index];
                    string str_currentBirthday = str_currentline_split[int_Birthday_index];
                    string str_currentInDate = str_currentline_split[int_InDate_index];
                    if (Convert.ToInt32(str_currentInDate.Substring(0, 4)) < 2000) continue;  //***年份限制
                    string str_currentGender = str_currentline_split[int_Gender_index];
                    string[] str_currentICDs = new string[] { str_currentline_split[int_ICD_index],
                        str_currentline_split[int_ICD_index + 1], str_currentline_split[int_ICD_index + 2] };
                    var newPT = new PatientBasedData_ID(str_currentID, str_currentBirthday);
                    int int_BinarySearchResult = List_PatientBasedData
                        .BinarySearch(newPT);
                    if (int_BinarySearchResult < 0)
                    {
                        Console.Write("\rCan not find ID in All patient! :{0}", ++int_error_count);
                    }
                    else
                    {
                        //各組次要診斷的迴圈
                        for (int i = 0; i < list_second_Diagnosis_Group.Count; i++)
                        {
                            //   核對該組ICD  符合的話更新病人的第一次診斷時間
                            if (int_BinarySearchResult >= 0 && check_ICD(list_second_Diagnosis_Group[i].list_ICD9, str_currentICDs))
                            {
                                List_PatientBasedData[int_BinarySearchResult]
                                    .secondary_Diagnosis[i].str_Event_Time = str_currentInDate;
                            }
                        }
                    }
                }
                Console.Write('\n');
            }
        }
        // --輸出 All Patient data
        public static void Generate_All_Patient(List<PatientBasedData_ID> list_PatientBasedData,
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            string str_path, bool studyGroupOnly, int int_data_start_year, int int_data_end_year)
        {
            using (var sw = new StreamWriter(str_path))
            {
                var str_title = "ID\tBirthday\tGender\tData_start_age\tData_End_Age\tPrimary_Diagnosis_Age";
                foreach (var currentGroup in list_second_Diagnosis_Group)
                {
                    str_title += "\t" + currentGroup.str_name + ":Event Age";
                }
                sw.WriteLine(str_title);
                foreach (var CurrentPatient in list_PatientBasedData)
                {
                    if (studyGroupOnly && (!CurrentPatient.IsStudyGroup || !CurrentPatient.IsPatientAvailable(int_data_start_year, int_data_end_year))) continue;
                    var str_line = CurrentPatient.str_ID + "\t" + CurrentPatient.str_Birthday;
                    str_line += "\t" + CurrentPatient.str_Gender;
                    str_line += "\t" + CurrentPatient.db_data_start_age(int_data_start_year, int_data_end_year).NullforNeg();
                    str_line += "\t" + CurrentPatient.db_data_end_age(int_data_start_year, int_data_end_year).NullforNeg();
                    str_line += "\t" + CurrentPatient.db_PrimaryDiagnosis_First_Age.NullforNeg();
                    for (int i = 0; i < list_second_Diagnosis_Group.Count(); i++)
                    {
                        str_line += "\t" + CurrentPatient.get_secondary_Diagnosis_Event_Age(i).NullforNeg();
                    }
                    sw.WriteLine(str_line);
                }
                sw.Flush();
            }
        }
        // --計算Calculate_Age_Specific_Incidence
        public static void Calculate_Age_Specific_Incidence(
            List<Age_Specific_Incidence> list_Age_specific_incidence,
            List<PatientBasedData_ID> list_PatientBasedData,
            List<SecondDiagnosisGroup> list_second_Diagnosis_Group,
            bool IsStudyGroup,
            int int_data_start_year, int int_data_end_year
            )
        {
            //初始化 ASI陣列
            for (int i = 0; i < 100; i++)
            {
                list_Age_specific_incidence.Add(
                    new Age_Specific_Incidence(i.ToString(), i, i + 1, list_second_Diagnosis_Group));
            }
            //開始計算Patient Based Data
            int int_patient_count = 0;
            foreach (var PatientBasedData in list_PatientBasedData)
            {
                if ((IsStudyGroup && !PatientBasedData.IsStudyGroup))
                    continue; //檢查是否只需study group

                double db_start_age = PatientBasedData.db_data_start_age(int_data_start_year, int_data_end_year);
                double db_end_age = PatientBasedData.db_data_end_age(int_data_start_year, int_data_end_year);
                double db_PrimaryDiagnosisAge = PatientBasedData.db_PrimaryDiagnosis_First_Age;
                //study group排除診斷前的時間
                if (IsStudyGroup) db_start_age = Math.Max(db_PrimaryDiagnosisAge, db_start_age);
                foreach (var Age_Specific_incidence in list_Age_specific_incidence)
                {
                    //此ASI群組的年齡上界，大於病人資料開始年齡且下界小於病人資料結束年齡（有重疊）
                    //計算total patient year
                    if (Age_Specific_incidence.db_age_upperL > db_start_age &&
                        Age_Specific_incidence.db_age_lowerL <= db_end_age)
                    {
                        double db_upper_end =
                            Math.Min(Age_Specific_incidence.db_age_upperL, db_end_age);
                        double db_lower_end =
                            Math.Max(Age_Specific_incidence.db_age_lowerL, db_start_age);

                        double increment = db_upper_end - db_lower_end;

                        if (PatientBasedData.str_Gender == "F")
                        {
                            Age_Specific_incidence.db_total_patient_year_Female += increment;
                            Age_Specific_incidence.db_total_patient_year += increment;
                        }
                        else if (PatientBasedData.str_Gender == "M")
                        {
                            Age_Specific_incidence.db_total_patient_year_Male += increment;
                            Age_Specific_incidence.db_total_patient_year += increment;
                        }
                    }
                    else { continue; }
                    //計算主診斷
                    if (PatientBasedData.IsStudyGroup)  //是擁有主診斷的case
                    {
                        //判斷年齡組別是否正確
                        if (PatientBasedData.db_PrimaryDiagnosis_First_Age >= Age_Specific_incidence.db_age_lowerL
                            && PatientBasedData.db_PrimaryDiagnosis_First_Age < Age_Specific_incidence.db_age_upperL)
                        {
                            if (PatientBasedData.str_Gender == "F")
                            {
                                Age_Specific_incidence.int_primary_diagnosis_Event_count_Female++;
                                Age_Specific_incidence.int_primary_diagnosis_Event_count++;
                            }
                            else if (PatientBasedData.str_Gender == "M")
                            {
                                Age_Specific_incidence.int_primary_diagnosis_Event_count_Male++;
                                Age_Specific_incidence.int_primary_diagnosis_Event_count++;
                            }
                        }
                    }
                    //次診斷
                    for (int i = 0; i < Age_Specific_incidence.Secondary_Diagnosis.Length; i++)
                    {
                        double db_secondary_diagnosis_age = PatientBasedData.get_secondary_Diagnosis_Event_Age(i);
                        //是擁有次診斷的case   。  如果是study group，次診斷不能比主診斷早
                        if (PatientBasedData.secondary_Diagnosis[i].IsSecondaryDiagnosis
                            && !(IsStudyGroup && db_secondary_diagnosis_age < PatientBasedData.db_PrimaryDiagnosis_First_Age))
                        {
                            //判斷年齡組別是否正確
                            if (db_secondary_diagnosis_age >= Age_Specific_incidence.db_age_lowerL
                                && db_secondary_diagnosis_age < Age_Specific_incidence.db_age_upperL)
                            {
                                if (PatientBasedData.str_Gender == "F")
                                {
                                    Age_Specific_incidence.Secondary_Diagnosis_Female[i].int_Event_count++;
                                    Age_Specific_incidence.Secondary_Diagnosis[i].int_Event_count++;
                                }
                                else if (PatientBasedData.str_Gender == "M")
                                {
                                    Age_Specific_incidence.Secondary_Diagnosis_Male[i].int_Event_count++;
                                    Age_Specific_incidence.Secondary_Diagnosis[i].int_Event_count++;
                                }
                            }
                        }
                    }

                }
                int_patient_count++;
                if (int_patient_count % 10000 == 0 || int_patient_count == list_PatientBasedData.Count())
                    Console.Write(" - Analyzed patient: {0}\r", int_patient_count);
            }
            Console.Write("\n");
        }
        // --輸出ASI
        public static void Generate_ASI(List<Age_Specific_Incidence> list_Age_specific_incidence,
            List<SecondDiagnosisGroup> list_second_diagnosis_group,
            string str_path)
        {
            using (var sw = new StreamWriter(str_path))
            {
                string title = "Group Name";
                title += "\ttotal Patient-Year\tPatient-Year Male\tPatient-Year Female";
                title += "\tPrmiary:Event\tPrmiary:Event Male\tPrmiary:Event Female";
                title += "\tPrimary:ASI\tPrimary:ASI Male\tPrimary:ASI Female";
                foreach (var secondDx in list_second_diagnosis_group)
                {
                    title += "\t" + secondDx.str_name + ":Event\t" + secondDx.str_name + ":ASI";
                    title += "\t" + secondDx.str_name + ":Event Male\t" + secondDx.str_name + ":ASI Male";
                    title += "\t" + secondDx.str_name + ":Event Female\t" + secondDx.str_name + ":ASI Female";
                }
                sw.WriteLine(title);
                foreach (var ASI in list_Age_specific_incidence)
                {
                    string line = ASI.str_group_Name;
                    line += "\t" + ASI.db_total_patient_year.Round(1);
                    line += "\t" + ASI.db_total_patient_year_Male.Round(1);
                    line += "\t" + ASI.db_total_patient_year_Female.Round(1);
                    line += "\t" + ASI.int_primary_diagnosis_Event_count.NullforZero();
                    line += "\t" + ASI.int_primary_diagnosis_Event_count_Male.NullforZero();
                    line += "\t" + ASI.int_primary_diagnosis_Event_count_Female.NullforZero();
                    line += "\t" + ASI.db_primary_diagnosis_ASI.PercentFormat(4);
                    line += "\t" + ASI.db_primary_diagnosis_ASI_Male.PercentFormat(4);
                    line += "\t" + ASI.db_primary_diagnosis_ASI_Female.PercentFormat(4);
                    for (int i = 0; i < ASI.Secondary_Diagnosis.Length; i++)
                    {
                        line += "\t" + ASI.Secondary_Diagnosis[i].int_Event_count.NullforZero();
                        line += "\t" + ASI.get_Secondary_diagnosis_ASI(i).PercentFormat(4);
                        line += "\t" + ASI.Secondary_Diagnosis_Male[i].int_Event_count.NullforZero();
                        line += "\t" + ASI.get_Secondary_diagnosis_ASI_Male(i).PercentFormat(4);
                        line += "\t" + ASI.Secondary_Diagnosis_Female[i].int_Event_count.NullforZero();
                        line += "\t" + ASI.get_Secondary_diagnosis_ASI_Female(i).PercentFormat(4);
                    }
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
        // --計算SIR
        public static void Calculate_SIR(
            List<SIR> list_SIR,
            List<Age_Specific_Incidence> list_Age_specific_incidence_AllPt,
            List<Age_Specific_Incidence> list_Age_specific_incidence_StudyGroup,
            List<SecondDiagnosisGroup> list_second_diagnosis_group
            )
        {
            //初始化 SIR List
            foreach (var seconddx in list_second_diagnosis_group)
            {
                list_SIR.Add(new SIR() { str_group_name = seconddx.str_name });
            }
            for (int i = 0; i < list_SIR.Count(); i++)
            {
                //統計observed count
                int int_observe_count = 0;
                int int_observe_count_Male = 0;
                int int_observe_count_Female = 0;

                //將study group每個年齡層的診斷次數相加 = Observed count
                foreach (var ASI in list_Age_specific_incidence_StudyGroup)
                {
                    int_observe_count += ASI.Secondary_Diagnosis[i].int_Event_count;
                    int_observe_count_Male += ASI.Secondary_Diagnosis_Male[i].int_Event_count;
                    int_observe_count_Female += ASI.Secondary_Diagnosis_Female[i].int_Event_count;
                }

                //統計estimate count = patient-year(Study) * Event(allPT) / patient-year(allPT)
                double db_estimated_count = 0;
                double db_estimated_count_Male = 0;
                double db_estimated_count_Female = 0;
                for (int j = 0; j < list_Age_specific_incidence_AllPt.Count(); j++)
                {
                    db_estimated_count_Male +=
                       list_Age_specific_incidence_StudyGroup[j].db_total_patient_year_Male *
                       list_Age_specific_incidence_AllPt[j].Secondary_Diagnosis_Male[i].int_Event_count /
                       list_Age_specific_incidence_AllPt[j].db_total_patient_year_Male;
                    db_estimated_count_Female +=
                       list_Age_specific_incidence_StudyGroup[j].db_total_patient_year_Female *
                       list_Age_specific_incidence_AllPt[j].Secondary_Diagnosis_Female[i].int_Event_count /
                       list_Age_specific_incidence_AllPt[j].db_total_patient_year_Female;
                    db_estimated_count = db_estimated_count_Male + db_estimated_count_Female;
                }
                list_SIR[i].int_observed = int_observe_count;
                list_SIR[i].int_observed_Male = int_observe_count_Male;
                list_SIR[i].int_observed_Female = int_observe_count_Female;
                list_SIR[i].db_estimated = db_estimated_count;
                list_SIR[i].db_estimated_Male = db_estimated_count_Male;
                list_SIR[i].db_estimated_Female = db_estimated_count_Female;
            }
        }
        // --輸出SIR
        public static void Generate_SIR(List<SIR> list_SIR, string str_path)
        {
            using (var sw = new StreamWriter(str_path))
            {
                string title = "\tMale\t\t\t\t" + "\tFemale\t\t\t\t" + "\tAll";
                string subtitle = "Secondary Diagnosis" +
                    "\tObserved\tEstimated\tSIR\t95%CI\tSignificant" +
                    "\tObserved\tEstimated\tSIR\t95%CI\tSignificant" +
                    "\tObserved\tEstimated\tSIR\t95%CI\tSignificant";
                sw.WriteLine(title);
                sw.WriteLine(subtitle);
                foreach (var SIR in list_SIR)
                {
                    string line = SIR.str_group_name;
                    line += "\t" + SIR.int_observed_Male.NullforZero();
                    line += "\t" + SIR.db_estimated_Male.Round(2).NullforZero();
                    line += "\t" + SIR.db_SIR_Male.Round(2).NullforZero();
                    line += "\t" + SIR.str_SIR_95CI_Male;
                    line += "\t" + (SIR.IsSignificant_Male ? "*" : "");

                    line += "\t" + SIR.int_observed_Female.NullforZero();
                    line += "\t" + SIR.db_estimated_Female.Round(2).NullforZero();
                    line += "\t" + SIR.db_SIR_Female.Round(2).NullforZero();
                    line += "\t" + SIR.str_SIR_95CI_Female;
                    line += "\t" + (SIR.IsSignificant_Female ? "*" : "");

                    line += "\t" + SIR.int_observed.NullforZero();
                    line += "\t" + SIR.db_estimated.Round(2).NullforZero();
                    line += "\t" + SIR.db_SIR.Round(2).NullforZero();
                    line += "\t" + SIR.str_SIR_95CI;
                    line += "\t" + (SIR.IsSignificant ? "*" : "");
                    sw.WriteLine(line);
                }
            }
        }

        // -- (Private)檢查ICD是否存在criteria List中
        static bool check_ICD(IEnumerable<string> List_criteriaICD, IEnumerable<string> List_ICDtoExam)
        {
            foreach (string str_ICD in List_ICDtoExam)
            {
                if (List_criteriaICD.Any(x => x == str_ICD.Substring(0, x.Length))) return true;
            }
            return false;
        }
    }


}
