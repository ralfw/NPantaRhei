using System;

namespace npantarhei.runtime.contract
{
	/*
	 * Examples:
	 * 		op		: operation with default ports
	 * 		a/b/op	: operation with a path
	 * 		op.p	: operation with an explicit port
	 * 		.p
	 *		a/b/.p	: port without an operation
	 * 
	 */
	public interface IPort
	{
		string Fullname {get;}
		string Path {get;}
		string OperationName {get;}
		string Name{get;}
		
		bool HasOperation {get;}
	    bool IsQualified {get;}
	}
}

