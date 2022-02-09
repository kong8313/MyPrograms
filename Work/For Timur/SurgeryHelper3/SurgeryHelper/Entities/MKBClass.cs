namespace SurgeryHelper.Entities
{
    public class MkbClass
    {
        /// <summary>
        /// Код МКБ
        /// </summary>
        public string MkbCode { get; set; }

        /// <summary>
        /// Название МКБ
        /// </summary>
        public string MkbName { get; set; }

        /// <summary>
        /// Код КСГ
        /// </summary>
        public string KsgCode { get; set; }

        /// <summary>
        /// Расшифровка КСГ
        /// </summary>
        public string KsgName { get; set; }
        
        public MkbClass()
        {
            MkbCode = MkbName = KsgCode = KsgName = string.Empty;
        }
    }
}
