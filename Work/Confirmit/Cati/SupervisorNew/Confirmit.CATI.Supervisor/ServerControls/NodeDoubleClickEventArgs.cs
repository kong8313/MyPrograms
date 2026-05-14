using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Event arguments for Infragistics web tree double click event.
    /// </summary>
    public class NodeDroppedEventArgs : EventArgs
    {
        
        public NodeDroppedEventArgs(string nodeDataPath, 
                                    string nodeDataKey)
        {
            NodeDataPath = nodeDataPath;
            NodeDataKey= nodeDataKey;
        }

        /// <summary>
        /// Gets or sets the data key of a double clicked tree node..
        /// </summary>
        public string NodeDataKey { get; set; }

        public string NodeDataPath { get; set; }
    }
}