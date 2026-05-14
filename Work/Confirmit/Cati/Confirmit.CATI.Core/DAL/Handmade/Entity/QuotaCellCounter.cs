namespace Confirmit.CATI.Core.DAL.Handmade.Entity
{
    public class QuotaCellCounter
    {
        /// <summary>
        /// List of precodes separated by comma.
        /// </summary>
        public string Descriptor{ get; set;}

        /// <summary>
        ///  Value of counter for corresponding cell.
        /// </summary>
        public int Value{ get; set;}
    }
}