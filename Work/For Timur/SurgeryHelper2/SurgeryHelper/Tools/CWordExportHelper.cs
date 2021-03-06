using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;

using SurgeryHelper.Essences;
using SurgeryHelper.Forms;
using SurgeryHelper.Workers;

using Application = Microsoft.Office.Interop.Word.Application;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace SurgeryHelper.Tools
{
    public class CWordExportHelper
    {
        private static Application _wordApp;
        private static Document _wordDoc;
        private static Paragraph _paragraph;
        private static Range _wordRange;
        private static Table _wordTable;
        private static Shape _wordShape;

        private static object _missingObject = Type.Missing;

        private static WaitForm _waitForm;

        private static bool _blockDocGeneration;

        /// <summary>
        /// Экспортировать в Word переводной эпикриз
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="hospitalizationInfo">Информация о госпитализации</param>
        /// /// <param name="operationWorker">Объект для работы с операциями</param>
        /// <param name="transferableEpicrisisInfo">Информация о переводном экпкризе</param>
        /// <param name="globalSettings">Глобальные настройки</param>
        public static void ExportTransferableEpicrisis(
            CPatient patientInfo,
            CHospitalization hospitalizationInfo,
            COperationWorker operationWorker,
            CTransferableEpicrisis transferableEpicrisisInfo,
            CGlobalSettings globalSettings)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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
                    "Пациент {0}, {1} лет, находится на лечении в {2} х.о. с {3} с диагнозом: {4}\r\n",
                    patientInfo.GetFullName(),
                    CConvertEngine.GetAge(patientInfo.Birthday),
                    globalSettings.DepartmentName,
                    CConvertEngine.DateTimeToString(hospitalizationInfo.DeliveryDate),
                    hospitalizationInfo.Diagnose);

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Проведено лечение:";

                var textStr = new StringBuilder();
                foreach (COperation operationInfo in operationWorker.GetListByHospitalizationId(hospitalizationInfo.Id))
                {
                    textStr.AppendFormat("{0} - {1}\r\n", CConvertEngine.DateTimeToString(operationInfo.DateOfOperation), operationInfo.Name);
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

                if (!string.IsNullOrEmpty(transferableEpicrisisInfo.AdditionalInfo))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = transferableEpicrisisInfo.AdditionalInfo + "\r\n";
                }

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Послеоперационный период " + transferableEpicrisisInfo.AfterOperationPeriod;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Планируется:";

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = transferableEpicrisisInfo.Plan;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Для дальнейшего лечения в удовлетворительном состоянии переводится на дневной стационар.";

                _waitForm.SetProgress(70);

                if (transferableEpicrisisInfo.IsIncludeDisabilityList)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Л/н № {0} продлен с {1} по {2}.\r\n",
                        transferableEpicrisisInfo.DisabilityList,
                        CConvertEngine.DateTimeToString(transferableEpicrisisInfo.WritingDate.AddDays(1)),
                        CConvertEngine.DateTimeToString(transferableEpicrisisInfo.WritingDate.AddDays(10)));
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "С режимом ознакомлен _____________________________\r\n";

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(transferableEpicrisisInfo.WritingDate);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + globalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + hospitalizationInfo.DoctorInChargeOfTheCase;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word этапный эпикриз
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="hospitalizationInfo">Информация о госпитализации</param>
        /// <param name="operationWorker">Объект для работы с операциями</param>
        /// <param name="lineOfCommunicationEpicrisisInfo">Информация о этапном экпкризе</param>
        /// <param name="globalSettings">Глобальные настройки</param>
        public static void ExportLineOfCommunicationEpicrisis(
            CPatient patientInfo,
            CHospitalization hospitalizationInfo,
            COperationWorker operationWorker,
            CLineOfCommunicationEpicrisis lineOfCommunicationEpicrisisInfo,
            CGlobalSettings globalSettings)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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
                    "Пациент {0}, {1} лет, находится на лечении в {2} х.о. с {3} с диагнозом: {4}\r\n",
                    patientInfo.GetFullName(),
                    CConvertEngine.GetAge(patientInfo.Birthday),
                    globalSettings.DepartmentName,
                    CConvertEngine.DateTimeToString(hospitalizationInfo.DeliveryDate),
                    hospitalizationInfo.Diagnose);

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Проведено лечение:";

                var textStr = new StringBuilder();
                foreach (COperation operationInfo in operationWorker.GetListByHospitalizationId(hospitalizationInfo.Id))
                {
                    textStr.AppendFormat("{0} - {1}\r\n", CConvertEngine.DateTimeToString(operationInfo.DateOfOperation), operationInfo.Name);
                }

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = textStr.ToString();

                if (!string.IsNullOrEmpty(lineOfCommunicationEpicrisisInfo.AdditionalInfo))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = lineOfCommunicationEpicrisisInfo.AdditionalInfo + "\r\n";
                }

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = "Планируется:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = lineOfCommunicationEpicrisisInfo.Plan + "\r\n";

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(lineOfCommunicationEpicrisisInfo.WritingDate);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + globalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + hospitalizationInfo.DoctorInChargeOfTheCase;
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
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word выписной эпикриз
        /// </summary>
        /// <param name="patient">Информация о пациенте</param>
        /// <param name="hospitalization">Информация о госпитализации</param>
        /// <param name="dischargeEpicrisis">Информация о выписном эпикризе</param>
        /// <param name="medicalInspection">Информация об осмотре в отделении</param>
        /// <param name="operationWorker">Класс с данными по всем операциям</param> 
        /// <param name="globalSettings">Глобальные настройки</param>        
        /// <param name="dischargeEpicrisisHeaderFilePath">Путь до файла с шапкой для выписного эпикриза</param>
        public static void ExportDischargeEpicrisis(
            CPatient patient,
            CHospitalization hospitalization,
            CDischargeEpicrisis dischargeEpicrisis,
            CMedicalInspection medicalInspection,
            COperationWorker operationWorker,
            CGlobalSettings globalSettings,
            object dischargeEpicrisisHeaderFilePath)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
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
                        patient,
                        hospitalization,
                        null,
                        operationWorker,
                        dischargeEpicrisis,
                        globalSettings);
                }

                _waitForm.SetProgress(30);

                object start = 0;
                object end = 0;
                _wordRange = _wordDoc.Range(ref start, ref end);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Ф.И.О. " + patient.GetFullName() + ", " + CConvertEngine.GetAge(patient.Birthday) + " лет";
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата поступления " + CConvertEngine.DateTimeToString(hospitalization.DeliveryDate);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                if (hospitalization.ReleaseDate.HasValue)
                {
                    _paragraph.Range.Text = "Дата выписки " + CConvertEngine.DateTimeToString(hospitalization.ReleaseDate.Value);
                }
                else
                {
                    _paragraph.Range.Text = "Дата выписки " + "НЕ УКАЗАНА";
                }

                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] diagnoseLines = hospitalization.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Диагноз " + diagnoseLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                if (diagnoseLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnoseLines[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] complaints = medicalInspection.Complaints.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Жалобы " + complaints[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                if (complaints.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = complaints[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] anMorbi = medicalInspection.AnamneseAnMorbi.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "An. morbi. " + anMorbi[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                if (anMorbi.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = anMorbi[1];
                }

                AddEmptyParagraph();

                _waitForm.SetProgress(50);

                // Добавляем информацию об операциях
                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 1;
                _paragraph.Range.Text = "Проведенное лечение:";

                string textStrFirstLine = string.Empty;
                var textStr = new StringBuilder();
                foreach (COperation operationInfo in operationWorker.GetListByHospitalizationId(hospitalization.Id))
                {
                    if (string.IsNullOrEmpty(textStrFirstLine))
                    {
                        textStrFirstLine = string.Format("{0} - {1}", CConvertEngine.DateTimeToString(operationInfo.DateOfOperation), operationInfo.Name);
                    }
                    else
                    {
                        textStr.AppendFormat("\t\t\t{0} - {1}\r\n", CConvertEngine.DateTimeToString(operationInfo.DateOfOperation), operationInfo.Name);
                    }
                }

                if (textStr.Length > 2)
                {
                    textStr.Remove(textStr.Length - 2, 2);
                }

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 0;
                _paragraph.Range.Text = "\tоперация:\t" + textStrFirstLine;
                SetWordsInRangeBold(_paragraph.Range, new[] { 2 });

                if (textStr.Length > 0)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = textStr.ToString();
                }

                // Добавляем информацию о консервативном лечении
                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tконсервативное лечение: " + dischargeEpicrisis.ConservativeTherapy;
                SetWordsInRangeBold(_paragraph.Range, new[] { 2, 3, 4 });

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] afterOperationLines = dischargeEpicrisis.AfterOperation.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "После  операции " + afterOperationLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (diagnoseLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnoseLines[1];
                }

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "ОАК({0}): эритроциты-{1}х1012/л, лейкоциты-{2}х109/л, Hb-{3} г/л, СОЭ-{4} мм/ч;",
                    CConvertEngine.DateTimeToString(dischargeEpicrisis.AnalysisDate),
                    dischargeEpicrisis.OakEritrocits,
                    dischargeEpicrisis.OakLekocits,
                    dischargeEpicrisis.OakHb,
                    dischargeEpicrisis.OakSoe);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(70);

                // Возводим в степень 10 в 12-ой и 10 в 9-ой.
                int charNum = _paragraph.Range.Text.IndexOf("х1012/л", StringComparison.Ordinal);
                _paragraph.Range.Characters[charNum + 4].Font.Superscript =
                _paragraph.Range.Characters[charNum + 5].Font.Superscript = 1;

                charNum = _paragraph.Range.Text.IndexOf("х109/л", StringComparison.Ordinal);
                _paragraph.Range.Characters[charNum + 4].Font.Superscript = 1;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "ОАМ({0}): цвет {1}, относит. плотность {2}, эритроциты {3}, лейкоциты {4}",
                    CConvertEngine.DateTimeToString(dischargeEpicrisis.AnalysisDate),
                    dischargeEpicrisis.OamColor,
                    dischargeEpicrisis.OamDensity,
                    dischargeEpicrisis.OamEritrocits,
                    dischargeEpicrisis.OamLekocits);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (!string.IsNullOrEmpty(dischargeEpicrisis.AdditionalAnalises))
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = dischargeEpicrisis.AdditionalAnalises;
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "ЭКГ: " + dischargeEpicrisis.Ekg;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 1;
                _paragraph.Range.Text = "Рекомендации при выписке:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Bold = 0;

                _waitForm.SetProgress(80);

                var recomendations = new StringBuilder();
                for (int i = 0; i < dischargeEpicrisis.Recomendations.Count; i++)
                {
                    recomendations.Append(dischargeEpicrisis.Recomendations[i] + "\r\n");
                }

                for (int i = 0; i < dischargeEpicrisis.AdditionalRecomendations.Count; i++)
                {
                    recomendations.Append(dischargeEpicrisis.AdditionalRecomendations[i] + "\r\n");
                }

                if (string.IsNullOrEmpty(recomendations.ToString()))
                {
                    _paragraph.Range.Text = "\tНет рекомендаций\r\n";
                }
                else
                {
                    _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                    _paragraph.Range.Text = recomendations.ToString();
                    _paragraph.Range.ListFormat.ApplyNumberDefaultOld();
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + hospitalization.DoctorInChargeOfTheCase;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + globalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word осмотр в отделении
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="hospitalizationInfo">Информация о госпитализации</param>
        /// <param name="medicalInspectionInfo">Информация об осмотре в отделении</param>
        /// <param name="operationWorker">Класс с данными по всем операциям</param> 
        /// <param name="globalSettings">Глобальные настройки</param>        
        public static void ExportMedicalInspection(
            CPatient patientInfo,
            CHospitalization hospitalizationInfo,
            CMedicalInspection medicalInspectionInfo,
            COperationWorker operationWorker,
            CGlobalSettings globalSettings)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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
                _wordDoc.PageSetup.LeftMargin = 50;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "Осмотр зав. отделением и лечащим врачом";

                AddEmptyParagraph();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = CConvertEngine.DateTimeToString(hospitalizationInfo.DeliveryDate, true);

                string[] complaints = medicalInspectionInfo.Complaints.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Жалобы: " + complaints[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _waitForm.SetProgress(20);

                if (complaints.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = complaints[1];
                }

                if (medicalInspectionInfo.IsAnamneseActive)
                {
                    string[] anMorbi = medicalInspectionInfo.AnamneseAnMorbi.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "An. morbi. " + anMorbi[0];
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                    if (anMorbi.Length > 1)
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = anMorbi[1];
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "An. vitae. туберкулез: {0}, желтуха: {1}, вен. заболевания: {2}, острозаразные: {3}",
                        medicalInspectionInfo.AnamneseAnVitae[0] ? "есть" : "нет",
                        medicalInspectionInfo.AnamneseAnVitae[1] ? "есть" : "нет",
                        medicalInspectionInfo.AnamneseAnVitae[2] ? "есть" : "нет",
                        medicalInspectionInfo.AnamneseAnVitae[3] ? "есть" : "нет");
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                    "Операции : {0}\r\n" +
                    "Травмы : {1}\r\n" +
                    "Хрон. заболевания:{2}. Воспаление легких: {3}; варикозная болезнь: {4}. Переливание крови: {5}. Контактов с инфекционными больными в последний месяц {6}.\r\n" +
                    "Аллергические реакции на лекарственные препараты: {7}.",
                    medicalInspectionInfo.AnamneseTextBoxes[2],
                    medicalInspectionInfo.AnamneseTextBoxes[6],
                    medicalInspectionInfo.AnamneseTextBoxes[0],
                    medicalInspectionInfo.AnamneseTextBoxes[1],
                    medicalInspectionInfo.AnamneseTextBoxes[4],
                    medicalInspectionInfo.AnamneseTextBoxes[5],
                    medicalInspectionInfo.AnamneseTextBoxes[3],
                    medicalInspectionInfo.AnamneseTextBoxes[7]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    _wordRange = _paragraph.Range;
                    object defaultTableBehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object autoFitBehavior = WdAutoFitBehavior.wdAutoFitFixed;
                    _wordTable = _wordDoc.Tables.Add(_wordRange, 7, 6, ref defaultTableBehavior, ref autoFitBehavior);

                    _wordTable.Range.Font.Name = "Times New Roman";
                    _wordTable.Range.Font.Size = 10;
                    _wordTable.Range.Font.Bold = 0;
                    _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    _wordTable.Rows.SetLeftIndent((float)8.5, WdRulerStyle.wdAdjustNone);

                    for (int i = 1; i <= _wordTable.Rows.Count; i++)
                    {
                        _wordTable.Rows[i].Cells[1].Width = 215;
                        _wordTable.Rows[i].Cells[2].Width = 20;
                        _wordTable.Rows[i].Cells[3].Width = 25;
                        _wordTable.Rows[i].Cells[4].Width = 185;
                        _wordTable.Rows[i].Cells[5].Width = 20;
                        _wordTable.Rows[i].Cells[6].Width = 25;
                    }

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
                    if (medicalInspectionInfo.AnamneseCheckboxes[0])
                    {
                        _wordTable.Rows[2].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[2].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[2].Cells[4].Range.Text = "7. Хроническое неспецифическое заболевание легких";
                    _wordTable.Rows[2].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[6])
                    {
                        _wordTable.Rows[2].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[2].Cells[6].Range.Text = "x";
                    }

                    _wordTable.Rows[3].Cells[1].Range.Text = "2. Постромботическая болезнь (тромбофилия)";
                    _wordTable.Rows[3].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[1])
                    {
                        _wordTable.Rows[3].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[3].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[3].Cells[4].Range.Text = "8. Ожирение";
                    _wordTable.Rows[3].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[7])
                    {
                        _wordTable.Rows[3].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[3].Cells[6].Range.Text = "x";
                    }

                    _wordTable.Rows[4].Cells[1].Range.Text = "3. Венозный тромбоз и ТЭЛА у биологических родственников (тромбофилия)";
                    _wordTable.Rows[4].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[2])
                    {
                        _wordTable.Rows[4].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[4].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[4].Cells[4].Range.Text = "9. Иммобилизация нижней конечности с пребыванием в постели 3 и более дней";
                    _wordTable.Rows[4].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[8])
                    {
                        _wordTable.Rows[4].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[4].Cells[6].Range.Text = "x";
                    }

                    _wordTable.Rows[5].Cells[1].Range.Text = "4. Прием антикоагулянтов";
                    _wordTable.Rows[5].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[3])
                    {
                        _wordTable.Rows[5].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[5].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[5].Cells[4].Range.Text = "10. Сахарный диабет";
                    _wordTable.Rows[5].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[9])
                    {
                        _wordTable.Rows[5].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[5].Cells[6].Range.Text = "x";
                    }

                    _wordTable.Rows[6].Cells[1].Range.Text = "5. Варикозное расширение вен";
                    _wordTable.Rows[6].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[4])
                    {
                        _wordTable.Rows[6].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[6].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[6].Cells[4].Range.Text = "11. Прием эстрогенов";
                    _wordTable.Rows[6].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[10])
                    {
                        _wordTable.Rows[6].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[6].Cells[6].Range.Text = "x";
                    }

                    _wordTable.Rows[7].Cells[1].Range.Text = "6. Инфаркт миокарда";
                    _wordTable.Rows[7].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[5])
                    {
                        _wordTable.Rows[7].Cells[2].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[7].Cells[3].Range.Text = "x";
                    }

                    _wordTable.Rows[7].Cells[4].Range.Text = "12. Онкозаболевание";
                    _wordTable.Rows[7].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    if (medicalInspectionInfo.AnamneseCheckboxes[11])
                    {
                        _wordTable.Rows[7].Cells[5].Range.Text = "x";
                    }
                    else
                    {
                        _wordTable.Rows[7].Cells[6].Range.Text = "x";
                    }
                }

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "St. praesens. Общее состояние: {0}. " +
                    "Сознание: {1}, положение: {2}. Питание: {3}. " +
                    "Кожный покров и видимые слизистые (вне зоны повреждения): {4}. " +
                    "Щитовидная железа: {5}. Лимфатические узлы: {6}.",
                    medicalInspectionInfo.StPraesensComboBoxes[0],
                    medicalInspectionInfo.StPraesensTextBoxes[0],
                    medicalInspectionInfo.StPraesensTextBoxes[1],
                    medicalInspectionInfo.StPraesensComboBoxes[1],
                    medicalInspectionInfo.StPraesensTextBoxes[2],
                    medicalInspectionInfo.StPraesensTextBoxes[3],
                    medicalInspectionInfo.StPraesensTextBoxes[4]);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "ЧДД {0} в мин. В легких дыхание {1}, {2}, хрипы: {3}. " +
                    "ЧСС {4} в мин. Тоны сердца {5}; ритм {6}. PS {7}.",
                    medicalInspectionInfo.StPraesensNumericUpDowns[0],
                    medicalInspectionInfo.StPraesensTextBoxes[5],
                    medicalInspectionInfo.StPraesensComboBoxes[2],
                    medicalInspectionInfo.StPraesensTextBoxes[6],
                    medicalInspectionInfo.StPraesensNumericUpDowns[1],
                    medicalInspectionInfo.StPraesensComboBoxes[3],
                    medicalInspectionInfo.StPraesensTextBoxes[7],
                    medicalInspectionInfo.StPraesensTextBoxes[8]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "АД {0}/{1} мм.рт.ст. Живот {2}. Печеночная тупость {3}, " +
                    "притупления в отлогих местах {4}, перистальтика {5}. " +
                    "Per rectum: {6}. Физиологические отправления: {7}. " +
                    "Нагрузка на кости таза, позвоночник {8}. Активные движения в " +
                    "неповрежденных конечностях {9}.",
                    medicalInspectionInfo.StPraesensNumericUpDowns[2],
                    medicalInspectionInfo.StPraesensNumericUpDowns[3],
                    medicalInspectionInfo.StPraesensTextBoxes[9],
                    medicalInspectionInfo.StPraesensTextBoxes[10],
                    medicalInspectionInfo.StPraesensTextBoxes[11],
                    medicalInspectionInfo.StPraesensTextBoxes[12],
                    medicalInspectionInfo.StPraesensTextBoxes[13],
                    medicalInspectionInfo.StPraesensTextBoxes[14],
                    medicalInspectionInfo.StPraesensTextBoxes[15],
                    medicalInspectionInfo.StPraesensTextBoxes[16]);

                AddEmptyParagraph();

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "St. localis:";
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });

                if (medicalInspectionInfo.IsStLocalisPart1Enabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Объем движений в суставах верхней конечности";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Size = 10;
                    _paragraph.Range.Text = string.Format(
                        "Плечевой пояс:\tэлевация / депрессия (F: 20-0-10): акт - {0}, пасс – {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[0],
                        medicalInspectionInfo.StLocalisPart1Fields[1]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tсгибание/разгибание (Т: 20-0-20): акт - {0}, пасс – {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[2],
                        medicalInspectionInfo.StLocalisPart1Fields[3]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Плечевой сустав:  разгибание/сгибание (S: 50-0-180): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[4],
                        medicalInspectionInfo.StLocalisPart1Fields[5]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   отведение/приведение (F: 180-0-0): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[6],
                        medicalInspectionInfo.StLocalisPart1Fields[7]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   горизонтальное разгибание и сгибание (Т: 30-0-135): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[8],
                        medicalInspectionInfo.StLocalisPart1Fields[9]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   нар. и вн. ротация при отведенном на 90° плече (R: 90-0-90): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[10],
                        medicalInspectionInfo.StLocalisPart1Fields[11]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t   нар. и вн. ротация при приведенном плече (R: 65-0-70): акт - {0}, пасс - {1}.",
                        medicalInspectionInfo.StLocalisPart1Fields[12],
                        medicalInspectionInfo.StLocalisPart1Fields[13]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Локтевой сустав: разгибание и сгибание (S: 0-0-150): акт - {0}, пасс - {1}.",
                        medicalInspectionInfo.StLocalisPart1Fields[14],
                        medicalInspectionInfo.StLocalisPart1Fields[15]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Луче-локтевые суставы: супинация и пронация (R: 90-0-90): акт - {0}, пасс - {1}.",
                        medicalInspectionInfo.StLocalisPart1Fields[16],
                        medicalInspectionInfo.StLocalisPart1Fields[17]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Лучезапястный сустав:\tразгибание и сгибание (S: 70-0-80): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[18],
                        medicalInspectionInfo.StLocalisPart1Fields[19]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\tотведение и приведение (F: 25-0-55): акт - {0}, пасс - {1}.",
                        medicalInspectionInfo.StLocalisPart1Fields[20],
                        medicalInspectionInfo.StLocalisPart1Fields[21]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Суставы 1-го пальца:";
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tЗПС:\tлучевое отведение и приведение (F: 35-0-15): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[22],
                        medicalInspectionInfo.StLocalisPart1Fields[23]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tладонное отведение и приведение (S: 40-0-0): акт - {0}, пасс - {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[24],
                        medicalInspectionInfo.StLocalisPart1Fields[25]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПФС.\tразгибание и сгибание (S: 5-0-50): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[26],
                        medicalInspectionInfo.StLocalisPart1Fields[27]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tМФС.\tразгибание и сгибание (S: 15-0-85): акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[28],
                        medicalInspectionInfo.StLocalisPart1Fields[29]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                    _waitForm.SetProgress(50);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\tоппозиция: {0} палец",
                        medicalInspectionInfo.StLocalisPart1OppositionFinger);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Суставы II-V-го пальцев:";
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6, 7, 8 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПФС: разгибание и сгибание (S: 35-0-90): IIп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[30],
                        medicalInspectionInfo.StLocalisPart1Fields[31]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  IIIп. - акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[32],
                        medicalInspectionInfo.StLocalisPart1Fields[33]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  IVп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[34],
                        medicalInspectionInfo.StLocalisPart1Fields[35]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t  V - акт - {0}, пасс - {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[36],
                        medicalInspectionInfo.StLocalisPart1Fields[37]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tотведение и приведение (F: 30-0-25): IIп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[38],
                        medicalInspectionInfo.StLocalisPart1Fields[39]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       IIIп. - акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[40],
                        medicalInspectionInfo.StLocalisPart1Fields[41]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       IVп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[42],
                        medicalInspectionInfo.StLocalisPart1Fields[43]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t       V - акт - {0}, пасс - {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[44],
                        medicalInspectionInfo.StLocalisPart1Fields[45]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tПМФС: разгибание и сгибание (S: 0-0-100): IIп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[46],
                        medicalInspectionInfo.StLocalisPart1Fields[47]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     IIIп. - акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[48],
                        medicalInspectionInfo.StLocalisPart1Fields[49]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     IVп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[50],
                        medicalInspectionInfo.StLocalisPart1Fields[51]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t     V - акт - {0}, пасс - {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[52],
                        medicalInspectionInfo.StLocalisPart1Fields[53]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\tДМФС: разгибание и сгибание (S: 0-0-80): IIп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[54],
                        medicalInspectionInfo.StLocalisPart1Fields[55]);
                    SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   IIIп. - акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[56],
                        medicalInspectionInfo.StLocalisPart1Fields[57]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   IVп. акт - {0}, пасс - {1};",
                        medicalInspectionInfo.StLocalisPart1Fields[58],
                        medicalInspectionInfo.StLocalisPart1Fields[59]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "\t\t\t\t\t\t   V - акт - {0}, пасс - {1}",
                        medicalInspectionInfo.StLocalisPart1Fields[60],
                        medicalInspectionInfo.StLocalisPart1Fields[61]);

                    AddEmptyParagraph();
                }

                _waitForm.SetProgress(60);

                if (medicalInspectionInfo.IsStLocalisPart2Enabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Size = 12;
                    string axisViolation;
                    if (medicalInspectionInfo.StLocalisPart2ComboBoxes[1] == "нет")
                    {
                        axisViolation = "нет";
                    }
                    else
                    {
                        string rigthComboBox1Value = medicalInspectionInfo.StLocalisPart2ComboBoxes[1] == "пальцев"
                                                         ? "пальца"
                                                         : "пястной кости";

                        axisViolation = string.Format(
                            "{0} {1} {2}",
                            medicalInspectionInfo.StLocalisPart2ComboBoxes[2],
                            rigthComboBox1Value,
                            medicalInspectionInfo.StLocalisPart2ComboBoxes[3]);
                    }

                    _paragraph.Range.Text = string.Format(
                        "Кисть: {0}. Кожа {1}. Нарушение оси {2}.",
                        medicalInspectionInfo.StLocalisPart2WhichHand,
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[0],
                        axisViolation);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Дефигурация: {0}, деформация: {1}, боль при нагрузке по оси: {2}, боль при пальпации: {3}.",
                        medicalInspectionInfo.StLocalisPart2TextBoxes[0],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[1],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[2],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[3]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Патологическая подвижность: {0}. Пружинистая подвижность: {1}.",
                        medicalInspectionInfo.StLocalisPart2TextBoxes[4],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[5]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Раны: {0}.",
                        medicalInspectionInfo.StLocalisPart2TextBoxes[6]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Форма раны: {0}. Края раны: {1}.",
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[4],
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[5]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Размеры ран(ы) {0}; {1}.",
                        medicalInspectionInfo.StLocalisPart2TextBoxes[7],
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[6]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Кровотечение: {0}. Некрозы: {1}.",
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[7],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[8]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Пульс на лучевой артерии: {0}.",
                        medicalInspectionInfo.StLocalisPart2TextBoxes[9]);

                    _waitForm.SetProgress(70);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _paragraph.Range.Text = "Дистальнее повреждения:";

                    if (medicalInspectionInfo.StLocalisPart2WhichHand == "правая, левая" ||
                       medicalInspectionInfo.StLocalisPart2WhichHand == "левая")
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        _paragraph.Range.Text = "Левая кисть";

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Активное сгибание ДМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2LeftHand[0],
                            medicalInspectionInfo.StLocalisPart2LeftHand[1],
                            medicalInspectionInfo.StLocalisPart2LeftHand[2],
                            medicalInspectionInfo.StLocalisPart2LeftHand[3],
                            medicalInspectionInfo.StLocalisPart2LeftHand[4]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "\t\tПМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2LeftHand[5],
                            medicalInspectionInfo.StLocalisPart2LeftHand[6],
                            medicalInspectionInfo.StLocalisPart2LeftHand[7],
                            medicalInspectionInfo.StLocalisPart2LeftHand[8],
                            medicalInspectionInfo.StLocalisPart2LeftHand[9]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Активное разгибание ДМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2LeftHand[10],
                            medicalInspectionInfo.StLocalisPart2LeftHand[11],
                            medicalInspectionInfo.StLocalisPart2LeftHand[12],
                            medicalInspectionInfo.StLocalisPart2LeftHand[13],
                            medicalInspectionInfo.StLocalisPart2LeftHand[14]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "\t\tПМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2LeftHand[15],
                            medicalInspectionInfo.StLocalisPart2LeftHand[16],
                            medicalInspectionInfo.StLocalisPart2LeftHand[17],
                            medicalInspectionInfo.StLocalisPart2LeftHand[18],
                            medicalInspectionInfo.StLocalisPart2LeftHand[19]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Приведение, отведение I пальца: {0}. Сведение/разведение пальцев: {1}.",
                            medicalInspectionInfo.StLocalisPart2LeftHand[20],
                            medicalInspectionInfo.StLocalisPart2LeftHand[21]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Цвет кожи: {0}; кожа: {1}.",
                            medicalInspectionInfo.StLocalisPart2LeftHand[22],
                            medicalInspectionInfo.StLocalisPart2LeftHand[23]);
                    }

                    _waitForm.SetProgress(80);

                    if (medicalInspectionInfo.StLocalisPart2WhichHand == "правая, левая" ||
                       medicalInspectionInfo.StLocalisPart2WhichHand == "правая")
                    {
                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        _paragraph.Range.Text = "Правая кисть";

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Активное сгибание  ДМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2RightHand[0],
                            medicalInspectionInfo.StLocalisPart2RightHand[1],
                            medicalInspectionInfo.StLocalisPart2RightHand[2],
                            medicalInspectionInfo.StLocalisPart2RightHand[3],
                            medicalInspectionInfo.StLocalisPart2RightHand[4]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "\t\t\tПМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2RightHand[5],
                            medicalInspectionInfo.StLocalisPart2RightHand[6],
                            medicalInspectionInfo.StLocalisPart2RightHand[7],
                            medicalInspectionInfo.StLocalisPart2RightHand[8],
                            medicalInspectionInfo.StLocalisPart2RightHand[9]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Активное разгибание ДМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2RightHand[10],
                            medicalInspectionInfo.StLocalisPart2RightHand[11],
                            medicalInspectionInfo.StLocalisPart2RightHand[12],
                            medicalInspectionInfo.StLocalisPart2RightHand[13],
                            medicalInspectionInfo.StLocalisPart2RightHand[14]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "\t\t\t  ПМФС: I – {0}, II – {1}, III – {2}, IV – {3}, V – {4};",
                            medicalInspectionInfo.StLocalisPart2RightHand[15],
                            medicalInspectionInfo.StLocalisPart2RightHand[16],
                            medicalInspectionInfo.StLocalisPart2RightHand[17],
                            medicalInspectionInfo.StLocalisPart2RightHand[18],
                            medicalInspectionInfo.StLocalisPart2RightHand[19]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Приведение, отведение I пальца: {0}. Сведение/разведение пальцев: {1}.",
                            medicalInspectionInfo.StLocalisPart2RightHand[20],
                            medicalInspectionInfo.StLocalisPart2RightHand[21]);

                        _wordDoc.Paragraphs.Add(ref _missingObject);
                        _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                        _paragraph.Range.Text = string.Format(
                            "Цвет кожи: {0}; кожа: {1}.",
                            medicalInspectionInfo.StLocalisPart2RightHand[22],
                            medicalInspectionInfo.StLocalisPart2RightHand[23]);
                    }

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Иннервация: {0}.",
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[8]);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Пузыри на коже: {0}. Бледное пятно при надавливании на {1} исчезает через {2} сек.",
                        medicalInspectionInfo.StLocalisPart2ComboBoxes[9],
                        medicalInspectionInfo.StLocalisPart2TextBoxes[10],
                        medicalInspectionInfo.StLocalisPart2NumericUpDown);

                    AddEmptyParagraph();
                }

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 12;
                _paragraph.Range.Text = medicalInspectionInfo.StLocalisDescription;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Рентгенограммы в двух проекциях: " + medicalInspectionInfo.StLocalisRentgen + ".";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Риск ТЭО: " + medicalInspectionInfo.TeoRisk + ".";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string expertAnamnes;
                if (medicalInspectionInfo.ExpertAnamnese == 1)
                {
                    expertAnamnes = string.Format(
                        "л/н выдан амбулаторно с {0} по {1}, всего дней нетрудоспособности {2}",
                        CConvertEngine.DateTimeToString(medicalInspectionInfo.LnWithNumberDateStart),
                        CConvertEngine.DateTimeToString(medicalInspectionInfo.LnWithNumberDateEnd),
                        CCompareEngine.GetDiffInDays(medicalInspectionInfo.LnWithNumberDateEnd, medicalInspectionInfo.LnWithNumberDateStart) + 1);
                }
                else if (medicalInspectionInfo.ExpertAnamnese == 2)
                {
                    expertAnamnes = string.Format(
                        "л/н открыт первично с {0}",
                        CConvertEngine.DateTimeToString(medicalInspectionInfo.LnFirstDateStart));
                }
                else
                {
                    expertAnamnes = "л/н не требуется.";
                }

                _paragraph.Range.Text = "Экспертный анамнез: " + expertAnamnes;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] diagnose = hospitalizationInfo.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Клинический диагноз: " + diagnose[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                if (diagnose.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnose[1];
                }

                for (int i = 0; i < 3; i++)
                {
                    AddEmptyParagraph();
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Врач:______________{0}                  З/о:_______________{1}",
                    hospitalizationInfo.DoctorInChargeOfTheCase,
                    globalSettings.BranchManager);

                _waitForm.SetProgress(95);

                if (medicalInspectionInfo.IsPlanEnabled)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        AddEmptyParagraph();
                    }

                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 1;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _paragraph.Range.Text = "ПЛАН ОБСЛЕДОВАНИЯ И ЛЕЧЕНИЯ\r\n";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                    _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                    _paragraph.Range.Text = "Обследование: " + medicalInspectionInfo.InspectionPlan + ".\r\n" +
                        "Оперативное лечение.\r\n" +
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

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Empty;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(hospitalizationInfo.DeliveryDate);
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Лечащий врач " + hospitalizationInfo.DoctorInChargeOfTheCase;
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });
                    AddEmptyParagraph();
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Добавить данные по операции. В отдельной функции, потому что печатается 2 раза
        /// </summary>
        /// <param name="operationInfo">Информация об операции</param>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="hospitalizationInfo">Информация о госпитализации</param>
        /// <param name="operationProtocolInfo">Информация о протоколе операции</param>
        private static void InsertTableForOperation(
            COperation operationInfo,
            CPatient patientInfo,
            CHospitalization hospitalizationInfo,
            COperationProtocol operationProtocolInfo)
        {
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 1;
            _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            _paragraph.Range.Text = "ОПЕРАЦИЯ";

            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Font.Bold = 0;
            _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            _wordRange = _paragraph.Range;
            _wordTable = _wordDoc.Tables.Add(_wordRange, 11, 3, ref _missingObject, ref _missingObject);

            _wordTable.Range.Font.Name = "Times New Roman";
            _wordTable.Range.Font.Size = 12;
            _wordTable.Range.Font.Bold = 0;
            _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            for (int i = 1; i <= _wordTable.Rows.Count; i++)
            {
                _wordTable.Rows[i].Cells[1].Width = 250;
                _wordTable.Rows[i].Cells[2].Width = 100;
                _wordTable.Rows[i].Cells[3].Width = 130;
            }

            int startRowForMerge = operationInfo.Surgeons.Count + operationInfo.Assistents.Count + 5;

            for (int i = startRowForMerge; i <= 11; i++)
            {
                object begCell = _wordTable.Cell(i, 1).Range.Start;
                object endCell = _wordTable.Cell(i, 3).Range.End;
                _wordRange = _wordDoc.Range(ref begCell, ref endCell);
                _wordRange.Select();
                _wordApp.Selection.Cells.Merge();
            }

            _wordTable.Rows[1].Cells[1].Range.Text = "Фамилия " + patientInfo.LastName;
            SetWordsInRangeBold(_wordTable.Rows[1].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[2].Cells[1].Range.Text = "Имя " + patientInfo.Name;
            SetWordsInRangeBold(_wordTable.Rows[2].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[3].Cells[1].Range.Text = "Отчество " + patientInfo.Patronymic;
            SetWordsInRangeBold(_wordTable.Rows[3].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[4].Cells[1].Range.Text = "Возраст " + CConvertEngine.GetAge(patientInfo.Birthday);
            SetWordsInRangeBold(_wordTable.Rows[4].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[5].Cells[1].Range.Text = "№ ист. б-ни " + hospitalizationInfo.NumberOfCaseHistory;
            SetWordsInRangeBold(_wordTable.Rows[5].Cells[1].Range, new[] { 1, 2, 3, 4, 5, 6 });

            _wordTable.Rows[6].Cells[1].Range.Text = "Дата поступления " + CConvertEngine.DateTimeToString(hospitalizationInfo.DeliveryDate);
            SetWordsInRangeBold(_wordTable.Rows[6].Cells[1].Range, new[] { 1, 2 });

            string timeOfOperation = operationInfo.EndTimeOfOperation.HasValue
                    ? CConvertEngine.TimeToString(operationInfo.EndTimeOfOperation.Value)
                    : "НЕ УКАЗАНА";

            _wordTable.Rows[7].Cells[1].Range.Text = string.Format(
                "Дата операции {0} {1}-{2}",
                CConvertEngine.DateTimeToString(operationInfo.DateOfOperation),
                CConvertEngine.TimeToString(operationInfo.StartTimeOfOperation),
                timeOfOperation);
            SetWordsInRangeBold(_wordTable.Rows[7].Cells[1].Range, new[] { 1, 2 });

            _wordTable.Rows[8].Cells[1].Range.Text = "Адрес " + patientInfo.GetAddress();
            SetWordsInRangeBold(_wordTable.Rows[8].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[9].Cells[1].Range.Text = "Диагноз " + hospitalizationInfo.Diagnose;
            SetWordsInRangeBold(_wordTable.Rows[9].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[10].Cells[1].Range.Text = "Операция " + operationInfo.Name;
            SetWordsInRangeBold(_wordTable.Rows[10].Cells[1].Range, new[] { 1 });

            _wordTable.Rows[11].Cells[1].Range.Text = "Ход операции: " + operationProtocolInfo.OperationCourse;
            SetWordsInRangeBold(_wordTable.Rows[11].Cells[1].Range, new[] { 1, 2 });

            int rowNum = 1;
            _wordTable.Rows[rowNum].Cells[2].Range.Text = "Хирург";
            _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

            foreach (string surgeoun in operationInfo.Surgeons)
            {
                _wordTable.Rows[rowNum].Cells[3].Range.Text = surgeoun;
                rowNum++;
            }

            if (operationInfo.Assistents.Count > 0)
            {
                _wordTable.Rows[rowNum].Cells[2].Range.Text = "Ассистент";
                _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

                foreach (string assistent in operationInfo.Assistents)
                {
                    _wordTable.Rows[rowNum].Cells[3].Range.Text = assistent;
                    rowNum++;
                }
            }

            if (!string.IsNullOrEmpty(operationInfo.HeAnaesthetist))
            {
                _wordTable.Rows[rowNum].Cells[2].Range.Text = "Анестезиолог";
                _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

                _wordTable.Rows[rowNum].Cells[3].Range.Text = operationInfo.HeAnaesthetist;
                rowNum++;
            }

            _wordTable.Rows[rowNum].Cells[2].Range.Text = "Опер. м/сестра";
            _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

            _wordTable.Rows[rowNum].Cells[3].Range.Text = operationInfo.ScrubNurse;
            rowNum++;

            if (!string.IsNullOrEmpty(operationInfo.SheAnaesthetist))
            {
                _wordTable.Rows[rowNum].Cells[2].Range.Text = "Анестезистка";
                _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

                _wordTable.Rows[rowNum].Cells[3].Range.Text = operationInfo.SheAnaesthetist;
                rowNum++;
            }

            _wordTable.Rows[rowNum].Cells[2].Range.Text = "Санитар";
            _wordTable.Rows[rowNum].Cells[2].Range.Font.Bold = 1;

            _wordTable.Rows[rowNum].Cells[3].Range.Text = operationInfo.Orderly;

            string surgeon = operationInfo.Surgeons.Count > 0 ? operationInfo.Surgeons[0] : "                          ";
            _wordDoc.Paragraphs.Add(ref _missingObject);
            _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
            _paragraph.Range.Text = string.Format("Дата {0}, {1}\t\t\t\t{2} _________________",
                operationInfo.DateOfOperation.ToString("dd.MM.yyyy"),
                operationInfo.EndTimeOfOperation == null ? DateTime.Now.ToString("HH:mm") : operationInfo.EndTimeOfOperation.Value.AddMinutes(15).ToString("HH:mm"),
                surgeon);
            SetWordsInRangeBold(_paragraph.Range, new[] { 1 });
        }


        /// <summary>
        /// Экспортировать в Word протокол операции
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="operationInfo">Информация об операции</param>        
        /// <param name="operationProtocolInfo">Информация об операционном протоколе</param>
        /// <param name="hospitalizationInfo">Информация о госпитализации</param>
        /// <param name="globalSettings">Глобальные настройки</param>
        public static void ExportOperationProtocol(
            CPatient patientInfo,
            COperation operationInfo,
            COperationProtocol operationProtocolInfo,
            CHospitalization hospitalizationInfo,
            CGlobalSettings globalSettings)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                // Создание плана обследования и лечения, если надо
                if (operationProtocolInfo.IsTreatmentPlanActiveInOperationProtocol)
                {
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 1;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    _paragraph.Range.Text = "ПЛАН ОБСЛЕДОВАНИЯ И ЛЕЧЕНИЯ\r\n";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                    _paragraph.Range.ListFormat.ApplyNumberDefault(ref _missingObject);
                    _paragraph.Range.Text = "Обследование: " + operationProtocolInfo.TreatmentPlanInspection + ".\r\n" +
                        "Оперативное лечение.\r\n" +
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

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Empty;
                    _paragraph.Range.ParagraphFormat.FirstLineIndent = 0;
                    _paragraph.Range.ParagraphFormat.LeftIndent = 0;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(operationProtocolInfo.TreatmentPlanDate);
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Лечащий врач " + hospitalizationInfo.DoctorInChargeOfTheCase;
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });
                    AddEmptyParagraph();
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        AddEmptyParagraph();
                    }
                }

                _waitForm.SetProgress(20);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "ПРЕДОПЕРАЦИОННЫЙ ЭПИКРИЗ";

                // Создание текст бокса с печатью
                _wordShape = _wordDoc.Shapes.AddTextbox(
                    MsoTextOrientation.msoTextOrientationHorizontal,
                    320,
                    operationProtocolInfo.IsTreatmentPlanActiveInOperationProtocol ? 80 : 50,
                    238,
                    124,
                    ref _missingObject);

                _wordShape.TextFrame.TextRange.Font.Name = "Times New Roman";
                _wordShape.TextFrame.TextRange.Font.Size = 10;
                _wordShape.TextFrame.TextRange.Font.Bold = 1;
                _wordShape.TextFrame.TextRange.Font.Spacing = -1;
                _wordShape.Line.Weight = 2;

                _wordShape.TextFrame.MarginLeft = (float)1.42;
                _wordShape.TextFrame.MarginRight = (float)1.42;
                _wordShape.TextFrame.MarginTop = (float)0.85;
                _wordShape.TextFrame.MarginBottom = (float)0.85;

                _wordShape.WrapFormat.Side = WdWrapSideType.wdWrapLeft;
                _wordShape.WrapFormat.Type = WdWrapType.wdWrapTight;
                _wordShape.WrapFormat.DistanceLeft = 3;
                _wordShape.WrapFormat.DistanceTop = 3;

                _wordShape.TextFrame.TextRange.Text =
                    "Назначение пяти и более лекарственных средств, назначение десяти и более лекарственных средств в течение месяца, является жизненно необходимым (необходимо для лечения) и  соответствует стандартам качества\r\n\r\n" +
                    "Заведующий отделением\r\n" +
                    "(ответственный дежурный врач)\r\n\r\n" +
                    "Лечащий врач (дежурный врач)";

                _wordShape.TextFrame.TextRange.Paragraphs[1].Alignment = WdParagraphAlignment.wdAlignParagraphJustify;

                _waitForm.SetProgress(30);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Осмотр зав. отделением";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Пациент " + patientInfo.GetFullName() + ", " + CConvertEngine.GetAge(patientInfo.Birthday) + " лет";
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                if (operationProtocolInfo.IsDairyEnabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Температура тела - " + operationProtocolInfo.Temperature +
                        ". Жалобы: " + operationProtocolInfo.Complaints;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Состояние " + operationProtocolInfo.State +
                        ". Пульс " + operationProtocolInfo.Pulse +
                        " в мин., АД " + operationProtocolInfo.ADFirst + "/" +
                        operationProtocolInfo.ADSecond + " мм.рт.ст., ЧДД " +
                        operationProtocolInfo.ChDD + " в мин.";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "В легких дыхание " + operationProtocolInfo.Breath +
                        ", хрипы - " + operationProtocolInfo.Wheeze;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Тоны сердца " + operationProtocolInfo.HeartSounds +
                        ", ритм " + operationProtocolInfo.HeartRhythm +
                        ". Живот " + operationProtocolInfo.Stomach;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Мочеиспускание " + operationProtocolInfo.Urination +
                        ". Стул " + operationProtocolInfo.Stool;

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    string[] stLocalisLines = operationProtocolInfo.StLocalis.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
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
                string[] diagnoseLines = hospitalizationInfo.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Диагноз: " + diagnoseLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (diagnoseLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnoseLines[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Показано оперативное лечение. Планируется операция: " + operationInfo.Name +
                    ".\r\nПациент согласен на операцию. Противопоказаний нет.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(operationInfo.DateOfOperation) +
                    ", " + CConvertEngine.TimeToString(operationProtocolInfo.TimeWriting);
                SetWordsInRangeBold(_paragraph.Range, new[] { 1 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Зав. отделением\t\t\t\t\t\t\t" + globalSettings.BranchManager;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Лечащий врач\t\t\t\t\t\t\t" + hospitalizationInfo.DoctorInChargeOfTheCase;
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                AddEmptyParagraph();

                _waitForm.SetProgress(50);

                InsertTableForOperation(operationInfo, patientInfo, hospitalizationInfo, operationProtocolInfo);

                _waitForm.SetProgress(75);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                object typeBreak = WdBreakType.wdPageBreak;
                _paragraph.Range.InsertBreak(ref typeBreak);

                InsertTableForOperation(operationInfo, patientInfo, hospitalizationInfo, operationProtocolInfo);

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word акушерский анамнез
        /// </summary>
        /// <param name="obstetricHistory">Информация об акушерском анамнезе</param>
        public static void ExportObstetricHistory(CObstetricHistory obstetricHistory)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "Акушерский анамнез\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                _paragraph.Range.Text = string.Format(
                    "Роды в срок {0} (нед.)  Предлежание: {1}",
                    obstetricHistory.ChildbirthWeeks,
                    obstetricHistory.Fetal);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Осложнения в период беременности (токсикоз, диабет, инфекция и т.д.) {0}",
                    obstetricHistory.ComplicationsInPregnancy);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Лекарственные препараты и хронические интоксикации в период беременности {0}",
                    obstetricHistory.DrugsInPregnancy);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Длительность родов {0} час. Родовая травма: {1}",
                    obstetricHistory.DurationOfLabor,
                    obstetricHistory.BirthInjury);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Осложнения в ходе родов {0}",
                    obstetricHistory.ComplicationsDuringChildbirth);

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Использование в ходе родов: щипцов: {0}, вакуума: {1}. Шкала Апгар: {2} баллов.  ",
                    obstetricHistory.IsTongsUsing ? "да" : "нет",
                    obstetricHistory.IsVacuumUsing ? "да" : "нет",
                    obstetricHistory.ApgarScore);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Вес при рождении {0} г,   рост при рождении {1} см.",
                    obstetricHistory.WeightAtBirth,
                    obstetricHistory.HeightAtBirth);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Когда и кем диагностирован акушерский паралич {0}",
                    obstetricHistory.ObstetricParalysis);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Стационарное лечение (даты госпитализаций) и проводимое лечение (включая операции):\r\n{0}",
                    obstetricHistory.HospitalTreatment);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Амбулаторное лечение (разработка объема пассивных движений, лечебная физкультура, шинирование и т.д.):\r\n{0}\r\n",
                    obstetricHistory.OutpatientCare);

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "Хронология восстановления активных движений верхней конечности";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordRange = _paragraph.Range;
                object defaultTableBehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
                object autoFitBehavior = WdAutoFitBehavior.wdAutoFitFixed;
                _wordTable = _wordDoc.Tables.Add(_wordRange, 8, 7, ref defaultTableBehavior, ref autoFitBehavior);

                _wordTable.Range.Font.Name = "Times New Roman";
                _wordTable.Range.Font.Size = 12;
                _wordTable.Range.Font.Bold = 0;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 1; i <= _wordTable.Rows.Count; i++)
                {
                    _wordTable.Rows[i].Cells[1].Width = 50;
                    _wordTable.Rows[i].Cells[2].Width = 50;
                    _wordTable.Rows[i].Cells[3].Width = 50;
                    _wordTable.Rows[i].Cells[4].Width = 50;
                    _wordTable.Rows[i].Cells[5].Width = 50;
                    _wordTable.Rows[i].Cells[6].Width = 50;
                    _wordTable.Rows[i].Cells[7].Width = 50;
                }

                _waitForm.SetProgress(60);

                _wordTable.Rows[1].Cells[1].Range.Text = "Месяц жизни";
                _wordTable.Rows[1].Cells[2].Range.Text = "Отведение плеча";
                _wordTable.Rows[1].Cells[3].Range.Text = "Наружная ротация плеча";
                _wordTable.Rows[1].Cells[4].Range.Text = "Сгибание в локте";
                _wordTable.Rows[1].Cells[5].Range.Text = "Разгибание в локте";
                _wordTable.Rows[1].Cells[6].Range.Text = "Разгибание кисти и пальцев";
                _wordTable.Rows[1].Cells[7].Range.Text = "Сгибание кисти и пальцев";

                _wordTable.Rows[2].Cells[1].Range.Text = "1";
                _wordTable.Rows[3].Cells[1].Range.Text = "2";
                _wordTable.Rows[4].Cells[1].Range.Text = "3";
                _wordTable.Rows[5].Cells[1].Range.Text = "4";
                _wordTable.Rows[6].Cells[1].Range.Text = "6";
                _wordTable.Rows[7].Cells[1].Range.Text = "10";
                _wordTable.Rows[8].Cells[1].Range.Text = "12";

                string saveClipboardText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    saveClipboardText = Clipboard.GetText();
                }

                Clipboard.SetImage(Properties.Resources.mark);

                _waitForm.SetProgress(70);

                for (int i = 2; i < 9; i++)
                {
                    for (int j = 2; j < 8; j++)
                    {
                        int index = ((i - 2) * 6) + j - 2;

                        if (obstetricHistory.Chronology[index])
                        {
                            _wordTable.Rows[i].Cells[j].Range.Paste();
                        }
                    }
                }

                if (string.IsNullOrEmpty(saveClipboardText))
                {
                    Clipboard.Clear();
                }
                else
                {
                    Clipboard.SetText(saveClipboardText);
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }

        private static string GeneratePersonalFolder()
        {
            string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(personalFolder, "Temp");
        }

        public static void RemoveOldMifrmFiles()
        {
            string personalFolder = GeneratePersonalFolder();

            if (!Directory.Exists(personalFolder))
            {
                Directory.CreateDirectory(personalFolder);
            }

            foreach (string fileNameForDelete in Directory.GetFiles(personalFolder, "MifrmDoc*.rtf"))
            {
                try
                {
                    File.Delete(fileNameForDelete);
                }
                catch { }
            }
        }

        public static string CreateMifrmFile()
        {
            RemoveOldMifrmFiles();
            string personalFolder = GeneratePersonalFolder();

            string fileName;
            int i = 0;
            do
            {
                i++;
                fileName = Path.Combine(personalFolder, "MifrmDoc" + i + ".rtf");
            }
            while (File.Exists(fileName));

            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write(Properties.Resources.MifrmDoc);
            }

            return fileName;
        }

        /// <summary>
        /// Создать медицинскую карту амбулаторного больного
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        public static void CreateMifrmDocument(CPatient patientInfo)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                object fileName = CreateMifrmFile();

                _wordApp = new Application();
                _wordDoc = _wordApp.Documents.Add(ref fileName, ref _missingObject, ref _missingObject, ref _missingObject);

                _waitForm.SetProgress(30);

                FindAndReplace("{cardN}", patientInfo.Id.ToString(CultureInfo.InvariantCulture));
                FindAndReplace("{medOrganization}", patientInfo.InsuranceName);
                _waitForm.SetProgress(35);

                FindAndReplace("{family}", patientInfo.LastName);
                FindAndReplace("{name}", patientInfo.Name);
                _waitForm.SetProgress(40);

                FindAndReplace("{patronymic}", patientInfo.Patronymic);
                FindAndReplace("{birthDate}", patientInfo.Birthday.ToString("dd.MM.yyyy"));
                _waitForm.SetProgress(45);

                FindAndReplace("{punkt}", patientInfo.CityName);
                FindAndReplace("{ulitsa}", patientInfo.StreetName);
                _waitForm.SetProgress(50);

                FindAndReplace("{dom}", patientInfo.HomeNumber);
                FindAndReplace("{corp}", patientInfo.BuildingNumber);
                _waitForm.SetProgress(55);

                FindAndReplace("{kv}", patientInfo.FlatNumber);
                FindAndReplace("{phone}", patientInfo.Phone);
                _waitForm.SetProgress(60);

                var insuranceNumber = new StringBuilder(patientInfo.InsuranceNumber);
                for (int i = insuranceNumber.Length; i < 24; i++)
                {
                    insuranceNumber.Append(" ");
                }

                const double shag = 40.0 / 24;
                double cur = 60;
                for (int i = 0; i < 24; i++)
                {
                    FindAndReplace("[" + i + "]", insuranceNumber[i].ToString(CultureInfo.InvariantCulture));
                    _waitForm.SetProgress(cur);
                    cur += shag;
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word картинки
        /// </summary>
        /// <param name="caption">Название документа, если есть</param>
        /// <param name="palette">Палитра, если есть</param>
        /// <param name="leftPicture">Левая картинка, если есть</param>
        /// <param name="rightPicture">Правая картинка, если есть</param>
        public static void ExportPicture(
            string caption, Bitmap palette, Bitmap leftPicture, Bitmap rightPicture)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";

                if (!string.IsNullOrEmpty(caption))
                {
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 1;
                    _paragraph.Range.Font.Size = 14;
                    _paragraph.Range.Text = caption;
                }

                _waitForm.SetProgress(40);

                string saveClipboardText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    saveClipboardText = Clipboard.GetText();
                }

                if (palette != null)
                {
                    Clipboard.SetImage(palette);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Paste();
                }

                _waitForm.SetProgress(60);

                if (leftPicture != null)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Range.Font.Size = 12;
                    _paragraph.Range.Text = "\r\n(S)";

                    Clipboard.SetImage(leftPicture);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Paste();
                }

                _waitForm.SetProgress(80);

                if (rightPicture != null)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Range.Font.Size = 12;
                    _paragraph.Range.Text = "\r\n(D)";

                    Clipboard.SetImage(rightPicture);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Paste();
                }

                if (string.IsNullOrEmpty(saveClipboardText))
                {
                    Clipboard.Clear();
                }
                else
                {
                    try
                    {
                        Clipboard.SetText(saveClipboardText);
                    }
                    catch
                    {
                        Clipboard.Clear();
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
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word карту повреждения плечевого сплетения
        /// </summary>
        /// <param name="brachialPlexusCard">Данные о повреждениях плечевого сплетения</param>
        /// <param name="isExportEnabled">Надо ли экспортировать картинку</param>
        public static void ExportBrachialPlexusCard(
            CBrachialPlexusCard brachialPlexusCard,
            bool isExportEnabled)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "Карта обследования пациента с повреждением плечевого сплетения";

                string sideName = brachialPlexusCard.SideOfCard == CardSide.Left
                    ? "(S)\r\n"
                    : "(D)\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 12;
                _paragraph.Range.Text = sideName;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Сосудистый статус: " + brachialPlexusCard.VascularStatus;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Диафрагма: " + brachialPlexusCard.Diaphragm;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Синдром Горнера: " + brachialPlexusCard.HornersSyndrome;

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Симптом Тинеля: " + brachialPlexusCard.TinelsSymptom;

                if (brachialPlexusCard.IsMyelographyEnabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "Миелография ({0}, {1}): {2}",
                        brachialPlexusCard.MyelographyType,
                        CConvertEngine.DateTimeToString(brachialPlexusCard.MyelographyDate),
                        brachialPlexusCard.Myelography);
                }

                if (brachialPlexusCard.IsEMNGEnabled)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = string.Format(
                        "ЭМНГ ({0}): {1}",
                        CConvertEngine.DateTimeToString(brachialPlexusCard.EMNGDate),
                        brachialPlexusCard.EMNG);
                }

                _waitForm.SetProgress(50);

                string saveClipboardText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    saveClipboardText = Clipboard.GetText();
                }

                AddEmptyParagraph();

                Clipboard.SetImage(Properties.Resources.palette);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Paste();

                _waitForm.SetProgress(80);

                if (isExportEnabled)
                {
                    Clipboard.SetImage(brachialPlexusCard.Picture);

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Paste();
                }

                if (string.IsNullOrEmpty(saveClipboardText))
                {
                    Clipboard.Clear();
                }
                else
                {
                    Clipboard.SetText(saveClipboardText);
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспортировать в Word карту объёма движений верхней конечности
        /// </summary>
        /// <param name="rangeOfMotionCard">Данные об объёме движений верхней конечности</param>
        public static void ExportRangeOfMotionCard(CRangeOfMotionCard rangeOfMotionCard)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "Объём движений верхней конечности";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 10;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = string.Format(
                    "\r\nПлечевой пояс:\tэлевация / депрессия (F: 20-0-10): акт - {0}, пасс – {1}",
                    rangeOfMotionCard.Fields[0],
                    rangeOfMotionCard.Fields[1]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\tсгибание/разгибание (Т: 20-0-20): акт - {0}, пасс – {1}",
                    rangeOfMotionCard.Fields[2],
                    rangeOfMotionCard.Fields[3]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Плечевой сустав:  разгибание/сгибание (S: 50-0-180): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[4],
                    rangeOfMotionCard.Fields[5]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t   отведение/приведение (F: 180-0-0): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[6],
                    rangeOfMotionCard.Fields[7]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t   горизонтальное разгибание и сгибание (Т: 30-0-135): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[8],
                    rangeOfMotionCard.Fields[9]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t   нар. и вн. ротация при отведенном на 90° плече (R: 90-0-90): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[10],
                    rangeOfMotionCard.Fields[11]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t   нар. и вн. ротация при приведенном плече (R: 65-0-70): акт - {0}, пасс - {1}.",
                    rangeOfMotionCard.Fields[12],
                    rangeOfMotionCard.Fields[13]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Локтевой сустав: разгибание и сгибание (S: 0-0-150): акт - {0}, пасс - {1}.",
                    rangeOfMotionCard.Fields[14],
                    rangeOfMotionCard.Fields[15]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Луче-локтевые суставы: супинация и пронация (R: 90-0-90): акт - {0}, пасс - {1}.",
                    rangeOfMotionCard.Fields[16],
                    rangeOfMotionCard.Fields[17]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "Лучезапястный сустав:\tразгибание и сгибание (S: 70-0-80): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[18],
                    rangeOfMotionCard.Fields[19]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\tотведение и приведение (F: 25-0-55): акт - {0}, пасс - {1}.",
                    rangeOfMotionCard.Fields[20],
                    rangeOfMotionCard.Fields[21]);

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Суставы 1-го пальца:";
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tЗПС:\tлучевое отведение и приведение (F: 35-0-15): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[22],
                    rangeOfMotionCard.Fields[23]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\tладонное отведение и приведение (S: 40-0-0): акт - {0}, пасс - {1}",
                    rangeOfMotionCard.Fields[24],
                    rangeOfMotionCard.Fields[25]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tПФС.\tразгибание и сгибание (S: 5-0-50): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[26],
                    rangeOfMotionCard.Fields[27]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tМФС.\tразгибание и сгибание (S: 15-0-85): акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[28],
                    rangeOfMotionCard.Fields[29]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2, 3 });

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\tоппозиция: {0} палец",
                    rangeOfMotionCard.OppositionFinger);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Суставы II-V-го пальцев:";
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 1, 2, 3, 4, 5, 6, 7, 8 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tПФС: разгибание и сгибание (S: 35-0-90): IIп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[30],
                    rangeOfMotionCard.Fields[31]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t  IIIп. - акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[32],
                    rangeOfMotionCard.Fields[33]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t  IVп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[34],
                    rangeOfMotionCard.Fields[35]);

                _waitForm.SetProgress(70);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t  V - акт - {0}, пасс - {1}",
                    rangeOfMotionCard.Fields[36],
                    rangeOfMotionCard.Fields[37]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tотведение и приведение (F: 30-0-25): IIп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[38],
                    rangeOfMotionCard.Fields[39]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t       IIIп. - акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[40],
                    rangeOfMotionCard.Fields[41]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t       IVп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[42],
                    rangeOfMotionCard.Fields[43]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t       V - акт - {0}, пасс - {1}",
                    rangeOfMotionCard.Fields[44],
                    rangeOfMotionCard.Fields[45]);

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tПМФС: разгибание и сгибание (S: 0-0-100): IIп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[46],
                    rangeOfMotionCard.Fields[47]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t     IIIп. - акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[48],
                    rangeOfMotionCard.Fields[49]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t     IVп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[50],
                    rangeOfMotionCard.Fields[51]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t     V - акт - {0}, пасс - {1}",
                    rangeOfMotionCard.Fields[52],
                    rangeOfMotionCard.Fields[53]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\tДМФС: разгибание и сгибание (S: 0-0-80): IIп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[54],
                    rangeOfMotionCard.Fields[55]);
                SetWordsInRangeUnderline(_paragraph.Range, new[] { 2 });

                _waitForm.SetProgress(90);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t   IIIп. - акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[56],
                    rangeOfMotionCard.Fields[57]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t   IVп. акт - {0}, пасс - {1};",
                    rangeOfMotionCard.Fields[58],
                    rangeOfMotionCard.Fields[59]);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = string.Format(
                    "\t\t\t\t\t\t   V - акт - {0}, пасс - {1}",
                    rangeOfMotionCard.Fields[60],
                    rangeOfMotionCard.Fields[61]);

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Экспорт карты для акушерского паралича
        /// </summary>
        /// <param name="obstetricParalysisCard">Данные об акушерском параличе</param>
        /// <param name="picture">Картинка, которую надо отобразить</param>
        public static void ExportObstetricParalysisCard(
            CObstetricParalysisCard obstetricParalysisCard, Bitmap picture)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "Карта на акушерский паралич";

                string sideName = obstetricParalysisCard.SideOfCard == CardSide.Left
                    ? "(S)\r\n"
                    : "(D)\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 12;
                _paragraph.Range.Text = sideName;

                string saveClipboardText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    saveClipboardText = Clipboard.GetText();
                }

                Clipboard.SetImage(picture);

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Paste();

                if (string.IsNullOrEmpty(saveClipboardText))
                {
                    Clipboard.Clear();
                }
                else
                {
                    Clipboard.SetText(saveClipboardText);
                }

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\r\nФункция кисти ";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Text = "Разгибание кисти и пальцев: " + obstetricParalysisCard.ComboBoxes[0];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Сгибание кисти и пальцев: " + obstetricParalysisCard.ComboBoxes[1];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "I-й палец:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tсгибание: " + obstetricParalysisCard.ComboBoxes[2];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tразгибание: " + obstetricParalysisCard.ComboBoxes[2];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tотведение: " + obstetricParalysisCard.ComboBoxes[2];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tприведение: " + obstetricParalysisCard.ComboBoxes[2];

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\tоппозиция: " + obstetricParalysisCard.ComboBoxes[2];

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Создать справку для консультации
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="visitInfo">Информация о консультации</param>
        public static void CreateVisitCertificate(CPatient patientInfo, CVisit visitInfo)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 90;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "ООО «МИФРМ» 150033, г.Ярославль, Тутаевское шоссе, 93";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 12;
                _paragraph.Range.Text = "Регистратура: 8 (4852) 91-66-63, факс: 8 (4852) 77-47-03";
                //_paragraph.Range.Text = "Регистратура: 8 (4852) 91-66-63, тел./факс 20-10-11";
                SetWordsInRangeBold(_paragraph.Range, new[] { 7, 8, 9, 10, 11, 19, 20, 21, 22, 23 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = visitInfo.Header + "\r\n";

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 12;
                _paragraph.Format.LineSpacing = 18;
                _paragraph.Range.Text = "ФИО: " + patientInfo.GetFullName();
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] diagnoseLines = visitInfo.Diagnose.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Диагноз: " + diagnoseLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (diagnoseLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = diagnoseLines[1];
                }

                _waitForm.SetProgress(70);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] evenlyLines = visitInfo.Evenly.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Объективно: " + evenlyLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (evenlyLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = evenlyLines[1];
                }

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                string[] recommendationLines = visitInfo.Recommendation.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                _paragraph.Range.Text = "Рекомендации: " + recommendationLines[0];
                SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2 });

                if (recommendationLines.Length > 1)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = recommendationLines[1];
                }

                _waitForm.SetProgress(80);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Text = string.Format(
                    "\t\t{0} г.\t\t\t____________________________________",
                    CConvertEngine.DateTimeToString(visitInfo.VisitDate));

                if (visitInfo.IsLastParagraphForCertificateNeeded)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Format.LineSpacing = 12;
                    _paragraph.Range.Text = "Необходимое обследование для оперативного лечения (срок годности анализа):";
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });
                    SetWordsInRangeItalic(_paragraph.Range, new[] { 6, 7, 8, 9, 10 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Range.Text = "1. Общий анализ крови (10 дн.) 2. Общий анализ мочи (10 дн.) 3. Анализ крови на сахар, ПТИ, креатинин, биллирубин, общий белок (10 дн.) 4. Анализ крови на HbsAg, НСV, RW, ВИЧ (3 мес.) 5. ЭКГ (10 дн.) 6. Флюорография (1 год) 7. Для женщин - осмотр гинеколога (6 мес) 8. Круппа крови и резус фактор (3 мес.)";
                    SetWordsInRangeItalic(_paragraph.Range, new[] { 6, 7, 8, 9, 15, 16, 17, 18, 34, 35, 36, 37, 50, 51, 52, 53, 57, 58, 59, 60, 64, 65, 66, 67, 75, 76, 77, 78, 86, 87, 88, 89 });
                }

                if (visitInfo.IsLastOdkbParagraphForCertificateNeeded)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Format.LineSpacing = 12;
                    _paragraph.Range.Text = "Необходимое обследование для оперативного лечения (срок годности анализа):";
                    SetWordsInRangeBold(_paragraph.Range, new[] { 1, 2, 3, 4, 5 });
                    SetWordsInRangeItalic(_paragraph.Range, new[] { 6, 7, 8, 9, 10 });

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Font.Bold = 0;
                    _paragraph.Range.Text = "1. Общий анализ крови (10 дн.) 2. Общий анализ мочи (10 дн.) 3. Анализ крови на сахар, ПТИ, креатинин, биллирубин, общий белок (10 дн.) 4. Анализ крови на HbsAg, НСV, RW, ВИЧ (3 мес.) 5. ЭКГ (10 дн.) 6. Анализ кала на я/г (10 дн.) 7. справка от педиатра о контактах и состоянии здоровья ребенка (2 дн.) 8. карта прививок и проб Манту 9. Флюорография для родителя госпитализируемого с ребенком (1 год) 10. Консультация фтизиатра (10 дней)";
                    SetWordsInRangeItalic(_paragraph.Range, new[] { 6, 7, 8, 9, 15, 16, 17, 18, 34, 35, 36, 37, 50, 51, 52, 53, 57, 58, 59, 60, 69, 70, 71, 72, 84, 85, 86, 87, 103, 104, 105, 106, 111, 112, 113, 114 });
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Создать документ "Договор о возмездном оказании медицинских услуг"
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="visitInfo">Информация о консультации</param>
        public static void CreateVisitContract(CPatient patientInfo, CVisit visitInfo)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 60;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";
                _wordRange.Paragraphs.LineSpacing = (float)9.5;

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "Договор о возмездном оказании медицинских услуг";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Size = 12;
                _paragraph.Range.Text = "№   ___  от  " + CConvertEngine.DateTimeToString(visitInfo.VisitDate) + "\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Text = "ООО «МИФРМ» в лице директора Абрамовой Ирины Леонидовны, действующего на основании Устава и лицензии № ЛО-76-01-002155 от 24 апреля 2017 года, именуемое в дальнейшем «Исполнитель», с одной стороны, и действующий от своего имени";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                _paragraph.Range.Text = patientInfo.IsSpecifyLegalRepresent ? patientInfo.LegalRepresent : patientInfo.GetFullName();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                _paragraph.Range.Text = "именуемый в дальнейшем «Заказчик», с другой стороны, заключили договор о нижеследующем:\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "1.Предмет договора";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "«Исполнитель» обязуется по поручению «Заказчика» оказать медицинскую услугу (услуги), а «Заказчик» обязуется оплатить эту медицинскую услугу (услуги), выполнять требования, обеспечивающие качественное предоставление данных медицинских услуг, включая предоставление «Исполнителю» сведений, необходимых для их оказания.\r\n";

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "2.Условия договора, порядок расчетов";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "«Исполнитель обязан оказать услугу в течение месяца со дня оплаты. «Заказчик» обязан оплатить услугу до ее оказания. Оплата услуг производится путем внесения денежных средств в кассу или перечисления на расчетный счет «Исполнителя». Медицинская услуга оплачивается «Заказчиком» по прейскуранту, действовавшему на момент обращения пациента за медицинской помощью. В случае, когда невозможность исполнения услуги возникла по обстоятельствам, за которые ни одна из сторон не отвечает. «Заказчик» оплачивает «Исполнителю» фактически понесенные им расходы.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "3.Гарантии качества и ответственность сторон";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "«Исполнитель» гарантирует своевременность, полноту, качество медицинских услуг в соответствии с установленными стандартами, нормативами и правилами оказания медицинской помощи. В случае некачественного оказания медицинской услуги, неисполнения или ненадлежащего исполнения условий договора, подтвержденных актом экспертизы, повторное их оказание и оплата услуг эксперта осуществляется за счет «Исполнителя». «Заказчик» ознакомлен с правами пациента, предупрежден о возможных осложнениях при оказании услуг.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "4.Рассмотрение споров";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Споры, возникающие   по настоящему Договору, рассматриваются и разрешаются по соглашению сторон либо в соответствии с действующим законодательством Российской Федерации.\r\n\r\nРеквизиты сторон:\r\nИсполнитель   					     Заказчик";

                _waitForm.SetProgress(70);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                object defaultTableBehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
                object autoFitBehavior = WdAutoFitBehavior.wdAutoFitFixed;
                _wordTable = _wordDoc.Tables.Add(_paragraph.Range, 1, 2, ref defaultTableBehavior, ref autoFitBehavior);
                _wordTable.Range.Font.Name = "Times New Roman";
                _wordTable.Range.Font.Size = 12;
                _wordTable.Range.Font.Bold = 0;
                _wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                _wordTable.Range.Paragraphs.LineSpacing = (float)12.0;

                _wordTable.Rows[1].Cells[1].Width = 285;
                _wordTable.Rows[1].Cells[2].Width = 210;

                _wordTable.Rows[1].Cells[1].Range.Text = "ООО «МИФРМ»\r\nИНН 7602130313\r\nКПП 760201001\r\nОРГН 1167627073448\r\nРасчетный  счет  40702810902410000118\r\nВ ПАО «МИнБанк» г. Москва\r\nБИК  044525600\r\nК.сч. 30101810300000000600\r\nАдрес: 150033, Ярославская область, г.Ярославль,\r\nТутаевское шоссе,д.93, помещение 43-50\r\n\r\nДиректор ___________________  Абрамова И.Л.\r\n";
                _wordTable.Rows[1].Cells[2].Range.Text = string.Format(
                    "ФИО: {0}\r\n____________________________________\r\nАдрес: {1}\r\n___________________________________\r\nИНН ______________________________\r\nПаспортные данные: {2}\r\n__________________________________\r\n\t\t(подпись)\r\n",
                    patientInfo.IsSpecifyLegalRepresent ? patientInfo.LegalRepresent : patientInfo.GetFullName(),
                    patientInfo.GetAddress(),
                    patientInfo.PassInformation.GetPassInformation());

                
                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _paragraph.Range.Text = "\r\n\r\nАкт на выполнение медицинских услуг";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Text = "Мы, нижеподписавшиеся,   «Исполнитель» с одной стороны и «Заказчик»с другой стороны, составили настоящий акт в том, что «Исполнитель» оказал «Заказчику» медицинские услуги по настоящему Договору в полном объеме в установленные сроки с надлежащим качеством. Стороны претензий друг к другу не имеют.\r\n\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 9;
                _paragraph.LineSpacing = (float)12.0;
                _paragraph.Range.Text = "ИСПОЛНИТЕЛЬ _______________________					ЗАКАЗЧИК________________";
                
                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        /// <summary>
        /// Создать документ "Информированное согласие"
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="visitInfo">Информация о консультации</param>
        public static void CreateVisitInformedConsent(CPatient patientInfo, CVisit visitInfo)
        {
            if (_blockDocGeneration)
            {
                return;
            }

            _blockDocGeneration = true;
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

                _waitForm.SetProgress(30);

                _wordDoc.PageSetup.TopMargin = 30;
                _wordDoc.PageSetup.LeftMargin = 75;
                _wordDoc.PageSetup.RightMargin = 30;
                _wordDoc.PageSetup.BottomMargin = 30;

                _wordRange = _wordDoc.Range(ref _missingObject, ref _missingObject);
                _wordRange.Font.Size = 12;
                _wordRange.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                _wordRange.Font.Name = "Times New Roman";

                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 1;
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Text = "ООО «МИФРМ»\r\nИНФОРМИРОВАННОЕ СОГЛАСИЕ ПАЦИЕНТА НА ПРОВЕДЕНИЕ ОБСЛЕДОВАНИЯ И ЛЕЧЕНИЯ\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Bold = 0;
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                _paragraph.Range.Text = patientInfo.GetFullName();

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                _paragraph.Range.Font.Size = 10;
                _paragraph.Range.Font.Name = "Arial";
                _paragraph.Range.Text = "(ФИО пациента и/или его законного представителя)";

                _waitForm.SetProgress(40);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Font.Size = 14;
                _paragraph.Range.Font.Name = "Times New Roman";
                _paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                _paragraph.Range.Text = "Мне подробно разъяснены моим лечащим врачом состояние моего здоровья, цели, характер, и объем планируемого обследования.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Я согласен(а) на выполнение инвазивных (пункция, катетеризация, зондирование, биопсия, эндоскопия и другие) и неинвазивных (клинические, физикальные, инструментальные, включая лучевые и другие) методик обследования и знаю обо всех возможных осложнениях, связанных с их применением.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Я подробно ознакомлен(а) лечащим врачом с возможными вариантами лечения и их исходами. Я добровольно выбрал(а) метод лечения и диагностики.\r\n______________________________________________________________________\r\nМой лечащий врач проинформировал меня и моих родственников о частоте возможных осложнений.  Я осознаю процентную вероятность этих осложнений и их последствий, и не буду иметь претензий при их возникновении.\r\n";

                _waitForm.SetProgress(50);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Я даю свое согласие на выбранные моим врачом методы и виды лекарственной терапии и физиотерапии.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Я не возражаю против фото- и видеосъемки меня и участков моего тела без идентификации личности. Даю согласие на использование полученных материалов для учебного процесса и научной работы.\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Со мною подробно обсуждены последствия отказа от обследования и лечения. Я получил(а) исчерпывающие и понятные мне ответы на все поставленные мною вопросы и имел(а) достаточно времени на обдумывание решения о согласии на предложенное обследование и  лечение.\r\n";

                _waitForm.SetProgress(60);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(visitInfo.VisitDate, true) + ",  Ф.И.О. и подпись пациента:";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\t" + patientInfo.GetFullName() + "   ____________\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(visitInfo.VisitDate, true) + ",  Ф.И.О. и подпись врача:";

                _waitForm.SetProgress(70);

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "\t" + visitInfo.Doctor + "   ____________\r\n";

                _wordDoc.Paragraphs.Add(ref _missingObject);
                _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                _paragraph.Range.Text = "Если пациент не может подписать информированное согласие своего состояния или по другим причинам (является несовершеннолетним, недееспособным):";

                _waitForm.SetProgress(85);

                if (patientInfo.IsSpecifyLegalRepresent)
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(visitInfo.VisitDate, true) + ",  Ф.И.О. и подпись законного представителя пациента:";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "\t" + patientInfo.LegalRepresent + "   ____________";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата " + CConvertEngine.DateTimeToString(visitInfo.VisitDate, true) + ",  Ф.И.О. и подпись врача:";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "\t" + visitInfo.Doctor + "   ____________\r\n";
                }
                else
                {
                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата ___________ 201__ г. Ф.И.О. и подпись законного представителя пациента:";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "_____________________________________________________________________";

                    _wordDoc.Paragraphs.Add(ref _missingObject);
                    _paragraph = _wordDoc.Paragraphs[_wordDoc.Paragraphs.Count];
                    _paragraph.Range.Text = "Дата ____________ 201__ г. Ф.И.О. и подпись врача:\r\n_____________________________________________________________________";
                }
                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _blockDocGeneration = false;
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
        /// <param name="hospitalization">Информация о госпитализации</param>
        /// <param name="visit">Информация о консультации</param>
        /// <param name="operationWorker">Классдля работы с операциями</param>
        /// <param name="dischargeEpicrisis">Информация о выписном эпикризе</param>
        /// /// <param name="globalSettings">Глобальные настройки</param>
        public static void ExportAdditionalDocument(
            object filePath,
            CPatient patientInfo,
            CHospitalization hospitalization,
            CVisit visit,
            COperationWorker operationWorker,
            CDischargeEpicrisis dischargeEpicrisis,
            CGlobalSettings globalSettings)
        {
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            _waitForm = new WaitForm();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                _waitForm.Show();
                System.Windows.Forms.Application.DoEvents();

                _wordApp = new Application();

                _wordDoc = _wordApp.Documents.OpenOld(ref filePath, ref _missingObject, ref _missingObject,
                    ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject,
                    ref _missingObject, ref _missingObject, ref _missingObject);

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
                        hospitalization,
                        visit,
                        operationWorker,
                        dischargeEpicrisis,
                        globalSettings);
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
                        hospitalization,
                        visit,
                        operationWorker,
                        dischargeEpicrisis,
                        globalSettings);
                }

                _waitForm.SetProgress(100);

                // Переходим к началу документа
                object unit = WdUnits.wdStory;
                object extend = WdMovementType.wdMove;
                _wordApp.Selection.HomeKey(ref unit, ref extend);
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waitForm.CloseForm();

                ReleaseComObject();

                Thread.CurrentThread.CurrentCulture = oldCi;
            }
        }


        private static void FindMarkAndReplace(
            string rangeText,
            Shape shape,
            double shift,
            ref double previousValue,
            ref double currentValue,
            CPatient patientInfo,
            CHospitalization hospitalization,
            CVisit visit,
            COperationWorker operationWorker,
            CDischargeEpicrisis dischargeEpicrisis,
            CGlobalSettings globalSettings)
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
                    string bracketNewText = GetRealParameterInsteadSpecialMark(bracketText, patientInfo, hospitalization, visit, operationWorker, dischargeEpicrisis, globalSettings);

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

        /// <summary>
        /// Вернуть нужное значение вместо параметра в документе
        /// </summary>
        /// <param name="mark">Метка в документе</param>
        /// <param name="patientInfo">Информация о пациенте</param>
        /// <param name="hospitalization">Информация о госпитализации</param>
        /// <param name="visit">Информация о консультации</param>
        /// <param name="operationWorker">Классдля работы с операциями</param>
        /// <param name="dischargeEpicrisis">Информация о выписном эпикризе</param>
        /// <param name="globalSettings">Глобальные настройки</param>
        /// <returns></returns>
        private static string GetRealParameterInsteadSpecialMark(
            string mark,
            CPatient patientInfo,
            CHospitalization hospitalization,
            CVisit visit,
            COperationWorker operationWorker,
            CDischargeEpicrisis dischargeEpicrisis,
            CGlobalSettings globalSettings)
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
                COperation[] operations;

                string startMark = string.Empty;
                if (mark.StartsWith("дата операции"))
                {
                    startMark = "дата операции";
                }
                else if (mark.StartsWith("даои"))
                {
                    startMark = "даои";
                }
                else if (mark.StartsWith("название операции"))
                {
                    startMark = "название операции";
                }
                else if (mark.StartsWith("неои"))
                {
                    startMark = "неои";
                }

                if (!string.IsNullOrEmpty(startMark))
                {
                    CheckHospitalization(hospitalization);
                    int operationNumber = Convert.ToInt32(mark.Substring(startMark.Length).Trim(' ')) - 1;
                    operations = operationWorker.GetListByHospitalizationId(hospitalization.Id);
                    if (operations.Length <= operationNumber)
                    {
                        throw new Exception("Нет операции с номером " + operationNumber);
                    }

                    if (startMark == "дата операции" || mark.StartsWith("даои"))
                    {
                        return operations[operationNumber].DateOfOperation.ToString("dd.MM.yyyy");
                    }

                    return operations[operationNumber].Name;
                }

                switch (mark)
                {
                    case "фио пациента":
                    case "фопа":
                        return patientInfo.GetFullName();
                    case "возраст":
                    case "вт":
                        return CConvertEngine.GetAge(patientInfo.Birthday);
                    case "адрес":
                    case "ас":
                        return patientInfo.GetAddress();
                    case "дата поступления":
                    case "дапя":
                        CheckHospitalization(hospitalization);
                        return hospitalization.DeliveryDate.ToString("dd.MM.yyyy");
                    case "дата выписки":
                    case "дави":
                        CheckHospitalization(hospitalization);
                        if (hospitalization.ReleaseDate.HasValue)
                        {
                            return hospitalization.ReleaseDate.Value.ToString("dd.MM.yyyy");
                        }

                        throw new Exception("Дата выписки не указана");
                    case "диагноз":
                    case "дз":
                        if (hospitalization != null)
                        {
                            return hospitalization.Diagnose;
                        }

                        if (visit != null)
                        {
                            return visit.Diagnose;
                        }

                        throw new Exception("Не выбрана госпитализация или консультация");
                    case "консервативная терапия":
                    case "кятя":
                        CheckDischargeEpicrisis(dischargeEpicrisis);
                        return dischargeEpicrisis.ConservativeTherapy;
                    case "№ истории болезни":
                    case "№ииби":
                    case "№ иб":
                        CheckHospitalization(hospitalization);
                        return hospitalization.NumberOfCaseHistory;
                    case "№ амбулаторной карты":
                    case "№ амб. карты":
                    case "№айкы":
                        return patientInfo.Id.ToString(CultureInfo.InvariantCulture);
                    case "№ отделения":
                    case "№оя":
                        return globalSettings.DepartmentName;
                    case "дата консультации":
                    case "даки":
                        CheckVisit(visit);
                        return visit.VisitDate.ToString("dd.MM.yyyy");
                    case "фио лечащего врача":
                    case "фолова":
                        if (hospitalization != null)
                        {
                            return hospitalization.DoctorInChargeOfTheCase;
                        }

                        if (visit != null)
                        {
                            return visit.Doctor;
                        }

                        throw new Exception("Не выбрана госпитализация или консультация");
                    case "фио зав. отделением":
                    case "фозавом":
                        return globalSettings.BranchManager;
                    case "сегодняшняя дата":
                    case "сяда":
                        return DateTime.Now.ToString("dd.MM.yyyy");
                    case "хирург":
                    case "хг":
                        CheckHospitalization(hospitalization);
                        operations = operationWorker.GetListByHospitalizationId(hospitalization.Id);
                        if (operations.Length > 0)
                        {
                            return CConvertEngine.ListToString(operations[0].Surgeons, ",");
                        }

                        return "{ХИРУРГ НЕ НАЙДЕН, Т.К. НЕТ ОПЕРАЦИЙ}";
                    case "анестезиолог":
                    case "аг":
                        CheckHospitalization(hospitalization);
                        operations = operationWorker.GetListByHospitalizationId(hospitalization.Id);
                        if (operations.Length > 0)
                        {
                            return operations[0].HeAnaesthetist;
                        }

                        return "{АНЕСТЕЗИОЛОГ НЕ НАЙДЕН, Т.К. НЕТ ОПЕРАЦИЙ}";
                    case "работа":
                    case "ра":
                        return patientInfo.WorkPlace;
                    default:
                        return mark.ToUpper();
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToUpper();
            }
        }

        private static void CheckVisit(CVisit visit)
        {
            if (visit == null)
            {
                throw new Exception("Консультация не найдена");
            }
        }


        private static void CheckDischargeEpicrisis(CDischargeEpicrisis dischargeEpicrisis)
        {
            if (dischargeEpicrisis == null)
            {
                throw new Exception("Выписной эпикриз не найден");
            }
        }


        private static void CheckHospitalization(CHospitalization hospitalization)
        {
            if (hospitalization == null)
            {
                throw new Exception("Госпитализация не найдена");
            }
        }


        /// <summary>
        /// Найти и заменить в документе
        /// </summary>
        /// <param name="findText">Текст для поиска</param>
        /// <param name="replaceText">Текст для замены</param>
        private static void FindAndReplace(string findText, string replaceText)
        {
            _wordApp.Selection.Find.ClearFormatting();
            _wordApp.Selection.Find.Text = findText;

            _wordApp.Selection.Find.Replacement.ClearFormatting();
            _wordApp.Selection.Find.Replacement.Text = replaceText;

            object replaceAll = WdReplace.wdReplaceAll;

            _wordApp.Selection.Find.Execute(
                ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject,
                ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject,
                ref replaceAll, ref _missingObject, ref _missingObject, ref _missingObject, ref _missingObject);
        }


        /// <summary>
        /// Освободить ресурсы после генерации документа
        /// </summary>
        private static void ReleaseComObject()
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
        private static void AddEmptyParagraph()
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
        private static void SetWordsInRangeBold(Range range, IEnumerable<int> wordNumbers)
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
        /// Выделить курсивом переданные номера слов в текущем Range
        /// </summary>
        /// <param name="range">Range, в котором выделяем слова</param>
        /// <param name="wordNumbers">Номера слов в Range, которые надо выделить курсивом</param>
        private static void SetWordsInRangeItalic(Range range, IEnumerable<int> wordNumbers)
        {
            foreach (int wordNumber in wordNumbers)
            {
                if (range.Words.Count > wordNumber)
                {
                    range.Words[wordNumber].Italic = 1;
                }
            }
        }


        /// <summary>
        /// Сделать подчёркнутыми переданные номера слов в текущем Range
        /// </summary>
        /// <param name="range">Range, в котором выделяем слова</param>
        /// <param name="wordNumbers">Номера слов в Range, которые надо сделать жирными</param>
        private static void SetWordsInRangeUnderline(Range range, IEnumerable<int> wordNumbers)
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
