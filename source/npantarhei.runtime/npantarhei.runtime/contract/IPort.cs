using System;

namespace npantarhei.runtime.contract
{
	public enum PortProcessingModes
	{
		Synchronous,
		Sequential,
		Parallel
	}
	
	/*
	 * Examples:
	 * 		op		: operation only port (synchronous processing)
	 * 		a/b/op	: operation with a path
	 * 		op.p	: operation with a port
	 * 		.p
	 *		a/b/.p	: port without an operation
	 * 		op*
	 * 		op.p*	: async sequential processing
	 * 		op**
	 * 		op.p**	: parallel processing
	 * 
	 */
	public interface IPort
	{
		string Fullname {get;}
		string Path {get;}
		string OperationName {get;}
		string Name{get;}
		PortProcessingModes ProcessingMode {get;}
		
		bool IsOperationPort {get;}
	}
}

