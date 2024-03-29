﻿using System;
using System.Collections.Generic;
using System.Text;
using SurgeryHelper.Engines;

namespace SurgeryHelper.Entities
{
    /// <summary>
    /// Класс с данными по пациенту
    /// </summary>
    public class PatientClass
    {
        /// <summary>
        /// Открытая для этого пациента форма с данными, если она есть
        /// </summary>
        public PatientViewForm OpenedPatientViewForm;

        /// <summary>
        /// Уникальный эдентификатор пользователя
        /// </summary>
        public int Id;

        /// <summary>
        /// Фамилия пациента
        /// </summary>
        public string LastName;

        /// <summary>
        /// Имя пациента
        /// </summary>
        public string Name;

        /// <summary>
        /// Отчество пациента
        /// </summary>
        public string Patronymic;

        /// <summary>
        /// День рождения пациента
        /// </summary>
        public DateTime Birthday;

        /// <summary>
        /// Город проживания
        /// </summary>
        public string CityName;

        /// <summary>
        /// Улица проживания
        /// </summary>
        public string StreetName;

        /// <summary>
        /// Номер дома
        /// </summary>
        public string HomeNumber;

        /// <summary>
        /// Номер корпуса
        /// </summary>
        public string BuildingNumber;

        /// <summary>
        /// Номер квартиры
        /// </summary>
        public string FlatNumber;

        /// <summary>
        /// Место работы
        /// </summary>
        public string WorkPlace;

        /// <summary>
        /// Серия и номер паспорта
        /// </summary>
        public string PassportNumber;

        /// <summary>
        /// Номер полиса
        /// </summary>
        public string PolisNumber;

        /// <summary>
        /// Номер СНИЛС
        /// </summary>
        public string SnilsNumber;

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone;

        /// <summary>
        /// Тип стационара
        /// </summary>
        public string TypeOfKSG;

        /// <summary>
        /// Код МКБ
        /// </summary>
        public string MKB;

        /// <summary>
        /// Код МКБ для сопутствующего диагноза
        /// </summary>
        public string ConcomitantDiagnoseMKB;

        /// <summary>
        /// Код МКБ для осложнений
        /// </summary>
        public string ComplicationsMKB;

        /// <summary>
        /// Код WWW
        /// </summary>
        public string WWW;

        /// <summary>
        /// Название услуги
        /// </summary>
        public string ServiceName;

        /// <summary>
        /// Код услуги
        /// </summary>
        public string ServiceCode;

        /// <summary>
        /// Код КСГ
        /// </summary>
        public string KsgCode;

        /// <summary>
        /// Расшифровка КСГ
        /// </summary>
        public string KsgDecoding;

        /// <summary>
        /// Дата поступления
        /// </summary>
        public DateTime DeliveryDate;

        /// <summary>
        /// Дата выписки
        /// </summary>
        public DateTime? ReleaseDate;

        /// <summary>
        /// Номер истории болезни
        /// </summary>
        public string NumberOfCaseHistory;

        /// <summary>
        /// Диагноз пациента
        /// </summary>
        public string Diagnose;

        /// <summary>
        /// Сопутствующий диагноз пациента
        /// </summary>
        public string ConcomitantDiagnose;

        /// <summary>
        /// Осложнения
        /// </summary>
        public string Complications;
        
        /// <summary>
        /// Лечащий врач
        /// </summary>
        public string DoctorInChargeOfTheCase;

        /// <summary>
        /// Нозология пациента
        /// </summary>
        public string Nosology;

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        public string Hospitalization;

        /// <summary>
        /// Поле с путём до файлов данного пациента
        /// </summary>
        public string PrivateFolder;

        /// <summary>
        /// Список операций
        /// </summary>
        public List<OperationClass> Operations;

        /// <summary>
        /// Дополнительная информация для переводного эпикриза
        /// </summary>
        public string TransferEpicrisAdditionalInfo;

        /// <summary>
        /// Послеоперационный период для переводного  эпикриза
        /// </summary>
        public string TransferEpicrisAfterOperationPeriod;

        /// <summary>
        /// Планирующиеся действия для переводного  эпикриза
        /// </summary>
        public string TransferEpicrisPlan;

        /// <summary>
        /// Дата написания документа для переводного эпикриза
        /// </summary>
        public DateTime TransferEpicrisWritingDate;

        /// <summary>
        /// Личный номер для переводного эпикриза
        /// </summary>
        public string TransferEpicrisDisabilityList;

        /// <summary>
        /// Включать ли личный номер в отчёт
        /// </summary>
        public bool TransferEpicrisIsIncludeDisabilityList;

        /// <summary>
        /// Дополнительная информация для этапного эпикриза
        /// </summary>
        public string LineOfCommEpicrisAdditionalInfo;

        /// <summary>
        /// Планирующиеся действия для этапного эпикриза
        /// </summary>
        public string LineOfCommEpicrisPlan;

        /// <summary>
        /// Дата написания документа для этапного эпикриза
        /// </summary>
        public DateTime LineOfCommEpicrisWritingDate;

        /// <summary>
        /// Дата взятия анализов
        /// </summary>
        public DateTime? DischargeEpicrisAnalysisDate;

        /// <summary>
        /// После операции
        /// </summary>
        public string DischargeEpicrisAfterOperation;

        /// <summary>
        /// Общий анализ крови, эритроциты
        /// </summary>
        public string DischargeEpicrisOakEritrocits;

        /// <summary>
        /// Общий анализ крови, лекоциты
        /// </summary>
        public string DischargeEpicrisOakLekocits;

        /// <summary>
        /// Общий анализ крови, Hb
        /// </summary>
        public string DischargeEpicrisOakHb;

        /// <summary>
        /// Общий анализ крови, СОЭ
        /// </summary>
        public string DischargeEpicrisOakSoe;

        /// <summary>
        /// Общий анализ мочи, цвет
        /// </summary>
        public string DischargeEpicrisOamColor;

        /// <summary>
        /// Общий анализ мочи, относительная плотность
        /// </summary>
        public string DischargeEpicrisOamDensity;

        /// <summary>
        /// Общий анализ мочи, эритроциты
        /// </summary>
        public string DischargeEpicrisOamEritrocits;

        /// <summary>
        /// Общий анализ мочи, лейкоциты
        /// </summary>
        public string DischargeEpicrisOamLekocits;

        /// <summary>
        /// Биохимический анализ крови, биллирубин
        /// </summary>
        public string DischargeEpicrisBakBillirubin;

        /// <summary>
        /// Биохимический анализ крови, общий белок
        /// </summary>
        public string DischargeEpicrisBakGeneralProtein;

        /// <summary>
        /// Биохимический анализ крови, сахар
        /// </summary>
        public string DischargeEpicrisBakSugar;

        /// <summary>
        /// Биохимический анализ крови, ПТИ
        /// </summary>
        public string DischargeEpicrisBakPTI;

        /// <summary>
        /// Биохимический анализ крови, группа крови
        /// </summary>
        public string DischargeEpicrisBloodGroup;

        /// <summary>
        /// Биохимический анализ крови, резус фактор
        /// </summary>
        public string DischargeEpicrisRhesusFactor;

        /// <summary>
        /// Общий анализ мочи, другие анализы
        /// </summary>
        public string DischargeEpicrisAdditionalAnalises;

        /// <summary>
        /// ЭКГ пациента
        /// </summary>
        public string DischargeEpicrisEkg;

        /// <summary>
        /// Осмотры врачей-специалистов, консилиумы врачей
        /// </summary>
        public string DischargeEpicrisConsultation;

        /// <summary>
        /// Рекомендации при выписке
        /// </summary>
        public List<string> DischargeEpicrisRecomendations;

        /// <summary>
        /// Дополнительные рекомендации при выписке
        /// </summary>
        public List<string> DischargeEpicrisAdditionalRecomendations;

        /// <summary>
        /// Осмотр в отделении, общие данные, включен ли план осмотра в отчёт
        /// </summary>
        public bool MedicalInspectionIsPlanEnabled;

        /// <summary>
        /// Осмотр в отделении, общие данные, генерировать осмотр в отделении совместно с начальником
        /// </summary>
        public bool MedicalInspectionWithBoss;

        /// <summary>
        /// Осмотр в отделении, общие данные, обследование
        /// </summary>
        public string MedicalInspectionInspectionPlan;

        /// <summary>
        /// Тип лечения: консервативный или оперативный
        /// </summary>
        public string MedicalInspectionTreatmentType;

        /// <summary>
        /// Осмотр в отделении, общие данные, жалобы
        /// </summary>
        public string MedicalInspectionComplaints;

        /// <summary>
        /// Осмотр в отделении, общие данные, риск ТЭО
        /// </summary>
        public string MedicalInspectionTeoRisk;

        /// <summary>
        /// Осмотр в отделении, общие данные, наличие параметров для риска ТЭО
        /// </summary>
        public bool MedicalInspectionTeoRiskEnabled;
        
        /// <summary>
        /// Осмотр в отделении, общие данные, 1, 2 или 3
        /// </summary>
        public int MedicalInspectionExpertAnamnese;

        /// <summary>
        /// Осмотр в отделении, общие данные, выдан амбулаторно с
        /// </summary>
        public DateTime MedicalInspectionLnWithNumberDateStart;

        /// <summary>
        /// Осмотр в отделении, общие данные, выдан амбулаторно до
        /// </summary>
        public DateTime MedicalInspectionLnWithNumberDateEnd;

        /// <summary>
        /// Осмотр в отделении, общие данные, выдан первично с
        /// </summary>
        public DateTime MedicalInspectionLnFirstDateStart;        

        /// <summary>
        /// Включён ли анамнез в общий отчёт
        /// </summary>
        public bool MedicalInspectionIsAnamneseActive;

        /// <summary>
        /// Осмотр в отделении, анамнез, AnMorbi
        /// </summary>
        public string MedicalInspectionAnamneseAnMorbi;

        /// <summary>
        /// Осмотр в отделении, анамнез, AnVitae
        /// </summary>
        public bool[] MedicalInspectionAnamneseAnVitae;

        /// <summary>
        /// Осмотр в отделении, анамнез, поля
        /// </summary>
        public string[] MedicalInspectionAnamneseTextBoxes;

        /// <summary>
        /// Осмотр в отделении, анамнез, checkbox-ы
        /// </summary>
        public bool[] MedicalInspectionAnamneseCheckboxes;

        /// <summary>
        /// Осмотр в отделении, st.praesens, текстовые поля
        /// </summary>
        public string[] MedicalInspectionStPraesensTextBoxes;

        /// <summary>
        /// Осмотр в отделении, st.praesens, комбобоксы
        /// </summary>
        public string[] MedicalInspectionStPraesensComboBoxes;

        /// <summary>
        /// Осмотр в отделении, st.praesens, числовые поля
        /// </summary>
        public int[] MedicalInspectionStPraesensNumericUpDowns;

        /// <summary>
        /// Осмотр в отделении, st.praesens, разное
        /// </summary>
        public string MedicalInspectionStPraesensOthers;

        /// <summary>
        /// Осмотр в отделении, st.praesens, температура
        /// </summary>
        public string MedicalInspectionStPraesensTemperature;

        /// <summary>
        /// Осмотр в отделении, описание St. localis-а
        /// </summary>
        public string MedicalInspectionStLocalisDescription;

        /// <summary>
        /// Осмотр в отделении, тип рентгена
        /// </summary>
        public string MedicalInspectionStLocalisRentgen;

        /// <summary>
        /// Включён ли st.localis часть 1 в общий отчёт
        /// </summary>
        public bool MedicalInspectionIsStLocalisPart1Enabled;

        /// <summary>
        /// Осмотр в отделении, st.localis part1, поля
        /// </summary>
        public string[] MedicalInspectionStLocalisPart1Fields;

        /// <summary>
        /// Осмотр в отделении, st.localis part1, номер пальца в оппозиции
        /// </summary>
        public string MedicalInspectionStLocalisPart1OppositionFinger;

        /// <summary>
        /// Назначенные препараты для консервативной терапии (с датой назначения после символа &amp;)
        /// </summary>
        public List<string> PrescriptionTherapy;

        /// <summary>
        /// Назначенные дополнительные методы обследования (с датой назначения после символа &amp;)
        /// </summary>
        public List<string> PrescriptionSurveys;

        /// <summary>
        /// Возраст пациента
        /// </summary>
        public string Age
        {
            get
            {
                return ConvertEngine.GetAge(Birthday);
            }
        }

        /// <summary>
        /// Консервативное лечение
        /// </summary>
        public string GetDischargeEpicrisConservativeTherapy()
        {
            StringBuilder result = new StringBuilder();

            foreach (string therapy in PrescriptionTherapy)
            {
                string[] therapyData = therapy.Split(new[] { '&' }, StringSplitOptions.None);
                if (therapyData.Length > 1)
                {
                    result.Append(therapyData[0] + GetDurationWithWords(therapyData[1]) + ", ");
                }
            }

            return result.Length > 2 ? result.ToString().Substring(0, result.Length - 2) : result.ToString();
        }

        private string GetDurationWithWords(string duration)
        {
            string result = "";
            if (!string.IsNullOrEmpty(duration))
            {
                string text;
                int cnt;
                if (int.TryParse(duration, out cnt))
                {
                    int rem = cnt > 10 ? cnt % 10 : cnt;

                    if ((cnt >= 5 && cnt <= 20) || rem < 1 || rem > 4)
                    {
                        text = " дней";
                    }
                    else if (rem == 1)
                    {
                        text = " день";
                    }
                    else
                    {
                        text = " дня";
                    }
                }
                else
                {
                    text = " дней";
                }

                result = " " + duration + text;
            }

            return result;
        }

        /// <summary>
        /// Вернуть полный адрес
        /// </summary>
        /// <returns></returns>
        public string GetAddress()
        {
            string address = string.Empty;

            if (!string.IsNullOrEmpty(CityName))
            {
                address += ", " + CityName;
            }

            if (!string.IsNullOrEmpty(StreetName))
            {
                address += ", улица " + StreetName;
            }

            if (!string.IsNullOrEmpty(HomeNumber))
            {
                address += ", дом №" + HomeNumber;
            }

            if (!string.IsNullOrEmpty(BuildingNumber))
            {
                address += ", корпус " + BuildingNumber;
            }

            if (!string.IsNullOrEmpty(FlatNumber))
            {
                address += ", квартира №" + FlatNumber;
            }

            if (address.Length > 2)
            {
                address = address.Substring(2);
            }

            return address;
        }

        /// <summary>
        /// Д/Н
        /// </summary>
        /// <returns></returns>
        public string GetDN()
        {
            if (NumberOfCaseHistory.ToLower().Contains("д"))
            {
                return "Д";
            }

            return "Н";
        }

        /// <summary>
        /// к/д
        /// </summary>
        /// <returns></returns>
        public string GetKD()
        {
            var tempKD = ReleaseDate.HasValue ? (ReleaseDate.Value.Date - DeliveryDate.Date).Days : (DateTime.Now.Date - DeliveryDate.Date).Days;

            if (NumberOfCaseHistory.ToLower().Contains("д"))
            {
                return (tempKD + 1).ToString();
            }

            return tempKD.ToString();
        }

        /// <summary>
        /// Вернуть полное имя пациента (Фалимия Имя Отчество)
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            return LastName + " " + Name + " " + Patronymic;
        }

        public PatientClass()
        {
            Id = 0;
            Operations = new List<OperationClass>();
            ConcomitantDiagnose = "нет";
            Complications = "нет";
            Hospitalization = "плановая";

            TransferEpicrisAfterOperationPeriod = "без особенностей";
            TransferEpicrisPlan = "перевязки до заживления ран, ЛФК";
            TransferEpicrisWritingDate = DateTime.Now;

            LineOfCommEpicrisPlan = "перевязки до заживления ран, ЛФК";
            LineOfCommEpicrisWritingDate = DateTime.Now;

            DischargeEpicrisAnalysisDate = DateTime.Now;
            DischargeEpicrisAfterOperation = "раны зажили первичным натяжением, швы сняты.";
            PrescriptionTherapy = new List<string>
            {
                string.Format("S. Ceftriaxoni 1,0 – 1 раз в день в/м&3&{0}", ConvertEngine.GetRightDateString(DateTime.Now)),
                string.Format("S. Ketoroli 1,0 – 3 раза в день в/м&3&{0}", ConvertEngine.GetRightDateString(DateTime.Now)),
            };
            PrescriptionSurveys = new List<string>();
            DischargeEpicrisOamColor = "с/ж";
            DischargeEpicrisOamDensity = "1015";
            DischargeEpicrisOamEritrocits = "нет";
            DischargeEpicrisOamLekocits = "нет";
            DischargeEpicrisEkg = "без патологии";
            DischargeEpicrisRecomendations = new List<string> { "notdefined" };
            DischargeEpicrisAdditionalRecomendations = new List<string> { "notdefined" };
            DischargeEpicrisAdditionalAnalises = "анализ крови на ВИЧ - отр.";

            MedicalInspectionExpertAnamnese = 3;
            MedicalInspectionIsPlanEnabled = true;
            MedicalInspectionWithBoss = true;
            MedicalInspectionInspectionPlan = "ОАК, ОАМ, ЭКГ, биохимический анализ крови";
            MedicalInspectionTreatmentType = "оперативное";
            MedicalInspectionTeoRisk = "отсутствует";
            MedicalInspectionTeoRiskEnabled = false;
            MedicalInspectionExpertAnamnese = 3;
            MedicalInspectionStLocalisRentgen = "без костной патологии";
            MedicalInspectionComplaints = "";

            MedicalInspectionAnamneseAnVitae = new bool[4];
            MedicalInspectionAnamneseTextBoxes = new[] // string[10]
            {
                "отрицает",
                "не болел",
                "не выполнялись",
                "не имеет",
                "нет",
                "не выполнялось",
                "не было",
                "отрицает",
                "отрицает",
                "отрицает"
            };
            MedicalInspectionAnamneseCheckboxes = new bool[12];

            MedicalInspectionStPraesensTextBoxes = new[] // string[17]
            {
                "ясное",
                "активное",
                "телесного цвета, обычной влажности, без патологических высыпаний",                
                "",
                "не пальпируются",
                "проводится во всех отделах",
                "нет",
                "правильный",
                "",
                @"пальпация органов брюшной полости безболезненна, печень по краю реберной дуги, селезенка: не пальпируется
оценка характера стула и кратности дефекации: без особенностей
наличие симптомов раздражения брюшины: не определяются
результат пальцевого ректального исследования: не проводилось
результаты обследования мочеполовой системы: патологических изменений не определяется, область проекции почек внешне не изменена, симптом поколачивания отрицательный
оценка характера мочеиспускания: свободное, не затруднено
наличие менингеальных симптомов: не определяются",
                "",
                "",
                "",
                "",
                "",
                "",
                "без патологических изменений"
            };
            MedicalInspectionStPraesensComboBoxes = new[] // string[5]
            {
                "удовлетворительное",
                "нормальное",
                "везикулярное",
                "ясные",
                "15"
            };
            MedicalInspectionStPraesensNumericUpDowns = new[] // int[4]
            {
                18,
                76,
                120, 
                70,
                99,
                170,
                70
            };
            MedicalInspectionStPraesensTemperature = "36,6";

            MedicalInspectionStLocalisPart1OppositionFinger = "I";
            MedicalInspectionStLocalisPart1Fields = new string[62];
            for (int i = 0; i < MedicalInspectionStLocalisPart1Fields.Length; i++)
            {
                MedicalInspectionStLocalisPart1Fields[i] = "N";
            }

            MedicalInspectionLnFirstDateStart = DateTime.Now;
            MedicalInspectionLnWithNumberDateEnd = DateTime.Now;
            MedicalInspectionLnWithNumberDateStart = DateTime.Now;
        }
        
        public PatientClass(PatientClass patientClass)
        {
            Id = patientClass.Id;
            LastName = patientClass.LastName;
            Name = patientClass.Name;
            Patronymic = patientClass.Patronymic;
            Birthday = ConvertEngine.CopyDateTime(patientClass.Birthday);
            BuildingNumber = patientClass.BuildingNumber;
            CityName = patientClass.CityName;
            Diagnose = patientClass.Diagnose;
            ConcomitantDiagnose = patientClass.ConcomitantDiagnose;
            Complications = patientClass.Complications;
            DoctorInChargeOfTheCase = patientClass.DoctorInChargeOfTheCase;
            FlatNumber = patientClass.FlatNumber;
            WorkPlace = patientClass.WorkPlace;
            PassportNumber = patientClass.PassportNumber;
            PolisNumber = patientClass.PolisNumber;
            SnilsNumber = patientClass.SnilsNumber;
            Phone = patientClass.Phone;
            TypeOfKSG = patientClass.TypeOfKSG;
            MKB = patientClass.MKB;
            ConcomitantDiagnoseMKB = patientClass.ConcomitantDiagnoseMKB;
            ComplicationsMKB = patientClass.ComplicationsMKB;
            WWW = patientClass.WWW;
            ServiceName = patientClass.ServiceName;
            ServiceCode = patientClass.ServiceCode;
            KsgCode = patientClass.KsgCode;
            KsgDecoding = patientClass.KsgDecoding;
            HomeNumber = patientClass.HomeNumber;
            Nosology = patientClass.Nosology;
            Hospitalization = patientClass.Hospitalization;
            NumberOfCaseHistory = patientClass.NumberOfCaseHistory;
            StreetName = patientClass.StreetName;
            PrivateFolder = patientClass.PrivateFolder;

            DeliveryDate = ConvertEngine.CopyDateTime(patientClass.DeliveryDate);
            ReleaseDate = ConvertEngine.CopyDateTime(patientClass.ReleaseDate);

            Operations = new List<OperationClass>();
            foreach (OperationClass operation in patientClass.Operations)
            {
                var newOperationClass = new OperationClass(operation);
                Operations.Add(newOperationClass);
            }

            TransferEpicrisAfterOperationPeriod = patientClass.TransferEpicrisAfterOperationPeriod;
            TransferEpicrisPlan = patientClass.TransferEpicrisPlan;
            TransferEpicrisWritingDate = ConvertEngine.CopyDateTime(patientClass.TransferEpicrisWritingDate);
            TransferEpicrisAdditionalInfo = patientClass.TransferEpicrisAdditionalInfo;
            TransferEpicrisDisabilityList = patientClass.TransferEpicrisDisabilityList;
            TransferEpicrisIsIncludeDisabilityList = patientClass.TransferEpicrisIsIncludeDisabilityList;

            LineOfCommEpicrisAdditionalInfo = patientClass.LineOfCommEpicrisAdditionalInfo;
            LineOfCommEpicrisPlan = patientClass.LineOfCommEpicrisPlan;
            LineOfCommEpicrisWritingDate = ConvertEngine.CopyDateTime(patientClass.LineOfCommEpicrisWritingDate);

            DischargeEpicrisAnalysisDate = patientClass.DischargeEpicrisAnalysisDate;
            DischargeEpicrisAfterOperation = patientClass.DischargeEpicrisAfterOperation;            
            DischargeEpicrisEkg = patientClass.DischargeEpicrisEkg;
            DischargeEpicrisConsultation = patientClass.DischargeEpicrisConsultation;
            DischargeEpicrisOakEritrocits = patientClass.DischargeEpicrisOakEritrocits;
            DischargeEpicrisOakHb = patientClass.DischargeEpicrisOakHb;
            DischargeEpicrisOakLekocits = patientClass.DischargeEpicrisOakLekocits;
            DischargeEpicrisOakSoe = patientClass.DischargeEpicrisOakSoe;
            DischargeEpicrisOamColor = patientClass.DischargeEpicrisOamColor;
            DischargeEpicrisOamDensity = patientClass.DischargeEpicrisOamDensity;
            DischargeEpicrisOamEritrocits = patientClass.DischargeEpicrisOamEritrocits;
            DischargeEpicrisOamLekocits = patientClass.DischargeEpicrisOamLekocits;
            DischargeEpicrisBakBillirubin = patientClass.DischargeEpicrisBakBillirubin;
            DischargeEpicrisBakGeneralProtein = patientClass.DischargeEpicrisBakGeneralProtein;
            DischargeEpicrisBakPTI = patientClass.DischargeEpicrisBakPTI;
            DischargeEpicrisBakSugar = patientClass.DischargeEpicrisBakSugar;
            DischargeEpicrisBloodGroup = patientClass.DischargeEpicrisBloodGroup;
            DischargeEpicrisRhesusFactor = patientClass.DischargeEpicrisRhesusFactor;

            DischargeEpicrisAdditionalAnalises = patientClass.DischargeEpicrisAdditionalAnalises;

            DischargeEpicrisRecomendations = new List<string>(patientClass.DischargeEpicrisRecomendations);            

            DischargeEpicrisAdditionalRecomendations = new List<string>(patientClass.DischargeEpicrisAdditionalRecomendations);

            PrescriptionTherapy = new List<string>(patientClass.PrescriptionTherapy);
            PrescriptionSurveys = new List<string>(patientClass.PrescriptionSurveys);

            MedicalInspectionAnamneseAnMorbi = patientClass.MedicalInspectionAnamneseAnMorbi;
            MedicalInspectionAnamneseAnVitae = CopyBoolArray(patientClass.MedicalInspectionAnamneseAnVitae);
            MedicalInspectionAnamneseCheckboxes = CopyBoolArray(patientClass.MedicalInspectionAnamneseCheckboxes);
            MedicalInspectionAnamneseTextBoxes = CopyStringArray(patientClass.MedicalInspectionAnamneseTextBoxes);
            MedicalInspectionComplaints = patientClass.MedicalInspectionComplaints;
            MedicalInspectionExpertAnamnese = patientClass.MedicalInspectionExpertAnamnese;
            MedicalInspectionStLocalisDescription = patientClass.MedicalInspectionStLocalisDescription;
            MedicalInspectionStLocalisRentgen = patientClass.MedicalInspectionStLocalisRentgen;
            MedicalInspectionInspectionPlan = patientClass.MedicalInspectionInspectionPlan;
            MedicalInspectionTreatmentType = patientClass.MedicalInspectionTreatmentType;
            MedicalInspectionIsAnamneseActive = patientClass.MedicalInspectionIsAnamneseActive;
            MedicalInspectionIsPlanEnabled = patientClass.MedicalInspectionIsPlanEnabled;
            MedicalInspectionWithBoss = patientClass.MedicalInspectionWithBoss;
            MedicalInspectionIsStLocalisPart1Enabled = patientClass.MedicalInspectionIsStLocalisPart1Enabled;
            MedicalInspectionLnFirstDateStart = ConvertEngine.CopyDateTime(patientClass.MedicalInspectionLnFirstDateStart);
            MedicalInspectionLnWithNumberDateEnd = ConvertEngine.CopyDateTime(patientClass.MedicalInspectionLnWithNumberDateEnd);
            MedicalInspectionLnWithNumberDateStart = ConvertEngine.CopyDateTime(patientClass.MedicalInspectionLnWithNumberDateStart);
            MedicalInspectionStLocalisPart1Fields = CopyStringArray(patientClass.MedicalInspectionStLocalisPart1Fields);
            MedicalInspectionStLocalisPart1OppositionFinger = patientClass.MedicalInspectionStLocalisPart1OppositionFinger;
            MedicalInspectionStPraesensComboBoxes = CopyStringArray(patientClass.MedicalInspectionStPraesensComboBoxes);
            MedicalInspectionStPraesensNumericUpDowns = CopyIntArray(patientClass.MedicalInspectionStPraesensNumericUpDowns);
            MedicalInspectionStPraesensOthers = patientClass.MedicalInspectionStPraesensOthers;
            MedicalInspectionStPraesensTemperature = patientClass.MedicalInspectionStPraesensTemperature;
            MedicalInspectionStPraesensTextBoxes = CopyStringArray(patientClass.MedicalInspectionStPraesensTextBoxes);
            MedicalInspectionTeoRisk = patientClass.MedicalInspectionTeoRisk;
            MedicalInspectionTeoRiskEnabled = patientClass.MedicalInspectionTeoRiskEnabled;
        }

        /// <summary>
        /// Скопировать данные о пациенте в переданного пациента (без использования new)
        /// </summary>
        /// <param name="patientInfo">Информация о пациенте</param>
        public void Copy(PatientClass patientInfo)
        {
            patientInfo.Id = Id;
            patientInfo.LastName = LastName;
            patientInfo.Name = Name;
            patientInfo.Patronymic = Patronymic;
            patientInfo.Birthday = ConvertEngine.CopyDateTime(Birthday);
            patientInfo.BuildingNumber = BuildingNumber;
            patientInfo.CityName = CityName;
            patientInfo.Diagnose = Diagnose;
            patientInfo.ConcomitantDiagnose = ConcomitantDiagnose;
            patientInfo.Complications = Complications;
            patientInfo.DoctorInChargeOfTheCase = DoctorInChargeOfTheCase;
            patientInfo.FlatNumber = FlatNumber;
            patientInfo.WorkPlace = WorkPlace;
            patientInfo.PassportNumber = PassportNumber;
            patientInfo.PolisNumber = PolisNumber;
            patientInfo.SnilsNumber = SnilsNumber;
            patientInfo.Phone = Phone;
            patientInfo.TypeOfKSG = TypeOfKSG;
            patientInfo.MKB = MKB;
            patientInfo.ConcomitantDiagnoseMKB = ConcomitantDiagnoseMKB;
            patientInfo.ComplicationsMKB = ComplicationsMKB;
            patientInfo.WWW = WWW;
            patientInfo.ServiceName = ServiceName;
            patientInfo.ServiceCode = ServiceCode;
            patientInfo.KsgCode = KsgCode;
            patientInfo.KsgDecoding = KsgDecoding;
            patientInfo.HomeNumber = HomeNumber;
            patientInfo.Nosology = Nosology;
            patientInfo.Hospitalization = Hospitalization;
            patientInfo.NumberOfCaseHistory = NumberOfCaseHistory;
            patientInfo.StreetName = StreetName;
            patientInfo.PrivateFolder = PrivateFolder;

            patientInfo.DeliveryDate = ConvertEngine.CopyDateTime(DeliveryDate);
            patientInfo.ReleaseDate = ConvertEngine.CopyDateTime(ReleaseDate);

            patientInfo.Operations = new List<OperationClass>();
            foreach (OperationClass operation in Operations)
            {
                var newOperationClass = new OperationClass(operation);
                patientInfo.Operations.Add(newOperationClass);
            }

            patientInfo.TransferEpicrisAfterOperationPeriod = TransferEpicrisAfterOperationPeriod;
            patientInfo.TransferEpicrisPlan = TransferEpicrisPlan;
            patientInfo.TransferEpicrisWritingDate = ConvertEngine.CopyDateTime(TransferEpicrisWritingDate);
            patientInfo.TransferEpicrisAdditionalInfo = TransferEpicrisAdditionalInfo;
            patientInfo.TransferEpicrisDisabilityList = TransferEpicrisDisabilityList;
            patientInfo.TransferEpicrisIsIncludeDisabilityList = TransferEpicrisIsIncludeDisabilityList;

            patientInfo.LineOfCommEpicrisAdditionalInfo = LineOfCommEpicrisAdditionalInfo;
            patientInfo.LineOfCommEpicrisPlan = LineOfCommEpicrisPlan;
            patientInfo.LineOfCommEpicrisWritingDate = ConvertEngine.CopyDateTime(LineOfCommEpicrisWritingDate);

            patientInfo.DischargeEpicrisAnalysisDate = DischargeEpicrisAnalysisDate;
            patientInfo.DischargeEpicrisAfterOperation = DischargeEpicrisAfterOperation;
            patientInfo.DischargeEpicrisEkg = DischargeEpicrisEkg;
            patientInfo.DischargeEpicrisConsultation = DischargeEpicrisConsultation;
            patientInfo.DischargeEpicrisOakEritrocits = DischargeEpicrisOakEritrocits;
            patientInfo.DischargeEpicrisOakHb = DischargeEpicrisOakHb;
            patientInfo.DischargeEpicrisOakLekocits = DischargeEpicrisOakLekocits;
            patientInfo.DischargeEpicrisOakSoe = DischargeEpicrisOakSoe;
            patientInfo.DischargeEpicrisOamColor = DischargeEpicrisOamColor;
            patientInfo.DischargeEpicrisOamDensity = DischargeEpicrisOamDensity;
            patientInfo.DischargeEpicrisOamEritrocits = DischargeEpicrisOamEritrocits;
            patientInfo.DischargeEpicrisOamLekocits = DischargeEpicrisOamLekocits;
            patientInfo.DischargeEpicrisBakBillirubin = DischargeEpicrisBakBillirubin;
            patientInfo.DischargeEpicrisBakGeneralProtein = DischargeEpicrisBakGeneralProtein;
            patientInfo.DischargeEpicrisBakPTI = DischargeEpicrisBakPTI;
            patientInfo.DischargeEpicrisBakSugar = DischargeEpicrisBakSugar;
            patientInfo.DischargeEpicrisBloodGroup = DischargeEpicrisBloodGroup;
            patientInfo.DischargeEpicrisRhesusFactor = DischargeEpicrisRhesusFactor;

            patientInfo.DischargeEpicrisAdditionalAnalises = DischargeEpicrisAdditionalAnalises;

            patientInfo.DischargeEpicrisRecomendations = new List<string>(DischargeEpicrisRecomendations);

            patientInfo.DischargeEpicrisAdditionalRecomendations = new List<string>(DischargeEpicrisAdditionalRecomendations);

            patientInfo.PrescriptionTherapy = new List<string>(PrescriptionTherapy);
            patientInfo.PrescriptionSurveys = new List<string>(PrescriptionSurveys);

            patientInfo.MedicalInspectionAnamneseAnMorbi = MedicalInspectionAnamneseAnMorbi;
            patientInfo.MedicalInspectionAnamneseAnVitae = CopyBoolArray(MedicalInspectionAnamneseAnVitae);
            patientInfo.MedicalInspectionAnamneseCheckboxes = CopyBoolArray(MedicalInspectionAnamneseCheckboxes);
            patientInfo.MedicalInspectionAnamneseTextBoxes = CopyStringArray(MedicalInspectionAnamneseTextBoxes);
            patientInfo.MedicalInspectionComplaints = MedicalInspectionComplaints;
            patientInfo.MedicalInspectionExpertAnamnese = MedicalInspectionExpertAnamnese;
            patientInfo.MedicalInspectionStLocalisDescription = MedicalInspectionStLocalisDescription;
            patientInfo.MedicalInspectionStLocalisRentgen = MedicalInspectionStLocalisRentgen;
            patientInfo.MedicalInspectionInspectionPlan = MedicalInspectionInspectionPlan;
            patientInfo.MedicalInspectionTreatmentType = MedicalInspectionTreatmentType;
            patientInfo.MedicalInspectionIsAnamneseActive = MedicalInspectionIsAnamneseActive;
            patientInfo.MedicalInspectionIsPlanEnabled = MedicalInspectionIsPlanEnabled;
            patientInfo.MedicalInspectionWithBoss = MedicalInspectionWithBoss;
            patientInfo.MedicalInspectionIsStLocalisPart1Enabled = MedicalInspectionIsStLocalisPart1Enabled;
            patientInfo.MedicalInspectionLnFirstDateStart = ConvertEngine.CopyDateTime(MedicalInspectionLnFirstDateStart);
            patientInfo.MedicalInspectionLnWithNumberDateEnd = ConvertEngine.CopyDateTime(MedicalInspectionLnWithNumberDateEnd);
            patientInfo.MedicalInspectionLnWithNumberDateStart = ConvertEngine.CopyDateTime(MedicalInspectionLnWithNumberDateStart);
            patientInfo.MedicalInspectionStLocalisPart1Fields = CopyStringArray(MedicalInspectionStLocalisPart1Fields);
            patientInfo.MedicalInspectionStLocalisPart1OppositionFinger = MedicalInspectionStLocalisPart1OppositionFinger;
            patientInfo.MedicalInspectionStPraesensComboBoxes = CopyStringArray(MedicalInspectionStPraesensComboBoxes);
            patientInfo.MedicalInspectionStPraesensNumericUpDowns = CopyIntArray(MedicalInspectionStPraesensNumericUpDowns);
            patientInfo.MedicalInspectionStPraesensOthers = MedicalInspectionStPraesensOthers;
            patientInfo.MedicalInspectionStPraesensTemperature = MedicalInspectionStPraesensTemperature;
            patientInfo.MedicalInspectionStPraesensTextBoxes = CopyStringArray(MedicalInspectionStPraesensTextBoxes);
            patientInfo.MedicalInspectionTeoRisk = MedicalInspectionTeoRisk;
            patientInfo.MedicalInspectionTeoRiskEnabled = MedicalInspectionTeoRiskEnabled;
        } 

        private static string[] CopyStringArray(string[] fromObj)
        {
            var temp = new string[fromObj.Length];
            fromObj.CopyTo(temp, 0);
            return temp;
        }

        private static int[] CopyIntArray(int[] fromObj)
        {
            var temp = new int[fromObj.Length];
            fromObj.CopyTo(temp, 0);
            return temp;
        }

        private static bool[] CopyBoolArray(bool[] fromObj)
        {
            var temp = new bool[fromObj.Length];
            fromObj.CopyTo(temp, 0);
            return temp;
        }

        /// <summary>
        /// Сгенерировать новый ID для операции
        /// </summary>
        /// <returns></returns>
        private int GetNewOperationID()
        {
            int max = 0;
            foreach (OperationClass operationInfo in Operations)
            {
                if (operationInfo.Id > max)
                {
                    max = operationInfo.Id;
                }
            }

            return max + 1;
        }

        /// <summary>
        /// Добавить новую операцию в список операций
        /// </summary>
        /// <param name="operationInfo">Информация про операцию</param>
        public void AddOperation(OperationClass operationInfo)
        {
            operationInfo.Id = GetNewOperationID();
            Operations.Add(operationInfo);
            Operations.Sort(OperationClass.Compare);
        }

        /// <summary>
        /// Изменение данных по операции
        /// </summary>
        /// <param name="operationInfo">Информация про операцию</param>
        public void UpdateOperation(OperationClass operationInfo)
        {
            int n = 0;
            while (Operations[n].Id != operationInfo.Id)
            {
                n++;
            }

            operationInfo.Copy(Operations[n]);
            Operations.Sort(OperationClass.Compare);
        }

        /// <summary>
        /// Удаление операции с указанным id
        /// </summary>
        /// <param name="operationId">ID операции</param>
        public void DeleteOperation(int operationId)
        {
            int n = 0;
            while (Operations[n].Id != operationId)
            {
                n++;
            }

            Operations.RemoveAt(n);
        }

        public static int CompareByName(PatientClass patientInfo1, PatientClass patientInfo2)
        {
            return string.Compare(patientInfo1.GetFullName(), patientInfo2.GetFullName());
        }

        public static int CompareByDeliveryDate(PatientClass patientInfo1, PatientClass patientInfo2)
        {
            return DateTime.Compare(patientInfo1.DeliveryDate, patientInfo2.DeliveryDate);
        }

        public static int CompareByReleaseDate(PatientClass patientInfo1, PatientClass patientInfo2)
        {
            if (patientInfo1.ReleaseDate.HasValue && patientInfo2.ReleaseDate.HasValue)
            {
                return DateTime.Compare(patientInfo1.ReleaseDate.Value, patientInfo2.ReleaseDate.Value);
            }

            if (patientInfo1.ReleaseDate.HasValue)
            {
                return 1;
            }

            if (patientInfo2.ReleaseDate.HasValue)
            {
                return -1;
            }

            return CompareByName(patientInfo1, patientInfo2);
        }

        public static int CompareByOperationDate(PatientClass patientInfo1, PatientClass patientInfo2)
        {
            int operationLastIndex1 = patientInfo1.Operations.Count - 1;
            int operationLastIndex2 = patientInfo2.Operations.Count - 1;
            if (operationLastIndex1 > -1 && operationLastIndex2 > -1)
            {
                return DateTime.Compare(
                    patientInfo1.Operations[operationLastIndex1].DataOfOperation, 
                    patientInfo2.Operations[operationLastIndex2].DataOfOperation);
            }

            if (operationLastIndex1 > -1)
            {
                return 1;
            }

            if (operationLastIndex2 > -1)
            {
                return -1;
            }

            return CompareByName(patientInfo1, patientInfo2);
        }
    }
}
