using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using SurgeryHelper.Entities;
using Application = Microsoft.Office.Interop.Word.Application;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace SurgeryHelper.Engines
{
    public class WordExportEngine
    {
        private Application _wordApp;
        private Document _wordDoc;
        private Paragraph _paragraph;
        private Range _wordRange;
        private Table _wordTable;
        private Shape _wordShape;

        private object _missingObject = Type.Missing;

        private WaitForm _waitForm;
        private DbEngine _dbEngine;        

        public WordExportEngine(DbEngine dbEngine)
        {
            _dbEngine = dbEngine;
        }

        /// <summary>
        /// Сгенерировать дневник наблюдений
        /// </summary>
        /// <param name="patientInfo"></param>
        public void GenerateDairy(PatientClass patientInfo)
        {
/*
в день поступления - "По дежурству"
на следующий день - без заголовка
далее ПН, СР, ПТ (и так до дня выписки)
в день выписки - без заголовка, но внизу фраза про "выписан..." и "л/н... - если есть"
если вдруг случилась операция: на следующий день - "1е сутки после операции", далее "2е.." и "3и..." подряд три дневника
каждый понедельник - "Обход зав. отделением Т.Э. Торно" (если совпадает с послеоперационными, то сначала "обход зав..." потом "...сутки после операции"
объединяем заголовки, если несколько на один день приходятся
могут совпадать операция, обход зав. отделением и выписка
*/

            var nosologyDayryInfo = _dbEngine.GetNosologyByName(patientInfo.Nosology)[0].DairyInfo;
            var dairyDataGenerator = new DairyDataGenerator(Convert.ToInt32(patientInfo.Age), nosologyDayryInfo);

            _waitForm = new WaitForm();

            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 50;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "По дежурству";

                DateTime date = patientInfo.DeliveryDate.AddDays(1);

                var operationDairyDay = GetOperationDairyDay(patientInfo, date);
                if (operationDairyDay > -1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph.Range.Font.Bold = 1;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _paragraph.Range.Text = $"{operationDairyDay}е сутки после операции";
                }

                PutGeneralDairyInfo(patientInfo, dairyDataGenerator, date, operationDairyDay);
                if (ConvertEngine.CompareDateTimes(date, patientInfo.ReleaseDate.Value, false) == 0)
                {
                    PutReleaseDairyInfo(patientInfo);
                }

                PutDoctorInfo(patientInfo);

                // Проходим по всем дням со второго после дня поступления и до сегодняшнего дня (или дня выписки) и генерим на каждую дату запись в дневнике, если надо                
                date = date.AddDays(1);
                
                TimeSpan ts = patientInfo.ReleaseDate.Value - date;
                var progressShift = 80 / ts.TotalDays;
                var progress = 10; 
                while (ConvertEngine.CompareDateTimes(date, patientInfo.ReleaseDate.Value, false) < 1)
                {
                    if (date.DayOfWeek == DayOfWeek.Monday)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph.Range.Font.Bold = 1;
                        _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        _paragraph.Range.Text = $"Совместный осмотр с зав. отделением {_dbEngine.GlobalSettings.BranchManager}";                        
                    }

                    operationDairyDay = GetOperationDairyDay(patientInfo, date);
                    if (operationDairyDay > -1)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph.Range.Font.Bold = 1;
                        _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        _paragraph.Range.Text = $"{operationDairyDay}е сутки после операции";
                    }

                    if (date.DayOfWeek == DayOfWeek.Monday)
                    {
                        PutGeneralDairyInfo(patientInfo, dairyDataGenerator, date, operationDairyDay);
                        PutDepartmentHeadInfo(patientInfo);
                        if (ConvertEngine.CompareDateTimes(date, patientInfo.ReleaseDate.Value, false) == 0)
                        {
                            PutReleaseDairyInfo(patientInfo);
                        }

                        PutDoctorAndBranchMasterInfo(patientInfo);
                    }
                    else if (operationDairyDay > -1 || date.DayOfWeek == DayOfWeek.Wednesday || date.DayOfWeek == DayOfWeek.Friday ||
                        ConvertEngine.CompareDateTimes(date, patientInfo.DeliveryDate, false) == 0 ||
                        ConvertEngine.CompareDateTimes(date, patientInfo.DeliveryDate.AddDays(1), false) == 0 ||
                        ConvertEngine.CompareDateTimes(date, patientInfo.ReleaseDate.Value, false) == 0)
                    {
                        PutGeneralDairyInfo(patientInfo, dairyDataGenerator, date, operationDairyDay);
                        if (ConvertEngine.CompareDateTimes(date, patientInfo.ReleaseDate.Value, false) == 0)
                        {
                            PutReleaseDairyInfo(patientInfo);
                        }

                        PutDoctorInfo(patientInfo);
                    }

                    progress = (int)(progress + progressShift);
                    _waitForm.SetProgress(progress);
                    date = date.AddDays(1);
                }
               
                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private void PutGeneralDairyInfo(PatientClass patientInfo, DairyDataGenerator dairyDataGenerator, DateTime date, int operationDairyDay)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            _paragraph.Range.Text = $"\r\n{ConvertEngine.GetRightDateString(date)}{dairyDataGenerator.GetRandomTemperature()}";
            SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });

            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = dairyDataGenerator.GetDairyText(operationDairyDay);
        }

        private void PutDepartmentHeadInfo(PatientClass patientInfo)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = "Диагноз: " + patientInfo.Diagnose;
            SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = "Рекомендации: - коррекция терапии не требуется.";
            SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });
        }

        private void PutReleaseDairyInfo(PatientClass patientInfo)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = "\r\n" + GetExpertAnamnes(patientInfo, true) + "\r\nВыписывается на амбулаторное долечивание, рекомендации даны в выписном эпикризе.\r\n";
        }

        private void PutDoctorInfo(PatientClass patientInfo)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = "Врач: " + patientInfo.DoctorInChargeOfTheCase + "\r\n";
        }

        private void PutDoctorAndBranchMasterInfo(PatientClass patientInfo)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Range.Text = $"Врач: {patientInfo.DoctorInChargeOfTheCase}\t\t\t\t\tЗав. отделением: {_dbEngine.GlobalSettings.BranchManager}\r\n";
        }

        private int GetOperationDairyDay(PatientClass patientInfo, DateTime date)
        {
            DateTime lastOperaionDate = new DateTime();
            foreach (OperationClass operaion in patientInfo.Operations)
            {
                if (operaion.DataOfOperation > lastOperaionDate)
                {
                    lastOperaionDate = operaion.DataOfOperation;
                }
            }

            for (int i = 1; i < 4; i++)
            {
                if (ConvertEngine.CompareDateTimes(date, lastOperaionDate.AddDays(i), false) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Экспортировать в Word переводной эпикриз
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        public void ExportTransferableEpicrisis(PatientClass patientInfo)
        {
            _waitForm = new WaitForm();

            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 50;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ПЕРЕВОДНОЙ ЭПИКРИЗ\r\n";

                _waitForm.SetProgress(20);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                _paragraph.Range.Text = string.Format(
                    "Пациент {0}, {1} {2}, находится на лечении в {3} х.о. с {4} с диагнозом: {5}\r\n",
                    patientInfo.GetFullName(),
                    patientInfo.Age,
                    ConvertEngine.GetAgeString(patientInfo.Age),
                    _dbEngine.GlobalSettings.DepartmentName,
                    ConvertEngine.GetRightDateString(patientInfo.DeliveryDate),
                    patientInfo.Diagnose);

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Проведено лечение:";

                var textStr = new StringBuilder();
                foreach (OperationClass operationInfo in patientInfo.Operations)
                {
                    textStr.AppendFormat("{0} - {1}\r\n", ConvertEngine.GetRightDateString(operationInfo.DataOfOperation), operationInfo.Name);
                }

                if (textStr.Length > 2)
                {
                    textStr.Remove(textStr.Length - 2, 2);
                }

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = textStr.ToString();

                if (!string.IsNullOrEmpty(patientInfo.TransferEpicrisAdditionalInfo))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = patientInfo.TransferEpicrisAdditionalInfo + "\r\n";
                }

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Послеоперационный период " + patientInfo.TransferEpicrisAfterOperationPeriod;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Планируется:";

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = patientInfo.TransferEpicrisPlan;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Для дальнейшего лечения в удовлетворительном состоянии переводится на дневной стационар.";

                _waitForm.SetProgress(70);

                if (patientInfo.TransferEpicrisIsIncludeDisabilityList)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Л/н № {0} продлен с {1} по {2}.\r\n",
                        patientInfo.TransferEpicrisDisabilityList,
                        ConvertEngine.GetRightDateString(patientInfo.TransferEpicrisWritingDate.AddDays(1)),
                        ConvertEngine.GetRightDateString(patientInfo.TransferEpicrisWritingDate.AddDays(10)));
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "С режимом ознакомлен _____________________________\r\n";

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + ConvertEngine.GetRightDateString(patientInfo.TransferEpicrisWritingDate);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + _dbEngine.GlobalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + patientInfo.DoctorInChargeOfTheCase;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        /// <summary>
        /// Экспортировать в Word этапный эпикриз
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        public void ExportLineOfCommunicationEpicrisis(PatientClass patientInfo)
        {
            _waitForm = new WaitForm();

            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 50;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ЭТАПНЫЙ ЭПИКРИЗ\r\n";

                _waitForm.SetProgress(20);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                _paragraph.Range.Text = string.Format(
                    "Пациент {0}, {1} {2}, находится на лечении в {3} х.о. с {4} с диагнозом: {5}\r\n",
                    patientInfo.GetFullName(),
                    patientInfo.Age,
                    ConvertEngine.GetAgeString(patientInfo.Age),
                    _dbEngine.GlobalSettings.DepartmentName,
                    ConvertEngine.GetRightDateString(patientInfo.DeliveryDate),
                    patientInfo.Diagnose);

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Проведено лечение:";

                var textStr = new StringBuilder();
                foreach (OperationClass operationInfo in patientInfo.Operations)
                {
                    textStr.AppendFormat("{0} - {1}\r\n", ConvertEngine.GetRightDateString(operationInfo.DataOfOperation), operationInfo.Name);
                }

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = textStr.ToString();

                if (!string.IsNullOrEmpty(patientInfo.LineOfCommEpicrisAdditionalInfo))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = patientInfo.LineOfCommEpicrisAdditionalInfo + "\r\n";
                }

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Планируется:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = patientInfo.LineOfCommEpicrisPlan + "\r\n";

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + ConvertEngine.GetRightDateString(patientInfo.LineOfCommEpicrisWritingDate);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + _dbEngine.GlobalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + patientInfo.DoctorInChargeOfTheCase;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        /// <summary>
        /// Экспортировать в Word выписной эпикриз
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="dischargeEpicrisisHeaderFilePath">Путь до файла с шапкой для выписного эпикриза</param>
        public void ExportDischargeEpicrisis(PatientClass patientInfo, object dischargeEpicrisisHeaderFilePath)
        {
            _waitForm = new WaitForm();

            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref dischargeEpicrisisHeaderFilePath, ref _missingObject, ref _missingObject, ref _missingObject);

                double previousValue = 20;
                double currentValue = previousValue;

                for (int num = 1; num <= _wordDoc.Content.Paragraphs.Count; num++)
                {
                    Paragraph paragraph = _wordDoc.Content.Paragraphs[num];
                    FindMarkAndReplace(
                        paragraph.Range.Text,
                        null,
                        0,
                        ref previousValue,
                        ref currentValue,
                        patientInfo,
                        false);
                }

                _waitForm.SetProgress(30);

                var rowCnt = string.IsNullOrEmpty(patientInfo.WWW) ? 13 : 14;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _wordRange = _paragraph.Range;
                _wordTable = _wordDoc.Tables.Add(_wordRange, rowCnt, 4, ref _missingObject, ref _missingObject);
                _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Range.Font.Size = 11;
                _wordTable.Range.Font.Bold = 0;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                object begCell;
                object endCell;

                for (int i = 1; i < 5; i++)
                {
                    begCell = _wordTable.Cell(i, 2).Range.Start;
                    endCell = _wordTable.Cell(i, 4).Range.End;
                    MergeCells(begCell, endCell);
                }

                _wordTable.Cell(1, 1).Range.Font.Bold = 0;
                _wordTable.Cell(1, 1).Range.Text = "ФИО больного";
                _wordTable.Cell(1, 2).Range.Font.Bold = 1;
                _wordTable.Cell(1, 2).Range.Text = patientInfo.GetFullName();

                _wordTable.Cell(2, 1).Range.Font.Bold = 0;
                _wordTable.Cell(2, 1).Range.Text = "Дата рождения";
                _wordTable.Cell(2, 2).Range.Font.Bold = 1;
                _wordTable.Cell(2, 2).Range.Text = ConvertEngine.GetRightDateString(patientInfo.Birthday);

                _wordTable.Cell(3, 1).Range.Font.Bold = 0;
                _wordTable.Cell(3, 1).Range.Text = "Регистрация по месту жительства";
                _wordTable.Cell(3, 2).Range.Font.Bold = 1;
                _wordTable.Cell(3, 2).Range.Text = patientInfo.GetAddress();

                _wordTable.Cell(4, 1).Range.Font.Bold = 0;
                _wordTable.Cell(4, 1).Range.Text = "Поступил в";
                _wordTable.Cell(4, 2).Range.Font.Bold = 1;
                _wordTable.Cell(4, 2).Range.Text = patientInfo.TypeOfKSG == "н" ? "стационар" : "дневной стационар";

                _wordTable.Cell(5, 1).Range.Font.Bold = 0;
                _wordTable.Cell(5, 1).Range.Text = "Дата поступления";
                _wordTable.Cell(5, 2).Range.Font.Bold = 1;
                _wordTable.Cell(5, 2).Range.Text = ConvertEngine.GetRightDateString(patientInfo.DeliveryDate, true);
                _wordTable.Cell(5, 3).Range.Font.Bold = 0;
                _wordTable.Cell(5, 3).Range.Text = "Дата выписки";
                _wordTable.Cell(5, 4).Range.Font.Bold = 1;
                _wordTable.Cell(5, 4).Range.Text = patientInfo.ReleaseDate.HasValue ? ConvertEngine.GetRightDateString(patientInfo.ReleaseDate.Value) + " 14:00" : "НЕ УКАЗАНА";

                _waitForm.SetProgress(40);

                begCell = _wordTable.Cell(6, 1).Range.Start;
                endCell = _wordTable.Cell(6, 3).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(6, 1).Range.Font.Bold = 0;
                _wordTable.Cell(6, 1).Range.Text = "Количество дней нахождения в медицинской организации";
                _wordTable.Cell(6, 2).Range.Font.Bold = 1;
                _wordTable.Cell(6, 2).Range.Text = patientInfo.GetKD();

                _wordTable.Cell(7, 1).Range.Font.Bold = 0;
                _wordTable.Cell(7, 1).Range.Text = "Исход госпитализации";
                _wordTable.Cell(7, 2).Range.Font.Bold = 1;
                _wordTable.Cell(7, 2).Range.Text = "выписан";

                _wordTable.Cell(8, 1).Range.Font.Bold = 0;
                _wordTable.Cell(8, 1).Range.Text = "Результат госпитализации";
                _wordTable.Cell(8, 2).Range.Font.Bold = 1;
                _wordTable.Cell(8, 2).Range.Text = "улучшение";

                begCell = _wordTable.Cell(9, 1).Range.Start;
                endCell = _wordTable.Cell(9, 3).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(9, 1).Range.Font.Bold = 0;
                _wordTable.Cell(9, 1).Range.Text = "Форма оказания медицинской помощи";
                _wordTable.Cell(9, 2).Range.Font.Bold = 1;
                _wordTable.Cell(9, 2).Range.Text = patientInfo.Hospitalization;

                begCell = _wordTable.Cell(10, 1).Range.Start;
                endCell = _wordTable.Cell(10, 4).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(10, 1).Range.Font.Bold = 0;
                _wordTable.Cell(10, 1).Range.Text = "Заключительный диагноз:";

                begCell = _wordTable.Cell(11, 2).Range.Start;
                endCell = _wordTable.Cell(11, 3).Range.End;
                MergeCells(begCell, endCell);
                float middleWidth = _wordTable.Cell(11, 2).Width;
                float rightWidth = _wordTable.Cell(11, 3).Width;
                _wordTable.Cell(11, 2).Width = middleWidth + rightWidth / 2;
                _wordTable.Cell(11, 3).Width = rightWidth / 2;
                _wordTable.Cell(11, 1).Range.Font.Bold = 0;
                _wordTable.Cell(11, 1).Range.Text = "\tОсновное\r\n\tзаболевание:";
                _wordTable.Cell(11, 2).Range.Font.Bold = 1;
                _wordTable.Cell(11, 2).Range.Text = patientInfo.Diagnose;
                _wordTable.Cell(11, 3).Range.Font.Bold = 1;
                _wordTable.Cell(11, 3).Range.Text = patientInfo.MKB;

                _waitForm.SetProgress(50);

                begCell = _wordTable.Cell(12, 2).Range.Start;
                endCell = _wordTable.Cell(12, 3).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(12, 2).Width = middleWidth + rightWidth / 2;
                _wordTable.Cell(12, 3).Width = rightWidth / 2;
                _wordTable.Cell(12, 1).Range.Font.Bold = 0;
                _wordTable.Cell(12, 1).Range.Text = "\tОсложнения:";
                _wordTable.Cell(12, 2).Range.Font.Bold = 1;
                _wordTable.Cell(12, 2).Range.Text = patientInfo.Complications;
                _wordTable.Cell(12, 3).Range.Font.Bold = 1;
                _wordTable.Cell(12, 3).Range.Text = patientInfo.ComplicationsMKB;

                begCell = _wordTable.Cell(13, 2).Range.Start;
                endCell = _wordTable.Cell(13, 3).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(13, 2).Width = middleWidth + rightWidth / 2;
                _wordTable.Cell(13, 3).Width = rightWidth / 2;
                _wordTable.Cell(13, 1).Range.Font.Bold = 0;
                _wordTable.Cell(13, 1).Range.Text = "\tСопутствующее\r\n\tзаболевание:";
                _wordTable.Cell(13, 2).Range.Font.Bold = 1;
                _wordTable.Cell(13, 2).Range.Text = patientInfo.ConcomitantDiagnose;
                _wordTable.Cell(13, 3).Range.Font.Bold = 1;
                _wordTable.Cell(13, 3).Range.Text = patientInfo.ConcomitantDiagnoseMKB;

                if (!string.IsNullOrEmpty(patientInfo.WWW))
                {
                    begCell = _wordTable.Cell(14, 2).Range.Start;
                    endCell = _wordTable.Cell(14, 3).Range.End;
                    MergeCells(begCell, endCell);
                    _wordTable.Cell(14, 2).Width = middleWidth + rightWidth / 2;
                    _wordTable.Cell(14, 3).Width = rightWidth / 2;
                    _wordTable.Cell(14, 1).Range.Font.Bold = 0;
                    _wordTable.Cell(14, 1).Range.Text = "\tВнешняя\r\n\tпричина:";
                    _wordTable.Cell(14, 2).Range.Font.Bold = 1;
                    _wordTable.Cell(14, 2).Range.Text = _dbEngine.GetMkbName(patientInfo.WWW);
                    _wordTable.Cell(14, 3).Range.Font.Bold = 1;
                    _wordTable.Cell(14, 3).Range.Text = patientInfo.WWW;
                }

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 0;
                _paragraph.Range.Font.Size = 11;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Дополнительные сведения о заболевании: " + patientInfo.MedicalInspectionAnamneseAnMorbi;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Состояние при поступлении: " + patientInfo.MedicalInspectionStLocalisDescription;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Осмотры врачей-специалистов, консилиумы врачей: " + patientInfo.DischargeEpicrisConsultation;

                if (!patientInfo.MedicalInspectionIsPlanEnabled || patientInfo.MedicalInspectionInspectionPlan == "обследован амбулаторно")
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph.Range.Bold = 0;
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Результаты медицинского обследования: обследован амбулаторно";
                }
                else
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Результаты медицинского обследования:";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "ОАК({0}): эритроциты-{1}х1012/л, лейкоциты-{2}х109/л, Hb-{3} г/л, СОЭ-{4} мм/ч;",
                        ConvertEngine.GetRightDateString((patientInfo.DischargeEpicrisAnalysisDate.HasValue ? patientInfo.DischargeEpicrisAnalysisDate.Value : DateTime.Now)),
                        patientInfo.DischargeEpicrisOakEritrocits,
                        patientInfo.DischargeEpicrisOakLekocits,
                        patientInfo.DischargeEpicrisOakHb,
                        patientInfo.DischargeEpicrisOakSoe);
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                    // Возводим в степень 10 в 12-ой и 10 в 9-ой.
                    int charNum = _paragraph.Range.Text.IndexOf("х1012/л");
                    _paragraph.Range.Characters[charNum + 4].Font.Superscript =
                    _paragraph.Range.Characters[charNum + 5].Font.Superscript = 1;

                    charNum = _paragraph.Range.Text.IndexOf("х109/л");
                    _paragraph.Range.Characters[charNum + 4].Font.Superscript = 1;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Eml отрицательный";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "ОАМ({0}): цвет {1}, относит. плотность {2}, эритроциты {3}, лейкоциты {4}",
                        ConvertEngine.GetRightDateString(patientInfo.DischargeEpicrisAnalysisDate.HasValue ? patientInfo.DischargeEpicrisAnalysisDate.Value : DateTime.Now),
                        patientInfo.DischargeEpicrisOamColor,
                        patientInfo.DischargeEpicrisOamDensity,
                        patientInfo.DischargeEpicrisOamEritrocits,
                        patientInfo.DischargeEpicrisOamLekocits);
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                    if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakBillirubin) ||
                        !string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakGeneralProtein) ||
                        !string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakPTI) ||
                        !string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakSugar) ||
                        !string.IsNullOrEmpty(patientInfo.DischargeEpicrisBloodGroup))
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];

                        string info = string.Format("БАК({0}): ", ConvertEngine.GetRightDateString(patientInfo.DischargeEpicrisAnalysisDate.HasValue ? patientInfo.DischargeEpicrisAnalysisDate.Value : DateTime.Now));

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakBillirubin))
                        {
                            info += "билирубин " + patientInfo.DischargeEpicrisBakBillirubin + " мкмоль/л, ";
                        }

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakGeneralProtein))
                        {
                            info += "креатинин " + patientInfo.DischargeEpicrisBakGeneralProtein + " мкмоль/л, ";
                        }

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakSugar))
                        {
                            info += "глюкоза " + patientInfo.DischargeEpicrisBakSugar + " ммоль/л, ";
                        }

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBakPTI))
                        {
                            info += "ПТИ " + patientInfo.DischargeEpicrisBakPTI + "%, ";
                        }

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisBloodGroup))
                        {
                            info += "группа крови " + patientInfo.DischargeEpicrisBloodGroup + " резус фактор " + patientInfo.DischargeEpicrisRhesusFactor + ", ";
                        }

                        if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisAdditionalAnalises))
                        {
                            info += patientInfo.DischargeEpicrisAdditionalAnalises + ", ";
                        }

                        _paragraph.Range.Text = info.Substring(0, info.Length - 2);
                        SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format("ЭКГ({0}): {1}",
                        ConvertEngine.GetRightDateString(patientInfo.DischargeEpicrisAnalysisDate.HasValue ? patientInfo.DischargeEpicrisAnalysisDate.Value : DateTime.Now), patientInfo.DischargeEpicrisEkg);
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Применение лекарственных препаратов: " + patientInfo.GetDischargeEpicrisConservativeTherapy();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Трансфузии донорской крови или ее компонентов -не проводились.";

                AddEmptyParagraph();

                _waitForm.SetProgress(70);

                if (patientInfo.MedicalInspectionTreatmentType == "оперативное" && patientInfo.Operations.Count > 0)
                {
                    string textStrFirstLine = string.Empty;
                    var textStr = new StringBuilder();
                    foreach (OperationClass operationInfo in patientInfo.Operations)
                    {
                        if (string.IsNullOrEmpty(textStrFirstLine))
                        {
                            textStrFirstLine = string.Format("{0} - {1}", ConvertEngine.GetRightDateString(operationInfo.DataOfOperation), operationInfo.Name);
                        }
                        else
                        {
                            textStr.AppendFormat("\t\t{0} - {1}\r\n", ConvertEngine.GetRightDateString(operationInfo.DataOfOperation), operationInfo.Name);
                        }
                    }

                    if (textStr.Length > 2)
                    {
                        textStr.Remove(textStr.Length - 2, 2);
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Bold = 0;
                    _paragraph.Range.Text = "Операции:\t" + textStrFirstLine;
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                    if (textStr.Length > 0)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = textStr.ToString();
                    }
                }

                _waitForm.SetProgress(80);

                if (!string.IsNullOrEmpty(patientInfo.DischargeEpicrisAfterOperation))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = $"Дополнительные сведения: {patientInfo.DischargeEpicrisAfterOperation}";
                }

                var dischargeInfo = string.Empty;
                if (patientInfo.DischargeEpicrisRecomendations.Count > 0 &&
                    IsRecommendationReason(patientInfo.DischargeEpicrisRecomendations[0]))
                {
                    dischargeInfo = patientInfo.DischargeEpicrisRecomendations[0];
                }
                else
                {
                    dischargeInfo = "листок нетрудоспособности не требуется";
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Состояние при выписке удовлетворительное, " + dischargeInfo;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 1;
                _paragraph.Range.Text = "Рекомендации:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 0;

                var recommendations = new StringBuilder();
                for (int i = 0; i < patientInfo.DischargeEpicrisRecomendations.Count; i++)
                {
                    if (IsRecommendationReason(patientInfo.DischargeEpicrisRecomendations[i]))
                    {
                        continue;
                    }

                    recommendations.AppendLine(patientInfo.DischargeEpicrisRecomendations[i]);
                }

                for (int i = 0; i < patientInfo.DischargeEpicrisAdditionalRecomendations.Count; i++)
                {
                    recommendations.AppendLine(patientInfo.DischargeEpicrisAdditionalRecomendations[i]);
                }

                if (string.IsNullOrEmpty(recommendations.ToString()))
                {
                    _paragraph.Range.Text = "\tНет рекомендаций\r\n";
                }
                else
                {
                    _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                    _paragraph.Range.Text = recommendations.ToString();
                    _paragraph.Range.ListFormat.ApplyNumberDefaultOld();
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Примечания: контактов нет, результаты дополнительных методов обследования выданы на руки\r\n";
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _wordRange = _paragraph.Range;
                _wordTable = _wordDoc.Tables.Add(_wordRange, 1, 4, ref _missingObject, ref _missingObject);
                _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Range.Font.Bold = 0;
                
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                SetColumnWidths(rowCnt, new[] { 110, 160, 110, 160 });

                begCell = _wordTable.Cell(1, 1).Range.Start;
                endCell = _wordTable.Cell(1, 4).Range.End;

                _wordRange = _wordDoc.Range(ref begCell, ref endCell);
                _wordRange.Select();
                _wordApp.Selection.Cells.Height = 20;

                _wordTable.Cell(1, 1).Range.Text = "Лечащий врач, " + _dbEngine.GetSpecialityBySurgeonName(patientInfo.DoctorInChargeOfTheCase);
                _wordTable.Cell(1, 2).Range.Text = "\t" + patientInfo.DoctorInChargeOfTheCase + "\t";
                _wordTable.Cell(1, 2).Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                _wordTable.Cell(1, 3).Range.Text = "Зав. отделением, " + _dbEngine.GetSpecialityBySurgeonName(_dbEngine.GlobalSettings.BranchManager);
                _wordTable.Cell(1, 4).Range.Text = "\t" + _dbEngine.GlobalSettings.BranchManager + "\t";
                _wordTable.Cell(1, 4).Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = patientInfo.ReleaseDate.HasValue ? 
                    ConvertEngine.GetRightDateString(patientInfo.ReleaseDate.Value) + " 14:00"
                    : "НЕ УКАЗАНА";

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private bool IsRecommendationReason(string recomendation)
        {
            return recomendation.Contains(" № ______");
        }

        /// <summary>
        /// Экспортировать в Word осмотр в отделении
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        public void ExportMedicalInspection(PatientClass patientInfo)
        {
            _waitForm = new WaitForm();

            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(20);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 50;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 10;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ПЕРВИЧНЫЙ ОСМОТР";

                if (patientInfo.MedicalInspectionWithBoss)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Range.Text = _dbEngine.GlobalSettings.BossJobTitle;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = ConvertEngine.GetRightDateString(patientInfo.DeliveryDate, true);

                AddEmptyParagraph();

                string[] complaints = patientInfo.MedicalInspectionComplaints.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Жалобы: " + complaints[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(30);

                if (complaints.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = complaints[1];
                }

                if (patientInfo.MedicalInspectionIsAnamneseActive)
                {
                    string[] anMorbi = patientInfo.MedicalInspectionAnamneseAnMorbi.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];

                    _paragraph.Range.Text = "Анамнез заболевания: " + anMorbi[0];
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                    if (anMorbi.Length > 1)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = anMorbi[1];
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Анамнез жизни: туберкулез: {0}, желтуха: {1}, вен. заболевания: {2}, острозаразные: {3}",
                        patientInfo.MedicalInspectionAnamneseAnVitae[0] ? "есть" : "нет",
                        patientInfo.MedicalInspectionAnamneseAnVitae[1] ? "есть" : "нет",
                        patientInfo.MedicalInspectionAnamneseAnVitae[2] ? "есть" : "нет",
                        patientInfo.MedicalInspectionAnamneseAnVitae[3] ? "есть" : "нет");
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                    "Операции: {0}. Травмы: {1}\r\n" +
                    "Хрон. заболевания: {2}. Воспаление легких: {3}; варикозная болезнь: {4}. Переливание крови: {5}. Контактов с инфекционными больными в последний месяц {6}.",
                    patientInfo.MedicalInspectionAnamneseTextBoxes[2],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[6],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[0],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[1],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[4],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[5],
                    patientInfo.MedicalInspectionAnamneseTextBoxes[3]);

                    if (patientInfo.MedicalInspectionTeoRiskEnabled)
                    {
                        int rowCnt = 7;

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Font.Bold = 0;
                        _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        _wordRange = _paragraph.Range;
                        object defaultTableBehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
                        object autoFitBehavior = WdAutoFitBehavior.wdAutoFitFixed;
                        _wordTable = _wordDoc.Tables.Add(_wordRange, rowCnt, 6, ref defaultTableBehavior, ref autoFitBehavior);

                        _wordTable.Range.Font.Name = "Times New Roman";
                        _wordTable.Range.Font.Size = 10;
                        _wordTable.Range.Font.Bold = 0;
                        _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        _wordTable.Rows.SetLeftIndent((float)8.5, WdRulerStyle.wdAdjustNone);

                        SetColumnWidths(rowCnt, new[] { 215, 20, 25, 185, 20, 25 });

                        _wordTable.Rows[1].Cells[1].Range.Text = "Факторы риска ТГВ и ТЭЛА";
                        _wordTable.Rows[1].Cells[1].Range.Font.Bold = 1;

                        _wordTable.Rows[1].Cells[2].Range.Text = "да";
                        _wordTable.Rows[1].Cells[2].Range.Font.Bold = 1;

                        _wordTable.Rows[1].Cells[3].Range.Text = "нет";
                        _wordTable.Rows[1].Cells[3].Range.Font.Bold = 1;

                        _wordTable.Rows[1].Cells[5].Range.Text = "да";
                        _wordTable.Rows[1].Cells[5].Range.Font.Bold = 1;

                        _wordTable.Rows[1].Cells[6].Range.Text = "нет";
                        _wordTable.Rows[1].Cells[6].Range.Font.Bold = 1;

                        _wordTable.Rows[2].Cells[1].Range.Text = "1. Венозный тромбоз и ТЭЛА в анамнезе у пациента (тромбофилия)";
                        _wordTable.Rows[2].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[0])
                        {
                            _wordTable.Rows[2].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[2].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[2].Cells[4].Range.Text = "7. Хроническое неспецифическое заболевание легких";
                        _wordTable.Rows[2].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[6])
                        {
                            _wordTable.Rows[2].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[2].Cells[6].Range.Text = "x";
                        }

                        _wordTable.Rows[3].Cells[1].Range.Text = "2. Постромботическая болезнь (тромбофилия)";
                        _wordTable.Rows[3].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[1])
                        {
                            _wordTable.Rows[3].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[3].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[3].Cells[4].Range.Text = "8. Ожирение";
                        _wordTable.Rows[3].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[7])
                        {
                            _wordTable.Rows[3].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[3].Cells[6].Range.Text = "x";
                        }

                        _wordTable.Rows[4].Cells[1].Range.Text = "3. Венозный тромбоз и ТЭЛА у биологических родственников (тромбофилия)";
                        _wordTable.Rows[4].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[2])
                        {
                            _wordTable.Rows[4].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[4].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[4].Cells[4].Range.Text = "9. Иммобилизация нижней конечности с пребыванием в постели 3 и более дней";
                        _wordTable.Rows[4].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[8])
                        {
                            _wordTable.Rows[4].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[4].Cells[6].Range.Text = "x";
                        }

                        _wordTable.Rows[5].Cells[1].Range.Text = "4. Прием антикоагулянтов";
                        _wordTable.Rows[5].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[3])
                        {
                            _wordTable.Rows[5].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[5].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[5].Cells[4].Range.Text = "10. Сахарный диабет";
                        _wordTable.Rows[5].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[9])
                        {
                            _wordTable.Rows[5].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[5].Cells[6].Range.Text = "x";
                        }

                        _wordTable.Rows[6].Cells[1].Range.Text = "5. Варикозное расширение вен";
                        _wordTable.Rows[6].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[4])
                        {
                            _wordTable.Rows[6].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[6].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[6].Cells[4].Range.Text = "11. Прием эстрогенов";
                        _wordTable.Rows[6].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[10])
                        {
                            _wordTable.Rows[6].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[6].Cells[6].Range.Text = "x";
                        }

                        _wordTable.Rows[7].Cells[1].Range.Text = "6. Инфаркт миокарда";
                        _wordTable.Rows[7].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[5])
                        {
                            _wordTable.Rows[7].Cells[2].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[7].Cells[3].Range.Text = "x";
                        }

                        _wordTable.Rows[7].Cells[4].Range.Text = "12. Онкозаболевание";
                        _wordTable.Rows[7].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (patientInfo.MedicalInspectionAnamneseCheckboxes[11])
                        {
                            _wordTable.Rows[7].Cells[5].Range.Text = "x";
                        }
                        else
                        {
                            _wordTable.Rows[7].Cells[6].Range.Text = "x";
                        }

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = "Риск ТЭО: " + patientInfo.MedicalInspectionTeoRisk + ".";
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Аллергические реакции на лекарственные препараты: " + patientInfo.MedicalInspectionAnamneseTextBoxes[7] + ".";
                }
                else
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                }
                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сведения о листке нетрудоспособности (при наличии): " + GetExpertAnamnes(patientInfo);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Физикальное исследование:";
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "тяжесть состояния пациента: " + patientInfo.MedicalInspectionStPraesensComboBoxes[0];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "уровень сознания по шкале Глазго: " + patientInfo.MedicalInspectionStPraesensComboBoxes[4] + ConvertEngine.GetBallString(patientInfo.MedicalInspectionStPraesensComboBoxes[4]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "оценка состояния кожных покровов: " + patientInfo.MedicalInspectionStPraesensTextBoxes[2];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "отеки: не определяются";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "оценка состояния видимых слизистых оболочек: розовые, без патологических высыпаний";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "состояние подкожно-жировой клетчатки: " + patientInfo.MedicalInspectionStPraesensComboBoxes[1];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "результаты пальпации лимфатических узлов: " + patientInfo.MedicalInspectionStPraesensTextBoxes[4];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "оценка костно-мышечной системы(вне зоны повреждения): " + patientInfo.MedicalInspectionStPraesensTextBoxes[16];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("результаты аускультации легких: дыхание {0}, {1}, хрипы: {2}.",
                    patientInfo.MedicalInspectionStPraesensTextBoxes[5],
                    patientInfo.MedicalInspectionStPraesensComboBoxes[2],
                    patientInfo.MedicalInspectionStPraesensTextBoxes[6]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("результаты перкуссии и аускультация сердца: перкуторно границы сердца не изменены; аускультативно тоны сердца {0}, ритм {1}, патологических шумов не определяется",
                    patientInfo.MedicalInspectionStPraesensComboBoxes[3],
                    patientInfo.MedicalInspectionStPraesensTextBoxes[7]);

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = patientInfo.MedicalInspectionStPraesensTextBoxes[9];

                if (!string.IsNullOrEmpty(patientInfo.MedicalInspectionStPraesensOthers))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = patientInfo.MedicalInspectionStPraesensOthers;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "термометрия: {0} °С, измерения частоты сердечных сокращений, частоты пульса: {1} ударов в минуту, " +
                    "артериальное  давление: {2} / {3} мм рт.ст, частота  дыхательных  движений: {4} в минуту, насыщение крови кислородом " +
                    "(сатурация): {5} %, антропометрия: рост {6} см, масса  тела {7} кг",
                    patientInfo.MedicalInspectionStPraesensTemperature,
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[1],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[2],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[3],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[0],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[4],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[5],
                    patientInfo.MedicalInspectionStPraesensNumericUpDowns[6]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "локальный статус: " + patientInfo.MedicalInspectionStLocalisDescription;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "рентгенография: " + patientInfo.MedicalInspectionStLocalisRentgen;

                _waitForm.SetProgress(60);

                if (patientInfo.MedicalInspectionIsStLocalisPart1Enabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Объем движений в суставах верхней конечности";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Плечевой пояс:\tэлевация / депрессия (F: 20-0-10): акт - {0}, пасс – {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[0],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[1]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tсгибание/разгибание (Т: 20-0-20): акт - {0}, пасс – {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[2],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[3]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Плечевой сустав:  разгибание/сгибание (S: 50-0-180): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[4],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[5]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   отведение/приведение (F: 180-0-0): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[6],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[7]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   горизонтальное разгибание и сгибание (Т: 30-0-135): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[8],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[9]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   нар. и вн. ротация при отведенном на 90° плече (R: 90-0-90): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[10],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[11]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   нар. и вн. ротация при приведенном плече (R: 65-0-70): акт - {0}, пасс - {1}.",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[12],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[13]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Локтевой сустав: разгибание и сгибание (S: 0-0-150): акт - {0}, пасс - {1}.",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[14],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[15]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Луче-локтевые суставы: супинация и пронация (R: 90-0-90): акт - {0}, пасс - {1}.",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[16],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[17]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Лучезапястный сустав:\tразгибание и сгибание (S: 70-0-80): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[18],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[19]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\tотведение и приведение (F: 25-0-55): акт - {0}, пасс - {1}.",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[20],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[21]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Суставы 1-го пальца:";
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tЗПС:\tлучевое отведение и приведение (F: 35-0-15): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[22],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[23]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tладонное отведение и приведение (S: 40-0-0): акт - {0}, пасс - {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[24],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[25]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПФС.\tразгибание и сгибание (S: 5-0-50): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[26],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[27]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tМФС.\tразгибание и сгибание (S: 15-0-85): акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[28],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[29]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _waitForm.SetProgress(70);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tоппозиция: {0} палец",
                        patientInfo.MedicalInspectionStLocalisPart1OppositionFinger);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Суставы II-V-го пальцев:";
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6, 7, 8 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПФС: разгибание и сгибание (S: 35-0-90): IIп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[30],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[31]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  IIIп. - акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[32],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[33]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  IVп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[34],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[35]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  V - акт - {0}, пасс - {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[36],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[37]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tотведение и приведение (F: 30-0-25): IIп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[38],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[39]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       IIIп. - акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[40],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[41]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       IVп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[42],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[43]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       V - акт - {0}, пасс - {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[44],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[45]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПМФС: разгибание и сгибание (S: 0-0-100): IIп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[46],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[47]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     IIIп. - акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[48],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[49]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     IVп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[50],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[51]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     V - акт - {0}, пасс - {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[52],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[53]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tДМФС: разгибание и сгибание (S: 0-0-80): IIп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[54],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[55]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   IIIп. - акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[56],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[57]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   IVп. акт - {0}, пасс - {1};",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[58],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[59]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   V - акт - {0}, пасс - {1}",
                        patientInfo.MedicalInspectionStLocalisPart1Fields[60],
                        patientInfo.MedicalInspectionStLocalisPart1Fields[61]);

                    AddEmptyParagraph();
                }

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Диагноз при поступлении";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] diagnose = patientInfo.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Обоснование диагноза: диагноз поставлен на основании анамнеза, жалоб, данных осмотра и дополнительных методов обследования";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Обоснование оказания медицинской помощи: пациенту с диагнозом {0} необходимо оказание помощи в условиях {1}",
                    patientInfo.Diagnose.ToLowerInvariant(),
                    patientInfo.TypeOfKSG == "н" ? "стационара" : "дневного стационара");

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Выполнены медицинские вмешательства: ---";

                AddEmptyParagraph();

                _waitForm.SetProgress(90);

                if (patientInfo.MedicalInspectionIsPlanEnabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "План обследования и лечения:";

                    if (patientInfo.MedicalInspectionTreatmentType == "оперативное")
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                        _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                        _paragraph.Range.Text = "Обследование: " + patientInfo.MedicalInspectionInspectionPlan + ".\r\n" +
                            "Оперативное лечение - " + patientInfo.ServiceName + ".\r\n" +
                            "Послеоперационное консервативное лечение:\r\n";
                        _paragraph.Range.ListFormat.ApplyNumberDefaultOld();
                        _paragraph.Range.ListFormat.ApplyBulletDefault(ref _missingObject);
                        _paragraph.Range.ParagraphFormat.FirstLineIndent = 0;
                        object index = 2;
                        _paragraph.Range.ParagraphFormat.TabStops.get_Item(ref index).Position = 50;
                        _paragraph.Range.Text = "медикаментозное лечение: анальгетики, антибиотики\r\n" +
                            "перевязки, ЛФК\r\n";
                        _paragraph.Range.ListFormat.ApplyBulletDefaultOld();
                        _paragraph.Range.ParagraphFormat.FirstLineIndent = -18;
                        _paragraph.Range.Text = "4.\tАмбулаторное долечивание.";
                    }
                    else
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Font.Bold = 0;
                        _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                        _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                        _paragraph.Range.Text = "Обследование: " + patientInfo.MedicalInspectionInspectionPlan + ".\r\n" +                            
                            "Консервативное лечение:\r\n";
                        _paragraph.Range.ListFormat.ApplyNumberDefaultOld();
                        _paragraph.Range.ListFormat.ApplyBulletDefault(ref _missingObject);
                        _paragraph.Range.ParagraphFormat.FirstLineIndent = 0;
                        object index = 2;
                        _paragraph.Range.ParagraphFormat.TabStops.get_Item(ref index).Position = 50;
                        _paragraph.Range.Text = " лечебно-охранительный режим\r\n" +
                            " медикаментозное лечение: анальгетики\r\n" +
                            " ЛФК, массаж\r\n";
                        _paragraph.Range.ListFormat.ApplyBulletDefaultOld();
                        _paragraph.Range.ParagraphFormat.FirstLineIndent = -18;
                        _paragraph.Range.Text = "3.\tАмбулаторное долечивание.";
                    }
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                if (patientInfo.MedicalInspectionIsPlanEnabled)
                {
                    _paragraph.Range.ListFormat.RemoveNumbers();
                }
                _paragraph.Range.Text = "Назначения: отражены в листе назначений";

                AddEmptyParagraph();

                AddDoctorForSign(patientInfo.DoctorInChargeOfTheCase);

                if (patientInfo.MedicalInspectionWithBoss)
                {
                    AddBossForSign();
                }

                AddEmptyParagraph();

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private string GetExpertAnamnes(PatientClass patientInfo, bool forChildren = false)
        {
            if (patientInfo.MedicalInspectionExpertAnamnese == 1)
            {
                if (forChildren)
                {
                    return $"л/н выдан амбулаторно по уходу за ребенком с {ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnWithNumberDateStart)} по {ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnWithNumberDateEnd)}";
                }

                return string.Format(
                        "л/н выдан амбулаторно с {0} по {1}, всего дней нетрудоспособности {2}",
                        ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnWithNumberDateStart),
                        ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnWithNumberDateEnd),
                        ConvertEngine.GetDiffInDays(patientInfo.MedicalInspectionLnWithNumberDateEnd, patientInfo.MedicalInspectionLnWithNumberDateStart) + 1);
            }
            
            if (patientInfo.MedicalInspectionExpertAnamnese == 2)
            {
                if (forChildren)
                {
                    return $"л/н по уходу за ребенком открыт первично с {ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnFirstDateStart)}";
                }

                return string.Format(
                    "л/н открыт первично с {0}",
                    ConvertEngine.GetRightDateString(patientInfo.MedicalInspectionLnFirstDateStart));
            }
            
            return "л/н не требуется.";
        }
        
        /// <summary>
        /// Экспортировать в Word протокол операции
        /// </summary>
        /// <param name="operationInfo">Информация об операции</param>
        /// <param name="patientInfo">Информация о пациенте</param>
        public void ExportOperationProtocol(OperationClass operationInfo, PatientClass patientInfo)
        {
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _waitForm.SetProgress(20);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ПРЕДОПЕРАЦИОННЫЙ ЭПИКРИЗ";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осмотр " + _dbEngine.GlobalSettings.BossJobTitle;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Дата " + ConvertEngine.GetRightDateString(operationInfo.DataOfOperation) +
                                        ", " + ConvertEngine.GetRightTimeString(operationInfo.StartTimeOfOperation.AddMinutes(-30));
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Пациент {0}, {1}, ({2})", 
                    patientInfo.GetFullName(), 
                    patientInfo.Age + " " + ConvertEngine.GetAgeString(patientInfo.Age),
                    ConvertEngine.GetRightDateString(patientInfo.Birthday));
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = string.Format("Дата поступления {0}, время поступления {1}",
                    ConvertEngine.GetRightDateString(patientInfo.DeliveryDate),
                    ConvertEngine.GetRightTimeString(patientInfo.DeliveryDate));
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 9, 10 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Диагноз:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                string[] diagnose = patientInfo.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Особенности анамнеза: " + (string.IsNullOrEmpty(patientInfo.MedicalInspectionAnamneseAnMorbi) ? "нет" : patientInfo.MedicalInspectionAnamneseAnMorbi);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Аллергические реакции на лекарственные препараты: " + patientInfo.MedicalInspectionAnamneseTextBoxes[7];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Постоянный приём лекарственных препаратов: " + patientInfo.MedicalInspectionAnamneseTextBoxes[8];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Наличие имплантированных медицинских изделий: " + patientInfo.MedicalInspectionAnamneseTextBoxes[9];


                if (operationInfo.BeforeOperationEpicrisisIsDairyEnabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Жалобы: " + operationInfo.BeforeOperationEpicrisisComplaints;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Температура тела - " + operationInfo.BeforeOperationEpicrisisTemperature;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Состояние " + operationInfo.BeforeOperationEpicrisisState +
                        ". Пульс " + operationInfo.BeforeOperationEpicrisisPulse +
                        " в мин., АД " + operationInfo.BeforeOperationEpicrisisADFirst + "/" +
                        operationInfo.BeforeOperationEpicrisisADSecond + " мм.рт.ст., ЧДД " +
                        operationInfo.BeforeOperationEpicrisisChDD + " в мин.";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "В легких дыхание " + operationInfo.BeforeOperationEpicrisisBreath +
                        ", хрипы - " + operationInfo.BeforeOperationEpicrisisWheeze;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Тоны сердца " + operationInfo.BeforeOperationEpicrisisHeartSounds +
                        ", ритм " + operationInfo.BeforeOperationEpicrisisHeartRhythm;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    string[] stLocalisLines = operationInfo.BeforeOperationEpicrisisStLocalis.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                    _paragraph.Range.Text = "St.localis: " + stLocalisLines[0];
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                    if (stLocalisLines.Length > 1)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = stLocalisLines[1];
                    }
                }

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Результаты медицинского обследования: общий анализ крови, общий анализ мочи, ЭКГ – без существенной патологии.";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Показано оперативное лечение. Планируется операция: " + operationInfo.GetNameFromLowerLetter() +
                    ", под " + operationInfo.AnesthesiaType + " анестезией";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Оформлено добровольное медицинское согласие на оперативное вмешательство.\r\nПротивопоказаний нет.";

                AddDoctorForSign(patientInfo.DoctorInChargeOfTheCase);
                
                AddBossForSign();

                AddEmptyParagraph();

                if (operationInfo.BeforeOperationEpicrisisIsAntibioticProphylaxisExist)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _paragraph.Range.Font.Bold = 1;
                    _paragraph.Range.Text = "Протокол периоперационной антибиотикопрофилактики";

                    AddEmptyParagraph();

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата: " + ConvertEngine.GetRightDateString(operationInfo.DataOfOperation) +
                        ", время " + ConvertEngine.GetRightTimeString(operationInfo.StartTimeOfOperation.AddMinutes(-30));                    

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Введен антибиотик: " + operationInfo.BeforeOperationEpicrisisAntibioticProphylaxis + "\r\n";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Подпись врача____________\t\t\tПодпись м/с___________\r\n";
                }

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ПРОТОКОЛ ОПЕРАТИВНОГО ВМЕШАТЕЛЬСТВА";

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = string.Format("Пациент {0}, {1} {2} ({3})", 
                    patientInfo.GetFullName(),
                    patientInfo.Age,
                    ConvertEngine.GetAgeString(patientInfo.Age),
                    ConvertEngine.GetRightDateString(patientInfo.Birthday));
                SetWordsInRangeBold(_paragraph.Range, new[] { 2, 3, 4, 5, 6, 7 });

                DateTime startDateTime = new DateTime(
                    operationInfo.DataOfOperation.Year, 
                    operationInfo.DataOfOperation.Month, 
                    operationInfo.DataOfOperation.Day, 
                    operationInfo.StartTimeOfOperation.Hour, 
                    operationInfo.StartTimeOfOperation.Minute, 
                    0);
                DateTime endDate = operationInfo.DataOfOperation;
                if (operationInfo.EndTimeOfOperation.Hour < operationInfo.StartTimeOfOperation.Hour ||
                    (operationInfo.EndTimeOfOperation.Hour == operationInfo.StartTimeOfOperation.Hour &&
                     operationInfo.EndTimeOfOperation.Minute < operationInfo.StartTimeOfOperation.Minute))
                {
                    endDate = endDate.AddDays(1);
                }

                DateTime endDateTime = new DateTime(
                    endDate.Year, endDate.Month, endDate.Day, operationInfo.EndTimeOfOperation.Hour, operationInfo.EndTimeOfOperation.Minute, 0);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Дата и время начала оперативного вмешательства: {0} {1}",
                    ConvertEngine.GetRightDateString(startDateTime),
                    ConvertEngine.GetRightTimeString(startDateTime));

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Дата и время окончания оперативного вмешательства: {0} {1}",
                    ConvertEngine.GetRightDateString(endDateTime),
                    ConvertEngine.GetRightTimeString(endDateTime));

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Продолжительность оперативного вмешательства: " + 
                    ConvertEngine.GetRightTimeLengthString(endDateTime - startDateTime);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Название оперативного вмешательства: " + operationInfo.Name;
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3, 4 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Код услуги: " + patientInfo.ServiceCode;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Диагноз до  оперативного вмешательства:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                if (operationInfo.BeforeOperationEpicrisisIsPremedicationExist)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Премедикация: " + operationInfo.BeforeOperationEpicrisisPremedication;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Группа крови: {0}, резус-фактор: {1}", 
                    string.IsNullOrEmpty(patientInfo.DischargeEpicrisBloodGroup) ? "____" : patientInfo.DischargeEpicrisBloodGroup,
                    string.IsNullOrEmpty(patientInfo.DischargeEpicrisRhesusFactor) ? "____" : patientInfo.DischargeEpicrisRhesusFactor);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Степень риска оперативного вмешательства: " + operationInfo.RiskLevel;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "План оперативного вмешательства (операции): " + operationInfo.Name;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] operationCourse = operationInfo.OperationCourse.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Описание оперативного вмешательства: " + operationCourse[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                if (operationCourse.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = operationCourse[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = @"Осложнения, возникшие в ходе оперативного вмешательства: нет
Использование медицинских изделий (оборудования): рентгеновское
Подсчет операционного материала: инструменты – 10,	   салфетки – 20
Кровопотеря во время оперативного вмешательства: 50 мл";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Вид анестезиологического пособия: " + operationInfo.AnesthesiaType;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Диагноз после оперативного вмешательства:";


                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Имплантированные медицинские изделия: " + (operationInfo.Implants.Count > 0 ? string.Join(", ", operationInfo.Implants.ToArray()) : "нет");

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = @"Назначения: нет
Операционный материал, взятый для проведения морфологического исследования: нет
Операционный материал направлен: ---
Состав оперирующей бригады:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "оперирующий врач – " + string.Join(", ", operationInfo.Surgeons.ToArray());
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "ассистирующий врач – " + string.Join(", ", operationInfo.Assistents.ToArray());

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "операционная медицинская сестра – " + operationInfo.ScrubNurse;

                if (!string.IsNullOrEmpty(operationInfo.Orderly))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "санитар – " + operationInfo.Orderly;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "состав бригады анестезиологии-реанимации:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "врач-анестезиолог-реаниматолог – " + operationInfo.HeAnesthetist;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "медицинская сестра-анестезист – " + operationInfo.SheAnesthetist;

                AddEmptyParagraph();

                var timeOfWriting = string.Format("Дата {0}, {1}", 
                    ConvertEngine.GetRightDateString(endDate), 
                    ConvertEngine.GetRightTimeString(endDateTime.AddMinutes(60)));
                AddDoctorForSign(patientInfo.DoctorInChargeOfTheCase, timeOfWriting);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                object typeBreak = WdBreakType.wdPageBreak;
                _paragraph.Range.InsertBreak(ref typeBreak);

                _waitForm.SetProgress(75);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ОПЕРАТИВНОЕ ВМЕШАТЕЛЬСТВО №___";

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Номер карты пациента: " + patientInfo.NumberOfCaseHistory;
                SetWordsInRangeBold(_paragraph.Range, new[] { 5 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = string.Format("Пациент {0}, {1} {2} ({3})",
                    patientInfo.GetFullName(),
                    patientInfo.Age,
                    ConvertEngine.GetAgeString(patientInfo.Age),
                    ConvertEngine.GetRightDateString(patientInfo.Birthday));
                SetWordsInRangeBold(_paragraph.Range, new[] { 2, 3, 4, 5, 6, 7 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Дата и время начала оперативного вмешательства: {0} {1}",
                    ConvertEngine.GetRightDateString(startDateTime),
                    ConvertEngine.GetRightTimeString(startDateTime));

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format("Дата и время окончания оперативного вмешательства: {0} {1}",
                    ConvertEngine.GetRightDateString(endDateTime),
                    ConvertEngine.GetRightTimeString(endDateTime));

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Продолжительность оперативного вмешательства: " +
                    ConvertEngine.GetRightTimeLengthString(endDateTime - startDateTime);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Диагноз до  оперативного вмешательства: ";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Название оперативного вмешательства: " + operationInfo.Name;
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3, 4 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Код услуги: " + patientInfo.ServiceCode;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Высокотехнологичная медицинская помощь: нет";
                SetWordsInRangeBold(_paragraph.Range, new[] { 5 });

                if (operationInfo.BeforeOperationEpicrisisIsPremedicationExist)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Премедикация: " + operationInfo.BeforeOperationEpicrisisPremedication;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Вид анестезиологического пособия: " + operationInfo.AnesthesiaType;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Описание оперативного вмешательства: " + operationCourse[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                if (operationCourse.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = operationCourse[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = @"Исход оперативного вмешательства: благоприятный
Операционный материал, взятый для проведения морфологического исследования: нет
Осложнения, возникшие в ходе оперативного вмешательства: нет
Диагноз после оперативного вмешательства:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Основное заболевание: " + diagnose[0];
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осложнения основного заболевания: " + (string.IsNullOrEmpty(patientInfo.Complications) ? "нет" : patientInfo.Complications);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Внешняя причина при травмах, отравлениях: " + (string.IsNullOrEmpty(patientInfo.WWW) ? "нет" : patientInfo.WWW);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сопутствующие заболевания: " + (string.IsNullOrEmpty(patientInfo.ConcomitantDiagnose) ? "нет" : patientInfo.ConcomitantDiagnose);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Состав оперирующей бригады:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "оперирующий врач – " + string.Join(", ", operationInfo.Surgeons.ToArray());
                SetWordsInRangeRegular(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "ассистирующий врач – " + string.Join(", ", operationInfo.Assistents.ToArray());

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "операционная медицинская сестра – " + operationInfo.ScrubNurse;

                if (!string.IsNullOrEmpty(operationInfo.Orderly))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "санитар – " + operationInfo.Orderly;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "состав бригады анестезиологии-реанимации:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "врач-анестезиолог-реаниматолог – " + operationInfo.HeAnesthetist;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "медицинская сестра-анестезист – " + operationInfo.SheAnesthetist;

                AddEmptyParagraph();

                AddDoctorForSign(patientInfo.DoctorInChargeOfTheCase, timeOfWriting);

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private void AddDoctorForSign(string doctorInChargeOfTheCase, string endStr = "")
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Text = string.Format("Врач, {0}: {1}__________________{2}",
                _dbEngine.GetSpecialityBySurgeonName(doctorInChargeOfTheCase),
                doctorInChargeOfTheCase,
                endStr);
        }

        private void AddBossForSign()
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            string bossTitleFormat = _dbEngine.GlobalSettings.BossJobTitle.Contains("главн") ? "Глав. врач, {0}: {1}__________________" : "Зав. отделением, {0}: {1}__________________";
            _paragraph.Range.Text = string.Format(bossTitleFormat,
                _dbEngine.GetSpecialityBySurgeonName(_dbEngine.GlobalSettings.BranchManager),
                _dbEngine.GlobalSettings.BranchManager);
        }

        private void MergeCells(object begCell, object endCell)
        {
            _wordRange = _wordDoc.Range(ref begCell, ref endCell);
            _wordRange.Select();
            _wordApp.Selection.Cells.Merge();
        }

        private void SetColumnWidths(int rowCnt, int[] widhts)
        {
            for (int i = 1; i <= widhts.Length; i++)
            {
                object begCell = _wordTable.Cell(1, i).Range.Start;
                object endCell = _wordTable.Cell(rowCnt, i).Range.End;

                _wordRange = _wordDoc.Range(ref begCell, ref endCell);
                _wordRange.Select();
                _wordApp.Selection.Cells.Width = widhts[i - 1];
            }
        }

        /// <summary>
        /// Экспорт листа назначений
        /// </summary>
        /// <param name="patientInfo">Данные о пациенте</param>
        public void ExportPrescriptionTherapy(PatientClass patientInfo)
        {
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.Orientation = WdOrientation.wdOrientPortrait;
                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _wordRange = _paragraph.Range;
                _wordTable = _wordDoc.Tables.Add(_wordRange, 2, 2, ref _missingObject, ref _missingObject);
                _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;
                _wordTable.Range.Font.Name = "Times New Roman";
                _wordTable.Range.Font.Size = 12;
                _wordTable.Range.Font.Bold = 0;

                for (int i = 1; i <= _wordTable.Rows.Count; i++)
                {
                    _wordTable.Rows[i].Cells[1].Width = 275;
                    _wordTable.Rows[i].Cells[2].Width = 275;
                }

                _wordTable.Cell(1, 1).Range.Text = "травматолого-ортопедическое отделение";
                _wordTable.Cell(2, 1).Range.Text = string.Format("ФИО   {0} {1} {2}",
                    patientInfo.GetFullName(), patientInfo.Age, ConvertEngine.GetAgeString(patientInfo.Age));
                _wordTable.Cell(1, 2).Range.Text = "палата №________________________";
                _wordTable.Cell(2, 2).Range.Text = "Диагноз: " + patientInfo.Diagnose;

                _wordDoc.Paragraphs.Add(ref _missingObject);                
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ЛИСТОК ВРАЧЕБНЫХ НАЗНАЧЕНИЙ";

                _waitForm.SetProgress(50);

                int rowCnt = 40;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 11;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordRange = _paragraph.Range;
                _wordTable = _wordDoc.Tables.Add(_wordRange, rowCnt, 6, ref _missingObject, ref _missingObject);

                _wordTable.Range.Font.Name = "Times New Roman";
                _wordTable.Range.Font.Size = 11;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                SetColumnWidths(rowCnt, new[] { 25, 145, 145, 65, 65, 40 });

                _wordTable.Cell(1, 1).Range.Text = "№ п/п";
                _wordTable.Cell(1, 1).Range.Font.Size = 10;
                _wordTable.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _wordTable.Cell(1, 2).Range.Text = "Наименование";
                _wordTable.Cell(1, 2).Range.Font.Size = 10;
                _wordTable.Cell(1, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _wordTable.Cell(1, 3).Range.Text = "Способ применения";
                _wordTable.Cell(1, 3).Range.Font.Size = 10;
                _wordTable.Cell(1, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                
                _wordTable.Cell(1, 4).Range.Text = "Дата назначения";
                _wordTable.Cell(1, 4).Range.Font.Size = 10;
                _wordTable.Cell(1, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _wordTable.Cell(1, 5).Range.Text = "Дата отмены";
                _wordTable.Cell(1, 5).Range.Font.Size = 10;
                _wordTable.Cell(1, 5).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _waitForm.SetProgress(70);

                for (int i = 0; i < patientInfo.PrescriptionTherapy.Count; i++)
                {
                    string[] data = patientInfo.PrescriptionTherapy[i].Split('&');
                    string[] nameAndApply = data[0].Split('–');

                   // _wordTable.Cell(i + 2, 1).Range.Font.Size = 11;
                    _wordTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _wordTable.Cell(i + 2, 1).Range.Text = (i + 1).ToString();

                    //_wordTable.Cell(i + 2, 2).Range.Font.Size = 11;
                    _wordTable.Cell(i + 2, 2).Range.Text = nameAndApply[0].Trim(' ');

                    if (nameAndApply.Length > 1)
                    {
                        //_wordTable.Cell(i + 2, 3).Range.Font.Size = 11;
                        _wordTable.Cell(i + 2, 3).Range.Text = nameAndApply[1].Trim(' ');
                    }

                    //_wordTable.Cell(i + 2, 4).Range.Font.Size = 11;
                    _wordTable.Cell(i + 2, 4).Range.Text = data[2];

                    if (!string.IsNullOrEmpty(data[1]))
                    {
                        DateTime endDate = ConvertEngine.GetDateTimeFromString(data[2]);
                        _wordTable.Cell(i + 2, 5).Range.Text = ConvertEngine.GetRightDateString(endDate.AddDays(Convert.ToInt32(data[1])));
                    }
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        /// <summary>
        /// Экспорт листа дополнительных методов обследования
        /// </summary>
        /// <param name="patientInfo">Данные о пациенте</param>
        public void ExportSurveys(PatientClass patientInfo)
        {
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.Add(ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _waitForm.SetProgress(10);

                _wordDoc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 20;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _waitForm.SetProgress(30);

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 16;
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ЛИСТ НАЗНАЧЕНИЙ ДОПОЛНИТЕЛЬНЫХ МЕТОДОВ ОБСЛЕДОВАНИЯ";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 18;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                _paragraph.Range.Text = string.Format("И.Б. {0} {1} {2} {3}, {4} отделение",
                    patientInfo.NumberOfCaseHistory, patientInfo.GetFullName(), patientInfo.Age, ConvertEngine.GetAgeString(patientInfo.Age), _dbEngine.GlobalSettings.DepartmentName);

                _waitForm.SetProgress(50);

                int rowCnt = 22;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordRange = _paragraph.Range;
                _wordTable = _wordDoc.Tables.Add(_wordRange, rowCnt, 4, ref _missingObject, ref _missingObject);

                _wordTable.Range.Font.Name = "Times New Roman";
                _wordTable.Range.Font.Size = 14;
                _wordTable.Range.Font.Bold = 0;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordTable.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                _wordTable.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                
                SetColumnWidths(rowCnt, new[] { 470, 75, 65, 65 });
                /*
                for (int i = 1; i <= _wordTable.Rows.Count; i++)
                {
                    _wordTable.Rows[i].Cells[1].Width = 470;
                    _wordTable.Rows[i].Cells[2].Width = 75;
                    _wordTable.Rows[i].Cells[3].Width = 65;
                    _wordTable.Rows[i].Cells[4].Width = 65;
                }
                */
                object begCell;
                object endCell;

                _wordTable.Cell(1, 1).Range.Text = "Анализы, дополнительные методы обследования, консультации, др. назначения";
                begCell = _wordTable.Cell(1, 1).Range.Start;
                endCell = _wordTable.Cell(2, 1).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _wordTable.Cell(1, 2).Range.Text = "Назначение";
                begCell = _wordTable.Cell(1, 2).Range.Start;
                endCell = _wordTable.Cell(1, 4).Range.End;
                MergeCells(begCell, endCell);
                _wordTable.Cell(1, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                _wordTable.Cell(2, 2).Range.Text = "Дата";
                _wordTable.Cell(2, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordTable.Cell(2, 3).Range.Text = "Врач";
                _wordTable.Cell(2, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordTable.Cell(2, 4).Range.Text = "м/с";
                _wordTable.Cell(2, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;               

                _waitForm.SetProgress(70);

                for (int i = 0; i < patientInfo.PrescriptionSurveys.Count; i++)
                {
                    string[] data = patientInfo.PrescriptionSurveys[i].Split('&');
                    _wordTable.Cell(i + 3, 1).Range.Text = data[0];
                    _wordTable.Cell(i + 3, 2).Range.Text = data[1];
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        /// <summary>
        /// Открыть в ворде указанный документ и провести замену специальных значений в скобках
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="patientInfo">Информация о пациенте</param>
        ///  <param name="setEmptyMissingObjects">Оставлять пустыми места, если не указан вставляемый параметр</param>
        public void ExportAdditionalDocument(object filePath, PatientClass patientInfo, bool setEmptyMissingObjects)
        {
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                try
                {
                    // Пробуем для 2007 офиса выставить стиль документов 2003 офиса.
                    // Для других офисов, вероятно, отвалимся с ошибкой, но для них и не
                    // надо ничего делать.
                    _wordDoc.ApplyQuickStyleSet("Word 2003");
                }
                catch
                {
                }

                _wordDoc = _wordApp.Documents.Add(filePath);

                _waitForm.SetProgress(10);
                double shift = 90.0 / (_wordDoc.Content.Paragraphs.Count + _wordDoc.Shapes.Count);
                double previousValue = 10;
                double currentValue = previousValue;

                for (int num = 1; num <= _wordDoc.Content.Paragraphs.Count; num++)
                {
                    Paragraph paragraph = _wordDoc.Content.Paragraphs[num];
                    FindMarkAndReplace(
                        paragraph.Range.Text,
                        null,
                        shift,
                        ref previousValue,
                        ref currentValue,
                        patientInfo,
                        setEmptyMissingObjects);
                }

                foreach (Shape shape in _wordDoc.Shapes)
                {
                    FindMarkAndReplace(
                        shape.TextFrame.TextRange.Text,
                        shape,
                        shift,
                        ref previousValue,
                        ref currentValue,
                        patientInfo,
                        setEmptyMissingObjects);
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);

                var newDocumentPath = GetNewAdditionalDocumentFilePath(filePath.ToString(), patientInfo.GetFullName());
                /*
                 Изменение имени документа без сохранения
                var dialog = _wordApp.Dialogs[WdWordDialog.wdDialogFileSummaryInfo];
                dialog.GetType().InvokeMember("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, dialog, new object[] { newDocumentPath });
                dialog.Execute();*/

                _wordDoc.SaveAs(newDocumentPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private string GetNewAdditionalDocumentFilePath(string templateFilePath, string newFileName)
        {
            FileInfo fileInfo = new FileInfo(templateFilePath);
            var now = DateTime.Now;
            string resultDirectory = Path.Combine(fileInfo.DirectoryName ?? "", "Generated"); // now.ToString("yyyy_MM_dd_HH_mm_ss")
            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            string newFilePath = Path.Combine(resultDirectory, newFileName);
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }

            return newFilePath;
        }

        private void FindMarkAndReplace(
           string rangeText,
           Shape shape,
           double shift,
           ref double previousValue,
           ref double currentValue,
           PatientClass patientInfo,
           bool setEmptyMissingObjects)
        {
            int closeBracketNumber = -1;
            bool isTextChanged = false;
            for (int i = rangeText.Length - 1; i >= 0; i--)
            {
                if (rangeText[i] == '}')
                {
                    closeBracketNumber = i;
                    continue;
                }

                if (rangeText[i] == '\n')
                {
                    closeBracketNumber = -1;
                    continue;
                }

                if (closeBracketNumber > -1 && rangeText[i] == '{')
                {
                    isTextChanged = true;
                    int startIndex = i;
                    int endIndex = closeBracketNumber + 1;

                    string bracketText = rangeText.Substring(startIndex, endIndex - startIndex);
                    string bracketNewText = GetRealParameterInsteadSpecialMark(bracketText, patientInfo, setEmptyMissingObjects);

                    if (shape == null)
                    {
                        FindAndReplace(bracketText, bracketNewText);
                    }

                    rangeText = rangeText.Substring(0, startIndex) + bracketNewText + rangeText.Substring(endIndex);

                    closeBracketNumber = -1;
                }
            }

            if (isTextChanged && shape != null)
            {
                rangeText = rangeText.TrimEnd('\r', '\n', '\a', ' ');
                shape.TextFrame.TextRange.Text = rangeText;
            }

            currentValue += shift;
            if (currentValue > previousValue)
            {
                previousValue = currentValue;
                _waitForm.SetProgress(previousValue);
            }
        }

        private int GetOperationNumber(PatientClass patientInfo, string mark)
        {
            // Получаем номер операции, начинающийся с 1
            string[] data = mark.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int number = Convert.ToInt32(data[data.Length - 1].Trim(' '));
            if (patientInfo.Operations.Count < number || number < 1)
            {
                throw new Exception("НЕТ ОПЕРАЦИИ С НОМЕРОМ " + number);
            }

            // Возвращаем номер операции, начинающийся с 0
            return number - 1;
        }

        /// <summary>
        /// Вернуть нужное значение вместо параметра в документе
        /// </summary>
        /// <param name="mark">Метка в документе</param>
        /// <param name="patientInfo">Информация о пациенте</param>
        ///  <param name="setEmptyMissingObjects">Оставлять пустыми объекты, если не найдено для них значения</param>
        /// <returns></returns>
        private string GetRealParameterInsteadSpecialMark(string mark, PatientClass patientInfo, bool setEmptyMissingObjects)
        {
            if (!mark.StartsWith("{") || !mark.EndsWith("}"))
            {
                return string.Empty;
            }

            mark = mark.Trim(new[] { '{', '}', ' ' }).ToLower();
            var sb = new StringBuilder();
            foreach (char ch in mark)
            {
                if (ch != ' ' || sb[sb.Length - 1] != ' ')
                {
                    sb.Append(ch);
                }
            }

            mark = sb.ToString();

            try
            {
                int operationNumber;
                if (mark.StartsWith("дата операции"))
                {
                    operationNumber = GetOperationNumber(patientInfo, mark);
                    return ConvertEngine.GetRightDateString(patientInfo.Operations[operationNumber].DataOfOperation);
                }

                if (mark.StartsWith("время начала операции"))
                {
                    operationNumber = GetOperationNumber(patientInfo, mark);
                    return ConvertEngine.GetRightTimeString(patientInfo.Operations[operationNumber].StartTimeOfOperation);
                }

                if (mark.StartsWith("время окончания операции"))
                {
                    operationNumber = GetOperationNumber(patientInfo, mark);
                    return ConvertEngine.GetRightTimeString(patientInfo.Operations[operationNumber].EndTimeOfOperation);
                }

                if (mark.StartsWith("название операции"))
                {
                    operationNumber = GetOperationNumber(patientInfo, mark);
                    return patientInfo.Operations[operationNumber].Name;
                }
            }
            catch (Exception ex)
            {
                return setEmptyMissingObjects ? "" : "{" + ex.Message.ToUpper() + "}";
            }

            switch (mark)
            {
                case "фио пациента":
                    return patientInfo.GetFullName();
                case "возраст":
                    return patientInfo.Age;
                case "адрес":
                    return patientInfo.GetAddress();
                case "дата рождения":
                    return ConvertEngine.GetRightDateString(patientInfo.Birthday);
                case "дата поступления":
                    return ConvertEngine.GetRightDateString(patientInfo.DeliveryDate);
                case "время поступления":
                    return ConvertEngine.GetRightTimeString(patientInfo.DeliveryDate);
                case "дата выписки":
                    if (patientInfo.ReleaseDate.HasValue)
                    {
                        return ConvertEngine.GetRightDateString(patientInfo.ReleaseDate.Value);
                    }

                    return setEmptyMissingObjects ? "" : "{ДАТА ВЫПИСКИ НЕ УКАЗАНА}";
                case "койко дни":
                    if (patientInfo.ReleaseDate.HasValue)
                    {
                        TimeSpan threatmentPeriod = patientInfo.ReleaseDate.Value - patientInfo.DeliveryDate;
                        return threatmentPeriod.Days.ToString();
                    }

                    return setEmptyMissingObjects ? "" : "{КОЙКО ДНИ НЕВЫЧИСЛИМЫ Т.К. ДАТА ВЫПИСКИ НЕ УКАЗАНА}";
                case "диагноз":
                    return patientInfo.Diagnose;
                case "консервативная терапия":
                    return patientInfo.GetDischargeEpicrisConservativeTherapy();
                case "№ иб":
                    return patientInfo.NumberOfCaseHistory;
                case "№ отделения":
                    return _dbEngine.GlobalSettings.DepartmentName;
                case "фио лечащего врача":
                    return patientInfo.DoctorInChargeOfTheCase;
                case "мкб":
                    return patientInfo.MKB;
                case "название услуги":
                    if (!string.IsNullOrEmpty(patientInfo.ServiceName))
                    {
                        return patientInfo.ServiceName;
                    }

                    return setEmptyMissingObjects ? "" : "{НАЗВАНИЕ УСЛУГИ НЕ УКАЗАНО}";
                case "код услуги":
                    if (!string.IsNullOrEmpty(patientInfo.ServiceCode))
                    {
                        return patientInfo.ServiceCode;
                    }

                    return setEmptyMissingObjects ? "" : "{КОД УСЛУГИ НЕ УКАЗАН}";
                case "код ксг":
                    if (!string.IsNullOrEmpty(patientInfo.KsgCode))
                    {
                        return patientInfo.KsgCode;
                    }

                    return setEmptyMissingObjects ? "" : "{КОД КСГ НЕ УКАЗАН}";
                case "расшифровка ксг":
                    if (!string.IsNullOrEmpty(patientInfo.KsgDecoding))
                    {
                        return patientInfo.KsgDecoding;
                    }

                    return setEmptyMissingObjects ? "" : "{РАСШИФРОВКА КСГ НЕ УКАЗАНА}";
                case "работа":
                    return patientInfo.WorkPlace;
                case "паспорт":
                    return patientInfo.PassportNumber;
                case "полис":
                    return patientInfo.PolisNumber;
                case "снилс":
                    return patientInfo.SnilsNumber;
                case "фио зав. отделением":
                    return _dbEngine.GlobalSettings.BranchManager;
                case "cегодняшняя дата":
                    return ConvertEngine.GetRightDateString(DateTime.Now);
                case "хирург":
                    if (patientInfo.Operations.Count > 0)
                    {
                        return ConvertEngine.ListToString(patientInfo.Operations[0].Surgeons, ",");
                    }

                    return setEmptyMissingObjects ? "" : "{ХИРУРГ НЕ НАЙДЕН, Т.К. НЕТ ОПЕРАЦИЙ}";
                case "анестезиолог":
                    if (patientInfo.Operations.Count > 0)
                    {
                        return patientInfo.Operations[0].HeAnesthetist;
                    }

                    return setEmptyMissingObjects ? "" : "{АНЕСТЕЗИОЛОГ НЕ НАЙДЕН, Т.К. НЕТ ОПЕРАЦИЙ}";
                default:
                    return setEmptyMissingObjects ? "" : "{" + mark.ToUpper() + "}";
            }
        }

        /// <summary>
        /// Найти и заменить в документе
        /// </summary>
        /// <param name="findText">Текст для поиска</param>
        /// <param name="replaceText">Текст для замены</param>
        private void FindAndReplace(string findText, string replaceText)
        {
            // Find the first instance of the findText
            var range = _wordApp.ActiveDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(findText);

            // Replace all instances of findText with replaceText
            while (range.Find.Found)
            {
                range.Text = replaceText;
                range.Collapse(WdCollapseDirection.wdCollapseEnd);
                range.Find.Execute(findText);
            }
        }

        /// <summary>
        /// Освободить ресурсы после генерации документа
        /// </summary>
        private void ReleaseComObject()
        {
            if (_wordApp != null)
            {
                _wordApp.Visible = true;
                _wordApp.Activate();

                if (_wordDoc != null)
                {
                    Marshal.ReleaseComObject(_wordDoc);
                    _wordDoc = null;
                }

                if (_wordShape != null)
                {
                    Marshal.ReleaseComObject(_wordShape);
                    _wordShape = null;
                }

                if (_paragraph != null)
                {
                    Marshal.ReleaseComObject(_paragraph);
                    _paragraph = null;
                }

                if (_wordRange != null)
                {
                    Marshal.ReleaseComObject(_wordRange);
                    _wordRange = null;
                }

                if (_wordTable != null)
                {
                    Marshal.ReleaseComObject(_wordTable);
                    _wordTable = null;
                }

                Marshal.ReleaseComObject(_wordApp);
                _wordApp = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Добавляем в текущий документ пустой параграф
        /// </summary>
        private void AddEmptyParagraph()
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Bold = 0;
            _paragraph.Range.Text = string.Empty;
        }

        /// <summary>
        /// Сделать жирным переданные номера слов в текущем Range
        /// </summary>
        /// <param name="range">Range, в котором выделяем слова</param>
        /// <param name="wordNumbers">Номера слов в Range, которые надо сделать жирными</param>
        private void SetWordsInRangeBold(Range range, IEnumerable<int> wordNumbers)
        {
            foreach (int wordNumber in wordNumbers)
            {
                if (range.Words.Count > wordNumber)
                {
                    range.Words[wordNumber].Bold = 1;
                }
            }
        }

        /// <summary>
        /// Сделать обычным переданные номера слов в текущем Range
        /// </summary>
        /// <param name="range">Range, в котором выделяем слова</param>
        /// <param name="wordNumbers">Номера слов в Range, которые надо сделать обычным</param>
        private void SetWordsInRangeRegular(Range range, IEnumerable<int> wordNumbers)
        {
            foreach (int wordNumber in wordNumbers)
            {
                if (range.Words.Count > wordNumber)
                {
                    range.Words[wordNumber].Bold = 0;
                }
            }
        }

        /// <summary>
        /// Сделать подчёркнутыми переданные номера слов в текущем Range
        /// </summary>
        /// <param name="range">Range, в котором выделяем слова</param>
        /// <param name="wordNumbers">Номера слов в Range, которые надо сделать жирными</param>
        private void SetWordsInRangeUnderline(Range range, IEnumerable<int> wordNumbers)
        {
            foreach (int wordNumber in wordNumbers)
            {
                if (range.Words.Count > wordNumber)
                {
                    range.Words[wordNumber].Underline = WdUnderline.wdUnderlineSingle;
                }
            }
        }
    }
}
