using System;
using System.IO;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Port : IPort
	{
		private PortProcessingModes _processingMode;
		
		public Port (string fullname)
		{
			if (fullname.EndsWith("**"))
				_processingMode = PortProcessingModes.Parallel;
			else if (fullname.EndsWith("*"))
				_processingMode = PortProcessingModes.Sequential;
			else
				_processingMode = PortProcessingModes.Synchronous;
			
			this.Fullname = fullname.Replace("\\", "/").Replace("*", "");
		}

		#region IPort implementation
		public string Fullname { get; private set; }
			
		public string Path {
			get {
				return System.IO.Path.GetDirectoryName(this.Fullname).Replace("\\", "/");
                // On Windows GetDirectoryName() seems to return "\" even though Fullname uses
                // just "/". An extra Replace() thus is necessary here.
			}
		}

		public string OperationName {
			get {
				return System.IO.Path.GetFileNameWithoutExtension(this.Fullname);
			}
		}

		public string Name {
			get {
				return System.IO.Path.GetExtension(this.Fullname).Replace(".", "");
			}
		}
		
		public PortProcessingModes ProcessingMode {
			get {
				return _processingMode;
			}
		}
		
		
		public bool IsOperationPort {
			get {
				return this.OperationName != "";
			}
		}
		#endregion


        public override string ToString()
        {
            return string.Format("Port(Fullname='{0}')", this.Fullname);
        }
	}
}

