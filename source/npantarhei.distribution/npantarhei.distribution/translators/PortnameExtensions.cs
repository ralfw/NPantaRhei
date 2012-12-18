using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.distribution.translators
{
    static class PortnameExtensions
    {
        // stand-in portname: remoteOp.someport
        // input portname: .someport@remoteOp
        public static string StandInPortnameToInputPortname(this string standInPortname)
        {
            var port = new Port(standInPortname);
            return string.Format(".{0}@{1}", port.Name, port.OperationName);
        }

        public static string OutputPortToStandInPortname(this IPort port)
        {
            var parts = port.Name.Split('@');
            return string.Format("{0}{1}", parts[1], parts[0] == "" ? "" : "."+parts[0]);
        }


        // stand-in operation: standInName#remoteOp.portname
        // output portname: remoteOp.portname
        public static string OutputPortToRemotePortname(this IPort port)
        {
            return string.Format("{0}{1}", port.InstanceNumber, port.Name == "" ? "" : "."+port.Name);
        }

        public static IPort RemotePortnameToInputPort(this string portname, string path, string standInOperationName)
        {
            var remotePort = new Port(portname);
            return new Port(string.Format("{0}{1}#{2}{3}", path == "" ? "" : path + "/",
                                                           standInOperationName, 
                                                           remotePort.OperationName, 
                                                           remotePort.Name == "" ? "" : "."+remotePort.Name));
        }
    }
}