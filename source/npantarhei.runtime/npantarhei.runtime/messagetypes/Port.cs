using System;
using System.IO;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Port : IPort
	{
		public Port (string fullname)
		{
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
		
		
		public bool HasOperation {
			get { return this.OperationName != ""; }
		}

	    public bool IsQualified {
            get { return this.Path.StartsWith("/"); }
	    }
		#endregion


        public override string ToString()
        {
            return string.Format("Port(Fullname='{0}')", this.Fullname);
        }
	}
}

