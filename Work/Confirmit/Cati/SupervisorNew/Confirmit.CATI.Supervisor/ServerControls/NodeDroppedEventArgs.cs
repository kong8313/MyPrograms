using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Event arguments for Infragistics web tree double click event.
    /// </summary>
    public class NodeDoubleClickEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDoubleClickEventArgs"/> class.
        /// </summary>
        /// <param name="dataPath">The data path.</param>
        /// <param name="dataKey">The data key.</param>
        public NodeDoubleClickEventArgs(string dataPath, string dataKey)
        {
            DataPath = dataPath;
            DataKey = dataKey;
        }

        /// <summary>
        /// Gets or sets the data path of a double clicked tree node.
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// Gets or sets the data key of a double clicked tree node..
        /// </summary>
        public string DataKey { get; set; }
    }
}