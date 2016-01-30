using System;
using System.Collections.Generic;
using System.Linq;
using NHIRD.Actions;
using NHIRD.DataTypes;

namespace NHIRD
{


    class Program
    {
        static void Main(string[] args)
        {
            NHIRD_ArgTable argTable = new NHIRD_ArgTable("2010", 2000, 2013);
            string path = @"D:\Google Drive\NHIRD DRUG EXPOSURE ASTHMA\Raw Data\";
            GetActionBasedData.CD(argTable.list_PrimaryActionBasedData_CD,
                path + @"Primary\2010 Less then 1 yo CD.txt", argTable.list_OrderGroupDefinision.Count);

            Console.WriteLine("End Of Program.");
            Console.ReadKey();
        }



        static void Preset_SIR(NHIRD_ArgTable argTable)
        {
            string str_path = @"D:\Google Drive\NHIRD WEI\" + argTable.str_year + @"\";

            // -- 初始化次要診斷群
            Console.Write("run: Initialize_Secondary_Diagnosis");
            action.Initialize_Secondary_Diagnosis(argTable.list_SecondDiagnosisGroupDefinition);
            Console.WriteLine(", {0} Groups loaded", argTable.list_SecondDiagnosisGroupDefinition.Count);

            // -- 使用ID summary建立 all patient總表
            Console.WriteLine("run: Load_All_Patient");
            action.Load_All_Patient(argTable.list_SecondDiagnosisGroupDefinition, argTable.list_PtBaseData_IDtable, str_path + @"1) ID Summary " + argTable.str_year + ".txt");

            // -- 加入含主診斷CD action based data 至List_PatientBasedData_All
            Console.WriteLine("run: Load_PrimaryDiagnosis_to_All_Patient");
            action.Load_PrimaryDiagnosis_to_All_Patient(argTable.list_SecondDiagnosisGroupDefinition, argTable.list_PtBaseData_IDtable, str_path + @"2) Primary CD " + argTable.str_year + ".txt");

            // -- 加入含次診斷CD action based data 至List_PatientBasedData_All
            Console.WriteLine("run: Load_SecondaryCD_to_All_Patient");
            action.Load_SecondaryCD_to_All_Patient(argTable.list_SecondDiagnosisGroupDefinition, argTable.list_PtBaseData_IDtable, str_path + @"3) Secondary CD " + argTable.str_year + ".txt");

            // -- 輸出all patient
            Console.WriteLine("run: Generate_All_Patient");
            action.Generate_All_Patient(argTable.list_PtBaseData_IDtable, argTable.list_SecondDiagnosisGroupDefinition,
                str_path + "R) Patient Based in ID " + argTable.str_year + ".txt", true,
                argTable.int_DataStartYear, argTable.int_DataEndYear);

            // -- 計算ASI(all pt)
            Console.WriteLine("run: Calculate_Age_Specific_Incidence(All PT)");
            action.Calculate_Age_Specific_Incidence(argTable.list_AgeSpecInc_AllPt, argTable.list_PtBaseData_IDtable,
                 argTable.list_SecondDiagnosisGroupDefinition, false, argTable.int_DataStartYear, argTable.int_DataEndYear);
            // -- 輸出ASI(all pt)
            Console.WriteLine("run: Generate_ASI");
            action.Generate_ASI(argTable.list_AgeSpecInc_AllPt, argTable.list_SecondDiagnosisGroupDefinition, str_path
                + @"R) ASI All Patient " + argTable.str_year + ".txt");

            // -- 計算ASI(study group)
            Console.WriteLine("run: Calculate_Age_Specific_Incidence(Study Group)");
            action.Calculate_Age_Specific_Incidence(argTable.list_AgeSpecInc_StdGrp, argTable.list_PtBaseData_IDtable,
                 argTable.list_SecondDiagnosisGroupDefinition, true, argTable.int_DataStartYear, argTable.int_DataEndYear);
            // -- 輸出ASI(study group)
            Console.WriteLine("run: Generate_ASI");
            action.Generate_ASI(argTable.list_AgeSpecInc_StdGrp,
                 argTable.list_SecondDiagnosisGroupDefinition, str_path + @"R) ASI study group " + argTable.str_year + ".txt");

            // --計算SIR
            Console.WriteLine("run: Calculate_SIR");
            action.Calculate_SIR(argTable.list_SIR, argTable.list_AgeSpecInc_AllPt
                , argTable.list_AgeSpecInc_StdGrp, argTable.list_SecondDiagnosisGroupDefinition);

            // --輸出SIR
            Console.WriteLine("run: Generate_SIR");
            action.Generate_SIR(argTable.list_SIR, str_path + @"R) SIR " + argTable.str_year + ".txt");
            argTable.GarbageCollection();
        }

        static void Preset()
        {
            string path = @"D:\Google Drive\NHIRD CHO\";

            // -- 初始化次要診斷群
            var list_second_Diagnosis_Group = new List<SecondDiagnosisGroup>();
            Console.Write("run: Initialize_Secondary_Diagnosis");
            action.Initialize_Secondary_Diagnosis(list_second_Diagnosis_Group);
            Console.WriteLine(", {0} Groups loaded", list_second_Diagnosis_Group.Count);


            // -- 使用含主診斷CD action based data 建立patient List
            var List_PatientBasedData = new List<PatientBasedData>();
            Console.WriteLine("run: Generate_patient_based_data, {0} Patient Created", List_PatientBasedData.Count);
            action.Generate_patient_based_data(List_PatientBasedData, list_second_Diagnosis_Group.Count, path + @"2) From CD action based 就診科別限制或07811.txt");
            // -- 使用patient List找到的DD檔，產生含次診斷統計的patient based data
            Console.WriteLine("run: Check_Secondary_Diagnosis");
            action.Check_Secondary_Diagnosis(list_second_Diagnosis_Group, List_PatientBasedData, path + @"3) DD use ID list.txt");
            // -- 輸出patient based data
            Console.WriteLine("run: Generate Report from patient based data");
            action.Generate_Report(List_PatientBasedData, list_second_Diagnosis_Group, path + @"Report_" + DateTime.Now.ToString("MMdd_hhmm") + ".txt");

            // -- 製作summarized Patient Data
            var list_summarized_Patient_data = new List<SumData>();
            Console.WriteLine("run: Summarize_Patient_AgeGrouped");
            action.Summarize_Patient_AgeGrouped(List_PatientBasedData, list_second_Diagnosis_Group, list_summarized_Patient_data, 0, 5, 20);

            // -- 輸出summarized Patient Data
            Console.WriteLine("run: Generate_Summary");
            action.Generate_Summary(list_summarized_Patient_data, list_second_Diagnosis_Group, path + @"Summary_" + DateTime.Now.ToString("MMdd_hhmm") + ".txt");


            // -- 使用ID summary建立 all patient總表
            var List_PatientBasedData_All = new List<PatientBasedData_ID>();
            Console.WriteLine("run: Load_All_Patient");
            action.Load_All_Patient(list_second_Diagnosis_Group, List_PatientBasedData_All, path + @"5) ID Summary 2000百萬歸人.txt");

            // -- 加入含主診斷CD action based data 至List_PatientBasedData_All
            Console.WriteLine("run: Load_PrimaryDiagnosis_to_All_Patient");
            action.Load_PrimaryDiagnosis_to_All_Patient(list_second_Diagnosis_Group, List_PatientBasedData_All, path + @"2) From CD action based 就診科別限制或07811.txt");

            // -- 加入含次診斷DD action based data 至List_PatientBasedData_All
            Console.WriteLine("run: Load_PrimaryDiagnosis_to_All_Patient");
            action.Load_SecondaryDiagnosis_to_All_Patient(list_second_Diagnosis_Group, List_PatientBasedData_All, path + @"4) DD Cancer all patient.txt");


            // -- 輸出all patient
            Console.WriteLine("run: Generate_All_Patient");
            action.Generate_All_Patient(List_PatientBasedData_All, list_second_Diagnosis_Group, path + "All patient.txt", true, 2000, 2013);


            // -- 計算ASI(all pt)
            var list_Age_specific_incidence_allPT = new List<Age_Specific_Incidence>();
            Console.WriteLine("run: Calculate_Age_Specific_Incidence");
            action.Calculate_Age_Specific_Incidence(list_Age_specific_incidence_allPT, List_PatientBasedData_All,
                list_second_Diagnosis_Group, false, 2000, 2013);
            // -- 輸出ASI(all pt)
            Console.WriteLine("run: Generate_ASI");
            action.Generate_ASI(list_Age_specific_incidence_allPT, list_second_Diagnosis_Group, path + @"Age Specific Incidence All Patient.txt");


            // -- 計算ASI(study group)
            var list_Age_specific_incidence_StudyGroup = new List<Age_Specific_Incidence>();
            Console.WriteLine("run: Calculate_Age_Specific_Incidence");
            action.Calculate_Age_Specific_Incidence(list_Age_specific_incidence_StudyGroup, List_PatientBasedData_All,
                list_second_Diagnosis_Group, true, 2000, 2013);
            // -- 輸出ASI(study group)
            Console.WriteLine("run: Generate_ASI");
            action.Generate_ASI(list_Age_specific_incidence_StudyGroup, list_second_Diagnosis_Group, path + @"Age Specific Incidence Study Group.txt");

            // --計算SIR
            var list_SIR = new List<SIR>();
            Console.WriteLine("run: Calculate_SIR");
            action.Calculate_SIR(list_SIR, list_Age_specific_incidence_allPT,
                list_Age_specific_incidence_StudyGroup, list_second_Diagnosis_Group);

            // --輸出SIR
            Console.WriteLine("run: Generate_SIR");
            action.Generate_SIR(list_SIR, path + @"SIR.txt");

        }
    }








}