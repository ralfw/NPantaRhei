using System;
using System.IO;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Port : IPort
	{
		public static Port Build(string operationname, string portname) { return Build("", operationname, "", portname); }
		public static Port Build(string path, string operationname, string portname) { return Build(path, operationname, "", portname); }
		public static Port Build(string path, string operationname, string instanceNumber, string portname)
		{
			if (path != "" && !path.EndsWith("/")) path += "/";
			if (portname != "" && !portname.StartsWith(".")) portname = "." + portname;
			if (instanceNumber != "" && !instanceNumber.StartsWith("#")) instanceNumber = "#" + instanceNumber;
			var portFullname = path + operationname + instanceNumber + portname;
			return new Port(portFullname);
		}

        // return f/a if /f/a
		public static IPort Uproot(IPort port)
		{
		    return new Port(port.Fullname.Substring(1));
		}


	    public Port (string fullname)
		{
			this.Fullname = fullname.Replace("\\", "/").Replace("*", "");
		}

		#region IPort implementation
		public string Fullname { get; private set; }

		// a/b/c of a/b/c/op#42.port
		public string Path {
			get {
				return System.IO.Path.GetDirectoryName(this.Fullname).Replace("\\", "/");
				// On Windows GetDirectoryName() seems to return "\" even though Fullname uses
				// just "/". An extra Replace() thus is necessary here.
			}
		}

		// op#42 of a/b/c/op#42.port
		public string InstanceName { get { return System.IO.Path.GetFileNameWithoutExtension(this.Fullname); } }

		// 42 of a/b/c/op#42.port
		public string InstanceNumber { get
		{
			if (this.InstanceName.IndexOf("#") < 0) return "";
			return this.InstanceName.Substring(this.InstanceName.IndexOf("#")+1);
		} }

		// op of a/b/c/op#42.port
		public string OperationName {
			get {
				if (this.InstanceName.IndexOf("#") < 0) return this.InstanceName;
				return this.InstanceName.Substring(0, this.InstanceName.IndexOf("#"));
			}
		}

		// port of a/b/c/op#42.port
		public string Name {
			get {
				return System.IO.Path.GetExtension(this.Fullname).Replace(".", "");
			}
		}
		
		// true for a/b/c/op#42.port, false for a/b/c/.port
		public bool HasOperation {
			get { return this.OperationName != ""; }
		}

		// true for /a/b/c/op#42.port, false for a/b/c/op#42.port
		public bool IsRooted {
			get { return this.Path.StartsWith("/"); }
		}
		#endregion


		public override string ToString()
		{
			return string.Format("Port(Fullname='{0}')", this.Fullname);
		}
	}
}

