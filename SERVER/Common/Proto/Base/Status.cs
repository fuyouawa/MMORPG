// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Base/Status.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Common.Proto.Base {

  /// <summary>Holder for reflection information generated from Base/Status.proto</summary>
  public static partial class StatusReflection {

    #region Descriptor
    /// <summary>File descriptor for Base/Status.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static StatusReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFCYXNlL1N0YXR1cy5wcm90bxIRQ29tbW9uLlByb3RvLkJhc2UqGwoGU3Rh",
            "dHVzEgYKAk9LEAASCQoFRVJST1IQAWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Common.Proto.Base.Status), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum Status {
    [pbr::OriginalName("OK")] Ok = 0,
    [pbr::OriginalName("ERROR")] Error = 1,
  }

  #endregion

}

#endregion Designer generated code